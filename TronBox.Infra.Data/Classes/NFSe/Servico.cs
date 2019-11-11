using System.Xml.Serialization;

namespace TronBox.Infra.Data.Classes.NFSe
{
    public class Servico
    {
        public Valores Valores { get; set; }
    }

    public class Valores
    {
        public decimal ValorServicos { get; set; }
    }
}
