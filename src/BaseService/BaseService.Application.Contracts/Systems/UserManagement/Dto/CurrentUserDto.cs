using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.UserManagement.Dto
{
    /// <summary>
    /// 用户登录信息
    /// </summary>
    public class CurrentUserDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuthenticated { get; set; }
        /// <summary>
        /// 用户登录名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 用户姓
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 用户角色集合
        /// </summary>
        public List<UserInRoles> Roles { get; set; }
        /// <summary>
        /// 用户权限集合
        /// </summary>
        public List<UserInAuthoritys> Auths { get; set; }
    }

    public class UserInRoles
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
    }

    public class UserInAuthoritys
    {
        public Guid AuthorityId { get; set; }
        public string AuthorityName { get; set; }
        public bool IsChecked { get; set; }

    }
}
