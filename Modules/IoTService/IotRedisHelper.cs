using Common.Redis;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Collections;
using System.Threading.Tasks;
using TemplateAction.Core;
using System.Collections.Generic;
using StackExchange.Redis;
using ChannelUtility.Tsl;
using Common.Json;

namespace IoTService
{
    public class IotRedisHelper : RedisHelper
    {
        private ITAServiceProvider _serviceProvider;
        private const string RULE_DATA_KEY = "RuleParam:";
        public IotRedisHelper(IOptions<GeneralOption> conf, ITAServiceProvider serviceProvider) : base(conf.Value.redisconn, 1)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task DeleteRule(long ruleId)
        {
            await this.KeyDeleteAsync(RULE_DATA_KEY + ruleId);
        }
        public async Task<Dictionary<string, object>> GetRuleVal(long ruleId, Dictionary<string, BaseInputValue> refvals)
        {
            var tmphs = await this.HashGetAllHashEntryAsync(RULE_DATA_KEY + ruleId);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (HashEntry he in tmphs)
            {
                if (he.Value.IsNull)
                {
                    continue;
                }
                if (refvals.TryGetValue(he.Name, out BaseInputValue tmpbi))
                {
                    if (!he.Value.IsNullOrEmpty)
                    {
                        if (tmpbi.type == "int")
                        {
                            string sval = he.Value;
                            if (sval.Contains('.'))
                            {
                                dict.Add(he.Name, (int)double.Parse(sval));
                            }
                            else
                            {
                                if (int.TryParse(sval, out int intResult))
                                {
                                    dict.Add(he.Name, intResult);
                                }
                                else if (long.TryParse(sval, out long longResult))
                                {
                                    dict.Add(he.Name, longResult);
                                }
                            }
                        }
                        else if (tmpbi.type == "date")
                        {
                            string sval = he.Value;
                            if (sval.Contains('.'))
                            {
                                dict.Add(he.Name, (long)double.Parse(sval));
                            }
                            else if (long.TryParse(sval, out long longResult))
                            {
                                dict.Add(he.Name, longResult);
                            }
                        }
                        else if (tmpbi.type == "float")
                        {
                            if (float.TryParse(he.Value, out float floatResult))
                            {
                                dict.Add(he.Name, floatResult);
                            }
                            else if (double.TryParse(he.Value, out double doubleResult))
                            {
                                dict.Add(he.Name, doubleResult);
                            }
                        }
                        else if (tmpbi.type == "string" || tmpbi.type == "enum")
                        {
                            dict.Add(he.Name, he.Value.ToString());
                        }
                        else
                        {
                            dict.Add(he.Name, System.Text.Json.JsonSerializer.Deserialize<object>(he.Value, MyDefaultTextJsonConfig.DefaultOptions));
                        }
                    }
                }
                else
                {
                    if (int.TryParse(he.Value, out int intResult))
                    {
                        dict.Add(he.Name, intResult);
                    }
                    // 尝试转换为 long
                    else if (long.TryParse(he.Value, out long longResult))
                    {
                        dict.Add(he.Name, longResult);
                    }
                    // 尝试转换为 float
                    else if (float.TryParse(he.Value, out float floatResult))
                    {
                        dict.Add(he.Name, floatResult);
                    }
                    // 尝试转换为 double
                    else if (double.TryParse(he.Value, out double doubleResult))
                    {
                        dict.Add(he.Name, doubleResult);
                    }
                    else
                    {
                        dict.Add(he.Name, he.Value.ToString());
                    }
                }
            }
            return dict;
        }
        public async Task<Dictionary<string, object>> GetRuleVal(long ruleId, string devid)
        {
            return await this.HashGetAsync<Dictionary<string, object>>(RULE_DATA_KEY + ruleId, devid);
        }
        public async Task SaveRuleVal(long ruleId, Dictionary<string, object> val)
        {
            await this.KeyDeleteAsync(RULE_DATA_KEY + ruleId);
            await this.HashSetAsync(RULE_DATA_KEY + ruleId, val);
        }
        public async Task SaveRuleVal(long ruleId, string devid, Dictionary<string, object> val)
        {
            await this.HashSetAsync(RULE_DATA_KEY + ruleId, devid, val);
        }
        public async Task ClearRuleVal(long ruleId, string[] keys)
        {
            await this.HashDeleteAsync(RULE_DATA_KEY + ruleId, keys);
        }
    }
}
