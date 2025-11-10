using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AirJointUI.Models
{
    [JsonSerializable(typeof(ApiResult<ListObject<DeviceItem>>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(ApiResult<IDictionary<string, object>>))]
    [JsonSerializable(typeof(ApiResult<int>))]
    [JsonSerializable(typeof(ApiResult<string>))]
    [JsonSerializable(typeof(ApiResult<ChannelInfo>))]
    [JsonSerializable(typeof(ApiResult<List<TagItem>>))]
    [JsonSerializable(typeof(ApiResult<ListObject<AlarmItem>>))]
    [JsonSerializable(typeof(ApiResult<ListObject<ProductItem>>))]
    [JsonSerializable(typeof(ApiResult<List<DevicePropItem>>))]
    [JsonSerializable(typeof(ApiResult<ListObject<RuleItem>>))]
    [JsonSerializable(typeof(ApiResult<RuleItem>))]
    [JsonSerializable(typeof(DeviceProto_In))]
    [JsonSerializable(typeof(TagSave_In))]
    [JsonSerializable(typeof(FunExe_In))]
    [JsonSerializable(typeof(ChangeChannelReq))]
    [JsonSerializable(typeof(RuleEnable_In))]
    [JsonSerializable(typeof(SaveRuleParamReq))]
    [JsonSerializable(typeof(List<RuleParamItem>))]
    [JsonSerializable(typeof(List<DeviceSave>))]
    [JsonSerializable(typeof(JsonElement))]
    [JsonSourceGenerationOptions(WriteIndented = true)]
    internal partial class GenericJsonContext : JsonSerializerContext
    {
    }
}
