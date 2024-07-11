using Cimc.Model.Base;
using System;

namespace BaseService.Systems.AuthorityManagerment.Dto
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class GetAuthorityDetailInputDto : PagedRequestDto
    {
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid AuthorityId { get; set; }
    }
}
