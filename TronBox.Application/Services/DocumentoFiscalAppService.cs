using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using CTe.Classes;
using DFe.Classes.Flags;
using DFe.Utils;
using Microsoft.AspNetCore.Http;
using NFe.Classes;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Detalhe;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.DTO;
using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.Enums;
using TronCore.DefinicoesConfiguracoes;
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

        public async Task<byte[]> DownloadPDF(Guid id)
        {
            var documentoFiscal = _mapper.Map<DocumentoFiscalDTO>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarPorId(id));

            if (documentoFiscal == null) return null;

            var conteudoXML = await BuscarConteudoXML(documentoFiscal.ChaveDocumentoFiscal);

            var documentoToDownload = new
            {
                docType = (int)documentoFiscal.TipoDocumentoFiscal,
                key = documentoFiscal.ChaveDocumentoFiscal,
                attachment = conteudoXML
            };

            return await UtilitarioHttpClient.PostRequest(Constantes.URI_BASE_PDF, "api/invoices/download", documentoToDownload);
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

            if ((documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfeEntrada) || (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfeSaida) || (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.Nfce))
                detalhesDocumento.NotaFiscalEletronica = FuncoesXml.XmlStringParaClasse<nfeProc>(conteudoXML);
            else if ((documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.CteEntrada) || (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.CteSaida))
                detalhesDocumento.ConhecimentoTransporteEletronico = FuncoesXml.XmlStringParaClasse<cteProc>(conteudoXML);

            return detalhesDocumento;
        }

        public IEnumerable<DocumentoFiscalDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<DocumentoFiscalDTO>>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>()
            .BuscarTodos(new UtilitarioSpecification<DocumentoFiscal>().CriarEspecificacaoFiltro(filtro)));

        public async Task<IEnumerable<string>> Inserir(EnviarArquivosDTO arquivos)
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());

            var documentosFiscais = await ProcessarArquivosEnviados(arquivos, empresa);

            var documentosValidos = new List<DocumentoFiscal>();
            var documentosCancelamento = new List<DocumentoFiscal>();

            DocumentosValidos(documentosFiscais, ref documentosValidos, ref documentosCancelamento);

            if (documentosValidos.Count > 0)
                _repositoryFactory.Instancie<IDocumentoFiscalRepository>().InserirTodos(documentosValidos);

            if (documentosCancelamento.Count > 0)
                _repositoryFactory.Instancie<IDocumentoFiscalRepository>().AtualizarTodos(documentosCancelamento);

            return documentosValidos.Select(c => c.NomeArquivo).Concat(documentosCancelamento.Select(c => c.NomeArquivo));
        }

        public void Deletar(Guid id) => _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Excluir(id);

        #region Private Methods
        private async Task<List<DocumentoFiscalDTO>> ProcessarArquivosEnviados(EnviarArquivosDTO arquivos, EmpresaDTO empresa)
        {
            var documentosFiscais = new List<DocumentoFiscalDTO>();

            var dadosOrigem = new DadosOrigemDocumentoFiscalDTO()
            {
                Origem = arquivos.Origem,
                Originador = arquivos.Originador
            };

            foreach (var arquivo in arquivos.Arquivos)
            {
                using (StreamReader reader = new StreamReader(arquivo.OpenReadStream()))
                {
                    var conteudoXML = reader.ReadToEnd();

                    try
                    {
                        if (Regex.IsMatch(conteudoXML, "<descEvento>Cancelamento</descEvento>", RegexOptions.IgnoreCase))
                        {
                            var chave = Regex.Match(conteudoXML, "(?<=>)([0-9]{44})(?=<)").Value;

                            documentosFiscais.Add(new DocumentoFiscalDTO(true, chave));
                        }
                        else if (Regex.IsMatch(conteudoXML, "<chNFe>(.*?)</chNFe>", RegexOptions.IgnoreCase))
                        {
                            var notaFiscal = ProcessarXMLparaNFe(empresa.Inscricao, conteudoXML, arquivo.FileName);

                            if (notaFiscal != null)
                                documentosFiscais.Add(await PrepararDocumentoFiscal(dadosOrigem, arquivo, notaFiscal));
                        }
                        else if (Regex.IsMatch(conteudoXML, "<chCTe>(.*?)</chCTe>", RegexOptions.IgnoreCase))
                        {
                            var conhecimentoTransporte = ProcessarXMLparaCTe(empresa.Inscricao, conteudoXML, arquivo.FileName);

                            if (conhecimentoTransporte != null)
                                documentosFiscais.Add(await PrepararDocumentoFiscal(dadosOrigem, arquivo, conhecimentoTransporte));
                        }
                        else if (Regex.IsMatch(conteudoXML, "<CompNfse[^>]*?>(.*?)</CompNfse>", RegexOptions.IgnoreCase))
                        {
                            // TODO IMPORTAÇÃO NFSE
                        }
                    }
                    catch (Exception)
                    {
                        NotificarDocumentoInvalidos(arquivo.FileName, "Documento não suportado.");
                        continue;
                    }
                }
            }

            return documentosFiscais;
        }

        private async Task<DocumentoFiscalDTO> PrepararDocumentoFiscal(DadosOrigemDocumentoFiscalDTO dadosOrigem, IFormFile arquivo, DocumentoFiscalDTO documentoFiscal)
        {
            documentoFiscal.DadosOrigem = dadosOrigem;
            documentoFiscal.NomeArquivo = arquivo.FileName;

            if (!documentoFiscal.Cancelado)
                await UploadFileToBlobStorage(arquivo, documentoFiscal);

            return documentoFiscal;
        }

        private async Task UploadFileToBlobStorage(IFormFile arquivo, DocumentoFiscalDTO documentoFiscal)
        {
            string folderName = ObterFolderNameFromKey(documentoFiscal.ChaveDocumentoFiscal);

            await _azureBlobStorage.UploadAsync(documentoFiscal.ChaveDocumentoFiscal, folderName, arquivo.OpenReadStream());
        }

        private DocumentoFiscalDTO ProcessarXMLparaCTe(string inscricaoEmpresa, string conteudoXML, string nomeArquivo)
        {
            var cte = FuncoesXml.XmlStringParaClasse<cteProc>(conteudoXML);

            var conhecimentoTransporte = ObterConhecimentoTransporteFromObject(inscricaoEmpresa, cte);

            if (conhecimentoTransporte == null)
            {
                NotificarDocumentoInvalidos(nomeArquivo, "Documento não pertence a empresa selecionada.");
                return null;
            }

            return conhecimentoTransporte;
        }

        private DocumentoFiscalDTO ProcessarXMLparaNFe(string inscricaoEmpresa, string conteudoXML, string nomeArquivo)
        {
            var nfe = FuncoesXml.XmlStringParaClasse<nfeProc>(conteudoXML);

            var notaFiscal = ObterNotaFiscalFromObject(inscricaoEmpresa, nfe);

            if (notaFiscal == null)
            {
                NotificarDocumentoInvalidos(nomeArquivo, "Documento não pertence a empresa selecionada.");
                return null;
            }

            return notaFiscal;
        }

        private DocumentoFiscalDTO ObterConhecimentoTransporteFromObject(string inscricaoEmpresa, cteProc cte)
        {
            var documentoFiscal = new DocumentoFiscalDTO()
            {
                DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                DataEmissaoDocumento = UtilitarioDatas.ConvertToIntDate(cte.CTe.infCte.ide.dhEmi.UtcDateTime),
                ChaveDocumentoFiscal = cte.protCTe.infProt.chCTe,
                SerieDocumentoFiscal = cte.CTe.infCte.ide.serie.ToString(),
                NumeroDocumentoFiscal = cte.CTe.infCte.ide.nCT.ToString(),
                ValorDocumentoFiscal = (double)cte.CTe.infCte.vPrest.vTPrest,
                TipoDocumentoFiscal = inscricaoEmpresa == cte.CTe.infCte.emit.CNPJ
                    ? ETipoDocumentoFiscal.CteSaida
                    : ObterTipoConhecimentoTransporte(inscricaoEmpresa, cte),
                DadosEmitenteDestinatario = new DadosFornecedorDTO()
                {
                    Inscricao = cte.CTe.infCte.emit.CNPJ,
                    RazaoSocial = cte.CTe.infCte.emit.xNome
                }
            };

            if (documentoFiscal.TipoDocumentoFiscal == 0) return null;

            return documentoFiscal;
        }

        private DocumentoFiscalDTO ObterNotaFiscalFromObject(string inscricaoEmpresa, nfeProc nfe)
        {
            var documentoFiscal = new DocumentoFiscalDTO()
            {
                DataArmazenamento = UtilitarioDatas.ConvertToIntDate(DateTime.Now),
                DataEmissaoDocumento = UtilitarioDatas.ConvertToIntDate(nfe.NFe.infNFe.ide.dhEmi.UtcDateTime),
                ChaveDocumentoFiscal = nfe.protNFe.infProt.chNFe,
                SerieDocumentoFiscal = nfe.NFe.infNFe.ide.serie.ToString(),
                NumeroDocumentoFiscal = nfe.NFe.infNFe.ide.nNF.ToString(),
                ValorDocumentoFiscal = (double)nfe.NFe.infNFe.total.ICMSTot.vNF,
                TipoDocumentoFiscal = nfe.NFe.infNFe.ide.mod == ModeloDocumento.NFCe
                    ? ETipoDocumentoFiscal.Nfce
                    : ObterTipoNotaFiscal(inscricaoEmpresa, nfe.NFe.infNFe.ide.tpNF, nfe.NFe.infNFe.emit, nfe.NFe.infNFe.dest, nfe.NFe.infNFe.det.FirstOrDefault())
            };

            // TODO DOCUMENTO NÃO É DA EMPRESA
            if (documentoFiscal.TipoDocumentoFiscal == 0) return null;

            var inscricaoEmitente = nfe.NFe.infNFe.emit.CNPJ ?? nfe.NFe.infNFe.emit.CPF;
            var empresaNaoEmitente = inscricaoEmpresa != inscricaoEmitente;

            if ((documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.Nfce) || (empresaNaoEmitente))
            {
                documentoFiscal.DadosEmitenteDestinatario = new DadosFornecedorDTO()
                {
                    Inscricao = inscricaoEmitente,
                    RazaoSocial = nfe.NFe.infNFe.emit.xNome
                };
            }
            else
            {
                documentoFiscal.DadosEmitenteDestinatario = new DadosFornecedorDTO()
                {
                    Inscricao = nfe.NFe.infNFe.dest.CPF ?? nfe.NFe.infNFe.dest.CNPJ,
                    RazaoSocial = nfe.NFe.infNFe.dest.xNome
                };
            }

            return documentoFiscal;
        }

        private ETipoDocumentoFiscal ObterTipoConhecimentoTransporte(string inscricaoEmpresa, cteProc cte)
        {
            switch (cte.CTe.infCte.ide.tomaBase3.toma)
            {
                case CTe.Classes.Informacoes.Tipos.toma.Remetente:
                    return inscricaoEmpresa == (cte.CTe.infCte.rem.CNPJ ?? cte.CTe.infCte.rem.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CteSaida;
                case CTe.Classes.Informacoes.Tipos.toma.Expedidor:
                    return inscricaoEmpresa == (cte.CTe.infCte.exped.CNPJ ?? cte.CTe.infCte.exped.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CteSaida;
                case CTe.Classes.Informacoes.Tipos.toma.Recebedor:
                    return inscricaoEmpresa == (cte.CTe.infCte.receb.CNPJ ?? cte.CTe.infCte.receb.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CteSaida;
                case CTe.Classes.Informacoes.Tipos.toma.Destinatario:
                    return inscricaoEmpresa == (cte.CTe.infCte.dest.CNPJ ?? cte.CTe.infCte.dest.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CteSaida;
                case CTe.Classes.Informacoes.Tipos.toma.Outros:
                    return inscricaoEmpresa == (cte.CTe.infCte.ide.toma4.CNPJ ?? cte.CTe.infCte.ide.toma4.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CteSaida;
                default:
                    return 0;
            };
        }

        private ETipoDocumentoFiscal ObterTipoNotaFiscal(string inscricaoEmpresa, TipoNFe tpNF, emit emit, dest dest, det det)
        {
            var inscricaoEmitente = emit.CNPJ ?? emit.CPF;
            var inscricaoDestinatario = dest.CNPJ ?? dest.CPF;

            if (inscricaoEmpresa == inscricaoDestinatario) return tpNF == TipoNFe.tnEntrada ? ETipoDocumentoFiscal.NfeSaida : ETipoDocumentoFiscal.NfeEntrada;

            if (inscricaoEmpresa == inscricaoEmitente)
            {
                var CFOP = Convert.ToInt32(det.prod.CFOP.ToString().Substring(0, 1));

                if (CFOP <= 3) return ETipoDocumentoFiscal.NfeEntrada;

                return tpNF == TipoNFe.tnSaida ? ETipoDocumentoFiscal.NfeSaida : ETipoDocumentoFiscal.NfeEntrada;
            }

            return 0;
        }

        private void DocumentosValidos(List<DocumentoFiscalDTO> documentosGerados, ref List<DocumentoFiscal> documentosValidos, ref List<DocumentoFiscal> documentosCancelamento)
        {
            var chavesGeradas = documentosGerados.Select(c => c.ChaveDocumentoFiscal);

            var documentosExistentes = _repositoryFactory.Instancie<IDocumentoFiscalRepository>()
                .BuscarTodos(d => chavesGeradas.Contains(d.ChaveDocumentoFiscal)).ToList();

            foreach (var documentoGerado in documentosGerados)
            {
                if (documentosExistentes.Any(d => d.ChaveDocumentoFiscal == documentoGerado.ChaveDocumentoFiscal))
                {
                    if (documentoGerado.Cancelado)
                    {
                        var documentoParaCancelar = documentosExistentes.Where(d => d.ChaveDocumentoFiscal == documentoGerado.ChaveDocumentoFiscal).FirstOrDefault();

                        documentoParaCancelar.Cancelado = true;

                        documentosCancelamento.Add(documentoParaCancelar);
                    }
                    else _bus.RaiseEvent(new DomainNotification(documentoGerado.NomeArquivo, "Documento já existente na base de dados."));

                    continue;
                }

                if (documentoGerado.Cancelado && documentoGerado.NumeroDocumentoFiscal == null)
                {
                    _bus.RaiseEvent(new DomainNotification(documentoGerado.NomeArquivo, "Não foi encontrado documento para o cancelamento enviado."));
                    continue;
                }

                var documentoFiscal = _mapper.Map<DocumentoFiscal>(documentoGerado);

                if (EhValido(documentoFiscal))
                {
                    documentosExistentes.Add(documentoFiscal);
                    documentosValidos.Add(documentoFiscal);
                }
            }
        }

        private void NotificarDocumentoInvalidos(string nomeArquivo, string mensagem) => _bus.RaiseEvent(new DomainNotification(nomeArquivo, mensagem));

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

                _bus.RaiseEvent(new DomainNotification(documentoFiscal.NomeArquivo, erros));
            }

            return validator.IsValid;
        }

        private async Task<string> BuscarConteudoXML(string chaveDocumentoFiscal)
        {
            string folderName = ObterFolderNameFromKey(chaveDocumentoFiscal);

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

        private static string ObterFolderNameFromKey(string chaveDocumentoFiscal)
        {
            var tipoDocumento = GetTipoDocumento(chaveDocumentoFiscal);

            return path.Replace("{tipo}", tipoDocumento).Replace("{anomes}", chaveDocumentoFiscal.Substring(2, 4));
        }

        private static string GetTipoDocumento(string chaveDocumentoFiscal)
        {
            var modelo = chaveDocumentoFiscal.Substring(20, 2);

            if (modelo == "55") return "nfe";

            if (modelo == "65") return "nfce";

            return "cte";
        }
        #endregion
    }
}