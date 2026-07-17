using ChannelUtility.Tsl;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Eval;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotWinRuleBLL
    {
        private ILogger<IotWinRuleBLL> _log;
        private IotWinRuleDAL _iotWinRuleDAL;
        private ITAServiceProvider _provider;
        public IotWinRuleBLL(IotWinRuleDAL iotWinRuleDAL, ITAServiceProvider serviceProvider, ILoggerFactory logFactory)
        {
            _iotWinRuleDAL = iotWinRuleDAL;
            _provider = serviceProvider;
            _log = logFactory.CreateLogger<IotWinRuleBLL>();
        }
        public virtual async Task<List<MZ_IotWinRule>> SelectList(string pid, IUserInfo user)
        {
            Expression<Func<MZ_IotWinRule, bool>> expression = x => x.OrgId == user.OrgId && x.ProductId == pid;
            var rsp = await _iotWinRuleDAL.SelectList(expression);
            return rsp;
        }
        public virtual async Task<List<MZ_IotWinRule>> PropRules(string pid, string code, IUserInfo user)
        {
            Expression<Func<MZ_IotWinRule, bool>> expression = x => x.OrgId == user.OrgId && x.ProductId == pid && x.PropCode == code;
            var rsp = await _iotWinRuleDAL.SelectList(expression);
            return rsp;
        }
        public virtual async Task<BusResponse<MZ_IotWinRule>> Info(string id)
        {
            var info = await _iotWinRuleDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_IotWinRule>.Error(111, "属性规则不存在");
            }
            return BusResponse<MZ_IotWinRule>.Success(info);
        }
        public virtual async Task<BusResponse<int>> Save(In_SavePropRules data, IUserInfo user)
        {
            if (string.IsNullOrEmpty(data.ProductId))
            {
                return BusResponse<int>.Error(111, "ProductId不能为空");
            }
            if (string.IsNullOrEmpty(data.PropCode))
            {
                return BusResponse<int>.Error(112, "PropCode不能为空");
            }

            //先删除不需要的
            var updateRules = data.Rules.Where(x => !string.IsNullOrEmpty(x.Id));
            var ruleIds = updateRules.Select(x => x.Id);
            if (ruleIds.Any())
            {
                await _iotWinRuleDAL.Delete(x => x.ProductId == data.ProductId && x.PropCode == data.PropCode && x.OrgId == user.OrgId && !ruleIds.Contains(x.Id));

                foreach (var upitem in updateRules)
                {
                    upitem.OrgId = null;
                    await _iotWinRuleDAL.Update(upitem, x => x.Id == upitem.Id && x.OrgId == user.OrgId);
                }
            }
            else
            {
                await _iotWinRuleDAL.Delete(x => x.ProductId == data.ProductId && x.PropCode == data.PropCode && x.OrgId == user.OrgId);
            }

            //再添加新的
            var newRules = data.Rules.Where(x => string.IsNullOrEmpty(x.Id)).ToList();
            if (newRules.Count > 0)
            {
                foreach (var newitem in newRules)
                {
                    newitem.OrgId = user.OrgId;
                }
                await _iotWinRuleDAL.Insert(newRules);
            }

            return BusResponse<int>.Success();
        }
        public virtual async Task<BusResponse<int>> Update(MZ_IotWinRule data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法修改属性规则");
            }
            var old = await _iotWinRuleDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "属性规则不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "无权修改当前属性规则");
            }

            data.OrgId = null;
            data.ProductId = null;
            data.PropCode = null;
            return BusResponse<int>.Success(await _iotWinRuleDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_IotWinRule data, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加属性规则");
            }
            var snowflake = _provider.GetService<SnowflakeHelper>();
            data.Id = snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            if (await _iotWinRuleDAL.Some(x => x.ProductId == data.ProductId && x.PropCode == data.PropCode))
            {
                return BusResponse<int>.Error(113, "属性规则已存在");
            }
            return BusResponse<int>.Success(await _iotWinRuleDAL.Insert(data));
        }
        public virtual async Task<BusResponse<int>> Delete(string[] ids, IUserInfo user)
        {
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法删除属性规则");
            }
            try
            {
                var num = await _iotWinRuleDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id));
                return BusResponse<int>.Success(num);

            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task NoticeCalDevice(QuartzContext context)
        {
            var serverBus = _provider.GetService<ServerBusProxy>();
            var nodeList = serverBus.GetNodeList();
            foreach (var node in nodeList)
            {
                await BusUtility.Dispatch("CalDev_" + node, context);
            }
        }
        public virtual async Task CalDevice(QuartzContext context)
        {
            try
            {
                var redis = _provider.GetService<IotRedisHelper>();
                var serverBus = _provider.GetService<ServerBusProxy>();
                var iotInfluxBLL = _provider.GetService<IotInfluxBLL>();
                var deviceDAL = _provider.GetService<IotDeviceDAL>();
                var productDAL = _provider.GetService<IotProductDAL>();
                var busProxy = _provider.GetService<ServerBusProxy>();

                var nodeIdx = serverBus.GetNodeIdx();
                var fireTime = context.ScheduledFireTimeUtc.Value.LocalDateTime;


                Dictionary<string, Dictionary<string, object>> propDict = new Dictionary<string, Dictionary<string, object>>();
                Dictionary<string, string> proidDict = new Dictionary<string, string>();
                Dictionary<string, string> idsDict = new Dictionary<string, string>();

                #region 触发统计每小时属性
                var winrules = await _iotWinRuleDAL.SelectList(x => x.WindowWay == 0);
                List<string> tproIds = winrules.Select(x => x.ProductId).Distinct().ToList();
                var allProducts = await productDAL.SelectList(x => tproIds.Contains(x.Id));
                var productDict = allProducts.ToDictionary(x => x.Id);
                var winruleGroup = winrules.GroupBy(x => x.ProductId);
                foreach (var wingk in winruleGroup)
                {
                    if (!productDict.TryGetValue(wingk.Key, out MZ_IotProduct pro))
                    {
                        await _iotWinRuleDAL.Delete(x => x.ProductId == wingk.Key);
                        continue;
                    }
                    var storageConfig = System.Text.Json.JsonSerializer.Deserialize<InfluxOption>(pro.StorageConfig, MyDefaultTextJsonConfig.DefaultOptions);
                    if (storageConfig == null || storageConfig.enable != "1")
                    {
                        continue;
                    }
                    List<MZ_IotDevice> deviceList = await redis.WaitReadNodeLockAsync(async () =>
                    {
                        return await deviceDAL.SelectList(x => x.ProductId == pro.Id && x.DeviceUpIdx == nodeIdx);
                    });
                    if (deviceList.Count == 0)
                    {
                        continue;
                    }


                    var model = TslModel.CreateFrom(pro.ModelTSL);
                    if (model == null)
                    {
                        continue;
                    }

                    int pageSize = 30;
                    int totalCount = deviceList.Count;
                    int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                    foreach (var winrule in wingk)
                    {
                        var curprop = model.properties.Where(x => x.code == winrule.PropCode).FirstOrDefault();
                        if (curprop == null)
                        {
                            await _iotWinRuleDAL.Delete(x => x.ProductId == winrule.ProductId && x.PropCode == winrule.PropCode);
                            continue;
                        }


                        In_HistoryMergeList query = new In_HistoryMergeList();
                        query.IsGroup = true;
                        query.Code = winrule.MergeCode;
                        query.MergeWay = new List<string>();
                        query.WindowWay = 2;
                        if (winrule.MergeWay == "range")
                        {
                            var prepreHourse = fireTime.AddHours(-2);
                            query.BeginTime = new DateTime(prepreHourse.Year, prepreHourse.Month, prepreHourse.Day, prepreHourse.Hour, 0, 0);
                            query.EndTime = new DateTime(fireTime.Year, fireTime.Month, fireTime.Day, fireTime.Hour, 0, 0);
                            query.MergeWay.Add("last");
                            for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                            {
                                int startIndex = pageIndex * pageSize;
                                int endIndex = Math.Min(startIndex + pageSize, totalCount);
                                var currentPageData = deviceList.GetRange(startIndex, endIndex - startIndex);
                                var currentPageDict = currentPageData.ToDictionary(x => x.Id);
                                var rsp = await iotInfluxBLL.SelectMergeList(query, pro, currentPageData, model, storageConfig);


                                if (rsp.IsSuccess())
                                {
                                    var rangeGroup = rsp.Data.GroupBy(x => x.Id);
                                    foreach (var mitemGroup in rangeGroup)
                                    {
                                        var titems = mitemGroup.OrderByDescending(x => x.Time).ToList();
                                        Out_MergeItem firstitem = null;
                                        Out_MergeItem lastitem = null;
                                        if (titems.Count > 1)
                                        {
                                            firstitem = titems[1];
                                            lastitem = titems[0];
                                        }

                                        if (firstitem != null && lastitem != null)
                                        {
                                            if (currentPageDict.TryGetValue(mitemGroup.Key, out MZ_IotDevice dev))
                                            {
                                                if (!propDict.TryGetValue(dev.DeviceId, out Dictionary<string, object> props))
                                                {
                                                    props = new Dictionary<string, object>();
                                                    propDict.Add(dev.DeviceId, props);
                                                    if (!proidDict.ContainsKey(dev.DeviceId))
                                                    {
                                                        proidDict.Add(dev.DeviceId, pro.Id);
                                                    }
                                                    if (!idsDict.ContainsKey(dev.DeviceId))
                                                    {
                                                        idsDict.Add(dev.DeviceId, dev.Id);
                                                    }
                                                }

                                                if (curprop.option.type == "int")
                                                {
                                                    IntOption intOp = (IntOption)curprop.option;
                                                    var maxval = Convert.ToInt32(lastitem.Val);
                                                    var minval = Convert.ToInt32(firstitem.Val);
                                                    int rangeval = maxval - minval;
                                                    if (intOp.min >= 0 && maxval < minval)
                                                    {
                                                        rangeval = maxval - intOp.min;
                                                    }
                                                    if (props.ContainsKey(winrule.PropCode))
                                                    {
                                                        props[winrule.PropCode] = rangeval;
                                                    }
                                                    else
                                                    {
                                                        props.Add(winrule.PropCode, rangeval);
                                                    }
                                                }
                                                else if (curprop.option.type == "float")
                                                {
                                                    FloatOption floatOp = (FloatOption)curprop.option;
                                                    var maxval = Convert.ToDouble(lastitem.Val);
                                                    var minval = Convert.ToDouble(firstitem.Val);
                                                    double rangeval = maxval - minval;
                                                    if (floatOp.min >= 0 && maxval < minval)
                                                    {
                                                        rangeval = maxval - floatOp.min;
                                                    }
                                                    if (props.ContainsKey(winrule.PropCode))
                                                    {
                                                        props[winrule.PropCode] = rangeval;
                                                    }
                                                    else
                                                    {
                                                        props.Add(winrule.PropCode, rangeval);
                                                    }
                                                }
                                            }


                                        }

                                    }
                                }

                            }
                        }
                        else
                        {
                            var preHourse = fireTime.AddHours(-1);
                            query.BeginTime = new DateTime(preHourse.Year, preHourse.Month, preHourse.Day, preHourse.Hour, 0, 0);
                            query.EndTime = new DateTime(fireTime.Year, fireTime.Month, fireTime.Day, fireTime.Hour, 0, 0);
                            query.MergeWay.Add(winrule.MergeWay);
                            for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                            {
                                int startIndex = pageIndex * pageSize;
                                int endIndex = Math.Min(startIndex + pageSize, totalCount);
                                var currentPageData = deviceList.GetRange(startIndex, endIndex - startIndex);
                                var currentPageDict = currentPageData.ToDictionary(x => x.Id);
                                var rsp = await iotInfluxBLL.SelectMergeList(query, pro, currentPageData, model, storageConfig);
                                if (rsp.IsSuccess())
                                {
                                    foreach (var mitem in rsp.Data)
                                    {
                                        if (currentPageDict.TryGetValue(mitem.Id, out MZ_IotDevice dev))
                                        {
                                            if (!propDict.TryGetValue(dev.DeviceId, out Dictionary<string, object> props))
                                            {
                                                props = new Dictionary<string, object>();
                                                propDict.Add(dev.DeviceId, props);
                                                if (!proidDict.ContainsKey(dev.DeviceId))
                                                {
                                                    proidDict.Add(dev.DeviceId, pro.Id);
                                                }
                                                if (!idsDict.ContainsKey(dev.DeviceId))
                                                {
                                                    idsDict.Add(dev.DeviceId, dev.Id);
                                                }
                                            }
                                            if (props.ContainsKey(winrule.PropCode))
                                            {
                                                props[winrule.PropCode] = mitem.Val;
                                            }
                                            else
                                            {
                                                props.Add(winrule.PropCode, mitem.Val);
                                            }
                                        }

                                    }
                                }

                            }
                        }


                    }
                }

                #endregion

                #region 触发统计每天属性
                if (fireTime.Hour == 0)
                {
                    winrules = await _iotWinRuleDAL.SelectList(x => x.WindowWay == 1, "Priority asc");
                    tproIds = winrules.Select(x => x.ProductId).Distinct().ToList();
                    allProducts = await productDAL.SelectList(x => tproIds.Contains(x.Id));
                    productDict = allProducts.ToDictionary(x => x.Id);
                    winruleGroup = winrules.GroupBy(x => x.ProductId);
                    foreach (var wingk in winruleGroup)
                    {
                        if (!productDict.TryGetValue(wingk.Key, out MZ_IotProduct pro))
                        {
                            await _iotWinRuleDAL.Delete(x => x.ProductId == wingk.Key);
                            continue;
                        }
                        var storageConfig = System.Text.Json.JsonSerializer.Deserialize<InfluxOption>(pro.StorageConfig, MyDefaultTextJsonConfig.DefaultOptions);
                        if (storageConfig == null || storageConfig.enable != "1")
                        {
                            continue;
                        }
                        List<MZ_IotDevice> deviceList = await redis.WaitReadNodeLockAsync(async () =>
                        {
                            return await deviceDAL.SelectList(x => x.ProductId == pro.Id && x.DeviceUpIdx == nodeIdx);
                        });
                        if (deviceList.Count == 0)
                        {
                            continue;
                        }


                        var model = TslModel.CreateFrom(pro.ModelTSL);
                        if (model == null)
                        {
                            continue;
                        }

                        int pageSize = 30;
                        int totalCount = deviceList.Count;
                        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                        foreach (var winrule in wingk)
                        {
                            var curprop = model.properties.Where(x => x.code == winrule.PropCode).FirstOrDefault();
                            if (curprop == null)
                            {
                                await _iotWinRuleDAL.Delete(x => x.ProductId == winrule.ProductId && x.PropCode == winrule.PropCode);
                                continue;
                            }

                            In_HistoryMergeList query = new In_HistoryMergeList();
                            query.IsGroup = true;
                            query.Code = winrule.MergeCode;
                            query.MergeWay = new List<string>();
                            query.WindowWay = 0;

                            if (winrule.MergeWay == "range")
                            {
                                var prepreDay = fireTime.AddDays(-2);
                                query.BeginTime = new DateTime(prepreDay.Year, prepreDay.Month, prepreDay.Day, 0, 0, 0);
                                query.EndTime = new DateTime(fireTime.Year, fireTime.Month, fireTime.Day, 0, 0, 0);
                                query.MergeWay.Add("last");
                                for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                                {
                                    int startIndex = pageIndex * pageSize;
                                    int endIndex = Math.Min(startIndex + pageSize, totalCount);
                                    var currentPageData = deviceList.GetRange(startIndex, endIndex - startIndex);
                                    var currentPageDict = currentPageData.ToDictionary(x => x.Id);
                                    var rsp = await iotInfluxBLL.SelectMergeList(query, pro, currentPageData, model, storageConfig);
                                    if (rsp.IsSuccess())
                                    {
                                        var rangeGroup = rsp.Data.GroupBy(x => x.Id);
                                        foreach (var mitemGroup in rangeGroup)
                                        {
                                            var titems = mitemGroup.OrderByDescending(x => x.Time).ToList();
                                            Out_MergeItem firstitem = null;
                                            Out_MergeItem lastitem = null;
                                            if (titems.Count > 1)
                                            {
                                                firstitem = titems[1];
                                                lastitem = titems[0];
                                            }
                                            if (firstitem != null && lastitem != null)
                                            {
                                                if (currentPageDict.TryGetValue(mitemGroup.Key, out MZ_IotDevice dev))
                                                {
                                                    if (!propDict.TryGetValue(dev.DeviceId, out Dictionary<string, object> props))
                                                    {
                                                        props = new Dictionary<string, object>();
                                                        propDict.Add(dev.DeviceId, props);
                                                        if (!proidDict.ContainsKey(dev.DeviceId))
                                                        {
                                                            proidDict.Add(dev.DeviceId, pro.Id);
                                                        }
                                                        if (!idsDict.ContainsKey(dev.DeviceId))
                                                        {
                                                            idsDict.Add(dev.DeviceId, dev.Id);
                                                        }
                                                    }
                                                    if (curprop.option.type == "int")
                                                    {
                                                        IntOption intOp = (IntOption)curprop.option;
                                                        var maxval = Convert.ToInt32(lastitem.Val);
                                                        var minval = Convert.ToInt32(firstitem.Val);
                                                        int rangeval = maxval - minval;
                                                        if (intOp.min >= 0 && maxval < minval)
                                                        {
                                                            rangeval = maxval - intOp.min;
                                                        }
                                                        if (props.ContainsKey(winrule.PropCode))
                                                        {
                                                            props[winrule.PropCode] = rangeval;
                                                        }
                                                        else
                                                        {
                                                            props.Add(winrule.PropCode, rangeval);
                                                        }

                                                    }
                                                    else if (curprop.option.type == "float")
                                                    {
                                                        FloatOption floatOp = (FloatOption)curprop.option;
                                                        var maxval = Convert.ToDouble(lastitem.Val);
                                                        var minval = Convert.ToDouble(firstitem.Val);
                                                        double rangeval = maxval - minval;
                                                        if (floatOp.min >= 0 && maxval < minval)
                                                        {
                                                            rangeval = maxval - floatOp.min;
                                                        }
                                                        if (props.ContainsKey(winrule.PropCode))
                                                        {
                                                            props[winrule.PropCode] = rangeval;
                                                        }
                                                        else
                                                        {
                                                            props.Add(winrule.PropCode, rangeval);
                                                        }

                                                    }
                                                }


                                            }

                                        }
                                    }

                                }
                            }
                            else
                            {
                                var preDay = fireTime.AddDays(-1);
                                query.BeginTime = new DateTime(preDay.Year, preDay.Month, preDay.Day, 0, 0, 0);
                                query.EndTime = new DateTime(fireTime.Year, fireTime.Month, fireTime.Day, 0, 0, 0);
                                query.MergeWay.Add(winrule.MergeWay);
                                for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                                {
                                    int startIndex = pageIndex * pageSize;
                                    int endIndex = Math.Min(startIndex + pageSize, totalCount);
                                    var currentPageData = deviceList.GetRange(startIndex, endIndex - startIndex);
                                    var currentPageDict = currentPageData.ToDictionary(x => x.Id);
                                    var rsp = await iotInfluxBLL.SelectMergeList(query, pro, currentPageData, model, storageConfig);
                                    if (rsp.IsSuccess())
                                    {
                                        foreach (var mitem in rsp.Data)
                                        {
                                            if (currentPageDict.TryGetValue(mitem.Id, out MZ_IotDevice dev))
                                            {
                                                if (!propDict.TryGetValue(dev.DeviceId, out Dictionary<string, object> props))
                                                {
                                                    props = new Dictionary<string, object>();
                                                    propDict.Add(dev.DeviceId, props);
                                                    if (!proidDict.ContainsKey(dev.DeviceId))
                                                    {
                                                        proidDict.Add(dev.DeviceId, pro.Id);
                                                    }
                                                    if (!idsDict.ContainsKey(dev.DeviceId))
                                                    {
                                                        idsDict.Add(dev.DeviceId, dev.Id);
                                                    }
                                                }
                                                if (props.ContainsKey(winrule.PropCode))
                                                {
                                                    props[winrule.PropCode] = mitem.Val;
                                                }
                                                else
                                                {
                                                    props.Add(winrule.PropCode, mitem.Val);
                                                }
                                            }


                                        }
                                    }

                                }
                            }


                        }
                    }

                }
                #endregion

                #region 触发统计每月属性
                if (fireTime.Day == 1 && fireTime.Hour == 0)
                {
                    winrules = await _iotWinRuleDAL.SelectList(x => x.WindowWay == 2, "Priority asc");
                    tproIds = winrules.Select(x => x.ProductId).Distinct().ToList();
                    allProducts = await productDAL.SelectList(x => tproIds.Contains(x.Id));
                    productDict = allProducts.ToDictionary(x => x.Id);
                    winruleGroup = winrules.GroupBy(x => x.ProductId);
                    foreach (var wingk in winruleGroup)
                    {
                        if (!productDict.TryGetValue(wingk.Key, out MZ_IotProduct pro))
                        {
                            await _iotWinRuleDAL.Delete(x => x.ProductId == wingk.Key);
                            continue;
                        }
                        var storageConfig = System.Text.Json.JsonSerializer.Deserialize<InfluxOption>(pro.StorageConfig, MyDefaultTextJsonConfig.DefaultOptions);
                        if (storageConfig == null || storageConfig.enable != "1")
                        {
                            continue;
                        }
                        List<MZ_IotDevice> deviceList = await redis.WaitReadNodeLockAsync(async () =>
                        {
                            return await deviceDAL.SelectList(x => x.ProductId == pro.Id && x.DeviceUpIdx == nodeIdx);
                        });
                        if (deviceList.Count == 0)
                        {
                            continue;
                        }


                        var model = TslModel.CreateFrom(pro.ModelTSL);
                        if (model == null)
                        {
                            continue;
                        }

                        int pageSize = 30;
                        int totalCount = deviceList.Count;
                        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                        foreach (var winrule in wingk)
                        {
                            var curprop = model.properties.Where(x => x.code == winrule.PropCode).FirstOrDefault();
                            if (curprop == null)
                            {
                                await _iotWinRuleDAL.Delete(x => x.ProductId == winrule.ProductId && x.PropCode == winrule.PropCode);
                                continue;
                            }

                            In_HistoryMergeList query = new In_HistoryMergeList();
                            query.IsGroup = true;
                            query.Code = winrule.MergeCode;
                            query.MergeWay = new List<string>();
                            query.WindowWay = 1;

                            if (winrule.MergeWay == "range")
                            {
                                var prepreMonth = fireTime.AddMonths(-2);
                                query.BeginTime = new DateTime(prepreMonth.Year, prepreMonth.Month, 1, 0, 0, 0);
                                query.EndTime = new DateTime(fireTime.Year, fireTime.Month, 1, 0, 0, 0);
                                query.MergeWay.Add("last");
                                for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                                {
                                    int startIndex = pageIndex * pageSize;
                                    int endIndex = Math.Min(startIndex + pageSize, totalCount);
                                    var currentPageData = deviceList.GetRange(startIndex, endIndex - startIndex);
                                    var currentPageDict = currentPageData.ToDictionary(x => x.Id);
                                    var rsp = await iotInfluxBLL.SelectMergeList(query, pro, currentPageData, model, storageConfig);
                                    if (rsp.IsSuccess())
                                    {
                                        var rangeGroup = rsp.Data.GroupBy(x => x.Id);
                                        foreach (var mitemGroup in rangeGroup)
                                        {
                                            var titems = mitemGroup.OrderByDescending(x => x.Time).ToList();
                                            Out_MergeItem firstitem = null;
                                            Out_MergeItem lastitem = null;
                                            if (titems.Count > 1)
                                            {
                                                firstitem = titems[1];
                                                lastitem = titems[0];
                                            }
                                            if (firstitem != null && lastitem != null)
                                            {
                                                if (currentPageDict.TryGetValue(mitemGroup.Key, out MZ_IotDevice dev))
                                                {
                                                    if (!propDict.TryGetValue(dev.DeviceId, out Dictionary<string, object> props))
                                                    {
                                                        props = new Dictionary<string, object>();
                                                        propDict.Add(dev.DeviceId, props);
                                                        if (!proidDict.ContainsKey(dev.DeviceId))
                                                        {
                                                            proidDict.Add(dev.DeviceId, pro.Id);
                                                        }
                                                        if (!idsDict.ContainsKey(dev.DeviceId))
                                                        {
                                                            idsDict.Add(dev.DeviceId, dev.Id);
                                                        }
                                                    }
                                                    if (curprop.option.type == "int")
                                                    {
                                                        IntOption intOp = (IntOption)curprop.option;
                                                        var maxval = Convert.ToInt32(lastitem.Val);
                                                        var minval = Convert.ToInt32(firstitem.Val);
                                                        int rangeval = maxval - minval;
                                                        if (intOp.min >= 0 && maxval < minval)
                                                        {
                                                            rangeval = maxval - intOp.min;
                                                        }
                                                        if (props.ContainsKey(winrule.PropCode))
                                                        {
                                                            props[winrule.PropCode] = rangeval;
                                                        }
                                                        else
                                                        {
                                                            props.Add(winrule.PropCode, rangeval);
                                                        }
                                                    }
                                                    else if (curprop.option.type == "float")
                                                    {
                                                        FloatOption floatOp = (FloatOption)curprop.option;
                                                        var maxval = Convert.ToDouble(lastitem.Val);
                                                        var minval = Convert.ToDouble(firstitem.Val);
                                                        double rangeval = maxval - minval;
                                                        if (floatOp.min >= 0 && maxval < minval)
                                                        {
                                                            rangeval = maxval - floatOp.min;
                                                        }
                                                        if (props.ContainsKey(winrule.PropCode))
                                                        {
                                                            props[winrule.PropCode] = rangeval;
                                                        }
                                                        else
                                                        {
                                                            props.Add(winrule.PropCode, rangeval);
                                                        }

                                                    }
                                                }


                                            }

                                        }
                                    }

                                }
                            }
                            else
                            {
                                var preMonth = fireTime.AddMonths(-1);
                                query.BeginTime = new DateTime(preMonth.Year, preMonth.Month, 1, 0, 0, 0);
                                query.EndTime = new DateTime(fireTime.Year, fireTime.Month, 1, 0, 0, 0);
                                query.MergeWay.Add(winrule.MergeWay);
                                for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                                {
                                    int startIndex = pageIndex * pageSize;
                                    int endIndex = Math.Min(startIndex + pageSize, totalCount);
                                    var currentPageData = deviceList.GetRange(startIndex, endIndex - startIndex);
                                    var currentPageDict = currentPageData.ToDictionary(x => x.Id);
                                    var rsp = await iotInfluxBLL.SelectMergeList(query, pro, currentPageData, model, storageConfig);
                                    if (rsp.IsSuccess())
                                    {
                                        foreach (var mitem in rsp.Data)
                                        {
                                            if (currentPageDict.TryGetValue(mitem.Id, out MZ_IotDevice dev))
                                            {
                                                if (!propDict.TryGetValue(dev.DeviceId, out Dictionary<string, object> props))
                                                {
                                                    props = new Dictionary<string, object>();
                                                    propDict.Add(dev.DeviceId, props);
                                                    if (!proidDict.ContainsKey(dev.DeviceId))
                                                    {
                                                        proidDict.Add(dev.DeviceId, pro.Id);
                                                    }
                                                    if (!idsDict.ContainsKey(dev.DeviceId))
                                                    {
                                                        idsDict.Add(dev.DeviceId, dev.Id);
                                                    }
                                                }
                                                if (props.ContainsKey(winrule.PropCode))
                                                {
                                                    props[winrule.PropCode] = mitem.Val;
                                                }
                                                else
                                                {
                                                    props.Add(winrule.PropCode, mitem.Val);
                                                }
                                            }


                                        }
                                    }

                                }
                            }


                        }
                    }
                }
                #endregion


                #region 触发存储
                int afterpageSize = 30;
                int aftertotalCount = proidDict.Keys.Count;
                int aftertotalPages = (int)Math.Ceiling(aftertotalCount / (double)afterpageSize);
                var tmpkeylist = proidDict.Keys.ToList();
                for (int pageIndex = 0; pageIndex < aftertotalPages; pageIndex++)
                {
                    int startIndex = pageIndex * afterpageSize;
                    int endIndex = Math.Min(startIndex + afterpageSize, aftertotalCount);
                    var currentPageData = tmpkeylist.GetRange(startIndex, endIndex - startIndex);
                    foreach (var devid in currentPageData)
                    {
                        if (propDict.TryGetValue(devid, out var props))
                        {
                            if (proidDict.TryGetValue(devid, out var proid))
                            {
                                if (idsDict.TryGetValue(devid, out var ttid))
                                {
                                    props.Add("$Id", ttid);
                                }
                                await busProxy.SendPropertyReply(proid, devid, props, null, true, new HashSet<long>() { -1 }, null, fireTime);
                            }
                        }
                    }
                    await Task.Delay(50);
                }
                #endregion
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
            }



        }

    }
}
