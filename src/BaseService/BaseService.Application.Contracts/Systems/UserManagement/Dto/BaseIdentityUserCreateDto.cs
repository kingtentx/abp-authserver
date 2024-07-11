using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

namespace BaseService.Systems.UserManagement.Dto
{
    ///// <summary>
    ///// 创建用户实体
    ///// </summary>
    //public class BaseIdentityUserCreateDto : IdentityUserCreateDto
    //{
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
    /// 创建用户实体
    /// </summary>
    public class BaseIdentityUserCreateDto : BaseIdentityUserCreateOrUpdateDto
    {
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        public string UserName { get; set; }

        [DisableAuditing]
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        public string Password { get; set; }


        [Required]
        [EmailAddress]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public override string Email { get; set; }

        public List<Guid> Roles { get; set; }
    }
}
