using ChannelUtility.Tsl;
using Common.EventBus;
using Common.IdGenerator;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
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
        private IotWinRuleDAL _iotWinRuleDAL;
        private ITAServiceProvider _provider;
        public IotWinRuleBLL(IotWinRuleDAL iotWinRuleDAL, ITAServiceProvider serviceProvider)
        {
            _iotWinRuleDAL = iotWinRuleDAL;
            _provider = serviceProvider;
        }
        public virtual async Task<List<MZ_IotWinRule>> SelectList(string pid, IUserInfo user)
        {
            Expression<Func<MZ_IotWinRule, bool>> expression = x => x.OrgId == user.OrgId && x.ProductId == pid;
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
            var redis = _provider.GetService<IotRedisHelper>();
            await redis.WaitReadLockAsync("NodeIdx", TimeSpan.FromSeconds(60));
            try
            {
                foreach (var node in nodeList)
                {
                    var callRes = await BusUtility.Call("CalDev_" + node, context);
                    if (!callRes.IsSuccess())
                    {
                        throw new Exception(callRes.Message);
                    }
                }
            }
            finally
            {
                await redis.ReleaseReadLockAsync("NodeIdx");
            }
        }
        public virtual async Task CalDevice(QuartzContext context)
        {
            var serverBus = _provider.GetService<ServerBusProxy>();
            var iotInfluxBLL = _provider.GetService<IotInfluxBLL>();
            var deviceDAL = _provider.GetService<IotDeviceDAL>();
            var productDAL = _provider.GetService<IotProductDAL>();
            var busProxy = _provider.GetService<ServerBusProxy>();

            var nodeIdx = serverBus.GetNodeIdx();
            var fireTime = context.ScheduledFireTimeUtc.Value.LocalDateTime;

            #region 触发统计每小时属性
            var winrules = await _iotWinRuleDAL.SelectList(x => x.WindowWay == 0, "Priority asc");
            var winruleGroup = winrules.GroupBy(x => x.ProductId);
            foreach (var wingk in winruleGroup)
            {
                var pro = await productDAL.Select(wingk.Key);
                if (pro == null)
                {
                    await _iotWinRuleDAL.Delete(x => x.ProductId == wingk.Key);
                    continue;
                }
                var storageConfig = System.Text.Json.JsonSerializer.Deserialize<InfluxOption>(pro.StorageConfig, MyDefaultTextJsonConfig.DefaultOptions);
                if (storageConfig == null || storageConfig.enable != "1")
                {
                    continue;
                }

                var deviceList = await deviceDAL.SelectList(x => x.ProductId == pro.Id && x.DeviceUpIdx == nodeIdx);
                if (deviceList.Count == 0)
                {
                    continue;
                }
                var model = TslModel.CreateFrom(pro.ModelTSL);
                if (model == null)
                {
                    continue;
                }

                int pageSize = 100;
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
                                        Dictionary<string, object> props = new Dictionary<string, object>();
                                        if (curprop.option.type == "int")
                                        {
                                            int rangeval = Convert.ToInt32(lastitem.Val) - Convert.ToInt32(firstitem.Val);
                                            props.Add(winrule.PropCode, rangeval);
                                        }
                                        else if (curprop.option.type == "float")
                                        {
                                            double rangeval = Convert.ToDouble(lastitem.Val) - Convert.ToDouble(firstitem.Val);
                                            props.Add(winrule.PropCode, rangeval);
                                        }
                                        if (currentPageDict.TryGetValue(mitemGroup.Key, out MZ_IotDevice dev) && props.Keys.Count > 0)
                                        {
                                            await busProxy.SendPropertyReply(pro.Id, dev.DeviceId, props, null, true, null, null, fireTime);
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
                                    Dictionary<string, object> props = new Dictionary<string, object>();
                                    props.Add(winrule.PropCode, mitem.Val);
                                    if (currentPageDict.TryGetValue(mitem.Id, out MZ_IotDevice dev))
                                    {
                                        await busProxy.SendPropertyReply(pro.Id, dev.DeviceId, props, null, false, null, null, fireTime);
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
                winruleGroup = winrules.GroupBy(x => x.ProductId);
                foreach (var wingk in winruleGroup)
                {
                    var pro = await productDAL.Select(wingk.Key);
                    if (pro == null)
                    {
                        await _iotWinRuleDAL.Delete(x => x.ProductId == wingk.Key);
                        continue;
                    }
                    var storageConfig = System.Text.Json.JsonSerializer.Deserialize<InfluxOption>(pro.StorageConfig, MyDefaultTextJsonConfig.DefaultOptions);
                    if (storageConfig == null || storageConfig.enable != "1")
                    {
                        continue;
                    }

                    var deviceList = await deviceDAL.SelectList(x => x.ProductId == pro.Id && x.DeviceUpIdx == nodeIdx);
                    if (deviceList.Count == 0)
                    {
                        continue;
                    }

                    var model = TslModel.CreateFrom(pro.ModelTSL);
                    if (model == null)
                    {
                        continue;
                    }

                    int pageSize = 100;
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
                                            Dictionary<string, object> props = new Dictionary<string, object>();
                                            if (curprop.option.type == "int")
                                            {
                                                int rangeval = Convert.ToInt32(lastitem.Val) - Convert.ToInt32(firstitem.Val);
                                                props.Add(winrule.PropCode, rangeval);
                                            }
                                            else if (curprop.option.type == "float")
                                            {
                                                double rangeval = Convert.ToDouble(lastitem.Val) - Convert.ToDouble(firstitem.Val);
                                                props.Add(winrule.PropCode, rangeval);
                                            }
                                            if (currentPageDict.TryGetValue(mitemGroup.Key, out MZ_IotDevice dev) && props.Keys.Count > 0)
                                            {
                                                await busProxy.SendPropertyReply(pro.Id, dev.DeviceId, props, null, true, null, null, fireTime);
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
                                        Dictionary<string, object> props = new Dictionary<string, object>();
                                        props.Add(winrule.PropCode, mitem.Val);
                                        if (currentPageDict.TryGetValue(mitem.Id, out MZ_IotDevice dev))
                                        {
                                            await busProxy.SendPropertyReply(pro.Id, dev.DeviceId, props, null, false, null, null, fireTime);
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
                winruleGroup = winrules.GroupBy(x => x.ProductId);
                foreach (var wingk in winruleGroup)
                {
                    var pro = await productDAL.Select(wingk.Key);
                    if (pro == null)
                    {
                        await _iotWinRuleDAL.Delete(x => x.ProductId == wingk.Key);
                        continue;
                    }
                    var storageConfig = System.Text.Json.JsonSerializer.Deserialize<InfluxOption>(pro.StorageConfig, MyDefaultTextJsonConfig.DefaultOptions);
                    if (storageConfig == null || storageConfig.enable != "1")
                    {
                        continue;
                    }

                    var deviceList = await deviceDAL.SelectList(x => x.ProductId == pro.Id && x.DeviceUpIdx == nodeIdx);
                    if (deviceList.Count == 0)
                    {
                        continue;
                    }

                    var model = TslModel.CreateFrom(pro.ModelTSL);
                    if (model == null)
                    {
                        continue;
                    }

                    int pageSize = 20;
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
                                            Dictionary<string, object> props = new Dictionary<string, object>();
                                            if (curprop.option.type == "int")
                                            {
                                                int rangeval = Convert.ToInt32(lastitem.Val) - Convert.ToInt32(firstitem.Val);
                                                props.Add(winrule.PropCode, rangeval);
                                            }
                                            else if (curprop.option.type == "float")
                                            {
                                                double rangeval = Convert.ToDouble(lastitem.Val) - Convert.ToDouble(firstitem.Val);
                                                props.Add(winrule.PropCode, rangeval);
                                            }
                                            if (currentPageDict.TryGetValue(mitemGroup.Key, out MZ_IotDevice dev) && props.Keys.Count > 0)
                                            {
                                                await busProxy.SendPropertyReply(pro.Id, dev.DeviceId, props, null, false, null, null, fireTime);
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
                                        Dictionary<string, object> props = new Dictionary<string, object>();
                                        props.Add(winrule.PropCode, mitem.Val);
                                        if (currentPageDict.TryGetValue(mitem.Id, out MZ_IotDevice dev))
                                        {
                                            await busProxy.SendPropertyReply(pro.Id, dev.DeviceId, props, null, false, null, null, fireTime);
                                        }
                                    }
                                }
                            }
                        }


                    }
                }
            }
            #endregion

        }
    }
}
