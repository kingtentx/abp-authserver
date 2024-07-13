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
        /// <summary>
        /// 菜单类型 0-菜单/1-iframe/2-外链/3-按钮
        /// </summary>
        [Comment("菜单类型")]
        public int MenuType { get; set; }
        /// <summary>
        /// 菜单或自定义信息允许添加到标签页
        /// </summary>
        public bool HigherMenuOptions { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// 菜单中文名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_50)]
        public string Title { get; set; }
        /// <summary>
        /// 路由
        /// </summary>
        [Comment("路由名称"), MaxLength(ModelUnits.Len_256)]
        public string Route { get; set; }
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
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 路由重定向
        /// </summary>
        [Comment("路由重定向"), MaxLength(ModelUnits.Len_256)]
        public string Redirect { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        [Comment("图标"), MaxLength(ModelUnits.Len_50)]
        public string Icon { get; set; }
        /// <summary>
        /// 扩展图标
        /// </summary>
        [Comment("扩展图标"), MaxLength(ModelUnits.Len_50)]
        public string ExtraIcon { get; set; }
        /// <summary>
        /// 页面进场加载动画
        /// </summary>
        [Comment("页面进场加载动画"), MaxLength(ModelUnits.Len_100)]
        public string EnterTransition { get; set; }
        /// <summary>
        /// 页面离场加载动画
        /// </summary>
        [Comment("页面离场加载动画"), MaxLength(ModelUnits.Len_100)]
        public string LeaveTransition { get; set; }
        /// <summary>
        /// 菜单激活
        /// </summary>
        [Comment("菜单激活"), MaxLength(ModelUnits.Len_100)]
        public string ActivePath { get; set; }
        /// <summary>
        /// 权限
        /// </summary>
        [Comment("权限"), MaxLength(ModelUnits.Len_100)]
        public string Auths { get; set; }
        /// <summary>
        /// iframe 链接地址
        /// </summary>
        [Comment("iframe 链接地址"), MaxLength(ModelUnits.Len_256)]
        public string FrameSrc { get; set; }
        /// <summary>
        /// iframe加载动画
        /// </summary>
        [Comment("iframe加载动画"), MaxLength(ModelUnits.Len_256)]
        public string FrameLoading { get; set; }
        /// <summary>
        /// 是否缓存页面
        /// </summary>
        public bool KeepAlive { get; set; }
        /// <summary>
        /// 是否允许添加到标签页
        /// </summary>
        public bool HiddenTag { get; set; }
        /// <summary>
        /// 是否固定显示标签页
        /// </summary>
        public bool FixedTag { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool ShowLink { get; set; }
        /// <summary>
        /// 是否显示父级菜单
        /// </summary>
        public bool ShowParent { get; set; } 

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 客户端类型 0-PC,1-APP,2-H5,5-外链
        /// </summary>
        [Comment("客户端类型 0-PC,1-APP,2-H5")]
        public int ClientType { get; set; }
       

        public Menu(Guid id) : base(id)
        {

        }
    }
}
