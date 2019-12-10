using Comum.Domain.Enums;
using Comum.DTO;

namespace TronBox.Domain.DTO
{
    public class EmpresaDTO
    {
        public string Id { get; set; }
        public eTipoInscricaoEmpresa TipoInscricao { get; set; }
        public string Inscricao { get; set; }
        public string RazaoSocial { get; set; }
        public string EmailPrincipal { get; set; }
        public ConfiguracaoEmpresaDTO ConfiguracaoEmpresa { get; set; }
        public CertificadoSimplificadoDTO Certificado { get; set; }
    }
}
