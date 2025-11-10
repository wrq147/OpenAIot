using System.Collections.Generic;
using System.Linq;
using TemplateAction.Core;
using Common;
using Common.Share;
using TemplateAction.Route;
using System.Threading.Tasks;
using AuthService.Model;

namespace AuthService.Controller
{
    /// <summary>
    /// 部门管理API
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
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ExcludeChild(long id)
        {
            if (id <= 0)
            {
                return this.Success(new List<MZ_Dept>());
            }
            MZ_Dept dept = await _dept.SelectDeptById(id);
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(Context);
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
        /// <param name="id"></param>
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
        public async Task<AjaxResult> TreeSelect(MZ_Dept data = null)
        {
            if (data == null)
            {
                data = new MZ_Dept();
            }
            List<MZ_Dept> depts = await _dept.SelectDeptList(data);
            List<TreeSelect<long>> tlist = MZ_Dept.DeptList2Tree(_dept.BuildDeptTree(depts));

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
        /// 批量设置部门负责人
        /// </summary>
        /// <param name="leaders"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> SetLeaders(In_SetDeptLeaders leaders)
        {
            return (await _dept.SetDeptLeaders(leaders)).ToAjaxResult();
        }

        /// <summary>
        /// 单个设置部门负责人
        /// </summary>
        /// <param name="deptId">部门Id</param>
        /// <param name="leaderId">负责人</param>
        /// <param name="isLeader">true为负责人，false取消</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> SetLeader(long deptId, long leaderId, bool isLeader)
        {
            return (await _dept.SetDeptLeader(deptId, leaderId, isLeader)).ToAjaxResult();
        }
        /// <summary>
        /// 获取指定部门的负责人Id数组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<long>>> GetLeaders(long id)
        {
            return this.Success(await _dept.SelectDeptLeaders(id));
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
