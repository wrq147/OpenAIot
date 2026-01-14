using AuthService.Controller;
using ChannelUtility.Message;
using Common;
using Common.Share;
using IoTVideoService.Business;
using System;
using TemplateAction.Route;

namespace IoTVideoService.Controller
{
    public class Ptz : AbstractLoginedController
    {
        private PtzBLL _ptzBLL;
        public Ptz(PtzBLL ptzBLL)
        {
            _ptzBLL = ptzBLL;
        }
        /// <summary>
        /// 获取预置点列表
        /// </summary>
        /// <param name="soureId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<PresetInfo>>> GetPresetList(string sourceId)
        {
            return this.Success(await _ptzBLL.GetPresetList(sourceId));
        }

        /// <summary>
        /// 获取视频播放Url
        /// </summary>
        /// <param name="sid">设备源Id</param>
        /// <param name="cid">通道Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> GetPlayUrl(string sid, string cid)
        {
            return this.Success(await _ptzBLL.GetPlayUrl(sid, cid));
        }


    }
}
