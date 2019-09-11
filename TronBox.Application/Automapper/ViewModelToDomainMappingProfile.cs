using AutoMapper;
using Comum.Domain.ViewModels;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
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

            CreateMap<EmpresaDTO, EmpresaViewModel> ();
        }
    }
}
