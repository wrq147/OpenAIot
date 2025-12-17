using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChannelUtility
{
    public class JsonMessageConverter<T> : JsonConverter<T> where T : BaseDeviceMessage
    {
        // 类型映射表，避免使用反射，支持AOT
        private static readonly Dictionary<string, Type> _typeMap = new()
        {
            { "Event", typeof(DeviceEventMessage) },
            { "Offline", typeof(DeviceOfflineMessage) },
            { "Online", typeof(DeviceOnlineMessage) },
            { "Function", typeof(FunctionInvokeMessage) },
            { "FunctionReply", typeof(FunctionInvokeMessageReply) },
            { "ReadProp", typeof(ReadPropertyMessage) },
            { "PropReply", typeof(ReadPropertyMessageReply) },
            { "Execute", typeof(ExecuteRuleMessage) },
            { "MobusRequest", typeof(ModbusMessage) },
            { "Bind", typeof(DeviceBindMessage) },
            { "BindReply", typeof(DeviceBindMessageReply) },
            { "RawData", typeof(RawDataMessage) },
            { "RawUpData", typeof(RawUpDataMessage) },
            { "ModbusMatch", typeof(ModbusMatchMessage) },
            { "QueryICCID", typeof(QueryICCIDMessage) },
            { "TempProduct", typeof(TempProductMessage) },
            { "QueryICCIDReply", typeof(QueryICCIDMessageReply) },
            { "ChangeProduct", typeof(ChangeProductMessage) },
            { "StartReadAll", typeof(StartReadAllMessage) },
            { "AIDetectReq",typeof(AIDetectRequestMeesage) },
            { "AIDetectResp",typeof(AIDetectResponseMessage) }
        };
        private static readonly JsonSerializerOptions _nestedOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // 首先将JSON加载到文档中
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // 获取type字段值
            if (!root.TryGetProperty("MsgType", out var typeElement))
            {
                throw new JsonException("缺少Type字段");
            }
            var typeName = typeElement.GetString();
            // 查找对应的类型
            if (!_typeMap.TryGetValue(typeName, out var type))
            {
                throw new JsonException($"未知的类型: {typeName}");
            }

            // 反序列化为具体类型
            var json = root.GetRawText();
            var result = JsonSerializer.Deserialize(json, type, _nestedOptions);

            return result as T;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), _nestedOptions);
        }
    }


}
