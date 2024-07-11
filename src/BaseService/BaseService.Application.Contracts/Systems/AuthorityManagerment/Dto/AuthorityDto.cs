using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.AuthorityManagerment.Dto
{
    public class AuthorityDto : AuditedEntityDto<Guid>
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// 权限属性类型: 0-公司 1-部门
        /// </summary>      
        public int AuthType { get; set; }
        /// <summary>
        /// 全名称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool Leaf { get; set; }
        /// <summary>
        /// 级连关系
        /// </summary>
        public string CascadeId { get; set; }
        /// <summary>
        /// 是否有独立访问地址
        /// </summary>
        ///public bool IsHasAddress { get; set; }
        /// <summary>
        /// 备注
        /// </summary>       
        public string Remark { get; set; }
        ///// <summary>
        ///// 租户ID
        ///// </summary>
        //public Guid? TenantId { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 分组ID
        /// </summary>
        public Guid? GroupId { get; set; }
        /// <summary>
        /// 分组名称
        /// </summary>
        public virtual string GroupName { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        public string AuthCode { get; set; }
    }
}
