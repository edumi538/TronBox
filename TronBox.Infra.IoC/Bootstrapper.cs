using AutoMapper;
using Comum.Application.Services;
using Comum.Application.Services.Interfaces;
using Comum.Domain.Aggregates.EmpresaAgg.Repository;
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
using TronCore.Enumeradores;
using TronCore.Persistencia.Context;
using TronCore.Utilitarios.EnvioDeArquivo;

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

            // Repositorios

            #region Add Comum
            services.AddScoped<IEmpresaAppService, EmpresaNoSqlAppService>();
            services.AddScoped<IPessoaAppService, PessoaNoSqlAppService>();
            services.AddScoped<IPessoaUsuarioAppService, PessoaUsuarioNoSqlAppService>();
            services.AddScoped<IPessoaEmpresaAppService, PessoaEmpresaNoSqlAppService>();

            services.AddScoped<IEmpresaRepository, EmpresaRepositoryNoSql>();
            services.AddScoped<IPessoaRepository, PessoaRepositoryNoSql>();
            services.AddScoped<IPessoaEmpresaRepository, PessoaEmpresaRepositoryNoSql>();
            services.AddScoped<IPessoaUsuarioRepository, PessoaUsuarioRepositoryNoSql>();

            Comum.Infra.IoC.Bootstrapper.RegisterServices(services, configuration, new AzureBlobSettings(
                    storageAccount: ConstantsEnvioArquivo.storageAccountNovo,
                    storageKey: ConstantsEnvioArquivo.storageKeyNovo,
                    containerName: Modulo.Box.ContainerName));
            #endregion

            //// UnitOfWork

            // Infra - Identity
            services.AddScoped<IPessoaUsuarioLogado, PessoaUsuarioLogado>();

            services.AddIdentityService();
        }
    }
}
