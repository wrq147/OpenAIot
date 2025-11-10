using AuthService;
using Common;
using Common.Share;
using MonitorService.Business;
using MonitorService.DAL;
using SKIT.FlurlHttpClient.Wechat.Api;
using SKIT.FlurlHttpClient.Wechat.Api.Models;
using SKIT.FlurlHttpClient.Wechat.Work;
using SKIT.FlurlHttpClient.Wechat.Work.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TemplateAction.Core;
using WeiXinService.Model;
namespace WeiXinService
{
    /// <summary>
    /// 获取小程序接口
    /// </summary>
    public class WxApiHelper
    {
        private ITAServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string REDIS_HASH_KEY = "WXRedis";
        private const string REDIS_HASH_TOKEN = "WXRedisToken";
        private const string REDIS_JS_TOKEN = "WXJSToken";
        public WxApiHelper(IHttpClientFactory httpClientFactory, ITAServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> DefaultAppId(string appid)
        {
            if (string.IsNullOrEmpty(appid))
            {
                var tmpconfig = await _serviceProvider.GetService<WeiXinConfig>().GetJsonConfig();
                return tmpconfig.default_appid;
            }
            return appid;
        }
        public async Task<BusResponse<string>> AddAccount(MZ_WXAccount account)
        {
            var redis = _serviceProvider.GetService<GeneralRedisHelper>();
            if (await redis.HashExistsAsync(REDIS_HASH_KEY, account.AppId))
            {
                return BusResponse<string>.Error(111, "微信应用已存在");
            }
            await redis.HashSetAsync(REDIS_HASH_KEY, account.AppId, account);

            string jobname = "WxRefreshJob";
            string group = "SYSTEM";
            var tmpjob = await _serviceProvider.GetService<JobDAL>().SelectJobByName(jobname, group);
            if (tmpjob != null)
            {
                await _serviceProvider.GetService<JobBLL>().RunJob(tmpjob);
            }
            return BusResponse<string>.Success();
        }
        public async Task<List<MZ_WXAccount>> AccountList()
        {
            var dict = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAllAsync<MZ_WXAccount>(REDIS_HASH_KEY);
            return dict.Select(x => x.Value).ToList();
        }
        public async Task RemoveAccount(string appid)
        {
            await _serviceProvider.GetService<GeneralRedisHelper>().HashDeleteAsync(REDIS_HASH_KEY, appid);
        }

        public async Task<MZ_WXAccount> AccountInfo(string appid)
        {
            return await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<MZ_WXAccount>(REDIS_HASH_KEY, appid);
        }





        #region 微信接口
        public async Task<WechatApiClient> CreateWxClient(string appid)
        {
            var account = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<MZ_WXAccount>(REDIS_HASH_KEY, appid);
            return await CreateWxClient(account);
        }
        public async Task<WechatApiClient> CreateWxClient(MZ_WXAccount account)
        {
            if (account == null)
                throw new Exception("未在配置项中找到该 AppId 对应的微信账号。");


            var wechatApiClientOptions = new WechatApiClientOptions()
            {
                AppId = account.AppId,
                AppSecret = account.AppSecret
            };
            var wechatApiClient = new WechatApiClient(wechatApiClientOptions);
            wechatApiClient.Configure((settings) => settings.FlurlHttpClientFactory = new DelegatingFlurlClientFactory(_httpClientFactory));
            return wechatApiClient;
        }
        public async Task<WechatWorkClient> CreateCorpClient(string appid)
        {
            var account = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<MZ_WXAccount>(REDIS_HASH_KEY, appid);
            return await CreateCorpClient(account);
        }
        public async Task<WechatWorkClient> CreateCorpClient(MZ_WXAccount account)
        {
            var options = new WechatWorkClientOptions()
            {
                CorpId = account.AppId,
                AgentId = Convert.ToInt32(account.AgentId),
                AgentSecret = account.AppSecret
            };
            var client = new WechatWorkClient(options);
            client.Configure((settings) => settings.FlurlHttpClientFactory = new DelegatingFlurlClientFactory(_httpClientFactory));
            return client;
        }
        /// <summary>
        /// 获取访问用户身份
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<BusResponse<Out_CorpUserInfo>> GetCorpUserInfo(string appid, string code)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var account = await AccountInfo(appid);
            var tmpclient = await CreateCorpClient(account);
            var request = new CgibinAuthGetUserInfoRequest() { AccessToken = appletToken, Code = code };
            var response = await tmpclient.ExecuteCgibinAuthGetUserInfoAsync(request);
            if (response.IsSuccessful())
            {
                if (string.IsNullOrEmpty(response.UserId) || response.UserId.Contains('/'))
                {
                    return BusResponse<Out_CorpUserInfo>.Error(111, "非企业成员");
                }
                Out_CorpUserInfo out_CorpUserInfo = new Out_CorpUserInfo();
                out_CorpUserInfo.userid = response.UserId;
                out_CorpUserInfo.user_ticket = response.UserTicket;
                out_CorpUserInfo.OrgId = account.OrgId;
                return BusResponse<Out_CorpUserInfo>.Success(out_CorpUserInfo);
            }
            else
            {
                return BusResponse<Out_CorpUserInfo>.Error(110, response.ErrorMessage);
            }
        }

