using Cimc.Model.Base;

namespace BaseService.Systems.MenuManagement.Dto
{
    /// <summary>
    /// 查询菜单
    /// </summary>
    public class GetMenuInputDto : PagedRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
    }
}
