using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Identity;

namespace BaseService.Systems
{
    public class UserRole : IdentityUserRole
    {
        public User User { get; set; } //User 导航属性

        public Role Role { get; set; } //Role 导航属性

        public UserRole(Guid? tenantId, Guid userId, Guid roleId)
            : base(userId, roleId, tenantId)
        { }
    }
}
