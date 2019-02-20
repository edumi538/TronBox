using AutoMapper;
using Comum.Application.Services;
using Comum.Application.Services.Interfaces;
using Comum.Domain.Aggregates.PessoaAgg.Repository;
using Comum.Domain.Interfaces;
using Comum.Domain.Services.Interfaces;
using Comum.Infra.Data.Context;
using Comum.Infra.Data.Repositories;
using Comum.Infra.IoC.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TronBox.Infra.IoC.Extensions;
using TronCore.Dominio.Notifications;
using TronCore.Persistencia.Context;

namespace TronBox.Infra.IoC
{
    public class Bootstrapper
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext, SuiteMongoDbContext>();

            // Aplicação
            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));       

            // Eventos de Domínio
            services.AddScoped<IDomainNotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Services
            
			//Services base
            services.AddScoped<IPessoaAppService, PessoaNoSqlAppService>();
            services.AddScoped<IPessoaUsuarioAppService, PessoaUsuarioNoSqlAppService>();            
            services.AddScoped<IPessoaEmpresaAppService, PessoaEmpresaNoSqlAppService>();

            // Repositorios
            

            //Acesso aos conceitos base
            services.AddScoped<IPessoaRepository, PessoaRepositoryNoSql>();
            services.AddScoped<IPessoaEmpresaRepository, PessoaEmpresaRepositoryNoSql>();
            services.AddScoped<IPessoaUsuarioRepository, PessoaUsuarioRepositoryNoSql>();

            //Invoca a construção do container da plataforma comum.
            Comum.Infra.IoC.Bootstrapper.RegisterServices(services, configuration);

            //// UnitOfWork

            // Infra - Identity
            services.AddScoped<IPessoaUsuarioLogado, PessoaUsuarioLogado>();

            services.AddIdentityService();
        }
    }
}
