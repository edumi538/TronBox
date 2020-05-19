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
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class HistoricoConsultaAppService : IHistoricoConsultaAppService
    {


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
