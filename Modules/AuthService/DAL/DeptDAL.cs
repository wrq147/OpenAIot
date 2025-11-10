using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Share;
using AuthService.Controller;

namespace AuthService
{
    /// <summary>
    /// 部门数据层
    /// </summary>
    public class DeptDAL : BaseRepository<MZ_Dept>
    {
        /// <summary>
        /// 查询部门管理数据
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public List<MZ_Dept> SelectDeptList(MZ_Dept dept)
        {
            using (DbHelp db = CreateDB())
            {
                return new SqlBuilder(db).Append("select d.* from mz_dept d where d.del_flag='0' ")
                    .Then(dept.dept_id != null, sql =>
                    {
                        sql.Append(" AND d.dept_id = ").AppendParam(dept.dept_id);
                    })
                    .Then(dept.parent_id != null, sql =>
                    {
                        sql.Append(" AND d.parent_id = ").AppendParam(dept.parent_id);
                    })
                    .Then(!string.IsNullOrEmpty(dept.dept_name), sql =>
                    {
                        sql.Append(" AND d.dept_name like concat('%', ").AppendParam(dept.dept_name).Append(", '%')");
                    })
                    .Then(!string.IsNullOrEmpty(dept.status), sql =>
                    {
                        sql.Append(" AND status = ").AppendParam(dept.status);
                    })
                    .Then(dept.OrgId != null, sql =>
                    {
                        sql.Append(" AND d.OrgId = ").AppendParam(dept.OrgId);
                    })
                    .Append(" order by d.parent_id, d.order_num").Do<DoQuerySql<MZ_Dept>>().ToList();
            }

        }



        /// <summary>
        /// 根据部门ID查询信息
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<MZ_Dept> SelectById(long deptId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_dept where dept_id = ").AppendParam(deptId).DoAsync<DoQuerySql<MZ_Dept>>()).ToFirst();
            }

        }

