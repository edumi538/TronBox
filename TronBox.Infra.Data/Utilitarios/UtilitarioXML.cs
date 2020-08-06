using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;

namespace TronBox.Infra.Data.Utilitarios
{
    public static class UtilitarioXML
    {
        private static readonly Hashtable CacheSerializers = new Hashtable();

        public static T XmlStringParaClasse<T>(string input) where T : class
        {
            var keyNomeClasseEmUso = typeof(T).FullName;
            var ser = BuscarNoCache(keyNomeClasseEmUso, typeof(T));

            using (var sr = new StringReader(input))
                return (T)ser.Deserialize(sr);
        }

        private static XmlSerializer BuscarNoCache(string chave, Type type)
        {
            if (CacheSerializers.Contains(chave)) return (XmlSerializer)CacheSerializers[chave];

            lock (CacheSerializers)
            {
                if (CacheSerializers.Contains(chave)) return (XmlSerializer)CacheSerializers[chave];

                var ser = XmlSerializer.FromTypes(new[] { type })[0];

                CacheSerializers.Add(chave, ser);

                return ser;
            }
        }
    }
}
