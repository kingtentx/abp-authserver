using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BaseService.Systems
{
 
    public class EdgeConfig : FullAuditedAggregateRoot<Guid>, IMultiTenant, IAuthority
    {
        /// <summary>
        /// 网关名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_50)]
        public string Name { get; set; }
        /// <summary>
        /// 访问地址
        /// </summary>
        [Comment("网关地址"), MaxLength(ModelUnits.Len_50)]
        public string Address { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        [Comment("应用ID"), MaxLength(ModelUnits.Len_50)]
        public string AppId { get; set; }
        /// <summary>
        /// 应用密钥
        /// </summary>
        [Comment("应用密钥"), MaxLength(ModelUnits.Len_50)]
        public string AppSecret { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注"), MaxLength(ModelUnits.Len_500)]
        public string Remark { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 权限ID 
        /// </summary>
        public Guid? AuthorityId { get; set; }
        /// <summary>
        /// 系统产品
        /// </summary>
        [Comment("系统服务"), MaxLength(ModelUnits.Len_20)]
        public string ServiceValue { get; set; }

        public EdgeConfig(Guid id, Guid? tenantId, [NotNull] string name, [NotNull] string address, [NotNull] string appId, [NotNull] string appSecret, string remark, bool isActive, Guid? authorityId, string serviceValue)
        {
            TenantId = tenantId;
            Id = id;
            Address = address;
            Name = name;
            AppId = appId;
            AppSecret = appSecret;
            Remark = remark;
            IsActive = isActive;
            AuthorityId = authorityId;
            ServiceValue = serviceValue;
        }
    }
}
