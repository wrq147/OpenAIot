using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Share;
using EfficiencyService.Model;
using MyAccess.DB;

namespace EfficiencyService.DAL
{
    public class PolicyDAL : BaseDbSupport
    {
        /// <summary>
        /// 新增电价政策
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddPolicy(T_Price_Policy data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 更新电价政策
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdatePolicy(T_Price_Policy data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除电价政策
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeletePolicy(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_price_policy set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 分页查询电价政策
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_Policy>> SelectPolicy(In_PolicyList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_Policy>().Append("select a.*,b.TypeName from t_price_policy a left join t_eng_factortype b on a.EnergyType = b.Id where a.del_flag='0' ")
                                    .Then(query.Id != null, sql =>
                                     {
                                         sql.Append(" AND a.Id = ").AppendParam(query.Id);
                                    })
                                    .Then(query.OrgId != null, sql =>
                                    {
                                        sql.Append(" AND a.OrgId = ").AppendParam(query.OrgId);
                                    })
                                    .Then(query.EnergyType != null, sql =>
                                    {
                                        sql.Append(" AND a.EnergyType = ").AppendParam(query.EnergyType);
                                    })
                                    .Then(query.Unit != null, sql =>
                                    {
                                        sql.Append(" AND a.Unit = ").AppendParam(query.Unit);
                                    })
                                    .Then(!string.IsNullOrEmpty(query.PolicyName), sq =>
                                    {
                                        string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.PolicyName) + "%";
                                        sq.Append(" and a.PolicyName like ").AppendParam(tmpkey);
                                    }).Append(" order by a.Id asc");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        /// <summary>
        /// 查询电价政策
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<Out_Policy> SelectPolicy(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<Out_Policy>().Append("select a.*,b.TypeName from t_price_policy a left join t_eng_factortype b on a.EnergyType = b.Id where a.Id= ").AppendParam(Id).DoAsync<DoQuerySql<Out_Policy>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 新增电价政策明细
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddPolicyDetil(T_Price_PolicyDetil data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 更新电价政策明细
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdatePolicyDetil(T_Price_PolicyDetil data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除电价政策明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeletePolicyDetil(string PolicyId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_price_policydetil set del_flag='2' where PolicyId=").AppendParam(PolicyId).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询当前电价政策
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<T_Price_PolicyDetil>> SelectPolicyDetil(string PolicyId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<T_Price_Policy>().Append("select * from t_price_policydetil where del_flag='0' and PolicyId= ").AppendParam(PolicyId).DoAsync<DoQuerySql<T_Price_PolicyDetil>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 新增尖峰平谷价格
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddPeriod(T_Price_Period data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 更新尖峰平谷价格
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdatePeriod(T_Price_Period data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除尖峰平谷价格
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeletePeriod(string DetilId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_price_period set del_flag='2' where DetilId=").AppendParam(DetilId).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询尖峰平谷价格
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<T_Price_Period>> SelectPeriod(string DetilId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<T_Price_Period>().Append("select * from t_price_period where DetilId=").AppendParam(DetilId)
                                    .Append(" order by Id asc").DoAsync<DoQuerySql<T_Price_Period>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 新增尖峰平谷时段
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddTime(T_Price_Time data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 更新尖峰平谷时段
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateTime(T_Price_Time data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除尖峰平谷时段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteTime(string DetilId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_price_time set del_flag='2' where DetilId=").AppendParam(DetilId).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询尖峰平谷时段
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<T_Price_Time>> SelectTime(string DetilId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<T_Price_Time>().Append("select * from t_price_time where DetilId=").AppendParam(DetilId)
                                    .Append(" order by Id asc").DoAsync<DoQuerySql<T_Price_Time>>();

                return docmd.ToList();
            }
        }


        /// <summary>
        /// 新增阶梯电价
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddTier(T_Price_Tier data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 更新阶梯电价
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateTier(T_Price_Tier data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除阶梯电价
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteTier(string DetilId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_price_tier set del_flag='2' where DetilId=").AppendParam(DetilId).DoAsync<DoExecSql>()).RowCount;
            }
        }


        /// <summary>
        /// 查询阶梯电价
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<List<T_Price_Tier>> SelectTier(string DetilId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<T_Price_Tier>().Append("select * from t_price_tier where DetilId=").AppendParam(DetilId)
                                    .Append(" order by Id asc").DoAsync<DoQuerySql<T_Price_Tier>>();

                return docmd.ToList();
            }
        }
    }
}
