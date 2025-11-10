using CardService.Model;
using Common;
using Common.Share;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CardService.DAL
{
    public class CardProDAL : BaseDbSupport
    {
        #region 产品
        public async Task<MZ_Card_Pro> SelectDeletedById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card_pro where del_flag='2' and Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Card_Pro>>();
                return docmd.ToFirst();
            }
        }
        public async Task<MZ_Card_Pro> SelectById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card_pro where del_flag='0' and Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Card_Pro>>();
                return docmd.ToFirst();
            }
        }
        public async Task<PageObject<MZ_Card_Pro>> SelectList(In_Card_Pro query)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_Card_Pro>().Append("select p.* from mz_card_pro p left join mz_dept d on p.DeptId = d.dept_id left join mz_card_procat cc on p.CategoryId=cc.Id where 1=1 ")
                .Then(query.orgId != null, sql =>
                {
                    sql.Append(" and p.OrgId=").AppendParam(query.orgId);
                })
                .Then(query.del_flag != null, sq => sq.Append(" and p.del_flag=").AppendParam(query.del_flag))
                .Then(!string.IsNullOrEmpty(query.filterAncestors), sq => sq.Append(" and instr(").AppendParam(query.filterAncestors).Append(",d.ancestors)=1"))
                .Then(!string.IsNullOrEmpty(query.Key), sq => sq.Append(" AND p.ProName like concat('%',").AppendParam(query.Key).Append(", '%')"))
                .Then(query.CategoryPath != null, sq => sq.Append(" and cc.Path like ").AppendParam(query.CategoryPath + "%"))
                .Then(query.beginTime != null, sq => sq.Append(" and p.create_time >= ").AppendParam(query.beginTime))
                .Then(query.endTime != null, sq => sq.Append(" and p.create_time <= ").AppendParam(query.endTime))
                .GeneratePageObjectAsync(query, "p.update_time desc");
            }
        }
        public async Task<List<MZ_Card_Pro>> SelectListInIds(List<long> ids)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_card_pro where Id in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<MZ_Card_Pro>>();
                return docmd.ToList();
            }
        }

        public async Task<int> Insert(MZ_Card_Pro data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> Update(MZ_Card_Pro data)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Update(data)
                         .DoAsync<DoExecSql>()).RowCount;
            }
        }
        public async Task<int> Delete(long id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Delete<MZ_Card_Pro>("Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        #endregion

        #region 产品分类
        public async Task<int> UpdateCategorySort(List<long> idList, long orgId)
        {
            using (DbHelp db = CreateDB())
            {

                string instr = string.Join(",", idList);
                string casestr = " case";
                for (int i = 0; i < idList.Count; i++)
                {
                    casestr += " when Id =" + idList[i] + " then " + i;
                }
                casestr += " end";
                string tsql = "UPDATE mz_card_procat SET Sort = " + casestr + " where Id in (" + instr + ") and OrgId=" + orgId;

                return (await new SqlBuilder(db).Append(tsql).DoAsync<DoExecSql>()).RowCount;
            }

        }
        public async Task<List<MZ_Card_Category>> SelectCategoryList(In_Card_Category query)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_card_procat where 1=1 ")
                .Then(query.OrgId != null, sq => sq.Append(" and OrgId=").AppendParam(query.OrgId))
                .Then(!string.IsNullOrEmpty(query.Key), sq => sq.Append(" AND CategoryName like concat('%',").AppendParam(query.Key).Append(", '%')"))
                .Then(query.ParentId != null, sq => sq.Append(" and ParentId=").AppendParam(query.ParentId))
                .Append(" order by Sort asc")
                .DoAsync<DoQuerySql<MZ_Card_Category>>()).ToList();
            }
        }

        public async Task<MZ_Card_Category> SelectCategoryById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_card_procat where Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Card_Category>>()).ToFirst();
            }

        }
        public async Task<int> UpdateCategory(MZ_Card_Category data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> InsertCategory(MZ_Card_Category data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<bool> ExistChildren(long id, long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select count(1) from mz_card_procat where OrgId=").AppendParam(orgId).Append(" and ParentId=").AppendParam(id).Append(" limit 1")
                         .DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
            }
        }
        public async Task<int> DeleteCategory(long id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Delete<MZ_Card_Category>("Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }


        #endregion
    }
}
