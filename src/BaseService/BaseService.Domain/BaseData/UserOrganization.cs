using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace BaseService.BaseData
{
    /// <summary>
    /// 用户机构表
    /// </summary> 
    public class UserOrganization : Entity, IMultiTenant, IAuthority
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 用记ID
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid? AuthorityId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { UserId, OrganizationId };
        }

        public UserOrganization(Guid? tenantId, Guid userId, Guid organizationId, Guid? authorityId)
        {
            TenantId = tenantId;
            UserId = userId;
            OrganizationId = organizationId;
            AuthorityId = authorityId;
        }
    }
}
