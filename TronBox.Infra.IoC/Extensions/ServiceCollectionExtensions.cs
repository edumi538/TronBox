using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TronBox.Infra.IoC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/acesso/login";
                options.AccessDeniedPath = "/acesso/negado";
                options.ReturnUrlParameter = "retornoUrl";
            });

            return services;
        }

        public static IServiceCollection AddContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSuiteDbContext(configuration);
            return services;
        }
    }
}
