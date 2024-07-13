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
        /// 菜单类型 0-菜单/1-iframe/2-外链/3-按钮
        /// </summary>      
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
        public string Title { get; set; }
        /// <summary>
        /// 路由
        /// </summary>      
        public string Route { get; set; }
        /// <summary>
        /// 菜单路径
        /// </summary>     
        public string Path { get; set; }
        /// <summary>
        /// 组件名称
        /// </summary>              
        public string Component { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 路由重定向
        /// </summary>      
        public string Redirect { get; set; }
        /// <summary>
        /// 图标
        /// </summary>     
        public string Icon { get; set; }
        /// <summary>
        /// 扩展图标
        /// </summary>      
        public string ExtraIcon { get; set; }
        /// <summary>
        /// 页面进场加载动画
        /// </summary>   
        public string EnterTransition { get; set; }
        /// <summary>
        /// 页面离场加载动画
        /// </summary>      
        public string LeaveTransition { get; set; }
        /// <summary>
        /// 菜单激活
        /// </summary>     
        public string ActivePath { get; set; }
        /// <summary>
        /// 权限
        /// </summary>       
        public string Auths { get; set; }
        /// <summary>
        /// iframe 链接地址
        /// </summary>       
        public string FrameSrc { get; set; }
        /// <summary>
        /// iframe加载动画
        /// </summary>       
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

        /// <summary>
        /// 客户端类型 0-PC,1-APP,2-H5,5-外链
        /// </summary>     
        public int ClientType { get; set; }
    }
}
