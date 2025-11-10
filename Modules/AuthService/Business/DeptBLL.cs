using AuthService.Controller;
using AuthService.Model;
using Common;
using Common.IdGenerator;
using Common.Share;
using MyAccess.Aop;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    public class DeptBLL
    {
        protected SnowflakeHelper _snowflake;
        protected DeptDAL _dept;
        protected ITAServiceProvider _provider;
        protected OrgDAL _org;
        protected ITAContext _context;
        public DeptBLL(DeptDAL dept, SnowflakeHelper snowflake, OrgDAL org, ITAContext context, ITAServiceProvider provider)
        {
            _dept = dept;
            _snowflake = snowflake;
            _provider = provider;
            _org = org;
            _context = context;
        }

        public virtual async Task<BusResponse<int>> UpdateDeptSort(List<long> idList, long orgId)
        {
            try
            {
                IUserInfo user = Data_ServerTokenInfo.From(_context);
                //判断是否有当前组织的部门编辑权限
                if (!await _org.CheckOrgPerm(user.UserId, orgId, "/AuthService/Dept/Edit"))
                {
                    return BusResponse<int>.Error(116, "没有组织的部门编辑权限");
                }
                return BusResponse<int>.Success(await _dept.UpdateDeptSort(idList, orgId));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }

        /// <summary>
        /// 查询部门列表
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_Dept>> SelectDeptList(MZ_Dept dept)
        {
            Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
            if (dept.OrgId == null)
            {
                dept.OrgId = user.OrgId;
            }
            else
            {
                if (!await _org.CheckExistOrg(user.UserId, dept.OrgId.Value))
                {
                    return new List<MZ_Dept>();
                }
            }

            return _dept.SelectDeptList(dept);
        }

        /// <summary>
        /// 查询部门列表
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public virtual async Task<List<MZ_Dept>> SelectDeptListForSync(MZ_Dept dept)
        {
            //Data_ServerTokenInfo user = Data_ServerTokenInfo.From(_context);
            //if (dept.OrgId == null)
            //{
            //    dept.OrgId = user.OrgId;
            //}
            //else
            //{
            //    if (!await _org.CheckExistOrg(user.UserId, dept.OrgId.Value))
            //    {
            //        return new List<MZ_Dept>();
            //    }
            //}

            return _dept.SelectDeptList(dept);
        }

        public virtual async Task<List<long>> ExistUserList(long orgId, List<long> ids)
        {
            return await _dept.ExistUserList(orgId, ids);
        }
        /// <summary>
        /// 构建前端所需要树结构
        /// </summary>
        /// <param name="depts"></param>
        /// <returns></returns>
        public virtual List<MZ_Dept> BuildDeptTree(List<MZ_Dept> depts)
        {
            List<MZ_Dept> returnList = new List<MZ_Dept>();
            List<long> tempList = new List<long>();
            foreach (MZ_Dept dept in depts)
            {
                tempList.Add(dept.dept_id.Value);
            }
            foreach (MZ_Dept dept in depts)
            {
                // 如果是顶级节点, 遍历该父节点的所有子节点
                if (!tempList.Contains(dept.parent_id.Value))
                {
                    _RecursionFn(depts, dept);
                    returnList.Add(dept);
                }
            }

            if (returnList.Count == 0)
            {
                returnList = depts;
            }
            return returnList;
        }


        /// <summary>
        /// 递归列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        private void _RecursionFn(List<MZ_Dept> list, MZ_Dept t)
        {
            // 得到子节点列表
            List<MZ_Dept> childList = _GetChildList(list, t);
            t.children = childList;
            foreach (MZ_Dept tChild in childList)
            {
                if (_GetChildList(list, tChild).Count > 0)
                {
                    _RecursionFn(list, tChild);
                }
            }
        }

        /// <summary>
        /// 得到子节点列表
        /// </summary>
        /// <param name="list"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private List<MZ_Dept> _GetChildList(List<MZ_Dept> list, MZ_Dept t)
        {
            List<MZ_Dept> tlist = new List<MZ_Dept>();
            foreach (MZ_Dept n in list)
            {
                if (n.parent_id == t.dept_id)
                {
                    tlist.Add(n);
                }
            }
            return tlist;
        }




        /// <summary>
        /// 根据ID查询所有子部门（正常状态）
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public virtual async Task<int> SelectNormalChildrenDeptById(long deptId)
        {
            return await _dept.SelectNormalChildrenById(deptId);
        }

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HasChildByDeptId(long deptId)
        {
            return await _dept.hasChildByDeptId(deptId);
        }

        /// <summary>
        /// 查询部门是否存在用户
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckDeptExistUser(long deptId)
        {
            return await _dept.CheckDeptExistUser(deptId);
        }





        /// <summary>
        /// 根据部门ID查询信息
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public virtual async Task<MZ_Dept> SelectDeptById(long deptId)
        {
            return await _dept.SelectById(deptId);
        }
        /// <summary>
        /// 获取指定部门的负责人Id数组
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public virtual async Task<List<long>> SelectDeptLeaders(long deptId)
        {
            return await _org.SelectDeptLeaders(deptId);
        }
        /// <summary>
        /// 批量设置部门负责人
        /// </summary>
        /// <param name="leaders"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<BusResponse<string>> SetDeptLeaders(In_SetDeptLeaders leaders)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Dept oldDept = await _dept.SelectById(leaders.depId);
            if (!await _org.CheckOrgPerm(user.UserId, oldDept.OrgId.Value, "/AuthService/Dept/Edit"))
            {
                return BusResponse<string>.Error(102, "没有部门的修改权限");
            }
            await _org.ClearLeaders(leaders.depId);
            await _org.SetLeaders(leaders.depId, leaders.Leaders);
            return BusResponse<string>.Success();
        }

        /// <summary>
        /// 单个设置部门负责人
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="leaderId"></param>
        /// <param name="isLeader"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<string>> SetDeptLeader(long deptId, long leaderId, bool isLeader)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Dept oldDept = await _dept.SelectById(deptId);
            if (!await _org.CheckOrgPerm(user.UserId, oldDept.OrgId.Value, "/AuthService/Dept/Edit"))
            {
                return BusResponse<string>.Error(102, "没有部门的修改权限");
            }
            await _org.SetLeader(deptId, leaderId, isLeader);
            return BusResponse<string>.Success();
        }
        /// <summary>
        /// 新增保存部门信息
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<long>> InsertDept(MZ_Dept dept)
        {
            if (_dept.CheckDeptNameUnique(dept.dept_name, 0, dept.parent_id.Value))
            {
                return BusResponse<long>.Error(122, "新增部门'" + dept.dept_name + "'失败，部门名称已存在");
            }
            if (dept.parent_id <= 0)
            {
                return BusResponse<long>.Error(123, "父部门不能为空");
            }
            var user = Data_ServerTokenInfo.From(_context);
            dept.SetCreateBy(Data_ServerTokenInfo.From(_context));
            dept.dept_id = _snowflake.NextId();
            dept.parent_id ??= 0;
            dept.status = "0";
            dept.del_flag = "0";
            dept.email ??= string.Empty;
            dept.phone ??= string.Empty;
            MZ_Dept info = await _dept.SelectById(dept.parent_id.Value);
            dept.OrgId = info.OrgId;
            if (!await _org.CheckOrgPerm(user.UserId, dept.OrgId.Value, "/AuthService/Dept/Add"))
            {
                return BusResponse<long>.Error(102, "没有部门的添加权限");
            }
            // 如果父节点不为正常状态,则不允许新增子节点
            if (info.status != "0")
            {
                return BusResponse<long>.Error(121, "部门停用，不允许新增");
            }
            dept.ancestors = info.ancestors + dept.dept_id + ",";


            await _dept.InsertDept(dept);

            return BusResponse<long>.Success(dept.dept_id.Value);
        }

        /// <summary>
        /// 修改保存部门信息
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateDept(MZ_Dept dept)
        {
            var user = Data_ServerTokenInfo.From(_context);

            if (_dept.CheckDeptNameUnique(dept.dept_name, dept.dept_id.Value, dept.parent_id.Value))
            {
                return BusResponse<int>.Error(122, "修改部门'" + dept.dept_name + "'失败，部门名称已存在");
            }
            else if (dept.parent_id == dept.dept_id)
            {
                return BusResponse<int>.Error(123, "修改部门'" + dept.dept_name + "'失败，上级部门不能是自己");
            }
            else if (dept.status == "1" && (await _dept.SelectNormalChildrenById(dept.dept_id.Value)) > 0)
            {
                return BusResponse<int>.Error(124, "停用失败，包含未停用的子部门！");
            }

            MZ_Dept oldDept = await _dept.SelectById(dept.dept_id.Value);
            if (!await _org.CheckOrgPerm(user.UserId, oldDept.OrgId.Value, "/AuthService/Dept/Edit"))
            {
                return BusResponse<int>.Error(102, "没有部门的修改权限");
            }
            if (dept.parent_id != null && oldDept.parent_id != dept.parent_id.Value)
            {
                MZ_Dept newParentDept = await _dept.SelectById(dept.parent_id.Value);
                string newAncestors = newParentDept.ancestors + dept.dept_id + ",";
                string oldAncestors = oldDept.ancestors;
                dept.ancestors = newAncestors;
                await _UpdateDeptChildren(dept.dept_id.Value, newAncestors, oldAncestors);
            }
            dept.SetUpdateBy(user);
            int result = await _dept.UpdateDept(dept);
            if (dept.status == "0" && dept.parent_id != null && dept.parent_id != 0)
            {
                // 如果该部门是启用状态，则启用该部门的所有上级部门
                await _UpdateParentDeptStatusNormal(dept);
            }
            return BusResponse<int>.Success(result);
        }


        /// <summary>
        /// 批量移动
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="uids"></param>
        /// <param name="memDeptId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<BusResponse<int>> MoveDept(List<long> ids, List<long> uids, long memDeptId, long parentId)
        {
            try
            {
                var user = Data_ServerTokenInfo.From(_context);
                int c = 0;
                MZ_Dept newParentDept = await _dept.SelectById(parentId);
                if (ids != null)
                {
                    //移入部门
                    foreach (long id in ids)
                    {
                        if (id == parentId)
                        {
                            continue;
                        }

                        MZ_Dept oldDept = await _dept.SelectById(id);
                        if (!await _org.CheckOrgPerm(user.UserId, oldDept.OrgId.Value, "/AuthService/Dept/Edit"))
                        {
                            return BusResponse<int>.Error(102, "没有部门的修改权限");
                        }
                        string newAncestors = newParentDept.ancestors + id + ",";
                        string oldAncestors = oldDept.ancestors;
                        MZ_Dept dept = new MZ_Dept();
                        dept.dept_id = id;
                        dept.parent_id = parentId;
                        dept.ancestors = newAncestors;
                        await _UpdateDeptChildren(dept.dept_id.Value, newAncestors, oldAncestors);
                        dept.SetUpdateBy(Data_ServerTokenInfo.From(_context));
                        await _dept.UpdateDept(dept);
                        c++;
                    }
                }

                if (uids != null)
                {
                    //移入成员
                    foreach (long id in uids)
                    {
                        if (await _org.CheckExistDept(id, parentId))
                        {
                            return BusResponse<int>.Error(114, "目标部门已存在分身");
                        }
                        int ex = await _org.UpdateUserOrg(id, newParentDept.OrgId.Value, memDeptId, parentId);
                        if (ex <= 0)
                        {
                            return BusResponse<int>.Error(108, "有不存在的成员");
                        }
                        c++;
                    }
                }

                return BusResponse<int>.Success(c);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(111, ex.Message);
            }


        }

        /// <summary>
        /// 修改该部门与父级部门状态
        /// </summary>
        /// <param name="dept"></param>
        private async Task _UpdateParentDeptStatusNormal(MZ_Dept dept)
        {
            string[] ancestors = dept.ancestors.Split(",", StringSplitOptions.RemoveEmptyEntries);
            long[] deptIds = Array.ConvertAll(ancestors, long.Parse);
            await _dept.UpdateDeptStatusNormal(deptIds);
        }
        /// <summary>
        /// 修改子元素关系
        /// </summary>
        /// <param name="deptId">被修改的部门ID</param>
        /// <param name="newAncestors">新的父ID集合</param>
        /// <param name="oldAncestors">旧的父ID集合</param>
        private async Task _UpdateDeptChildren(long deptId, string newAncestors, string oldAncestors)
        {
            List<MZ_Dept> children = await _dept.SelectChildrenById(deptId);
            foreach (MZ_Dept child in children)
            {
                child.ancestors = newAncestors + child.ancestors.Substring(oldAncestors.Length);
            }
            if (children.Count > 0)
            {
                await _dept.UpdateDeptChildren(children);
            }
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> DeleteDeptById(long deptId)
        {
            var user = Data_ServerTokenInfo.From(_context);
            MZ_Dept oldDept = await _dept.SelectById(deptId);
            if (!await _org.CheckOrgPerm(user.UserId, oldDept.OrgId.Value, "/AuthService/Dept/Remove"))
            {
                return BusResponse<int>.Error(102, "没有部门的删除权限");
            }
            if (await _dept.hasChildByDeptId(deptId))
            {
                return BusResponse<int>.Error(121, "存在下级部门,不允许删除");
            }
            if (await _dept.CheckDeptExistUser(deptId))
            {
                return BusResponse<int>.Error(122, "部门存在用户,不允许删除");
            }
            try
            {
                await _dept.DeleteDeptById(deptId);
                return BusResponse<int>.Success();
            }
            catch
            {
                return BusResponse<int>.ErrorBusy();
            }

        }
    }
}
