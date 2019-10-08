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
        public DadosOrigemDTO DadosOrigem { get; set; }
        public DadosFornecedorDTO DadosFornecedor { get; set; }
        public DadosRetornoDTO DadosManifestacao { get; set; }
        public DadosRetornoDTO DadosDownload { get; set; }
    }

    public class DadosOrigemDTO
    {
        public OrigemManifesto Origem { get; set; }
        public string Originador { get; set; }
    }

    public class DadosFornecedorDTO
    {
        public string Inscricao { get; set; }
        public string RazaoSocial { get; set; }
    }

    public class DadosRetornoDTO
    {
        public string CodigoRetorno { get; set; }
        public string Motivo { get; set; }
        public bool Rejeitado { get; set; }
    }
}
