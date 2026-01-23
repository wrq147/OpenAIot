using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common.Attr
{
    /// <summary>
    /// 转成完整文件url
    /// </summary>
    public class FileUrl : JsonConverter<string>
    {
        /// <summary>
        /// 反序列化：移除URL前缀
        /// </summary>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 读取原始字符串值
            string val = reader.GetString();
            if (string.IsNullOrEmpty(val))
            {
                return val;
            }

            // 获取当前上下文和配置
            TAAction ac = TAAction.Current;
            if (ac == null)
            {
                return val;
            }

            ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;
            GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;

            // 移除minio_url前缀
            if (!string.IsNullOrEmpty(go.minio_url))
            {
                val = val.Replace(go.minio_url, "");
            }

            // 移除url前缀（兼容http/https）
            if (!string.IsNullOrEmpty(go.url))
            {
                val = val.ToLower();
                string tmpurl1 = go.url.ToLower().Replace("https:", "http:");
                string tmpurl2 = tmpurl1.Replace("http:", "https:");
                val = val.Replace(tmpurl1, string.Empty).Replace(tmpurl2, string.Empty);
            }

            return val;
        }

        /// <summary>
        /// 序列化：添加URL前缀
        /// </summary>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            if (string.IsNullOrEmpty(value))
            {
                writer.WriteStringValue(value);
                return;
            }

            // 获取当前上下文和配置
            TAAction ac = TAAction.Current;
            if (ac == null)
            {
                writer.WriteStringValue(value);
                return;
            }

            ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;
            GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;

            string val = value;
            // 根据路径规则添加前缀
            if (!string.IsNullOrEmpty(go.minio_bucket) && val.StartsWith("/" + go.minio_bucket))
            {
                val = go.minio_url + val;
            }
            else if (val.StartsWith("/"))
            {
                val = go.url + val;
            }

            // 写入处理后的字符串
            writer.WriteStringValue(val);
        }


    }
}
