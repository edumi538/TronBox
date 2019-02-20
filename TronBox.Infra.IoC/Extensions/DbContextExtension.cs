using Comum.Infra.Data.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TronBox.Infra.IoC.Extensions
{
    public static class DbContextExtension
    {

        public static IServiceCollection AddSuiteDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SuiteDbContext>(ServiceLifetime.Transient);
            return services;
        }
    }
}
