using Comum.Infra.Data.Context;
using TronCore.Dominio.Base;
using TronCore.InjecaoDependencia;
using TronCore.Persistencia;
using TronCore.Persistencia.Interfaces;

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