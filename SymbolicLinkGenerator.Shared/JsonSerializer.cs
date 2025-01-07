using System;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace SymbolicLinkGenerator.Shared
{
    internal static class JsonSerializer
    {
        private static JavaScriptSerializer _serializer = new JavaScriptSerializer();

        /// <summary>
        /// 序列化为 Json 字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// 反序列化 Json 字符串为对象 T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                Debug.Print($"Deserialize err:{ex.Message}");
                return default;
            }
        }
    }
}
