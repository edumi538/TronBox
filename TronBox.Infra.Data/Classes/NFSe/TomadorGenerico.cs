namespace TronBox.Infra.Data.Classes.NFSe
{
    public class TomadorGenerico
    {
        public IdentificacaoTomador IdentificacaoTomador { get; set; }
        public string RazaoSocial { get; set; }
    }

    public class IdentificacaoTomador
    {
        public CpfCnpj CpfCnpj { get; set; }
    }
}
