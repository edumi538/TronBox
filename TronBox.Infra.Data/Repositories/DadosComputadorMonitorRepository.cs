using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.DadosComputadorMonitorAgg;
using TronBox.Domain.Aggregates.DadosComputadorMonitorAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class DadosComputadorMonitorRepository : Repository<DadosComputadorMonitor>, IDadosComputadorMonitorRepository
    {
        public DadosComputadorMonitorRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
