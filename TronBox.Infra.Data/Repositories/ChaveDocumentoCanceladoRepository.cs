using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.ChaveDocumentoCanceladoAgg;
using TronBox.Domain.Aggregates.ChaveDocumentoCanceladoAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class ChaveDocumentoCanceladoRepository : Repository<ChaveDocumentoCancelado>, IChaveDocumentoCanceladoRepository
    {
        public ChaveDocumentoCanceladoRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
