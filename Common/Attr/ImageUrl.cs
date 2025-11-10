using Common.Share;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common.Attr
{
    /// <summary>
    /// 转成完整图片url
    /// </summary>
    public class ImageUrl : JsonConverter
    {
        private bool _isAvatar;
        public ImageUrl()
        {
            _isAvatar = false;
        }
        public ImageUrl(bool isAvatar)
        {
            _isAvatar = isAvatar;
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
        /// <summary>
        /// 替换前缀
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            TAAction ac = TAAction.Current;
            if (ac == null || reader.Value == null) return reader.Value;
            string val = reader.Value.ToString();

            ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;
            if (val.StartsWith("data:"))
            {
                return globalServiceProvider.GetService<FileHelper>().UploadBase64(val).Result;
            }

            GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;
            if (!string.IsNullOrEmpty(go.minio_url))
            {
                val = val.Replace(go.minio_url, "");
            }
            if (!string.IsNullOrEmpty(go.url))
            {
                val = val.ToLower();
                string tmpurl1 = go.url.ToLower();
                tmpurl1 = tmpurl1.Replace("https:", "http:");
                string tmpurl2 = tmpurl1.Replace("http:", "https:");
                val = val.Replace(tmpurl1, string.Empty).Replace(tmpurl2, string.Empty);
            }
            return val;
        }
        /// <summary>
        /// 添加前缀
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            TAAction ac = TAAction.Current;
            if (ac == null)
            {
                writer.WriteValue(value);
                return;
            }
            string val = value.ToString();
            ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;
            GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;
            if (string.IsNullOrEmpty(val))
            {
                var defaulturl = go.default_imgurl;
                if (_isAvatar)
                {
                    defaulturl = go.default_avatar;
                }
                if (!string.IsNullOrEmpty(go.minio_bucket) && defaulturl.StartsWith("/" + go.minio_bucket))
                {
                    val = go.minio_url + defaulturl;
                }
                else if (defaulturl.StartsWith("/"))
                {
                    val = go.url + defaulturl;
                }
                else
                {
                    val = defaulturl;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(go.minio_bucket) && val.StartsWith("/" + go.minio_bucket))
                {
                    val = go.minio_url + val;
                }
                else if (val.StartsWith("/"))
                {
                    val = go.url + val;
                }

            }
            writer.WriteValue(val);
        }

    }
}
