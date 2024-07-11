
using Volo.Abp.Application.Dtos;

namespace Cimc.Model.Base
{
    public class PagedRequestDto : IPagedAndSortedResultRequest
    {
        /// <summary>
        /// 排序字段 如：id asc , id desc
        /// </summary>
        public string Sorting { get; set; }

        private int skipCount = 1;

        /// <summary>
        /// 当前页码 页码从1开始
        /// </summary>
        public int SkipCount
        {
            get
            {
                return ((this.skipCount >= 1 ? this.skipCount : 1) - 1) * maxResultCount;
            }
            set
            {
                this.skipCount = value;
            }
        }

        private int maxResultCount = 10;

        /// <summary>
        /// 当前查询条数 默认10条
        /// </summary>
        public int MaxResultCount
        {
            get
            {
                return this.maxResultCount;
            }
            set
            {
                this.maxResultCount = value;
            }
        }

    }
}
