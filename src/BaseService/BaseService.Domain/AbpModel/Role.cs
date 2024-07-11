using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Identity;

namespace BaseService.Systems
{
    public class Role : IdentityRole, IAuthority, IActive
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public override Guid? TenantId { get; protected set; }
        ///// <summary>
        /////  默认角色 
        ///// </summary>
        //public override bool IsDefault { get; set; }
        ///// <summary>
        ///// A static role can not be deleted/renamed
        ///// 禁止删除/重命名静态角色 租户管理员角色
        ///// </summary>
        //public override bool IsStatic { get; set; }
        ///// <summary>
        ///// A user can see other user's public roles
        ///// 用户可以查看其他用户的公共角色
        ///// </summary>
        //public override bool IsPublic { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public virtual int Sort { get; set; }
        /// <summary>
        /// 备注
        /// </summary>      
        public virtual string Remark { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public virtual Guid? AuthorityId { get; set; }

        public Role(Guid id, string name, string normalizedName, Guid? tenantId = null, Guid? authorityId = null)
        {
            Id = id;
            Name = name;
            NormalizedName = normalizedName;
            TenantId = tenantId;
            AuthorityId = authorityId;
        }
    }
}
