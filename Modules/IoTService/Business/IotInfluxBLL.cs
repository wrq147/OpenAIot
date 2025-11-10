using ChannelUtility;
using ChannelUtility.Tsl;
using Common.Share;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TimeZoneConverter;

namespace IoTService.Business
{
    public class IotInfluxBLL
    {
        private ITAServiceProvider _provider;
        private ILogger<IotInfluxBLL> _log;

        public IotInfluxBLL(ITAServiceProvider serviceProvider, ILoggerFactory factory)
        {
            _provider = serviceProvider;
            _log = factory.CreateLogger<IotInfluxBLL>();
        }

        public virtual async Task<BusResponse<string>> SaveOnline(string productId, string deviceId, string dvId, InfluxOption storageConfig, DateTime timestamp)
        {
            List<PointData> pointlist = new List<PointData>();
            DateTime utcnow = timestamp.ToUniversalTime();
            utcnow = ChangeMilliseconds(utcnow, 200);
            var point = PointData.Measurement("onoffline").Tag("DeviceId", deviceId).Tag("DxId", dvId).Field("stat", 1).Timestamp(utcnow, WritePrecision.Ns);
            pointlist.Add(point);
            try
            {
                using var client = new InfluxDBClient(storageConfig.url, storageConfig.token);
                var writeApi = client.GetWriteApiAsync();
                await writeApi.WritePointsAsync(pointlist, storageConfig.bucket, storageConfig.org);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(114, ex.Message);
            }
        }
        private DateTime ChangeMilliseconds(DateTime originalDateTime, int milliseconds)
        {
            // 计算新的Ticks值，其中包括原始的Ticks数减去毫秒数，再加上新的毫秒数
            long newTicks = originalDateTime.Ticks - (originalDateTime.Ticks % TimeSpan.TicksPerMillisecond) + milliseconds * TimeSpan.TicksPerMillisecond;

            // 创建一个新的DateTime对象，使用计算出的Ticks值
            return new DateTime(newTicks);
        }
        public virtual async Task<BusResponse<string>> SaveOffline(string productId, string deviceId, string dvId, InfluxOption storageConfig, DateTime timestamp)
        {
            List<PointData> pointlist = new List<PointData>();
            DateTime utcnow = timestamp.ToUniversalTime();
            utcnow = ChangeMilliseconds(utcnow, 100);
            var point = PointData.Measurement("onoffline").Tag("DeviceId", deviceId).Tag("DxId", dvId).Field("stat", 0).Timestamp(utcnow, WritePrecision.Ns);
            pointlist.Add(point);
            try
            {
                using var client = new InfluxDBClient(storageConfig.url, storageConfig.token);
                var writeApi = client.GetWriteApiAsync();
                await writeApi.WritePointsAsync(pointlist, storageConfig.bucket, storageConfig.org);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(114, ex.Message);
            }
        }
        public virtual async Task<PageObject<Out_OnOffline>> SelectOnlines(In_OnOffline query)
        {
            IotDeviceDAL deviceDAL = _provider.GetService<IotDeviceDAL>();
            IotProductDAL productDAL = _provider.GetService<IotProductDAL>();


            MZ_IotDevice device = null;
            if (query is In_OnOfflineList quid)
            {
                device = await deviceDAL.Select(quid.Id);
            }
            else if (query is In_OnOfflineListSync qusyn)
            {
                device = (await deviceDAL.SelectList(x => x.DeviceNumber == qusyn.Number)).FirstOrDefault();
            }

            if (device == null)
            {
                throw new Exception("设备不存在");
            }

            var product = await productDAL.Select(device.ProductId);
            if (string.IsNullOrEmpty(product.StorageConfig))
            {
                throw new Exception("请配置产品的存储方式");
            }
            var storageConfig = JsonConvert.DeserializeObject<InfluxOption>(product.StorageConfig);
            if (string.IsNullOrEmpty(storageConfig.url))
            {
                throw new Exception("请配置产品的存储方式");
            }
            using var client = new InfluxDBClient(storageConfig.url, storageConfig.token);
            var queryApi = client.GetQueryApi();
            int totalNumbers = -1;
            StringBuilder querysql = new StringBuilder();

            querysql.Append($"from(bucket:\"{storageConfig.bucket.Replace("\"", "")}\")");
            DateTime? queryStart = query.BeginTime;
            DateTime? queryEnd = query.EndTime;
            if (queryStart != null && queryEnd != null)
            {
                querysql.Append($" |> range(start: {TimeZoneInfo.ConvertTime(queryStart.Value, TimeZoneInfo.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ")},stop:{TimeZoneInfo.ConvertTime(queryEnd.Value, TimeZoneInfo.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ")})");
            }
            else
            {
                querysql.Append($" |> range(start: -inf,stop:now())");
            }
            querysql.Append(" |> filter(fn: (r) => r[\"_measurement\"] == \"onoffline\")");
            querysql.Append(" |> filter(fn: (r) => r[\"DeviceId\"] == \"" + device.DeviceId + "\")");
            querysql.Append(" |> filter(fn: (r) => r[\"_field\"] == \"stat\")");
            int startRow = (query.pageNum.Value - 1) * query.pageSize.Value;
            if (startRow < 0) startRow = 0;
            querysql.Append(" |> sort(columns: [\"_time\"], desc: true)");
            querysql.Append(" |> limit(n:" + query.pageSize + ",offset:" + startRow + ")");

