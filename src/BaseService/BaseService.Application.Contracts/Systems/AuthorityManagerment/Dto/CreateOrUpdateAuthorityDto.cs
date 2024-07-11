using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.AuthorityManagerment.Dto
{
    /// <summary>
    /// 创建/更新 权限实体
    /// </summary>
    public class CreateOrUpdateAuthorityDto : EntityDto<Guid?>
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
        /// 权限属性类型: 0-集团 1-基地 2-公司 3-部门
        /// </summary>      
        public int AuthType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>       
        public string Remark { get; set; }
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
    }
}
