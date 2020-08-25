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

        public DadosManifestacaoNFeDTO(string chaveDocumentoFiscal, string inscricaoEmpresa, string uF, string tipoManifestacao, string empresa)
        {
            ChaveDocumentoFiscal = chaveDocumentoFiscal;
            InscricaoEmpresa = inscricaoEmpresa;
            UF = uF;
            TipoManifestacao = tipoManifestacao;
            Empresa = empresa;
        }
    }
}
