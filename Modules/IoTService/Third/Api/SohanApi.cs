using Common;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Third.Api
{
    public class SohanApi : IThirdApi
    {
        private ITAServiceProvider _provider;
        private SohanOption _option;
        public SohanApi(ITAServiceProvider provider, MZ_IotConfig config)
        {
            _provider = provider;
            if (string.IsNullOrEmpty(config.SohanOption))
            {
                _option = new SohanOption();
            }
            else
            {
                _option = System.Text.Json.JsonSerializer.Deserialize<SohanOption>(config.SohanOption, MyDefaultTextJsonConfig.DefaultOptions);
            }
        }
        private int GetCarrier(string iccid)
        {
            if (iccid.StartsWith("898600") || iccid.StartsWith("898602") || iccid.StartsWith("898604") || iccid.StartsWith("898607") || iccid.StartsWith("898608"))
            {
                return 1;
            }
            else if (iccid.StartsWith("898601") || iccid.StartsWith("898606") || iccid.StartsWith("898609"))
            {
                return 2;
            }
            else if (iccid.StartsWith("898603") || iccid.StartsWith("898611"))
            {
                return 3;
            }
            return 0;
        }
        private string GetSign(IDictionary<string, object> map, string key)
        {
            map.Remove("sign");
            List<string> list = new List<string>();
            foreach (var entry in map)
            {
                object value = entry.Value;
                if (null != value)
                {
                    if (value is IList)
                    {
                        list.Add(entry.Key + "=" + System.Text.Json.JsonSerializer.Serialize(value, MyDefaultTextJsonConfig.DefaultOptions) + "&");
                    }
                    else
                    {
                        list.Add(entry.Key + "=" + value.ToString() + "&");
                    }
                }
            }


            string[] arrstr = list.OrderBy(x => x).ToArray();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < arrstr.Length; i++)
            {
                sb.Append(arrstr[i]);
            }
            string result = sb.ToString();
            result += "key=" + key;
            return MyAccess.Core.Crypter.MD5(result).ToUpper();
        }
        public async Task<MZ_IotCard> QueryCardInfo(string iccid)
        {
            int carrier = GetCarrier(iccid);
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("method", "sohan.m2m.iccidinfo.queryone");
            map.Add("username", _option.app_id);
            map.Add("timestamp", MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now));
            map.Add("iccid", iccid);
            map.Add("operator", carrier);
            string tmpsign = GetSign(map, _option.app_secret);
            map.Add("sign", tmpsign);

            var res = await HttpHelper.Instance.PostJsonAsync("https://apim2m.iot-sohan.cn/index/m2m/api/v1", System.Text.Json.JsonSerializer.Serialize(map, MyDefaultTextJsonConfig.DefaultOptions), Encoding.UTF8);
            dynamic resObj = System.Text.Json.JsonSerializer.Deserialize<object>(res, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
            if (resObj.errorCode != null && resObj.errorCode == "SUCCESS")
            {
                MZ_IotCard card = new MZ_IotCard();
                card.ICCID = resObj.data.iccid;
                card.IMSI = resObj.data.imsi;
                card.MSISDN = resObj.data.msisdn;
                card.StartDate = resObj.data.activatedTime;
                card.SpeedLimit = 153600;
                if (resObj.data.sourceType == 0)
                {
                    card.CardType = "SINGLE";
                }
                else
                {
                    card.CardType = "POOL";
                }
                if (carrier == 1)
                {
                    card.Carrier = "移动";
                }
                else if (carrier == 2)
                {
                    card.Carrier = "联通";
                }
                else if (carrier == 3)
                {
                    card.Carrier = "电信";
                }
                card.CardPoolId = resObj.data.poolCode;

                if (resObj.data.cardStatus == 1)
                {
                    card.Status = "pending-activation";
                }
                else if (resObj.data.cardStatus == 2)
                {
                    card.Status = "activation";
                }
                else if (resObj.data.cardStatus == 3)
                {
                    card.Status = "deactivation";
                }
                else if (resObj.data.cardStatus == 4)
                {
                    card.Status = "retired";
                }
                else if (resObj.data.cardStatus == 5)
                {
                    card.Status = "inventory";
                }
                else
                {
                    card.Status = "testing";
                }

                card.RatePlanName = resObj.data.bagList[0].bagName;
                card.RatePlanId = resObj.data.bagList[0].bagNo;
                card.TotalDataVolume = double.Parse(resObj.data.totalFlow.ToString());
                card.UsedDataVolume = double.Parse(resObj.data.totalUsedFlow.ToString());
                card.UseCountAsVolume = false;
                return card;
            }
            return null;
        }

        public async Task<List<MZ_IotCard>> QueryCardInfoList(List<string> iccids)
        {
            int carrier = GetCarrier(iccids[0]);
            List<MZ_IotCard> tlist = new List<MZ_IotCard>();
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("method", "sohan.m2m.iccidinfo.query");
            map.Add("username", _option.app_id);
            map.Add("timestamp", MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now));
            map.Add("iccids", iccids);
            map.Add("operator", carrier);
            string tmpsign = GetSign(map, _option.app_secret);
            map.Add("sign", tmpsign);

            var res = await HttpHelper.Instance.PostJsonAsync("https://apim2m.iot-sohan.cn/index/m2m/api/v1", System.Text.Json.JsonSerializer.Serialize(map, MyDefaultTextJsonConfig.DefaultOptions), Encoding.UTF8);
            dynamic resObj = System.Text.Json.JsonSerializer.Deserialize<object>(res, MyDefaultTextJsonConfig.DefaultOptions) as ExpandoObject;
            if (resObj.errorCode != null && resObj.errorCode == "SUCCESS")
            {
                foreach (var tobj in resObj.data.iccds)
                {
                    MZ_IotCard card = new MZ_IotCard();
                    card.ICCID = tobj.iccid;
                    card.IMSI = tobj.imsi;
                    card.MSISDN = tobj.msisdn;
                    card.StartDate = tobj.activatedTime;
                    card.SpeedLimit = 153600;
                    if (tobj.poolCode == "-")
                    {
                        card.CardType = "SINGLE";
                    }
                    else
                    {
                        card.CardType = "POOL";
                    }
                    if (carrier == 1)
                    {
                        card.Carrier = "移动";
                    }
                    else if (carrier == 2)
                    {
                        card.Carrier = "联通";
                    }
                    else if (carrier == 3)
                    {
                        card.Carrier = "电信";
                    }
                    card.CardPoolId = tobj.poolCode;

                    if (tobj.cardStatus == 1)
                    {
                        card.Status = "pending-activation";
                    }
                    else if (tobj.cardStatus == 2)
                    {
                        card.Status = "activation";
                    }
                    else if (tobj.cardStatus == 3)
                    {
                        card.Status = "deactivation";
                    }
                    else if (tobj.cardStatus == 4)
                    {
                        card.Status = "retired";
                    }
                    else if (tobj.cardStatus == 5)
                    {
                        card.Status = "inventory";
                    }
                    else
                    {
                        card.Status = "testing";
                    }

                    card.RatePlanName = tobj.bagList[0].bagName;
                    card.RatePlanId = tobj.bagList[0].bagNo;
                    card.TotalDataVolume = double.Parse(tobj.totalFlow.ToString());
                    card.UsedDataVolume = double.Parse(tobj.totalUsedFlow.ToString());
                    card.UseCountAsVolume = false;
                    tlist.Add(card);
                }
            }
            return tlist;
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
            foreach (string iccid in iccids)
            {
                Dictionary<string, object> map = new Dictionary<string, object>();
                map.Add("method", "sohan.m2m.iccid.deactivate");
                map.Add("username", _option.app_id);
                map.Add("timestamp", MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now));
                map.Add("iccid", iccid);
                string tmpsign = GetSign(map, _option.app_secret);
                map.Add("sign", tmpsign);
                await HttpHelper.Instance.PostJsonAsync("https://apim2m.iot-sohan.cn/index/m2m/api/v1", System.Text.Json.JsonSerializer.Serialize(map, MyDefaultTextJsonConfig.DefaultOptions), Encoding.UTF8);
            }
            return BusResponse<string>.Success();
        }
    }
}
