using System;
using Volo.Abp.TenantManagement;

namespace BaseService.Systems.TenantManagement.Dto
{
    public class SystemTenantUpdateDto : TenantCreateOrUpdateDtoBase
    {
        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string AdminPassword { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string AdminEmailAddress { get; set; }

        public string ConcurrencyStamp { get; set; }
    }
}
