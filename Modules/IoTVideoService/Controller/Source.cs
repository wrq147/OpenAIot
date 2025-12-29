using AuthService.Controller;
using Common;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using IoTVideoService.Business;
using IoTVideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;
namespace IoTVideoService.Controller
{
    public class Source : AbstractLoginedController
    {
        private VideoSourceBLL _videoSourceBLL;
        public Source(VideoSourceBLL sourceBLL)
        {
            _videoSourceBLL = sourceBLL;
        }
        /// <summary>
        /// 视频源列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_VideoSource>>> ListPage(In_VideoSourcePage query)
        {
            return this.Success(await _videoSourceBLL.SelectPage(query, GetUser()));
        }


        /// <summary>
        /// 获取视频源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_VideoSource>> Info(string id)
        {
            return (await _videoSourceBLL.Info(id)).ToAjaxResult();
        }


        /// <summary>
        /// 添加视频源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Add(MZ_VideoSource data)
        {
            return (await _videoSourceBLL.Insert(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 编辑视频源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_VideoSource data)
        {
            return (await _videoSourceBLL.Update(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除视频源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _videoSourceBLL.Delete(id, GetUser())).ToAjaxResult();
        }
    }
}
