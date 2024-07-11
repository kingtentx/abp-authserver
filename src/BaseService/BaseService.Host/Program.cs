using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace BaseService
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
#else
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error) //¹ýÂËEF sqlÊä³ö               
#endif
                .Enrich.WithProperty("Application", "BaseService")
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File($"Logs/.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90))
#if !DEBUG
                 //.WriteTo.Elasticsearch(
                 //   new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearch:Url"]))
                 //   {
                 //       AutoRegisterTemplate = true,
                 //       AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                 //       IndexFormat = "base-log-{0:yyyy.MM.dd}"
                 //   })
#endif
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information("Starting BaseService.Host.");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "BaseService.Host terminated unexpectedly!");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseAutofac()
                .UseSerilog();
    }
}
