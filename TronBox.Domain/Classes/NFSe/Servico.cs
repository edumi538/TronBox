namespace TronBox.Domain.Classes.NFSe
{
    public class Servico
    {
        public Valores Valores { get; set; }
        public string Discriminacao { get; set; }
    }

    public class Valores
    {
        public decimal ValorServicos { get; set; }
        public decimal IssRetido { get; set; }
        public decimal ValorDeducoes { get; set; }
        public decimal ValorIss { get; set; }
        public decimal BaseCalculo { get; set; }
        public decimal Aliquota { get; set; }
        public decimal ValorLiquidoNfse { get; set; }
        public decimal DescontoCondicionado { get; set; }
        public decimal DescontoIncondicionado { get; set; }
    }
}
