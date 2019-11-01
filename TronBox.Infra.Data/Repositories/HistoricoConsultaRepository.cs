using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class HistoricoConsultaRepository : Repository<HistoricoConsulta>, IHistoricoConsultaRepository
    {
        public HistoricoConsultaRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
