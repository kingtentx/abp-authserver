
namespace Cimc.Helper
{
    public static class RetryHelper
    {
        static int sleepMillisecondsTimeout = 1000;

        /// <summary>
        /// 若发生 Exception ，重复执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static T Retry<T>(Func<T> handler, int retryTimes = 3)
        {
            if (retryTimes <= 0)
            {
                return default(T);
            }

            try
            {
                return handler();
            }
            catch (Exception e)
            {
                retryTimes--;
                //Log.Error($"剩余重试次数: {retryTimes}, retry error: {e.Message}, Exception detail: {e.ToJsonString()}");
                Thread.Sleep(sleepMillisecondsTimeout);
                return Retry(handler, retryTimes);
            }
        }

        /// <summary>
        /// 传入多个，遇到 Exception依次执行
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <returns></returns>
        public static T Retry<T>(params Func<T>[] handlers)
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                var handler = handlers[i];
                try
                {
                    return handler();
                }
                catch (Exception e)
                {
                    //logger.Error($"第 {i}次执行错误(start from 0): retry error: {e.Message}, Exception detail: {e.ToJsonString()}");
                    Thread.Sleep(sleepMillisecondsTimeout);
                }
            }
            return default(T);
        }

        /// <summary>
        /// 发生 Exception ，重复执行
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="retryTimes">预设 3次，传入 0直接 return</param>
        public static void Retry(Action handler, int retryTimes = 3)
        {
            if (retryTimes <= 0)
            {
                return;
            }

            try
            {
                handler();
            }
            catch (Exception e)
            {
                retryTimes--;
                // logger.Error($"剩余次数: {retryTimes}, retry error: {e.Message}, Exception detail: {e.ToJsonString()}");
                Thread.Sleep(sleepMillisecondsTimeout);
                Retry(handler, retryTimes);
            }
        }
    }
}
