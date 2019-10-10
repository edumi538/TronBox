using System.Collections.Generic;

namespace TronBox.Domain.DTO
{
    public class DocumentosGeradosDTO
    {
        public List<DocumentoFiscalDTO> DocumentosProcessados { get; set; }
        public List<string> documentosNaoProcessados { get; set; }
    }
}
