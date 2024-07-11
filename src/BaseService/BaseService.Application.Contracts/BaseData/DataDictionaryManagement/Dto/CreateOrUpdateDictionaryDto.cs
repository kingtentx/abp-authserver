using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.DataDictionaryManagement.Dto
{
    /// <summary>
    /// 创建/更新 字典表实体
    /// </summary>
    public class CreateOrUpdateDictionaryDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 字典表关键字 唯一值
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
