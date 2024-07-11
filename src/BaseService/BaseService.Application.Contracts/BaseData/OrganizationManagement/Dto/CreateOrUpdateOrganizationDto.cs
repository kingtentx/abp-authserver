using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.OrganizationManagement.Dto
{
    /// <summary>
    /// 创建/更新 机构实体
    /// </summary>
    public class CreateOrUpdateOrganizationDto : EntityDto<Guid?>
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
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
    }
}
