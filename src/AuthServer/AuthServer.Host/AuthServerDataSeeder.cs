using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using ApiResource = Volo.Abp.IdentityServer.ApiResources.ApiResource;
using ApiScope = Volo.Abp.IdentityServer.ApiScopes.ApiScope;
using Client = Volo.Abp.IdentityServer.Clients.Client;

namespace AuthServer.Host
{
    public class AuthServerDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IApiResourceRepository _apiResourceRepository;
        private readonly IApiScopeRepository _apiScopeRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IIdentityResourceDataSeeder _identityResourceDataSeeder;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IPermissionDataSeeder _permissionDataSeeder;
        private string[] MicroServices { get; }

        public AuthServerDataSeeder(
            IClientRepository clientRepository,
            IApiResourceRepository apiResourceRepository,
            IApiScopeRepository apiScopeRepository,
            IIdentityResourceDataSeeder identityResourceDataSeeder,
            IGuidGenerator guidGenerator,
            IPermissionDataSeeder permissionDataSeeder,
            IConfiguration configuration)
        {
            _clientRepository = clientRepository;
            _apiResourceRepository = apiResourceRepository;
            _apiScopeRepository = apiScopeRepository;
            _identityResourceDataSeeder = identityResourceDataSeeder;
            _guidGenerator = guidGenerator;
            _permissionDataSeeder = permissionDataSeeder;
            MicroServices = configuration.GetSection("MicroServices").Get<string[]>();
        }

        [UnitOfWork]
        public virtual async Task SeedAsync(DataSeedContext context)
        {
            await _identityResourceDataSeeder.CreateStandardResourcesAsync();
            await CreateApiResourcesAsync();
            await CreateApiScopesAsync();
            await CreateClientsAsync();
        }

        private async Task CreateApiScopesAsync()
        {
            //await CreateApiScopeAsync("BaseService");
            //await CreateApiScopeAsync("InternalGateway");
            //await CreateApiScopeAsync("WebAppGateway");

            foreach (var service in MicroServices)
            {
                await CreateApiScopeAsync(service);
            }

        }

        private async Task CreateApiResourcesAsync()
        {
            var commonApiUserClaims = new[]
            {
                "email",
                "email_verified",
                "name",
                "sur_name",
                "phone_number",
                "phone_number_verified",
                "role",
                "given_name"
            };

            //await CreateApiResourceAsync("BaseService", commonApiUserClaims);
            //await CreateApiResourceAsync("InternalGateway", commonApiUserClaims);
            //await CreateApiResourceAsync("WebAppGateway", commonApiUserClaims);

            foreach (var service in MicroServices)
            {
                await CreateApiResourceAsync(service, commonApiUserClaims);
            }
        }

        private async Task<ApiResource> CreateApiResourceAsync(string name, IEnumerable<string> claims)
        {
            var apiResource = await _apiResourceRepository.FindByNameAsync(name);
            if (apiResource == null)
            {
                apiResource = await _apiResourceRepository.InsertAsync(
                    new ApiResource(
                        _guidGenerator.Create(),
                        name,
                        name + " API"
                    ),
                    autoSave: true
                );
            }

            foreach (var claim in claims)
            {
                if (apiResource.FindClaim(claim) == null)
                {
                    apiResource.AddUserClaim(claim);
                }
            }

            return await _apiResourceRepository.UpdateAsync(apiResource);
        }

        private async Task<ApiScope> CreateApiScopeAsync(string name)
        {
            var apiScope = await _apiScopeRepository.FindByNameAsync(name);
            if (apiScope == null)
            {
                apiScope = await _apiScopeRepository.InsertAsync(
                    new ApiScope(
                        _guidGenerator.Create(),
                        name,
                        name + " API"
                    ),
                    autoSave: true
                );
            }

            return apiScope;
        }

