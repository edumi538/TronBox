namespace TronBox.Infra.Data.Classes.NFSe
{
    public class PrestadorGenerico
    {
        public IdentificacaoPrestador IdentificacaoPrestador { get; set; }
        public string RazaoSocial { get; set; }
    }

    public class IdentificacaoPrestador
    {
        public CpfCnpj CpfCnpj { get; set; }
        public string Cnpj { get; set; }
    }
}
