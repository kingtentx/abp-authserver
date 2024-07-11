using BaseService.Systems.AuthorityManagerment.Dto;
using BaseService.Systems.UserManagement.Dto;

using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BaseService.InternalApi
{
    public interface IInternalAppService : IApplicationService
    {
        Task<ResultDto<BaseIdentityUserDto>> GetUserInfoAsync(Guid? tenantId, Guid userId);

        Task<ResultDto<AuthorityConfigDto>> GetTenantAuthorityEdgeConfig(Guid? tenantId, string appId);

        /// <summary>
        /// 根据权限Id获取用户列表，用户名称转换
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="authorityId"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<BaseIdentityUserDto>> GetUserList(Guid? tenantId, Guid authorityId, string filter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="AuthorityId"></param>
        /// <param name="userIds"></param>
        /// <returns></returns>
        Task<List<BaseIdentityUserDto>> GetUserList(Guid? tenantId, Guid AuthorityId, List<Guid> userIds);
    }
}
