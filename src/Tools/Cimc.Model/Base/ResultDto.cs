
namespace Cimc.Model.Base
{
    /// <summary>
    /// 返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultDto<T>
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; } = (int)ResultCode.Fail;
        /// <summary>
        /// 消息
        /// </summary>      
        public string Message { get; set; } = "fail";
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        ///  数据赋值
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public virtual void SetData(int code = (int)ResultCode.Success, string message = "success")
        {
            this.Code = code;
            this.Message = message;
        }
        /// <summary>
        /// 数据赋值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public virtual void SetData(T data, int code = (int)ResultCode.Success, string message = "success")
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }
    }

    /// <summary>
    /// 返回结果状态码
    /// </summary>
    public enum ResultCode
    {
        /// <summary>
        /// 成功200
        /// </summary>
        Success = 200,
        /// <summary>
        /// 失败400
        /// </summary>
        Fail = 400,
        /// <summary>
        /// 无权限401
        /// </summary>
        Nopermit = 401,
        /// <summary>
        /// 访问次数达到上限403
        /// </summary>
        Limited = 403,
        /// <summary>
        /// 无该记录405
        /// </summary>
        NULL = 405,
        /// <summary>
        /// 参数不全406
        /// </summary>
        ParmsError = 406,
        /// <summary>
        /// 服务器处理失败500
        /// </summary>
        ServerError = 500,
    }
}
