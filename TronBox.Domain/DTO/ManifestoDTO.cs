using TronBox.Domain.DTO.InnerClassDTO;
using TronBox.Domain.Enums;

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
        public SituacaoManifesto SituacaoManifesto { get; set; }
        public SituacaoDocumentoFiscal SituacaoDocumentoFiscal { get; set; }
        public bool Cancelado { get; set; }
        public bool Rejeitado { get; set; }
        public DadosOrigemManifestoDTO DadosOrigem { get; set; }
        public DadosFornecedorDTO DadosFornecedor { get; set; }
        public DadosRetornoDTO DadosManifestacao { get; set; }
        public DadosRetornoDTO DadosDownload { get; set; }
    }
}
