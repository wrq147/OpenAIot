using AuthService;
using AuthService.Model;
using Common;
using FlowService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowService.DAL
{
    public class OrgDAL : BaseDbSupport
    {
        public async Task<MZ_Dept> SelectTopDeptById(long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_dept where parent_id=0 and OrgId = ").AppendParam(orgId).DoAsync<DoQuerySql<MZ_Dept>>()).ToFirst();
            }

        }
        public async Task<List<long>> SelectAllDeptIn(long[] ids, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                string likesql = string.Empty;
                foreach(long id in ids)
                {
                    likesql = id + ",|";
                }
                likesql.TrimEnd('|');

                SqlBuilder sql = new SqlBuilder(db).Append("select dept_id from mz_dept where OrgId=").AppendParam(orgId).Append(" and ancestors REGEXP ").AppendParam(likesql);
                DoQuerySql<long> execsql = await sql.DoAsync<DoQuerySql<long>>();
                return execsql.ToList();
            }
        }
        public async Task<List<MZ_UserDept>> SelectDeptByUserId(List<long> ids, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                SqlBuilder sql = new SqlBuilder(db).Append("select u.Id,d.* from mz_dept d left join mz_admin_ov u on u.dept_id=d.dept_id where d.OrgId=").AppendParam(orgId).Append(" and u.Id in (").AppendParam(ids).Append(")");
                DoQuerySql<MZ_UserDept> execsql = await sql.DoAsync<DoQuerySql<MZ_UserDept>>();
                return execsql.ToList();
            }
        }
        public async Task<MZ_UserDept> SelectDeptByUid(long uid,long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                SqlBuilder sql = new SqlBuilder(db).Append("select u.Id,d.* from mz_dept d left join mz_admin_ov u on u.dept_id=d.dept_id where d.OrgId=").AppendParam(orgId).Append(" and u.Id=").AppendParam(uid);
                DoQuerySql<MZ_UserDept> execsql = await sql.DoAsync<DoQuerySql<MZ_UserDept>>();
                return execsql.ToFirst();
            }
        }
        public async Task<List<Out_PickerItem>> GetDeptList(In_OrgList query)
        {
            using (DbHelp db = CreateDB())
            {
                long parentId = 0;
                if (query.deptId != null)
                {
                    parentId = (long)query.deptId;
                }
                var sqldm = await new SqlBuilder(db).Append("select 'dept' as type,dept_id as id,dept_name as name from mz_dept where parent_id=").AppendParam(parentId)
                    .Then(query.orgId != null, sq => sq.Append(" AND OrgId = ").AppendParam(query.orgId))
                    .DoAsync<DoQuerySql<Out_PickerItem>>();
                return sqldm.ToList();
            }

        }
        public async Task<MZ_Dept> SelectById(long deptId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_dept where dept_id = ").AppendParam(deptId).DoAsync<DoQuerySql<MZ_Dept>>()).ToFirst();
            }
        }
        public async Task<List<MZ_Dept>> SelectByList(long[] deptIds)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_dept where dept_id in (").AppendParam(deptIds).Append(")").DoAsync<DoQuerySql<MZ_Dept>>()).ToList();
            }
        }

        public async Task<List<Out_PickerItem>> GetUserList(In_OrgList query)
        {
            using (DbHelp db = CreateDB())
            {
                if (query.deptId == null)
                {
                    return new List<Out_PickerItem>();
                }
                var sqldm = await new SqlBuilder(db).Append("select 'user' as type,u.Id as id,u.RealName as name,u.Avatar as avatar,u.Signature as remark from mz_admin_ov u where u.OrgId=").AppendParam(query.orgId).Append(" and  u.del_flag='0' and u.dept_id=").AppendParam(query.deptId)
                    .DoAsync<DoQuerySql<Out_PickerItem>>();
                return sqldm.ToList();
            }

        }
        public async Task<MZ_User_Org_V> SelectUserOrgViewById(long userId, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select o.*,uo.UserId,uo.dept_id,uo.post_name,d.dept_name from mz_user_org uo left join mz_org o on uo.OrgId=o.Id left join mz_dept d on uo.dept_id=d.dept_id where o.del_flag='0' and uo.UserId=").AppendParam(userId).Append(" and uo.OrgId=").AppendParam(orgId).Append(" and uo.IsPrimary=1").DoAsync<DoQuerySql<MZ_User_Org_V>>()).ToFirst();
            }

        }
        public async Task<List<Out_PickerItem>> SearchUserList(string key)
        {
            using (DbHelp db = CreateDB())
            {
                var sqldm = await new SqlBuilder(db).Append("select 'user' as type,Id as id,RealName as name,Avatar as avatar from mz_admin where RealName like concat('%',").AppendParam(key).Append(", '%')")
                    .DoAsync<DoQuerySql<Out_PickerItem>>();
                return sqldm.ToList();
            }

        }

        public async Task<List<MZ_AdminInfo>> SelectUserIn(List<long> ids, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                SqlBuilder sql = new SqlBuilder(db).Append("select u.*,d.dept_name from mz_admin_ov u left join mz_dept d on u.dept_id = d.dept_id where u.OrgId=").AppendParam(orgId).Append(" and u.IsPrimary=1 and u.Id in (").AppendParam(ids).Append(")");
                return (await sql.DoAsync<DoQuerySql<MZ_AdminInfo>>()).ToList();
            }

        }
        public async Task<List<long>> SelectUserByRoles(List<long> roles)
        {
            using (DbHelp db = CreateDB())
            {
                SqlBuilder sql = new SqlBuilder(db).Append("select UserId from mz_user_role ur where ur.RoleID in (").AppendParam(roles).Append(")");
                return (await sql.DoAsync<DoQuerySql<long>>()).ToList();
            }
        }
        public virtual async Task<List<long>> SelectDeptLeaders(long deptId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select UserId from mz_user_org where dept_id=").AppendParam(deptId).Append(" and IsLeader=1").DoAsync<DoQuerySql<long>>()).ToList();
            }
      
        }
    }
}
