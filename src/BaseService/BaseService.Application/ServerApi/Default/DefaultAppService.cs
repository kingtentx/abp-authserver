using BaseService.Consts;
using BaseService.CurrentAuthorityService;
using BaseService.Enums;
using BaseService.Systems;
using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.ProductManagement.Dto;
using BaseService.Systems.RoleManagement.Dto;
using BaseService.Systems.RoleMenusManagement.Dto;
using Cimc.Cache;
using Cimc.Helper;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace BaseService.ServerApi.Default
{
    /// <summary>
    /// 默认调用接口
    /// </summary>
    [Area("Base")]
    [Route("api/base/default")]
    [Authorize]
    [DisableAuditing]
    public class DefaultAppService : ApplicationService, IDefaultAppService
    {
        private readonly IRepository<Authority, Guid> _repository;
        private readonly IRepository<Menu, Guid> _menuRepository;
        private readonly IRepository<Role, Guid> _roleRepository;
        private readonly IRepository<RoleMenu> _roleMenuRepository;

        private readonly IRepository<EdgeConfig, Guid> _edgeRepository;
        //private readonly IDistributedCache<CurrentAuthorityDto> _currentAuthorityCache;
        private readonly IDistributedCache<AuthorityConfigDto> _configCache;
        private ICurrentUserAuthorityService CurrentUserAuthority;
        private ICacheService _currentAuthorityCache;

        public DefaultAppService(IRepository<Authority, Guid> repository,
            IRepository<Menu, Guid> menuRepository,
            IRepository<Role, Guid> roleRepository,
            IRepository<RoleMenu> roleMenuRepository,
            IRepository<EdgeConfig, Guid> edgeRepository,
            //IDistributedCache<CurrentAuthorityDto> currentAuthorityCache,
            IDistributedCache<AuthorityConfigDto> configCache,
             ICurrentUserAuthorityService currentUserAuthority,
            ICacheService currentAuthorityCache
            )
        {
            _repository = repository;
            _roleMenuRepository = roleMenuRepository;
            _edgeRepository = edgeRepository;
            _menuRepository = menuRepository;
            _roleRepository = roleRepository;
            _currentAuthorityCache = currentAuthorityCache;
            _configCache = configCache;
            CurrentUserAuthority = currentUserAuthority;
            //_cache = cache;
        }

        /// <summary>
        /// 获取用户当前选择的权限工厂(后端接口路由，前端不用调用)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-authroity-config")]
        public async Task<ResultDto<AuthorityConfigDto>> GetCurrentAuthorityConfig(Guid userId, string serviceName)
        {
            var result = new ResultDto<AuthorityConfigDto>();
            var config = new AuthorityConfigDto();//不管有没有网关都要返回值 ，以免业务系统出错        

            try
            {
                var authorityConfig = _currentAuthorityCache.Get<CurrentAuthorityDto>(CacheConsts.CurrentAuthority + userId);

                if (authorityConfig == null)
                {
                    Log.Warning($"{CacheConsts.CurrentAuthority + userId}缓存为null");
                    result.SetData(config);
                    return result;
                }

                //if (authorityConfig == null) //如权限值为空，模拟用户登录一次
                //{
                //    await this.GetCurrentUserAuthority();
                //    authorityConfig = await _currentAuthorityCache.GetAsync(CacheConsts.CurrentAuthority + userId);
                //}

                if (authorityConfig.EdgeConifgs.Any())
                {
                    //根据业务系统找出他的网关配置
                    var configs = authorityConfig.EdgeConifgs.Where(p => p.ServiceValue.Equals(serviceName, StringComparison.OrdinalIgnoreCase)).ToList();

                    if (configs.Any())
                    {
                        config = configs.FirstOrDefault();
                    }
                    else
                    {
                        config.Id = authorityConfig.Id;
                        config.AuthorityName = authorityConfig.AuthorityName;
                        config.AuthType = authorityConfig.AuthType;
                        config.AuthCode = authorityConfig.AuthCode;
                        config.ServiceValue = serviceName;
                    }
                }
                else
                {
                    config.Id = authorityConfig.Id;
                    config.AuthorityName = authorityConfig.AuthorityName;
                    config.AuthType = authorityConfig.AuthType;
                    config.AuthCode = authorityConfig.AuthCode;
                    config.ServiceValue = serviceName;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{CacheConsts.CurrentAuthority + userId}缓存不存在，错误===>{ex.Message}");
            }
            result.SetData(config);
            return result;
        }

        /// <summary>
        /// 获取当前用户的权限对象
        /// </summary>       
        /// <returns></returns>
        [HttpGet]
        [Route("user-authroityids")]
        public async Task<ResultDto<ListResultDto<AuthorityFactoryDto>>> GetCurrentUserAuthority()
        {
            var result = new ResultDto<ListResultDto<AuthorityFactoryDto>>();

            var items = await CurrentUserAuthority.GetAuthoritys((int)AuthType.Company);

            List<AuthorityFactoryDto> list = new();
            if (items.Count > 0)
            {
                int i = 0;
                foreach (var item in items)
                {
                    AuthorityFactoryDto model = new()
                    {
                        Id = item.Id,
                        Name = item.DisplayName,
                        Sort = item.Sort,
                        IsChecked = i == 0
                    };

                    list.Add(model);
                    i++;
                }
                //设置默认选择的工厂权限ID
                await SetCurrentAuthority(list.Where(p => p.IsChecked == true).Select(p => p.Id).FirstOrDefault());
            }

            var dto = new ListResultDto<AuthorityFactoryDto>(list);

            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 设置当前用户的权限对象值（工厂级）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("set-authroityid")]
        public async Task<ResultDto<bool>> SetCurrentAuthority(Guid id)
        {
            var result = new ResultDto<bool>();

            var strKey = CacheConsts.CurrentAuthority + CurrentUser.Id;

            _currentAuthorityCache.Remove(strKey);//清除缓存
                                                  // _currentAuthorityCache.Remove(strKey);//清除缓存          

            var authority = await _repository.GetAsync(id);
            var edges = await _edgeRepository.GetListAsync(p => p.AuthorityId == authority.Id);

            var configList = new List<AuthorityConfigDto>();
            foreach (var edge in edges)
            {
                configList.Add(
                    new AuthorityConfigDto()
                    {
                        Id = authority.Id,
                        AuthorityName = authority.DisplayName,
                        AuthType = authority.AuthType,
                        AuthCode = authority.AuthCode,
                        EdgeName = edge.Name,
                        Address = edge.Address,
                        AppId = edge.AppId,
                        AppSecret = edge.AppSecret,
                        ServiceValue = edge.ServiceValue
                    });
            }

            var currentAuthority = new CurrentAuthorityDto()
            {
                Id = authority.Id,
                AuthorityName = authority.DisplayName,
                AuthType = authority.AuthType,
                AuthCode = authority.AuthCode,
                EdgeConifgs = configList
            };
            //await _currentAuthorityCache.SetAsync(strKey, currentAuthority, PolicyHelper.SetPolicy(TimeSpan.FromDays(BaseConsts.len_30)));//重新生成缓存
            _currentAuthorityCache.Add(strKey, currentAuthority, TimeSpan.FromDays(SystemConsts.len_30));//重新生成缓存

            result.SetData(true);
            return result;
        }

        /// <summary>
        /// 获取当前角色菜单
        /// </summary>
        /// <param name="clientType">客户端类型 0-PC,1-APP,2-H5,5-外链</param>
        /// <returns></returns>
        [HttpGet]
        [Route("menus")]
        public async Task<ResultDto<ListResultDto<RoleMenuDto>>> GetRoleMenus(int clientType = 0)
        {


            try
            {
                var result = new ResultDto<ListResultDto<RoleMenuDto>>();

                List<Guid> roleMenus = new List<Guid>();
                if (CurrentTenant.Id == null && CurrentUser.UserName.ToLower().Equals(SystemConsts.SuperAdmin)) //当前租户为null超级管理员直接获取所有菜单{
                {
                    roleMenus = (await _menuRepository.GetListAsync()).Select(p => p.Id).ToList();
                }
                else if (CurrentTenant.Id == null)
                {
                    var roleIds = await (await _roleRepository.GetQueryableAsync()).Where(p => CurrentUser.Roles.Contains(p.Name)).Select(p => p.Id).ToListAsync();
                    roleMenus = await (await _roleMenuRepository.GetQueryableAsync()).Where(p => roleIds.Contains(p.RoleId)).Select(p => p.MenuId).ToListAsync();
                }
                else
                {
                    //当前租户的实际菜单集合
                    var tenantMenuIds = await (from r in await _roleRepository.GetQueryableAsync()
                                               join rm in await _roleMenuRepository.GetQueryableAsync()
                                               on r.Id equals rm.RoleId
                                               where r.IsStatic == true && r.TenantId == CurrentTenant.Id
                                               select rm.MenuId)
                                               .ToListAsync();

                    var roleIds = await (await _roleRepository.GetQueryableAsync()).Where(p => CurrentUser.Roles.Contains(p.Name)).Select(p => p.Id).ToListAsync();
                    roleMenus = await (await _roleMenuRepository.GetQueryableAsync()).Where(p => roleIds.Contains(p.RoleId) && tenantMenuIds.Contains(p.MenuId)).Select(p => p.MenuId).ToListAsync();
                }


                var menus = await _menuRepository.GetListAsync(p => p.CategoryId <= (int)MenuType.Menu && roleMenus.Contains(p.Id) && p.ClientType == clientType);

                var root = menus.Where(p => p.Pid == null).OrderBy(p => p.Sort).ToList();

                var data = new ListResultDto<RoleMenuDto>(await LoadRoleMenusTree(root, menus));

                result.SetData(data);
                return result;

            }
            catch (Exception ex)
            {
                Log.Information("获取当前角色菜单 （前端调用）GetRoleMenus 报错：" + ex.Message);
                throw new UserFriendlyException(ex.Message);

            }

        }

        private async Task<List<RoleMenuDto>> LoadRoleMenusTree(List<Menu> roots, List<Menu> menus)
        {
            var result = new List<RoleMenuDto>();
            foreach (var root in roots)
            {

                var rootpath = root.Path;

                var menu = new RoleMenuDto
                {
                    Path = rootpath,
                    Name = root.Name,
                    Label = root.Label,
                    Component = root.Component,
                    Meta = new MenuMeta { Icon = root.Icon, Title = root.Name },
                    AlwaysShow = root.AlwaysShow,
                    Hidden = root.Hidden,
                    OtherPlatformCode = root.OtherPlatformCode

                };
                if (menus.Where(p => p.Pid == root.Id).Any())
                {
                    menu.Children = await LoadRoleMenusTree(menus.Where(p => p.Pid == root.Id).OrderBy(p => p.Sort).ToList(), menus);
                }
                result.Add(menu);
            }
            return result;
        }

        /// <summary>
        /// 获取appid的网关配置 (后端接口路由，前端不用调用)
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-edge-config")]
        public async Task<ResultDto<AuthorityConfigDto>> GetAuthorityEdgeConfig(string appId)
        {
            var result = new ResultDto<AuthorityConfigDto>();

            var config = await _configCache.GetOrAddAsync(CacheConsts.AuthorityEdgeConfig + appId,
               async () =>
               {
                   var authoritys = await _repository.GetQueryableAsync();
                   var edges = await _edgeRepository.GetQueryableAsync();
                   return await (from auth in authoritys
                                 join edge in edges
                                 on auth.Id equals edge.AuthorityId
                                 where edge.AppId == appId
                                 select new AuthorityConfigDto()
                                 {
                                     Id = auth.Id,
                                     AuthorityName = auth.DisplayName,
                                     AuthType = auth.AuthType,
                                     AuthCode = auth.AuthCode,
                                     EdgeName = edge.Name,
                                     Address = edge.Address,
                                     AppId = edge.AppId,
                                     AppSecret = edge.AppSecret,
                                     ServiceValue = edge.ServiceValue
                                 }).FirstOrDefaultAsync();

               },
               () => PolicyHelper.SetPolicy(TimeSpan.FromHours(SystemConsts.len_1), false)
               );

            result.SetData(config);

            return result;
        }

        /// <summary>
        /// 获取系统服务
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("products")]
        public ResultDto<ListResultDto<ProductServiceDto>> GetProductService()
        {
            var result = new ResultDto<ListResultDto<ProductServiceDto>>();

            var product = new List<ProductServiceDto>();
            foreach (var p in Enum.GetValues(typeof(ProductService)))
            {
                product.Add(
                    new ProductServiceDto()
                    {
                        ServiceName = ((ProductService)p).GetDescription(),
                        ServiceValue = p.ToString(),

                    }
               );
            }

            var dto = new ListResultDto<ProductServiceDto>(product);
            result.SetData(dto);
            return result;
        }

        /// <summary>
        /// 获取当前权限对象的子集
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("authority-nodes")]
        public async Task<ResultDto<ListResultDto<AuthorityTreeDto>>> GetAuthorityNodes()
        {
            var result = new ResultDto<ListResultDto<AuthorityTreeDto>>();

            var list = new List<AuthorityTreeDto>();

            var authorityConfig = _currentAuthorityCache.Get<CurrentAuthorityDto>(CacheConsts.CurrentAuthority + CurrentUser.Id);
            var roots = await _repository.GetListAsync(p => p.Id == authorityConfig.Id);
            if (roots.Any())
            {
                var authoritys = await _repository.GetListAsync(p => p.CascadeId.StartsWith(roots.FirstOrDefault().CascadeId));
                list = AuthorityTree(roots, authoritys);
            }

            var dto = new ListResultDto<AuthorityTreeDto>(list);

            result.SetData(dto);
            return result;
        }

        private List<AuthorityTreeDto> AuthorityTree(List<Authority> roots, List<Authority> datalist)
        {
            var result = new List<AuthorityTreeDto>();
            foreach (var root in roots.OrderBy(p => p.Sort))
            {
                var tree = new AuthorityTreeDto
                {
                    Id = root.Id,
                    Name = root.DisplayName,
                    Pid = root.Pid,
                    AuthType = root.AuthType,
                    Sort = root.Sort,
                    IsChecked = root.AuthType == (int)AuthType.Company//标记是否是公司
                };
                if (datalist.Where(p => p.Pid == root.Id).Any())
                {
                    tree.Children = AuthorityTree(datalist.Where(p => p.Pid == root.Id).ToList(), datalist);
                }
                else
                {
                    tree.Children = new List<AuthorityTreeDto>();
                }
                result.Add(tree);
            }
            return result;
        }

    }
}
