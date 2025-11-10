using AuthService.Controller;
using Common;
using Common.Share;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace ReportService.Controller
{
    public class ReportGroup : AbstractLoginedController
    {
        private ReportGroupBLL _groupBLL;
        public ReportGroup(ReportGroupBLL groupBLL)
        {
            _groupBLL = groupBLL;
        }
        /// <summary>
        /// 报表分组树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/ReportService/Report/List")]
        public async Task<DefaultAjaxResult<List<MZ_ReportGroup>>> ListTree()
        {
            var allcls = await _groupBLL.SelectAllOfOrg(GetUser());
            return this.Success(MZ_ReportGroup.BuildTree(allcls));
        }
        /// <summary>
        /// 报表分组选择树
        /// </summary>
        /// <returns></returns>
        [About("/ReportService/Report/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<List<TreeSelect<string>>>> TreeSelect()
        {
            var allcls = await _groupBLL.SelectAllOfOrg(GetUser());
            List<TreeSelect<string>> tlist = MZ_ReportGroup.GroupList2Tree(MZ_ReportGroup.BuildTree(allcls));
            return this.Success(tlist);
        }
        /// <summary>
        /// 获取指定分组信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_ReportGroup>> Info(string id)
        {
            return this.Success(await _groupBLL.Info(id));
        }

        [About("/ReportService/Report/List")]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_ReportGroup group)
        {
            return (await _groupBLL.InsertGroup(group, GetUser())).ToAjaxResult();
        }

        [About("/ReportService/Report/List")]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_ReportGroup group)
        {
            group.OrgId = null;
            return (await _groupBLL.UpdateGroup(group, GetUser())).ToAjaxResult();
        }


        [About("/ReportService/Report/List")]
        [HttpGet]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _groupBLL.DeleteGroup(id, GetUser())).ToAjaxResult();
        }
    }
}
