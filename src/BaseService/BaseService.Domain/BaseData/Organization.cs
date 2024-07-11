using BaseService.ExtModels;
using JetBrains.Annotations;
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
    /// 机构表
    /// </summary>
    public class Organization : AuditedAggregateRoot<Guid>, ISoftDelete, IMultiTenant, IAuthority
    {
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 机构类型：0-集团 1-基地 2-公司 3-部门
        /// </summary>
        [Comment("机构类型：0-集团 1-基地 2-公司 3-部门")]
        public int OrgType { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        [Comment("名称"), MaxLength(ModelUnits.Len_50)]
        public string Name { get; set; }
        /// <summary>
        /// 全名称
        /// </summary>
        [Comment("全名称"), MaxLength(ModelUnits.Len_256)]
        public string FullName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Comment("排序")]
        public int Sort { get; set; }
        /// <summary>
        /// 是否叶子节点
        /// </summary>
        [Comment("是否叶子节点")]
        public bool Leaf { get; set; }
        /// <summary>
        /// 级联
        /// </summary>
        [Comment("级联层级"), MaxLength(ModelUnits.Len_500)]
        public string CascadeId { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid? AuthorityId { get; set; }

        public Organization(Guid id, Guid? tenantId, int orgType, Guid? pid, [NotNull] string name, string fullName, int sort, bool leaf, bool isActive, Guid? authorityId)
        {
            TenantId = tenantId;
            Id = id;
            OrgType = orgType;
            Pid = pid;
            Name = name;
            FullName = fullName;
            Sort = sort;
            IsActive = isActive;
            Leaf = leaf;
            AuthorityId = authorityId;
        }
    }
}
