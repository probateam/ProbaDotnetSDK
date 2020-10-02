using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ProbaDotnetSDK
{
    public static class StringUtils
    {
        public static string ToJson(this object obj)
            => Newtonsoft.Json.JsonConvert.SerializeObject(obj);

        public static T FromJson<T>(this string json)
            => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

        public static dynamic FromJson(this string json)
            => Newtonsoft.Json.JsonConvert.DeserializeObject(json);

        public static string ToUTF8(this byte[] data)
            => System.Text.Encoding.UTF8.GetString(data);

        public static byte[] FromUTF8(this string data)
            => System.Text.Encoding.UTF8.GetBytes(data);

        public static byte[] FromBase64String(this string data)
            => Convert.FromBase64String(data);

        public static string ToBase64String(this byte[] data)
            => Convert.ToBase64String(data);

    }
}
