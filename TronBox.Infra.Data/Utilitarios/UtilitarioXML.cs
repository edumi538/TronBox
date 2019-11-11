using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TronBox.Infra.Data.Utilitarios
{
    public static class UtilitarioXML
    {
        public static T XmlStringParaClasse<T>(string input) where T : class
        {
            using (TextReader textReader = new StringReader(input))
            {
                using (XmlTextReader reader = new XmlTextReader(textReader))
                {
                    reader.Namespaces = false;
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader);
                }
            }
        }
    }
}
