using AuthService;
using ShortLinkService.Business;
using System;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace ShortLinkService.Controller
{
    public class JmpLink : TANetController
    {
        private ConfigBLL _config;
        private ShortLinkBLL _shortLinkBLL;
        public JmpLink(ConfigBLL config, ShortLinkBLL shortLinkBLL)
        {
            _config = config;
            _shortLinkBLL = shortLinkBLL;
        }
        /// <summary>
        /// 短信跳转小程序中转页面
        /// </summary>
        /// <returns></returns>
        [Route("wx")]
        public async Task<ViewResult> Wx()
        {
            string jpage = await _config.SelectConfigByKey("smsjmp");
            return new ViewResult(jpage);
        }
        /// <summary>
        /// 短链接跳转
        /// </summary>
        /// <returns></returns>
        [Route("lk/{id}")]
        public async Task<TextResult> Short(string id)
        {
            var tshortLink = await _shortLinkBLL.Info(id);
            if (tshortLink == null)
            {
                return new TextResult("id不存在");
            }
            Response.Redirect(tshortLink.Url);
            return new TextResult(string.Empty);
        }
    }
}
