using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace BaseService.HttpApi.Client;

[DependsOn(
    typeof(AbpHttpClientModule),
    typeof(BaseServiceApplicationContractsModule)
)]
public class BaseServiceHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    // 添加动态api客户端
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(BaseServiceApplicationContractsModule).Assembly,
            RemoteServiceName
        );
    }
}
