using TronBox.Domain.Enums;

namespace TronBox.Domain.DTO
{
    public class ManifestarDTO
    {
        public string ChaveDocumentoFiscal { get; set; }
        public ESituacaoManifesto TipoManifestacao { get; set; }
        public string Justificativa { get; set; }
    }
}
