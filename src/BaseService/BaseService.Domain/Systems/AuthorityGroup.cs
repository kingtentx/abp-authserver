using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BaseService.Systems
{
    public class AuthorityGroup : AuditedEntity<Guid>, IMultiTenant
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_100)]
        public string GroupName { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        public AuthorityGroup(Guid id, string groupName, Guid? tenantId) : base(id)
        {
            GroupName = groupName;
            TenantId = tenantId;
        }
    }
}
