using Cimc.Model.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace BaseService.BaseData.DataDictionaryManagement.Dto
{
    /// <summary>
    ///查询参数
    /// </summary>
    public class GetDictionaryDetailInputDto : PagedRequestDto
    {
        /// <summary>
        /// 字典ID
        /// </summary>
        [Required]
        public Guid DictionaryId { get; set; }

    }
}
