using Comum.Domain.Enums;
using System.Collections.Generic;
using TronBox.Domain.Enums;

namespace TronBox.Domain.DTO
{
    public class ConfiguracaoEmpresaDTO
    {
        public string Id { get; set; }
        public string Inscricao { get; set; }
        public bool PermiteEmpresaEditarPerfil { get; set; }
        public bool PermiteDownloadSemManifesto { get; set; }
        public ArquiteturaDownload ArquiteturaDownload { get; set; }
        public DadosMatoGrossoDTO DadosMatoGrosso { get; set; }
        public IEnumerable<InscricaoComplementarDTO> InscricoesComplementares { get; set; }
    }

    #region Dados Complementares
    public class DadosMatoGrossoDTO
    {
        public TipoAcessoMatoGrosso Tipo { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class InscricaoComplementarDTO
    {
        public bool ConsultaMatoGrosso { get; set; }
        public eSituacao Situacao { get; set; }
        public string InscricaoEstadual { get; set; }
        public bool SalvarCteEntrada { get; set; }
        public bool SalvarCteSaida { get; set; }
        public string UltimoNsuNfe { get; set; }
        public string UltimoNsuCTe { get; set; }
        public string ErroConsulta { get; set; }
        public DadosIcmsTransparenteDTO DadosIcmsTransparente { get; set; }
        public IEnumerable<DadosMunicipaisDTO> DadosMunicipais { get; set; }
    }

    public class DadosIcmsTransparenteDTO
    {
        public string CodigoAcesso { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }

    public class DadosMunicipaisDTO
    {
        public string InscricaoMunicipal { get; set; }
        public eSituacao Situacao { get; set; }
    }
    #endregion
}
