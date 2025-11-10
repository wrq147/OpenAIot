using AuthService.Controller;
using Common.Share;
using Common;
using StorageService.Business;
using StorageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace StorageService.Controller
{
    /// <summary>
    /// 仓库API
    /// </summary>
    public class House : AbstractLoginedController
    {
        private HouseBLL _houseBLL;
        public House(HouseBLL houseBLL)
        {
            _houseBLL = houseBLL;
        }
        /// <summary>
        /// 仓库列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_StoreHouse>>> List(In_HouseList query)
        {
            return this.Success(await _houseBLL.SelectList(query));
        }
        /// <summary>
        /// 添加仓库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Add(MZ_StoreHouse data)
        {
            return (await _houseBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 修改仓库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_StoreHouse data)
        {
            return (await _houseBLL.Edit(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除仓库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _houseBLL.Delete(id)).ToAjaxResult();
        }

        /// <summary>
        /// 获取仓库
        /// </summary>
        /// <param name="id"></param>
        /// <param name="showTemplateName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_StoreHouse>> Info(string id, bool showTemplateName = true)
        {
            return (await _houseBLL.Info(id, showTemplateName)).ToAjaxResult();
        }
        /// <summary>
        /// 修改仓库状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> ChangeStaus(string id, string status)
        {
            return (await _houseBLL.ChangeStaus(id, status)).ToAjaxResult();
        }
    }
}
