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
        public string NomeFantasia { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string EmailPrincipal { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }
        public int CodigoCidade { get; set; }
        public string UF { get; set; }
        public ConfiguracaoEmpresaDTO ConfiguracaoEmpresa { get; set; }
        public CertificadoSimplificadoDTO Certificado { get; set; }
    }
}
