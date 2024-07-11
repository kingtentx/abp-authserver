
using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.OrganizationManagement.Dto
{
    /// <summary>
    /// 岗位实体
    /// </summary>
    public class OrganizationDto : EntityDto<Guid>
    {
        /// <summary>
        /// 机构类型：0-集团 1-基地 2-公司 3-部门
        /// </summary>
        public int OrgType { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// 机构名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 是否有子集
        /// </summary>
        public bool HasChildren { get; set; }
        /// <summary>
        /// 是否叶子节点
        /// </summary>
        public bool Leaf { get; set; }

        #region   >扩展字段<
        /// <summary>
        /// 
        /// </summary>
        public string Label { get; set; }
        #endregion
    }
}
