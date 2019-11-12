namespace TronBox.Domain.Classes.NFSe
{
    public class TomadorGenerico
    {
        public IdentificacaoTomador IdentificacaoTomador { get; set; }
        public string RazaoSocial { get; set; }
        public Endereco Endereco { get; set; }
        public Contato Contato { get; set; }
    }

    public class IdentificacaoTomador
    {
        public CpfCnpj CpfCnpj { get; set; }
    }
}
