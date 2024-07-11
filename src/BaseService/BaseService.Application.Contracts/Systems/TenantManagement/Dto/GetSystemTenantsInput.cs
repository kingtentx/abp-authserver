using Cimc.Model.Base;

namespace BaseService.Systems.TenantManagement.Dto
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class GetSystemTenantsInput : PagedRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
    }
}
