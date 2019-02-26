using Comum.Infra.Data.Context;
using TronBox.Domain.Aggregates.CustomerAgg;
using TronBox.Domain.Aggregates.CustomerAgg.Repository;

namespace TronBox.Infra.Data.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(SuiteMongoDbContext context) : base(context)
        {
        }
    }
}
