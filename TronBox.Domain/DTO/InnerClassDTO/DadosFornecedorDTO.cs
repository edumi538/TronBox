using TronCore.Dominio.Base;

namespace TronBox.Domain.DTO.InnerClassDTO
{
    public class DadosFornecedorDTO
    {
        public string Inscricao { get; set; }
        public string RazaoSocial { get; set; }
        public string InscricaoRazaoSocialFormatada
        {
            get => $"{Inscricao.AdicionarMascaraInscricao()} - {RazaoSocial}";
        }
    }
}
