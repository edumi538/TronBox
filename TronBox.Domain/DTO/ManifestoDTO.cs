using System;
using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.Enums;
using TronCore.Enumeradores.Helpers;
using TronCore.Utilitarios;

namespace TronBox.Domain.DTO
{
    public class ManifestoDTO
    {
        public string Id { get; set; }
        public string ChaveDocumentoFiscal { get; set; }
        public string NumeroDocumentoFiscal { get; set; }
        public double ValorDocumentoFiscal { get; set; }
        public int DataArmazenamento { get; set; }
        public int DataEmissaoManifesto { get; set; }
        public int DataManifesto { get; set; }
        public ESituacaoManifesto SituacaoManifesto { get; set; }
        public ESituacaoDocumentoFiscal SituacaoDocumentoFiscal { get; set; }
        public DadosOrigemManifestoDTO DadosOrigem { get; set; }
        public DadosFornecedorDTO DadosFornecedor { get; set; }
        public DateTime DataArmazenamentoFormatada
        {
            get => UtilitarioDatas.ConvertIntToDate(DataArmazenamento);
        }
        public DateTime DataEmissaoManifestoFormatada
        {
            get => UtilitarioDatas.ConvertIntToDate(DataEmissaoManifesto);
        }
        public DateTime DataManifestoFormatada
        {
            get => UtilitarioDatas.ConvertIntToDate(DataManifesto);
        }
        public string DescricaoSituacaoManifesto
        {
            get => EnumHelper<ESituacaoManifesto>.GetDisplayValue(SituacaoManifesto);
        }
    }
}
