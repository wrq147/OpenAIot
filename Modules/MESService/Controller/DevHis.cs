using AuthService.Controller;
using Common;
using Common.Share;
using MESService.Business;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace MESService.Controller
{
    /// <summary>
    /// 工序设备采集历史API
    /// </summary>
    public class DevHis : AbstractLoginedController
    {
        private BatchDevHisBLL _devHisBLL;
        public DevHis(BatchDevHisBLL devHisBLL)
        {
            _devHisBLL = devHisBLL;
        }

        /// <summary>
        /// 查询生产的设备采集数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_BatchDevHis>>> List(In_BatchDevHisList query)
        { 
            return this.Success(await _devHisBLL.SelectByPage(query, GetUser()));
        }

        /// <summary>
        /// 记录生产的设备采集数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Report/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Record(In_BatchDevHisData data)
        {
            return (await _devHisBLL.Insert(data, GetUser())).ToAjaxResult();
        }

    }
}
