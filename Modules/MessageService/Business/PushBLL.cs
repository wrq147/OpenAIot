using Common;
using Common.Share;
using MessageService.DAL;
using MessageService.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MessageService.Business
{
    public class PushBLL
    {
        private PushClientDAL _clientDAL;
        protected ITAServiceProvider _provider;
        public PushBLL(PushClientDAL clientDAL, ITAServiceProvider provider)
        {
            _clientDAL = clientDAL;
            _provider = provider;
        }

        public virtual async Task<BusResponse<int>> Update(MZ_PushClient data)
        {
            data.UpdatedOn = DateTime.Now;
            var rs = await _clientDAL.CreateOrUpdate(data);
            return BusResponse<int>.Success(rs);
        }
        public const string PUSH_AUTH_TOKEN = "Push_Token";
        public virtual async Task RefreshToken()
        {
            var tmpconfig = await _provider.GetService<MessageConfig>().GetJsonConfig();
            long timestamp = MyAccess.Core.TypeConvert.Time2Unix(DateTime.Now);
            string encodesign = MyAccess.Core.Crypter.SHA256(tmpconfig.push_appkey + timestamp + tmpconfig.push_mastersecret, Encoding.UTF8);
            string newjson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                sign = encodesign,
                timestamp = timestamp,
                appkey = tmpconfig.push_appkey
            });
            int trycount = 0;
            bool isSuccess = false;
            while (!isSuccess && trycount < 4)
            {
                var rsp = await HttpHelper.Instance.PostJsonAsync($"https://restapi.getui.com/v2/{tmpconfig.push_appid}/auth", newjson, Encoding.UTF8);
                var bar = Newtonsoft.Json.JsonConvert.DeserializeObject<PushAuthResponse>(rsp);
                if (bar != null && bar.code == 0)
                {
                    isSuccess = true;
                    await _provider.GetService<GeneralRedisHelper>().StringSetAsync(PUSH_AUTH_TOKEN, bar.data.token);
                }
                ++trycount;
            }
        }
        /// <summary>
        /// 单推
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> PushSingle(long uid, string title, string content, string payload)
        {
            var client = await _clientDAL.Select(uid);
            if (client == null)
            {
                return BusResponse<string>.Error(1, "客户端Id不存在");
            }

            var tmpconfig = await _provider.GetService<MessageConfig>().GetJsonConfig();
            string newjson = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                request_id = MyAccess.Core.StringTool.GetGUID(),
                audience = new
                {
                    cid = new string[] { client.ClientId }
                },
                push_message = new
                {
                    notification = new
                    {
                        title = title,
                        body = content,
                        click_type = "payload",
                        payload = payload
                    }
                }
            });
            var authToken = await _provider.GetService<GeneralRedisHelper>().StringGetAsync(PUSH_AUTH_TOKEN);
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("token", authToken);
            var rsp = await HttpHelper.Instance.PostJsonWithHeaderAsync($"https://restapi.getui.com/v2/{tmpconfig.push_appid}/push/single/cid", newjson, headers, Encoding.UTF8);
            var spres = Newtonsoft.Json.JsonConvert.DeserializeObject<SinglePushResponse>(rsp);
            if (spres != null && spres.code == 0)
            {
                return BusResponse<string>.Success();
            }
            return BusResponse<string>.Error(spres.code, spres.msg);
        }
    }
}
