using System;

namespace BaseService.Systems.RoleMenusManagement.Dto
{
    public class AuthorityArrary
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid AuthId { get; set; }
        /// <summary>
        /// 系统类型：0-集团 1-基地 2-公司 3-部门
        /// </summary>
        public int AuthType { get; set; }
    }

}
