using AuthService.Controller;
using Common.Share;
using Common;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using ProducerService.Business;

namespace ProducerService.Controller
{
    /// <summary>
    /// 单位API
    /// </summary>
    public class Unit : AbstractLoginedController
    {
        private UnitBLL _unitBLL;
        public Unit(UnitBLL unitBLL)
        {
            _unitBLL = unitBLL;
        }
        /// <summary>
        /// 单位列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_Unit>>> List(In_UnitList query)
        {
            return this.Success(await _unitBLL.SelectList(query, GetUser()));
        }
        /// <summary>
        /// 添加单位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/ProducerService/Unit/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Unit data)
        {
            return (await _unitBLL.Add(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改单位
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/ProducerService/Unit/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_Unit data)
        {
            return (await _unitBLL.Edit(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除单位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/ProducerService/Unit/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _unitBLL.Delete(id, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 获取单位
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Unit>> Info(string id)
        {
            return (await _unitBLL.Info(id)).ToAjaxResult();
        }

    }
}