        /// <summary>
        /// 获取访问用户敏感信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<BusResponse<Out_CorpUserDetail>> GetCorpUserDetail(string appid, string userid)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var tmpclient = await CreateCorpClient(appid);

            SKIT.FlurlHttpClient.Wechat.Work.Models.CgibinUserGetRequest userRequest = new SKIT.FlurlHttpClient.Wechat.Work.Models.CgibinUserGetRequest() { AccessToken = appletToken, UserId = userid };
            var userResponse = await tmpclient.ExecuteCgibinUserGetAsync(userRequest);
            if (!userResponse.IsSuccessful())
            {
                return BusResponse<Out_CorpUserDetail>.Error(109, userResponse.ErrorMessage);
            }

            CgibinDepartmentListRequest deptReuest = new CgibinDepartmentListRequest() { AccessToken = appletToken };
            var deptRsp = await tmpclient.ExecuteCgibinDepartmentListAsync(deptReuest);
            if (!deptRsp.IsSuccessful())
            {
                return BusResponse<Out_CorpUserDetail>.Error(111, deptRsp.ErrorMessage);
            }
            var mainDept = deptRsp.DepartmentList.Where(x => userResponse.MainDepartmentId == x.DepartmentId).FirstOrDefault();
            Out_CorpUserDetail tmpdetail = new Out_CorpUserDetail();
            tmpdetail.userid = userResponse.UserId;
            tmpdetail.avatar = userResponse.AvatarUrl;
            tmpdetail.qr_code = userResponse.QrcodeUrl;
            tmpdetail.name = userResponse.Name;
            tmpdetail.mobile = userResponse.MobileNumber;
            if (userResponse.Gender == 1)
            {
                tmpdetail.gender = "0";
            }
            else if (userResponse.Gender == 2)
            {
                tmpdetail.gender = "1";
            }
            else
            {
                tmpdetail.gender = "2";
            }
            tmpdetail.gender = userResponse.Gender.ToString();
            tmpdetail.postname = userResponse.Position;
            if (mainDept != null)
            {
                tmpdetail.deptname = mainDept.Name;
            }

            return BusResponse<Out_CorpUserDetail>.Success(tmpdetail);
        }
        /// <summary>
        /// 推送企业微信消息
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public async Task<BusResponse<string>> PushCorpTemplateMsg(string appid, List<string> toUsers, string title, string content, string url)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var tmpclient = await CreateCorpClient(appid);
            var request = new CgibinMessageSendRequest() { AccessToken = appletToken };
            request.ToUserIdList = toUsers;
            if (string.IsNullOrEmpty(url))
            {
                request.MessageType = "text";
                request.MessageContentForText = new CgibinMessageSendRequest.Types.TextMessage()
                {
                    Content = content,
                };
            }
            else
            {
                request.MessageType = "textcard";
                request.MessageContentForTextCard = new CgibinMessageSendRequest.Types.TextCardMessage()
                {
                    Title = title,
                    Description = content,
                    Url = url
                };
            }

