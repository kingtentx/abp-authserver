using System;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.PositionManagement.Dto
{
    /// <summary>
    /// 岗位实体
    /// </summary>
    public class PositionDto : EntityDto<Guid>
    {
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否激活
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
