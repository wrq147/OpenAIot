using AirJointUI.Models;
using AirJointUI.Utils;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirJointUI.Api
{
    public static class RuleApi
    {
        /// <summary>
        /// 获取规则列表
        /// </summary>
        /// <returns></returns>
        public static async Task<List<RuleItem>> GetRuleList()
        {
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("showAll", "true");
            reqparams.Add("pageSize", "0");
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTRulesService/HttpRule/RuleListPage"), reqparams);
            var rsobj = JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultListObjectRuleItem);
            if (rsobj != null && rsobj.code == 0)
            {
                return rsobj.data.List;
            }
            else
            {
                return new List<RuleItem>();
            }
        }
        public static async Task<RuleItem> RuleInfo(long id)
        {
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("id", id.ToString());
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTRulesService/HttpRule/RuleInfo"), reqparams);
            var rsobj = JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultRuleItem);
            return rsobj.data;
        }
        public static async Task<ApiResult<int>> EnableRule(long id, bool enable)
        {
            RuleEnable_In tmpin = new RuleEnable_In()
            {
                Id = id,
                IsEnable = enable
            };
            string jsonstr = JsonSerializer.Serialize(tmpin, GenericJsonContext.Default.RuleEnable_In);
            string rs = await HttpHelper.Instance.PostJsonAsync(Constants.ContactUrl("/IoTRulesService/HttpRule/EnableRule"), jsonstr, System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultInt32);
        }
        public static async Task<ApiResult<int>> SaveRuleParams(long id, List<RuleParamItem> httpParams)
        {
            SaveRuleParamReq req = new SaveRuleParamReq();
            req.Id = id;
            req.HttpParams = JsonSerializer.Serialize(httpParams, GenericJsonContext.Default.ListRuleParamItem);
            string jsonstr = JsonSerializer.Serialize(req, GenericJsonContext.Default.SaveRuleParamReq);
            string rs = await HttpHelper.Instance.PostJsonAsync(Constants.ContactUrl("/IoTRulesService/HttpRule/SaveRuleParams"), jsonstr, System.Text.Encoding.UTF8);
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultInt32);
        }
        public static async Task<ApiResult<int>> ResetRule(long id)
        {
            string rs = await HttpHelper.Instance.GetAsync(Constants.ContactUrl("/IoTRulesService/HttpRule/ResetRule?id=" + id));
            return JsonSerializer.Deserialize(rs, GenericJsonContext.Default.ApiResultInt32);
        }
    }
}
