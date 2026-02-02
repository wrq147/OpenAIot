using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using TemplateAction.Core;

namespace Common.Json
{
    public class DateTimeConverter : JsonConverter<object>
    {
        private const string FORMAT_STR = "yyyy-MM-dd HH:mm:ss";
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(DateTime?) && reader.TokenType == JsonTokenType.Null)
                return null;

            var dateStr = reader.GetString();
            if (string.IsNullOrWhiteSpace(dateStr))
                return typeToConvert == typeof(DateTime?) ? null : throw new FormatException("日期字符串为空");

            if (DateTime.TryParse(dateStr, out var date))
            {
                TAAction ac = TAAction.Current;
                if (ac != null)
                {
                    string clientTZ = ac.Context.Request.Header["TZ"];
                    if (!string.IsNullOrEmpty(clientTZ))
                    {
                        int tz;
                        if (int.TryParse(clientTZ, out tz))
                        {
                            var clientTime = DateTime.SpecifyKind(date.AddMinutes(tz), DateTimeKind.Utc);
                            return TimeZoneInfo.ConvertTimeFromUtc(clientTime, TimeZoneInfo.Local);
                        }

                    }

                }
                return date;
            }
            throw new FormatException($"日期 {dateStr} 不符合格式：{FORMAT_STR}");
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            if (value is DateTime date)
            {
                TAAction ac = TAAction.Current;
                if (ac != null)
                {
                    string clientTZ = ac.Context.Request.Header["TZ"];
                    if (!string.IsNullOrEmpty(clientTZ))
                    {
                        int tz;
                        if (int.TryParse(clientTZ, out tz))
                        {
                            var clientTime = TimeZoneInfo.ConvertTimeToUtc(date).AddMinutes(-tz);
                            writer.WriteStringValue(clientTime.ToString(FORMAT_STR));
                            return;
                        }
                    }
                }
                writer.WriteStringValue(date.ToString(FORMAT_STR));
            }

            else
                throw new ArgumentException("仅支持 DateTime/DateTime? 类型");
        }
    }
}
