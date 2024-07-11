using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BaseService.Systems
{
    public class AuthorityDetail : AuditedAggregateRoot<Guid>, ISoftDelete, IMultiTenant, IAuthority
    {
        /// <summary>
        ///数据项名称 
        /// </summary>
        [Comment("数据项名称"), MaxLength(ModelUnits.Len_100)]
        public string DataName { get; set; }
        /// <summary>
        ///数据项ID 
        /// </summary>
        [Comment("数据项ID")]
        public Guid DataItemId { get; set; }
        /// <summary>
        /// 数据类型 0-设备
        /// </summary>
        public int DataType { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid? AuthorityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }

        public AuthorityDetail(Guid id, Guid? tenantId, Guid? authorityId, [NotNull] Guid dataItemId, string dataName, int dataType)
        {
            Id = id;
            TenantId = tenantId;
            AuthorityId = authorityId;
            DataItemId = dataItemId;
            DataName = dataName;
            DataType = dataType;
        }

    }
}
