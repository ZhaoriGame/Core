// ========================================================
// Autor：Zhaori 
// CreateTime：2019/08/21 15:17:45 
// Des：Json工具
// ========================================================

using LitJson;
//using Newtonsoft.Json.Linq;

namespace IL.Game
{

    /// <summary>
    /// Json工具类
    /// </summary>
    public class JsonUtil
    {
        /// <summary>
        /// 对象转Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToJson<T>(T param)
        {
            return JsonMapper.ToJson(param);
        }

        /// <summary>
        /// Json字符串转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T FromJson<T>(string param)
        {
           

            if (typeof(T) == typeof(string))
            {
                return (T)(object)param;
            }
            return JsonMapper.ToObject<T>(param);
        }

        /// <summary>
        /// 获取JSON里的Key对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        //public static T GetObject<T>(string param, string key)
        //{
        //   // return JToken.Parse(param)[key].ToObject<T>();
        //}
    }
}

