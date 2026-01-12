using ChannelUtility.Message;
using System;
using System.Text.Encodings.Web;
using System.Text.Json;


namespace ChannelUtility
{
    public static class JsonMessageSerializerConfig
    {
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = JsonMessageSerializerContext.Default,
            Converters = { new JsonMessageConverter<BaseDeviceMessage>(), new JsonMessageConverter<BaseUpDeviceMessage>(), new JsonObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public static readonly JsonSerializerOptions ObjectOptions = new JsonSerializerOptions
        {
            Converters = { new JsonObjectConverter() },
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        public static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }
}
