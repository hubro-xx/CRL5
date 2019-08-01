using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace CRL.DynamicWebApi
{
    static class Extensions
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static T ToObject<T>(this string json)
        {
            if(string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
        public static object ToObject(this string json, Type type)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject(json, type);
        }
        public static byte[] ToByte(this object obj)
        {
            var json = obj.ToJson();
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
