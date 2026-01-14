using AuthService.Controller;
using ChannelUtility.Message;
using Common;
using Common.Share;
using IoTService.Models;
using IoTVideoService.Business;
using IoTVideoService.Models;
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
        /// <param name="sid">设备源Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<PresetInfo>>> GetPresetList(string sid)
        {
            return this.Success(await _ptzBLL.GetPresetList(sid));
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

        /// <summary>
        /// 获取视频的通道列表
        /// </summary>
        /// <param name="sid">设备源Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_VideoChannel>>> GetChannelList(string sid)
        {
            return this.Success(await _ptzBLL.GetChannelList(sid));
        }


        /// <summary>
        /// 执行PTZ控制
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ControlPTZ(In_PtzControlParam data)
        {
            return (await _ptzBLL.ControlPTZ(data)).ToAjaxResult();
        }
    }
}
