using System.Collections.Generic;

namespace TronConnect.Models
{
    public class PushModel
    {
        public List<string> Emails { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }

        public PushModel()
        {
            Emails = new List<string>();
        }
    }
}
