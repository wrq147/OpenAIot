using Common;
using Common.Json;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Third.Api
{
    public class UnicomApi : IThirdApi
    {
        private ITAServiceProvider _provider;
        private UnicomOption _option;
        public UnicomApi(ITAServiceProvider provider, MZ_IotConfig config)
        {
            _provider = provider;
            if (string.IsNullOrEmpty(config.UnicomOption))
            {
                _option = new UnicomOption();
            }
            else
            {
                _option = System.Text.Json.JsonSerializer.Deserialize<UnicomOption>(config.UnicomOption, MyDefaultTextJsonConfig.DefaultOptions);
            }
        }
        private static string GetEncodePassword(string text, string encodeKey, string ivKey)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.Zeros;
                aesAlg.Key = Encoding.UTF8.GetBytes(encodeKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(ivKey);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] passwordBytes = Encoding.UTF8.GetBytes(text);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(passwordBytes, 0, passwordBytes.Length);
                        csEncrypt.FlushFinalBlock();
                    }
                    byte[] encryptedBytes = msEncrypt.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }
        private async Task<string> GetToken()
        {
            string tkey = "unicom" + _option.app_id + ":token";
            var redis = _provider.GetService<IotRedisHelper>();
            string tk = await redis.StringGetAsync(tkey);
            if (tk == null)
            {
                string lockkey = "unicom_lock_" + _option.app_id;
                if (await redis.WaitLockTakeAsync(lockkey))
                {
                    try
                    {
                        tk = await redis.StringGetAsync(tkey);
                        if (tk == null)
                        {
                            string loginAuthorization = "Basic " + MyAccess.Core.Crypter.EncodeBase64(_option.app_id + ":" + _option.app_secret, System.Text.Encoding.UTF8);
                            Dictionary<string, string> headers = new Dictionary<string, string>();
                            headers.Add("Tenant", _option.tenantId);
                            headers.Add("Authorization", loginAuthorization);


                            string encodingpass = GetEncodePassword(_option.password, _option.encodeKey, _option.ivKey);
                            Dictionary<string, string> formBodys = new Dictionary<string, string>();
                            formBodys.Add("username", _option.username);
                            formBodys.Add("password", encodingpass);
                            formBodys.Add("grant_type", "password");
                            formBodys.Add("scope", "server");

                            string res = await HttpHelper.Instance.PostWithHeaderAsync($"{_option.appUrl}/auc/oauth/token", formBodys, headers, System.Text.Encoding.UTF8);
                            UnicomResult<UnicomToken> resObj = System.Text.Json.JsonSerializer.Deserialize<UnicomResult<UnicomToken>>(res, MyDefaultTextJsonConfig.DefaultOptions);
                            if (resObj.success == true)
                            {
                                tk = resObj.data.access_token;
                                await redis.StringSetAsync(tkey, tk, TimeSpan.FromSeconds(resObj.data.expires_in - 60));
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
            reqparams.Add("iccid", iccid);
            reqparams.Add("appId", _option.app_id);
            var res = await HttpHelper.Instance.PostJsonAsync($"{_option.appUrl}/cop-platform/api/device/detail", System.Text.Json.JsonSerializer.Serialize(reqparams, MyDefaultTextJsonConfig.DefaultOptions), System.Text.Encoding.UTF8);
            var resObj = System.Text.Json.JsonSerializer.Deserialize<UnicomResult<UnicomCardInfo>>(res, MyDefaultTextJsonConfig.DefaultOptions);
            if (resObj.success == true)
            {
                MZ_IotCard card = new MZ_IotCard();
                card.ICCID = resObj.data.iccid;
                card.IMSI = string.Empty;
                card.MSISDN = resObj.data.msisdn;
                card.StartDate = string.IsNullOrEmpty(resObj.data.activatedTime) ? null : Convert.ToDateTime(resObj.data.activatedTime);
                card.SpeedLimit = -1;
                switch (resObj.data.status)
                {
                    case 1:
                        card.Status = "activation";
                        break;
                    case 2:
                        card.Status = "pending-activation";
                        break;
                    case 3:
                        card.Status = "deactivation";
                        break;
                    case 4:
                    case 8:
                        card.Status = "retired";
                        break;
                    case 5:
                        card.Status = "testing";
                        break;
                    case 6:
                        card.Status = "inventory";
                        break;
                    default:
                        card.Status = string.Empty;
                        break;
                }
                card.RatePlanId = resObj.data.currentRateId;
                Dictionary<string, string> jfparams = new Dictionary<string, string>();
                jfparams.Add("id", resObj.data.currentRateId);
                jfparams.Add("appId", _option.app_id);
                var newres = await HttpHelper.Instance.PostJsonAsync($"{_option.appUrl}/cop-platform/api/rate/details", System.Text.Json.JsonSerializer.Serialize(jfparams, MyDefaultTextJsonConfig.DefaultOptions), System.Text.Encoding.UTF8);
                var newresObj = System.Text.Json.JsonSerializer.Deserialize<UnicomResult<UnicomCharge>>(newres, MyDefaultTextJsonConfig.DefaultOptions);
                if (newresObj.success == true)
                {
                    card.RatePlanName = newresObj.data.rateName;
                    card.TotalDataVolume = newresObj.data.ratedFlowTotal == null ? 0 : Convert.ToDouble(newresObj.data.ratedFlowTotal);
                    card.UsedDataVolume = Convert.ToDouble(resObj.data.cycleUsedFlow);
                    card.UseCountAsVolume = false;
                    card.Carrier = "联通";
                    card.CardType = "POOL";
                    card.CardPoolId = string.Empty;
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
            foreach (string iccid in iccids)
            {
                var tmpption = _provider.GetService<GeneralOption>();
                string newjson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    iccid = iccid,
                    callbackUrl = $"{tmpption.url}/IoTService/UnicomCallback/StausChange",
                    status = 3,
                    appId = _option.app_id
                }, MyDefaultTextJsonConfig.DefaultOptions);
                var res = await HttpHelper.Instance.PostJsonAsync($"{_option.appUrl}/cop-platform/api/device/async-device-state", newjson, Encoding.UTF8);
                var resObj = System.Text.Json.JsonSerializer.Deserialize<UnicomResult<string>>(res, MyDefaultTextJsonConfig.DefaultOptions);
            }
            return BusResponse<string>.Success();
        }
    }


    public class UnicomResult<T>
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 业务数据实体
        /// </summary>
        public T data { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
    }
    public class UnicomToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string license { get; set; }
        public bool active { get; set; }
        public object user_info { get; set; }
    }
    public class UnicomCardInfo
    {
        public string iccid { get; set; }
        public string msisdn { get; set; }
        public string imei { get; set; }
        /// <summary>
        /// 设备状态
        /// 1:已激活；2:可激活；3:已停用；4:已失效；5:可测试；6:库存；7:已更换；8:已清除;
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 实名状态
        /// 0:未实名；1：已实名；2:无需实名
        /// </summary>
        public int isRealName { get; set; }
        /// <summary>
        /// 归属公司
        /// </summary>
        public string accountName { get; set; }
        /// <summary>
        /// 子账户
        /// </summary>
        public string subAccount { get; set; }
        /// <summary>
        /// 设备首次激活时间
        /// </summary>
        public string activatedTime { get; set; }
        /// <summary>
        /// 设备同步时间
        /// 仅在设备激活期间会与设备进行同步
        /// </summary>
        public string synchronizeTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 当前资费ID
        /// </summary>
        public string currentRateId { get; set; }
        /// <summary>
        /// 当前资费类型
        /// 1：月付灵活共享；
        /// 2：月付阶梯计费；
        /// 3：预付单个连接；
        /// 6：月付单个连接；
        /// </summary>
        public int currentRateType { get; set; }
        /// <summary>
        /// 沉默期时间
        /// </summary>
        public string deadlineTime { get; set; }
        /// <summary>
        /// 沉默期资费
        /// </summary>
        public string deadlineRateName { get; set; }
        /// <summary>
        /// 套包账期已用流量（MB）未使用为0
        /// </summary>
        public decimal cycleUsedFlow { get; set; }
        /// <summary>
        /// 套包账期已用语音（Min）未使用为0
        /// </summary>
        public int cycleUsedVoice { get; set; }
        /// <summary>
        /// 套包账期已用短信（条） 未使用为0
        /// </summary>
        public int cycleUsedSms { get; set; }
        /// <summary>
        /// 设备账期已用流量（MB）未使用为0
        /// </summary>
        public decimal simCycleUsedFlow { get; set; }
        /// <summary>
        /// 设备账期已用语音（Min）未使用为0
        /// </summary>
        public int simCycleUsedVoice { get; set; }
        /// <summary>
        /// 设备账期已用短信（条） 未使用为0
        /// </summary>
        public int simCycleUsedSms { get; set; }
    }
    public class UnicomCharge
    {
        /// <summary>
        /// 资费ID
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 资费名称
        /// </summary>
        public string rateName { get; set; }
        /// <summary>
        /// 状态:1 已启用 2 已停用
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 资费类型
        /// 1月付灵活共享 2月付阶梯计费 3预付单个连接 6月付单个连接
        /// </summary>
        public int rateType { get; set; }
        /// <summary>
        /// 售价
        /// </summary>
        public decimal price { get; set; }
        /// <summary>
        /// 额定流量总量（MB）
        /// </summary>
        public decimal? ratedFlowTotal { get; set; }
    }


    public class CallbackResult : AjaxResult
    {
        public override string ToString()
        {
            return "{\"success\": true,\"msg\": \"成功\"}";
        }
    }
}
