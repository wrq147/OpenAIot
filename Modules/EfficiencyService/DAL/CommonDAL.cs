using Common;
using Common.Share;
using EfficiencyService.Model;
using EfficiencyService.Model.Common;
using IoTService.Models;
using MyAccess.DB;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyService.DAL
{
    public class CommonDAL: BaseDbSupport
    {
        #region 产品
        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddProduct(T_Com_Product data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateProduct(T_Com_Product data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteProduct(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_product set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 分页查询产品
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<T_Com_Product>> SelectProductPage(In_ProductPageList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<T_Com_Product>().Append("select a.Id,a.ProductModel,a.Unit,a.ProductName,a.ShortName,a.BrandName,a.ProductType,a.ProductPrice,a.OnMarket,a.MarketTime,a.Memo,a.del_flag,b.RealName as 'createName',a.createId,a.updateId,a.create_time,b.RealName as 'updateName',a.update_time from t_com_product a left join mz_admin b on a.createId = b.id left join mz_admin c on a.updateId = c.id  where a.del_flag = 0 ")
                                        .Then(!string.IsNullOrEmpty(query.ProductName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProductName) + "%";
                                            sq.Append(" and a.ProductName like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ShortName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ShortName) + "%";
                                            sq.Append(" and a.ShortName like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.BrandName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.BrandName) + "%";
                                            sq.Append(" and a.BrandName like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductModel), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProductModel) + "%";
                                            sq.Append(" and a.ProductModel like ").AppendParam(tmpkey);
                                        })
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductType), sql =>
                                        {
                                            sql.Append(" and a.ProductType = ").AppendParam(query.ProductType);
                                        }).Append(" order by a.create_time asc");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        /// <summary>
        /// 分页查询产品能效指标
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_ProductEnergy>> SelectProductEnergyPage(In_ProductEnergyPageList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_ProductEnergy>().Append(@"select a.*,t1.OutPut,t2.UseVale,t2.EUnit,t2.LageEUnit,t2.EnergyType,t2.TypeName
                                     from t_com_product a 
                                     left join 
                                     (select ROUND(SUM(OutPut),2) OutPut,ProductId,FacilityId from t_prod_production 
                                        where del_flag='0' ")
                                        .Then(!string.IsNullOrEmpty(query.beginDate), sql =>
                                        {
                                            sql.Append(" and DDate >= ").AppendParam(query.beginDate);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.endDate), sql =>
                                        {
                                            sql.Append(" and DDate <= ").AppendParam(query.endDate);
                                        })
                                        .Append(@" group by ProductId,FacilityId) t1 on t1.ProductId = a.Id
                                     left join 
                                     (select ROUND(SUM(UseVale),2) UseVale,c.Unit EUnit,c.LageUnit LageEUnit,d.FacilityId,c.FactorId EnergyType,c.FactorName TypeName
                                     from t_prod_energy c 
                                     left join t_com_equipment d on c.EquipmentId = d.Id
                                     where c.del_flag='0' ")
                                         .Then(!string.IsNullOrEmpty(query.beginDate), sql =>
                                         {
                                             sql.Append(" and c.DDate >= ").AppendParam(query.beginDate);
                                         })
                                        .Then(!string.IsNullOrEmpty(query.endDate), sql =>
                                        {
                                            sql.Append(" and c.DDate <= ").AppendParam(query.endDate);
                                        })
                                     .Append(@" group by c.Unit,c.LageUnit,d.FacilityId,c.FactorId,c.FactorName ) t2 on t2.FacilityId = t1.FacilityId
                                     where a.del_flag='0' and t1.OutPut>0 ")
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.EnergyType), sql =>
                                        {
                                            sql.Append(" and t2.EnergyType = ").AppendParam(query.EnergyType);
                                        });

                return await tsql.GeneratePageObjectAsync(query);
            }
        }
        /// <summary>
        /// 查询产品信息
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_Product> SelectProduct(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select a.Id,a.ProductModel,a.ProductName,a.Unit,a.ShortName,a.BrandName,a.ProductType,a.ProductPrice,a.OnMarket,a.MarketTime,a.Memo,a.del_flag,b.RealName as 'createName',a.createId,a.updateId,a.create_time,b.RealName as 'updateName',a.update_time from t_com_product a left join mz_admin b on a.createId = b.id left join mz_admin c on a.updateId = c.id  where  a.Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Com_Product>>();

                return docmd.ToFirst();
            }
        }
        #endregion

        #region 设施
        /// <summary>
        /// 新增设施
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddFacility(T_Com_Facility data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新设施
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateFacility(T_Com_Facility data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除设施
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteFacility(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_facility set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 查询设施信息
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_Facility> SelectFacility(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_facility where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Com_Facility>>();

                return docmd.ToFirst();
            }
        }



        /// <summary>
        /// 查询子设施信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Facility>> SelectFacilityByParentId(string ParentId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_facility where del_flag = 0 and ParentId = ").AppendParam(ParentId)
                                     .DoAsync<DoQuerySql<T_Com_Facility>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询设施下产品列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Product>> SelectFacilityProduct(InFacilityProductList query)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.* from t_com_product a
											 right join (select a.ProductId from t_prod_production a where a.del_flag = 0 ")
                                             .Then(query.OrgId != null, sq =>
                                             {
                                                 sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                             })
                                            //.Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                            //{
                                            //    sq.Append(" and a.FacilityId = ").AppendParam(query.FacilityId);
                                            //})
                                            .Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                            {
                                                sq.Append(" and a.FacilityId in( " + query.FacilityId + ")");
                                            })
                                            .Then(!string.IsNullOrEmpty(query.BeginDate), sql =>
                                            {
                                                sql.Append(" and a.DDate >= ").AppendParam(query.BeginDate);
                                            })
                                            .Then(!string.IsNullOrEmpty(query.EndDate), sql =>
                                            {
                                                sql.Append(" and a.DDate <= ").AppendParam(query.EndDate);
                                            })
                                            .Append(" group by a.ProductId) b on a.Id = b.ProductId ")
                                     .DoAsync<DoQuerySql<T_Com_Product>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询设施列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Facility>> SelectFacilityTree(long OrgId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_facility where del_flag = 0 and OrgId = ").AppendParam(OrgId)
                                       .DoAsync<DoQuerySql<T_Com_Facility>>();

                return docmd.ToList();
            }
        }
        #endregion

        #region 设备
        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddEquipment(T_Com_Equipment data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateEquipment(T_Com_Equipment data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteEquipment(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_equipment set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 查询设备信息
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_Equipment> SelectEquipment(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_equipment where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Com_Equipment>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询设备关联的详细信息
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Equipment> SelectEquipmentInfo(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.Id,a.OrgId,a.EquipmentCode,a.EquipmentName,a.DataState,a.PolicyId,a.ThirdId,
a.FacilityId,a.del_flag,b.RealName as 'createName',a.createId,a.updateId,a.create_time,b.RealName as 'updateName',
a.update_time,d.PolicyName,d.Unit,e.FactorName,d.EnergyType,f.TypeName,g.FacilityName 
from t_com_equipment a 
left join mz_admin b on a.createId = b.id 
left join mz_admin c on a.updateId = c.id 
left join t_price_policy d on a.PolicyId = d.Id 
left join t_eng_factor e on d.FactorId = e.Id  
left join t_eng_factortype f on f.Id = d.EnergyType  
left join t_com_facility g on g.Id = a.FacilityId
where a.Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<Out_Equipment>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 查询企业下所有设备
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Equipment>> SelectEquipmentList(long OrgId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_equipment where del_flag='0' and OrgId=").AppendParam(OrgId)
                                     .DoAsync<DoQuerySql<T_Com_Equipment>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询所有设备需要采集的设备
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Equipment>> SelectEnergyEquipmentList()
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_equipment where del_flag='0' and ThirdId<>'-' ").DoAsync<DoQuerySql<T_Com_Equipment>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询所有物联设备
        /// </summary>
        /// <returns></returns>
        public async Task<List<MZ_IotDevice>> SelectIotDeviceList()
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_iot_device ").DoAsync<DoQuerySql<MZ_IotDevice>>();

                return docmd.ToList();
            }
        }
        /// <summary>
        /// 查询所有物联设备
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_EquipemntType>> SelectEquipmentTypeList(long OrgId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"SELECT IFNULL(b.EnergyType,'-') EnergyType,IFNULL(c.TypeName,'未分类') TypeName,Count(0) TypeCount FROM t_com_equipment a
                        left join t_price_policy b on a.PolicyId = b.Id
                        left join t_eng_factortype c on b.EnergyType=c.Id
                        where a.del_flag='0' and a.OrgId=").AppendParam(OrgId)
                        .Append(" GROUP BY b.EnergyType,c.TypeName ").DoAsync<DoQuerySql<Out_EquipemntType>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 分页查询设备
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_Equipment>> SelectEquipmentPage(In_EquipmentList query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_Equipment>().Append(@"select a.Id,a.OrgId,a.EquipmentCode,a.EquipmentName,a.DataState,
a.PolicyId,a.ThirdId,a.FacilityId,a.del_flag,b.RealName as 'createName',a.createId,a.updateId,a.create_time,b.RealName as 'updateName',
a.update_time,d.PolicyName,d.Unit,e.FactorName,d.EnergyType,f.TypeName,g.FacilityName,ifnull(h.Online,'2') 'Online' 
from t_com_equipment a 
left join mz_admin b on a.createId = b.id 
left join mz_admin c on a.updateId = c.id 
left join t_price_policy d on a.PolicyId = d.Id 
left join t_eng_factor e on d.FactorId = e.Id  
left join t_eng_factortype f on f.Id = d.EnergyType 
left join t_com_facility g on g.Id = a.FacilityId
left join mz_iot_device h on h.DeviceNumber = a.ThirdId
where a.del_flag='0' ")
                                    .Then(query.OrgId != null, sq =>
                                    {
                                        sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                    })
                                    .Then(!string.IsNullOrEmpty(query.DataState), sq =>
                                    {
                                         sq.Append(" and a.DataState = ").AppendParam(query.DataState);
                                    })
                                    .Then(!string.IsNullOrEmpty(query.TypeId), sq =>
                                    {
                                        sq.Append(" and d.EnergyType = ").AppendParam(query.TypeId);
                                    })
                                    .Then(!string.IsNullOrEmpty(query.FacilityId), sq =>
                                    {
                                         sq.Append(" and a.FacilityId in(" + query.FacilityId + ")");
                                    })
                                    .Then(!string.IsNullOrEmpty(query.PolicyState), sq =>
                                    {
                                        if (query.PolicyState == "1")
                                        {
                                            sq.Append(" and a.PolicyId <> '-' ");
                                        }
                                        else if (query.PolicyState == "2")
                                        {
                                            sq.Append(" and a.PolicyId = '-' ");
                                        }
                                    })
                                    .Then(!string.IsNullOrEmpty(query.FacilityState), sq =>
                                    {
                                        if (query.FacilityState == "1")
                                        {
                                            sq.Append(" and a.FacilityId <> '-' ");
                                        }
                                        else if (query.FacilityState == "2")
                                        {
                                            sq.Append(" and a.FacilityId = '-' ");
                                        }
                                    })
                                    .Then(!string.IsNullOrEmpty(query.ThirdState), sq =>
                                    {
                                        if (query.ThirdState == "1")
                                        {
                                            sq.Append(" and a.ThirdId <> '-' ");
                                        }
                                        else if (query.ThirdState == "2")
                                        {
                                            sq.Append(" and a.ThirdId = '-' ");
                                        }
                                    })
                                    .Then(!string.IsNullOrEmpty(query.EquipmentName), sq =>
                                    {
                                        string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.EquipmentName) + "%";
                                        sq.Append(" and a.EquipmentName like ").AppendParam(tmpkey);
                                    })
                                    .Then(!string.IsNullOrEmpty(query.EquipmentCode), sq =>
                                    {
                                        string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.EquipmentCode) + "%";
                                        sq.Append(" and a.EquipmentCode like ").AppendParam(tmpkey);
                                    }).Append(" order by a.create_time asc");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        #endregion

        #region 排放类别
        /// <summary>
        /// 新增排放类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddClass(T_Com_Class data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新排放类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateClass(T_Com_Class data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除排放类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteClass(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_class set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询排放类别
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Class> SelectClass(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_class  where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<Out_Class>>();

                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 分页查询排放类别
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_Class>> SelectClassPage(In_ClassPageLis query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_Class>().Append(@"select c.* from (
                                                                                    select a.*,(
                                                                                    select GROUP_CONCAT(b.SubClassName)
                                                                                    from t_com_subclass b where a.Id=b.ClassId and b.del_flag='0'
                                                                                    ) SubClassName
                                                                                    from t_com_class a
                                                                                    where a.del_flag='0' ) c")
                                        .Then(!string.IsNullOrEmpty(query.SubClassName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.SubClassName) + "%";
                                            sq.Append(" where c.SubClassName like ").AppendParam(tmpkey);
                                        })
                                        .Append(" order by c.ClassNo asc");

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        /// <summary>
        /// 查询排放类别
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Class>> SelectClassAll()
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_class  where del_flag='0' order by ClassNo asc ")
                                       .DoAsync<DoQuerySql<Out_Class>>();

                return docmd.ToList();
            }
        }

        #region 子排放类型
        /// <summary>
        /// 新增子排放类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddSubClass(T_Com_SubClass data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新子排放类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateSubClass(T_Com_SubClass data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除子排放类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteSubClass(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_subclass set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询排放类别的子排放类别
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_SubClass>> SelectSubClass(string ClassId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_subclass  where del_flag='0' and ClassId = ").AppendParam(ClassId)
                                       .DoAsync<DoQuerySql<T_Com_SubClass>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询子排放类别
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_SubClass> SelectSubClassInfo(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_subclass  where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Com_SubClass>>();

                return docmd.ToFirst();
            }
        }
        #endregion

        #endregion

        #region 企业排放类别
        /// <summary>
        /// 新增排企业排放类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddOrgClass(T_Com_OrgClass data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新企业排放类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateOrgClass(T_Com_OrgClass data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除企业排放类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteOrgClass(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_orgclass set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询企业排放类别
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_OrgClass>> SelectOrgClass(long? OrgId, string ClassId)
        {
            using (DbHelp db = CreateDB()) 
            {
                var docmd = await new SqlBuilder(db).Append(@"select a.*,c.SubClassName,d.FacilityName,e.FactorName,e.EmissionFactor,f.TypeName,f.FactorUnit,f.ActivityUnit
                                                                 from t_com_orgclass a  
                                                                 left join t_com_subclass c on a.SubClassId = c.Id
                                                                 left join t_com_facility d on a.FacilityId = d.Id
                                                                 left join t_eng_factor e on a.FactorId = e.Id
                                                                 left join t_eng_factortype f on a.FactorType = f.Id
                                                                 where a.del_flag='0' and a.OrgId = ").AppendParam(OrgId)
                                        .Append(" and a.ClassId=").AppendParam(ClassId)
                                       .DoAsync<DoQuerySql<T_OrgClass>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询企业排放类别
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_OrgClass> SelectOrgClassInfo(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_orgclass  where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Com_OrgClass>>();

                return docmd.ToFirst();
            }
        }

        #endregion

        #region 环节
        /// <summary>
        /// 查询生产环节
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_link>> SelectLink(string ProductBorder)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_link where ProductBorder = ").AppendParam(ProductBorder)
                                       .DoAsync<DoQuerySql<Out_link>>();

                return docmd.ToList();
            }
        }
        /// <summary>
        /// 查询生产环节
        /// </summary>
        /// <returns></returns>
        public async Task<Out_link> SelectLinkInfo(string LinkId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_link where LinkId = ").AppendParam(LinkId)
                                       .DoAsync<DoQuerySql<Out_link>>();

                return docmd.ToFirst();
            }
        }
        #endregion

        #region 产品生命周期模型
        /// <summary>
        /// 新增生命周期模型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddModel(T_Com_Model data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新生命周期模型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateModel(T_Com_Model data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除生命周期模型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> DeleteModel(string id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_model set del_flag='2' where Id=").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 分页查询生命周期模型
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_ModelList>> SelectModelPage(In_ModelPageLis query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_ModelList>().Append(@"select a.*,c.RealName as 'createName',d.RealName as 'updateName',b.ProductModel,b.ProductName,
                                                   (select BorderTitle from t_com_link where ProductBorder=a.ProductBorder LIMIT 1) BorderTitle,
                                                    (select BorderName from t_com_link where ProductBorder=a.ProductBorder LIMIT 1) BorderName
                                                                            from t_com_model a
                                                                            left join t_com_product b on b.Id = a.ProductId
                                                                            left join mz_admin c on a.createId = c.id 
                                                                            left join mz_admin d on a.updateId = d.id 
                                                                                    where a.del_flag='0' ")
                                        .Then(!string.IsNullOrEmpty(query.ProductName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProductName) + "%";
                                            sq.Append(" and b.ProductName like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductModel), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProductModel) + "%";
                                            sq.Append(" and b.ProductModel like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductBorder), sq =>
                                        {
                                            sq.Append(" and a.ProductBorder = ").AppendParam(query.ProductBorder);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductId), sq =>
                                        {
                                            sq.Append(" and a.ProductId = ").AppendParam(query.ProductId);
                                        })
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(query.beginTime != null, sq =>
                                        {
                                            sq.Append(" and a.beginTime >= ").AppendParam(query.beginTime);
                                        })
                                        .Then(query.endTime != null, sq =>
                                        {
                                            sq.Append(" and a.endTime <= ").AppendParam(query.endTime);
                                        })
                                        .Then(query.createId != null, sq =>
                                        {
                                            sq.Append(" and a.createId <= ").AppendParam(query.createId);
                                        });

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        /// <summary>
        /// 查询生命周期模型
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_Model> SelectModel(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_model where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Com_Model>>();

                return docmd.ToFirst();
            }
        }

        #endregion

        #region 产品工序
        /// <summary>
        /// 新增产品工序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddProcess(T_Com_Process data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新产品工序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateProcess(T_Com_Process data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除产品工序
        /// </summary>
        /// <param name="ModelId"></param>
        /// <returns></returns>
        public async Task<int> DeleteProcess(string ModelId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_com_process where ModelId=").AppendParam(ModelId).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询生命周期模型
        /// </summary>
        /// <returns></returns>
        public async Task<List<T_Com_Process>> SelectProcess(string ModelId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_process where ModelId = ").AppendParam(ModelId)
                                       .DoAsync<DoQuerySql<T_Com_Process>>();

                return docmd.ToList();
            }
        }
        /// <summary>
        /// 新增产品工序设施
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddProcessItem(T_Com_ProcessItem data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除产品工序设施
        /// </summary>
        /// <param name="ModelId">模型编码</param>
        /// <returns></returns>
        public async Task<int> DeleteProcessItem(string ModelId)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("delete from t_com_process where ModelId=").AppendParam(ModelId).DoAsync<DoExecSql>()).RowCount;
            }
        }

        /// <summary>
        /// 查询产品工序设施
        /// </summary>
        /// <param name="ProcessId">工序编码</param>
        /// <returns></returns>
        public async Task<List<T_Com_ProcessItem>> SelectProcessItem(string ProcessId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_processitem where ProcessId = ").AppendParam(ProcessId)
                                       .DoAsync<DoQuerySql<T_Com_ProcessItem>>();

                return docmd.ToList();
            }
        }

        /// <summary>
        /// 查询产品工序设施
        /// </summary>
        /// <param name="FacilityId">编码</param>
        /// <returns></returns>
        public async Task<List<ShuRu>> SelectProcessEnergy(string FacilityId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append(@"select b.EnergyType,c.TypeName,c.FactorUnit from 
                                t_com_equipment a
                                left join t_price_policy b on a.PolicyId = b.Id
                                left join t_eng_factortype c on b.EnergyType = c.Id
                                where c.Id is not null  and  a.FacilityId in (")
                                .Append(FacilityId).Append(") group by b.EnergyType,c.TypeName,c.FactorUnit")
                                       .DoAsync<DoQuerySql<ShuRu>>();

                return docmd.ToList();
            }
        }

        #endregion

        #region 产品碳足迹
        /// <summary>
        /// 新增产品碳足迹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> AddProductModel(T_Com_ProductModel data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(data).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 更新产品碳足迹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<int> UpdateProductModel(T_Com_ProductModel data)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(data, "Id=").AppendParam(data.Id).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        /// <summary>
        /// 删除产品碳足迹
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<int> DeleteProductModel(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("update t_com_productmodel set del_flag='2' where Id=").AppendParam(Id).DoAsync<DoExecSql>()).RowCount;
            }
        }
        /// <summary>
        /// 分页查询产品碳足迹
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<Out_ProductModelList>> SelectProductModelPage(In_ModelPageLis query)
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = new SqlBuilder(db).Query<Out_ProductModelList>().Append(@"select a.*,c.RealName as 'createName',d.RealName as 'updateName',b.ProductModel,b.ProductName,b.ProductType,b.Unit,
                                        (select BorderTitle from t_com_link where ProductBorder=e.ProductBorder LIMIT 1) BorderTitle,
                                        (select BorderName from t_com_link where ProductBorder=e.ProductBorder LIMIT 1) BorderName,
                                        (select ROUND(SUM(t1.OutPut),1) from t_prod_production t1 where t1.ProductId = b.Id and t1.DDate >= a.BeginDate  and t1.DDate <= a.EndDate and FIND_IN_SET(t1.FacilityId,
                                        (select GROUP_CONCAT(t2.FacilityIds) from t_com_processitem t2 where ModelId=a.ModelId))) OutPut ")
                                        .Then(!string.IsNullOrEmpty(query.CarbonEmission), sq =>
                                        {
                                            sq.Append(@" ,(select ROUND(SUM(t1.CarbonEmission),1) from t_prod_energy t1 left join t_com_equipment t3 on t1.EquipmentId=t3.Id where t1.DDate >= a.BeginDate  and t1.DDate <= a.EndDate and FIND_IN_SET(t3.FacilityId,
                                        (select GROUP_CONCAT(t2.FacilityIds) from t_com_processitem t2 where ModelId=a.ModelId))) CarbonEmission ");
                                        })
                                        .Append(@" from t_com_productmodel a
                                        left join mz_admin c on a.createId = c.id 
                                        left join mz_admin d on a.updateId = d.id 
                                        left join t_com_model e on a.ModelId=e.Id
                                        left join t_com_product b on b.Id = e.ProductId
                                        where a.del_flag='0' ")
                                        .Then(!string.IsNullOrEmpty(query.ProductName), sq =>
                                        {
                                            string tmpkey = "%" + Common.StringHelper.SqlLikeFilter(query.ProductName) + "%";
                                            sq.Append(" and b.ProductName like ").AppendParam(tmpkey);
                                        })
                                        .Then(!string.IsNullOrEmpty(query.ProductBorder), sq =>
                                        {
                                            sq.Append(" and e.ProductBorder = ").AppendParam(query.ProductBorder);
                                        })
                                        .Then(query.OrgId != null, sq =>
                                        {
                                            sq.Append(" and a.OrgId = ").AppendParam(query.OrgId);
                                        })
                                        .Then(query.beginTime != null, sq =>
                                        {
                                            sq.Append(" and a.beginTime >= ").AppendParam(query.beginTime);
                                        })
                                        .Then(query.endTime != null, sq =>
                                        {
                                            sq.Append(" and a.endTime <= ").AppendParam(query.endTime);
                                        })
                                         .Then(query.beginDate != null, sq =>
                                         {
                                             sq.Append(" and a.BeginDate >= ").AppendParam(query.beginDate);
                                         })
                                        .Then(query.endDate != null, sq =>
                                        {
                                            sq.Append(" and a.EndDate <= ").AppendParam(query.endDate);
                                        })
                                        .Then(query.createId != null, sq =>
                                        {
                                            sq.Append(" and a.createId <= ").AppendParam(query.createId);
                                        });

                return await tsql.GeneratePageObjectAsync(query);
            }
        }

        /// <summary>
        /// 查询产品碳足迹
        /// </summary>
        /// <returns></returns>
        public async Task<T_Com_ProductModel> SelectProductModel(string Id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from t_com_productmodel where Id = ").AppendParam(Id)
                                       .DoAsync<DoQuerySql<T_Com_ProductModel>>();

                return docmd.ToFirst();
            }
        }

        #endregion
    }
}
