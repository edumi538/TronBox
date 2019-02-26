using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.CustomerAgg;
using TronBox.Domain.Aggregates.CustomerAgg.Repository;
using TronBox.Domain.DTO;
using TronCore.Dominio.Bus;
using TronCore.Dominio.Specifications;
using TronCore.Persistencia.Interfaces;
using TronCore.Utilitarios.Specifications;

namespace TronBox.Application.Services
{
    public class CustomerAppService : ICustomerAppService
    {

        private readonly IBus _bus;
        private readonly IMapper _mapper;
        private readonly IRepositoryFactory _repositoryFactory;

        public CustomerAppService(IBus bus, IMapper mapper, IRepositoryFactory repositoryFactory)
        {
            _bus = bus;
            _mapper = mapper;
            _repositoryFactory = repositoryFactory;
        }
               
        public void Atualizar(CustomerDTO customerDTO)
        {
            var customer = _mapper.Map<Customer>(customerDTO);
            _repositoryFactory.Instancie<ICustomerRepository>().Atualizar(customer);
        }

        public IEnumerable<CustomerDTO> BuscarTodos(string filtro)
        {
            Specification<Customer> spec = new UtilitarioSpecification<Customer>().CriarEspecificacaoFiltro(filtro);
            return _mapper.Map<IEnumerable<CustomerDTO>>(_repositoryFactory.Instancie<ICustomerRepository>().BuscarTodos(spec));
        }

        public void Deletar(Guid id)
        {
            _repositoryFactory.Instancie<ICustomerRepository>().Excluir(id);
        }

        public void Dispose()
        {            
        }

        public void Inserir(CustomerDTO customerDTO)
        {
            var customer = _mapper.Map<Customer>(customerDTO);
            _repositoryFactory.Instancie<ICustomerRepository>().Inserir(customer);
        }
    }
}
