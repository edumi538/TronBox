using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Comum.Domain.Enums;
using CTe.Classes;
using DFe.Classes.Flags;
using DFe.Utils;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using NFe.Classes;
using NFe.Classes.Informacoes.Destinatario;
using NFe.Classes.Informacoes.Detalhe;
using NFe.Classes.Informacoes.Emitente;
using NFe.Classes.Informacoes.Identificacao.Tipos;
using Sentinela.Domain.Interfaces;
using Sentry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ChaveDocumentoCanceladoAgg;
using TronBox.Domain.Aggregates.ChaveDocumentoCanceladoAgg.Repository;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.Aggregates.ManifestoAgg.Repository;
using TronBox.Domain.Classes.NFSe;
using TronBox.Domain.DTO;
using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.Enums;
using TronBox.Domain.InnerClass;
using TronBox.Infra.Data.Utilitarios;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Dominio.Base;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Dominio.Specifications;
using TronCore.InjecaoDependencia;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.Email;
using TronCore.Utilitarios.EnvioDeArquivo.Interface;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class DocumentoFiscalAppService : IDocumentoFiscalAppService
    {
        public static string path = "documentosfiscais/{tipo}/{anomes}";
        public static string URL_AGENTE_MANIFESTACAO_NFE = "http://10.20.30.28:8085";
        public static string URL_AGENTE_MANIFESTACAO_CTE = "http://10.20.30.28:8005";
        public static string URL_SCRAPER_SEFAZ_MT = "http://10.20.30.28:7002";
        public static string URL_SCRAPER_SEFAZ_MS = "http://10.20.30.28:7007";

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

            var conteudoXML = await BuscarConteudoXML(documentoFiscal);

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

        public async Task<DetalhesDocumentoFiscalDTO> BuscarPorId(Guid id)
        {
            var documentoFiscal = _mapper.Map<DocumentoFiscalDTO>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarPorId(id));

            if (documentoFiscal == null) return null;

            var conteudoXML = await BuscarConteudoXML(documentoFiscal);

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
            else if (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.CteEntrada || documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.CteSaida || documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.CTeNaoTomador)
                detalhesDocumento.ConhecimentoTransporteEletronico = FuncoesXml.XmlStringParaClasse<cteProc>(conteudoXML);
            else if ((documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseEntrada) || (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseSaida))
                detalhesDocumento.NotaFiscalServicoEletronico = UtilitarioXML.XmlStringParaClasse<CompNfse>(GetXml(conteudoXML));

            return detalhesDocumento;
        }

        public IEnumerable<DocumentoFiscalDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<DocumentoFiscalDTO>>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>()
            .BuscarTodos(new UtilitarioSpecification<DocumentoFiscal>().CriarEspecificacaoFiltro(filtro)));

        public IEnumerable<DocumentoFiscalDTO> BuscarPendentes(string filtro)
        {
            var spec = new UtilitarioSpecification<DocumentoFiscal>().CriarEspecificacaoFiltro(filtro);

            spec &= new DirectSpecification<DocumentoFiscal>(c => c.DadosImportacao == null || c.DadosImportacao.DataImportacao == 0);

            return _mapper.Map<IEnumerable<DocumentoFiscalDTO>>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarTodos(spec));
        }

        public async Task<IEnumerable<RetornoDocumentoFiscalDTO>> Inserir(EnviarArquivosDTO arquivos)
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());
            empresa.ConfiguracaoEmpresa = _mapper.Map<ConfiguracaoEmpresaDTO>(_repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().BuscarTodos().FirstOrDefault());

            var documentosFiscais = await ProcessarArquivosEnviados(arquivos, empresa);

            var documentosValidos = new List<DocumentoFiscal>();
            var documentosCancelamento = new List<DocumentoFiscal>();

            DocumentosValidos(documentosFiscais, arquivos.DetalhesEnvio, ref documentosValidos, ref documentosCancelamento);

            if (documentosValidos.Count > 0)
            {
                AtualizaCancelamentoDocumentos(documentosValidos);
                _repositoryFactory.Instancie<IDocumentoFiscalRepository>().InserirTodos(documentosValidos);
            }

            if (documentosCancelamento.Count > 0)
                _repositoryFactory.Instancie<IDocumentoFiscalRepository>().AtualizarTodos(documentosCancelamento);

            var retornoDocumentosValidos = documentosValidos.Select(c => new RetornoDocumentoFiscalDTO() { NomeArquivo = c.NomeArquivo, ChaveDocumentoFiscal = c.ChaveDocumentoFiscal });
            var retornoDocumentosCancelados = documentosCancelamento.Select(c => new RetornoDocumentoFiscalDTO() { NomeArquivo = c.NomeArquivo, ChaveDocumentoFiscal = c.ChaveDocumentoFiscal });

            AtualizarManifestoDocumentoArmazenado(retornoDocumentosValidos);
            AtualizarManifestoDocumentoCancelado(retornoDocumentosCancelados);

            return retornoDocumentosValidos.Concat(retornoDocumentosCancelados);
        }

        public void Deletar(Guid id) => _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Excluir(id);

        public bool ExisteNaoProcessado() => _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Contar(
            new DirectSpecification<DocumentoFiscal>(c => c.TipoDocumentoFiscal != ETipoDocumentoFiscal.CTeNaoTomador && c.DadosImportacao == null || c.DadosImportacao.DataImportacao == 0)) > 0;

        public void ConfirmarImportacao(Guid id, DadosImportacaoDTO dadosImportacao)
        {
            var documentoFiscalEncontrado = _mapper.Map<DocumentoFiscalDTO>(_repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarPorId(id));

            if (documentoFiscalEncontrado == null)
            {
                _bus.RaiseEvent(new DomainNotification("NaoEncontrado", "Documento Fiscal informado não foi encontrado."));
                return;
            }

            if (!documentoFiscalEncontrado.Processado || dadosImportacao.Desfazer)
            {
                documentoFiscalEncontrado.DadosImportacao = dadosImportacao.Desfazer ? null : dadosImportacao;

                var documentoFiscal = _mapper.Map<DocumentoFiscal>(documentoFiscalEncontrado);

                if (EhValido(documentoFiscal))
                    _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Atualizar(documentoFiscal);
            }
        }

        public void CancelarDocumentos(IEnumerable<string> chavesCancelamento)
        {
            var documentosExistentes = _repositoryFactory.Instancie<IDocumentoFiscalRepository>()
                .BuscarTodos(d => chavesCancelamento.Contains(d.ChaveDocumentoFiscal)).ToList();

            if (documentosExistentes.Any())
            {
                var documentosAtualizados = documentosExistentes.Select(c => { c.Cancelado = true; return c; });

                _repositoryFactory.Instancie<IDocumentoFiscalRepository>().AtualizarTodos(documentosAtualizados);
            }

            var manifestosExistentes = _repositoryFactory.Instancie<IManifestoRepository>().BuscarTodos(d => chavesCancelamento.Contains(d.ChaveDocumentoFiscal));

            if (manifestosExistentes.Any())
            {
                var manifestosAtualizados = manifestosExistentes.Select(c => { c.SituacaoManifesto = ESituacaoManifesto.Cancelado; return c; });

                _repositoryFactory.Instancie<IManifestoRepository>().AtualizarTodos(manifestosAtualizados);
            }
        }

        public void BuscarManualmente(ETipoDocumentoConsulta tipo, DadosBuscaDTO dadosBuscaDTO)
        {
            if (tipo == ETipoDocumentoConsulta.NFe)
                RealizarBuscaNFe(dadosBuscaDTO);
            if (tipo == ETipoDocumentoConsulta.CTe)
                RealizarBuscaCTe(dadosBuscaDTO);
        }

        #region Private Methods
        private void AtualizaCancelamentoDocumentos(List<DocumentoFiscal> documentosValidos)
        {
            var chaveDocumentoRepository = _repositoryFactory.Instancie<IChaveDocumentoCanceladoRepository>();

            var chaves = documentosValidos.Select(c => c.ChaveDocumentoFiscal)
                .ToList();

            var cancelamentos = chaveDocumentoRepository.BuscarTodos(c => chaves.Contains(c.ChaveDocumentoFiscal))
                .ToList();

            var manifestos = _repositoryFactory.Instancie<IManifestoRepository>()
                 .BuscarTodos(m => chaves.Contains(m.ChaveDocumentoFiscal) && m.SituacaoManifesto == ESituacaoManifesto.Cancelado)
                 .ToList();


            foreach (var documento in documentosValidos)
            {
                var cancelamento = cancelamentos.FirstOrDefault(c => c.ChaveDocumentoFiscal == documento.ChaveDocumentoFiscal);

                documento.Cancelado = cancelamento != null
                    || manifestos.Any(m => m.ChaveDocumentoFiscal == documento.ChaveDocumentoFiscal);

                if (cancelamento != null)
                    chaveDocumentoRepository.Excluir(cancelamento);

            }
        }

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
                try
                {
                    using (StreamReader reader = new StreamReader(arquivo.OpenReadStream()))
                    {
                        var conteudoXML = reader.ReadToEnd();

                        if (Regex.IsMatch(conteudoXML, "<descEvento>Cancelamento</descEvento>", RegexOptions.IgnoreCase))
                        {
                            var chave = Regex.Match(conteudoXML, "(?<=>)([0-9]{44})(?=<)").Value;

                            documentosFiscais.Add(new DocumentoFiscalDTO(true, chave));
                        }
                        else if (Regex.IsMatch(conteudoXML, "<chNFe>(.*?)</chNFe>", RegexOptions.IgnoreCase))
                        {
                            var notaFiscal = ProcessarXMLparaNFe(empresa.Inscricao, conteudoXML, arquivo.FileName);

                            if (notaFiscal != null)
                                documentosFiscais.Add(await PrepararDocumentoFiscal(dadosOrigem, notaFiscal, arquivo.FileName, arquivo.OpenReadStream()));
                        }
                        else if (Regex.IsMatch(conteudoXML, "<chCTe>(.*?)</chCTe>", RegexOptions.IgnoreCase))
                        {
                            var conhecimentoTransporte = ProcessarXMLparaCTe(empresa.Inscricao, empresa.ConfiguracaoEmpresa != null && empresa.ConfiguracaoEmpresa.SalvarCteNaoTomador,
                                conteudoXML, arquivo.FileName);

                            if (conhecimentoTransporte != null)
                                documentosFiscais.Add(await PrepararDocumentoFiscal(dadosOrigem, conhecimentoTransporte, arquivo.FileName, arquivo.OpenReadStream()));
                        }
                        else
                        {
                            var matches = Regex.Matches(conteudoXML, "<CompNfse[^>]*?>(.*?)</CompNfse>|<ComplNfse[^>]*?>(.*?)</ComplNfse>", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline);

                            if (matches.Count == 0)
                            {
                                await NotificarDocumentoInvalidos(empresa, arquivo, null, "Documento não suportado.");
                                continue;
                            }

                            foreach (Match match in matches)
                            {
                                var notaFiscalServico = ProcessarXMLparaNFse(empresa.Inscricao, GetXml(match.Value), arquivo.FileName);

                                if (notaFiscalServico != null)
                                {
                                    using (var streamFile = new MemoryStream(Encoding.UTF8.GetBytes(match.Value)))
                                        documentosFiscais.Add(await PrepararDocumentoFiscal(dadosOrigem, notaFiscalServico, arquivo.FileName, streamFile));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await NotificarDocumentoInvalidos(empresa, arquivo, ex, "Documento não suportado.");
                    continue;
                }
            }

            return documentosFiscais;
        }

        private async Task<DocumentoFiscalDTO> PrepararDocumentoFiscal(DadosOrigemDocumentoFiscalDTO dadosOrigem, DocumentoFiscalDTO documentoFiscal, string nomeArquivo, Stream arquivo)
        {
            documentoFiscal.DadosOrigem = dadosOrigem;
            documentoFiscal.NomeArquivo = nomeArquivo;

            await UploadFileToBlobStorage(arquivo, documentoFiscal);

            return documentoFiscal;
        }

        private async Task UploadFileToBlobStorage(Stream arquivo, DocumentoFiscalDTO documentoFiscal)
        {
            var ehNotaFiscalServico = (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseSaida) || (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseEntrada);

            if (ehNotaFiscalServico || !documentoFiscal.Cancelado)
            {
                string folderName = ObterFolderNameFromKey(ehNotaFiscalServico ? documentoFiscal.DataEmissaoDocumento.ToString() : documentoFiscal.ChaveDocumentoFiscal, ehNotaFiscalServico);

                await _azureBlobStorage.UploadAsync(documentoFiscal.ChaveDocumentoFiscal, folderName, arquivo);
            }
        }

        private DocumentoFiscalDTO ProcessarXMLparaCTe(string inscricaoEmpresa, bool salvarCteNaoTomador, string conteudoXML, string nomeArquivo)
        {
            var cte = UtilitarioXML.XmlStringParaClasse<cteProc>(conteudoXML);

            var conhecimentoTransporte = ObterConhecimentoTransporteFromObject(inscricaoEmpresa, salvarCteNaoTomador, cte);

            if (conhecimentoTransporte == null)
            {
                NotificarAplicacao(nomeArquivo, "Documento não pertence a empresa selecionada.");
                return null;
            }

            return conhecimentoTransporte;
        }

        private DocumentoFiscalDTO ProcessarXMLparaNFe(string inscricaoEmpresa, string conteudoXML, string nomeArquivo)
        {
            var nfe = UtilitarioXML.XmlStringParaClasse<nfeProc>(conteudoXML);

            var notaFiscal = ObterNotaFiscalFromObject(inscricaoEmpresa, nfe);

            if (notaFiscal == null)
            {
                NotificarAplicacao(nomeArquivo, "Documento não pertence a empresa selecionada.");
                return null;
            }

            return notaFiscal;
        }

        private DocumentoFiscalDTO ProcessarXMLparaNFse(string inscricaoEmpresa, string conteudoXML, string nomeArquivo)
        {
            var compNfse = UtilitarioXML.XmlStringParaClasse<CompNfse>(conteudoXML);

            var notaFiscalServicoEletronica = ObterNotaFiscalServicoFromObject(inscricaoEmpresa, compNfse);

            if (notaFiscalServicoEletronica == null)
            {
                NotificarAplicacao(nomeArquivo, "Documento não pertence a empresa selecionada.");
                return null;
            }

            return notaFiscalServicoEletronica;
        }

        private DocumentoFiscalDTO ObterConhecimentoTransporteFromObject(string inscricaoEmpresa, bool salvarCteNaoTomador, cteProc cte)
        {
            var documentoFiscal = new DocumentoFiscalDTO()
            {
                DataArmazenamento = UtilitarioDatas.ConvertToIntDateTime(DateTime.Now),
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

            if (documentoFiscal.TipoDocumentoFiscal == 0 || (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.CTeNaoTomador && !salvarCteNaoTomador)) return null;

            return documentoFiscal;
        }

        private DocumentoFiscalDTO ObterNotaFiscalFromObject(string inscricaoEmpresa, nfeProc nfe)
        {
            var documentoFiscal = new DocumentoFiscalDTO()
            {
                DataArmazenamento = UtilitarioDatas.ConvertToIntDateTime(DateTime.Now),
                DataEmissaoDocumento = UtilitarioDatas.ConvertToIntDate(nfe.NFe.infNFe.ide.dhEmi.UtcDateTime),
                ChaveDocumentoFiscal = nfe.protNFe.infProt.chNFe,
                SerieDocumentoFiscal = nfe.NFe.infNFe.ide.serie.ToString(),
                NumeroDocumentoFiscal = nfe.NFe.infNFe.ide.nNF.ToString(),
                ValorDocumentoFiscal = (double)nfe.NFe.infNFe.total.ICMSTot.vNF,
                TipoDocumentoFiscal = nfe.NFe.infNFe.ide.mod == ModeloDocumento.NFCe
                    ? ETipoDocumentoFiscal.Nfce
                    : ObterTipoNotaFiscal(inscricaoEmpresa, nfe.NFe.infNFe.ide.tpNF, nfe.NFe.infNFe.emit, nfe.NFe.infNFe.dest, nfe.NFe.infNFe.det.FirstOrDefault())
            };

            if (documentoFiscal.TipoDocumentoFiscal == 0) return null;

            var inscricaoEmitente = nfe.NFe.infNFe.emit.CNPJ ?? nfe.NFe.infNFe.emit.CPF;
            var inscricaoDestinatario = nfe.NFe.infNFe.dest != null ? nfe.NFe.infNFe.dest.CNPJ ?? nfe.NFe.infNFe.dest.CPF : string.Empty;

            if (inscricaoEmpresa != inscricaoEmitente && inscricaoEmpresa != inscricaoDestinatario) return null;

            if (inscricaoEmpresa == inscricaoEmitente)
                documentoFiscal.InscricaoEstadual = nfe.NFe.infNFe.emit.IE;
            else if (inscricaoEmpresa == inscricaoDestinatario)
                documentoFiscal.InscricaoEstadual = nfe.NFe.infNFe.dest.IE;

            if ((documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.Nfce) || (inscricaoEmpresa != inscricaoEmitente))
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

            if (nfe.protNFe != null && nfe.protNFe.infProt != null && nfe.protNFe.infProt.cStat == 101)
                documentoFiscal.Cancelado = true;

            return documentoFiscal;
        }

        private DocumentoFiscalDTO ObterNotaFiscalServicoFromObject(string inscricaoEmpresa, CompNfse compNfse)
        {
            var documentoFiscal = new DocumentoFiscalDTO()
            {
                DataArmazenamento = UtilitarioDatas.ConvertToIntDateTime(DateTime.Now),
                DataEmissaoDocumento = UtilitarioDatas.ConvertToIntDate(compNfse.Nfse.InfNfse.DataEmissao),
                ChaveDocumentoFiscal = compNfse.Nfse.InfNfse.CodigoVerificacao,
                NumeroDocumentoFiscal = compNfse.Nfse.InfNfse.Numero,
            };

            PrestadorGenerico dadosPrestador = (PrestadorGenerico)compNfse.Nfse.InfNfse.PrestadorServico ?? compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico.InfDeclaracaoPrestacaoServico.Prestador;
            TomadorGenerico dadosTomador = (TomadorGenerico)compNfse.Nfse.InfNfse.TomadorServico ?? compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico.InfDeclaracaoPrestacaoServico.Tomador;

            var inscricaoPrestador = dadosPrestador.IdentificacaoPrestador != null
                ? (dadosPrestador.IdentificacaoPrestador.Cnpj ?? dadosPrestador.IdentificacaoPrestador.CpfCnpj.Cnpj ?? dadosPrestador.IdentificacaoPrestador.CpfCnpj.Cpf)
                : (dadosPrestador.CpfCnpj.Cnpj ?? dadosPrestador.CpfCnpj.Cpf);

            var inscricaoTomador = dadosTomador.IdentificacaoTomador.CpfCnpj.Cnpj ?? dadosTomador.IdentificacaoTomador.CpfCnpj.Cpf;

            if (!string.IsNullOrEmpty(inscricaoPrestador) && inscricaoEmpresa == inscricaoPrestador.RemoveMascaras())
                documentoFiscal.TipoDocumentoFiscal = ETipoDocumentoFiscal.NfseSaida;
            else if (!string.IsNullOrEmpty(inscricaoTomador) && inscricaoEmpresa == inscricaoTomador.RemoveMascaras())
                documentoFiscal.TipoDocumentoFiscal = ETipoDocumentoFiscal.NfseEntrada;

            if (compNfse.Nfse.InfNfse.Servico != null && compNfse.Nfse.InfNfse.Servico.Valores.ValorServicos > 0)
                documentoFiscal.ValorDocumentoFiscal = (double)compNfse.Nfse.InfNfse.Servico.Valores.ValorServicos;
            if (compNfse.Nfse.InfNfse.ValoresNfse != null && compNfse.Nfse.InfNfse.ValoresNfse.ValorServicos > 0)
                documentoFiscal.ValorDocumentoFiscal = (double)compNfse.Nfse.InfNfse.ValoresNfse.ValorServicos;
            else if (compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico != null &&
                compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico.InfDeclaracaoPrestacaoServico != null &&
                compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico.InfDeclaracaoPrestacaoServico.Servico != null &&
                compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico.InfDeclaracaoPrestacaoServico.Servico.Valores != null &&
                compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico.InfDeclaracaoPrestacaoServico.Servico.Valores.ValorServicos > 0)
                documentoFiscal.ValorDocumentoFiscal = (double)compNfse.Nfse.InfNfse.DeclaracaoPrestacaoServico.InfDeclaracaoPrestacaoServico.Servico.Valores.ValorServicos;

            if (documentoFiscal.TipoDocumentoFiscal == 0) return null;

            if ((documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseSaida))
            {
                documentoFiscal.DadosEmitenteDestinatario = new DadosFornecedorDTO()
                {
                    Inscricao = inscricaoTomador,
                    RazaoSocial = dadosTomador.RazaoSocial
                };
            }
            else
            {
                documentoFiscal.DadosEmitenteDestinatario = new DadosFornecedorDTO()
                {
                    Inscricao = inscricaoPrestador,
                    RazaoSocial = dadosPrestador.RazaoSocial
                };
            }

            return documentoFiscal;
        }

        private ETipoDocumentoFiscal ObterTipoConhecimentoTransporte(string inscricaoEmpresa, cteProc cte)
        {
            if (cte.CTe.infCte.ide.tomaBase3 == null && cte.CTe.infCte.ide.toma4 != null)
                return inscricaoEmpresa == (cte.CTe.infCte.ide.toma4.CNPJ ?? cte.CTe.infCte.ide.toma4.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CTeNaoTomador;

            switch (cte.CTe.infCte.ide.tomaBase3.toma)
            {
                case CTe.Classes.Informacoes.Tipos.toma.Remetente:
                    return inscricaoEmpresa == (cte.CTe.infCte.rem.CNPJ ?? cte.CTe.infCte.rem.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CTeNaoTomador;
                case CTe.Classes.Informacoes.Tipos.toma.Expedidor:
                    return inscricaoEmpresa == (cte.CTe.infCte.exped.CNPJ ?? cte.CTe.infCte.exped.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CTeNaoTomador;
                case CTe.Classes.Informacoes.Tipos.toma.Recebedor:
                    return inscricaoEmpresa == (cte.CTe.infCte.receb.CNPJ ?? cte.CTe.infCte.receb.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CTeNaoTomador;
                case CTe.Classes.Informacoes.Tipos.toma.Destinatario:
                    return inscricaoEmpresa == (cte.CTe.infCte.dest.CNPJ ?? cte.CTe.infCte.dest.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CTeNaoTomador;
                case CTe.Classes.Informacoes.Tipos.toma.Outros:
                    return inscricaoEmpresa == (cte.CTe.infCte.ide.toma4.CNPJ ?? cte.CTe.infCte.ide.toma4.CPF) ? ETipoDocumentoFiscal.CteEntrada : ETipoDocumentoFiscal.CTeNaoTomador;
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

        private void DocumentosValidos(List<DocumentoFiscalDTO> documentosGerados, List<EnviarArquivosDTO.DetalhesEnvioDTO> detalhesEnvio,
            ref List<DocumentoFiscal> documentosValidos, ref List<DocumentoFiscal> documentosCancelamento)
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
                    var repository = _repositoryFactory.Instancie<IChaveDocumentoCanceladoRepository>();
                    var cancelado = repository.BuscarPorExpressao(c => c.ChaveDocumentoFiscal == documentoGerado.ChaveDocumentoFiscal);

                    if (cancelado == null)
                        repository.Inserir(new ChaveDocumentoCancelado { ChaveDocumentoFiscal = documentoGerado.ChaveDocumentoFiscal });

                    _bus.RaiseEvent(new DomainNotification(documentoGerado.NomeArquivo, "Não foi encontrado documento para o cancelamento enviado."));
                    continue;
                }

                var documentoFiscal = _mapper.Map<DocumentoFiscal>(documentoGerado);

                if (EhValido(documentoFiscal))
                {
                    if (detalhesEnvio != null && detalhesEnvio.Count > 0)
                    {
                        var detalheEnvio = detalhesEnvio.Where(c => c.ChaveDocumentoFiscal == documentoFiscal.ChaveDocumentoFiscal).FirstOrDefault();

                        if (detalheEnvio != null)
                            documentoFiscal.NsuDocumentoFiscal = detalheEnvio.NsuDocumentoFiscal;
                    }

                    documentosExistentes.Add(documentoFiscal);
                    documentosValidos.Add(documentoFiscal);
                }
            }
        }

        private bool EhValido(DocumentoFiscal documentoFiscal)
        {
            var valido = true;

            #region Validar Documento
            var validator = new DocumentoFiscalValidator().Validate(documentoFiscal);

            if (!validator.IsValid)
            {
                valido = false;

                CriarMensagensErro(documentoFiscal.NomeArquivo, validator);
            }
            #endregion

            #region Validar Dados Origem
            if (documentoFiscal.DadosOrigem != null)
            {
                var validatorDadosOrigem = new DadosOrigemDocumentoFiscalValidator().Validate(documentoFiscal.DadosOrigem);

                if (!validatorDadosOrigem.IsValid)
                {
                    valido = false;

                    CriarMensagensErro(documentoFiscal.NomeArquivo, validatorDadosOrigem);
                }
            }
            #endregion

            #region Validar Dados Importacao
            if (documentoFiscal.DadosImportacao != null)
            {
                var validatorDadosImportacao = new DadosImportacaoValidator().Validate(documentoFiscal.DadosImportacao);

                if (!validatorDadosImportacao.IsValid)
                {
                    valido = false;

                    CriarMensagensErro(documentoFiscal.NomeArquivo, validatorDadosImportacao);
                }
            }
            #endregion

            #region Validar Dados Fornecedor
            if (documentoFiscal.DadosEmitenteDestinatario != null)
            {
                var validatorDadosFornecedor = new DadosFornecedorValidator().Validate(documentoFiscal.DadosEmitenteDestinatario);

                if (!validatorDadosFornecedor.IsValid)
                {
                    valido = false;

                    CriarMensagensErro(documentoFiscal.NomeArquivo, validatorDadosFornecedor);
                }
            }
            #endregion

            return valido;
        }

        private async Task<string> BuscarConteudoXML(DocumentoFiscalDTO documentoFiscal)
        {
            var ehNotaFiscalServico = (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseSaida) || (documentoFiscal.TipoDocumentoFiscal == ETipoDocumentoFiscal.NfseEntrada);

            string folderName = ObterFolderNameFromKey(ehNotaFiscalServico ? documentoFiscal.DataEmissaoDocumento.ToString() : documentoFiscal.ChaveDocumentoFiscal, ehNotaFiscalServico);

            var arquivoExiste = await _azureBlobStorage.ExistsAsync(documentoFiscal.ChaveDocumentoFiscal, folderName);

            if (arquivoExiste)
            {
                var anexoDownloaded = await _azureBlobStorage.DownloadAsync(documentoFiscal.ChaveDocumentoFiscal, folderName);

                using (MemoryStream originalFileMemoryStream = new MemoryStream(anexoDownloaded.ToArray()))
                {
                    using (StreamReader reader = new StreamReader(originalFileMemoryStream))
                        return reader.ReadToEnd();
                }
            }

            return string.Empty;
        }

        private static string ObterFolderNameFromKey(string value, bool ehNotaFiscalServico)
        {
            if (ehNotaFiscalServico)
                return path.Replace("{tipo}", "nfse").Replace("{anomes}", value.Substring(2, 4));

            var tipoDocumento = GetTipoDocumento(value);

            return path.Replace("{tipo}", tipoDocumento).Replace("{anomes}", value.Substring(2, 4));
        }

        private static string GetTipoDocumento(string chaveDocumentoFiscal)
        {
            var modelo = chaveDocumentoFiscal.Substring(20, 2);

            if (modelo == "55") return "nfe";

            if (modelo == "65") return "nfce";

            return "cte";
        }

        private static string GetXml(string conteudoXML)
        {
            var regex = new Regex("tc:|ns2:|ns3:");

            return regex.Replace(conteudoXML, string.Empty).Replace("ComplNfse", "CompNfse");
        }

        private void AtualizarManifestoDocumentoCancelado(IEnumerable<RetornoDocumentoFiscalDTO> retornoDocumentosCancelados)
        {
            var documentosCancelados = retornoDocumentosCancelados.Select(c => c.ChaveDocumentoFiscal);

            var manifestos = _repositoryFactory.Instancie<IManifestoRepository>().BuscarTodos(d => documentosCancelados.Contains(d.ChaveDocumentoFiscal));

            if (manifestos.Any())
            {
                var manifestosAtualizados = manifestos.Select(c => { c.SituacaoManifesto = ESituacaoManifesto.Cancelado; return c; });

                _repositoryFactory.Instancie<IManifestoRepository>().AtualizarTodos(manifestosAtualizados);
            }
        }

        private void AtualizarManifestoDocumentoArmazenado(IEnumerable<RetornoDocumentoFiscalDTO> retornoDocumentosValidos)
        {
            var documentosValidos = retornoDocumentosValidos.Select(c => c.ChaveDocumentoFiscal);

            var manifestos = _repositoryFactory.Instancie<IManifestoRepository>().BuscarTodos(d => documentosValidos.Contains(d.ChaveDocumentoFiscal));

            if (manifestos.Any())
            {
                var manifestosAtualizados = manifestos.Select(c => { c.SituacaoDocumentoFiscal = ESituacaoDocumentoFiscal.Armazenado; return c; });

                _repositoryFactory.Instancie<IManifestoRepository>().AtualizarTodos(manifestosAtualizados);
            }
        }

        private async Task NotificarDocumentoInvalidos(EmpresaDTO empresa, IFormFile arquivo, Exception ex, string mensagem)
        {
            if (ex != null) NotificarExcecaoSentry(arquivo, ex);

            await NotificarEmail(empresa, arquivo);

            NotificarAplicacao(arquivo.FileName, mensagem);
        }

        private static void NotificarExcecaoSentry(IFormFile arquivo, Exception ex)
        {
            if (arquivo != null)
            {
                var sentry = new SentryEvent(ex);

                using (StreamReader reader = new StreamReader(arquivo.OpenReadStream()))
                {
                    sentry.Message = reader.ReadToEnd();

                    SentrySdk.CaptureEvent(sentry);
                }
            }
        }

        private static async Task NotificarEmail(EmpresaDTO empresa, IFormFile arquivo)
        {
            using (var memoryStream = new MemoryStream())
            {
                await arquivo.CopyToAsync(memoryStream);

                await TemplateEmail.EnviarEmailDocumentoNaoSuportado("suporte.smart@tron.com.br", $"{empresa.Inscricao.AdicionarMascaraInscricao()} - {empresa.RazaoSocial}",
                    arquivo.FileName, memoryStream);
            }
        }

        private void NotificarAplicacao(string nomeArquivo, string mensagem) => _bus.RaiseEvent(new DomainNotification(nomeArquivo, mensagem));

        private void CriarMensagensErro(string nomeArquivo, ValidationResult validation)
        {
            var erros = validation.Errors.Select(c => new
            {
                Chave = c.PropertyName,
                Mensagem = c.ErrorMessage
            });

            _bus.RaiseEvent(new DomainNotification(nomeArquivo, erros));
        }

        private void RealizarBuscaNFe(DadosBuscaDTO dadosBuscaDTO)
        {
            var tenantId = FabricaGeral.Instancie<ITenantProvider>().GetTenant().Id.ToString();

            RealizarBuscaManualPortal(dadosBuscaDTO, tenantId);

            if (dadosBuscaDTO.UF == "MT")
                RealizarBuscaManualMatoGrosso(dadosBuscaDTO, tenantId);
            else if (dadosBuscaDTO.UF == "MS")
                RealizarBuscaManualMatoGrossoSul(dadosBuscaDTO, tenantId);
        }

        private static void RealizarBuscaCTe(DadosBuscaDTO dadosBuscaDTO)
        {
            var tenantId = FabricaGeral.Instancie<ITenantProvider>().GetTenant().Id.ToString();

            var dadosBusca = new DadosConsultaCTeDTO(dadosBuscaDTO.Inscricao, "0", dadosBuscaDTO.UF, (int)ETipoConsulta.Manual,
                dadosBuscaDTO.MetodoBusca == EMetodoBusca.UltimosTrintaDias, tenantId);

            UtilitarioHttpClient.PostRequest(string.Empty, URL_AGENTE_MANIFESTACAO_CTE, "api/ctes/consultar", dadosBusca);
        }

        private static void RealizarBuscaManualPortal(DadosBuscaDTO dadosBuscaDTO, string tenantId)
        {
            var dadosBusca = new DadosConsultaNFeDTO(dadosBuscaDTO.Inscricao, "0", dadosBuscaDTO.ManifestarAutomaticamente, dadosBuscaDTO.UF,
                dadosBuscaDTO.MetodoBusca == EMetodoBusca.UltimosTrintaDias, dadosBuscaDTO.SalvarSomenteManifestadas, false, (int)ETipoConsulta.Manual, tenantId);

            UtilitarioHttpClient.PostRequest(string.Empty, URL_AGENTE_MANIFESTACAO_NFE, "api/nfes/consultar", dadosBusca);
        }

        private void RealizarBuscaManualMatoGrosso(DadosBuscaDTO dadosBuscaDTO, string tenantId)
        {
            var configuracaoEmpresa = _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().BuscarTodos().FirstOrDefault();

            if (configuracaoEmpresa != null && configuracaoEmpresa.DadosMatoGrosso != null)
            {
                if (!string.IsNullOrEmpty(configuracaoEmpresa.DadosMatoGrosso.Usuario) && !string.IsNullOrEmpty(configuracaoEmpresa.DadosMatoGrosso.Senha))
                {
                    foreach (var inscricaoComplementar in configuracaoEmpresa.InscricoesComplementares)
                    {
                        if (inscricaoComplementar.ConsultaPortalEstadual && inscricaoComplementar.Situacao == eSituacao.Ativo)
                        {
                            var inscricaoEstadual = inscricaoComplementar.InscricaoEstadual.PadLeft(11, '0');

                            var dataInicial = configuracaoEmpresa.MetodoBusca == EMetodoBusca.UltimosTrintaDias ? UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-30)) : UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-90));
                            var dataFinal = UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-1));

                            var dadosBuscaMatoGrosso = new DadosBuscaMatoGrossoDTO(tenantId, $"{dadosBuscaDTO.Inscricao} - {inscricaoEstadual}", dadosBuscaDTO.Inscricao,
                                inscricaoEstadual, dataInicial, dataFinal, 2, configuracaoEmpresa.DadosMatoGrosso.Usuario, configuracaoEmpresa.DadosMatoGrosso.Senha,
                                (int)configuracaoEmpresa.DadosMatoGrosso.Tipo);

                            UtilitarioHttpClient.PostRequest(string.Empty, URL_SCRAPER_SEFAZ_MT, $"api/scraper", dadosBuscaMatoGrosso);
                        }
                    }
                }
            }
        }

        private void RealizarBuscaManualMatoGrossoSul(DadosBuscaDTO dadosBuscaDTO, string tenantId)
        {
            var configuracaoEmpresa = _repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().BuscarTodos().FirstOrDefault();

            if (configuracaoEmpresa != null && configuracaoEmpresa.DadosMatoGrossoSul != null)
            {
                if (!string.IsNullOrEmpty(configuracaoEmpresa.DadosMatoGrossoSul.Usuario) && !string.IsNullOrEmpty(configuracaoEmpresa.DadosMatoGrossoSul.CodigoAcesso) && !string.IsNullOrEmpty(configuracaoEmpresa.DadosMatoGrossoSul.Senha))
                {
                    foreach (var inscricaoComplementar in configuracaoEmpresa.InscricoesComplementares)
                    {
                        if (inscricaoComplementar.ConsultaPortalEstadual && inscricaoComplementar.Situacao == eSituacao.Ativo)
                        {
                            var dataInicial = configuracaoEmpresa.MetodoBusca == EMetodoBusca.UltimosTrintaDias ? UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-30)) : UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-90));
                            var dataFinal = UtilitarioDatas.ConvertToIntDate(DateTime.Now.AddDays(-1));

                            var dadosBuscaMatoGrossoSul = new DadosBuscaMatoGrossoSulDTO(tenantId, dadosBuscaDTO.Inscricao, dadosBuscaDTO.Inscricao,
                                dataInicial, dataFinal, 2, configuracaoEmpresa.DadosMatoGrossoSul.Usuario, configuracaoEmpresa.DadosMatoGrossoSul.CodigoAcesso,
                                configuracaoEmpresa.DadosMatoGrossoSul.Senha);

                            UtilitarioHttpClient.PostRequest(string.Empty, URL_SCRAPER_SEFAZ_MS, $"api/scraper", dadosBuscaMatoGrossoSul);
                        }
                    }
                }
            }
        }
        #endregion
    }
}