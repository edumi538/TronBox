using AutoMapper;
using Comum.Domain.ViewModels;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
using System.Collections.Generic;
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
            CreateMap<EmpresaViewModel, EmpresaDTO>();
            #endregion

            #region Listas
            CreateMap<List<EmpresaViewModel>, List<EmpresaDTO>>();
            #endregion
        }
    }
}
