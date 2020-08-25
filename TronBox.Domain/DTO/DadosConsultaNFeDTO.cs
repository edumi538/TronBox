using Newtonsoft.Json;

namespace TronBox.Domain.DTO
{
    public class DadosConsultaNFeDTO
    {
        [JsonProperty("inscricaoEmpresa")]
        public string InscricaoEmpresa { get; set; }
        [JsonProperty("ultimoNsu")]
        public string UltimoNSU { get; set; }
        [JsonProperty("manifestarAutomaticamente")]       
        public bool ManifestarAutomaticamente { get; set; }
        [JsonProperty("uf")]
        public string UF { get; set; }
        [JsonProperty("buscarMesAtual")]
        public bool BuscarMesAtual { get; set; }
        [JsonProperty("salvarSomenteManifestadas")]
        public bool SalvarSomenteManifestadas { get; set; }
        [JsonProperty("limitarConsulta")]
        public bool LimitarConsulta { get; set; }
        [JsonProperty("tipoConsulta")]
        public int TipoConsulta { get; set; }
        [JsonProperty("tenantId")]
        public string Empresa { get; set; }

        public DadosConsultaNFeDTO(string inscricaoEmpresa, string ultimoNSU, bool manifestarAutomaticamente, string uF, bool buscarMesAtual, bool salvarSomenteManifestadas, bool limitarConsulta, int tipoConsulta, string empresa)
        {
            InscricaoEmpresa = inscricaoEmpresa;
            UltimoNSU = ultimoNSU;
            ManifestarAutomaticamente = manifestarAutomaticamente;
            UF = uF;
            BuscarMesAtual = buscarMesAtual;
            SalvarSomenteManifestadas = salvarSomenteManifestadas;
            LimitarConsulta = limitarConsulta;
            TipoConsulta = tipoConsulta;
            Empresa = empresa;
        }
    }
}
