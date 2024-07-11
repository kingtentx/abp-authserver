using BaseService.ExtModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace BaseService.BaseData
{
    /// <summary>
    /// 字典明细表
    /// </summary> 
    public class DataDictionaryDetail : AuditedAggregateRoot<Guid>, ISoftDelete, IMultiTenant, IAuthority
    {

        /// <summary>
        /// 字典ID
        /// </summary>
        [Comment("字典ID"), Required]
        public Guid DictionaryId { get; set; }
        /// <summary>
        /// 字典值描述
        /// </summary>
        [Comment("字典健"), MaxLength(ModelUnits.Len_256), Required]
        public string Label { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        [Comment("字典值"), MaxLength(ModelUnits.Len_100), Required]
        public string Value { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Comment("排序")]
        public int Sort { get; set; }
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

        public DataDictionaryDetail(Guid id, Guid? tenantId, Guid dictionaryId, string label, string value, int sort, Guid? authorityId)
        {
            TenantId = tenantId;
            Id = id;
            DictionaryId = dictionaryId;
            Label = label;
            Value = value;
            Sort = sort;
            AuthorityId = authorityId;

        }
    }
}
