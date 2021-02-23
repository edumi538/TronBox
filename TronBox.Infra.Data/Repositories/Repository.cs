using Comum.Infra.Data.Context;
using Comum.Infra.Data.Repositories;
using TronCore.Dominio.Base;

namespace TronBox.Infra.Data.Repositories
{
    public class Repository<TEntity> : RepositoryNoSql<TEntity> where TEntity : Entity<TEntity>, new()
    {
        #region Construtor
        protected Repository(SuiteMongoDbContext context) : base(context)
        {
            Context = context;
        }
        #endregion
    }
}