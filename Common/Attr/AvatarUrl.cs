using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common.Attr
{
    /// <summary>
    /// 头像URL转换器
    /// </summary>
    public class AvatarUrl : ImageUrl
    {
        /// <summary>
        /// 序列化：空值时使用默认头像地址
        /// </summary>
        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            string val = value ?? string.Empty;
            TAAction ac = TAAction.Current;

            if (ac == null)
            {
                writer.WriteStringValue(val);
                return;
            }

            ITAServiceProvider globalServiceProvider = ac.Context.Application.ServiceProvider;
            GeneralOption go = globalServiceProvider.GetService<IOptions<GeneralOption>>().Value;

            // 空值时使用默认头像地址（核心差异点）
            if (string.IsNullOrEmpty(val))
            {
                val = GetDefaultUrl(go.default_avatar, go);
            }
            else
            {
                val = GetFullUrl(val, go);
            }

            writer.WriteStringValue(val);
        }
    }
}
