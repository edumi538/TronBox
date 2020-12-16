using Newtonsoft.Json;

namespace TronBox.Domain.DTO
{
    public class DadosManifestacaoNFeDTO
    {
        [JsonProperty("chaveDocumentoFiscal")]
        public string ChaveDocumentoFiscal { get; set; }
        [JsonProperty("inscricaoEmpresa")]
        public string InscricaoEmpresa { get; set; }
        [JsonProperty("uf")]
        public string UF { get; set; }
        [JsonProperty("tpEvent")]
        public string TipoManifestacao { get; set; }
        [JsonProperty("tenantId")]
        public string Empresa { get; set; }
        [JsonProperty("justificativa")]
        public string Justificativa { get; set; }
    }
}
