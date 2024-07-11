using Cimc.Model.Base;

namespace BaseService.Systems.RoleManagement.Dto
{
    /// <summary>
    /// 查询菜单
    /// </summary>
    public class GetBaseIdentityRoleInput : PagedRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool? IsActive { get; set; }

    }
}
