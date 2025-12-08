using Common.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SMSService
{
    public class AliSmsSender
    {
        private static readonly HashSet<char> TextTable =
            new HashSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~");

        private static readonly HttpClient HttpClient = new HttpClient();

        public AliSmsSender(string accessKeyId, string accessKeySecret, string signName)
        {
            if (string.IsNullOrWhiteSpace(accessKeyId))
            {
                throw new ArgumentNullException(nameof(accessKeyId));
            }

            if (string.IsNullOrWhiteSpace(accessKeySecret))
            {
                throw new ArgumentNullException(nameof(accessKeySecret));
            }

            if (string.IsNullOrWhiteSpace(signName))
            {
                throw new ArgumentNullException(nameof(signName));
            }

            AccessKeyId = accessKeyId;
            AccessKeySecret = accessKeySecret;
            SignName = signName;
        }

        public string AccessKeyId { get; }
        public string AccessKeySecret { get; }
        public string SignName { get; }

        public async Task<SendSmsResponse> SendAsync(string phone, string templateCode, object param)
        {
            var dictionary = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                {"AccessKeyId", AccessKeyId},
                {"Action", "SendSms"},
                {"Format", "JSON"},
                {"PhoneNumbers", phone},
                {"SignatureMethod", "HMAC-SHA1"},
                {"SignatureNonce", Guid.NewGuid().ToString()},
                {"SignatureVersion", "1.0"},
                {"SignName", SignName},
                {"TemplateCode", templateCode},
                {"TemplateParam",System.Text.Json.JsonSerializer.Serialize(param, MyDefaultTextJsonConfig.DefaultOptions)},
                {"Timestamp", GetTimeStamp()},
                {"Version", "2017-05-25"}
            };
            var source = ComposeStringToSign("POST", dictionary);
            var value = SignString(source, AccessKeySecret + "&");
            dictionary.Add("Signature", value);
            using (var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    "http://dysmsapi.aliyuncs.com"
                )
            {
                Content = new FormUrlEncodedContent(dictionary)
            }
            )
            {
                using (var response = await HttpClient.SendAsync(request))
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return System.Text.Json.JsonSerializer.Deserialize<SendSmsResponse>(json, MyDefaultTextJsonConfig.DefaultOptions);
                }
            }
        }

        private string SignString(string source, string accessSecret)
        {
            string result;
            using (var keyedHashAlgorithm = KeyedHashAlgorithm.Create("HMACSHA1"))
            {
                keyedHashAlgorithm.Key = Encoding.UTF8.GetBytes(accessSecret.ToCharArray());
                result = Convert.ToBase64String(
                    keyedHashAlgorithm.ComputeHash(
                        Encoding.UTF8.GetBytes(
                            source.ToCharArray()
                        )
                    )
                );
            }

            return result;
        }

        private string ComposeStringToSign(string method, IEnumerable<KeyValuePair<string, string>> items)
        {
            var data = ComposeStringToSign(items);
            return $"{method}&{PercentEncode("/")}&{PercentEncode(data)}";
        }

        private string ComposeStringToSign(IEnumerable<KeyValuePair<string, string>> items)
        {
            var builder = new StringBuilder();
            foreach (var item in items)
            {
                builder.Append(PercentEncode(item.Key))
                    .Append('=')
                    .Append(PercentEncode(item.Value))
                    .Append('&');
            }

            if (builder.Length > 0)
            {
                builder.Length--;
            }

            return builder.ToString();
        }

        private string GetTimeStamp()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo);
        }

        private string PercentEncode(string value)
        {
            var stringBuilder = new StringBuilder();
            var bytes = Encoding.UTF8.GetBytes(value);
            foreach (var b in bytes)
            {
                var c = (char)b;
                if (TextTable.Contains(c))
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append('%')
                        .Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }

            return stringBuilder.ToString();
        }

        public class SendSmsResponse
        {
            [JsonProperty("RequestId")] public string RequestId { get; set; }

            [JsonProperty("Message")] public string Message { get; set; }

            [JsonProperty("BizId")] public string BizId { get; set; }

            [JsonProperty("Code")] public string Code { get; set; }
        }
    }
}
