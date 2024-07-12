using Cimc.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace BaseService.BaseData.UserFeatureManagement.Dto
{
    public class GetUserFeatureInputDto : PagedRequestDto
    {
        /// <summary>
        /// 关键词
        /// </summary>
        public string Filter { get; set; }
    }
}
