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
    /// <summary>
    /// 权限
    /// </summary>
    public class Authority : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        [Comment("显示名称"), MaxLength(ModelUnits.Len_100)]
        public string DisplayName { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// 级联
        /// </summary>
        [Comment("级联层级"), MaxLength(ModelUnits.Len_256)]
        public string CascadeId { get; set; }
        /// <summary>
        /// 权限属性类型: 0-公司 1-部门
        /// </summary>
        [Comment("0-公司 1-部门")]
        public int AuthType { get; set; }
        /// <summary>
        /// 全名称
        /// </summary>
        [Comment("全名称"), MaxLength(ModelUnits.Len_500)]
        public string FullName { get; set; }
        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool Leaf { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Comment("备注"), MaxLength(ModelUnits.Len_500)]
        public string Remark { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public Guid? GroupId { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        [Comment("权限代码"), MaxLength(ModelUnits.Len_50)]
        public string AuthCode { get; set; }
        public Authority(Guid id, Guid? tenantId, [NotNull] string displayName, int authType, Guid? pid, string fullName, int sort, bool leaf, bool isActive, string remark, Guid? groupId, string authCode)
        {
            Id = id;
            TenantId = tenantId;
            DisplayName = displayName;
            AuthType = authType;
            Pid = pid;
            FullName = fullName;
            Sort = sort;
            IsActive = isActive;
            Leaf = leaf;
            Remark = remark;
            GroupId = groupId;
            AuthCode = authCode;
        }
    }
}
