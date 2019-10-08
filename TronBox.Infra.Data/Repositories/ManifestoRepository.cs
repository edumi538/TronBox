using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.ManifestoAgg;
using TronBox.Domain.Aggregates.ManifestoAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class ManifestoRepository : Repository<Manifesto>, IManifestoRepository
    {
        public ManifestoRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
