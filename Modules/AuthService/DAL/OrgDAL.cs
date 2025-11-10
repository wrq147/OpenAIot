using AuthService.Controller;
using AuthService.Model;
using Common;
using Common.Share;
using JiebaNet.Segmenter;
using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    /// <summary>
    /// 组织数据层
    /// </summary>
    public class OrgDAL : BaseRepository<MZ_Org>
    {
        public virtual async Task<List<Out_MemberTreeItem>> SearchUserList(string key)
        {
            var sqldm = await new SqlBuilder(help).Append("select 'user' as type,Id as id,RealName as name,Avatar as avatar from mz_admin where RealName like concat('%',").AppendParam(key).Append(", '%')")
                .DoAsync<DoQuerySql<Out_MemberTreeItem>>();
            return sqldm.ToList();
        }

        public virtual async Task<List<Out_MemberTreeItem>> GetDeptList(In_MemberTreeList query)
        {
            long parentId = 0;
            if (query.deptId != null)
            {
                parentId = (long)query.deptId;
            }
            var sqldm = await new SqlBuilder(help).Append("select 'dept' as type,dept_id as id,dept_name as name from mz_dept where parent_id=").AppendParam(parentId)
                .Then(query.orgId != null, sq => sq.Append(" AND OrgId = ").AppendParam(query.orgId))
                .DoAsync<DoQuerySql<Out_MemberTreeItem>>();
            return sqldm.ToList();
        }

        public virtual async Task<List<Out_MemberTreeItem>> GetUserList(In_MemberTreeList query)
        {
            if (query.deptId == null)
            {
                return new List<Out_MemberTreeItem>();
            }
            var sqldm = await new SqlBuilder(help).Append("select 'user' as type,u.Id as id,u.RealName as name,u.Avatar as avatar,u.Signature as remark from mz_admin_ov u where u.OrgId=").AppendParam(query.orgId).Append(" and  u.del_flag='0' and u.dept_id=").AppendParam(query.deptId)
                .DoAsync<DoQuerySql<Out_MemberTreeItem>>();
            return sqldm.ToList();
        }
        public virtual async Task<List<MZ_Org>> SearchOrg(string key, bool isSys)
        {
            var keyarr = new JiebaSegmenter().CutForSearch(key);
            string appendStr = isSys ? string.Empty : " and Id<>1";
            var tsql = new SqlBuilder(help).Append("select OrgName,Id from mz_org where del_flag='0' and (").FullSearch("KeyWords", keyarr);
            foreach (var k in keyarr)
            {
                tsql.Append(" or OrgName like ").AppendParam(k + "%");
            }
            return (await tsql.Append(")" + appendStr).DoAsync<DoQuerySql<MZ_Org>>()).ToList();
        }

        /// <summary>
        /// 判断企业名称是否已存在
        /// </summary>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckOrgNameUnique(string orgName)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_org where status='2' and del_flag = '0' and OrgName=").AppendParam(orgName).Append(" limit 1")
                       .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断用户是否加入企业
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckExistOrg(long uid, long orgId)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_user_org where UserId=").AppendParam(uid).Append(" and OrgId=").AppendParam(orgId).Append(" limit 1")
           .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        public virtual async Task<bool> CheckExistDept(long uid, long deptId)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_user_org where UserId=").AppendParam(uid).Append(" and dept_id=").AppendParam(deptId).Append(" limit 1")
           .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断用户是否有管理企业的权限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckManOrg(long uid, long orgId)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_user_role where UserId=").AppendParam(uid).Append(" and RoleID=2 and OrgId=").AppendParam(orgId).Append(" limit 1")
.DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断用户是否有指定企业角色权限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="orgId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckRoleOrg(long uid, long orgId, long roleId)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_user_role where UserId=").AppendParam(uid).Append(" and RoleID=" + 5 + " and OrgId=").AppendParam(orgId).Append(" limit 1")
.DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断用户是否有指定企业的权限
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="orgId"></param>
        /// <param name="perm"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckOrgPerm(long uid, long orgId, string perm)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_role_permission r left join mz_menu m on r.MenuId=m.menu_id left join mz_user_role ur on r.RoleID=ur.RoleID where ur.UserId=")
                .AppendParam(uid).Append(" and ur.OrgId=").AppendParam(orgId).Append(" and m.perms=").AppendParam(perm).Append(" limit 1").DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 修改企业信息
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<int> UpdateOrg(MZ_Org org)
        {
            if (org.OrgName != null)
            {
                //同步更新最顶级部门名称
                MZ_Dept newdept = new MZ_Dept();
                newdept.dept_name = org.OrgName;
                await new SqlBuilder(help).Update<MZ_Dept>(newdept, "OrgId=").AppendParam(org.Id).Append(" and parent_id=0").DoAsync<DoExecSql>();
            }
            return (await new SqlBuilder(help).Update(org).DoAsync<DoExecSql>()).RowCount;
        }

        /// <summary>
        /// 根据ID查询信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<MZ_Org> SelectById(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_org where Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Org>>()).ToFirst();
        }
        /// <summary>
        /// 获取指定用户拥有的企业数量
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public virtual async Task<int> SelectCount(long uid)
        {
            var docmd = await new SqlBuilder(help).Append("select count(1) from mz_user_org where UserId=").AppendParam(uid).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        /// <summary>
        /// 新增企业
        /// </summary>
        /// <param name="org"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<int> InsertOrgTrans(MZ_Org org)
        {
            var docmd = await new SqlBuilder(help).Insert(org).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        [Trans]
        public virtual async Task<int> InsertDeptTrans(MZ_Dept dept)
        {
            var docmd = await new SqlBuilder(help).Insert(dept).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        [Trans]
        public virtual async Task<int> InsertUserOrgTrans(MZ_User_Org userOrg)
        {
            var docmd = await new SqlBuilder(help).Insert(userOrg).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        /// <summary>
        /// 添加用户组织关联
        /// </summary>
        /// <param name="userOrg"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertUserOrg(MZ_User_Org userOrg)
        {
            var docmd = await new SqlBuilder(help).Insert(userOrg).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        public virtual async Task<int> InsertUserOrgList(List<MZ_User_Org> userOrgList)
        {
            var docmd = await new SqlBuilder(help).Insert(userOrgList).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        /// <summary>
        /// 获取用户组织数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<MZ_User_Org_V> SelectUserOrgViewById(long userId, long orgId)
        {
            return (await new SqlBuilder(help).Append("select o.*,uo.UserId,uo.dept_id,uo.post_name,d.dept_name, from mz_user_org uo left join mz_org o on uo.OrgId=o.Id left join mz_dept d on uo.dept_id=d.dept_id where o.del_flag='0' and uo.UserId=").AppendParam(userId).Append(" and uo.OrgId=").AppendParam(orgId).Append(" and uo.IsPrimary=1").DoAsync<DoQuerySql<MZ_User_Org_V>>()).ToFirst();
        }
        public virtual async Task<MZ_User_Org> SelectUserOrg(long userId, long orgId)
        {
            return (await new SqlBuilder(help).Append("select * from mz_user_org where UserId=").AppendParam(userId).Append(" and OrgId=").AppendParam(orgId).Append(" and IsPrimary=1").DoAsync<DoQuerySql<MZ_User_Org>>()).ToFirst();
        }
        public virtual async Task<List<MZ_User_Org>> SelectLeaderUserOrg(long userId, long orgId)
        {
            return (await new SqlBuilder(help).Append("select * from mz_user_org where UserId=").AppendParam(userId).Append(" and OrgId=").AppendParam(orgId).Append(" and IsLeader=1").DoAsync<DoQuerySql<MZ_User_Org>>()).ToList();
        }
        public virtual async Task<MZ_User_Org> SelectUserOrgByDept(long userId, long deptId)
        {
            return (await new SqlBuilder(help).Append("select * from mz_user_org where UserId=").AppendParam(userId).Append(" and dept_id=").AppendParam(deptId).DoAsync<DoQuerySql<MZ_User_Org>>()).ToFirst();
        }

        public virtual async Task<List<Out_UserOrg>> SelectUserOrgListIn(List<long> userIds)
        {
            return await new SqlBuilder(help).Query<Out_UserOrg>().Append("select uo.UserId,o.* from mz_user_org uo left join mz_org o on uo.OrgId=o.Id where o.del_flag='0' and uo.UserId in (").AppendParam(userIds).Append(")").ToListAsync();
        }
        public virtual async Task<MZ_User_Org> SelectUserOrgByNewest(long userId)
        {
            return await new SqlBuilder(help).Query<MZ_User_Org>().Append("select uo.* from mz_user_org uo left join mz_org o on uo.OrgId=o.Id where o.del_flag='0' and uo.UserId=").AppendParam(userId).Take(1).ToFirstAsync();
        }
        public virtual async Task<List<MZ_Org>> SelectUserOrgList(long userId)
        {
            return await new SqlBuilder(help).Query<MZ_Org>().Append("select o.* from mz_user_org uo left join mz_org o on uo.OrgId=o.Id where o.del_flag='0' and uo.UserId=").AppendParam(userId).ToListAsync();
        }
        public virtual async Task<List<MZ_Org>> SelectOrgListByRole(long userId, long roleId)
        {
            return await new SqlBuilder(help).Query<MZ_Org>().Append("select o.* from mz_user_role ur left join mz_org o on ur.OrgId=o.Id where o.del_flag='0' and ur.UserId=").AppendParam(userId).Append(" and ur.RoleID=").AppendParam(roleId).ToListAsync();
        }
        public virtual async Task<List<long>> SelectDeptLeaders(long deptId)
        {
            return (await new SqlBuilder(help).Append("select UserId from mz_user_org where dept_id=").AppendParam(deptId).Append(" and IsLeader=1").DoAsync<DoQuerySql<long>>()).ToList();
        }
        public virtual async Task<List<long>> SelectDeptIds(long userId, long orgId)
        {
            return (await new SqlBuilder(help).Append("select dept_id from mz_user_org where UserId=").AppendParam(userId).Append(" and OrgId=").AppendParam(orgId).DoAsync<DoQuerySql<long>>()).ToList();
        }
        public virtual async Task<List<Out_UserDept>> SelectUserDept(long userId, long orgId)
        {
            return (await new SqlBuilder(help).Append("select d.*,uo.post_name,uo.IsLeader,uo.IsPrimary from mz_user_org uo left join mz_dept d on uo.dept_id=d.dept_id where uo.UserId=").AppendParam(userId).Append(" and uo.OrgId=").AppendParam(orgId).DoAsync<DoQuerySql<Out_UserDept>>()).ToList();
        }
        public virtual async Task ClearLeaders(long deptId)
        {
            MZ_User_Org userOrg = new MZ_User_Org();
            userOrg.IsLeader = false;
            await new SqlBuilder(help).Update(userOrg, "dept_id=").AppendParam(deptId).DoAsync<DoExecSql>();
        }

        public virtual async Task SetLeader(long deptId, long uid, bool isLeader)
        {
            MZ_User_Org userOrg = new MZ_User_Org();
            userOrg.IsLeader = isLeader;
            await new SqlBuilder(help).Update(userOrg, "dept_id=").AppendParam(deptId).Append(" and UserId=").AppendParam(uid).DoAsync<DoExecSql>();
        }


        public virtual async Task SetLeaders(long deptId, long[] uids)
        {
            MZ_User_Org userOrg = new MZ_User_Org();
            userOrg.IsLeader = true;
            await new SqlBuilder(help).Update(userOrg, "dept_id=").AppendParam(deptId).Append(" and UserId in (").AppendParam(uids).Append(")").DoAsync<DoExecSql>();
        }

        public virtual async Task<int> UpdateUserOrg(long uid, long orgId, long memDeptId, long deptId)
        {
            MZ_User_Org uorg = new MZ_User_Org();
            uorg.IsLeader = false;
            uorg.dept_id = deptId;
            var docmd = await new SqlBuilder(help).Update(uorg, "UserId=").AppendParam(uid).Append(" and OrgId=").AppendParam(orgId).Append(" and dept_id=").AppendParam(memDeptId).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        /// <summary>
        /// 修改用户组织关联
        /// </summary>
        /// <param name="userOrg"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateUserOrg(MZ_User_Org userOrg)
        {
            var docmd = await new SqlBuilder(help).Update(userOrg, x => x.UserId == userOrg.UserId && x.OrgId == userOrg.OrgId && x.IsPrimary == true).DoAsync<DoExecSql>();
            return docmd.RowCount;
        }
        /// <summary>
        /// 删除用户组织关联
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteUserOrg(long userId, long orgId)
        {
            return await new SqlBuilder(help).Delete<MZ_User_Org>("OrgId=").AppendParam(orgId).Append(" and UserId=").AppendParam(userId).DoAsync();
        }
        public virtual async Task<int> DeleteUserOrgByDept(long userId, long deptId)
        {
            return await new SqlBuilder(help).Delete<MZ_User_Org>("dept_id=").AppendParam(deptId).Append(" and UserId=").AppendParam(userId).Append(" and IsPrimary=0").DoAsync();
        }
        public virtual async Task<int> DeleteAllOtherUserOrgByOrg(long userId, long orgId)
        {
            return await new SqlBuilder(help).Delete<MZ_User_Org>("OrgId=").AppendParam(orgId).Append(" and UserId=").AppendParam(userId).Append(" and IsPrimary=0").DoAsync();
        }
        /// <summary>
        /// 删除指定组织的所有用户关联
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteUserOrg(long orgId)
        {
            return await new SqlBuilder(help).Delete<MZ_User_Org>("OrgId=").AppendParam(orgId).DoAsync();
        }
        /// <summary>
        /// 标记删除企业
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task DeleteTrans(long id)
        {
            //删除用户关联
            await new SqlBuilder(help).Delete<MZ_User_Org>("OrgId=").AppendParam(id).DoAsync<DoExecSql>();
            //用户的OrgId设置0
            await new SqlBuilder(help).Append("update mz_admin set OrgId=0 where OrgId=").AppendParam(id).DoAsync<DoExecSql>();
            //删除企业
            await new SqlBuilder(help).Append("update mz_org set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>();
        }
    }
}
