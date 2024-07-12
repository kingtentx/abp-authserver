using Cimc.Model.Base;

namespace BaseService.Systems.EdgeConfigManagement.Dto
{
    public class GetGatewayConfigInputDto : PagedRequestDto
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }
    }
}
