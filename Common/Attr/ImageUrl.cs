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
    /// 转成完整图片url
    /// </summary>
    public class ImageUrl : JsonConverter<string>
    {
        /// <summary>
        /// 反序列化：移除URL前缀，Base64图片自动上传并返回URL
        /// </summary>
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 读取原始字符串值
            string val = reader.GetString();
            if (string.IsNullOrEmpty(val))
            {
                return val;
            }

            // 获取当前上下文
            TAAction ac = TAAction.Current;
            if (ac == null)
            {
                return val;
            }

            ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;

            // 处理Base64格式图片（自动上传并返回URL）
            if (val.StartsWith("data:"))
            {
                // 优化异步调用，避免Result导致的线程阻塞
                return globalServiceProvider.GetService<FileHelper>()
                    .UploadBase64(val)
                    .GetAwaiter()
                    .GetResult();
            }

            // 获取配置项
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
        /// 序列化：添加URL前缀，空值时使用默认图片地址
        /// </summary>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            string val = value ?? string.Empty;
            TAAction ac = TAAction.Current;

            // 上下文为空时直接写入原始值
            if (ac == null)
            {
                writer.WriteStringValue(val);
                return;
            }

            ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;
            GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;

            // 空值时使用默认图片地址
            if (string.IsNullOrEmpty(val))
            {
                val = GetDefaultUrl(go.default_imgurl, go);
            }
            else
            {
                // 非空值添加对应前缀
                val = GetFullUrl(val, go);
            }

            writer.WriteStringValue(val);
        }

        /// <summary>
        /// 通用方法：拼接完整URL前缀
        /// </summary>
        protected string GetFullUrl(string path, GeneralOption go)
        {
            if (!string.IsNullOrEmpty(go.minio_bucket) && path.StartsWith("/" + go.minio_bucket))
            {
                return go.minio_url + path;
            }
            else if (path.StartsWith("/"))
            {
                return go.url + path;
            }
            return path;
        }

        /// <summary>
        /// 通用方法：获取带前缀的默认URL
        /// </summary>
        protected string GetDefaultUrl(string defaultPath, GeneralOption go)
        {
            if (string.IsNullOrEmpty(defaultPath))
            {
                return string.Empty;
            }

            if (!string.IsNullOrEmpty(go.minio_bucket) && defaultPath.StartsWith("/" + go.minio_bucket))
            {
                return go.minio_url + defaultPath;
            }
            else if (defaultPath.StartsWith("/"))
            {
                return go.url + defaultPath;
            }
            return defaultPath;
        }
    }
}
