using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Share;
using EfficiencyService.Model;
using MyAccess.DB;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace EfficiencyService.DAL
{
    public class FactorDAL : BaseDbSupport
    {
        #region 排放因子
        /// <summary>
        /// 新增排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddFactor(T_ENG_Factor data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateFactor(T_ENG_Factor data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除排放因子
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteFactor(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_eng_factor set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询排放因子
        /// </summary>
        /// <param name="year"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        //public async Task<PageObject<T_ENG_Factor>> SelectFactorPage(In_Factor query)
        //{
        //    using (DbHelp db = CreateDB())
        //    {
        //        var tsql = new SqlBuilder(db).Query<T_ENG_Factor>().Append("select * from t_eng_factor where del_flag = 0 ")
        //                               .Then(!string.IsNullOrEmpty(query.Year), sql =>
        //                                {
        //                                    sql.Append(" and Year = ").AppendParam(query.Year);
        //                                })
        //                               .Then(!string.IsNullOrEmpty(query.TypeId), sql =>
        //                               {
        //                                   sql.Append(" and TypeId = ").AppendParam(query.TypeId);
        //                               })
        //                               .Then(!string.IsNullOrEmpty(query.Id), sql =>
        //                               {
        //                                   sql.Append(" and Id = ").AppendParam(query.Id);
        //                               });

        //        return await tsql.GeneratePageObjectAsync(query);
        //    }
        //}

        /// <summary>
        /// 查询排放因子
        /// </summary>
        /// <param name="year"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<T_ENG_Factor>> SelectFactorList(In_Factor query)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<T_ENG_Factor>().Append("select * from t_eng_factor where del_flag = 0 ")
                                       .Then(!string.IsNullOrEmpty(query.Year), sql =>
                                       {
                                           sql.Append(" and Year = ").AppendParam(query.Year);
                                       })
                                       .Then(!string.IsNullOrEmpty(query.Version), sql =>
                                        {
                                            sql.Append(" and Version = ").AppendParam(query.Version);
                                        })
                                       .Then(!string.IsNullOrEmpty(query.TypeId), sql =>
                                       {
                                           sql.Append(" and TypeId = ").AppendParam(query.TypeId);
                                       })
                                       .Then(!string.IsNullOrEmpty(query.Id), sql =>
                                       {
                                           sql.Append(" and Id = ").AppendParam(query.Id);
                                       }).DoAsync<DoQuerySql<T_ENG_Factor>>(); ;

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询排放因子
        /// </summary>
        /// <param name="year"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<T_ENG_Factor>> SelectFactorNewList()
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<T_ENG_Factor>().Append(@"SELECT *
                                        FROM (
                                            SELECT *,
                                                   ROW_NUMBER() OVER (PARTITION BY FactorName, Year ORDER BY Version DESC) AS rn 
                                            FROM t_eng_factor where del_flag = 0 ) t
                                        WHERE rn = 1 ")
                                      .DoAsync<DoQuerySql<T_ENG_Factor>>(); ;

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询排放因子
        /// </summary>
        /// <returns></returns>
        public async Task<T_ENG_Factor> SelectFactor(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<T_ENG_Factor>().Append("select * from t_eng_factor where Id = ").AppendParam(Id)
                                        .DoAsync<DoQuerySql<T_ENG_Factor>>(); ;

                return docmd.ToFirst();
            }
        }
        #endregion

        #region 排放因子类型
        /// <summary>
        /// 新增排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddFactorType(T_ENG_FactorType data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新排放因子类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateFactorType(T_ENG_FactorType data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除排放因子类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteFactorType(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_eng_factortype set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询排放因子
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<T_ENG_FactorType>> SelectFactorType(string Id = "")
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_eng_factortype where del_flag = 0 ")
                                        .Then(!string.IsNullOrEmpty(Id), sql =>
                                        {
                                            sql.Append(" and Id = ").AppendParam(Id);
                                        })
                                       .DoAsync<DoQuerySql<T_ENG_FactorType>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询排放因子
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<T_ENG_FactorType> SelectFactorTypeInfo(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_eng_factortype where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_ENG_FactorType>>();

                return docmd.ToFirst();
            }
        }
        #endregion

        #region 排放年份

        /// <summary>
        /// 查询发布年份
        /// </summary>
        /// <param name="year"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<Out_FactorYear>> SelectFactorYear()
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_eng_factoryear where del_flag = 0 order by Year desc")
                                       .DoAsync<DoQuerySql<Out_FactorYear>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询发布年份版本
        /// </summary>
        /// <param name="year"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<Out_FactorYearVersion>> SelectFactorYearVersion(string YearId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_eng_factoryearversion where del_flag = 0 and YearId =")
                                        .AppendParam(YearId)
                                        .Append(" order by Version desc")
                                       .DoAsync<DoQuerySql<Out_FactorYearVersion>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 删除排放年份
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteFactorYear(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_eng_factoryear set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 删除排放年份版本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteFactorYearVersion(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_eng_factoryearversion set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 更新排放年份
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateFactorYear(T_ENG_FactorYear data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 更新排放年份版本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateFactorYearVersion(T_ENG_FactorYearVersion data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 新增年份
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddFactorYear(T_ENG_FactorYear data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 新增年份版本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddFactorYearVersion(T_ENG_FactorYearVersion data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        #endregion

        #region 企业排放因子

        /// <summary>
        /// 新增企业排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddFactorOrg(T_ENG_FactorOrg data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除企业排放因子
        /// </summary>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public async Task<int> DeleteFactorOrg(long? OrgId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_eng_factororg where OrgId=").AppendParam(OrgId).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询企业排放因子
        /// </summary>
        /// <param name="year"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<Out_Factor>> SelectFactororg(string OrgId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select f.*,a.TypeName,a.FactorUnit,a.ActivityUnit from t_eng_factororg fy left join t_eng_factor f on fy.FactorId= f.Id left join t_eng_factortype a on f.TypeId = a.Id where fy.OrgId=")
                                       .AppendParam(OrgId)
                                       .DoAsync<DoQuerySql<Out_Factor>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询企业排放因子
        /// </summary>
        /// <param name="year"></param>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public async Task<List<Out_Factor>> SelectFactorOrgByTypeId(string OrgId, string TypeId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select f.*,a.TypeName from t_eng_factororg fy left join t_eng_factor f on fy.FactorId= f.Id left join t_eng_factortype a on f.TypeId = a.Id where fy.OrgId=")
                                       .AppendParam(OrgId)
                                       .Append(" and  f.TypeId=").AppendParam(TypeId)
                                       .DoAsync<DoQuerySql<Out_Factor>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询企业排放源
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<List<T_ENG_FactorType>> SelectOrgFactorType(string OrgId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select c.Id,c.TypeName from t_eng_factororg a 
                                                            left join t_eng_factor b on a.FactorId = b.Id
                                                            left join t_eng_factortype c on b.TypeId = c.Id
                                                            where a.OrgId=").AppendParam(OrgId).Append(" group by c.Id,c.TypeName")
                                       .DoAsync<DoQuerySql<T_ENG_FactorType>>();

                return docmd.ToList();
            }
        }

        #endregion
    }
}
