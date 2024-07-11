using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.DataDictionaryManagement.Dto
{
    /// <summary>
    /// 字典表实体
    /// </summary>
    public class DictionaryDto : EntityDto<Guid>
    {
        /// <summary>
        /// 字典表关键字 唯一值
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

    }
}
