using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace ChannelUtility
{
    [JsonSerializable(typeof(List<object>))]
    [JsonSerializable(typeof(List<string>))]
    [JsonSerializable(typeof(HashSet<long>))]
    [JsonSerializable(typeof(IDictionary<string, object>))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(IDictionary<string, string>))]
    [JsonSerializable(typeof(MediaNotReaderMessage))]
    [JsonSerializable(typeof(MediaNotFoundMessage))]
    [JsonSerializable(typeof(MediaChannelMessage))]
    [JsonSerializable(typeof(MediaUserVerifyMessage))]
    [JsonSerializable(typeof(MediaDelItemMessage))]
    [JsonSerializable(typeof(MediaItemMessage))]
    [JsonSerializable(typeof(DeviceEventMessage))]
    [JsonSerializable(typeof(DeviceOfflineMessage))]
    [JsonSerializable(typeof(DeviceOnlineMessage))]
    [JsonSerializable(typeof(FunctionInvokeMessage))]
    [JsonSerializable(typeof(FunctionInvokeMessageReply))]
    [JsonSerializable(typeof(ReadPropertyMessage))]
    [JsonSerializable(typeof(ReadPropertyMessageReply))]
    [JsonSerializable(typeof(ExecuteRuleMessage))]
    [JsonSerializable(typeof(ModbusMessage))]
    [JsonSerializable(typeof(DeviceBindMessage))]
    [JsonSerializable(typeof(DeviceBindMessageReply))]
    [JsonSerializable(typeof(RawDataMessage))]
    [JsonSerializable(typeof(AIDetectResponseMessage))]
    [JsonSerializable(typeof(AIDetectRequestMeesage))]
    [JsonSerializable(typeof(RawUpDataMessage))]
    [JsonSerializable(typeof(TempProductMessage))]
    [JsonSerializable(typeof(ModbusMatchMessage))]
    [JsonSerializable(typeof(QueryICCIDMessage))]
    [JsonSerializable(typeof(QueryICCIDMessageReply))]
    [JsonSerializable(typeof(ChangeProductMessage))]
    [JsonSerializable(typeof(StartReadAllMessage))]
    [JsonSerializable(typeof(BaseUpDeviceMessage))]
    [JsonSerializable(typeof(RequestMessage))]
    [JsonSerializable(typeof(BaseDeviceMessage))]
    public partial class JsonMessageSerializerContext : JsonSerializerContext
    {
    }
}
