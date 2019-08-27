using AutoMapper;
using Comum.Infra.Data.Context;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using TronBox.Infra.IoC;
using TronBox.UI.Helpers;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Infra.Bus;
using TronCore.InjecaoDependencia;
using TronCore.Seguranca.Filtros;

namespace TronBox
{
    public class Startup
    {
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Constantes.CHAVE_TOKEN));

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        //public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddAutoMapper();
            services.AddScoped<AuditarAttribute>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Contexts
            services.AddScoped<SuiteMongoDbContext>();
            services.AddEntityFrameworkSqlServer().AddDbContext<SuiteDbContext>(ServiceLifetime.Scoped);
            #endregion

            Bootstrapper.RegisterServices(services, Configuration);
            services.AddScoped<AuditarAttribute>();

            #region Mvc
            services.AddMvc(options => options.Filters.Add(typeof(AutorizacaoActionFilter)))
                .AddFluentValidation()
                .AddControllersAsServices()
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddNodeServices();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "Sentinela",

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = tokenValidationParameters);

            services.AddAuthorization(o => o.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

            services.Configure<JwtIssuerOptions>(o =>
            {
                o.Issuer = "Sentinela";
                o.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];

                o.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
            #endregion

            // Publicação servidor de testes
            services.Configure<IISOptions>(options => options.ForwardClientCertificate = false);

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor accessor)
        {
#if DEBUG
            app.UseCors(builder => builder.WithOrigins("http://localhost:3000", "http://beta.tronbox.com.br").AllowAnyMethod().AllowAnyHeader());
#else
            app.UseCors(builder => builder.WithOrigins("http://box.tron.com.br").AllowAnyMethod().AllowAnyHeader());
#endif

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var error = context.Features.Get<IExceptionHandlerFeature>();

                    if (error != null)
                    {
                        var result = JsonConvert.SerializeObject(new { sucesso = false, erro = error.Error.Message });
                        context.Response.ContentType = "application/json";

                        context.Response.AddApplicationError(error.Error.Message);
                        await context.Response.WriteAsync(result);
                    }
                });
            });

            app.UseAuthentication();
            app.UseMvc();

            InMemoryBus.ContainerAccessor = () => accessor.HttpContext.RequestServices;
            ContainerInjecaoDependencia.Instancia.ContainerAccessor = () => accessor.HttpContext.RequestServices;
        }
    }
}
