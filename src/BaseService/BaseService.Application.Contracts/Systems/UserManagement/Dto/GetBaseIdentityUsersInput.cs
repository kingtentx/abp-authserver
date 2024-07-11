using Cimc.Model.Base;
using System;

namespace BaseService.Systems.UserManagement.Dto
{
    /// <summary>
    /// 查询用户
    /// </summary>
    public class GetBaseIdentityUsersInput : PagedRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public Guid? OrganizationId { get; set; }
    }
}
