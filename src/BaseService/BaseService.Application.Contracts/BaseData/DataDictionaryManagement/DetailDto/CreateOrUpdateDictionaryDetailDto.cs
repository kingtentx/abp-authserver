using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.DataDictionaryManagement.Dto
{
    /// <summary>
    /// 创建/更新 字典表子集合实体
    /// </summary>
    public class CreateOrUpdateDictionaryDetailDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 字典ID
        /// </summary>
        public Guid DictionaryId { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        [Required]
        public string Label { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>
        [Required]
        public string Value { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Required]
        public int Sort { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}