        /// <summary>
        /// 获取存在用户的部门列表
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<long>> ExistUserList(long orgId, List<long> ids)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select distinct dept_id from mz_user_org where OrgId=").AppendParam(orgId).Append(" and dept_id in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<long>>()).ToList();
            }
        }
        /// <summary>
        /// 根据ID查询所有子部门
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<List<MZ_Dept>> SelectChildrenById(long deptId)
        {
            var depinfo = await SelectById(deptId);
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("SELECT * FROM mz_dept where del_flag = '0' and ancestors like concat(").AppendParam(depinfo.ancestors).Append(",'%') and dept_id<>" + deptId)
                    .DoAsync<DoQuerySql<MZ_Dept>>()).ToList();
            }
        }
        public async Task<List<MZ_Dept>> SelectChildrenByRoot(MZ_Dept depinfo)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("SELECT * FROM mz_dept where del_flag = '0' and ancestors like concat(").AppendParam(depinfo.ancestors).Append(",'%') and dept_id<>" + depinfo.dept_id)
                    .DoAsync<DoQuerySql<MZ_Dept>>()).ToList();
            }
        }
        public async Task<MZ_Dept> SelectRoot(long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_Dept>().Append("SELECT * FROM mz_dept where OrgId=").AppendParam(orgId).Append(" and parent_id=0").ToFirstAsync();
            }
        }

        /// <summary>
        /// 根据ID查询所有子部门数量（正常状态）
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<int> SelectNormalChildrenById(long deptId)
        {
            var depinfo = await SelectById(deptId);
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("SELECT count(*) FROM mz_dept where status = 0 and del_flag = '0' and dept_id<>" + deptId + " and ancestors like concat(").AppendParam(depinfo.ancestors).Append(",'%');")
                    .DoAsync<DoQueryScalar>()).GetValueInt(0);
            }

        }
        /// <summary>
        /// 搜索指定名称的部门
        /// </summary>
        /// <param name="name"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public async Task<List<MZ_Dept>> SearchDeptByName(string name, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_dept where OrgId=").AppendParam(orgId).Append(" and dept_name like ").AppendParam("%" + name + "%")
        .DoAsync<DoQuerySql<MZ_Dept>>()).ToList();
            }
        }
        /// <summary>
        /// 是否存在子节点
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<bool> hasChildByDeptId(long deptId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select count(1) from mz_dept where del_flag = '0' and parent_id = ").AppendParam(deptId).Append(" limit 1")
                 .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
            }

        }


        /// <summary>
        /// 查询部门是否存在用户
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<bool> CheckDeptExistUser(long deptId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select count(1) from mz_admin_v where dept_id = ").AppendParam(deptId).Append(" and del_flag = '0'").Append(" limit 1")
                         .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
            }

        }


        /// <summary>
        /// 校验部门名称是否唯一
        /// </summary>
        /// <param name="deptName"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public bool CheckDeptNameUnique(string deptName, long editDeptId, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return new SqlBuilder(db).Append("select count(1) from mz_dept where del_flag = '0' and dept_name=").AppendParam(deptName).Append(" and OrgId = ").AppendParam(orgId)
                    .Then(editDeptId > 0, x => x.Append(" and dept_id<>").AppendParam(editDeptId))
                    .Append(" limit 1")
                       .Do<DoQueryScalar>().GetValueInt(0) > 0;
            }
        }
        /// <summary>
        /// 校验部门是否与组织一致
        /// </summary>
        /// <param name="deptId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public bool CheckDeptId(long deptId, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return new SqlBuilder(db).Append("select count(1) from mz_dept where OrgId=").AppendParam(orgId).Append(" and dept_id=").AppendParam(deptId).Append(" limit 1")
        .Do<DoQueryScalar>().GetValueInt(0) > 0;
            }

        }

        /// <summary>
        /// 新增部门信息
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public async Task<int> InsertDept(MZ_Dept dept)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Insert(dept).DoAsync<DoExecSql>()).RowCount;
            }

        }


        /// <summary>
        /// 修改部门信息
        /// </summary>
        /// <param name="dept"></param>
        /// <returns></returns>
        public async Task<int> UpdateDept(MZ_Dept dept)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Update(dept).DoAsync<DoExecSql>()).RowCount;
            }

        }


        /// <summary>
        /// 修改所在部门正常状态
        /// </summary>
        /// <param name="deptIds"></param>
        public async Task<int> UpdateDeptStatusNormal(long[] deptIds)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update mz_dept set status = '0' where dept_id in (").AppendParam(deptIds).Append(")").DoAsync<DoExecSql>()).RowCount;
            }

        }


        /// <summary>
        /// 修改子元素关系
        /// </summary>
        /// <param name="depts"></param>
        /// <returns></returns>
        public async Task<int> UpdateDeptChildren(List<MZ_Dept> depts)
        {
            using (DbHelp db = CreateDB())
            {

                SqlBuilder sql = new SqlBuilder(db);
                string whenlist = string.Empty;
                string whenin = string.Empty;
                foreach (MZ_Dept dept in depts)
                {
                    whenlist = whenlist + " when " + dept.dept_id + " then '" + dept.ancestors + "'";
                    whenin += "," + dept.dept_id;
                }
                whenin = whenin.Substring(1);
                sql.Append("update mz_dept set ancestors=case dept_id ").Append(whenlist).Append(" end").Append(" where dept_id in (").AppendParam(whenin).Append(")");
                return (await sql.DoAsync<DoExecSql>()).RowCount;
            }

        }

        public async Task<int> UpdateDeptSort(List<long> idList, long orgId)
        {
            using (DbHelp db = CreateDB())
            {

                string instr = string.Join(",", idList);
                string casestr = " case";
                for (int i = 0; i < idList.Count; i++)
                {
                    casestr += " when dept_id =" + idList[i] + " then " + i;
                }
                casestr += " end";
                string tsql = "UPDATE mz_dept SET order_num = " + casestr + " where dept_id in (" + instr + ") and OrgId=" + orgId;

                return (await new SqlBuilder(db).Append(tsql).DoAsync<DoExecSql>()).RowCount;
            }

        }

        /// <summary>
        /// 删除部门管理信息
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        public async Task<int> DeleteDeptById(long deptId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update mz_dept set del_flag = '2' where dept_id = ").AppendParam(deptId).DoAsync<DoExecSql>()).RowCount;
            }

        }

    }
}
