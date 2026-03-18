using Common;
using Common.Share;
using DeveloperService;
using DeveloperService.Model;
using IoTVideoService.Business;
using System;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;
namespace IoTVideoService.Controller
{
    public class HttpSync : AbstractDeveloperController
    {
        private MZ_Developer _develper;
        /// <summary>
        /// 校验开发者权限
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            _develper = GetDeveloper();
            if (_develper.UserType != 1)
            {
                return this.Error<string>(11, "必需为企业开发者");
            }
            return await base.CallAction(ac, parameters);
        }

        /// <summary>
        /// 获取指定视频监控的Rtmp播放流
        /// </summary>
        /// <param name="id">通讯Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Dictionary<string, string>>> GetRtmpPlayUrl(string id)
        {
            var ptzBLL = this.ServiceProvider.GetService<PtzBLL>();

            var tdict = await ptzBLL.GetPlayUrlDict(id, "rtmp");
            return this.Success(tdict);
        }
    }
}
