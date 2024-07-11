using System;
using System.Collections.Generic;
using Volo.Abp.Identity;

namespace BaseService.Systems.UserManagement.Dto
{
    /// <summary>
    /// 用户实体
    /// </summary>
    public class BaseIdentityUserDto : IdentityUserDto
    {
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string JobNo { get; set; }
        /// <summary>
        /// 所属岗位
        /// </summary>
        public List<Guid> PositionIds { get; set; }
        /// <summary>
        /// 所属机构
        /// </summary>
        public List<Guid> OrganizationIds { get; set; }
        /// <summary>
        /// 角色ID集合
        /// </summary>
        public List<Guid> Roles { get; set; }

        #region   >扩展字段<
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleNames { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string OrganizationNames { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string PositionNames { get; set; }

        #endregion
    }
}
