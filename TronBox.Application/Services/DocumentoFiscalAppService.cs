using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using CTe.Classes;
using DFe.Utils;
using Newtonsoft.Json;
using NFe.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.EnvioDeArquivo.Interface;
using TronCore.Utilitarios.Specifications;
using ZionDanfe;
using ZionDanfe.Modelo;

namespace TronBox.Application.Services
{
    public class DocumentoFiscalAppService : IDocumentoFiscalAppService
    {
        public static string path = "documentosfiscais/{tipo}/{anomes}";

        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IAzureBlobStorage _azureBlobStorage;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public DocumentoFiscalAppService(IBus bus, IMapper mapper, IAzureBlobStorage azureBlobStorage, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _azureBlobStorage = azureBlobStorage;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public async Task<byte[]> DownloadDanfe(string chaveDocumentoFiscal)
        {
            var modelo = await BuscarNfeViewModel(chaveDocumentoFiscal);

            if (modelo != null)
            {
                using (var danfe = new Danfe(modelo))
                {
                    danfe.Gerar();

                    MemoryStream pdfContent = new MemoryStream();

                    danfe.Salvar(pdfContent);

                    return pdfContent.ToArray();
                }
            }

            return null;
        }

        public void Dispose()
        {
        }

        public void Atualizar(DocumentoFiscalDTO documentoFiscalDTO)
        {
            var documentoFiscal = _mapper.Map<DocumentoFiscal>(documentoFiscalDTO);

            if (EhValido(documentoFiscal)) _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Atualizar(documentoFiscal);
        }

        public async Task<DetalhesDocumentoFiscalDTO> BuscarPorId(Guid id)
        {
            var documentoFiscal = _mapper.Map<DocumentoFiscalDTO>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarPorId(id));

            if (documentoFiscal == null) return null;

            var conteudoXML = await BuscarConteudoXML(documentoFiscal.ChaveDocumentoFiscal);

            var detalhesDocumento = new DetalhesDocumentoFiscalDTO()
            {
                DataArmazenamento = documentoFiscal.DataArmazenamento,
                Cancelado = documentoFiscal.Cancelado,
                Denegada = documentoFiscal.Denegada,
                Rejeitado = documentoFiscal.Rejeitado,
                DadosOrigem = documentoFiscal.DadosOrigem,
                DadosImportacao = documentoFiscal.DadosImportacao
            };

            if ((documentoFiscal.TipoDocumentoFiscal == TipoDocumentoFiscal.NfeEntrada) || (documentoFiscal.TipoDocumentoFiscal == TipoDocumentoFiscal.NfeSaida))
                detalhesDocumento.NotaFiscalEletronica = FuncoesXml.XmlStringParaClasse<nfeProc>(conteudoXML);
            else if ((documentoFiscal.TipoDocumentoFiscal == TipoDocumentoFiscal.CteEntrada) || (documentoFiscal.TipoDocumentoFiscal == TipoDocumentoFiscal.CteSaida))
                detalhesDocumento.ConhecimentoTransporteEletronico = FuncoesXml.XmlStringParaClasse<cteProc>(conteudoXML);

            return detalhesDocumento;
        }

        public IEnumerable<DocumentoFiscalDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<DocumentoFiscalDTO>>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>()
            .BuscarTodos(new UtilitarioSpecification<DocumentoFiscal>().CriarEspecificacaoFiltro(filtro)));

