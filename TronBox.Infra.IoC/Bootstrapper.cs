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
using Sentinela.Domain.Aggregates.LoggingAgg;
using TronBox.Application.Services;
using TronBox.Application.Services.Interfaces;
using TronBox.Domain.Aggregates.ChaveDocumentoCanceladoAgg.Repository;
using TronBox.Domain.Aggregates.ConfiguracaoEmpresaAgg.Repository;
using TronBox.Domain.Aggregates.ConfiguracaoUsuarioAgg.Repository;
using TronBox.Domain.Aggregates.DadosComputadorMonitorAgg.Repository;
using TronBox.Domain.Aggregates.DocumentoFiscalAgg.Repository;
using TronBox.Domain.Aggregates.EstatisticaAgg.Repository;
using TronBox.Domain.Aggregates.HistoricoConsultaAgg.Repository;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoAgg.Repository;
using TronBox.Domain.Aggregates.HistoricoConsultaMatoGrossoSulAgg.Repository;
using TronBox.Domain.Aggregates.ManifestoAgg.Repository;
using TronBox.Infra.Data.Repositories;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Dominio.Auditoria;
using TronCore.Dominio.Notifications;
using TronCore.Enumeradores;
using TronCore.Notificacao.Interfaces;
using TronCore.Persistencia.Context;
using TronCore.Servicos;
using TronCore.Utilitarios.EnvioDeArquivo;

namespace TronBox.Infra.IoC
{
    public class Bootstrapper
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDbContext, SuiteMongoDbContext>();

            // Eventos de Domínio
            services.AddScoped<IDomainNotificationHandler<DomainNotification>, DomainNotificationHandler>();

            // Services
            services.AddScoped<IConfiguracaoEmpresaAppService, ConfiguracaoEmpresaAppService>();
            services.AddScoped<IDashboardAppService, DashboardAppService>();
            services.AddScoped<IDadosComputadorMonitorAppService, DadosComputadorMonitorAppService>();
            services.AddScoped<IDocumentoFiscalAppService, DocumentoFiscalAppService>();
            services.AddScoped<IEnumeradorAppService, EnumeradorAppService>();
            services.AddScoped<IHistoricoConsultaAppService, HistoricoConsultaAppService>();
            services.AddScoped<IHistoricoConsultaMatoGrossoAppService, HistoricoConsultaMatoGrossoAppService>();
            services.AddScoped<IHistoricoConsultaMatoGrossoSulAppService, HistoricoConsultaMatoGrossoSulAppService>();
            services.AddScoped<IManifestoAppService, ManifestoAppService>();
            services.AddScoped<IConfiguracaoUsuarioAppService, ConfiguracaoUsuarioAppService>();
            services.AddScoped<IEstatisticaAppService, EstatisticaAppService>();

            // Repositorios
            services.AddScoped<IConfiguracaoEmpresaRepository, ConfiguracaoEmpresaRepository>();
            services.AddScoped<IDadosComputadorMonitorRepository, DadosComputadorMonitorRepository>();
            services.AddScoped<IDocumentoFiscalRepository, DocumentoFiscalRepository>();
            services.AddScoped<IHistoricoConsultaRepository, HistoricoConsultaRepository>();
            services.AddScoped<IHistoricoConsultaMatoGrossoRepository, HistoricoConsultaMatoGrossoRepository>();
            services.AddScoped<IHistoricoConsultaMatoGrossoSulRepository, HistoricoConsultaMatoGrossoSulRepository>();
            services.AddScoped<IManifestoRepository, ManifestoRepository>();
            services.AddScoped<IChaveDocumentoCanceladoRepository, ChaveDocumentoCanceladoRepository>();
            services.AddScoped<IConfiguracaoUsuarioRepository, ConfiguracaoUsuarioRepository>();
            services.AddScoped<IEstatisticaRepository, EstatisticaRepository>();

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

            services.AddScoped<INotificacao>(factory => new Notificacao(Constantes.APP_CENTER_TOKEN, string.Empty, string.Empty));
            #endregion

            //// UnitOfWork

            // Infra - Identity
            services.AddScoped<IPessoaUsuarioLogado, PessoaUsuarioLogado>();

            services.AddTransient<IRegistroLogging, RegistroLogging>();
        }
    }
}
