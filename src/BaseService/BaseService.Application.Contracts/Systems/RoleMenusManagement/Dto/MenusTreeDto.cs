using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.RoleMenusManagement.Dto
{
    /// <summary>
    /// 菜单树
    /// </summary>
    public class MenusTreeDto : EntityDto<Guid>
    {
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// 菜单代码
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// 客户端类型 0-PC,1-APP,2-H5,5-外链
        /// </summary>
        public int ClientType { get; set; }
    }
}
