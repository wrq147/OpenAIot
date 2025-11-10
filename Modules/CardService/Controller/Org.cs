using AuthService;
using AuthService.Controller;
using CardService.Business;
using System;
using TemplateAction.Core;
using TemplateAction.Route;
using System.Threading.Tasks;
using CardService.Model;
using Common;
using AuthService.Model;
using Common.Share;
using WeiXinService.Model;
using static SKIT.FlurlHttpClient.Wechat.Api.Models.CgibinTagsMembersGetBlackListResponse.Types;

namespace CardService.Controller
{
    /// <summary>
    /// 名片组织关联API
    /// </summary>
    public class Org : AbstractLoginedController
    {
        private CardOrgBLL _cardOrg;
        private CardSmsBLL _sms;
        private OrgBLL _org;
        public Org(CardOrgBLL cardOrg, CardSmsBLL sms, OrgBLL org)
        {
            _cardOrg = cardOrg;
            _sms = sms;
            _org = org;
        }
        /// <summary>
        /// 从当前组织中移除指定用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RemoveUser(long id)
        {
            return (await _cardOrg.DeleteUser(id)).ToAjaxResult();
        }
        /// <summary>
        /// 用户的关联企业信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> OrgView(long id)
        {
            return this.Success(await _cardOrg.SelectUserOrgViewById(id));
        }
        /// <summary>
        /// 修改企业（名片前端用）
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Org data)
        {
            return (await _cardOrg.UpdateOrg(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改用户的企业信息
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> EditUser(MZ_User_Org uo)
        {
            return (await _cardOrg.EditUserOrg(uo)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑指定企业的职位
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="postName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> EditPost(long orgId, string postName)
        {
            IUserInfo user = Data_ServerTokenInfo.From(Context);
            MZ_User_Org userOrg = new MZ_User_Org();
            userOrg.UserId = user.UserId;
            userOrg.OrgId = orgId;
            userOrg.post_name = postName;
            return (await _org.UpdateUserOrg(userOrg)).ToAjaxResult();
        }
        /// <summary>
        /// 配置组织模块信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Config(MZ_Card_Org data)
        {
            return (await _cardOrg.UpdateCardOrg(data)).ToAjaxResult();
        }
        /// <summary>
        /// 获取指定组织详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _cardOrg.SelectById(id));
        }
        /// <summary>
        /// 获取指定用户组织信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UserInfo(long id)
        {
            return (await _cardOrg.SelectUserOrg(id)).ToAjaxResult();
        }
        /// <summary>
        /// 创建新企业
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Create(In_Org org)
        {
            return (await _cardOrg.Create(org)).ToAjaxResult();
        }

        /// <summary>
        /// 切换企业
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Switch(long id)
        {
            return (await _cardOrg.Switch(id)).ToAjaxResult();
        }

        /// <summary>
        /// 用户的关联企业列表
        /// </summary>
        /// <param name="man"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UserOrgList(bool man = false)
        {
            return this.Success(await _cardOrg.SelectUserOrgListById(man));
        }
        /// <summary>
        /// 解散企业
        /// </summary>
        /// <param name="id">企业Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _cardOrg.Delete(id)).ToAjaxResult();
        }
        /// <summary>
        /// 直接加入指定企业
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> JoinAt(In_Join data)
        {
            return (await _cardOrg.Join(data)).ToAjaxResult();
        }
        /// <summary>
        /// 通过邀请码创建或加入企业
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Join(In_JoinOrg data)
        {
            return (await _cardOrg.Join(data)).ToAjaxResult();
        }
        /// <summary>
        /// 退出企业
        /// </summary>
        /// <param name="id">企业Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Exit(long id)
        {
            return (await _cardOrg.ExitOrg(id)).ToAjaxResult();
        }
        /// <summary>
        /// 判断是否加入了指定企业
        /// </summary>
        /// <param name="id">企业Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> IsJoin(long id)
        {
            return (await _cardOrg.IsJoin(id)).ToAjaxResult();
        }
        /// <summary>
        /// 生成邀请码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Invite(MZ_YQCode data)
        {
            return (await _cardOrg.Invite(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 发送邀请短信
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [RepeatableMiddleware]
        public async Task<AjaxResult> SendYqSms(In_YqSms data)
        {
            return (await _sms.SendYaoQing(data.appid, data.tel, data.tk)).ToAjaxResult();
        }
        /// <summary>
        /// 解释邀请码
        /// </summary>
        /// <param name="id">邀请码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ParseYq(string id)
        {
            return (await _cardOrg.ParseYqCode(id)).ToAjaxResult();
        }


    }
}
