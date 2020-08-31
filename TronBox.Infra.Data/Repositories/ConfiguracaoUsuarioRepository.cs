using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg;
using TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class ConfiguracaoUsuarioRepository : Repository<ConfiguracaoUsuario>, IConfiguracaoUsuarioRepository
    {
        public ConfiguracaoUsuarioRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
