using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.AuthorityManagerment.Dto
{
    public class CreateOrUpdateAuthorityGroupDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 显示名称
        /// </summary>    
        public string GroupName { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
    }
}
