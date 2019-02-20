using TronCore.Dominio.Base;
using Comum.Infra.Data.Context;
using TronCore.Persistencia.Interfaces;
using TronCore.InjecaoDependencia;
using TronCore.Persistencia;

namespace TronBox.Infra.Data.Repositories
{
    public class Repository<TEntity> : RepositoryNoSqlBase<TEntity>, IRepository<TEntity> where TEntity : Entity<TEntity>, new()
    {
        protected SuiteMongoDbContext Context { get; set; }
        public IRepositoryFactory RepositoryFactory { get { return FabricaGeral.Instancie<IRepositoryFactory>().ObtenhaInstancia(Context); } }

        #region Construtor
        public Repository(SuiteMongoDbContext context) : base(context)
        {
            Context = context;
        }
        #endregion
    }
}