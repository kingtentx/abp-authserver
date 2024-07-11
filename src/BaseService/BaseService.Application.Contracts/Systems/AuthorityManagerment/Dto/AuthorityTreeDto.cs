using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.AuthorityManagerment.Dto
{
    public class AuthorityTreeDto : EntityDto<Guid>
    {

        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public Guid? Pid { get; set; }
        /// <summary>
        /// 权限属性类型: 0-公司 1-部门
        /// </summary>      
        public int AuthType { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsChecked { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public List<AuthorityTreeDto> Children { get; set; }
    }
}
