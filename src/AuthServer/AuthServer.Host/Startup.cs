using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Host
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {          
            services.AddApplication<AuthServerHostModule>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {    
            app.InitializeApplication();
        }      
    }
}
