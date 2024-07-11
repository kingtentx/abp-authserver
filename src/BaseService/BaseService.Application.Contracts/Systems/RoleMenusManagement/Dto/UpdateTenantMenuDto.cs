using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseService.Systems.RoleMenusManagement.Dto
{
    /// <summary>
    /// 更新租户菜单
    /// </summary>
    public class UpdateTenantMenuDto
    {
        /// <summary>
        /// 菜单集合
        /// </summary>
        public List<Guid> MenuIds { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        [Required]
        public Guid TenantId { get; set; }

        /// <summary>
        /// 客户端类型 0-PC,1-APP,2-H5,5-外链
        /// </summary>
        [Required]
        public int ClientType { get; set; }
    }
}
