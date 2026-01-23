using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Attr
{
    /// <summary>
    /// 只反串行化(只能添加修改，不会输出给用户）
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OnlyDeserializeAttribute : Attribute
    {
    }
}
