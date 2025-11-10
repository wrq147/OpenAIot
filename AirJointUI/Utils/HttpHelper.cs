using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Utils
{
    /// <summary>
    /// http请求类
    /// </summary>
    public class HttpHelper
    {
        private static object _lock = new object();
        private static HttpHelper _instance;
        private HttpClient _client;

        public static HttpHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new HttpHelper();
                        }
                    }
                }
                return _instance;
            }
        }
        private HttpHelper()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            _client.DefaultRequestHeaders.Add("token", "123456789");
        }

        public async Task<string> SendAsync(HttpRequestMessage request)
        {
            return await SendAsync(request, Encoding.UTF8);
        }
        public async Task<string> SendAsync(HttpRequestMessage request, Encoding encode)
        {
            var response = await _client.SendAsync(request);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            return encode.GetString(responseBytes);
        }
        public string Get(string url)
        {
            return Get(url, Encoding.UTF8);
        }
        public string Get(string url, Encoding encode)
        {
            var response = _client.GetAsync(url).Result;
            var responseBytes = response.Content.ReadAsByteArrayAsync().Result;
            return encode.GetString(responseBytes);
        }
        public async Task<string> GetAsync(string url)
        {
            return await GetAsync(url, Encoding.UTF8);
        }
        public async Task<string> GetAsync(string url, Encoding encode)
        {
            var response = await _client.GetAsync(url);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            return encode.GetString(responseBytes);
        }
        public async Task<string> GetAsync(string url, IEnumerable<KeyValuePair<string, string>> getparams)
        {
            return await GetAsync(url, getparams, Encoding.UTF8);
        }
        public async Task<string> GetAsync(string url, IEnumerable<KeyValuePair<string, string>> getparams, Encoding encode)
        {
            url += "?" + string.Join("&", getparams.Select(x => $"{x.Key}={x.Value}"));
            var response = await _client.GetAsync(url);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            return encode.GetString(responseBytes);
        }
        public string Post(string url, IEnumerable<KeyValuePair<string, string>> postparams)
        {
            return Post(url, postparams, Encoding.UTF8);
        }
        public string Post(string url, IEnumerable<KeyValuePair<string, string>> postparams, Encoding encode)
        {
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(postparams);
            HttpResponseMessage response = _client.PostAsync(url, formContent).Result;
            var strBytes = response.Content.ReadAsByteArrayAsync().Result;
            return encode.GetString(strBytes);
        }
        public async Task<string> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> postparams)
        {
            return await PostAsync(url, postparams, Encoding.UTF8);
        }
        public async Task<string> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> postparams, Encoding encode)
        {
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(postparams);
            HttpResponseMessage response = await _client.PostAsync(url, formContent);
            var strBytes = await response.Content.ReadAsByteArrayAsync();
            return encode.GetString(strBytes);
        }
        public string PostJson(string url, string json, Encoding encode)
        {
            var response = _client.PostAsync(url, new StringContent(json, encode, "application/json")).Result;
            var strBytes = response.Content.ReadAsByteArrayAsync().Result;
            return encode.GetString(strBytes);
        }
        public async Task<string> PostJsonAsync(string url, string json, Encoding encode)
        {
            var response = await _client.PostAsync(url, new StringContent(json, encode, "application/json")).ConfigureAwait(false);
            var strBytes = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return encode.GetString(strBytes);
        }

    }
}
