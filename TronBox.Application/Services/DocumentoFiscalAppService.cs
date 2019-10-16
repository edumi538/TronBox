using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.EnvioDeArquivo.Interface;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class DocumentoFiscalAppService : IDocumentoFiscalAppService
    {
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

        public void Dispose()
        {
        }

        public void Atualizar(DocumentoFiscalDTO documentoFiscalDTO)
        {
            var documentoFiscal = _mapper.Map<DocumentoFiscal>(documentoFiscalDTO);

            if (EhValido(documentoFiscal)) _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Atualizar(documentoFiscal);
        }

        public DocumentoFiscalDTO BuscarPorId(Guid id) => _mapper.Map<DocumentoFiscalDTO>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarPorId(id));

        public IEnumerable<DocumentoFiscalDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<DocumentoFiscalDTO>>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>()
            .BuscarTodos(new UtilitarioSpecification<DocumentoFiscal>().CriarEspecificacaoFiltro(filtro)));

        public void Deletar(Guid id) => _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Excluir(id);

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

        // TODO MELHORAR
        private void NotificarDocumentoInvalidos(List<string> documentosNaoProcessados)
        {
            foreach (var documentoNaoProcessado in documentosNaoProcessados)
                _bus.RaiseEvent(new DomainNotification("CHAVE TAL", documentoNaoProcessado));
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
        #endregion
    }
}