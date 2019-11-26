using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
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
        //public static string URL_AGENTE_MANIFESTACAO = "http://192.168.10.229:8082";
        public static string URL_AGENTE_MANIFESTACAO = "http://10.20.30.28:8085";

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

        public void BuscarManualmente()
        {
            var empresa = _mapper.Map<EmpresaDTO>(_repositoryFactory.Instancie<IEmpresaRepository>().BuscarTodos().FirstOrDefault());

            var configuracaoEmpresa = _mapper.Map<ConfiguracaoEmpresaDTO>(_repositoryFactory.Instancie<IConfiguracaoEmpresaRepository>()
                .BuscarTodos().FirstOrDefault());

            if (empresa.UF == null || configuracaoEmpresa == null)
            {
                _bus.RaiseEvent(new DomainNotification("ConfiguracaoIncompleta", "O cadastro da empresa está incompleto."));
                return;
            }

            var dadosBusca = new
            {
                ultNSU = "0",
                autoManifest = configuracaoEmpresa.ManifestarAutomaticamente,
                empresa.UF,
                saveOnlyManifestedInvoices = configuracaoEmpresa.SalvarSomenteManifestadas,
                previousInvoices = configuracaoEmpresa.MetodoBusca == EMetodoBusca.UltimosMeses ? "last_three_months" : "current_month",
                typeConsult = ETipoConsulta.Manual,
                limitConsults = false
            };

            UtilitarioHttpClient.PostRequest(string.Empty, URL_AGENTE_MANIFESTACAO, $"mdf-e/send-nsu/registry/{configuracaoEmpresa.Inscricao}", dadosBusca);
        }

        #region Private Methods
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
