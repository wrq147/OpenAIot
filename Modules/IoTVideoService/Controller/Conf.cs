using AuthService.Controller;
using Common.Share;
using IoTService.Models;
using IoTVideoService.Business;
using IoTVideoService.Models;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using Common;

namespace IoTVideoService.Controller
{
    public class Conf : AbstractLoginedController
    {
        private ConfigBLL _confBLL;
        public Conf(ConfigBLL confBLL)
        {
            _confBLL = confBLL;
        }

        /// <summary>
        /// 获取策略分页列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotVideoConfig>>> ListPage(In_VideoConfigPage query)
        {
            return this.Success(await _confBLL.SelectPage(query, GetUser()));
        }

        /// <summary>
        /// 获取指定策略信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotVideoConfig>> Info(string id)
        {
            return this.Success(await _confBLL.Info(id));
        }

        /// <summary>
        /// 添加策略
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Add(MZ_IotVideoConfig data)
        {
            return (await _confBLL.Insert(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 编辑策略
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_IotVideoConfig data)
        {
            return (await _confBLL.Update(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除策略
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _confBLL.Remove(id, GetUser())).ToAjaxResult();
        }
    }
}
