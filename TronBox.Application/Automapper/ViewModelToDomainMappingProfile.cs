using AutoMapper;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;

namespace TronBox.Domain.Automapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            #region Tenant

            CreateMap<TenantViewModel, Tenant>();

            #endregion

        }
    }
}
