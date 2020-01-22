using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
using System.Collections.Generic;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.DTO;
using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.InnerClass;

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
            CreateMap<InscricaoComplementar, InscricaoComplementarDTO>();
            CreateMap<DocumentoFiscal, DocumentoFiscalDTO>();
            CreateMap<HistoricoConsulta, HistoricoConsultaDTO>();
            CreateMap<HistoricoConsultaMatoGrosso, HistoricoConsultaMatoGrossoDTO>();
            CreateMap<Manifesto, ManifestoDTO>();
            CreateMap<DadosFornecedor, DadosFornecedorDTO>();
            CreateMap<DadosImportacao, DadosImportacaoDTO>();
            CreateMap<DadosOrigemDocumentoFiscal, DadosOrigemDocumentoFiscalDTO>();
            CreateMap<DadosMatoGrosso, DadosMatoGrossoDTO>();
            CreateMap<DadosOrigemManifesto, DadosOrigemManifestoDTO>();
            #endregion

            #region Listas
            CreateMap<List<Empresa>, List<EmpresaDTO>>();
            CreateMap<List<ConfiguracaoEmpresa>, List<ConfiguracaoEmpresaDTO>>();
            CreateMap<List<InscricaoComplementar>, List<InscricaoComplementarDTO>>();
            CreateMap<List<DocumentoFiscal>, List<DocumentoFiscalDTO>>();
            CreateMap<List<HistoricoConsultaMatoGrosso>, List<HistoricoConsultaMatoGrossoDTO>>();
            CreateMap<List<Manifesto>, List<ManifestoDTO>>();
            #endregion
        }
    }
}
