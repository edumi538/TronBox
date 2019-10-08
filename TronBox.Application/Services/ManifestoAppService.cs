using AutoMapper;
using System;
using System.Collections.Generic;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.Aggregates.ManifestoAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class ManifestoAppService : IManifestoAppService
    {
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

        public void Atualizar(ManifestoDTO manifestoDTO)
        {
            var manifesto = _mapper.Map<Manifesto>(manifestoDTO);

            if (EhValido(manifesto)) _repositoryFactory.Instancie<IManifestoRepository>().Atualizar(manifesto);
        }

        public ManifestoDTO BuscarPorId(Guid id) => _mapper.Map<ManifestoDTO>(_repositoryFactory.Instancie<IManifestoRepository>().BuscarPorId(id));

        public IEnumerable<ManifestoDTO> BuscarTodos(string filtro) => _mapper.Map<IEnumerable<ManifestoDTO>>(_repositoryFactory.Instancie<IManifestoRepository>()
            .BuscarTodos(new UtilitarioSpecification<Manifesto>().CriarEspecificacaoFiltro(filtro)));

        public void Deletar(Guid id) => _repositoryFactory.Instancie<IManifestoRepository>().Excluir(id);

        public void Inserir(ManifestoDTO manifestoDTO)
        {
            var manifesto = _mapper.Map<Manifesto>(manifestoDTO);

            if (EhValido(manifesto)) _repositoryFactory.Instancie<IManifestoRepository>().Inserir(manifesto);
        }

        #region Private Methods
        private bool EhValido(Manifesto manifesto)
        {
            var validator = new ManifestoValidator().Validate(manifesto);

            foreach (var error in validator.Errors)
                _bus.RaiseEvent(new DomainNotification(error.PropertyName, error.ErrorMessage));

            return validator.IsValid;
        }
        #endregion
    }
}
