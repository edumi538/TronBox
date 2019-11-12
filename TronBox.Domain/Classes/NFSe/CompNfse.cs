using System;
using System.Xml.Serialization;

namespace TronBox.Domain.Classes.NFSe
{
    public partial class CompNfse
    {
        public Nfse Nfse { get; set; }
    }

    public class Nfse
    {
        public InfNfse InfNfse { get; set; }
        [XmlAttribute(AttributeName = "versao")]
        public string Versao { get; set; }
    }

    public class InfNfse
    {
        public string Numero { get; set; }
        public string CodigoVerificacao { get; set; }       
        public DateTime DataEmissao { get; set; }
        public string NaturezaOperacao { get; set; }
        public ValoresNfse ValoresNfse { get; set; }
        public Servico Servico { get; set; }
        public PrestadorServico PrestadorServico { get; set; }
        public TomadorServico TomadorServico { get; set; }
        public DeclaracaoPrestacaoServico DeclaracaoPrestacaoServico { get; set; }
    }

    public class ValoresNfse
    {
        public string BaseCalculo { get; set; }
        public string Aliquota { get; set; }
        public string ValorIss { get; set; }
        public string ValorLiquidoNfse { get; set; }
    }
}
