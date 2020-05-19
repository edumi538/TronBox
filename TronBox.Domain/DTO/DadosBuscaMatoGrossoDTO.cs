using Newtonsoft.Json;

namespace TronBox.Domain.DTO
{
    public class DadosBuscaMatoGrossoDTO
    {
        [JsonProperty("id")]
        public string Empresa { get; set; }
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonProperty("inscricaoEmpresa")]       
        public string InscricaoEmpresa { get; set; }
        [JsonProperty("inscricaoEstadual")]
        public string InscricaoEstadual { get; set; }
        [JsonProperty("dataInicial")]
        public int DataInicial { get; set; }
        [JsonProperty("dataFinal")]
        public int DataFinal { get; set; }
        [JsonProperty("tipoConsulta")]
        public int TipoConsulta { get; set; }
        [JsonProperty("dadosMatoGrosso")]
        public DadosAcessoMatoGrossoDTO DadosMatoGrosso { get; set; }

        public class DadosAcessoMatoGrossoDTO
        {
            [JsonProperty("usuario")]
            public string Usuario { get; set; }
            [JsonProperty("senha")]
            public string senha { get; set; }
            [JsonProperty("tipo")]
            public int Tipo { get; set; }

            public DadosAcessoMatoGrossoDTO(string usuario, string senha, int tipo)
            {
                Usuario = usuario;
                this.senha = senha;
                Tipo = tipo;
            }
        }

        public DadosBuscaMatoGrossoDTO(string empresa, string nome, string inscricaoEmpresa, string inscricaoEstadual, int dataInicial, int dataFinal, int tipoConsulta, string usuario, string senha, int tipo)
        {
            Empresa = empresa;
            Nome = nome;
            InscricaoEmpresa = inscricaoEmpresa;
            InscricaoEstadual = inscricaoEstadual;
            DataInicial = dataInicial;
            DataFinal = dataFinal;
            TipoConsulta = tipoConsulta;
            DadosMatoGrosso = new DadosAcessoMatoGrossoDTO(usuario, senha, tipo);
        }
    }
}
