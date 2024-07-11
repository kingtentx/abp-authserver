using Microsoft.Extensions.Caching.Distributed;

namespace Cimc.Helper
{
    public class PolicyHelper
    {
        /// <summary>
        /// 设置过期日间
        /// </summary>
        /// <param name="expirationTime">过期时间</param>
        /// <param name="isAbsoulte">true绝对过期; false:滑动过期</param>
        /// <returns></returns>
        public static DistributedCacheEntryOptions SetPolicy(TimeSpan expirationTime, bool isAbsoulte = true)
        {
            var policy = new DistributedCacheEntryOptions();
            if (isAbsoulte)
                policy.AbsoluteExpirationRelativeToNow = expirationTime;
            else
                policy.SlidingExpiration = expirationTime;
            return policy;
        }
    }
}
