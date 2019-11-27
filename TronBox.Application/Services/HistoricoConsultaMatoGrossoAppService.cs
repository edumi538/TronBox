using AutoMapper;
using System.Collections.Generic;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class HistoricoConsultaMatoGrossoAppService : IHistoricoConsultaMatoGrossoAppService
    {
        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public HistoricoConsultaMatoGrossoAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public IEnumerable<HistoricoConsultaMatoGrossoDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<HistoricoConsultaMatoGrossoDTO>>(_repositoryFactory
            .Instancie<IHistoricoConsultaMatoGrossoRepository>().BuscarTodos(new UtilitarioSpecification<HistoricoConsultaMatoGrosso>().CriarEspecificacaoFiltro(filtro)));

        public void Dispose()
        {
        }

        public void Inserir(HistoricoConsultaMatoGrossoDTO historicoConsulta)
        {
            var historico = _mapper.Map<HistoricoConsultaMatoGrosso>(historicoConsulta);

            if (EhValido(historico)) _repositoryFactory.Instancie<IHistoricoConsultaMatoGrossoRepository>().Inserir(historico);
        }

        #region Private Methods
        private bool EhValido(HistoricoConsultaMatoGrosso historico)
        {
            var validator = new HistoricoConsultaMatoGrossoValidator().Validate(historico);

            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));

            return validator.IsValid;
        }
        #endregion
    }
}
