using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg.Repository;
using TronBox.Domain.DTO;
using TronBox.Domain.Enums;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class HistoricoConsultaAppService : IHistoricoConsultaAppService
    {
        public static string URL_AGENTE_MANIFESTACAO_NFE = "http://10.20.30.28:8085";
        public static string URL_AGENTE_MANIFESTACAO_CTE = "http://10.20.30.28:8083";

        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public HistoricoConsultaAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public void Dispose()
        {
        }

        public IEnumerable<HistoricoConsultaDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<HistoricoConsultaDTO>>(_repositoryFactory.Instancie<IHistoricoConsultaRepository>()
            .BuscarTodos(new UtilitarioSpecification<HistoricoConsulta>().CriarEspecificacaoFiltro(filtro)));

        public void Inserir(HistoricoConsultaDTO historicoConsulta)
        {
            if (historicoConsulta == null)
            {
                _bus.RaiseEvent(new DomainNotification("HistoricoConsulta", "Histórico de Consulta enviado não informado ou está inválido."));
                return;
            }

            if (historicoConsulta.Id == null)
                historicoConsulta.Id = Guid.NewGuid().ToString();

            var historico = _mapper.Map<HistoricoConsulta>(historicoConsulta);

            if (EhValido(historico)) _repositoryFactory.Instancie<IHistoricoConsultaRepository>().Inserir(historico);
        }

        public HistoricoConsultaDTO ObterUltimaConsulta() => _mapper.Map<HistoricoConsultaDTO>(_repositoryFactory.Instancie<IHistoricoConsultaRepository>()
            .BuscarTodos().OrderByDescending(c => c.DataHoraConsulta).Take(1).FirstOrDefault());

        public string ObterUltimoNSU(ETipoDocumentoConsulta tipoDocumento)
        {
            var historicoConsulta = _mapper.Map<HistoricoConsultaDTO>(_repositoryFactory.Instancie<IHistoricoConsultaRepository>()
                .BuscarTodos(c => c.TipoDocumentoConsulta == tipoDocumento).OrderByDescending(c => Convert.ToInt32(c.UltimoNSU)).Take(1).FirstOrDefault());

            return historicoConsulta != null ? historicoConsulta.UltimoNSU : "0";
        }

        public void BuscarManualmente(ETipoDocumentoConsulta tipo, DadosBuscaDTO dadosBuscaDTO)
        {
            if (tipo == ETipoDocumentoConsulta.NFe)
                RealizarBuscaNFe(dadosBuscaDTO);
            if (tipo == ETipoDocumentoConsulta.CTe)
                RealizarBuscaCTe(dadosBuscaDTO);
        }


        #region Private Methods
        private static void RealizarBuscaNFe(DadosBuscaDTO dadosBuscaDTO)
        {
            var dadosBusca = new DadosManifestacaoNFeDTO("0", dadosBuscaDTO.UF, dadosBuscaDTO.MetodoBusca == EMetodoBusca.UltimosMeses ? "last_three_months" : "current_month",
                (int)ETipoConsulta.Manual, dadosBuscaDTO.ManifestarAutomaticamente, dadosBuscaDTO.SalvarSomenteManifestadas, false);

            UtilitarioHttpClient.PostRequest(string.Empty, URL_AGENTE_MANIFESTACAO_NFE, $"mdf-e/send-nsu/registry/{dadosBuscaDTO.Inscricao}", dadosBusca);
        }

        private static void RealizarBuscaCTe(DadosBuscaDTO dadosBuscaDTO)
        {
            var dadosBusca = new DadosManifestacaoCTeDTO("0", dadosBuscaDTO.UF, (int)ETipoConsulta.Manual, dadosBuscaDTO.MetodoBusca == EMetodoBusca.MesAtual);

            UtilitarioHttpClient.PostRequest(string.Empty, URL_AGENTE_MANIFESTACAO_CTE, $"cte/customer/{dadosBuscaDTO.Inscricao}/new-documents", dadosBusca);
        }

        private bool EhValido(HistoricoConsulta historico)
        {
            var validator = new HistoricoConsultaValidator().Validate(historico);

            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));

            return validator.IsValid;
        }
        #endregion
    }
}
