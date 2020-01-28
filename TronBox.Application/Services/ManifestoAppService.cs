using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.Aggregates.ManifestoAgg.Repository;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Dominio.Bus;
using TronCore.Dominio.JsonPatch;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class ManifestoAppService : IManifestoAppService
    {
        //public static string URL_AGENTE_MANIFESTACAO = "http://192.168.10.229:3000";
        public static string URL_AGENTE_MANIFESTACAO = "http://10.20.30.28:8085";

        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public ManifestoAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public void Dispose()
        {
        }

        public void Atualizar(Guid id, dynamic manifestoDTO)
        {
            var manifestoDb = BuscarPorId(id);

            var manifesto = _mapper.Map<Manifesto>(AjudanteJsonPatch.Instancia.ApplyPatch(manifestoDb, manifestoDTO));

            // Quando o manifesto está cancelado não permitido alterar a situação.
            if (manifestoDb.SituacaoManifesto == ESituacaoManifesto.Cancelado && manifesto.SituacaoManifesto != manifestoDb.SituacaoManifesto)
                manifesto.SituacaoManifesto = manifestoDb.SituacaoManifesto;
            // Quando o manifesto existente é Ciência Automática e for enviado 
            // evento de Ciência não é permitido alterar a situação para Ciência.
            if ((manifesto.SituacaoManifesto == ESituacaoManifesto.Ciencia) && (manifestoDb.SituacaoManifesto == ESituacaoManifesto.CienciaAutomatica))
                manifesto.SituacaoManifesto = ESituacaoManifesto.CienciaAutomatica;

            if (EhValido(manifesto)) _repositoryFactory.Instancie<IManifestoRepository>().Atualizar(manifesto);
        }

        public ManifestoDTO BuscarPorId(Guid id) => _mapper.Map<ManifestoDTO>(_repositoryFactory.Instancie<IManifestoRepository>().BuscarPorId(id));

        public IEnumerable<ManifestoDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<ManifestoDTO>>(_repositoryFactory.Instancie<IManifestoRepository>()
            .BuscarTodos(new UtilitarioSpecification<Manifesto>().CriarEspecificacaoFiltro(filtro)));

        public void Deletar(Guid id) => _repositoryFactory.Instancie<IManifestoRepository>().Excluir(id);

        public void Inserir(ManifestoDTO manifestoDTO)
        {
            if (manifestoDTO == null)
            {
                _bus.RaiseEvent(new DomainNotification("Manifesto", "Manifesto enviado não informado ou está inválido."));
                return;
            }

            if (manifestoDTO.Id == null)
                manifestoDTO.Id = Guid.NewGuid().ToString();

            var manifesto = _mapper.Map<Manifesto>(manifestoDTO);

            if (_repositoryFactory.Instancie<IManifestoRepository>().BuscarTodos(c => c.ChaveDocumentoFiscal == manifesto.ChaveDocumentoFiscal).Any())
            {
                _bus.RaiseEvent(new DomainNotification("ManifestoExistente", "Manifesto já existente na base de dados."));
                return;
            }

            if (EhValido(manifesto)) _repositoryFactory.Instancie<IManifestoRepository>().Inserir(manifesto);
        }

        public async Task<RespostaManifestacaoDTO> Manifestar(ManifestarDTO manifestarDTO)
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());
            var configuracaoEmpresa = _mapper.Map<ConfiguracaoEmpresaDTO>(_repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>().BuscarTodos().FirstOrDefault());

            if (configuracaoEmpresa == null || !configuracaoEmpresa.InscricoesComplementares.Any())
            {
                _bus.RaiseEvent(new DomainNotification("CadastroIncompleto", "Cadastro da empresa está incompleto."));
                return null;
            }

            var manifestacao = new
            {
                configuracaoEmpresa.InscricoesComplementares.FirstOrDefault().UF,
                registry = empresa.Inscricao,
                keyNFe = manifestarDTO.ChaveDocumentoFiscal,
                tpEvent = ObterTIpoManifestacao(manifestarDTO.TipoManifestacao)
            };

            var manifesto = _repositoryFactory.Instancie<IManifestoRepository>().BuscarPorExpressao(c => c.ChaveDocumentoFiscal == manifestarDTO.ChaveDocumentoFiscal);

            if (manifesto != null && manifesto.SituacaoManifesto != ESituacaoManifesto.Cancelado)
            {
                var result = await UtilitarioHttpClient.PostRequest(string.Empty, URL_AGENTE_MANIFESTACAO, "mdf-e/manifest-document", manifestacao);

                var respostaManifestacao = JsonConvert.DeserializeObject<RespostaManifestacaoDTO>(result);

                if (respostaManifestacao != null)
                    AtualizarManifesto(manifestarDTO, manifesto, respostaManifestacao);

                return respostaManifestacao;
            }

            return null;
        }

        #region Private Methods
        private void AtualizarManifesto(ManifestarDTO manifestarDTO, Manifesto manifesto, RespostaManifestacaoDTO respostaManifestacao)
        {
            if (respostaManifestacao.Success)
                manifesto.SituacaoManifesto = manifestarDTO.TipoManifestacao;
            if (respostaManifestacao.Data.InfEvento.CStat == "650")
                manifesto.SituacaoManifesto = ESituacaoManifesto.Cancelado;

            _repositoryFactory.Instancie<IManifestoRepository>().Atualizar(manifesto);

            if (manifesto.SituacaoManifesto == ESituacaoManifesto.Cancelado)
                CancelarDocumentoFiscal(manifesto);
        }

        private void CancelarDocumentoFiscal(Manifesto manifesto)
        {
            var documentoFiscal = _repositoryFactory.Instancie<IDocumentoFiscalRepository>().BuscarPorExpressao(c => c.ChaveDocumentoFiscal == manifesto.ChaveDocumentoFiscal);

            if (documentoFiscal != null)
            {
                documentoFiscal.Cancelado = true;

                _repositoryFactory.Instancie<IDocumentoFiscalRepository>().Atualizar(documentoFiscal);
            }
        }

        private bool EhValido(Manifesto manifesto)
        {
            var validator = new ManifestoValidator().Validate(manifesto);

            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));

            return validator.IsValid;
        }

        private string ObterTIpoManifestacao(ESituacaoManifesto tipoManifestacao)
        {
            switch (tipoManifestacao)
            {
                case ESituacaoManifesto.Ciencia:
                    return "210210";
                case ESituacaoManifesto.Confirmado:
                    return "210200";
                case ESituacaoManifesto.Desconhecido:
                    return "210240";
                case ESituacaoManifesto.NaoRealizado:
                    return "210220";
                case ESituacaoManifesto.Cancelado:
                    return "110111";
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}
