using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class DocumentoFiscalRepository : Repository<DocumentoFiscal>, IDocumentoFiscalRepository
    {
        public DocumentoFiscalRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
