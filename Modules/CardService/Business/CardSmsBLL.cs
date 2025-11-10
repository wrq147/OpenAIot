using AuthService;
using CardService.Model;
using Common;
using Common.EventBus;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using WeiXinService;

namespace CardService.Business
{
    public class CardSmsBLL
    {
        private ITAServiceProvider _provider;
        private ITAContext _context;
        public CardSmsBLL(ITAServiceProvider serviceProvider, ITAContext context)
        {
            _provider = serviceProvider;
            _context = context;
        }
        public async Task<BusResponse<string>> SendYaoQing(string appid, string phone, string tick)
        {
            WxApiHelper apiHelper = _provider.GetService<WxApiHelper>();
            appid = await apiHelper.DefaultAppId(appid);
            var user = Data_ServerTokenInfo.From(_context);
            GeneralRedisHelper redis = _provider.GetService<GeneralRedisHelper>();
            string tkey = "yq_count_" + user.UserId;
            Tx_Yq_Count cc = await redis.StringGetAsync<Tx_Yq_Count>(tkey);
            if (cc == null)
            {
                cc = new Tx_Yq_Count();
                cc.first_send = new DateTime();
                cc.count = 1;
            }
            else
            {
                cc.count++;
            }
            if (cc.count > 10)
            {
                return BusResponse<string>.Error(110, "短时间内发送太多邀请短信,请1小时后重试");
            }
            await redis.StringSetAsync(tkey, cc, TimeSpan.FromMinutes(30));

            var rs = await apiHelper.GenerateWxSchemeTick(appid, "/pages/index", "jt=3&yq=" + tick);
            if (!rs.IsSuccess())
            {
                return rs;
            }
            //发送短信
            List<TargetUser> users = new List<TargetUser>();
            users.Add(new TargetUser()
            {
                phone = phone
            });
            var nt = new NoticeEvent(2, users.ToArray(), new string[] { "SMS" });
            nt.TargetType = "跳转短信";
            nt.TargetUrl = phone;
            nt.Content = rs.Data;
            await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
            return BusResponse<string>.Success();
        }
    }
}
