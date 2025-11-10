
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using Common;
using AuthService.Controller;
using TemplateAction.Route;
using FlowService.Model;
using FlowService.Business;
using System.Threading.Tasks;

namespace FlowService.Controller
{
    [About("/FlowService/Flow")]
    public class Group : AbstractLoginedController
    {
        private GroupBLL _groupBLL;
        public Group(GroupBLL groupBLL)
        {
            _groupBLL = groupBLL;
        }
        [About("List")]
        [HttpGet]
        public async Task<AjaxResult> List(In_GroupList query = null)
        {
            if (query == null) query = new In_GroupList();
            return this.Success(await _groupBLL.SelectGroupList(query));
        }

        [About("List")]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_FlowGroup group)
        {
            return (await _groupBLL.InsertGroup(group)).ToAjaxResult();
        }



        [About("List")]
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_FlowGroup group)
        {
            group.OrgId = null;
            return (await _groupBLL.UpdateGroup(group)).ToAjaxResult();
        }

        [About("List")]
        [HttpPost]
        public async Task<AjaxResult> Sort(List<long> list)
        {
            return (await _groupBLL.UpdateGroupSort(list)).ToAjaxResult();
        }

        [About("List")]
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _groupBLL.DeleteGroup(id)).ToAjaxResult();
        }
    }
}
