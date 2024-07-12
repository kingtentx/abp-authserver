using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.DataDictionaryManagement.Dto
{
    /// <summary>
    /// 字典表子集合实体
    /// </summary>
    public class DictionaryDetailDto : EntityDto<Guid>
    {
        /// <summary>
        /// 字典ID
        /// </summary>
        public Guid DictionaryId { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>      
        public string Label { get; set; }
        /// <summary>
        /// 字典值
        /// </summary>     
        public string Value { get; set; }
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
