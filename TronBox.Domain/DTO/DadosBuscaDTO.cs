using TronBox.Domain.Enums;

namespace TronBox.Domain.DTO
{
    public class DadosBuscaDTO
    {
        public string Inscricao { get; set; }
        public bool ManifestarAutomaticamente { get; set; }
        public bool SalvarSomenteManifestadas { get; set; }
        public EMetodoBusca MetodoBusca { get; set; }
        public string UF { get; set; }
    }
}
