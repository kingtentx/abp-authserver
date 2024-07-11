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
    /// 岗位表
    /// </summary> 
    public class Position : AuditedAggregateRoot<Guid>, ISoftDelete, IMultiTenant, IAuthority
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_50)]
        public string Name { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Comment("排序")]
        public int Sort { get; set; }
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

        public Position(Guid id, Guid? tenantId, string name, bool isActive, int sort, string description, Guid? authorityId)
        {
            TenantId = tenantId;
            Id = id;
            Name = name;
            IsActive = isActive;
            Sort = sort;
            Description = description;
            AuthorityId = authorityId;
        }
    }
}
