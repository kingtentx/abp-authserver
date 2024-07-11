using System.ComponentModel;

namespace BaseService.Enums
{
    /// <summary>
    /// 系统产品服务
    /// </summary>
    public enum ProductService
    {
        /// <summary>
        /// 节拍系统
        /// </summary>
        [Description("基础服务")]
        BaseService,
        /// <summary>
        /// 节拍系统
        /// </summary>
        [Description("节拍服务")]
        ADSSysService,
        /// <summary>
        /// 能源系统
        /// </summary>
        [Description("能源服务")]
        EnergyService,
        /// <summary>
        /// 消防系统
        /// </summary>
        [Description("消防服务")]
        HSEService

    }

    /// <summary>
    /// 系统角色类型
    /// </summary>
    public enum ProviderNameType
    {
        /// <summary>
        /// 角色
        /// </summary>
        R,
        /// <summary>
        /// 用户
        /// </summary>
        U
    }

    /// <summary>
    /// 权限对象类型
    /// </summary>
    public enum AuthType
    {
        /// <summary>
        /// 公司（工厂）
        /// </summary>
        Company = 0,
        /// <summary>
        /// 部门
        /// </summary>
        Department = 1
    }

    /// <summary>
    /// 系统机构类型：0-集团 1-基地 2-公司 3-部门
    /// </summary>
    public enum SystemOrgType
    {
        /// <summary>
        /// 根组织
        /// </summary>
        Root = 0,
        /// <summary>
        /// 基地
        /// </summary>
        Group = 1,
        /// <summary>
        /// 公司
        /// </summary>
        Company = 2,
        /// <summary>
        /// 部门
        /// </summary>
        Department = 3

    }


    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 模块
        /// </summary>
        Module = 0,
        /// <summary>
        /// 菜单
        /// </summary>
        Menu = 1,
        /// <summary>
        /// 按钮
        /// </summary>
        Button = 2

    }
}
