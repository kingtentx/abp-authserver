using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace BaseService.Systems
{

    public class RoleAuthority : Entity, IMultiTenant, IAuthority
    {
        public Guid? TenantId { get; set; }

        public Guid RoleId { get; set; }

        public Guid? AuthorityId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { RoleId, AuthorityId };
        }

        public RoleAuthority(Guid? tenantId, Guid roleId, Guid? authorityId)
        {
            TenantId = tenantId;
            RoleId = roleId;
            AuthorityId = authorityId;
        }
    }
}
