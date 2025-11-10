using AuthService.Controller;
using Common;
using Common.Share;
using IoTRulesService.Business;
using IoTRulesService.Model;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace IoTRulesService.Controller
{
    public class RuleGroup : AbstractLoginedController
    {
        private RuleGroupBLL _groupBLL;
        public RuleGroup(RuleGroupBLL groupBLL)
        {
            _groupBLL = groupBLL;
        }
        /// <summary>
        /// 设备分组树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_RuleGroup>>> ListTree()
        {
            var allcls = await _groupBLL.SelectAllOfOrg(GetUser());
            return this.Success(MZ_RuleGroup.BuildTree(allcls));
        }
        /// <summary>
        /// 设备分组选择树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<TreeSelect<string>>>> TreeSelect()
        {
            var allcls = await _groupBLL.SelectAllOfOrg(GetUser());
            List<TreeSelect<string>> tlist = MZ_RuleGroup.GroupList2Tree(MZ_RuleGroup.BuildTree(allcls));
            return this.Success(tlist);
        }
        /// <summary>
        /// 获取指定分组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_RuleGroup>> Info(string id)
        {
            return this.Success(await _groupBLL.Info(id));
        }
        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_RuleGroup data)
        {
            return (await _groupBLL.Insert(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑分组
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_RuleGroup data)
        {
            return (await _groupBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _groupBLL.Remove(id)).ToAjaxResult();
        }
    }
}
