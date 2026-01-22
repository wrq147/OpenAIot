using IoTService.Models;
using IoTService.Third.Api.SimBoss;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TemplateAction.Core;
using Common.Share;
using IoTService.DAL;
using Common.Json;

namespace IoTService.Third.Api
{
    public class SimBossApi : IThirdApi
    {
        private static String defaultCharSet = "utf-8";

        private HttpClient _httpClient;
        private SimBossOption _option;
        private ITAServiceProvider _provider;
        public SimBossApi(ITAServiceProvider provider, MZ_IotConfig config)
        {
            _provider = provider;
            this._httpClient = new HttpClient();
            if (string.IsNullOrEmpty(config.SimBossOption))
            {
                _option = new SimBossOption();
            }
            else
            {
                _option = System.Text.Json.JsonSerializer.Deserialize<SimBossOption>(config.SimBossOption, MyDefaultTextJsonConfig.DefaultOptions);
            }
        }


        public async Task<MZ_IotCard> QueryCardInfo(string iccid)
        {
            DeviceDetailRequest request = new DeviceDetailRequest();
            request.Iccid = iccid;
            SimbossResponse<DeviceDetailModel> response = await this.Excute<DeviceDetailModel>(request);
            if (response.Success)
            {
                var detail = response.Data;
                MZ_IotCard card = new MZ_IotCard();
                card.ICCID = detail.Iccid;
                card.IMSI = detail.Imsi;
                card.MSISDN = detail.Msisdn;
                card.SpeedLimit = detail.SpeedLimit;
                card.CardPoolId = detail.CardPoolId.ToString();
                card.CardType = detail.Type;
                if (detail.Carrier == "cmcc")
                {
                    card.Carrier = "移动";
                }
                else if (detail.Carrier == "unicom")
                {
                    card.Carrier = "联通";
                }
                else if (detail.Carrier == "chinanet")
                {
                    card.Carrier = "电信";
                }
                card.Status = detail.Status;
                card.RatePlanName = detail.IratePlanName;
                card.RatePlanId = detail.RatePlanId.ToString();
                card.TotalDataVolume = detail.TotalDataVolume;
                card.UsedDataVolume = detail.UsedDataVolume;
                card.UseCountAsVolume = detail.UseCountAsVolume;
                card.StartDate = detail.StartDate;
                card.ExpirationDate = detail.ExpireDate;
                return card;
            }
            return null;
        }
        public async Task<List<MZ_IotCard>> QueryCardInfoList(List<string> iccids)
        {
            DeviceDetailBatchRequest request = new DeviceDetailBatchRequest();
            foreach (string iccid in iccids)
            {
                request.AddIccid(iccid);
            }

            SimbossResponse<List<DeviceDetailModel>> response = await this.Excute<List<DeviceDetailModel>>(request);
            if (response.Success)
            {
                List<MZ_IotCard> tlist = new List<MZ_IotCard>();
                foreach (var detail in response.Data)
                {
                    MZ_IotCard card = new MZ_IotCard();
                    card.ICCID = detail.Iccid;
                    card.IMSI = detail.Imsi;
                    card.MSISDN = detail.Msisdn;
                    card.SpeedLimit = detail.SpeedLimit;
                    card.CardPoolId = detail.CardPoolId.ToString();
                    card.CardType = detail.Type;
                    if (detail.Carrier == "cmcc")
                    {
                        card.Carrier = "移动";
                    }
                    else if (detail.Carrier == "unicom")
                    {
                        card.Carrier = "联通";
                    }
                    else if (detail.Carrier == "chinanet")
                    {
                        card.Carrier = "电信";
                    }
                    card.Status = detail.Status;
                    card.RatePlanName = detail.IratePlanName;
                    card.RatePlanId = detail.RatePlanId.ToString();
                    card.TotalDataVolume = detail.TotalDataVolume;
                    card.UsedDataVolume = detail.UsedDataVolume;
                    card.UseCountAsVolume = detail.UseCountAsVolume;
                    card.StartDate = detail.StartDate;
                    card.ExpirationDate = detail.ExpireDate;
                    tlist.Add(card);
                }
                return tlist;
            }
            return new List<MZ_IotCard>();
        }

        public async Task<BusResponse<string>> Recharge(MZ_IotCard card, int month)
        {
            DeviceRechargeRequest request = new DeviceRechargeRequest();
            request.Iccid = card.ICCID;
            request.RatePlanId = Convert.ToInt32(card.RatePlanId);
            request.Month = month;
            SimbossResponse<string> response = await this.Excute<string>(request);
            if (response.Success)
            {
                return BusResponse<string>.Success(response.Data);
            }
            else
            {
                return BusResponse<string>.Error(Convert.ToInt32(response.Code), response.Message);
            }
        }



        private async Task<SimbossResponse<T>> Excute<T>(SimbossRequest request)
        {
            SimbossResponse<T> response;
            String result = null;
            try
            {
                String url = UriConstants.API_URL + request.GetUri();
                SortedDictionary<String, String> paramDic = GetRequestParam(request);
                result = await Post(url, paramDic);
                response = System.Text.Json.JsonSerializer.Deserialize<SimbossResponse<T>>(result, MyDefaultTextJsonConfig.DefaultOptions);
            }
            catch (System.Exception e)
            {
                response = new SimbossResponse<T>();
                response.Success = false;
                response.Message = e.Message + ", result: " + result;
                response.Code = "599";
            }
            return response;
        }

        private async Task<string> Post(string url, SortedDictionary<String, String> paramDic)
        {
            var encoding = Encoding.GetEncoding(defaultCharSet);
            List<String> paramList = new List<string>();
            foreach (KeyValuePair<String, String> kv in paramDic)
            {
                paramList.Add(string.Format(
                    string.Format("{0}={1}",
                    HttpUtility.UrlEncode(kv.Key, encoding),
                    HttpUtility.UrlEncode(kv.Value, encoding))));
            }
            String data = string.Join("&", paramList);
            StringContent content = new StringContent(data, encoding, "application/x-www-form-urlencoded");
            //content.Headers.ContentEncoding.Add(defaultCharSet);
            using (var message = await _httpClient.PostAsync(url, content).ConfigureAwait(false))
            {
                var response = await message.Content.ReadAsByteArrayAsync();
                var responseString = encoding.GetString(response);
                return responseString;
            }
        }

        private SortedDictionary<String, String> GetRequestParam(SimbossRequest request)
        {
            SortedDictionary<String, String> paramDic = request.GetParam();
            paramDic.Add("appid", _option.app_id);
            paramDic.Add("timestamp", TimeHelper.GetTimeStamp());
            String sign = SignatureHelper.GetSignature(paramDic, _option.app_secret);
            paramDic.Add("sign", sign);
            return paramDic;
        }
        public void Dispose()
        {
            this._httpClient.Dispose();
        }

        public Task<BusResponse<string>> StopSimStatusBatch(List<string> msisdns, List<string> iccids)
        {
            throw new NotImplementedException();
        }
    }
}
