﻿using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg;
using Sentinela.Domain.Aggregates.TenantAgg;
using Sentinela.Domain.DTO.ViewModels;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.DTO;

namespace TronBox.Domain.Automapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<TenantViewModel, Tenant>();

            CreateMap<EmpresaDTO, Empresa>();
            CreateMap<ConfiguracaoEmpresaDTO, ConfiguracaoEmpresa>();
            CreateMap<DocumentoFiscalDTO, DocumentoFiscal>();
            CreateMap<HistoricoConsultaDTO, HistoricoConsulta>();
            CreateMap<HistoricoConsultaMatoGrossoDTO, HistoricoConsultaMatoGrosso>();
            CreateMap<ManifestoDTO, Manifesto>();
        }
    }
}
