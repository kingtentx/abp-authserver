using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BaseService.BaseData
{
    /// <summary>
    /// 字典表
    /// </summary> 
    public class DataDictionary : AuditedAggregateRoot<Guid>, ISoftDelete, IMultiTenant, IAuthority
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_100),Required]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Comment("描述"), MaxLength(ModelUnits.Len_256)]
        public string Description { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid? AuthorityId { get; set; }

        public DataDictionary(Guid id, Guid? tenantId, [NotNull] string name, string description, Guid? authorityId)
        {
            TenantId = tenantId;
            Id = id;
            Name = name;
            Description = description;
            AuthorityId = authorityId;
        }
    }
}
