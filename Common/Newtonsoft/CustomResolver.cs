using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Newtonsoft
{
    public class CustomResolver : DefaultContractResolver
    {

        /// <summary>
        /// 创建属性
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="memberSerialization">序列化成员</param>
        /// <returns></returns>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
                .Select(x =>
                {
                    var property = CreateProperty(x, memberSerialization);

                    // 获取所有 JsonConverterAttribute（注意使用泛型方法）
                    var converterAttributes = x.GetCustomAttributes<JsonConverterAttribute>(true).ToList();

                    if (converterAttributes.Any())
                    {
                        // 使用最后一个转换器（按声明顺序）
                        var lastAttribute = converterAttributes.Last();
                        property.Converter = (JsonConverter)Activator.CreateInstance(lastAttribute.ConverterType);
                    }
                    if (property.Converter != null)
                    {
                        if (!property.Converter.CanRead)
                        {
                            property.ShouldDeserialize = instance => false;
                        }
                        if (!property.Converter.CanWrite)
                        {
                            property.ShouldSerialize = instance => false;
                        }
                    }
                    property.ValueProvider = new CustomValueProvider(x);
                    return property;
                }).ToList();
        }

    }
}
