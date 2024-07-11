using AuthServer.Host.EntityFrameworkCore;
using Autofac.Core;
using BaseService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Threading;

namespace AuthServer.Host
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpPermissionManagementEntityFrameworkCoreModule),
        typeof(AbpAuditLoggingEntityFrameworkCoreModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule),
        typeof(AbpIdentityEntityFrameworkCoreModule),
        typeof(AbpIdentityApplicationContractsModule),
        typeof(AbpAccountApplicationModule),
        typeof(AbpIdentityServerEntityFrameworkCoreModule),
        typeof(AbpEntityFrameworkCoreMySQLModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule),
        typeof(AbpTenantManagementApplicationContractsModule),
        typeof(BaseServiceApplicationContractsModule)

    )]
    public class AuthServerHostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            context.Services.AddAbpDbContext<AuthServerDbContext>(options =>
            {
                options.AddDefaultRepositories();
            });

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseMySQL();
            });

            //审计日志
            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabled = false;//不启用审计日志
                options.ApplicationName = "AuthServer";
            });

            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.AutoValidate = false; //表示不验证防伪令牌
            });


            Configure<AbpMultiTenancyOptions>(options =>
            {
                options.IsEnabled = true;
            });

            ConfigureCors(context, configuration);
            ConfigureRedisCache(context, configuration);
            ConfigureSwaggerServices(context, configuration);
            ConfigureLocalization();
            context.Services.AddSameSiteCookiePolicy();
            context.Services.AddHealthChecks();
            //services.AddHealthChecksUI().AddInMemoryStorage();
        }


        /// <summary>
        /// 跨域
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        /// <summary>
        /// RedisCache
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        private void ConfigureRedisCache(ServiceConfigurationContext context, IConfiguration configuration)
        {
            if (Convert.ToBoolean(configuration["Redis:IsEnabled"]))
            {
                context.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = configuration["Redis:Configuration"];
                    options.InstanceName = "AuthServer:";
                });

                //#region DataProtection
                ////设置应用程序唯一标识
                //var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
                //context.Services.AddDataProtection().SetApplicationName("FisheryApp")
                //    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                //#endregion
            }
        }

        /// <summary>
        /// swagger
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        private static void ConfigureSwaggerServices(ServiceConfigurationContext context, IConfiguration configuration)
        {
            if (Convert.ToBoolean(configuration["UseSwagger"]))
            {
                context.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthServer API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Description = "请输入JWT令牌，例如：Bearer 12345abcdef",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });

                    //foreach (var path in new[] { "AuthServer.Host.xml" })
                    //{
                    //    var basepath = AppContext.BaseDirectory;
                    //    var xmlPath = Path.Combine(basepath, path);
                    //    options.IncludeXmlComments(xmlPath, true);
                    //}
                });

            }
        }

        /// <summary>
        /// 国际化
        /// </summary>
        private void ConfigureLocalization()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
                options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            });
        }


        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            var pathBase = Environment.GetEnvironmentVariable("AUTHSERVER_PATH");
            if (!string.IsNullOrWhiteSpace(pathBase))
            {
                app.Use((context, next) =>
                {
                    context.Request.PathBase = Environment.GetEnvironmentVariable("AUTHSERVER_PATH");
                    return next();
                });
                app.UsePathBase(pathBase);
            }

            #region  解决反向代理域名是https的，但是端点无法映射到https
            // ref: https://github.com/aspnet/Docs/issues/2384

            var forwardOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            forwardOptions.KnownNetworks.Clear();
            forwardOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardOptions);

            #endregion

            app.UseCookiePolicy();
            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //    endpoints.MapGet("/health", () => "Client01");
            //});
            app.UseHealthChecks("/health");
            //app.UseHealthChecksUI();         

            app.UseCors(DefaultCorsPolicyName);
            app.UseAbpRequestLocalization();
            app.UseAuthentication();
            app.UseMultiTenancy();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseAuditing();
            app.UseConfiguredEndpoints();

            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    if (httpReq.Headers.ContainsKey("X-Request-Uri"))
                    {
                        var index = httpReq.Headers["X-Request-Uri"].ToString().IndexOf("/swagger/");
                        if (index > 0)
                        {
                            var serverUrl = $"{httpReq.Headers["X-Request-Uri"].ToString().Substring(0, index)}/";
                            swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = serverUrl } };
                        }
                    }
                });
            });
            app.UseSwaggerUI(options => options.SwaggerEndpoint("v1/swagger.json", "BaseService Service API"));

            AsyncHelper.RunSync(async () =>
            {
                using (var scope = context.ServiceProvider.CreateScope())
                {
                    await scope.ServiceProvider
                        .GetRequiredService<IDataSeeder>()
                        .SeedAsync();
                }
            });
        }


    }
}
