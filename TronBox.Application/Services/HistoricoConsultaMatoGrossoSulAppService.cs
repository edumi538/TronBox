using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoSulAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoSulAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class HistoricoConsultaMatoGrossoSulAppService : IHistoricoConsultaMatoGrossoSulAppService
    {
        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public HistoricoConsultaMatoGrossoSulAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public IEnumerable<HistoricoConsultaMatoGrossoSulDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<HistoricoConsultaMatoGrossoSulDTO>>(_repositoryFactory
            .Instancie<IHistoricoConsultaMatoGrossoSulRepository>().BuscarTodos(new UtilitarioSpecification<HistoricoConsultaMatoGrossoSul>().CriarEspecificacaoFiltro(filtro)));

        public void Dispose()
        {
        }
        public DateTime? ObterUltimoPeriodo()
        {
            var historicoConsulta = _mapper.Map<HistoricoConsultaMatoGrossoSulDTO>(_repositoryFactory.Instancie<IHistoricoConsultaMatoGrossoSulRepository>()
                .BuscarTodos().OrderByDescending(c => c.DataFinalConsultada).Take(1).FirstOrDefault());

            if (historicoConsulta != null)
                return historicoConsulta.DataFinalConsultadaFormatada;

            return null;
        }

        public void Inserir(HistoricoConsultaMatoGrossoSulDTO historicoConsulta)
        {
            if (historicoConsulta == null)
            {
                _bus.RaiseEvent(new DomainNotification("HistoricoConsultaMatoGrossoSul", "Histórico de Consulta do Mato Groso do Sul enviado não informado ou está inválido."));
                return;
            }

            if (historicoConsulta.Id == null)
                historicoConsulta.Id = Guid.NewGuid().ToString();

            var historico = _mapper.Map<HistoricoConsultaMatoGrossoSul>(historicoConsulta);

            if (EhValido(historico)) _repositoryFactory.Instancie<IHistoricoConsultaMatoGrossoSulRepository>().Inserir(historico);
        }

        #region Private Methods
        private bool EhValido(HistoricoConsultaMatoGrossoSul historico)
        {
            var validator = new HistoricoConsultaMatoGrossoSulValidator().Validate(historico);

            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));

            return validator.IsValid;
        }
        #endregion
    }
}
