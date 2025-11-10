using Common;
using IoTRulesService.DAL;
using IoTRulesService.Model;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTRulesService
{
    /// <summary>
    /// 规则缓存
    /// </summary>
    public class RuleCache
    {
        private ITAServiceProvider _provider;
        private CacheHelper _cacheHelper;
        public RuleCache(ITAServiceProvider provider, CacheHelper cacheHelper)
        {
            _provider = provider;
            _cacheHelper = cacheHelper;
        }
        public async Task<List<MZ_RuleTemplate>> SelectByTriggers(string productPath, string devicePath, string msgType)
        {
            var ruleTemplateDAL = _provider.GetService<RuleTemplateDAL>();
            var productRules = _cacheHelper.GetCache<List<MZ_RuleTemplate>>(productPath + "#" + msgType);
            if (productRules == null)
            {
                productRules = await ruleTemplateDAL.SelectProPathByTriggers(productPath, msgType);
                _cacheHelper.SetCache(productPath + "#" + msgType, productRules, DateTime.Now.AddHours(24));
            }

            var deviceRules = _cacheHelper.GetCache<List<MZ_RuleTemplate>>(devicePath + "#" + msgType);
            if (deviceRules == null)
            {
                deviceRules = await ruleTemplateDAL.SelectDevPathByTriggers(devicePath, msgType);
                _cacheHelper.SetCache(devicePath + "#" + msgType, deviceRules, DateTime.Now.AddHours(24));
            }

            return productRules.Concat(deviceRules).ToList();
        }
        public bool Clear(string topic, string msgType)
        {
            return _cacheHelper.RemoveCache(topic + "#" + msgType);
        }
        public void StartDebug(string ruleId)
        {
            _cacheHelper.SetCache("#DebugRuleId" + ruleId, string.Empty, DateTime.Now.AddHours(24));
        }
        public void StopDebug(string ruleId)
        {
            _cacheHelper.RemoveCache("#DebugRuleId" + ruleId);
        }
        public bool IsDebug(long ruleId)
        {
            return _cacheHelper.GetCache<string>("#DebugRuleId" + ruleId) != null;
        }
        public int? GetRuleParamState(long ruleId, string devid)
        {
            return _cacheHelper.GetCache<int?>("#RuleParamState" + ruleId + "#" + devid);
        }
        public void SetRuleParamState(long ruleId, string devid, int state)
        {
            _cacheHelper.SetCache<int?>("#RuleParamState" + ruleId + "#" + devid, state);
        }
    }
}
