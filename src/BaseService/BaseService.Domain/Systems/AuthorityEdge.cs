using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace BaseService.Systems
{
   
    public class AuthorityEdge : Entity, IMultiTenant, IAuthority
    {
        public Guid? TenantId { get; set; }

        public Guid EdgeId { get; set; }

        public Guid? AuthorityId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { EdgeId, AuthorityId };
        }

        public AuthorityEdge(Guid? tenantId, Guid edgeId, Guid? authorityId)
        {
            TenantId = tenantId;
            EdgeId = edgeId;
            AuthorityId = authorityId;
        }
    }
}
