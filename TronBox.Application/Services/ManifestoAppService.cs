using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using Newtonsoft.Json;
using Sentinela.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
using TronCore.InjecaoDependencia;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class ManifestoAppService : IManifestoAppService
    {
#if DEBUG
        public static string URL_AGENTE_MANIFESTACAO = "http://10.20.30.33:5001";
#else
        public static string URL_AGENTE_MANIFESTACAO = "http://10.20.30.33:3001";
#endif

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

        public ManifestoDTO BuscarPorId(Guid id) => _mapper.Map<ManifestoDTO>(_repositoryFactory.Instancie<IManifestoRepository>().BuscarPorId(id));

        public IEnumerable<ManifestoDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<ManifestoDTO>>(_repositoryFactory.Instancie<IManifestoRepository>()
            .BuscarTodos(new UtilitarioSpecification<Manifesto>().CriarEspecificacaoFiltro(filtro)));

        public IEnumerable<ManifestoDTO> BuscarPorChaves(IEnumerable<string> chaves) => _mapper.Map<IEnumerable<ManifestoDTO>>(_repositoryFactory.Instancie<IManifestoRepository>()
            .BuscarTodos(c => chaves.Contains(c.ChaveDocumentoFiscal)));

        public void Deletar(Guid id) => _repositoryFactory.Instancie<IManifestoRepository>().Excluir(id);

        public void DeletarDuplicados()
        {
            var manifestos = _mapper.Map<IEnumerable<ManifestoDTO>>(_repositoryFactory.Instancie<IManifestoRepository>().BuscarTodos());

            var listaDuplicados = manifestos.GroupBy(c => c.ChaveDocumentoFiscal).Where(c => c.Count() > 1);

            foreach (var duplicados in listaDuplicados)
            {
                var manifestoMantido = duplicados.Last();

                foreach (var manifesto in duplicados)
                    if (manifesto.Id != manifestoMantido.Id) Deletar(Guid.Parse(manifesto.Id));
            }
        }

        public int InserirOuAtualizar(IEnumerable<dynamic> manifestosDTO)
        {
            if (manifestosDTO == null)
            {
                _bus.RaiseEvent(new DomainNotification("Manifesto", "Manifestos não informados ou inválidos."));
                return 0;
            }

            var inseridos = 0;

            var listaChaves = manifestosDTO.Select(c => c.chaveDocumentoFiscal);

            var manifestosExistentes = _mapper.Map<IEnumerable<ManifestoDTO>>(_repositoryFactory.Instancie<IManifestoRepository>()
                .BuscarTodos(c => listaChaves.Contains(c.ChaveDocumentoFiscal)));

            foreach (var manifestoDTO in manifestosDTO)
            {
                var manifestoExistente = manifestosExistentes.FirstOrDefault(c => c.ChaveDocumentoFiscal == manifestoDTO.chaveDocumentoFiscal.Value);

                if (manifestoExistente == null)
                    Inserir(JsonConvert.DeserializeObject<ManifestoDTO>(JsonConvert.SerializeObject(manifestoDTO)));
                else
                    Atualizar(manifestoExistente, manifestoDTO);

                inseridos += manifestoExistente == null ? 1 : 0;
            }

            return inseridos;
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

            var manifesto = _repositoryFactory.Instancie<IManifestoRepository>().BuscarPorExpressao(c => c.ChaveDocumentoFiscal == manifestarDTO.ChaveDocumentoFiscal);

            if (manifesto != null && (manifesto.SituacaoManifesto != ESituacaoManifesto.Cancelado && !(manifestarDTO.TipoManifestacao == ESituacaoManifesto.Ciencia && manifesto.SituacaoManifesto == ESituacaoManifesto.CienciaAutomatica)))
            {
                var tenantId = FabricaGeral.Instancie<ITenantProvider>().GetTenant().Id.ToString();

                var manifestacao = new DadosManifestacaoNFeDTO(manifestarDTO.ChaveDocumentoFiscal, empresa.Inscricao, configuracaoEmpresa.InscricoesComplementares.FirstOrDefault().UF,
                    ObterTipoManifestacao(manifestarDTO.TipoManifestacao), tenantId);

                var result = await UtilitarioHttpClient.PostRequest(string.Empty, URL_AGENTE_MANIFESTACAO, "api/nfes/manifestar", manifestacao);

                var respostaManifestacao = JsonConvert.DeserializeObject<RespostaManifestacaoDTO>(result);

                if (respostaManifestacao != null)
                    AtualizarManifesto(manifestarDTO, manifesto, respostaManifestacao);

                return respostaManifestacao;
            }

            return null;
        }

        #region Private Methods
        private void Atualizar(ManifestoDTO manifestoExistente, dynamic manifestoDTO)
        {
            var manifesto = _mapper.Map<Manifesto>(AjudanteJsonPatch.Instancia.ApplyPatch(manifestoExistente, manifestoDTO));

            // Quando o manifesto está cancelado não permitido alterar a situação.
            if (manifestoExistente.SituacaoManifesto == ESituacaoManifesto.Cancelado && manifesto.SituacaoManifesto != manifestoExistente.SituacaoManifesto)
                manifesto.SituacaoManifesto = manifestoExistente.SituacaoManifesto;
            // Quando o manifesto existente é Ciência Automática e for enviado 
            // evento de Ciência não é permitido alterar a situação para Ciência.
            if ((manifesto.SituacaoManifesto == ESituacaoManifesto.Ciencia) && (manifestoExistente.SituacaoManifesto == ESituacaoManifesto.CienciaAutomatica))
                manifesto.SituacaoManifesto = ESituacaoManifesto.CienciaAutomatica;

            if (EhValido(manifesto)) _repositoryFactory.Instancie<IManifestoRepository>().Atualizar(manifesto);
        }

        private void Inserir(ManifestoDTO manifestoDTO)
        {
            if (manifestoDTO.Id == null)
                manifestoDTO.Id = Guid.NewGuid().ToString();

            var manifesto = _mapper.Map<Manifesto>(manifestoDTO);

            if (EhValido(manifesto)) _repositoryFactory.Instancie<IManifestoRepository>().Inserir(manifesto);
        }

        private void AtualizarManifesto(ManifestarDTO manifestarDTO, Manifesto manifesto, RespostaManifestacaoDTO respostaManifestacao)
        {
            if (respostaManifestacao.Sucesso)
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

        private string ObterTipoManifestacao(ESituacaoManifesto tipoManifestacao)
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
