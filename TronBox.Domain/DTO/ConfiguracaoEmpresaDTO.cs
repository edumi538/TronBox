using Comum.Domain.Enums;
using System.Collections.Generic;
using TronBox.Domain.Enums;

namespace TronBox.Domain.DTO
{
    public class ConfiguracaoEmpresaDTO
    {
        public string Id { get; set; }
        public bool SalvarCteEntrada { get; set; }
        public bool SalvarCteSaida { get; set; }
        public bool ManifestarAutomaticamente { get; set; }
        public bool SalvarSomenteManifestadas { get; set; }
        public EMetodoBusca MetodoBusca { get; set; }
        public EEstruturaDownload EstruturaDownload { get; set; }
        public DadosMatoGrossoDTO DadosMatoGrosso { get; set; }
        public DadosMatoGrossoSulDTO DadosMatoGrossoSul { get; set; }
        public IEnumerable<InscricaoComplementarDTO> InscricoesComplementares { get; set; }
    }

    public class DadosMatoGrossoDTO
    {
        public ETipoAcessoMatoGrosso Tipo { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class DadosMatoGrossoSulDTO
    {
        public string Usuario { get; set; }
        public string CodigoAcesso { get; set; }
        public string Senha { get; set; }
    }

    public class InscricaoComplementarDTO
    {
        public string Id { get; set; }
        public string NomeFantasia { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string InscricaoEstadual { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public int CodigoCidade { get; set; }
        public string UF { get; set; }
        public bool ConsultaPortalEstadual { get; set; }
        public eSituacao Situacao { get; set; }
    }
}
