using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cimc.Helper.Extensions
{
    public static partial class Extensions
    {
        /// <summary>
        /// 转成对象<泛型>
        /// </summary>
        /// <param name="Json">json字串</param>
        /// <returns></returns>
        public static T ToObject<T>(this string jsonString)
        {
            return jsonString == null ? default(T) : JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// 转成json字串
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="isCamelCase">驼峰格式</param>
        /// <returns></returns>
        public static string ToJson(this object obj, bool isCamelCase = true)
        {
            if (obj == null) return null;
            if (isCamelCase)
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                return JsonConvert.SerializeObject(obj, serializerSettings);
            }
            else
                return JsonConvert.SerializeObject(obj);
        }
    }
}
