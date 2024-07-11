using Cimc.Model.Base;

namespace BaseService.BaseData.DataDictionaryManagement.Dto
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class GetDictionaryInputDto : PagedRequestDto
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }
    }
}
