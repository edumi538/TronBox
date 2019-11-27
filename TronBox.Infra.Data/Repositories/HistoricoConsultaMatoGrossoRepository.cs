using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class HistoricoConsultaMatoGrossoRepository : Repository<HistoricoConsultaMatoGrosso>, IHistoricoConsultaMatoGrossoRepository
    {
        public HistoricoConsultaMatoGrossoRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
