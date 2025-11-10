using Common;
using FlowService.Model;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowService.DAL
{
    public class GroupDAL : BaseDbSupport
    {
        public async Task<int> UpdateGroupSort(List<long> list)
        {
            using (DbHelp db = CreateDB())
            {
                string instr = string.Join(",", list);
                string casestr = " case";
                for (int i = 0; i < list.Count; i++)
                {
                    casestr += " when Id =" + list[i] + " then " + i;
                }
                casestr += " end";
                string tsql = "UPDATE mz_flow_group SET Sort = " + casestr + " where Id in (" + instr + ")";

                return (await new SqlBuilder(db).Append(tsql).DoAsync<DoExecSql>()).RowCount;
            }
        }
        public async Task<MZ_FlowGroup> SelecById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_flow_group where Id = ").AppendParam(id).DoAsync<DoQuerySql<MZ_FlowGroup>>()).ToFirst();
            }

        }

        public async Task<List<MZ_FlowGroup>> SelectGroupList(In_GroupList query)
        {
            using (DbHelp db = CreateDB())
            {
                var qcmd = await new SqlBuilder(db).Append("select * from mz_flow_group where OrgId=").AppendParam(query.orgId)
                    .Append(" order by Sort asc").DoAsync<DoQuerySql<MZ_FlowGroup>>();
                return qcmd.ToList();
            }
        }

        public async Task<long> InsertGroup(MZ_FlowGroup g)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(g).DoReturnIdentityAsync();
                return docmd.LastInsertedId;
            }
        }
        public async Task<int> UpdateGroup(MZ_FlowGroup g)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(g).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 分组指定序号自动排序
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task SortIncrease(long orgId, int sort)
        {
            using (DbHelp db = CreateDB())
            {
                var doquery = await new SqlBuilder(db).Append("select Id from mz_flow_group where OrgId=").AppendParam(orgId).Append(" and Sort=").AppendParam(sort).DoAsync<DoQuerySql<long>>();
                if (doquery.Count > 0)
                {
                    await new SqlBuilder(db).Append("update mz_flow_group set Sort=Sort+1 where OrgId=").AppendParam(orgId).Append(" and Sort>=").AppendParam(sort).DoAsync<DoExecSql>();
                }
            }
        }

        public async Task<int> DeleteGroupById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Delete<MZ_FlowGroup>("Id=").AppendParam(id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

    }
}
