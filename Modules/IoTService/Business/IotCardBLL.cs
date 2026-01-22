using IoTService.DAL;
using IoTService.Models;
using IoTService.Third.Api;
using IoTService.Third;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using MyAccess.Core;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Common.Share;
using System.Linq.Expressions;
using MyAccess.DB.Builder.WhereToSql;
using Common;
using System.IO;
using AuthService;
using System.Text;
using Common.IdGenerator;
using Common.EventBus;
using System.Diagnostics;
using Common.Json;


namespace IoTService.Business
{
    public class IotCardBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        public IotCardBLL(ITAServiceProvider provider, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _snowflake = snowflake;
        }
        private IThirdApi CreateApi(string t, MZ_IotConfig config)
        {
            switch (t)
            {
                case "YiDong":
                    return new YiDongApi(_provider, config);
                case "SimBoss":
                    return new SimBossApi(_provider, config);
                case "Sohan":
                    return new SohanApi(_provider, config);
                case "Unicom":
                    return new UnicomApi(_provider, config);
                default:
                    return null;
            }
        }

        public virtual async Task<MZ_IotCard> Info(string id)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            return await cardDAL.Select(id);
        }
        public virtual async Task<BusResponse<string>> StopCard(string id)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            var configDAL = _provider.GetService<IotConfigDAL>();
            var cardInfo = await cardDAL.Select(id);
            var config = await configDAL.Select(cardInfo.OrgId.Value);
            var api = CreateApi(cardInfo.CardFrom, config);
            List<string> msisdns = new List<string>();
            msisdns.Add(cardInfo.MSISDN);
            List<string> iccids = new List<string>();
            iccids.Add(cardInfo.ICCID);
            var rs = await api.StopSimStatusBatch(msisdns, iccids);
            return rs;
        }
        /// <summary>
        /// 查询最新卡信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<MZ_IotCard>> QueryNewestInfo(string id)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            var configDAL = _provider.GetService<IotConfigDAL>();
            var cardInfo = await cardDAL.Select(id);
            var config = await configDAL.Select(cardInfo.OrgId.Value);
            if (cardInfo.CardFrom == "Unknow")
            {
                return BusResponse<MZ_IotCard>.Error(112, "无法查询未知来源的卡");
            }
            var newcard = await CreateApi(cardInfo.CardFrom, config).QueryCardInfo(cardInfo.ICCID);
            if (newcard == null)
            {
                return BusResponse<MZ_IotCard>.Error(111, "卡信息查询失败");
            }
            newcard.Id = cardInfo.Id;
            newcard.LastSyncDate = DateTime.Now;
            await cardDAL.Update(newcard);
            newcard.OrgId = cardInfo.OrgId;
            newcard.CreatedOn = cardInfo.CreatedOn;
            newcard.CardFrom = cardInfo.CardFrom;
            newcard.UsingDevice = cardInfo.UsingDevice;
            return BusResponse<MZ_IotCard>.Success(newcard);
        }
        public virtual async Task<BusResponse<string>> Import(string cardFrom, IRequestFile file, IUserInfo user)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            var configDAL = _provider.GetService<IotConfigDAL>();

            var lastIdx = file.FileName.LastIndexOf(".");
            string ext = file.FileName.Substring(lastIdx + 1);
            List<string> ccids = null;
            if (ext == "txt")
            {
                ccids = new List<string>();
                Stream st = file.OpenReadStream();

                using (StreamReader reader = new StreamReader(st))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        ccids.Add(line);
                    }
                }


            }
            else
            {
                Stream st = file.OpenReadStream();
                Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
                FiedNames.Add("ICCID号", new ParamImportToList("ICCID"));
                var tmplist = _provider.GetService<ExcelHelper>().ExcelToList<MZ_IotCard>(st, FiedNames);
                ccids = tmplist.Select(x => x.ICCID).ToList();
            }

            int failureNum = 0;
            StringBuilder failureMsg = new StringBuilder();

            var config = await configDAL.Select(user.OrgId);
            var newcardList = await CreateApi(cardFrom, config).QueryCardInfoList(ccids);
            foreach (var card in newcardList)
            {
                var oldcard = (await cardDAL.SelectList(x => x.ICCID == card.ICCID)).FirstOrDefault();
                if (oldcard == null)
                {
                    card.Id = _snowflake.NextId().ToString();
                    card.UsingDevice = string.Empty;
                    card.OrgId = user.OrgId;
                    card.CardFrom = cardFrom;
                    card.CreatedOn = DateTime.Now;
                    card.LastSyncDate = card.CreatedOn;
                    switch (card.CardFrom)
                    {
                        case "YiDong":
                            {
                                var yidongOption = System.Text.Json.JsonSerializer.Deserialize<YiDongOption>(config.YiDongOption, MyDefaultTextJsonConfig.DefaultOptions);
                                card.ExpirationDate = DateTime.Now.AddMonths(yidongOption.expire_month);
                            }
                            break;
                        case "Sohan":
                            {
                                var sohanOption = System.Text.Json.JsonSerializer.Deserialize<SohanOption>(config.SohanOption, MyDefaultTextJsonConfig.DefaultOptions);
                                card.ExpirationDate = DateTime.Now.AddMonths(sohanOption.expire_month);
                            }
                            break;
                        case "Unicom":
                            {
                                var unicomOption = System.Text.Json.JsonSerializer.Deserialize<UnicomOption>(config.UnicomOption, MyDefaultTextJsonConfig.DefaultOptions);
                                card.ExpirationDate = DateTime.Now.AddMonths(unicomOption.expire_month);
                            }
                            break;
                    }
                    try
                    {
                        await cardDAL.Insert(card);
                    }
                    catch (Exception ex)
                    {
                        failureNum++;
                        failureMsg.Append("<br/>" + failureNum + "、物联卡 " + card.ICCID + " : " + ex.Message);
                    }
                }
                else
                {
                    card.Id = oldcard.Id;
                    card.LastSyncDate = DateTime.Now;
                    try
                    {
                        await cardDAL.Update(card);
                    }
                    catch (Exception ex)
                    {
                        failureNum++;
                        failureMsg.Append("<br/>" + failureNum + "、物联卡 " + card.ICCID + " : " + ex.Message);
                    }
                }

            }

            if (failureNum > 0)
            {
                failureMsg.Insert(0, "很抱歉，共 " + failureNum + " 条数据导入失败，错误如下：");
                return BusResponse<string>.Error(23, failureMsg.ToString());
            }
            else
            {
                return BusResponse<string>.Success(null, "恭喜您，数据已全部导入成功！");
            }

        }

        /// <summary>
        /// 定时处理物联卡信息
        /// </summary>
        /// <returns></returns>
        public virtual async Task SyncCard()
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            var configDAL = _provider.GetService<IotConfigDAL>();
            var userDAL = _provider.GetService<UserDAL>();

            RefAsync<int> alltotal = 0;

            //定时物联网卡自动停机
            var stopcardlist = await cardDAL.SelectPage(x => (x.CardFrom == "YiDong" || x.CardFrom == "Sohan" || x.CardFrom == "Unicom") && x.Status == "activation" && x.ExpirationDate < DateTime.Now.AddDays(1), 1, 100, alltotal, string.Empty);
            var stopcardGroups = stopcardlist.GroupBy(x => new { x.OrgId });
            foreach (var group in stopcardGroups)
            {
                var config = await configDAL.Select(group.Key.OrgId.Value);

                //停止移动卡
                if (!string.IsNullOrEmpty(config.YiDongOption))
                {
                    var api = CreateApi("YiDong", config);
                    var tmpOption = System.Text.Json.JsonSerializer.Deserialize<YiDongOption>(config.YiDongOption, MyDefaultTextJsonConfig.DefaultOptions);
                    if (tmpOption.enable_expire == true)
                    {
                        List<MZ_IotCard> updateList = group.Where(x => x.CardFrom == "YiDong").ToList();
                        List<string> msisdnlist = updateList.Select(x => x.MSISDN).ToList();
                        List<string> iccidlist = updateList.Select(x => x.ICCID).ToList();
                        var rsp = await api.StopSimStatusBatch(msisdnlist, iccidlist);
                    }
                }

                //停止Sohan卡
                if (!string.IsNullOrEmpty(config.SohanOption))
                {
                    var api = CreateApi("Sohan", config);
                    var tmpOption = System.Text.Json.JsonSerializer.Deserialize<SohanOption>(config.SohanOption, MyDefaultTextJsonConfig.DefaultOptions);
                    if (tmpOption.enable_expire == true)
                    {
                        List<MZ_IotCard> updateList = group.Where(x => x.CardFrom == "Sohan").ToList();
                        List<string> msisdnlist = updateList.Select(x => x.MSISDN).ToList();
                        List<string> iccidlist = updateList.Select(x => x.ICCID).ToList();
                        var rsp = await api.StopSimStatusBatch(msisdnlist, iccidlist);
                    }
                }

                //停止联通卡
                if (!string.IsNullOrEmpty(config.UnicomOption))
                {
                    var api = CreateApi("Unicom", config);
                    var tmpOption = System.Text.Json.JsonSerializer.Deserialize<UnicomOption>(config.UnicomOption, MyDefaultTextJsonConfig.DefaultOptions);
                    if (tmpOption.enable_expire == true)
                    {
                        List<MZ_IotCard> updateList = group.Where(x => x.CardFrom == "Unicom").ToList();
                        List<string> msisdnlist = updateList.Select(x => x.MSISDN).ToList();
                        List<string> iccidlist = updateList.Select(x => x.ICCID).ToList();
                        var rsp = await api.StopSimStatusBatch(msisdnlist, iccidlist);
                    }

                }
            }


            //定时同步物联网卡信息
            bool continueSync = true;
            while (continueSync)
            {
                var cardlist = await cardDAL.SelectPage(x => x.Status != "retired" && x.LastSyncDate < DateTime.Now.AddHours(-48), 1, 100, alltotal, "LastSyncDate asc");
                if (cardlist.Count == 0)
                {
                    continueSync = false;
                }

                var cardGroups = cardlist.GroupBy(x => new { x.OrgId, x.CardFrom });
                foreach (var group in cardGroups)
                {
                    var config = await configDAL.Select(group.Key.OrgId.Value);
                    var api = CreateApi(group.Key.CardFrom, config);


                    List<string> iccids = new List<string>();
                    Dictionary<string, MZ_IotCard> updateDict = new Dictionary<string, MZ_IotCard>();
                    foreach (var card in group)
                    {
                        card.LastSyncDate = DateTime.Now;
                        iccids.Add(card.ICCID);
                        updateDict.Add(card.ICCID, card);
                    }

                    var newcardList = await api.QueryCardInfoList(iccids);
                    foreach (var newCard in newcardList)
                    {
                        if (updateDict.TryGetValue(newCard.ICCID, out MZ_IotCard newupdate))
                        {
                            newupdate.CardPoolId = newCard.CardPoolId;
                            newupdate.IMSI = newCard.IMSI;
                            newupdate.MSISDN = newCard.MSISDN;
                            newupdate.SpeedLimit = newCard.SpeedLimit;
                            newupdate.CardPoolId = newCard.CardPoolId;
                            newupdate.CardType = newCard.CardType;
                            newupdate.Carrier = newCard.Carrier;
                            newupdate.Status = newCard.Status;
                            newupdate.RatePlanName = newCard.RatePlanName;
                            newupdate.RatePlanId = newCard.RatePlanId;
                            newupdate.TotalDataVolume = newCard.TotalDataVolume;
                            newupdate.UsedDataVolume = newCard.UsedDataVolume;
                            newupdate.UseCountAsVolume = newCard.UseCountAsVolume;
                            newupdate.StartDate = newCard.StartDate;
                            newupdate.ExpirationDate = newCard.ExpirationDate;
                            newupdate.Status = newCard.Status;
                        }
                    }
                    await cardDAL.UpdateCardList(group.ToList());

                }

                Thread.Sleep(100);
            }

        }


        public virtual async Task<PageObject<MZ_IotCard>> ListPage(In_IotCardListPage query, IUserInfo user)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            Expression<Func<MZ_IotCard, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.Status))
            {
                expression = expression.And(x => x.Status == query.Status);
            }
            if (!string.IsNullOrEmpty(query.CardFrom))
            {
                expression = expression.And(x => x.CardFrom == query.CardFrom);
            }
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(x => x.ICCID.StartsWith(query.Key) || x.IMSI.StartsWith(query.Key) || x.MSISDN.StartsWith(query.Key));
            }
            if (!string.IsNullOrEmpty(query.DeviceKey))
            {
                string tmpkey = StringHelper.SqlLikeFilter(query.DeviceKey);
                string isexistDevice = "EXISTS(select Id from mz_iot_device where mz_iot_card.UsingDevice=Id and (DeviceNumber like '" + tmpkey + "%' or DeviceId like '" + tmpkey + "%' or Name like '" + tmpkey + "%'))";
                expression = expression.And(x => SonSqlFun.SqlCondition(isexistDevice));
            }
            if (query.IsBindDev != null)
            {
                if (query.IsBindDev == true)
                {
                    expression = expression.And(x => x.UsingDevice != "");
                }
                else
                {
                    expression = expression.And(x => x.UsingDevice == "");
                }
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.CreatedOn <= query.endTime);
            }
            if (query.ExpirBeginTime != null)
            {
                expression = expression.And(x => x.ExpirationDate >= query.ExpirBeginTime);
            }
            if (query.ExpirEndTime != null)
            {
                expression = expression.And(x => x.ExpirationDate <= query.ExpirEndTime);
            }
            var devDAL = _provider.GetService<IotDeviceDAL>();
            var tlistpage = await cardDAL.SelectPage(expression, query, "CreatedOn desc");
            var devDict = await devDAL.NavigateDict<MZ_IotCard, string>(tlistpage.List, x => !string.IsNullOrEmpty(x.UsingDevice), x => x.UsingDevice);
            foreach (var t in tlistpage.List)
            {
                if (devDict != null)
                {
                    if (devDict.TryGetValue(t.UsingDevice, out MZ_IotDevice dev))
                    {
                        t.UsingDeviceName = dev.Name;
                    }
                }
            }
            return tlistpage;
        }
        public virtual async Task<BusResponse<int>> BindDevice(string iccid, MZ_IotDevice device)
        {
            var deviceDAL = _provider.GetService<IotDeviceDAL>();
            var configDAL = _provider.GetService<IotConfigDAL>();
            var config = await configDAL.Select(device.OrgId.Value);
            if (config != null && config.EnableAutoAdd == true)
            {
                MZ_IotCard newcard = null;
                string cardfrom = "";
                try
                {
                    if (!string.IsNullOrEmpty(config.SimBossOption) && !config.SimBossOption.Contains("\"app_id\":\"\""))
                    {
                        cardfrom = "SimBoss";
                        newcard = await CreateApi(cardfrom, config).QueryCardInfo(iccid);
                    }

                    if (newcard == null && !string.IsNullOrEmpty(config.YiDongOption) && !config.YiDongOption.Contains("\"app_id\":\"\""))
                    {
                        cardfrom = "YiDong";
                        newcard = await CreateApi(cardfrom, config).QueryCardInfo(iccid);
                        var yidongOption = System.Text.Json.JsonSerializer.Deserialize<YiDongOption>(config.YiDongOption, MyDefaultTextJsonConfig.DefaultOptions);
                        if (newcard != null)
                        {
                            newcard.ExpirationDate = DateTime.Now.AddMonths(yidongOption.expire_month);
                        }
                    }

                    if (newcard == null && !string.IsNullOrEmpty(config.SohanOption) && !config.SohanOption.Contains("\"app_id\":\"\""))
                    {
                        cardfrom = "Sohan";
                        newcard = await CreateApi(cardfrom, config).QueryCardInfo(iccid);
                        var sohanOption = System.Text.Json.JsonSerializer.Deserialize<SohanOption>(config.SohanOption, MyDefaultTextJsonConfig.DefaultOptions);
                        if (newcard != null)
                        {
                            newcard.ExpirationDate = DateTime.Now.AddMonths(sohanOption.expire_month);
                        }
                    }

                    if (newcard == null && !string.IsNullOrEmpty(config.UnicomOption) && !config.UnicomOption.Contains("\"app_id\":\"\""))
                    {
                        cardfrom = "Unicom";
                        newcard = await CreateApi(cardfrom, config).QueryCardInfo(iccid);
                        var unicomOption = System.Text.Json.JsonSerializer.Deserialize<UnicomOption>(config.UnicomOption, MyDefaultTextJsonConfig.DefaultOptions);
                        if (newcard != null)
                        {
                            newcard.ExpirationDate = DateTime.Now.AddMonths(unicomOption.expire_month);
                        }
                    }

                }
                catch { }


                if (newcard == null)
                {
                    cardfrom = "Unknow";
                    newcard = new MZ_IotCard();
                    newcard.ICCID = iccid;
                    newcard.IMSI = string.Empty;
                    newcard.MSISDN = string.Empty;
                    newcard.SpeedLimit = -1;
                    newcard.CardPoolId = string.Empty;
                    newcard.CardType = string.Empty;
                    newcard.Carrier = "未知";
                    newcard.Status = string.Empty;
                    newcard.RatePlanName = string.Empty;
                    newcard.RatePlanId = string.Empty;
                    newcard.TotalDataVolume = -1;
                    newcard.UsedDataVolume = -1;
                    newcard.UseCountAsVolume = false;
                    newcard.StartDate = null;
                    newcard.ExpirationDate = null;
                }


                newcard.CardFrom = cardfrom;

                var cardDAL = _provider.GetService<IotCardDAL>();
                var tmpcardlist = await cardDAL.SelectList(x => x.ICCID == iccid && x.OrgId == device.OrgId);
                if (tmpcardlist.Count > 0)
                {
                    if (device.Id != tmpcardlist[0].UsingDevice)
                    {
                        newcard.UsingOn = DateTime.Now;
                        newcard.UsingDevice = device.Id;
                    }
                    newcard.LastSyncDate = DateTime.Now;
                    newcard.Id = tmpcardlist[0].Id;
                    var rs = await cardDAL.Update(newcard);
                    return BusResponse<int>.Success(rs);
                }
                else
                {
                    newcard.OrgId = device.OrgId;
                    newcard.CreatedOn = DateTime.Now;
                    newcard.LastSyncDate = newcard.CreatedOn;
                    newcard.Id = _snowflake.NextId().ToString();
                    newcard.UsingOn = DateTime.Now;
                    newcard.UsingDevice = device.Id;
                    var rs = await cardDAL.Insert(newcard);
                    return BusResponse<int>.Success(rs);
                }
            }
            else
            {
                return BusResponse<int>.Error(5, "未配置第三方接入或未启用自动添加物联卡");
            }

        }
        public virtual async Task<BusResponse<int>> UnUsing(string id)
        {
            try
            {
                MZ_IotCard card = new MZ_IotCard();
                card.Id = id;
                card.UsingDevice = string.Empty;
                var cardDAL = _provider.GetService<IotCardDAL>();
                return BusResponse<int>.Success(await cardDAL.Update(card));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task<BusResponse<int>> UsingDevice(string id, string devId)
        {
            try
            {
                MZ_IotCard card = new MZ_IotCard();
                card.Id = id;
                card.UsingDevice = devId;
                card.UsingOn = DateTime.Now;
                var cardDAL = _provider.GetService<IotCardDAL>();
                return BusResponse<int>.Success(await cardDAL.Update(card));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> Remove(string[] ids, IUserInfo user)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            try
            {
                return BusResponse<int>.Success(await cardDAL.Delete(x => x.OrgId == user.OrgId && ids.Contains(x.Id)));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }
        }
        public virtual async Task<MZ_IotCard> GetCard(string number)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            var cards = await cardDAL.QueryCards(number);
            return cards.OrderByDescending(x => x.UsingOn).FirstOrDefault();
        }
        public virtual async Task<List<MZ_IotCard>> GetCardList(string[] number)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            return await cardDAL.QueryCardList(number);
        }
        public virtual async Task<BusResponse<string>> Recharge(string[] ids, int month)
        {
            var cardDAL = _provider.GetService<IotCardDAL>();
            var configDAL = _provider.GetService<IotConfigDAL>();
            var cardlist = await cardDAL.SelectList(x => ids.Contains(x.Id));
            var cardGroups = cardlist.GroupBy(x => new { x.OrgId, x.CardFrom });
            int failureNum = 0;
            StringBuilder failureMsg = new StringBuilder();
            foreach (var group in cardGroups)
            {
                var config = await configDAL.Select(group.Key.OrgId.Value);
                var api = CreateApi(group.Key.CardFrom, config);
                foreach (var card in group)
                {
                    if (api == null)
                    {
                        failureNum++;
                        failureMsg.Append("<br/>" + failureNum + "、物联卡 " + card.ICCID + " 充值失败: 未知卡来源");
                        continue;
                    }
                    var res = await api.Recharge(card, month);
                    if (!res.IsSuccess())
                    {
                        failureNum++;
                        failureMsg.Append("<br/>" + failureNum + "、物联卡 " + card.ICCID + " 充值失败: " + res.Message);
                    }

                    var newcard = await api.QueryCardInfo(card.ICCID);
                    if (newcard != null)
                    {
                        newcard.Id = card.Id;
                        newcard.LastSyncDate = DateTime.Now;
                        await cardDAL.Update(newcard);
                    }
                }
            }

            if (failureNum > 0)
            {
                failureMsg.Insert(0, "很抱歉，共 " + failureNum + " 条数据充值失败，错误如下：");
                return BusResponse<string>.Error(23, failureMsg.ToString());
            }
            else
            {
                return BusResponse<string>.Success(null, "恭喜您，已全部充值成功！");
            }
        }
    }
}
