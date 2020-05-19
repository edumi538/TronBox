using Newtonsoft.Json;

namespace TronBox.Domain.DTO
{
    public class DadosManifestacaoNFeDTO
    {
        [JsonProperty("ultNSU")]
        public string UltimoNSU { get; set; }
        public string UF { get; set; }
        [JsonProperty("previousInvoices")]       
        public string MetodoBusca { get; set; }
        [JsonProperty("consultType")]
        public int TipoConsulta { get; set; }
        [JsonProperty("autoManifest")]
        public bool ManifestarAutomaticamente { get; set; }
        [JsonProperty("saveOnlyManifestedInvoices")]
        public bool SalvarSomenteManifestadas { get; set; }
        [JsonProperty("limitConsults")]
        public bool LimitarConsulta { get; set; }
        [JsonProperty("tenantId")]
        public string Empresa { get; set; }

        public DadosManifestacaoNFeDTO(string ultimoNSU, string uf, string metodoBusca, int tipoConsulta, bool manifestarAutomaticamente, bool salvarSomenteManifestadas, bool limitarConsulta, string empresa)
        {
            UltimoNSU = ultimoNSU;
            UF = uf;
            MetodoBusca = metodoBusca;
            TipoConsulta = tipoConsulta;
            ManifestarAutomaticamente = manifestarAutomaticamente;
            SalvarSomenteManifestadas = salvarSomenteManifestadas;
            LimitarConsulta = limitarConsulta;
            Empresa = empresa;
        }
    }
}
