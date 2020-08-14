using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoSulAgg;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoSulAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class HistoricoConsultaMatoGrossoSulRepository : Repository<HistoricoConsultaMatoGrossoSul>, IHistoricoConsultaMatoGrossoSulRepository
    {
        public HistoricoConsultaMatoGrossoSulRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
