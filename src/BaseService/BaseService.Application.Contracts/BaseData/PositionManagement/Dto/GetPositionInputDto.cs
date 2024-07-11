using Cimc.Model.Base;

namespace BaseService.BaseData.PositionManagement.Dto
{
    /// <summary>
    /// 查询岗位
    /// </summary>
    public class GetPositionInputDto : PagedRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
    }
}
