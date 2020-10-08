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
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using TronBox.API.Helpers;
using TronBox.Domain.Automapper;
using TronBox.Infra.IoC;
using TronBox.UI.Helpers;
using TronCore.DefinicoesConfiguracoes;
using TronCore.Enumeradores;
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
        {
            services.AddOptions();
            services.AddSingleton(AutoMapperConfiguration.RegisterMappings());
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<AuditarAttribute>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Contexts
            services.AddScoped<SuiteMongoDbContext>();
            #endregion

            Bootstrapper.RegisterServices(services, Configuration);
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
            services.AddScoped<AuditarAttribute>();

            #region Mvc
            services.AddMvc(options => options.Filters.Add(typeof(AutorizacaoActionFilter)))
                .AddFluentValidation()
                .AddControllersAsServices()
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

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

            #region Swagger
            // Configurando o serviço de documentação do Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Tron Box",
                        Version = "v1",
                        Description = Modulo.Box.Titulo,
                        Contact = new OpenApiContact
                        {
                            Name = "Tron Box",
                            Url = new Uri(Constantes.URI_BASE_BX_PORTAL)
                        }
                    });

                c.DocumentFilter<SwaggerFilterOutControllers>();

                string caminhoAplicacao =
                    PlatformServices.Default.Application.ApplicationBasePath;
                string nomeAplicacao =
                    PlatformServices.Default.Application.ApplicationName;
                string caminhoXmlDoc =
                    Path.Combine(caminhoAplicacao, $"{nomeAplicacao}.xml");

                c.IncludeXmlComments(caminhoXmlDoc);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                });

                c.AddSecurityDefinition("ServiceIdentify", new OpenApiSecurityScheme
                {
                    Description = "Service Identify is definition of tenant. Example: \"ServiceIdentify: {tenantId}\"",
                    Name = "ServiceIdentify",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });
            #endregion

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor accessor)
        {
            app.UseCors(builder => builder.WithOrigins(Constantes.URI_BASE_BX_PORTAL).AllowAnyMethod().AllowAnyHeader());

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                    var error = context.Features.Get<IExceptionHandlerFeature>();

                    if (error != null)
                    {
                        var result = JsonConvert.SerializeObject(new { sucesso = false, erro = error.Error.Message, stackTrace = error.Error.StackTrace });
                        context.Response.ContentType = "application/json";

                        context.Response.AddApplicationError(error.Error.Message);
                        await context.Response.WriteAsync(result);
                    }
                });
            });

            #region Swagger
            // Ativando middlewares para uso do Swagger
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tron Box");
                    c.RoutePrefix = "developer";
                });
            }
            #endregion

            app.UseAuthentication();
            app.UseMvc();

            InMemoryBus.ContainerAccessor = () => accessor.HttpContext.RequestServices;
            ContainerInjecaoDependencia.Instancia.ContainerAccessor = () => accessor.HttpContext.RequestServices;
        }
    }
}
