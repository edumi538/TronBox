using System.Xml.Serialization;

namespace TronBox.Domain.Classes.NFSe
{
    public class Endereco
    {
        [XmlElement(ElementName = "Endereco")]
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Uf { get; set; }
        public string Cep { get; set; }
    }
}
