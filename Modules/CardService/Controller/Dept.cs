using AuthService;
using AuthService.Controller;
using AuthService.Model;
using CardService.Business;
using Common;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 名片用部门API
    /// </summary>
    public class Dept : AbstractLoginedController
    {
        private DeptBLL _dept;
        public Dept(DeptBLL dept)
        {
            _dept = dept;
        }

        /// <summary>
        /// 部门排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Sort(In_DeptSort sort)
        {
            return (await _dept.UpdateDeptSort(sort.idList, sort.orgId)).ToAjaxResult();
        }

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List(MZ_Dept dept = null)
        {
            if (dept == null)
            {
                dept = new MZ_Dept();
            }

            List<MZ_Dept> depts = await _dept.SelectDeptList(dept);
            return this.Success(depts);
        }


        /// <summary>
        /// 查询部门列表（排除节点）
        /// </summary>
        [HttpGet]
        public async Task<AjaxResult> ExcludeChild(MZ_Dept dept)
        {
            List<MZ_Dept> depts = await _dept.SelectDeptList(dept);

            depts = depts.Where(d =>
            {
                string[] anarr = d.ancestors.Split(",", System.StringSplitOptions.RemoveEmptyEntries);
                return anarr.Contains(dept.dept_id.ToString());
            }).ToList();
            return this.Success(depts);
        }



        /// <summary>
        /// 根据部门编号获取详细信息
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _dept.SelectDeptById(id));
        }


        /// <summary>
        /// 获取部门下拉树列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> TreeSelect(MZ_Dept dept)
        {
            List<MZ_Dept> depts = await _dept.SelectDeptList(dept);
            List<TreeSelect<long>> tlist = MZ_Dept.DeptList2Tree(_dept.BuildDeptTree(depts));
            if (dept.OrgId != null && tlist.Count > 0)
            {
                List<TreeSelect<long>> linktree;
                if (tlist[0].parentId > 0)
                {
                    linktree = tlist;
                }
                else
                {
                    linktree = tlist[0].children;
                }
                List<long> tmpdeplist = linktree.Select(x => x.id).ToList();
                if (tmpdeplist.Count > 0)
                {
                    List<long> initdeptlist = await _dept.ExistUserList(dept.OrgId.Value, tmpdeplist);
                    HashSet<long> hs = new HashSet<long>();
                    foreach (long ii in initdeptlist)
                    {
                        hs.Add(ii);
                    }
                    foreach (TreeSelect<long> ts in linktree)
                    {
                        if (ts.children == null && hs.Contains(ts.id))
                        {
                            ts.children = new List<TreeSelect<long>>();
                        }
                    }
                }
    
            }

            return this.Success(tlist);
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Dept dept)
        {
            return (await _dept.InsertDept(dept)).ToAjaxResult();
        }



        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Dept dept)
        {
            return (await _dept.UpdateDept(dept)).ToAjaxResult();
        }
        /// <summary>
        /// 移动部门
        /// </summary>
        /// <param name="move"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Move(In_DeptMove move)
        {
            return (await _dept.MoveDept(move.ids, move.uids, move.memDeptId, move.parentId)).ToAjaxResult();
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _dept.DeleteDeptById(id)).ToAjaxResult();
        }
    }
}
