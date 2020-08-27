using TronCore.Dominio.Base;

namespace TronBox.Domain.Aggregates.ChaveDocumentoCanceladoAgg
{
    public class ChaveDocumentoCancelado : Entity<ChaveDocumentoCancelado>
    {
        public string ChaveDocumentoFiscal { get; set; } 
    }
}
