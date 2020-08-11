using AutoMapper;
using System;
using System.Collections.Generic;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.DadosComputadorMonitorAgg;
using TronBox.Domain.Aggregates.DadosComputadorMonitorAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class DadosComputadorMonitorAppService : IDadosComputadorMonitorAppService
    {
        #region Membros
        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;
        #endregion

        #region Construtor
        public DadosComputadorMonitorAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
        #endregion

        public void Dispose()
        {
        }

        public IEnumerable<DadosComputadorMonitorDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<DadosComputadorMonitorDTO>>(_repositoryFactory.Instancie<IDadosComputadorMonitorRepository>()
            .BuscarTodos(new UtilitarioSpecification<DadosComputadorMonitor>().CriarEspecificacaoFiltro(filtro)));

        public void Inserir(DadosComputadorMonitorDTO dadosComputadorMonitorDTO)
        {
            if (dadosComputadorMonitorDTO == null)
            {
                _bus.RaiseEvent(new DomainNotification("DadosComputadorMonitor", "Dados do Computador do Monitor enviado não informado ou está inválido."));
                return;
            }

            if (dadosComputadorMonitorDTO.Id == null)
                dadosComputadorMonitorDTO.Id = Guid.NewGuid().ToString();

            var dadosComputadorMonitor = _mapper.Map<DadosComputadorMonitor>(dadosComputadorMonitorDTO);

            if (EhValido(dadosComputadorMonitor)) _repositoryFactory.Instancie<IDadosComputadorMonitorRepository>().Inserir(dadosComputadorMonitor);
        }

        #region Private Methods
        private bool EhValido(DadosComputadorMonitor dadosComputadorMonitor)
        {
            var validator = new DadosComputadorMonitorValidator().Validate(dadosComputadorMonitor);

            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));

            return validator.IsValid;
        }
        #endregion
    }
}
