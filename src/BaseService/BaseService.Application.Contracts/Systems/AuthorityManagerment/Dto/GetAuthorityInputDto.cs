using Cimc.Model.Base;
using System;

namespace BaseService.Systems.AuthorityManagerment.Dto
{   /// <summary>
    /// 查询参数
    /// </summary>
    public class GetAuthorityInputDto : PagedRequestDto
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 权限属性类型: 0-集团 1-基地 2-公司 3-部门
        /// </summary>      
        public int? AuthType { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }

    }
}
