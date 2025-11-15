using Common;
using Common.Share;
using EfficiencyService.Model;
using EfficiencyService.Model.Production;
using InfluxDB.Client.Api.Domain;
using MyAccess.DB;
using NPOI.POIFS.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.DAL
{
    public class ProductionDAL : BaseDbSupport
    {
        #region 生产数据采集表
        /// <summary>
        /// 新增生产数据采集记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddProduction(T_Prod_Production data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新生产数据采集记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateProduction(T_Prod_Production data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除生产数据采集记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteProduction(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_prod_production set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 分页查询生产数据采集记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_Production>> SelectProductionPage(InProductionPageList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_Production>().Append(@"select a.*,b.ProductName,b.ProductType,c.RealName as 'updateName',d.RealName as 'createName',e.FacilityName,b.ProductModel 
                                                                from t_prod_production a
                                                                left join t_com_product b on a.ProductId = b.Id
                                                                left join mz_admin c on a.updateId = c.id
                                                                left join mz_admin d on a.createId = d.id
                                                                left join t_com_facility e on a.FacilityId = e.Id
                                                                where a.del_flag = 0 ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                        {
                                            sq.Append(" and a.FacilityId in( " + query.FacilityId + ")");
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductId), sq =>
                                        {
                                            sq.Append(" and a.ProductId = ").AppendParam(query.ProductId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductType), sq =>
                                        {
                                            sq.Append(" and b.ProductType = ").AppendParam(query.ProductType);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.BeginDate), sql =>
                                        {
                                            sql.Append(" and a.DDate >= ").AppendParam(query.BeginDate);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EndDate), sql =>
                                        {
                                            sql.Append(" and a.DDate <= ").AppendParam(query.EndDate);
                                        })
                                        .Then(query.beginTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time >= ").AppendParam(query.beginTime);
                                        })
                                        .Then(query.endTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time <= ").AppendParam(query.endTime);
                                        })
                                        .Append(" order by a.DDate asc");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }
        /// <summary>
        /// 查询生产数据采集记录信息
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Production> SelectProduction(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*,b.ProductName,b.ProductType,c.RealName as 'updateName',d.RealName as 'createName',e.FacilityName,b.ProductModel 
                                                                from t_prod_production a
                                                                left join t_com_product b on a.ProductId = b.Id
                                                                left join mz_admin c on a.updateId = c.id
                                                                left join mz_admin d on a.createId = d.id
                                                                left join t_com_facility e on a.FacilityId = e.Id
                                                                where a.Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<Out_Production>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 分页查询设备每日每月每年能耗采集记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_ProductionDay>> SelectProductionDatePage(InProductionDayPageList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_ProductionDay>().Append(@"select a.DDate,a.OutPut,a.OutValue,a.ProductId,b.ProductName,b.Unit
                                                                from (select a.ProductId,")
                                        .Then(!string.IsNullOrEmpty(query.DateType), sql =>
                                        {
                                            if (query.DateType == "日")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y-%m-%d') as 'DDate' ");
                                            }
                                            if (query.DateType == "月")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y-%m') as 'DDate' ");
                                            }
                                            if (query.DateType == "年")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y') as 'DDate' ");
                                            }
                                        })
                                        .Append(@",ROUND(SUM(a.OutPut),1) as 'OutPut',ROUND(SUM(a.OutValue),1) as 'OutValue'
                                                                from t_prod_production a 
                                                                where a.del_flag = 0 ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                        {
                                            sq.Append(" and a.FacilityId in( " + query.FacilityId + ")");
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductId), sql =>
                                        {
                                            sql.Append(" and a.ProductId = ").AppendParam(query.ProductId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.BeginDate), sql =>
                                        {
                                            sql.Append(" and a.DDate >= ").AppendParam(query.BeginDate);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EndDate), sql =>
                                        {
                                            sql.Append(" and a.DDate <= ").AppendParam(query.EndDate);
                                        })
                                        .Then(query.beginTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time >= ").AppendParam(query.beginTime);
                                        })
                                        .Then(query.endTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time <= ").AppendParam(query.endTime);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.DateType), sql =>
                                        {
                                            if (query.DateType == "日")
                                            {
                                                sql.Append(@"  group by a.ProductId,DATE_FORMAT(a.DDate,'%Y-%m-%d') ");
                                            }
                                            if (query.DateType == "月")
                                            {
                                                sql.Append(@"  group by a.ProductId,DATE_FORMAT(a.DDate,'%Y-%m') ");
                                            }
                                            if (query.DateType == "年")
                                            {
                                                sql.Append(@"  group by a.ProductId,DATE_FORMAT(a.DDate,'%Y') ");
                                            }
                                        })
                                        .Then(!string.IsNullOrEmpty(query.Operator), sql =>
                                        {
                                            if (query.Operator == ">")
                                            {
                                                sql.Append(" HAVING SUM(a.OutPut) > ").AppendParam(query.OutPut);
                                            }
                                            if (query.Operator == "<")
                                            {
                                                sql.Append(" HAVING SUM(a.OutPut) < ").AppendParam(query.OutPut);
                                            }
                                            if (query.Operator == "=")
                                            {
                                                sql.Append(" HAVING SUM(a.OutPut) = ").AppendParam(query.OutPut);
                                            }
                                            if (query.Operator == "≥")
                                            {
                                                sql.Append(" HAVING SUM(a.OutPut) >= ").AppendParam(query.OutPut);
                                            }
                                            if (query.Operator == "≤")
                                            {
                                                sql.Append(" HAVING SUM(a.OutPut) <= ").AppendParam(query.OutPut);
                                            }
                                        })
                                        .Append(@" ) as a left join t_com_product as b on a.ProductId = b.Id ");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        #endregion

        #region 设备能耗采集表
        /// <summary>
        /// 新增设备能耗采集记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddEnergy(T_Prod_Energy data)
        {
            data.UseVale = Math.Round(data.UseVale, 2);
            data.CostVale = Math.Round(data.CostVale, 2);
            data.CarbonEmission = Math.Round(data.CarbonEmission, 2);
            data.ConvertCoal = Math.Round(data.ConvertCoal, 2);
            if (data.DataSource == "2" && data.UseVale == 0)//没有用量不新增记录
            {
                return 1;
            }
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新设备能耗采集记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateEnergy(T_Prod_Energy data)
        {
            data.UseVale = Math.Round(data.UseVale, 2);
            data.CostVale = Math.Round(data.CostVale, 2);
            data.CarbonEmission = Math.Round(data.CarbonEmission, 2);
            data.ConvertCoal = Math.Round(data.ConvertCoal, 2);
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除设备能耗采集记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteEnergy(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_prod_energy set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 分页查询设备能耗采集记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_Energy>> SelectEnergyPage(InEnergyPageList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_Energy>().Append(@"select a.*,b.RealName as 'updateName',c.RealName as 'createName',d.EquipmentName
                                                                        from t_prod_energy a
                                                                        left join mz_admin b on a.updateId = b.id
                                                                        left join mz_admin c on a.createId = c.id
                                                                        left join t_com_equipment d on a.EquipmentId = d.Id
                                                                        left join t_price_policy e on d.PolicyId = e.Id
                                                                        where a.del_flag = 0 and a.UseVale>0 ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                        {
                                            sq.Append(" and d.FacilityId in(" + query.FacilityId + ") ");
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EquipmentId), sq =>
                                        {
                                            sq.Append(" and a.EquipmentId = ").AppendParam(query.EquipmentId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FactorId), sq =>
                                        {
                                            sq.Append(" and a.FactorId = ").AppendParam(query.FactorId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.DataSource), sq =>
                                        {
                                            sq.Append(" and a.DataSource = ").AppendParam(query.DataSource);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.BeginDate), sql =>
                                        {
                                            sql.Append(" and a.DDate >= ").AppendParam(query.BeginDate);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EndDate), sql =>
                                        {
                                            sql.Append(" and a.DDate <= ").AppendParam(query.EndDate);
                                        }).Append(" order by a.DDate,a.Memo asc");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        /// <summary>
        /// 分页查询设备每日每月每年能耗采集记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_EnergyDay>> SelectEnergyDatePage(InEnergyDayPageList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_EnergyDay>().Append(@"select b.EquipmentName,a.FactorId,a.FactorName,a.DDate,a.UseVale,a.CostVale,a.CarbonEmission,a.ConvertCoal,c.Unit,c.LageUnit,a.DataSource 
                                                                from (select a.EquipmentId,a.FactorId,a.FactorName,")
                                        .Then(!string.IsNullOrEmpty(query.DateType), sql =>
                                        {
                                            if (query.DateType == "日")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y-%m-%d') as 'DDate' ");
                                            }
                                            if (query.DateType == "月")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y-%m') as 'DDate' ");
                                            }
                                            if (query.DateType == "年")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y') as 'DDate' ");
                                            }
                                        })
                                        .Append(@",ROUND(SUM(a.UseVale),1) as 'UseVale',ROUND(sum(a.CostVale),1) as 'CostVale',a.DataSource,ROUND(sum(a.CarbonEmission),1) as 'CarbonEmission',ROUND(sum(a.ConvertCoal),1) as 'ConvertCoal'
                                                                from t_prod_energy a 
                                                                left join t_com_equipment b on a.EquipmentId = b.Id
                                                                left join t_price_policy c on b.PolicyId = c.Id
                                                                where a.del_flag = 0 ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                        {
                                            sq.Append(" and b.FacilityId in( " + query.FacilityId + ")");
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FactorId), sq =>
                                        {
                                            sq.Append(" and c.EnergyType = ").AppendParam(query.FactorId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EquipmentId), sq =>
                                        {
                                            sq.Append(" and a.EquipmentId = ").AppendParam(query.EquipmentId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.DataSource), sq =>
                                        {
                                            sq.Append(" and a.DataSource = ").AppendParam(query.DataSource);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.BeginDate), sql =>
                                        {
                                            sql.Append(" and a.DDate >= ").AppendParam(query.BeginDate);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EndDate), sql =>
                                        {
                                            sql.Append(" and a.DDate <= ").AppendParam(query.EndDate);
                                        })
                                        .Then(query.beginTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time >= ").AppendParam(query.beginTime);
                                        })
                                        .Then(query.endTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time <= ").AppendParam(query.endTime);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.DateType), sql =>
                                        {
                                            if (query.DateType == "日")
                                            {
                                                sql.Append(@"  group by a.EquipmentId,DATE_FORMAT(a.DDate,'%Y-%m-%d'),a.DataSource,a.FactorName,a.FactorId ");
                                            }
                                            if (query.DateType == "月")
                                            {
                                                sql.Append(@"  group by a.EquipmentId,DATE_FORMAT(a.DDate,'%Y-%m'),a.DataSource,a.FactorName,a.FactorId ");
                                            }
                                            if (query.DateType == "年")
                                            {
                                                sql.Append(@"  group by a.EquipmentId,DATE_FORMAT(a.DDate,'%Y'),a.DataSource,a.FactorName,a.FactorId ");
                                            }
                                        })
                                        .Then(!string.IsNullOrEmpty(query.Operator), sql =>
                                        {
                                            if (query.Operator == ">")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) > ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "<")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) < ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "=")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) = ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "≥")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) >= ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "≤")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) <= ").AppendParam(query.UseVale);
                                            }
                                        })
                                        .Append(@" ) as a left join t_com_equipment b on a.EquipmentId = b.Id
                                                                left join t_price_policy c on c.Id = b.PolicyId
                                                                left join t_eng_factortype d on d.Id = c.EnergyType  ");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        /// <summary>
        /// 分页查询设施每日每月每年能耗采集记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_FacilityDay>> SelectFacilityEnergyDatePage(InEnergyDayPageList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_FacilityDay>().Append(@"select a.FacilityId,a.FactorId,b.FacilityName,a.FactorName,a.DDate,a.UseVale,a.CostVale,a.CarbonEmission,a.ConvertCoal,a.Unit,a.LageUnit,a.DataSource 
                                                                from (select b.FacilityId,a.FactorName,a.DataSource,a.Unit,a.LageUnit,a.FactorId,")
                                        .Then(!string.IsNullOrEmpty(query.DateType), sql =>
                                        {
                                            if (query.DateType == "日")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y-%m-%d') as 'DDate' ");
                                            }
                                            if (query.DateType == "月")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y-%m') as 'DDate' ");
                                            }
                                            if (query.DateType == "年")
                                            {
                                                sql.Append(@" DATE_FORMAT(a.DDate,'%Y') as 'DDate' ");
                                            }
                                        })
                                        .Append(@",ROUND(SUM(a.UseVale),1) as 'UseVale',ROUND(sum(a.CostVale),1) as 'CostVale',ROUND(sum(a.CarbonEmission),1) as 'CarbonEmission',ROUND(sum(a.ConvertCoal),1) as 'ConvertCoal'
                                                                from t_prod_energy a 
                                                                left join t_com_equipment b on a.EquipmentId = b.Id
                                                                left join t_price_policy c on b.PolicyId = c.Id
                                                                where a.del_flag = 0 ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                        {
                                            sq.Append(" and b.FacilityId in( " + query.FacilityId + ")");
                                        })
                                        .Then(!string.IsNullOrEmpty(query.FactorId), sq =>
                                        {
                                            sq.Append(" and c.EnergyType = ").AppendParam(query.FactorId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EquipmentId), sq =>
                                        {
                                            sq.Append(" and a.EquipmentId = ").AppendParam(query.EquipmentId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.DataSource), sq =>
                                        {
                                            sq.Append(" and a.DataSource = ").AppendParam(query.DataSource);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.BeginDate), sql =>
                                        {
                                            sql.Append(" and a.DDate >= ").AppendParam(query.BeginDate);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EndDate), sql =>
                                        {
                                            sql.Append(" and a.DDate <= ").AppendParam(query.EndDate);
                                        })
                                        .Then(query.beginTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time >= ").AppendParam(query.beginTime);
                                        })
                                        .Then(query.endTime != null, sql =>
                                        {
                                            sql.Append(" and a.create_time <= ").AppendParam(query.endTime);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.DateType), sql =>
                                        {
                                            if (query.DateType == "日")
                                            {
                                                sql.Append(@"  group by b.FacilityId,a.FactorName,DATE_FORMAT(a.DDate,'%Y-%m-%d'),a.DataSource,a.Unit,a.LageUnit,a.FactorId ");
                                            }
                                            if (query.DateType == "月")
                                            {
                                                sql.Append(@"  group by b.FacilityId,a.FactorName,DATE_FORMAT(a.DDate,'%Y-%m'),a.DataSource,a.Unit,a.LageUnit,a.FactorId ");
                                            }
                                            if (query.DateType == "年")
                                            {
                                                sql.Append(@"  group by b.FacilityId,a.FactorName,DATE_FORMAT(a.DDate,'%Y'),a.DataSource,a.Unit,a.LageUnit,a.FactorId ");
                                            }
                                        })
                                        .Then(!string.IsNullOrEmpty(query.Operator), sql =>
                                        {
                                            if (query.Operator == ">")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) > ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "<")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) < ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "=")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) = ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "≥")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) >= ").AppendParam(query.UseVale);
                                            }
                                            if (query.Operator == "≤")
                                            {
                                                sql.Append(" HAVING SUM(a.UseVale) <= ").AppendParam(query.UseVale);
                                            }
                                        })
                                        .Append(@" ) as a left join t_com_facility b on a.FacilityId = b.Id ");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }
        /// <summary>
        /// 查询设备能耗采集记录信息
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Energy> SelectEnergy(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*,b.RealName as 'updateName',c.RealName as 'createName',d.EquipmentName
                                                                        from t_prod_energy a
                                                                        left join mz_admin b on a.updateId = b.id
                                                                        left join mz_admin c on a.createId = c.id
                                                                        left join t_com_equipment d on a.EquipmentId = d.Id
                                                                        where a.Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<Out_Energy>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询期末能耗数据
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Energy> SelectEnergyMax(string DDate, string EquipmentId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"SELECT * FROM `t_prod_energy` where DDate<")
                    .AppendParam(DDate).Append(" and EquipmentId=").AppendParam(EquipmentId).Append(" order by EndVale desc LIMIT 1 ")
                                       .DoAsync<DoQuerySql<Out_Energy>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询期末能耗数据
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Energy_H> SelectEnergyHourMax(string DDate, string EquipmentId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"SELECT * FROM `t_prod_energy_h` where DDate<")
                    .AppendParam(DDate).Append(" and EquipmentId=").AppendParam(EquipmentId).Append(" order by EndVale desc LIMIT 1 ")
                                       .DoAsync<DoQuerySql<T_Prod_Energy_H>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询期末能耗数据
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Energy_T> SelectEnergyTimeMax(string DDate, string EquipmentId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"SELECT * FROM `t_prod_energy_t` where DDate<")
                    .AppendParam(DDate).Append(" and EquipmentId=").AppendParam(EquipmentId).Append(" order by EndVale desc LIMIT 1 ")
                                       .DoAsync<DoQuerySql<T_Prod_Energy_T>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询设备能耗采集记录
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_EnergyList>> SelectEnergyList(InEnergyTree query)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*,b.FacilityId,b.EquipmentName 
                                                            from t_prod_energy a 
                                                            left join t_com_equipment b on a.EquipmentId=b.Id  
                                                            where a.del_flag=0 and b.DataState='1' ")
                                                            .Then(query.OrgId != null, sq =>
                                                            {
                                                                sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                                            })
                                                            .Then(!string.IsNullOrEmpty(query.FactorId), sq =>
                                                            {
                                                                sq.Append(" and a.FactorId = ").AppendParam(query.FactorId);
                                                            })
                                                            .Then(!string.IsNullOrEmpty(query.EquipmentId), sq =>
                                                            {
                                                                sq.Append(" and a.EquipmentId = ").AppendParam(query.EquipmentId);
                                                            })
                                                            .Then(!string.IsNullOrEmpty(query.BeginDate), sql =>
                                                            {
                                                                sql.Append(" and a.DDate >= ").AppendParam(query.BeginDate);
                                                            })
                                                            .Then(!string.IsNullOrEmpty(query.EndDate), sql =>
                                                            {
                                                                sql.Append(" and a.DDate <= ").AppendParam(query.EndDate);
                                                            })
                                                            .DoAsync<DoQuerySql<Out_EnergyList>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 删除能耗时段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteEnergyList(string EquipmentId, DateTime DDate)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_prod_energy where EquipmentId=").AppendParam(EquipmentId).Append(" and DDate=").AppendParam(DDate.ToString("yyyy-MM-dd")).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 新增采集配置记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddConfig(T_Prod_Config data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新采集配置记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateConfig(T_Prod_Config data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 查询采集配置记录
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Config> SelectConfig(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select * from t_prod_config where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Prod_Config>>();

                return docmd.ToFirst();
            }
        }
        #endregion

        #region 供应商和物料管理

        #region 供应商
        /// <summary>
        /// 新增供应商
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddProvider(T_Prod_Provider data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新供应商
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateProvider(T_Prod_Provider data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteProvider(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_prod_provider set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 分页查询供应商记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<T_Prod_Provider2>> SelectProviderPage(In_Provider query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<T_Prod_Provider2>().Append(@"select a.*,c.RealName as 'updateName',d.RealName as 'createName',e.Province,e.City,e.District
                                                                from t_prod_provider a 
                                                                left join mz_admin c on a.updateId = c.id
                                                                left join mz_admin d on a.createId = d.id
                                                                left join mz_area e on a.Area = e.ParentPath
                                                            where a.del_flag = 0 ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProviderName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProviderName) + "%";
                                            sq.Append(" and a.ProviderName like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.Manager), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.Manager) + "%";
                                            sq.Append(" and a.Manager like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.Area), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.Area) + "%";
                                            sq.Append(" and a.Area like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProviderCode), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProviderCode) + "%";
                                            sq.Append(" and a.ProviderCode like ").AppendParam(tmpkey);
                                        });

                return await tsql.GeneratePageObjectAsync(query);
            }
        }
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Provider> SelectProvider(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*  from t_prod_provider a  where a.Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Prod_Provider>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询供应商
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Provider> SelectProviderByProviderName(string ProviderCode)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*  from t_prod_provider a where a.ProviderCode = ").AppendParam(ProviderCode)
                                       .DoAsync<DoQuerySql<T_Prod_Provider>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询物料对应供应商信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Prod_Provider>> SelectMaterialProvider(string MaterialId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"SELECT a.MaterialId,a.ProviderId ,b.MaterialCode,b.MaterialName,b.MaterialType,
                                    b.MaterialUnit,c.ProviderCode,c.ProviderName,c.ProviderAddress,a.ProductModel
                                    FROM t_prod_providermaterial a 
                                    left join t_prod_material b on a.MaterialId = b.Id
                                    left join t_prod_provider c on a.ProviderId = c.Id  where a.MaterialId = ").AppendParam(MaterialId)
                                       .DoAsync<DoQuerySql<Out_Prod_Provider>>();

                return docmd.ToList();
            }
        }
        #endregion

        #region 物料
        /// <summary>
        /// 新增物料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddMaterial(T_Prod_Material data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新物料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateMaterial(T_Prod_Material data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除物料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteMaterial(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_prod_material set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 查询物料信息
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Material> SelectMaterial(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*  from t_prod_material a  where a.Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<Out_Material>>();

                return docmd.ToFirst();
            }
        }
        /// <summary>
        /// 查询物料信息
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Material> SelectMaterialByMaterialCode(string MaterialCode)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*  from t_prod_material a  where a.MaterialCode = ").AppendParam(MaterialCode)
                                       .DoAsync<DoQuerySql<Out_Material>>();

                return docmd.ToFirst();
            }
        }
        /// <summary>
        /// 分页查询物料记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_Material>> SelectMaterialPage(In_Material query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_Material>().Append(@"select a.*,
                                    (select Count(*) from t_prod_providermaterial b where b.MaterialId=a.Id) ProviderCount
                                        from t_prod_material a where a.del_flag = '0' ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.MaterialType), sq =>
                                        {
                                            sq.Append(" and a.MaterialType = ").AppendParam(query.MaterialType);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.MaterialUnit), sq =>
                                        {
                                            sq.Append(" and a.MaterialUnit = ").AppendParam(query.MaterialUnit);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.MaterialCode), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.MaterialCode) + "%";
                                            sq.Append(" and a.MaterialCode like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.MaterialName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.MaterialName) + "%";
                                            sq.Append(" and a.MaterialName like ").AppendParam(tmpkey);
                                        });

                return await tsql.GeneratePageObjectAsync(query);
            }
        }
        /// <summary>
        /// 新增供应商物料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddProviderMaterial(T_Prod_ProviderMaterial data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 删除供应商物料
        /// </summary>
        /// <param name="MaterialId">物料编码</param>
        /// <returns></returns>
        public async Task<int> DeleteProviderMaterial(string MaterialId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_prod_providermaterial where MaterialId=").AppendParam(MaterialId).DoAsync<DoExecSql>()).RowCount;
            }
        }
        #endregion

        #region 物料碳足迹

        /// <summary>
        /// 新增物料碳足迹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddMaterialCarbon(T_Prod_MaterialCarbon data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新物料碳足迹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateMaterialCarbon(T_Prod_MaterialCarbon data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除物料碳足迹
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteMaterialCarbon(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_prod_materialcarbon set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 分页查询物料碳足迹记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_MaterialCarbon>> SelectMaterialCarbonPage(In_MaterialCarbon query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_MaterialCarbon>().Append(@"select a.Id,a.MaterialId,b.MaterialCode,b.MaterialName,b.MaterialType,b.MaterialUnit,
                                                        a.ProviderId,a.ProductBorder,a.CarbonEmission,a.CarbonUnit,c.ProviderCode,c.ProviderName,
                                                        c.ProviderAddress,d.ProductModel
                                                        from t_prod_materialcarbon a 
                                                        left join t_prod_material b on a.MaterialId = b.Id
                                                        left join t_prod_provider c on a.ProviderId = c.Id
                                                        left join t_prod_providermaterial d on a.MaterialId = d.MaterialId and a.ProviderId=d.ProviderId
                                                        where a.del_flag='0' ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.MaterialName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.MaterialName) + "%";
                                            sq.Append(" and b.MaterialName like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductModel), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProductModel) + "%";
                                            sq.Append(" and d.ProductModel like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.MaterialType), sq =>
                                        {
                                            sq.Append(" and b.MaterialType = ").AppendParam(query.MaterialType);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductBorder), sq =>
                                        {
                                            sq.Append(" and a.ProductBorder = ").AppendParam(query.ProductBorder);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProviderId), sq =>
                                        {
                                            sq.Append(" and a.ProviderId = ").AppendParam(query.ProviderId);
                                        });

                return await tsql.GeneratePageObjectAsync(query);
            }
        }
        /// <summary>
        /// 查询物料碳足迹信息
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_MaterialCarbon> SelectMaterialCarbon(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*  from t_prod_materialcarbon a  where a.Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Prod_MaterialCarbon>>();

                return docmd.ToFirst();
            }
        }
        #endregion

        #region 碳排计划表
        /// <summary>
        /// 新增碳排计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddCarbonPlan(T_Prod_CarbonPlan data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新碳排计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateCarbonPlan(T_Prod_CarbonPlan data)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_prod_carbonplan set Unit=").AppendParam(data.Unit)
                     .Append(" ,Granularity=").AppendParam(data.Granularity)
                     .Append(" ,Month1=").AppendParam(data.Month1)
                     .Append(" ,Month2=").AppendParam(data.Month2)
                     .Append(" ,Month3=").AppendParam(data.Month3)
                     .Append(" ,Month4=").AppendParam(data.Month4)
                     .Append(" ,Month5=").AppendParam(data.Month5)
                     .Append(" ,Month6=").AppendParam(data.Month6)
                     .Append(" ,Month7=").AppendParam(data.Month7)
                     .Append(" ,Month8=").AppendParam(data.Month8)
                     .Append(" ,Month9=").AppendParam(data.Month9)
                     .Append(" ,Month10=").AppendParam(data.Month10)
                     .Append(" ,Month11=").AppendParam(data.Month11)
                     .Append(" ,Month12=").AppendParam(data.Month12)
                     .Append(" ,YearTotal=").AppendParam(data.YearTotal)
                    .Append(" where Year=").AppendParam(data.Year)
                    .Append(" and OrgId=").AppendParam(data.OrgId)
                    .Append(" and CarbonType=").AppendParam(data.CarbonType)
                    .DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 查询碳排计划
        /// </summary>
        /// <returns></returns>
        public async Task<In_CarbonPlanDetail> SelectCarbonPlan(string Year, long OrgId, string CarbonType)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*  from t_prod_carbonplan a  where a.Year = ").AppendParam(Year)
                                    .Append(" and a.OrgId=").AppendParam(OrgId)
                                    .Append(" and a.CarbonType=").AppendParam(CarbonType)
                                    .DoAsync<DoQuerySql<In_CarbonPlanDetail>>();

                return docmd.ToFirst();
            }
        }
        #endregion

        #region 产品物料清单
        /// <summary>
        /// 新增产品物料清单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddBom(T_Prod_Bom data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 删除产品物料清单
        /// </summary>
        /// <param name="ModelId"></param>
        /// <param name="LinkId"></param>
        /// <returns></returns>
        public async Task<int> DeleteBom(string ModelId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_prod_bom where ModelId=").AppendParam(ModelId).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 查询产品物料清单
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Bom>> SelectBomList(string ModelId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.Id,a.MaterialId,b.MaterialCode,b.MaterialName,b.MaterialType,b.MaterialUnit,
                                                        a.ProviderId,a.ProductBorder,a.CarbonEmission,a.CarbonUnit,c.ProviderCode,c.ProviderName,
                                                        c.ProviderAddress,d.ProductModel,e.Dosage,e.LinkId,e.BomType
                                                        from t_prod_bom e
                                                        left join t_prod_materialcarbon a on e.MaterialCarbonId = a.Id
                                                        left join t_prod_material b on a.MaterialId = b.Id
                                                        left join t_prod_provider c on a.ProviderId = c.Id
                                                        left join t_prod_providermaterial d on a.MaterialId = d.MaterialId and a.ProviderId=d.ProviderId
                                                        where e.ModelId =").AppendParam(ModelId)
                                       .DoAsync<DoQuerySql<Out_Bom>>();

                return docmd.ToList();
            }
        }
        #endregion
        #endregion

        #region 能耗时段
        /// <summary>
        /// 新增能耗时段
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddEnergyHour(T_Prod_Energy_H data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除能耗时段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteEnergyHour(string EquipmentId, DateTime DDate)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_prod_energy_h where EquipmentId=").AppendParam(EquipmentId).Append(" and DDate=").AppendParam(DDate.ToString("yyyy-MM-dd")).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Energy_Hour> SelectEnergyHour(string FacilityId, string DDate)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.TTime,ROUND(sum(UseVale),2) UseVale
                                                                from t_prod_energy_h a 
                                                                left join t_com_equipment b on a.EquipmentId=b.Id
                                                                where ")
                                                            .Append(" b.FacilityId in(" + FacilityId + ") ")
                                                            .Append(" and a.DDate =").AppendParam(DDate)
                                                            .Append(@" group by a.TTime 
                                                            order by sum(UseVale) desc
                                                            LIMIT 1")
                                       .DoAsync<DoQuerySql<T_Prod_Energy_Hour>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Prod_Energy_Hour>> SelectEnergyHourList(string EquipmentId, string BeginDate, string EndDate)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*
                                                                from t_prod_energy_h a ")
                                       .Append(" where a.EquipmentId=").AppendParam(EquipmentId)
                                       .Append(" and a.DDate>=").AppendParam(BeginDate)
                                       .Append(" and a.DDate<=").AppendParam(EndDate)
                                       .DoAsync<DoQuerySql<T_Prod_Energy_Hour>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Prod_Energy_T>> SelectEnergyTimeList(string EquipmentId, string BeginDate, string EndDate, string FactorId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*
                                                                from t_prod_energy_t a ")
                                       .Append(" where a.EquipmentId=").AppendParam(EquipmentId)
                                       .Append(" and a.FactorId=").AppendParam(FactorId)
                                       .Append(" and a.DDate>=").AppendParam(BeginDate)
                                       .Append(" and a.DDate<=").AppendParam(EndDate)
                                       .DoAsync<DoQuerySql<T_Prod_Energy_T>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Prod_Energy_T2>> SelectEnergyTimeList2(string FacilityId, string BeginDate, string EndDate, string FactorId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*,d.EquipmentName 
                                                                from t_prod_energy_t a 
                                                                left join t_com_equipment d on a.EquipmentId = d.Id ")
                                       .Append(" where d.FacilityId in(" + FacilityId + ") ")
                                       .Append(" and a.FactorId=").AppendParam(FactorId)
                                       .Append(" and a.DDate>=").AppendParam(BeginDate)
                                       .Append(" and a.DDate<=").AppendParam(EndDate)
                                       .DoAsync<DoQuerySql<T_Prod_Energy_T2>>();
                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询能耗时段
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Energy_H> SelectEnergyTimeList3(string FacilityId, string BeginDate, string EndDate)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"SELECT a.TTime,sum(a.UseVale) UseVale,MAX(d.PolicyId) EquipmentId
                                                                from t_prod_energy_h a 
                                                                left join t_com_equipment d on a.EquipmentId = d.Id ")
                                       .Append(" where d.FacilityId in(" + FacilityId + ") ")
                                       .Append(" and a.DDate>=").AppendParam(BeginDate)
                                       .Append(" and a.DDate<=").AppendParam(EndDate)
                                       .Append(" group by a.TTime order by sum(a.UseVale) desc ")
                                       .DoAsync<DoQuerySql<T_Prod_Energy_H>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 新增能耗时段
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddEnergyTime(T_Prod_Energy_T data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除能耗时段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteEnergyTime(string EquipmentId, DateTime DDate)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_prod_energy_t where EquipmentId=").AppendParam(EquipmentId).Append(" and DDate=").AppendParam(DDate.ToString("yyyy-MM-dd")).DoAsync<DoExecSql>()).RowCount;
            }
        }
        #endregion
    }
}
