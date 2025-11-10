using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModbusChannel
{
    public class HttpApi
    {
        private static object _lock = new object();
        private static HttpApi _instance;
        private HttpClient _client;

        public static HttpApi Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new HttpApi();
                        }
                    }
                }
                return _instance;
            }
        }
        private HttpApi()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            _client.DefaultRequestHeaders.Add("token", "123456789");
        }

        /// <summary>
        /// 获取规则列表
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetRuleList()
        {
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("showAll", "true");
            reqparams.Add("pageSize", "0");
            return await GetAsync("http://127.0.0.1:880/IoTRulesService/HttpRule/RuleListPage", reqparams);
        }
        public async Task<string> EnableRule(long id, bool enable)
        {
            string jsonstr = System.Text.Json.JsonSerializer.Serialize(new
            {
                Id = id,
                IsEnable = enable
            });
            return await PostJsonAsync("http://127.0.0.1:880/IoTRulesService/HttpRule/EnableRule", jsonstr, System.Text.Encoding.UTF8);
        }

        private async Task<string> GetAsync(string url, IEnumerable<KeyValuePair<string, string>> getparams)
        {
            return await GetAsync(url, getparams, Encoding.UTF8);
        }
        private async Task<string> GetAsync(string url, IEnumerable<KeyValuePair<string, string>> getparams, Encoding encode)
        {
            url += "?" + string.Join("&", getparams.Select(x => $"{x.Key}={x.Value}"));
            var response = await _client.GetAsync(url);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            return encode.GetString(responseBytes);
        }

        private async Task<string> PostJsonAsync(string url, string json, Encoding encode)
        {
            var response = await _client.PostAsync(url, new StringContent(json, encode, "application/json")).ConfigureAwait(false);
            var strBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return encode.GetString(strBytes);
        }
    }

}
