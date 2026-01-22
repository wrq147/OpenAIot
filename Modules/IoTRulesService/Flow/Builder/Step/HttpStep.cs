using Common.Json;
using IoTRulesService.Flow.Node;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private async Task ReplaceDictValue(RuleExecutionContext context, IDictionary<string, object> dict)
        {
            foreach (var pd in dict)
            {
                if (pd.Value == null)
                {
                    continue;
                }
                if (pd.Value is IDictionary<string, object> tmpdictx)
                {
                    await ReplaceDictValue(context, tmpdictx);
                }
                else if (pd.Value is IList tmplist)
                {
                    for (int i = 0; i < tmplist.Count; i++)
                    {
                        var tmpiitt = tmplist[i];
                        if (tmpiitt == null)
                        {
                            continue;
                        }
                        if (tmpiitt is IDictionary<string, object> tmpsubobj)
                        {
                            await ReplaceDictValue(context, tmpsubobj);
                        }
                        else if (tmpiitt is string tmpstttr)
                        {
                            string arrayStr = tmpstttr;
                            if (arrayStr.StartsWith("$"))
                            {
                                tmplist[i] = await context.ReadSourceString(arrayStr);
                            }
                        }
                    }

                }
                else if (pd.Value is string tmpxxzstr)
                {
                    string kvst = tmpxxzstr;
                    if (kvst.StartsWith("$"))
                    {
                        dict[pd.Key] = await context.ReadSourceString(kvst);
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
                    string jsonstr = System.Text.Json.JsonSerializer.Serialize(reqparams, MyDefaultTextJsonConfig.DefaultOptions);
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
                        var tmpobj = System.Text.Json.JsonSerializer.Deserialize<IDictionary<string, object>>(props.rawString, MyDefaultTextJsonConfig.DefaultOptions);
                        await ReplaceDictValue(context, tmpobj);
                        tmpjson = System.Text.Json.JsonSerializer.Serialize(tmpobj, MyDefaultTextJsonConfig.DefaultOptions);
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
