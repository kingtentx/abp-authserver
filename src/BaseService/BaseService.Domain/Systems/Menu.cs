using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace BaseService.Systems
{
    /// <summary>
    /// 菜单表
    /// </summary>
    public class Menu : AuditedAggregateRoot<Guid>, ISoftDelete
    {

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
        /// 菜单中文名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_50)]
        public string Name { get; set; }
        /// <summary>
        /// 菜单标签
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_100)]
        public string Label { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 菜单路径
        /// </summary>
        [Comment("菜单路径"), MaxLength(ModelUnits.Len_256)]
        public string Path { get; set; }
        /// <summary>
        /// 组件名称
        /// </summary>        
        [Comment("组件名称"), MaxLength(ModelUnits.Len_256)]
        public string Component { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        [Comment("权限"), MaxLength(ModelUnits.Len_100)]
        public string Permission { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [Comment("图标"), MaxLength(ModelUnits.Len_50)]
        public string Icon { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// 总是显示
        /// </summary>
        public bool AlwaysShow { get; set; }

        public bool IsDeleted { get; set; }
        /// <summary>
        /// 客户端类型 0-PC,1-APP,2-H5,5-外链
        /// </summary>
        [Comment("客户端类型 0-PC,1-APP,2-H5,5-外链")]
        public int ClientType { get; set; }
        /// <summary>
        /// 第三方平台代码
        /// </summary>
        [Comment("第三方平台代码"), MaxLength(ModelUnits.Len_100)]
        public string OtherPlatformCode { get; set; }
        /// <summary>
        /// 业务系统
        /// </summary>
        [Comment("业务系统"), MaxLength(ModelUnits.Len_100)]
        public string Business { get; set; }

        public Menu(Guid id) : base(id)
        {

        }
    }
}
