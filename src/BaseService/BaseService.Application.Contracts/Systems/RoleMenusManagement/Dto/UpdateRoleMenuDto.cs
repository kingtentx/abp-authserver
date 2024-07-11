using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseService.Systems.RoleMenusManagement.Dto
{
    /// <summary>
    /// 更新角色菜单
    /// </summary>
    public class UpdateRoleMenuDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }
        /// <summary>
        /// 菜单集合
        /// </summary>
        public List<Guid> MenuIds { get; set; }
        /// <summary>
        /// 权限对象集合
        /// </summary>
        public List<Guid> AuthorityIds { get; set; }

        /// <summary>
        /// 客户端类型 0-PC,1-APP,2-H5,5-外链
        /// </summary>
        [Required]
        public int ClientType { get; set; }

        ///// <summary>
        ///// 权限代码
        ///// </summary>
        //public UpdatePermissionDto[] Permissions { get; set; }

    }
}
