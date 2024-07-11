using System;
using System.Collections.Generic;

namespace BaseService.Systems.MenuManagement.Dto
{
    public class MenuNodesDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 菜单类型 1-菜单 2-按钮
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 组件名称
        /// </summary>
        public string Component { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 菜单标签名
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 菜单Meta
        /// </summary>
        public NodeMeta Meta { get; set; }
        /// <summary>
        /// 子菜单 集合
        /// </summary>
        public List<MenuNodesDto> Children { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// 允许查看
        /// </summary>
        public bool AlwaysShow { get; set; }

        public Guid? Pid { get; set; }

        public int Sort { get; set; }

        public string Icon { get; set; }

        public string Permission { get; set; }

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

    public class NodeMeta
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
    }
}
