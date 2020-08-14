using Newtonsoft.Json;

namespace TronBox.Domain.DTO
{
    public class DadosBuscaMatoGrossoSulDTO
    {
        [JsonProperty("id")]
        public string Empresa { get; set; }
        [JsonProperty("nome")]
        public string Nome { get; set; }
        [JsonProperty("inscricaoEmpresa")]       
        public string InscricaoEmpresa { get; set; }
        [JsonProperty("dataInicial")]
        public int DataInicial { get; set; }
        [JsonProperty("dataFinal")]
        public int DataFinal { get; set; }
        [JsonProperty("tipoConsulta")]
        public int TipoConsulta { get; set; }
        [JsonProperty("dadosMatoGrossoSul")]
        public DadosAcessoMatoGrossoSulDTO DadosMatoGrossoSul { get; set; }

        public class DadosAcessoMatoGrossoSulDTO
        {
            [JsonProperty("usuario")]
            public string Usuario { get; set; }
            [JsonProperty("codigoAcesso")]
            public string CodigoAcesso { get; set; }
            [JsonProperty("senha")]
            public string Senha { get; set; }

            public DadosAcessoMatoGrossoSulDTO(string usuario, string codigoAcesso, string senha)
            {
                Usuario = usuario;
                CodigoAcesso = codigoAcesso;
                Senha = senha;
            }
        }

        public DadosBuscaMatoGrossoSulDTO(string empresa, string nome, string inscricaoEmpresa, int dataInicial, int dataFinal, int tipoConsulta, string usuario, string codigoAcesso, string senha)
        {
            Empresa = empresa;
            Nome = nome;
            InscricaoEmpresa = inscricaoEmpresa;
            DataInicial = dataInicial;
            DataFinal = dataFinal;
            TipoConsulta = tipoConsulta;
            DadosMatoGrossoSul = new DadosAcessoMatoGrossoSulDTO(usuario, codigoAcesso, senha);
        }
    }
}
