using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.Systems.AuthorityManagerment.Dto
{
    public class AuthorityFactoryDto : EntityDto<Guid>
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsChecked { get; set; } = false;
    }

}
