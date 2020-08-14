using AutoMapper;
using Comum.Domain.Aggregates.EmpresaAgg;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.DadosComputadorMonitorAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoSulAgg;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.DTO;
using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.InnerClass;

namespace TronBox.Domain.Automapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<EmpresaDTO, Empresa>();
            CreateMap<ConfiguracaoEmpresaDTO, ConfiguracaoEmpresa>();
            CreateMap<InscricaoComplementarDTO, InscricaoComplementar>();
            CreateMap<DocumentoFiscalDTO, DocumentoFiscal>();
            CreateMap<DadosComputadorMonitorDTO, DadosComputadorMonitor>();
            CreateMap<HistoricoConsultaDTO, HistoricoConsulta>();
            CreateMap<HistoricoConsultaMatoGrossoDTO, HistoricoConsultaMatoGrosso>();
            CreateMap<HistoricoConsultaMatoGrossoSulDTO, HistoricoConsultaMatoGrossoSul>();
            CreateMap<ManifestoDTO, Manifesto>();
            CreateMap<DadosMatoGrossoDTO, DadosMatoGrosso>();
            CreateMap<DadosMatoGrossoSulDTO, DadosMatoGrossoSul>();
            CreateMap<InscricaoComplementarDTO, InscricaoComplementar>();
            CreateMap<DadosOrigemManifestoDTO, DadosOrigemManifesto>();
            CreateMap<DadosFornecedorDTO, DadosFornecedor>();
            CreateMap<DadosImportacaoDTO, DadosImportacao>();
            CreateMap<DadosOrigemDocumentoFiscalDTO, DadosOrigemDocumentoFiscal>();
        }
    }
}
