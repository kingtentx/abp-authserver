using System;
using Volo.Abp.Identity;

namespace BaseService.Systems.RoleManagement.Dto
{
    /// <summary>
    /// 更新角色实体
    /// </summary>
    public class BaseIdentityRoleUpdateDto : IdentityRoleUpdateDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        ///// <summary>
        ///// 权限对象集合
        ///// </summary>
        //public List<AuthorityArrary> AuthorityIds { get; set; }
    }
}
