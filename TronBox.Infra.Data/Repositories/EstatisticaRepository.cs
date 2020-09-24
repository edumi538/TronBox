using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.EstatisticaAgg;
using TronBox.Domain.Aggregates.EstatisticaAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class EstatisticaRepository : Repository<Estatistica>, IEstatisticaRepository
    {
        public EstatisticaRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
