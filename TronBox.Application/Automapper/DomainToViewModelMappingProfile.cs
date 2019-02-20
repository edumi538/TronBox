using AutoMapper;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;

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
            
            #endregion

            #region Listas
            
            #endregion
        }
    }
}