            var fluxTable = await queryApi.QueryAsync(querysql.ToString(), storageConfig.org);
            List<Out_OnOffline> tlist = new List<Out_OnOffline>();
            for (int i = 0; i < fluxTable.Count; i++)
            {
                //i是参数
                for (int j = 0; j < fluxTable[i].Records.Count; j++)
                {
                    try
                    {
                        //j是数据
                        DateTime? time = fluxTable[i].Records[j].GetTimeInDateTime()?.ToLocalTime();
                        bool val = Convert.ToInt32(fluxTable[i].Records[j].Values["_value"]) == 1 ? true : false;
                        tlist.Add(new Out_OnOffline()
                        {
                            CreatedOn = time.Value,
                            IsOnline = val
                        });
                    }
                    catch { }
                }
            }
            var rt = new PageObject<Out_OnOffline>();
            rt.List = tlist.OrderByDescending(x => x.CreatedOn).ToList();
            rt.Total = tlist.Count;
            return rt;
        }
        public virtual async Task<BusResponse<string>> SaveHistory(string productId, string dtuId, string dId, InfluxOption storageConfig, DateTime timestamp, Dictionary<string, DevicePropertyValue> data, List<BaseProperty> list)
        {
            Dictionary<string, BaseProperty> dic = new Dictionary<string, BaseProperty>();
            foreach (BaseProperty bp in list)
            {
                dic.Add(bp.code, bp);
            }
            List<PointData> pointlist = new List<PointData>();
            DateTime utcnow = timestamp.ToUniversalTime();
            foreach (var kvp in data)
            {
                BaseProperty bp;
                if (!dic.TryGetValue(kvp.Key, out bp))
                {
                    continue;
                }
                switch (bp.option.type)
                {
                    case "date":
                        {
                            long tmpv = (long)kvp.Value.val;
                            if (tmpv == 0)
                            {
                                continue;
                            }
                            var point = PointData.Measurement("device").Tag("DeviceId", dtuId).Tag("DxId", dId).Field(bp.option.type + "#" + kvp.Key, tmpv).Timestamp(utcnow, WritePrecision.Ns);
                            pointlist.Add(point);
                        }
                        break;
                    case "float":
                        {
                            double tmpv = Convert.ToDouble(kvp.Value.val);
                            var point = PointData.Measurement("device").Tag("DeviceId", dtuId).Tag("DxId", dId).Field(bp.option.type + "#" + kvp.Key, tmpv).Timestamp(utcnow, WritePrecision.Ns);
                            pointlist.Add(point);
                        }
                        break;
                    case "int":
                        {
                            int tmpv = Convert.ToInt32(kvp.Value.val);
                            var point = PointData.Measurement("device").Tag("DeviceId", dtuId).Tag("DxId", dId).Field(bp.option.type + "#" + kvp.Key, tmpv).Timestamp(utcnow, WritePrecision.Ns);
                            pointlist.Add(point);
                        }
                        break;
                    case "enum":
                        {
                            if (kvp.Value == null)
                            {
                                continue;
                            }
                            var tmpv = kvp.Value.val as string;
                            if (tmpv != null)
                            {
                                var point = PointData.Measurement("device").Tag("DeviceId", dtuId).Tag("DxId", dId).Field(bp.option.type + "#" + kvp.Key, tmpv).Timestamp(utcnow, WritePrecision.Ns);
                                pointlist.Add(point);
                            }
                        }
                        break;
                    case "geo":
                        {
                            if (kvp.Value == null || kvp.Value.val == null)
                            {
                                continue;
                            }
                            var point = PointData.Measurement("device").Tag("DeviceId", dtuId).Tag("DxId", dId).Field("geo#" + kvp.Key, JsonConvert.SerializeObject(kvp.Value.val)).Timestamp(utcnow, WritePrecision.Ns);
                            pointlist.Add(point);
                        }
                        break;
                    default:
                        continue;
                }
            }
            if (pointlist.Count == 0)
            {
                return BusResponse<string>.Success();
            }
            try
            {
                using var client = new InfluxDBClient(storageConfig.url, storageConfig.token);
                var writeApi = client.GetWriteApiAsync();
                await writeApi.WritePointsAsync(pointlist, storageConfig.bucket, storageConfig.org);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(114, ex.Message);
            }

        }
        public virtual async Task<BusResponse<string>> DeleteHistory(In_HistoryDelete query)
        {
            IotDeviceDAL deviceDAL = _provider.GetService<IotDeviceDAL>();
            IotProductDAL productDAL = _provider.GetService<IotProductDAL>();
            MZ_IotDevice device = await deviceDAL.Select(query.Id);
            if (device == null)
            {
                return BusResponse<string>.Error(101, "设备不存在");
            }
            var product = await productDAL.Select(device.ProductId);
            if (string.IsNullOrEmpty(product.StorageConfig))
            {
                return BusResponse<string>.Error(102, "请配置产品的存储方式");
            }
            var storageConfig = JsonConvert.DeserializeObject<InfluxOption>(product.StorageConfig);
            if (string.IsNullOrEmpty(storageConfig.url))
            {
                return BusResponse<string>.Error(103, "请配置产品的存储方式");
            }
            string pred = "_measurement=\"device\" AND DeviceId=\"" + device.DeviceId + "\"";
            using var client = new InfluxDBClient(storageConfig.url, storageConfig.token);
            try
            {
                var delApi = client.GetDeleteApi();
                await delApi.Delete(TimeZoneInfo.ConvertTime(query.BeginTime.Value, TimeZoneInfo.Utc), TimeZoneInfo.ConvertTime(query.EndTime.Value, TimeZoneInfo.Utc), pred, storageConfig.bucket.Replace("\"", ""), storageConfig.org);
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<List<Out_MergeItem>>> SelectMergeList(In_HistoryMergeBase query, MZ_IotProduct prod = null, List<MZ_IotDevice> devices = null, TslModel model = null, InfluxOption storageConfig = null)
        {
            if (string.IsNullOrEmpty(query.Code))
            {
                return BusResponse<List<Out_MergeItem>>.Error(111, "属性标识不能为空");
            }
            IotDeviceDAL deviceDAL = _provider.GetService<IotDeviceDAL>();
            IotProductDAL productDAL = _provider.GetService<IotProductDAL>();
            if (devices == null)
            {
                if (query is In_HistoryMergeListSync quid)
                {
                    devices = await deviceDAL.SelectList(x => quid.Numbers.Contains(x.DeviceNumber));
                }
                else if (query is In_HistoryMergeList qusyn)
                {
                    devices = await deviceDAL.SelectList(x => qusyn.Ids.Contains(x.Id));
                }
                else
                {
                    return BusResponse<List<Out_MergeItem>>.Error(131, "参数格式错误");
                }
            }
            var dviddict = devices.ToDictionary(x => x.DeviceId);

            if (prod == null)
            {
                List<string> pids = devices.Select(x => x.ProductId).ToList();
                var products = await productDAL.SelectList(x => pids.Contains(x.Id));
                if (products.Count > 1)
                {
                    return BusResponse<List<Out_MergeItem>>.Error(112, "无法操作多个产品");
                }
                if (products.Count == 0)
                {
                    return BusResponse<List<Out_MergeItem>>.Error(101, "产品不存在");
                }
                prod = products[0];
            }

            Dictionary<string, BaseProperty> hsdict = new Dictionary<string, BaseProperty>();
            if (model == null)
            {
                model = TslModel.CreateFrom(prod.ModelTSL);
            }
            foreach (BaseProperty bp in model.properties)
            {
                hsdict.Add(bp.code, bp);
            }
            if (string.IsNullOrEmpty(prod.StorageConfig))
            {
                return BusResponse<List<Out_MergeItem>>.Error(113, "未设置历史存储配置");
            }
            if (storageConfig == null)
            {
                storageConfig = JsonConvert.DeserializeObject<InfluxOption>(prod.StorageConfig);
                if (storageConfig == null || storageConfig.enable != "1")
                {
                    return BusResponse<List<Out_MergeItem>>.Error(114, "未设置存储配置url");
                }
            }


            if (query.MergeWay == null || query.MergeWay.Count == 0)
            {
                return BusResponse<List<Out_MergeItem>>.Error(124, "MergeWay参数不能为空");
            }
            string influxDBTimeZone = TimeZoneInfo.Local.Id;
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                influxDBTimeZone = TZConvert.WindowsToIana(localTimeZone.Id);
            }

            List<Out_MergeItem> finalList = new List<Out_MergeItem>();
            foreach (var merge_way in query.MergeWay)
            {
                using var client = new InfluxDBClient(storageConfig.url, storageConfig.token);
                var queryApi = client.GetQueryApi();
                int totalNumbers = -1;
                StringBuilder querysql = new StringBuilder();
                if (query.Hours != null && query.Hours.Count > 0)
                {
                    querysql.Append("import \"date\"\n");
                }
                querysql.Append($"from(bucket:\"{storageConfig.bucket.Replace("\"", "")}\")");
                DateTime? queryStart = query.BeginTime;
                DateTime? queryEnd = query.EndTime;
                if (queryEnd == null)
                {
                    queryEnd = DateTime.Now;
                }
                if (queryStart == null)
                {
                    queryStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                }
                querysql.Append($" |> range(start: {TimeZoneInfo.ConvertTime(queryStart.Value, TimeZoneInfo.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ")},stop:{TimeZoneInfo.ConvertTime(queryEnd.Value, TimeZoneInfo.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ")})");
                querysql.Append(" |> filter(fn: (r) => r[\"_measurement\"] == \"device\")");
                StringBuilder dvsb = new StringBuilder();
                var devlist = devices.Where(x => x.ProductId == prod.Id).ToList();
                for (int i = 0; i < devlist.Count; i++)
                {
                    if (i == 0)
                    {
                        dvsb.Append("r[\"DeviceId\"] == \"" + devlist[i].DeviceId + "\"");
                    }
                    else
                    {
                        dvsb.Append(" or r[\"DeviceId\"] == \"" + devlist[i].DeviceId + "\"");
                    }
                }
                querysql.Append(" |> filter(fn: (r) => (" + dvsb.ToString() + "))");

                BaseProperty pp;
                if (!hsdict.TryGetValue(query.Code, out pp))
                {
                    return BusResponse<List<Out_MergeItem>>.Error(115, "参数Code值不存在");
                }
                if (pp.option.type == "geo" || pp.option.type == "string" || pp.option.type == "enum" || pp.option.type == "date")
                {
                    return BusResponse<List<Out_MergeItem>>.Error(116, "无法统计非数值字段");
                }
                querysql.Append(" |> filter(fn: (r) => r[\"_field\"] == \"" + pp.option.type + "#" + query.Code + "\")");
                if (query.Hours != null && query.Hours.Count > 0)
                {
                    StringBuilder timesql = new StringBuilder();
                    int i = 0;
                    foreach (var t in query.Hours)
                    {
                        if (i > 0)
                        {
                            timesql.Append(" and ");
                        }
                        timesql.Append("date.hour(t: r._time, location: {zone: \"" + influxDBTimeZone + "\", offset: 0s})>=" + t.StartHour + " and date.hour(t: r._time, location: {zone: \"" + influxDBTimeZone + "\", offset: 0s})<" + t.EndHour);
                        i++;
                    }
                    querysql.Append(" |> filter(fn: (r) => " + timesql.ToString() + ")");
                }
                if (queryEnd == null)
                {
                    queryEnd = DateTime.Now;
                }
                if (queryStart == null)
                {
                    queryStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                }

                switch (merge_way)
                {
                    case "max":
                    case "min":
                    case "mean":
                    case "sum":
                    case "first":
                    case "last":
                    case "count":
                        break;
                    default:
                        return BusResponse<List<Out_MergeItem>>.Error(115, "参数MergeWay值不存在");
                }
                if (query.IsGroup == true)
                {
                    querysql.Append(" |> group(columns: [\"DeviceId\"])");
                }
                switch (query.WindowWay)
                {
                    case 0:
                        querysql.Append(" |> aggregateWindow(every: 1d, fn: " + merge_way + ", location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        break;
                    case 1:
                        querysql.Append(" |> aggregateWindow(every: 1mo, fn: " + merge_way + ", location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        break;
                    case 2:
                        querysql.Append(" |> aggregateWindow(every: 1h, fn: " + merge_way + ", location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        break;
                    case 3:
                        querysql.Append(" |> aggregateWindow(every: 1m, fn: " + merge_way + ", location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        break;
                    case 4:
                        querysql.Append(" |> aggregateWindow(every: 15m, fn: " + merge_way + ", location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        break;
                }


                querysql.Append(" |> sort(columns: [\"_time\"], desc: true)");

                var fluxTable = await queryApi.QueryAsync(querysql.ToString(), storageConfig.org);
                for (int i = 0; i < fluxTable.Count; i++)
                {
                    //i是参数
                    for (int j = 0; j < fluxTable[i].Records.Count; j++)
                    {
                        try
                        {
                            //j是数据
                            DateTime? time = fluxTable[i].Records[j].GetTimeInDateTime()?.ToLocalTime();
                            string fieldOri = fluxTable[i].Records[j].GetField();
                            int prefixidx = fieldOri.IndexOf("#");
                            if (prefixidx != -1)
                            {
                                fieldOri = fieldOri.Substring(prefixidx + 1);
                            }
                            Dictionary<string, object> values = fluxTable[i].Records[j].Values;
                            string tmpdxId = string.Empty;
                            if (values.TryGetValue("DxId", out object tmpid))
                            {
                                if (tmpid != null)
                                {
                                    tmpdxId = tmpid.ToString();
                                }
                            }
                            string tmpNumber = string.Empty;
                            if (values.TryGetValue("DeviceId", out object tmpdid))
                            {
                                if (tmpdid != null)
                                {
                                    tmpNumber = dviddict[tmpdid.ToString()].DeviceNumber;
                                }
                            }
                            switch (pp.option.type)
                            {
                                case "int":
                                    {
                                        var tmpintop = ((IntOption)pp.option);
                                        finalList.Add(new Out_MergeItem()
                                        {
                                            Id = tmpdxId,
                                            Number = tmpNumber,
                                            MergeWay = merge_way,
                                            Val = values["_value"],
                                            Unit = tmpintop.unit,
                                            Time = time.Value,
                                        });
                                    }
                                    break;
                                case "float":
                                    {
                                        var tmpfloatop = ((FloatOption)pp.option);
                                        finalList.Add(new Out_MergeItem()
                                        {
                                            Id = tmpdxId,
                                            Number = tmpNumber,
                                            MergeWay = merge_way,
                                            Val = values["_value"],
                                            Unit = tmpfloatop.unit,
                                            Time = time.Value,
                                        });
                                    }
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.LogError("influxdb错误" + Newtonsoft.Json.JsonConvert.SerializeObject(fluxTable[i].Records[j]));
                        }

                    }
                }

            }

            //同期多个值合并
            IEnumerable<IGrouping<string, Out_MergeItem>> tmplist;
            if (query.WindowWay == 0)
            {
                tmplist = finalList.OrderByDescending(x => x.Time).GroupBy(x => x.Time.ToString("yyyy-MM-dd") + "|" + x.Number + "|" + x.MergeWay);
            }
            else if (query.WindowWay == 1)
            {
                tmplist = finalList.OrderByDescending(x => x.Time).GroupBy(x => x.Time.ToString("yyyy-MM") + "|" + x.Number + "|" + x.MergeWay);
            }
            else if (query.WindowWay == 2)
            {
                tmplist = finalList.OrderByDescending(x => x.Time).GroupBy(x => x.Time.ToString("yyyy-MM-dd HH") + "|" + x.Number + "|" + x.MergeWay);
            }
            else if (query.WindowWay == 3 || query.WindowWay == 4)
            {
                tmplist = finalList.OrderByDescending(x => x.Time).GroupBy(x => x.Time.ToString("yyyy-MM-dd HH:mm") + "|" + x.Number + "|" + x.MergeWay);
            }
            else
            {
                tmplist = finalList.OrderByDescending(x => x.Time).GroupBy(x => x.Time.ToString("yyyy-MM-dd HH:mm:ss") + "|" + x.Number + "|" + x.MergeWay);
            }


            List<Out_MergeItem> mergeList = new List<Out_MergeItem>();
            foreach (var tmp in tmplist)
            {
                if (tmp.Count() > 1)
                {
                    if (tmp.Key.EndsWith("max"))
                    {
                        mergeList.Add(tmp.MaxBy(x => x.Val));
                    }
                    else if (tmp.Key.EndsWith("min"))
                    {
                        mergeList.Add(tmp.MinBy(x => x.Val));
                    }
                    else if (tmp.Key.EndsWith("mean"))
                    {
                        var tmpavg = tmp.Select(x => Convert.ToDouble(x.Val)).Average();
                        var tmpfirst = tmp.First();
                        mergeList.Add(new Out_MergeItem()
                        {
                            MergeWay = "mean",
                            Val = tmpavg,
                            Unit = tmpfirst.Unit,
                            Time = tmpfirst.Time,
                            Id = tmpfirst.Id,
                            Number = tmpfirst.Number
                        });
                    }
                    else if (tmp.Key.EndsWith("sum"))
                    {
                        var tmpsum = tmp.Select(x => Convert.ToDouble(x.Val)).Sum();
                        var tmpfirst = tmp.First();
                        mergeList.Add(new Out_MergeItem()
                        {
                            MergeWay = "sum",
                            Val = tmpsum,
                            Unit = tmp.First().Unit,
                            Time = tmp.First().Time,
                            Id = tmpfirst.Id,
                            Number = tmpfirst.Number
                        });
                    }
                    else if (tmp.Key.EndsWith("first"))
                    {
                        mergeList.Add(tmp.First());
                    }
                    else if (tmp.Key.EndsWith("last"))
                    {
                        mergeList.Add(tmp.Last());
                    }

                }
                else
                {
                    mergeList.Add(tmp.First());
                }
            }

            return BusResponse<List<Out_MergeItem>>.Success(mergeList);
        }
        public virtual async Task<PageObject<DeviceProperty>> SelectHistory(In_HistoryBase query)
        {
            IotDeviceDAL deviceDAL = _provider.GetService<IotDeviceDAL>();
            IotProductDAL productDAL = _provider.GetService<IotProductDAL>();


            MZ_IotDevice device = null;
            if (query is In_HistoryList quid)
            {
                device = await deviceDAL.Select(quid.Id);
            }
            else if (query is In_HistoryListSync qusyn)
            {
                device = (await deviceDAL.SelectList(x => x.DeviceNumber == qusyn.Number)).FirstOrDefault();
            }

            if (device == null)
            {
                throw new Exception("设备不存在");
            }

            var product = await productDAL.Select(device.ProductId);
            var model = TslModel.CreateFrom(product.ModelTSL);

            Dictionary<string, BaseProperty> hsdict = new Dictionary<string, BaseProperty>();
            foreach (BaseProperty bp in model.properties)
            {
                hsdict.Add(bp.code, bp);
            }

            if (string.IsNullOrEmpty(product.StorageConfig))
            {
                throw new Exception("请配置产品的存储方式");
            }

            var storageConfig = JsonConvert.DeserializeObject<InfluxOption>(product.StorageConfig);
            if (string.IsNullOrEmpty(storageConfig.url))
            {
                throw new Exception("请配置产品的存储方式");
            }
            using var client = new InfluxDBClient(storageConfig.url, storageConfig.token);
            var queryApi = client.GetQueryApi();
            int totalNumbers = -1;
            StringBuilder querysql = new StringBuilder();

            querysql.Append($"from(bucket:\"{storageConfig.bucket.Replace("\"", "")}\")");
            DateTime? queryStart = query.BeginTime;
            DateTime? queryEnd = query.EndTime;
            if (queryStart != null && queryEnd != null)
            {
                querysql.Append($" |> range(start: {TimeZoneInfo.ConvertTime(queryStart.Value, TimeZoneInfo.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ")},stop:{TimeZoneInfo.ConvertTime(queryEnd.Value, TimeZoneInfo.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ")})");
            }
            else
            {
                querysql.Append($" |> range(start: -inf,stop:now())");
            }
            querysql.Append(" |> filter(fn: (r) => r[\"_measurement\"] == \"device\")");
            querysql.Append(" |> filter(fn: (r) => r[\"DeviceId\"] == \"" + device.DeviceId + "\")");


            if (query.pageNum != null && query.pageNum > 0)
            {
                if (!string.IsNullOrEmpty(query.Code))
                {
                    string[] codes = query.Code.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    string filterfields = "";
                    foreach (string tmpcode in codes)
                    {
                        BaseProperty pp;
                        if (hsdict.TryGetValue(tmpcode, out pp))
                        {
                            if (string.IsNullOrEmpty(filterfields))
                            {
                                filterfields = "r[\"_field\"] == \"" + pp.option.type + "#" + tmpcode + "\"";
                            }
                            else
                            {
                                filterfields = filterfields + " or r[\"_field\"] == \"" + pp.option.type + "#" + tmpcode + "\"";
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(filterfields))
                    {
                        querysql.Append($" |> filter(fn: (r) => {filterfields})");
                    }

                }

                if (query.showTotal == true)
                {
                    string totalCountQuery = querysql.ToString() + " |> count()";
                    var totalTable = await queryApi.QueryAsync(totalCountQuery, storageConfig.org);
                    if (totalTable.Count > 0)
                    {
                        totalNumbers = Convert.ToInt32(totalTable[0].Records[0].Values["_value"]);
                    }
                }


                int startRow = (query.pageNum.Value - 1) * query.pageSize.Value;
                if (startRow < 0) startRow = 0;
                querysql.Append(" |> sort(columns: [\"_time\"], desc: true)");
                querysql.Append(" |> limit(n:" + query.pageSize + ",offset:" + startRow + ")");

            }
            else
            {
                bool isObjCode = false;
                if (!string.IsNullOrEmpty(query.Code))
                {
                    BaseProperty pp;
                    if (!hsdict.TryGetValue(query.Code, out pp))
                    {
                        return new PageObject<DeviceProperty>();
                    }
                    if (pp.option.type == "geo" || pp.option.type == "string" || pp.option.type == "enum")
                    {
                        isObjCode = true;
                    }
                    querysql.Append(" |> filter(fn: (r) => r[\"_field\"] == \"" + pp.option.type + "#" + query.Code + "\")");

                    if (queryEnd == null)
                    {
                        queryEnd = DateTime.Now;
                    }
                    if (queryStart == null)
                    {
                        queryStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                    }
                    if (!isObjCode)
                    {
                        TimeSpan ts = queryEnd.Value - queryStart.Value;
                        string influxDBTimeZone = TimeZoneInfo.Local.Id;
                        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                        {
                            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                            influxDBTimeZone = TZConvert.WindowsToIana(localTimeZone.Id);
                        }
                        if (ts.TotalHours > 1 && ts.TotalHours <= 24)
                        {
                            querysql.Append(" |> aggregateWindow(every: 2m, fn: mean, location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        }
                        else if (ts.TotalHours > 24 && ts.TotalHours <= 720)
                        {
                            querysql.Append(" |> aggregateWindow(every: 1h, fn: mean, location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        }
                        else if (ts.TotalHours > 720)
                        {
                            querysql.Append(" |> aggregateWindow(every: 1d, fn: mean, location: {zone: \"" + influxDBTimeZone + "\", offset: 0s}, createEmpty: false)");
                        }
                    }
                }
                querysql.Append(" |> sort(columns: [\"_time\"], desc: true)");
            }

            var fluxTable = await queryApi.QueryAsync(querysql.ToString(), storageConfig.org);
            List<DeviceProperty> tlist = new List<DeviceProperty>();
            for (int i = 0; i < fluxTable.Count; i++)
            {
                //i是参数
                for (int j = 0; j < fluxTable[i].Records.Count; j++)
                {
                    try
                    {
                        //j是数据
                        DateTime? time = fluxTable[i].Records[j].GetTimeInDateTime()?.ToLocalTime();
                        string fieldOri = fluxTable[i].Records[j].GetField();
                        int prefixidx = fieldOri.IndexOf("#");
                        if (prefixidx != -1)
                        {
                            fieldOri = fieldOri.Substring(prefixidx + 1);
                        }
                        Dictionary<string, object> values = fluxTable[i].Records[j].Values;
                        BaseProperty pp;
                        if (!hsdict.TryGetValue(fieldOri, out pp))
                        {
                            continue;
                        }
                        switch (pp.option.type)
                        {
                            case "int":
                                {
                                    var tmpintop = ((IntOption)pp.option);
                                    tlist.Add(new DeviceProperty()
                                    {
                                        Name = pp.name,
                                        Code = pp.code,
                                        Value = values["_value"],
                                        Unit = tmpintop.unit,
                                        UpdatedOn = time,
                                        OptionType = pp.option.type,
                                        Description = pp.description
                                    });
                                }
                                break;
                            case "float":
                                {
                                    var tmpfloatop = ((FloatOption)pp.option);
                                    tlist.Add(new DeviceProperty()
                                    {
                                        Name = pp.name,
                                        Code = pp.code,
                                        Value = values["_value"],
                                        Unit = tmpfloatop.unit,
                                        UpdatedOn = time,
                                        OptionType = pp.option.type,
                                        Description = pp.description
                                    });
                                }
                                break;
                            case "enum":
                                {
                                    var tmpenumop = ((EnumOption)pp.option);
                                    tlist.Add(new DeviceProperty()
                                    {
                                        Name = pp.name,
                                        Code = pp.code,
                                        Value = values["_value"],
                                        Unit = string.Empty,
                                        UpdatedOn = time,
                                        OptionType = pp.option.type,
                                        Description = pp.description
                                    });
                                }
                                break;
                            case "date":
                                {
                                    long tmpl = Convert.ToInt64(values["_value"]);
                                    var dto = DateTimeOffset.FromUnixTimeMilliseconds(tmpl);
                                    tlist.Add(new DeviceProperty()
                                    {
                                        Name = pp.name,
                                        Code = pp.code,
                                        Value = dto.LocalDateTime.ToString(((DateOption)pp.option).format),
                                        Unit = string.Empty,
                                        UpdatedOn = time,
                                        OptionType = pp.option.type,
                                        Description = pp.description
                                    });
                                }
                                break;
                            default:
                                tlist.Add(new DeviceProperty()
                                {
                                    Name = pp.name,
                                    Code = pp.code,
                                    Value = JsonConvert.DeserializeObject(values["_value"].ToString()),
                                    Unit = string.Empty,
                                    UpdatedOn = time,
                                    OptionType = pp.option.type,
                                    Description = pp.description
                                });
                                continue;
                        }
                    }
                    catch { }

                }
            }

            var rt = new PageObject<DeviceProperty>();
            if (totalNumbers != -1)
            {
                rt.List = tlist.OrderByDescending(x => x.UpdatedOn).ToList();
                rt.Total = totalNumbers;
            }
            else
            {
                rt.List = tlist.OrderByDescending(x => x.UpdatedOn).ToList();
                rt.Total = tlist.Count;
            }
            return rt;
        }
    }

}
