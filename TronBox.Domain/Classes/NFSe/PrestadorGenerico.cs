namespace TronBox.Domain.Classes.NFSe
{
    public class PrestadorGenerico
    {
        public IdentificacaoPrestador IdentificacaoPrestador { get; set; }
        public CpfCnpj CpfCnpj { get; set; }
        public string RazaoSocial { get; set; }
        public Endereco Endereco { get; set; }
        public Contato Contato { get; set; }
    }

    public class IdentificacaoPrestador
    {
        public CpfCnpj CpfCnpj { get; set; }
        public string Cnpj { get; set; }
    }
}
