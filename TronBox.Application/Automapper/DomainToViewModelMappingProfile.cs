using AutoMapper;
using Comum.Domain.ViewModels;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
using System.Collections.Generic;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.DTO;

namespace TronBox.Domain.Automapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Tenant, TenantViewModel>();

            #region Modelos
            CreateMap<EmpresaViewModel, EmpresaDTO>();
            CreateMap<ConfiguracaoEmpresa, ConfiguracaoEmpresaDTO>();
            CreateMap<Manifesto, ManifestoDTO>();
            #endregion

            #region Listas
            CreateMap<List<EmpresaViewModel>, List<EmpresaDTO>>();
            CreateMap<List<ConfiguracaoEmpresa>, List<ConfiguracaoEmpresaDTO>>();
            CreateMap<List<Manifesto>, List<ManifestoDTO>>();
            #endregion
        }
    }
}
