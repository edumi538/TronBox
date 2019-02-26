using AutoMapper;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
using TronBox.Domain.Aggregates.CustomerAgg;
using TronBox.Domain.DTO;

namespace TronBox.Domain.Automapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            #region Tenant

            CreateMap<TenantViewModel, Tenant>();

            #endregion

            CreateMap<CustomerDTO, Customer>();

        }
    }
}