            var response = await tmpclient.ExecuteCgibinMessageSendAsync(request);
            if (response.IsSuccessful())
            {

                return BusResponse<string>.Success();
            }
            else
            {
                return BusResponse<string>.Error(110, response.ErrorMessage);
            }
        }
        /// <summary>
        /// 获取企业微信JSSDK配置信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<BusResponse<IDictionary<string, string>>> GetCorpWxConfigJson(string appid, string url)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_JS_TOKEN, appid);
            var client = await CreateCorpClient(appid);
            var rs = client.GenerateParametersForJSSDKAgentConfig(appletToken, url);
            return BusResponse<IDictionary<string, string>>.Success(rs);
        }
        /// <summary>
        /// 获取企业微信的所有部门信息
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        public async Task<BusResponse<CgibinDepartmentListResponse.Types.Department[]>> GetDepartmentList(string appid)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var tmpclient = await CreateCorpClient(appid);
            var request = new CgibinDepartmentListRequest() { AccessToken = appletToken };
            var response = await tmpclient.ExecuteCgibinDepartmentListAsync(request);
            if (response.IsSuccessful())
            {

                return BusResponse<CgibinDepartmentListResponse.Types.Department[]>.Success(response.DepartmentList);
            }
            else
            {
                return BusResponse<CgibinDepartmentListResponse.Types.Department[]>.Error(110, response.ErrorMessage);
            }
        }
        /// <summary>
        /// 获取企业微信的部门成员
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<BusResponse<CgibinUserListResponse.Types.User[]>> GetDepartmentMembers(string appid, long deptId)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var tmpclient = await CreateCorpClient(appid);
            var request = new CgibinUserListRequest() { AccessToken = appletToken, DepartmentId = deptId };
            var response = await tmpclient.ExecuteCgibinUserListAsync(request);
            if (response.IsSuccessful())
            {
                return BusResponse<CgibinUserListResponse.Types.User[]>.Success(response.UserList);
            }
            else
            {
                return BusResponse<CgibinUserListResponse.Types.User[]>.Error(110, response.ErrorMessage);
            }
        }
        /// <summary>
        /// 生成小程序中转tick
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="path"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<BusResponse<string>> GenerateWxSchemeTick(string appid, string path, string query)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var request = new WxaGenerateSchemeRequest() { AccessToken = appletToken };
            request.MiniProgram = new WxaGenerateSchemeRequest.Types.MiniProgram();
            request.MiniProgram.Path = path;
            request.MiniProgram.Query = query;
            var client = await CreateWxClient(appid);
            var response = await client.ExecuteWxaGenerateSchemeAsync(request);
            if (response == null || response.ErrorCode != 0)
            {
                return BusResponse<string>.Error(110, response.ErrorMessage);
            }
            return BusResponse<string>.Success(response.UrlScheme.Replace("weixin://dl/business/?t=", ""));
        }
        /// <summary>
        /// 获取微信JSSDK配置信息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<BusResponse<IDictionary<string, string>>> GetWxConfigJson(string appid, string url)
        {
            var appletTicket = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_JS_TOKEN, appid);
            var client = await CreateWxClient(appid);
            var rs = client.GenerateParametersForJSSDKConfig(appletTicket, url);
            return BusResponse<IDictionary<string, string>>.Success(rs);
        }
        /// <summary>
        /// 获取手机号
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<BusResponse<string>> GetWxMobile(string appid, string code)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var request = new WxaBusinessGetUserPhoneNumberRequest() { AccessToken = appletToken };
            request.Code = code;
            var client = await CreateWxClient(appid);
            var response = await client.ExecuteWxaBusinessGetUserPhoneNumberAsync(request);
            if (response == null || response.ErrorCode != 0)
            {
                return BusResponse<string>.Error(110, response.ErrorMessage);
            }
            return BusResponse<string>.Success(response.PhoneInfo.PhoneNumber);
        }

        /// <summary>
        /// 推送微信公众号消息
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="openId"></param>
        /// <param name="templateId"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="appId"></param>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public async Task<BusResponse<string>> PushTemplateMsg(string appid, string openId, string templateId, string url, Dictionary<string, string> data, string appId = null, string pagePath = null)
        {
            var appletToken = await _serviceProvider.GetService<GeneralRedisHelper>().HashGetAsync<string>(REDIS_HASH_TOKEN, appid);
            var request = new CgibinMessageTemplateSendRequest() { AccessToken = appletToken };
            request.ToUserOpenId = openId;
            request.TemplateId = templateId;
            request.Url = url;
            if (appId != null)
            {
                request.MiniProgram = new CgibinMessageTemplateSendRequest.Types.MiniProgram() { AppId = appId, PagePath = pagePath };
            }
            request.Data = new Dictionary<string, CgibinMessageTemplateSendRequest.Types.DataItem>();
            foreach (var item in data)
            {
                request.Data.Add(item.Key, new CgibinMessageTemplateSendRequest.Types.DataItem()
                {
                    Value = item.Value
                });
            }

            var client = await CreateWxClient(appid);
            var response = await client.ExecuteCgibinMessageTemplateSendAsync(request);
            if (response == null || response.ErrorCode != 0)
            {
                return BusResponse<string>.Error(110, response.ErrorMessage);
            }
            return BusResponse<string>.Success();
        }
        #endregion
        /// <summary>
        /// 刷新微信各个应用的令牌
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> RefreshToken()
        {
            try
            {
                GeneralRedisHelper tmpredis = _serviceProvider.GetService<GeneralRedisHelper>();
                var accountDict = await tmpredis.HashGetAllAsync<MZ_WXAccount>(REDIS_HASH_KEY);
                foreach (var item in accountDict)
                {
                    if (item.Value.AccType == "corp")
                    {
                        var tmpclient = await CreateCorpClient(item.Value);
                        var request = new CgibinGetTokenRequest();
                        var response = await tmpclient.ExecuteCgibinGetTokenAsync(request);
                        if (response == null || response.ErrorCode != 0)
                        {
                            return BusResponse<string>.Error(111, response.ErrorMessage);
                        }
                        await tmpredis.HashSetAsync(REDIS_HASH_TOKEN, item.Value.AppId, response.AccessToken);


                        var jsrequest = new CgibinGetJsapiTicketRequest()
                        {
                            AccessToken = response.AccessToken
                        };
                        var jsresponse = await tmpclient.ExecuteCgibinGetJsapiTicketAsync(jsrequest);
                        await tmpredis.HashSetAsync(REDIS_JS_TOKEN, item.Value.AppId, jsresponse.Ticket);
                    }
                    else
                    {
                        var request = new CgibinTokenRequest() { GrantType = "client_credential" };
                        var tmpclient = await CreateWxClient(item.Value);
                        var response = await tmpclient.ExecuteCgibinTokenAsync(request);
                        if (response == null || response.ErrorCode != 0)
                        {
                            return BusResponse<string>.Error(111, response.ErrorMessage);
                        }

                        await tmpredis.HashSetAsync(REDIS_HASH_TOKEN, item.Value.AppId, response.AccessToken);


                        if (item.Value.AccType == "js")
                        {
                            var jsrequest = new CgibinTicketGetTicketRequest()
                            {
                                AccessToken = response.AccessToken
                            };
                            var jsresponse = await tmpclient.ExecuteCgibinTicketGetTicketAsync(jsrequest);
                            await tmpredis.HashSetAsync(REDIS_JS_TOKEN, item.Value.AppId, jsresponse.Ticket);
                        }
                    }

                }

                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(112, ex.Message);
            }
        }
    }

    internal class DelegatingFlurlClientFactory : Flurl.Http.Configuration.DefaultHttpClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DelegatingFlurlClientFactory(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public override HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            return _httpClientFactory.CreateClient();
        }
    }
}
