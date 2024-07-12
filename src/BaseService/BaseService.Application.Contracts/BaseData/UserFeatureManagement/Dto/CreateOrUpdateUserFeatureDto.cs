using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.UserFeatureManagement.Dto
{
    public class CreateOrUpdateUserFeatureDto : EntityDto<Guid?>
    {
        /// <summary>
        /// 名称
        /// </summary>      
        public string Name { get; set; }

        /// <summary>
        /// 健值
        /// </summary>      
        public string DataKey { get; set; }

        /// <summary>
        /// 数据
        /// </summary>       
        public string DataValue { get; set; }

        ///// <summary>
        ///// 特征类型  如：0-个性化表格列 1-皮肤 ...
        ///// </summary>      
        //public int FeatureType { get; set; }
    }
}
