using AutoMapper;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
using System.Collections.Generic;
using TronBox.Domain.Aggregates.CustomerAgg;
using TronBox.Domain.DTO;

namespace TronBox.Domain.Automapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            #region Tenant
            CreateMap<Tenant, TenantViewModel>();
            #endregion

            #region Modelos
            CreateMap<Customer, CustomerDTO>();
            #endregion

            #region Listas
            CreateMap<List<Customer>, List<CustomerDTO>>();
            #endregion
        }
    }
}
