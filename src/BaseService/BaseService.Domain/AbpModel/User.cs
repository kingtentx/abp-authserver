using BaseService.ExtModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Users;

namespace BaseService.Systems
{
    public class User : FullAuditedAggregateRoot<Guid>, IUser, IAuthority
    {
        #region Base properties

        /* These properties are shared with the IdentityUser entity of the Identity module.
         * Do not change these properties through this class. Instead, use Identity module
         * services (like IdentityUserManager) to change them.
         * So, this properties are designed as read only!
         */

        public virtual Guid? TenantId { get; private set; }

        public virtual string UserName { get; private set; }

        public virtual string Name { get; private set; }

        public virtual string Surname { get; private set; }

        public virtual string Email { get; private set; }

        public virtual bool EmailConfirmed { get; private set; }

        public virtual string PhoneNumber { get; private set; }

        public virtual bool PhoneNumberConfirmed { get; private set; }

        public virtual bool IsActive { get; private set; }

        #endregion
        /// <summary>
        /// 性别
        /// </summary>        
        public virtual string Sex { get; set; }
        /// <summary>
        /// 工号
        /// </summary>        
        public virtual string JobNo { get; set; }
        /// <summary>
        /// 权限对象
        /// </summary>
        public virtual Guid? AuthorityId { get; set; }

        public virtual ICollection<UserRole> Roles { get; private set; }

        public User(Guid id, [NotNull] string userName, [NotNull] string email, string phoneNumber, Guid? tenantId)
        {
            Check.NotNull(userName, nameof(userName));
            Check.NotNull(email, nameof(email));
            Id = id;
            TenantId = tenantId;
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            IsActive = true;

            Roles = new Collection<UserRole>();
        }

        public virtual void AddRole(Guid roleId)
        {
            Check.NotNull(roleId, nameof(roleId));

            if (IsInRole(roleId))
            {
                return;
            }

            Roles.Add(new UserRole(TenantId, Id, roleId));
        }

        public virtual void AddRole(List<Guid> roleIds)
        {
            foreach (var roleId in roleIds)
            {
                AddRole(roleId);
            }
        }

        public virtual void RemoveRole(Guid roleId)
        {
            Check.NotNull(roleId, nameof(roleId));

            if (!IsInRole(roleId))
            {
                return;
            }

            Roles.RemoveAll(r => r.RoleId == roleId);
        }

        public virtual void RemoveRoleNotInList(List<Guid> roleIds)
        {
            Roles.RemoveAll(r => !roleIds.Contains(r.RoleId));
        }

        public virtual bool IsInRole(Guid roleId)
        {
            Check.NotNull(roleId, nameof(roleId));

            return Roles.Any(r => r.RoleId == roleId);
        }
    }
}
