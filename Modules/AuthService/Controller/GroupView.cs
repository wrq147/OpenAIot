using Common.Share;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using AuthService.Business;
using TemplateAction.Route;
using AuthService.Model;

namespace AuthService.Controller
{
    /// <summary>
    /// 通用分组API
    /// </summary>
    public class GroupView : AbstractLoginedController
    {
        private GroupViewBLL _groupViewBLL;
        public GroupView(GroupViewBLL groupViewBLL)
        {
            _groupViewBLL = groupViewBLL;
        }

        /// <summary>
        /// 获取分组列表
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_GroupView>>> List(string table)
        {
            return this.Success(await _groupViewBLL.SelectGroupList(table, GetUser()));
        }
        /// <summary>
        /// 获取指定分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_GroupView>> Info(string id)
        {
            return this.Success(await _groupViewBLL.Info(id));
        }
        /// <summary>
        /// 分组排序
        /// </summary>
        /// <param name="ids">排序Id列表</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Sort(List<string> ids)
        {
            return (await _groupViewBLL.UpdateSort(ids)).ToAjaxResult();
        }
        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_GroupView data)
        {
            return (await _groupViewBLL.Insert(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 编辑分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_GroupView data)
        {
            return (await _groupViewBLL.Update(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _groupViewBLL.Remove(id, GetUser())).ToAjaxResult();
        }
    }
}