        public async Task<IEnumerable<string>> Inserir(EnviarArquivosDTO arquivos)
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());

            var documentosGerados = await ObterDocumentosMicroservico(arquivos, empresa);

            var documentosValidos = DocumentosValidos(documentosGerados.DocumentosProcessados);
            NotificarDocumentoInvalidos(documentosGerados.DocumentosNaoProcessados);

            if (documentosValidos.Count > 0)
                _repositoryFactory.Instancie<IDocumentoFiscalRepository>().InserirTodos(documentosValidos);

            return documentosValidos.Select(c => c.ChaveDocumentoFiscal);
        }

        #region Private Methods
        private List<DocumentoFiscal> DocumentosValidos(List<DocumentoFiscalDTO> documentosGerados)
        {
            var documentosFiscais = new List<DocumentoFiscal>();

            var chavesGeradas = documentosGerados.Select(c => c.ChaveDocumentoFiscal);

            var documentosExistentes = _repositoryFactory.Instancie<IDocumentoFiscalRepository>()
                .BuscarTodos(d => chavesGeradas.Contains(d.ChaveDocumentoFiscal));

            foreach (var documentoGerado in documentosGerados)
            {
                var documentoFiscal = _mapper.Map<DocumentoFiscal>(documentoGerado);

                if (documentosExistentes.Any(d => d.ChaveDocumentoFiscal == documentoFiscal.ChaveDocumentoFiscal))
                {
                    _bus.RaiseEvent(new DomainNotification(documentoFiscal.ChaveDocumentoFiscal, "Documento já existente na base de dados."));
                    continue;
                }

                if (EhValido(documentoFiscal)) documentosFiscais.Add(documentoFiscal);
            }

            return documentosFiscais;
        }

        private void NotificarDocumentoInvalidos(List<DocumentoFiscalNaoGeradoDTO> documentosNaoProcessados)
        {
            foreach (var documentoNaoProcessado in documentosNaoProcessados)
                _bus.RaiseEvent(new DomainNotification(documentoNaoProcessado.ChaveDocumentoFiscal, documentoNaoProcessado.Mensagem));
        }

        private bool EhValido(DocumentoFiscal documentoFiscal)
        {
            var validator = new DocumentoFiscalValidator().Validate(documentoFiscal);

            if (validator.Errors.Any())
            {
                var erros = validator.Errors.Select(c => new
                {
                    Chave = c.PropertyName,
                    Mensagem = c.ErrorMessage
                });

                _bus.RaiseEvent(new DomainNotification(documentoFiscal.ChaveDocumentoFiscal, erros));
            }

            return validator.IsValid;
        }

        private async Task<DocumentosGeradosDTO> ObterDocumentosMicroservico(EnviarArquivosDTO arquivos, EmpresaDTO empresa)
        {
            var dictionary = new Dictionary<string, dynamic>
            {
                { "empresaSelecionada", empresa.Inscricao },
                { "origem", ((int) arquivos.Origem).ToString() },
                { "originador", arquivos.Originador },
                { "arquivos", arquivos.Arquivos }
            };

            var result = await UtilitarioHttpClient.PostRequest(string.Empty, "http://localhost:3000", "api/uploads", dictionary, "documento.xml");

            return JsonConvert.DeserializeObject<DocumentosGeradosDTO>(result);
        }

        private async Task<string> BuscarConteudoXML(string chaveDocumentoFiscal)
        {
            var tipoDocumento = chaveDocumentoFiscal.Substring(20, 2) == "55" ? "nfe" : "cte";

            var folderName = path.Replace("{tipo}", tipoDocumento).Replace("{anomes}", chaveDocumentoFiscal.Substring(2, 4));

            var arquivoExiste = await _azureBlobStorage.ExistsAsync(chaveDocumentoFiscal, folderName);

            if (arquivoExiste)
            {
                var anexoDownloaded = await _azureBlobStorage.DownloadAsync(chaveDocumentoFiscal, folderName);

                using (MemoryStream originalFileMemoryStream = new MemoryStream(anexoDownloaded.ToArray()))
                {
                    using (StreamReader reader = new StreamReader(originalFileMemoryStream))
                        return reader.ReadToEnd();
                }
            }

            return string.Empty;
        }

        private async Task<DanfeViewModel> BuscarNfeViewModel(string chaveDocumentoFiscal)
        {
            var conteudoXML = await BuscarConteudoXML(chaveDocumentoFiscal);

            return DanfeViewModelCreator.CriarDeStringXml(conteudoXML);
        }
        #endregion
    }
}