        private async Task CreateClientsAsync()
        {
            var commonScopes = new[]
            {
                "email",
                "openid",
                "profile",
                "role",
                "phone",
                "address"
            };

            await CreateClientAsync(
              name: "basic-app",
              scopes: MicroServices,
              grantTypes: new[] { "password" },
              secret: null,
              requireClientSecret: false
          );

            await CreateClientAsync(
                name: "basic-web",
                scopes: MicroServices,
                grantTypes: new[] { "password" },
                secret: null,
                requireClientSecret: false
            );

            await CreateClientAsync(
                name: "business-app",
                scopes: new[] { "BaseService" },
                grantTypes: new[] { "client_credentials" },
                secret: "1q2w3e*".Sha256()
            //permissions: new[] { IdentityPermissions.Users.Default, IdentityPermissions.UserLookup.Default, }
            );
        }

        private async Task<Client> CreateClientAsync(
            string name,
            IEnumerable<string> scopes,
            IEnumerable<string> grantTypes,
            string secret = null,
            string redirectUri = null,
            string postLogoutRedirectUri = null,
            string frontChannelLogoutUri = null,
            bool requireClientSecret = true,
            bool requirePkce = false,
            IEnumerable<string> permissions = null,
            IEnumerable<string> corsOrigins = null)
        {
            var client = await _clientRepository.FindByClientIdAsync(name);
            if (client == null)
            {
                client = await _clientRepository.InsertAsync(
                    new Client(
                        _guidGenerator.Create(),
                        name
                    )
                    {
                        ClientName = name,
                        ProtocolType = "oidc",
                        Description = name,
                        AlwaysIncludeUserClaimsInIdToken = true,//在请求id token和access token时，如果用户声明始终将其添加到id token而不是请求客户端使用userinfo endpoint
                        AllowOfflineAccess = true,//指定此客户端是否可以请求刷新令牌
                        AbsoluteRefreshTokenLifetime = 2592000,//刷新令牌的最长生命周期（秒）。默认为2592000秒/ 30天
                        AccessTokenLifetime = 3600, //访问令牌的生命周期，以秒为单位（默认为3600秒/ 1小时）
                        AuthorizationCodeLifetime = 300,//授权代码的生命周期，以秒为单位（默认为300秒/ 5分钟）
                        IdentityTokenLifetime = 300,//身份令牌的生命周期，以秒为单位（默认为300秒/ 5分钟）
                        RequireConsent = false,//指定是否需要同意屏幕
                        FrontChannelLogoutUri = frontChannelLogoutUri,
                        RequireClientSecret = requireClientSecret,
                        RequirePkce = requirePkce
                    },
                    autoSave: true
                );
            }

            foreach (var scope in scopes)
            {
                if (client.FindScope(scope) == null)
                {
                    client.AddScope(scope);
                }
            }

            foreach (var grantType in grantTypes)
            {
                if (client.FindGrantType(grantType) == null)
                {
                    client.AddGrantType(grantType);
                }
            }

            if (!secret.IsNullOrEmpty())
            {
                if (client.FindSecret(secret) == null)
                {
                    client.AddSecret(secret);
                }
            }

            if (redirectUri != null)
            {
                if (client.FindRedirectUri(redirectUri) == null)
                {
                    client.AddRedirectUri(redirectUri);
                }
            }

            if (postLogoutRedirectUri != null)
            {
                if (client.FindPostLogoutRedirectUri(postLogoutRedirectUri) == null)
                {
                    client.AddPostLogoutRedirectUri(postLogoutRedirectUri);
                }
            }

            if (permissions != null)
            {
                await _permissionDataSeeder.SeedAsync(
                    ClientPermissionValueProvider.ProviderName,
                    name,
                    permissions,
                    null
                );
            }

            if (corsOrigins != null)
            {
                foreach (var origin in corsOrigins)
                {
                    if (!origin.IsNullOrWhiteSpace() && client.FindCorsOrigin(origin) == null)
                    {
                        client.AddCorsOrigin(origin);
                    }
                }
            }

            return await _clientRepository.UpdateAsync(client);
        }
    }
}
