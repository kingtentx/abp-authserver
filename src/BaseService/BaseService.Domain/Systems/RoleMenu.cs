using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace BaseService.Systems
{

    public class RoleMenu : Entity, IMultiTenant
    {
        public Guid? TenantId { get; set; }

        public Guid RoleId { get; set; }

        public Guid MenuId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { RoleId, MenuId };
        }

        public RoleMenu(Guid? tenantId, Guid roleId, Guid menuId)
        {
            TenantId = tenantId;
            RoleId = roleId;
            MenuId = menuId;
        }
    }
}
