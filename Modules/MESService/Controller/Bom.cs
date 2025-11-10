using AuthService.Controller;
using Common;
using Common.Share;
using MESService.Business;
using MESService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace MESService.Controller
{
    /// <summary>
    /// 物料清单API
    /// </summary>
    public class Bom : AbstractLoginedController
    {
        private BomBLL _bomBLL;
        public Bom(BomBLL bomBLL)
        {
            _bomBLL = bomBLL;
        }
        /// <summary>
        /// 查询物料清单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_BomHeader>>> List(In_BomList query)
        {
            return this.Success(await _bomBLL.SelectList(query, GetUser()));
        }

        /// <summary>
        /// 查询指定产品的所有层级的物料清单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About]
        public async Task<DefaultAjaxResult<List<Out_BomTreeItem>>> ListTree(string id)
        {
            return this.Success(await _bomBLL.SelectBomTree(id));
        }
        /// <summary>
        /// 获取物料清单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_BomHeader>> Info(string id)
        {
            return (await _bomBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 新增物料清单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Bom/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_BomHeader data)
        {
            return (await _bomBLL.Insert(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 编辑物料清单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Bom/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_BomHeader data)
        {
            return (await _bomBLL.Update(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除物料清单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/MESService/Bom/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _bomBLL.Delete(id, GetUser())).ToAjaxResult();
        }
    }
}
