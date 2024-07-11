using Cimc.Model.Base;
using System;

namespace BaseService.BaseData.OrganizationManagement.Dto
{
    /// <summary>
    /// 查询机构
    /// </summary>
    public class GetOrganizationInputDto : PagedRequestDto
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 机构类型：0-集团 1-基地 2-公司 3-部门
        /// </summary>
        public int? OrgType { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
    }
}
