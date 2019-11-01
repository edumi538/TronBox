using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
using System.Collections.Generic;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg;
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
            CreateMap<Empresa, EmpresaDTO>();
            CreateMap<ConfiguracaoEmpresa, ConfiguracaoEmpresaDTO>();
            CreateMap<DocumentoFiscal, DocumentoFiscalDTO>();
            CreateMap<HistoricoConsulta, HistoricoConsultaDTO>();
            CreateMap<Manifesto, ManifestoDTO>();
            #endregion

            #region Listas
            CreateMap<List<Empresa>, List<EmpresaDTO>>();
            CreateMap<List<ConfiguracaoEmpresa>, List<ConfiguracaoEmpresaDTO>>();
            CreateMap<List<DocumentoFiscal>, List<DocumentoFiscalDTO>>();
            CreateMap<List<HistoricoConsulta>, List<HistoricoConsultaDTO>>();
            CreateMap<List<Manifesto>, List<ManifestoDTO>>();
            #endregion
        }
    }
}
