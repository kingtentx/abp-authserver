using Autofac.Core;
using BaseService.Consts;
using BaseService.EntityFrameworkCore;
using Cimc.Cache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Volo.Abp;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Json;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.Threading;

namespace BaseService
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(BaseServiceApplicationModule),
        typeof(BaseServiceEntityFrameworkCoreModule),
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpHttpClientIdentityModelWebModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpMultiTenancyModule),
        typeof(AbpTenantManagementEntityFrameworkCoreModule)
    )]
    public class BaseServiceHostModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();

            Configure<AbpDbContextOptions>(options =>
            {
                options.UseMySQL();
            });
            //审计日志
            Configure<AbpAuditingOptions>(options =>
            {
                options.IsEnabled = false;//不启用审计日志
                options.ApplicationName = SystemConsts.ServiceName;
            });

            //令牌验证
            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.AutoValidate = false;//不验证防伪令牌            
            });

            //// 禁用防伪造令牌验证
            //context.Services.AddAntiforgery(options =>
            //{
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            //    options.Cookie.HttpOnly = false;
            //    options.Cookie.Name = "XSRF-TOKEN";
            //    options.Cookie.SameSite = SameSiteMode.None;
            //    options.SuppressXFrameOptionsHeader = false;
            //    options.HeaderName = "X-XSRF-TOKEN";
            //});

            Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always; // 设置Secure属性
            });

            ConfigureMultiTenancy();
            ConfigureConventionalControllers();
            ConfigureAuthentication(context, configuration);
            ConfigureRedisCache(context, configuration);
            ConfigureAuthorityCache(context, configuration);
            ConfigureCors(context, configuration);
            ConfigureSwaggerServices(context, configuration);
            ConfigureCap(context, configuration);
            ConfigureResetPassword(context);
            ConfigureDateTime();
            ConfigureLocalization();

        }

        /// <summary>
        /// 多租户
        /// </summary>
        private void ConfigureMultiTenancy()
        {
            Configure<AbpMultiTenancyOptions>(options =>
            {
                options.IsEnabled = true;
            });
        }

        /// <summary>
        /// jwt认证
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.Audience = SystemConsts.ServiceName;
                });
        }

        /// <summary>
        /// MVC动态路由
        /// </summary>
        private void ConfigureConventionalControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(BaseServiceApplicationModule).Assembly);
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
                    options.InstanceName = "BaseService:";
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
        /// 格式化日期
        /// </summary>
        public void ConfigureDateTime()
        {           
            Configure<AbpJsonOptions>(options =>
                options.OutputDateTimeFormat = "yyyy-MM-dd HH:mm:ss"    //对类型为DateTimeOffset生效
            );
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        public void ConfigureResetPassword(ServiceConfigurationContext context)
        {
            //添加第二个身份服务、获取重置密码token使用
            context.Services.AddIdentityCore<Volo.Abp.Identity.IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddTokenProvider<DataProtectorTokenProvider<Volo.Abp.Identity.IdentityUser>>(TokenOptions.DefaultProvider);
        }

        /// <summary>
        /// 权限对象缓存
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        private void ConfigureAuthorityCache(ServiceConfigurationContext context, IConfiguration configuration)
        {
            context.Services.AddSingleton(typeof(ICacheService), new RedisHelper(new RedisCacheOptions
            {
                Configuration = configuration["AuthorityCache:Configuration"]
            }));
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
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BaseService Service API", Version = "v1" });
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

                    foreach (var path in new[] { "BaseService.Application.Contracts.xml", "BaseService.Application.xml" })
                    {
                        var basepath = AppContext.BaseDirectory;
                        var xmlPath = Path.Combine(basepath, path);
                        options.IncludeXmlComments(xmlPath, true);
                    }
                });

            }
        }

        /// <summary>
        /// CAP-rabbitMQ
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configuration"></param>
        private void ConfigureCap(ServiceConfigurationContext context, IConfiguration configuration)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            context.Services.AddCap(x =>
            {
                //配置数据库连接               
                x.UseMySql(configuration["ConnectionStrings:Default"]);
                //配置消息队列RabbitMQ
                x.UseRabbitMQ(mq =>
                {
                    mq.HostName = configuration["RabbitMQ:Connections:Default:HostName"];
                    mq.Port = int.Parse(configuration["RabbitMQ:Connections:Default:Port"]);
                    mq.UserName = configuration["RabbitMQ:Connections:Default:UserName"];
                    mq.Password = configuration["RabbitMQ:Connections:Default:Password"];
                    mq.VirtualHost = configuration["RabbitMQ:Connections:Default:VirtualHost"];
                    mq.ExchangeName = configuration["RabbitMQ:EventBus:ExchangeName"];
                });
                x.UseDashboard();//使用Cap可视化面板
                //bool auth = !hostingEnvironment.IsDevelopment();
                //x.UseDashboard(options => { options.UseAuth = auth; });
                x.FailedRetryInterval = 30;//失败后的重拾间隔，默认60秒
                x.FailedRetryCount = 10;//失败后的重试次数，默认50次；在FailedRetryInterval默认60秒的情况下，即默认重试50*60秒(50分钟)之后放弃失败重试
                x.SucceedMessageExpiredAfter = 60 * 60; //设置成功信息的删除时间默认24*3600秒
            });
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

            app.UseCorrelationId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseCors(DefaultCorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMultiTenancy();

            app.Use(async (ctx, next) =>
            {
                var currentPrincipalAccessor = ctx.RequestServices.GetRequiredService<ICurrentPrincipalAccessor>();
                var map = new Dictionary<string, string>()
                {
                    { "sub", AbpClaimTypes.UserId },
                    { "role", AbpClaimTypes.Role },
                    { "email", AbpClaimTypes.Email },
                    { "name", AbpClaimTypes.UserName }                  
                };
                var mapClaims = currentPrincipalAccessor.Principal.Claims.Where(p => map.Keys.Contains(p.Type)).ToList();
                currentPrincipalAccessor.Principal.AddIdentity(new ClaimsIdentity(mapClaims.Select(p => new Claim(map[p.Type], p.Value, p.ValueType, p.Issuer))));
                await next();
            });

            app.UseAbpRequestLocalization();

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

            app.UseAuditing();
            app.UseAbpSerilogEnrichers();
            app.UseUnitOfWork();
            app.UseConfiguredEndpoints();

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
