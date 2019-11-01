using MongoDB.Bson.Serialization.Attributes;
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
        [BsonIgnoreIfDefault]
        public int DataManifesto { get; set; }
        public ESituacaoManifesto SituacaoManifesto { get; set; }
        [BsonIgnoreIfDefault]
        public ESituacaoDocumentoFiscal SituacaoDocumentoFiscal { get; set; }
        [BsonIgnoreIfDefault]
        public bool Cancelado { get; set; }
        [BsonIgnoreIfDefault]
        public bool Rejeitado { get; set; }
        public DadosOrigemManifestoDTO DadosOrigem { get; set; }
        [BsonIgnoreIfDefault]
        public DadosFornecedorDTO DadosFornecedor { get; set; }
        [BsonIgnoreIfDefault]
        public DadosRetornoDTO DadosManifestacao { get; set; }
        [BsonIgnoreIfDefault]
        public DadosRetornoDTO DadosDownload { get; set; }
    }
}
