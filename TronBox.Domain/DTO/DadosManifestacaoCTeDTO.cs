using Newtonsoft.Json;

namespace TronBox.Domain.DTO
{
    public class DadosManifestacaoCTeDTO
    {
        [JsonProperty("nsu")]
        public string UltimoNSU { get; set; }
        public string UF { get; set; }
        [JsonProperty("consultType")]
        public int TipoConsulta { get; set; }
        [JsonProperty("current_month_only")]       
        public bool BuscarSomenteMesAtual { get; set; }

        public DadosManifestacaoCTeDTO(string ultimoNSU, string uf, int tipoConsulta, bool buscarSomenteMesAtual)
        {
            UltimoNSU = ultimoNSU;
            UF = uf;
            TipoConsulta = tipoConsulta;
            BuscarSomenteMesAtual = buscarSomenteMesAtual;
        }
    }
}
