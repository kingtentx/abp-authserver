
using System.Net;
using System.Text;

namespace Cimc.Helper
{
    /// <summary>
    /// 基于HttpWebRequest\HttpWebResponse封装的请求类
    /// </summary>
    public class HttpWebRequest
    {
        /// <summary>
        /// Get请求获取url地址输出内容,Encoding.UTF8编码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDownloadString(string url)
        {
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return client.DownloadString(url);
        }

        /// <summary>   
        /// Get请求获取url地址输出内容   
        /// </summary> 
        /// <param name="url">url</param>   
        /// <param name="encoding">返回内容编码方式，例如：Encoding.UTF8</param>   
        public static string Get(string url, string token)
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("Authorization", $"Bearer {token}"); ;
                request.Timeout = 20000;
                request.ServicePoint.Expect100Continue = false;
                HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
                StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                return sr.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Post请求获取url地址输出内容
        /// </summary>
        public static string Post(string url, string postContent, string contentType = "application/json")
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 20000;
                request.ContentType = contentType;
                request.ServicePoint.Expect100Continue = false;
                Encoding encoding = Encoding.UTF8;
                if (!string.IsNullOrEmpty(postContent))
                {
                    Stream stream = request.GetRequestStream();
                    byte[] dataMenu = encoding.GetBytes(postContent);
                    stream.Write(dataMenu, 0, dataMenu.Length);
                    stream.Flush();
                    stream.Close();
                }
                WebResponse response = request.GetResponse();
                Stream inStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(inStream);
                return sr.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get请求获取url地址输出流
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Stream GetStream(string url)
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 20000;
                request.ServicePoint.Expect100Continue = false;
                HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse();
                Stream sr = webResponse.GetResponseStream();
                return sr;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get请求获取url地址输出
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebResponse GetWebResponse(string url)
        {
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.Timeout = 20000;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                return webResponse;
            }
            catch
            {
                return null;
            }
        }
    }
}
