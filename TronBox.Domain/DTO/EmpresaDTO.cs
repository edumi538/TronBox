using Comum.Domain.Enums;

namespace TronBox.Domain.DTO
{
    public class EmpresaDTO
    {
        public string Id { get; set; }
        public eTipoInscricaoEmpresa TipoInscricao { get; set; }
        public string Inscricao { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string EmailPrincipal { get; set; }        
        public bool Status { get; set; }
        public ConfiguracaoEmpresaDTO ConfiguracaoEmpresa { get; set; }
    }
}
