using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.MenuManagement.Dto
{
    /// <summary>
    /// 创建/修改 菜单实体
    /// </summary>
    public class CreateOrUpdateMenuDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid? FormId { get; set; }
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
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 菜单标签名
        /// </summary>
        [Required]
        public string Label { get; set; }
        /// <summary>
        /// 菜单排序
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
        /// 权限代码
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// 总是显示
        /// </summary>
        public bool AlwaysShow { get; set; }
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
