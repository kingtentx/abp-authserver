using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.MenuManagement.Dto
{
    /// <summary>
    /// 菜单实体
    /// </summary>
    public class MenuDto : EntityDto<Guid>
    {
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// 菜单类型 0-目录 1-菜单 2-按钮
        /// </summary>
        public int CategoryId { get; set; }
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
        /// 菜单路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 菜单控件
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// 菜单路由
        /// </summary>
        public string Route { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        public string ParentLabel { get; set; }
        /// <summary>
        /// 客户端类型 0-PC,1-APP,2-H5,5-外链
        /// </summary>      
        public int ClientType { get; set; }
        /// <summary>
        /// 第三方平台代码
        /// </summary>      
        public string OtherPlatformCode { get; set; }
        /// <summary>
        /// 业务系统
        /// </summary>      
        public string Business { get; set; }
    }
}
