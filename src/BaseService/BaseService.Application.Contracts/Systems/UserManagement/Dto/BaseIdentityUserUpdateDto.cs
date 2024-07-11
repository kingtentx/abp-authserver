using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace BaseService.Systems.UserManagement.Dto
{
    ///// <summary>
    ///// 更新用户实体
    ///// </summary>
    //public class BaseIdentityUserUpdateDto : IdentityUserUpdateDto
    //{
    //    /// <summary>
    //    /// Id
    //    /// </summary>
    //    public Guid Id { get; set; }
    //    /// <summary>
    //    /// 性别
    //    /// </summary>
    //    public string Sex { get; set; }

    //    /// <summary>
    //    /// 归属根组织ID orgId 
    //    /// </summary>
    //    public Guid? RootOrgId { get; set; }
    //    /// <summary>
    //    /// 所属机构
    //    /// </summary>
    //    public List<Guid> OrganizationIds { get; set; }
    //    /// <summary>
    //    /// 所属岗位
    //    /// </summary>
    //    public List<Guid> PositionIds { get; set; }

    //}

    /// <summary>
    /// 更新用户实体
    /// </summary>
    public class BaseIdentityUserUpdateDto : BaseIdentityUserCreateOrUpdateDto
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        [DisableAuditing]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        public string Password { get; set; }
        /// <summary>
        /// 并发戳
        /// </summary>
        public string ConcurrencyStamp { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public override string Email { get; set; }
        /// <summary>
        /// 角色ID集合
        /// </summary>
        public List<Guid> Roles { get; set; }

    }

}
