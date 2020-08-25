using Newtonsoft.Json;

namespace TronBox.Domain.DTO
{
    public class DadosManifestacaoCTeDTO
    {
        [JsonProperty("inscricaoEmpresa")]
        public string InscricaoEmpresa { get; set; }
        [JsonProperty("ultimoNsu")]
        public string UltimoNSU { get; set; }
        [JsonProperty("uf")]
        public string UF { get; set; }
        [JsonProperty("tipoConsulta")]
        public int TipoConsulta { get; set; }
        [JsonProperty("buscarMesAtual")]
        public bool BuscarSomenteMesAtual { get; set; }
        [JsonProperty("tenantId")]
        public string Empresa { get; set; }

        public DadosManifestacaoCTeDTO(string inscricaoEmpresa, string ultimoNSU, string uf, int tipoConsulta, bool buscarSomenteMesAtual, string empresa)
        {
            InscricaoEmpresa = inscricaoEmpresa;
            UltimoNSU = ultimoNSU;
            UF = uf;
            TipoConsulta = tipoConsulta;
            BuscarSomenteMesAtual = buscarSomenteMesAtual;
            Empresa = empresa;
        }
    }
}
