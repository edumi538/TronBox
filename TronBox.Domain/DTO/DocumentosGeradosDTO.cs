using System.Collections.Generic;

namespace TronBox.Domain.DTO
{
    public class DocumentosGeradosDTO
    {
        public List<DocumentoFiscalDTO> DocumentosProcessados { get; set; }
        public List<DocumentoFiscalNaoGeradoDTO> DocumentosNaoProcessados { get; set; }
    }

    public class DocumentoFiscalNaoGeradoDTO
    {
        public string ChaveDocumentoFiscal { get; set; }
        public string Mensagem { get; set; }
    }
}
