using System;
using System.Collections.Generic;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace BaseService.Systems.UserManagement.Dto
{
    public class BaseIdentityUserCreateOrUpdateDto : ExtensibleObject
    {
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public virtual string Email { get; set; }
        /// <summary>
        /// 名
        /// </summary>
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxNameLength))]
        public string Name { get; set; }
        /// <summary>
        /// 姓
        /// </summary>
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxSurnameLength))]
        public string Surname { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPhoneNumberLength))]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 是否允许锁定
        /// </summary>
        public bool? LockoutEnabled { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string JobNo { get; set; }
        /// <summary>
        /// 所属机构
        /// </summary>
        public List<Guid> OrganizationIds { get; set; }
        /// <summary>
        /// 所属岗位
        /// </summary>
        public List<Guid> PositionIds { get; set; }

        public List<string> RoleNames { get; set; }

        protected BaseIdentityUserCreateOrUpdateDto() : base(false)
        {

        }
    }
}
