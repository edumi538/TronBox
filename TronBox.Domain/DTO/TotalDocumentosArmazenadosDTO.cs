using System;

namespace TronBox.Domain.DTO
{
    public class TotalDocumentosArmazenadosDTO
    {
        public Guid TenantId { get; set; }
        public int PeriodoInicial { get; set; }
        public int PeriodoFinal { get; set; }
        public long TotalDocumentos { get; set; }
    }
}
