using Common;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Third.Api
{
    public class YiDongApi : IThirdApi
    {
        private ITAServiceProvider _provider;
        private YiDongOption _option;
        private int _count;
        public YiDongApi(ITAServiceProvider provider, MZ_IotConfig config)
        {
            _provider = provider;
            _count = 0;
            if (string.IsNullOrEmpty(config.YiDongOption))
            {
                _option = new YiDongOption();
            }
            else
            {
                _option = System.Text.Json.JsonSerializer.Deserialize<YiDongOption>(config.YiDongOption, MyDefaultTextJsonConfig.DefaultOptions);
            }
        }
        private string GenerateTransId()
        {
            int cc = Interlocked.Increment(ref _count) % 10;
            return _option.app_id + DateTime.Now.ToString("yyyyMMddHHmmss") + MyAccess.Core.StringTool.GetDigitChar(7) + cc;
        }
        private async Task<string> GetToken()
        {
            string tkey = _option.app_id + ":token";
            var redis = _provider.GetService<IotRedisHelper>();
            string tk = await redis.StringGetAsync(tkey);
            if (tk == null)
            {
                string lockkey = "yidong_lock_" + _option.app_id;
                if (await redis.WaitLockTakeAsync(lockkey))
                {
                    try
                    {
                        tk = await redis.StringGetAsync(tkey);
                        if (tk == null)
                        {
                            Dictionary<string, string> reqparams = new Dictionary<string, string>();
                            reqparams.Add("appid", _option.app_id);
                            reqparams.Add("password", _option.password);
                            reqparams.Add("transid", GenerateTransId());
                            string server_Ip = "https://api.iot.10086.cn/v5/ec";
                            string res = await HttpHelper.Instance.GetAsync(server_Ip, reqparams);
                            dynamic resObj = System.Text.Json.JsonSerializer.Deserialize<object>(res, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
                            if (resObj.status != null && resObj.status == "0")
                            {
                                tk = resObj.result[0].token;
                                await redis.StringSetAsync(tkey, tk, TimeSpan.FromMinutes(50));
                            }
                        }
                    }
                    finally
                    {
                        await redis.LockReleaseAsync(lockkey);
                    }
                }

            }
            return tk;
        }

        public async Task<MZ_IotCard> QueryCardInfo(string iccid)
        {
            string token = await GetToken();
            Dictionary<string, string> reqparams = new Dictionary<string, string>();
            reqparams.Add("transid", GenerateTransId());
            reqparams.Add("token", token);
            reqparams.Add("iccid", iccid);
            var res = await HttpHelper.Instance.GetAsync("https://api.iot.10086.cn/v5/ec/query/sim-basic-info", reqparams);
            dynamic resObj = System.Text.Json.JsonSerializer.Deserialize<object>(res, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
            if (resObj.status != null && resObj.status == "0")
            {
                MZ_IotCard card = new MZ_IotCard();
                card.ICCID = resObj.result[0].iccid;
                card.IMSI = resObj.result[0].imsi;
                card.MSISDN = resObj.result[0].msisdn;
                card.StartDate = resObj.result[0].activeDate;
                card.SpeedLimit = 153600;


                //获取卡状态
                reqparams = new Dictionary<string, string>();
                reqparams.Add("transid", GenerateTransId());
                reqparams.Add("token", token);
                reqparams.Add("iccid", iccid);
                res = await HttpHelper.Instance.GetAsync("https://api.iot.10086.cn/v5/ec/query/sim-status", reqparams);
                resObj = System.Text.Json.JsonSerializer.Deserialize<object>(res, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
                if (resObj.status != null && resObj.status == "0")
                {
                    string tmpstatus = resObj.result[0].cardStatus;
                    switch (tmpstatus)
                    {
                        case "06":
                        case "07":
                        case "1":
                            card.Status = "pending-activation";
                            break;
                        case "00":
                        case "2":
                            card.Status = "activation";
                            break;
                        case "01":
                        case "02":
                        case "4":
                            card.Status = "deactivation";
                            break;
                        case "6":
                            card.Status = "testing";
                            break;
                        case "7":
                        case "05":
                            card.Status = "inventory";
                            break;
                        case "8":
                        case "03":
                            card.Status = "retired";
                            break;
                        case "99":
                            return null;
                    }

                    //获取卡本月套餐流量用量
                    reqparams = new Dictionary<string, string>();
                    reqparams.Add("transid", GenerateTransId());
                    reqparams.Add("token", token);
                    reqparams.Add("iccid", iccid);
                    res = await HttpHelper.Instance.GetAsync("https://api.iot.10086.cn/v5/ec/query/sim-status", reqparams);
                    resObj = System.Text.Json.JsonSerializer.Deserialize<object>(res, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
                    if (resObj.status != null && resObj.status == "0")
                    {
                        var accmlist = resObj.result[0].accmMarginList;
                        card.RatePlanId = accmlist[0].offeringId;
                        card.RatePlanName = accmlist[0].offeringName;
                        card.TotalDataVolume = double.Parse(accmlist[0].totalAmount.ToString()) / 1024;
                        card.UsedDataVolume = double.Parse(accmlist[0].useAmount.ToString()) / 1024;
                        card.UseCountAsVolume = false;
                        card.Carrier = "移动";
                        card.CardType = "POOL";
                        card.CardPoolId = string.Empty;
                    }

                }
                return card;
            }
            return null;
        }
        public async Task<List<MZ_IotCard>> QueryCardInfoList(List<string> iccids)
        {
            List<MZ_IotCard> cardlist = new List<MZ_IotCard>();
            foreach (var iccid in iccids)
            {
                var card = await QueryCardInfo(iccid);
                if (card == null)
                {
                    continue;
                }
                cardlist.Add(card);
            }
            return cardlist;
        }

        public async Task<BusResponse<string>> Recharge(MZ_IotCard card, int month)
        {
            try
            {
                var cardDAL = _provider.GetService<IotCardDAL>();
                var rs = await cardDAL.AddExpireDate(card.Id, month);
                return BusResponse<string>.Success(rs.ToString());
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(111, ex.Message);
            }
        }


        public async Task<BusResponse<string>> StopSimStatusBatch(List<string> msisdns, List<string> iccids)
        {
            string token = await GetToken();
            var reqparams = new Dictionary<string, string>();
            reqparams.Add("transid", GenerateTransId());
            reqparams.Add("token", token);
            reqparams.Add("msisdns", string.Join('_', msisdns));
            reqparams.Add("operType", "11");
            reqparams.Add("reason", "物联卡过期自动停机");
            var res = await HttpHelper.Instance.GetAsync("https://api.iot.10086.cn/v5/ec/change/sim-status/batch", reqparams);
            dynamic resObj = System.Text.Json.JsonSerializer.Deserialize<object>(res, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
            if (resObj.status != null && resObj.status == "0")
            {
                return BusResponse<string>.Success();
            }
            else
            {
                return BusResponse<string>.Error(111, resObj.message);
            }
        }
    }
}
