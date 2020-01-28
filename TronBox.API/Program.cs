using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Sentry.Extensibility;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using TronBox.Domain.Enums;
using TronCore.DefinicoesConfiguracoes;
using Utilitarios.Atualizacao;

namespace TronBox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                //Invoca a atualização das funções do Módulo.
                AjudanteAtualizacaoFuncoes.ExcutarAtualizacao(typeof(eFuncaoTronBox));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //Inicio o serviço da WebAPI
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseUrls("http://*.localhost:6004")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseSentry(options =>
                {
                    options.Dsn = "https://a12ec570bf324eefbb39ee20cd8c1fb8@sentry.io/2036782";
                    options.MaxRequestBodySize = RequestSize.Always;
                    options.Release = "BX-1.0.0";
                    options.MaxBreadcrumbs = 200;
                    options.DecompressionMethods = DecompressionMethods.None;
                    options.MaxQueueItems = 100;
                    options.ShutdownTimeout = TimeSpan.FromSeconds(5);
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    ConfigurationService.Enviroment = env.EnvironmentName;
                    if (env.EnvironmentName == "Production")
                    {
                        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    }
                    else
                    {
                        config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    }

                    if (env.IsDevelopment())
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            config.AddUserSecrets(appAssembly, optional: true);
                        }
                    }

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }

                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>();
        }
    }
}
