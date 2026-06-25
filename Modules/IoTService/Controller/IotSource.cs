using AuthService.Controller;
using Common.Share;
using Common;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;
using IoTService.Business;
using TemplateAction.Core;

namespace IoTService.Controller
{
    /// <summary>
    /// 物联历史数据的数据源
    /// </summary>
    public class IotSource : AbstractLoginedController
    {
        private IotHisSourceBLL _iotHisSourceBLL;
        public IotSource(IotHisSourceBLL iotHisSourceBLL)
        {
            _iotHisSourceBLL = iotHisSourceBLL;
        }

        /// <summary>
        /// 物联历史数据源列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotHisSource>>> ListPage(In_HisSourceList query)
        {
            return this.Success(await _iotHisSourceBLL.SelectPage(query, GetUser()));
        }

        /// <summary>
        /// 获取物联历史数据源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotHisSource>> Info(string id)
        {
            return (await _iotHisSourceBLL.Info(id)).ToAjaxResult();
        }


        /// <summary>
        /// 添加物联历史数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Add(MZ_IotHisSource data)
        {
            return (await _iotHisSourceBLL.Insert(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 编辑物联历史数据源
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_IotHisSource data)
        {
            return (await _iotHisSourceBLL.Update(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除物联历史数据源
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string[] ids)
        {
            return (await _iotHisSourceBLL.Delete(ids, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除历史数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> RemoveHistory(In_HistoryAllDelete data)
        {
            var rsp = await this.ServiceProvider.GetService<IotInfluxBLL>().DeleteAllHistory(data, GetUser());
            return rsp.ToAjaxResult();
        }
    }
}
