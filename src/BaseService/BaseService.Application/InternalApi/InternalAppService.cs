using BaseService.Consts;
using BaseService.Systems;
using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.UserManagement.Dto;
using Cimc.Helper;
using Cimc.Model.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace BaseService.InternalApi
{
    /// <summary>
    /// 内部服务api （后端调用）
    /// </summary>
    //[RemoteService(false)]
    [Area("Base")]
    [Route("api/base/internal")]
    [DisableAuditing]
    public class InternalAppService : ApplicationService, IInternalAppService
    {
        private readonly IDistributedCache<AuthorityConfigDto> _configCache;
        private readonly IRepository<Authority, Guid> _repository;
        private readonly IRepository<GatewayConfig, Guid> _edgeRepository;
        private readonly IRepository<User, Guid> _userRepository;

        public InternalAppService(
            IDistributedCache<AuthorityConfigDto> configCache,
            IRepository<Authority, Guid> repository,
            IRepository<GatewayConfig, Guid> edgeRepository,
            IRepository<User, Guid> userRepository
         )
        {
            _configCache = configCache;
            _repository = repository;
            _edgeRepository = edgeRepository;
            _userRepository = userRepository;

        }

        /// <summary>
        /// 用户ID获取用户信息 （后端调用）
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("userinfo")]
        public async Task<ResultDto<BaseIdentityUserDto>> GetUserInfoAsync(Guid? tenantId, Guid userId)
        {
            var result = new ResultDto<BaseIdentityUserDto>();

            using (CurrentTenant.Change(tenantId))
            {
                var user = await _userRepository.FindAsync(userId);
                if (user != null)
                {
                    var dto = ObjectMapper.Map<User, BaseIdentityUserDto>(user);
                    result.SetData(dto);
                }
                else
                {
                    result.Message = "用户不存在";
                }
            }
            return result;
        }

        /// <summary>
        /// 获取租户的appid的权限网关配置（后端调用）
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>      
        [HttpGet]
        [Route("tenant-edge-config")]
        public async Task<ResultDto<AuthorityConfigDto>> GetTenantAuthorityEdgeConfig(Guid? tenantId, string appId)
        {
            var result = new ResultDto<AuthorityConfigDto>();

            using (CurrentTenant.Change(tenantId))
            {
                var config = await _configCache.GetOrAddAsync(CacheConsts.AuthorityEdgeConfig + appId,
                   async () =>
                   {
                       var authoritys = (await _repository.GetQueryableAsync()).Where(p => p.TenantId == tenantId);
                       var edges = (await _edgeRepository.GetQueryableAsync()).Where(p => p.TenantId == tenantId);

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
                   () => PolicyHelper.SetPolicy(TimeSpan.FromMinutes(SystemConsts.len_1), false)
               );
                result.SetData(config);
            }
            return result;
        }


        /// <summary>
        /// 根据权限Id获取用户列表，用户名称转换
        /// Sam 2023年2月6日09:41:28
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="authorityId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("user-list")]
        public async Task<List<BaseIdentityUserDto>> GetUserList(Guid? tenantId, Guid authorityId, string filter)
        {
            List<BaseIdentityUserDto> result = new List<BaseIdentityUserDto>();

            using (CurrentTenant.Change(tenantId))
            {
                var query = (await _userRepository.GetQueryableAsync()).Where(w => w.AuthorityId == authorityId)
              .WhereIf(!string.IsNullOrWhiteSpace(filter), w => w.UserName.Contains(filter) || w.Name.Contains(filter));

                var items = await query.ToListAsync();


                result = ObjectMapper.Map<List<User>, List<BaseIdentityUserDto>>(items);

            }
            return result;
        }

        /// <summary>
        /// 根据用户id集合，查询用户信息
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="AuthorityId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("user-list-byid")]
        public async Task<List<BaseIdentityUserDto>> GetUserList(Guid? tenantId, Guid AuthorityId, List<Guid> userIds)
        {
            List<BaseIdentityUserDto> result = new List<BaseIdentityUserDto>();

            using (CurrentTenant.Change(tenantId))
            {
                var items = await _userRepository.GetListAsync(p => userIds.Contains(p.Id) && p.AuthorityId == AuthorityId);

                result = ObjectMapper.Map<List<User>, List<BaseIdentityUserDto>>(items);

            }
            return result;
        }
    }
}
