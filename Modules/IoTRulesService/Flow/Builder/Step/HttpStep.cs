using IoTRulesService.Flow.Node;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace IoTRulesService.Flow.Builder.Step
{
    /// <summary>
    /// Http节点
    /// </summary>
    public class HttpStep : RuleflowStep
    {
        private static HttpClient client = new HttpClient();
        public HttpProps props { get; set; }
        private async Task ReplaceDictValue(RuleExecutionContext context, JObject dict)
        {
            IEnumerable<JProperty> properties = dict.Properties();
            foreach (var pd in properties)
            {
                if (pd.Value == null)
                {
                    continue;
                }
                if (pd.Value.Type == JTokenType.Object)
                {
                    await ReplaceDictValue(context, pd.Value.ToObject<JObject>());
                }
                else if (pd.Value.Type == JTokenType.String)
                {
                    string kvst = pd.Value.ToString();
                    if (kvst.StartsWith("$"))
                    {
                        pd.Value = await context.ReadSourceString(kvst);
                    }
                }
            }
        }

        public override async Task Run(RuleExecutionContext context)
        {
            //调用接口
            try
            {
                HttpRequestMessage req = new HttpRequestMessage();
                req.Method = new HttpMethod(props.method);
                req.RequestUri = new Uri(props.url);
                foreach (var hd in props.headers)
                {
                    if (hd.isField)
                    {
                        req.Headers.Add(hd.name, await context.ReadSourceString(hd.value));
                    }
                    else
                    {
                        req.Headers.Add(hd.name, hd.value);
                    }
                }

                if (props.contentType == "JSON")
                {
                    SortedDictionary<string, string> reqparams = new SortedDictionary<string, string>();
                    foreach (var pd in props.xparams)
                    {
                        if (pd.isField)
                        {
                            reqparams.Add(pd.name, await context.ReadSourceString(pd.value));
                        }
                        else
                        {
                            reqparams.Add(pd.name, pd.value);
                        }
                    }
                    string jsonstr = Newtonsoft.Json.JsonConvert.SerializeObject(reqparams);
                    req.Content = new StringContent(jsonstr, Encoding.UTF8, "application/json");
                    if (context.IsDebug)
                    {
                        await context.Print($"开始{req.Method.ToString()}请求,json参数:{jsonstr}");
                    }
                }
                else if (props.contentType == "FORM")
                {
                    SortedDictionary<string, string> reqparams = new SortedDictionary<string, string>();
                    foreach (var pd in props.xparams)
                    {
                        if (pd.isField)
                        {
                            reqparams.Add(pd.name, await context.ReadSourceString(pd.value));
                        }
                        else
                        {
                            reqparams.Add(pd.name, pd.value);
                        }
                    }
                    req.Content = new FormUrlEncodedContent(reqparams);
                    if (context.IsDebug)
                    {
                        StringBuilder builder = new StringBuilder();
                        foreach (KeyValuePair<string, string> item in reqparams)
                        {
                            builder.Append(item.Key + "=" + Uri.EscapeDataString(item.Value));
                        }
                        string formstr = builder.ToString().TrimEnd('&');
                        await context.Print($"开始{req.Method.ToString()}请求,form参数:{formstr}");
                    }
                }
                else if (props.contentType == "RAW")
                {
                    string tmpjson = string.Empty;
                    if (!string.IsNullOrEmpty(props.rawString))
                    {
                        JObject tmpobj = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(props.rawString);
                        await ReplaceDictValue(context, tmpobj);
                        tmpjson = Newtonsoft.Json.JsonConvert.SerializeObject(tmpobj);
                    }
                    req.Content = new StringContent(tmpjson, Encoding.UTF8, "application/json");
                    if (context.IsDebug)
                    {
                        await context.Print($"开始{req.Method.ToString()}请求,json参数:{tmpjson}");
                    }
                }
                else
                {
                    await context.Print("开始Http请求,未知的参数格式");
                }

                var response = await client.SendAsync(req);
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                string rss = Encoding.UTF8.GetString(responseBytes);
                if (rss != null && rss.Contains(props.okTxt))
                {
                    this.IsActive = true;
                    if (context.IsDebug)
                    {
                        await context.Print("Http请求成功结果:" + rss);
                    }
                }
                else
                {
                    this.IsActive = false;
                    await context.Print("Http请求失败结果:" + rss ?? "");
                }
                await context.ExcuteNext(RuleResult.Next());

            }
            catch (Exception ex)
            {
                await context.Print("Http请求失败：" + ex.Message);
            }

        }
    }
}
