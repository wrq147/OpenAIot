using AuthService.Controller;
using CardService.Business;
using CardService.Model;
using Common;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 名片产品API
    /// </summary>
    public class Pro : AbstractLoginedController
    {
        private CardProBLL _cardPro;
        public Pro(CardProBLL cardPro)
        {
            _cardPro = cardPro;
        }
        #region 产品
        /// <summary>
        /// 查询产品列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List(In_Card_Pro query)
        {
            return this.Success(await _cardPro.SelectList(query));
        }
        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return (await _cardPro.SelectById(id)).ToAjaxResult();
        }
        /// <summary>
        /// 管理添加产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [TANetValid]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Card_Pro data)
        {
            return (await _cardPro.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 管理编辑产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Card_Pro data)
        {
            return (await _cardPro.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 标记删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _cardPro.Remove(id)).ToAjaxResult();
        }
        /// <summary>
        /// 恢复产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Recover(long id)
        {
            return (await _cardPro.Recover(id)).ToAjaxResult();
        }
        #endregion

        #region 产品分类
        /// <summary>
        /// 查询产品分类
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> CategoryList(In_Card_Category query)
        {
            return this.Success(await _cardPro.SelectCategoryList(query));
        }
        /// <summary>
        /// 产品分类排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> SortCategory(In_ProCateSort sort)
        {
            return (await _cardPro.UpdateCategorySort(sort.idList, sort.orgId)).ToAjaxResult();
        }
        /// <summary>
        /// 添加产品分类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [TANetValid]
        [HttpPost]
        public async Task<AjaxResult> AddCategory(MZ_Card_Category data)
        {
            return (await _cardPro.InsertCategory(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑产品分类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> EditCategory(MZ_Card_Category data)
        {
            return (await _cardPro.UpdateCategory(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除产品分类
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RemoveCategory(long id)
        {
            return (await _cardPro.RemoveCategory(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取分类下拉树列表
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> CategoryTreeSelect(In_Card_Category query)
        {
            List<MZ_Card_Category> depts = await _cardPro.SelectCategoryList(query);
            List<TreeSelect<long>> tlist = _cardPro.CategoryList2Tree(_cardPro.BuildCategoryTree(depts));
            return this.Success(tlist);
        }

        #endregion
    }
}
