using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ChannelUtility.Tsl
{
    [JsonSerializable(typeof(List<BaseInputValue>))]
    [JsonSerializable(typeof(List<object>))]
    [JsonSerializable(typeof(IDictionary<string, object>))]
    [JsonSerializable(typeof(Dictionary<string, object>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(IDictionary<string, string>))]
    [JsonSerializable(typeof(byte[]))]
    [JsonSerializable(typeof(double[]))]
    [JsonSerializable(typeof(string[]))]
    [JsonSerializable(typeof(long))]
    [JsonSerializable(typeof(long?))]
    [JsonSerializable(typeof(bool))]
    [JsonSerializable(typeof(bool?))]
    [JsonSerializable(typeof(byte))]
    [JsonSerializable(typeof(byte?))]
    [JsonSerializable(typeof(ushort))]
    [JsonSerializable(typeof(ushort?))]
    [JsonSerializable(typeof(short))]
    [JsonSerializable(typeof(short?))]
    [JsonSerializable(typeof(int))]
    [JsonSerializable(typeof(int?))]
    [JsonSerializable(typeof(float))]
    [JsonSerializable(typeof(float?))]
    [JsonSerializable(typeof(double))]
    [JsonSerializable(typeof(double?))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(BaseAll))]
    [JsonSerializable(typeof(BaseEvent))]
    [JsonSerializable(typeof(BaseFunc))]
    [JsonSerializable(typeof(BaseInputValue))]
    [JsonSerializable(typeof(BaseOutputValue))]
    [JsonSerializable(typeof(BaseProperty))]
    [JsonSerializable(typeof(BaseTagInfo))]
    [JsonSerializable(typeof(BaseValueOption))]
    [JsonSerializable(typeof(BooleanOption))]
    [JsonSerializable(typeof(DateOption))]
    [JsonSerializable(typeof(EnumOption))]
    [JsonSerializable(typeof(FileOption))]
    [JsonSerializable(typeof(FirmwareInfo))]
    [JsonSerializable(typeof(FloatOption))]
    [JsonSerializable(typeof(GeoOption))]
    [JsonSerializable(typeof(IntOption))]
    [JsonSerializable(typeof(ModbusInfo))]
    [JsonSerializable(typeof(ModbusMatch))]
    [JsonSerializable(typeof(ModbusMatchItem))]
    [JsonSerializable(typeof(StringOption))]
    [JsonSerializable(typeof(TslModel))]
    public partial class TslJsonSerializerContext : JsonSerializerContext
    {
    }
}
