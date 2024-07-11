using BaseService.Systems;
using BaseService.Systems.AuthorityManagerment.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseService.CurrentAuthorityService
{
    /// <summary>
    /// 当前用户所拥有的所有工厂权限对象
    /// </summary>
    public interface ICurrentUserAuthorityService
    {
        /// <summary>
        /// 当前用户的权限对象
        /// </summary>
        /// <returns></returns>
        Task<List<AuthorityDto>> GetAuthoritys(int? authType);
        /// <summary>
        /// 当前角色的权限对象
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<AuthorityDto>> GetAuthoritys(Guid roleId);

        /// <summary>
        /// 权限对象（树）
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<AuthorityTreeDto>> GetAuthorityTree(Guid roleId);


    }
}
