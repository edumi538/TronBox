using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class ConfiguracaoEmpresaRepository : Repository<ConfiguracaoEmpresa>, IConfiguracaoEmpresaRepository
    {
        public ConfiguracaoEmpresaRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
