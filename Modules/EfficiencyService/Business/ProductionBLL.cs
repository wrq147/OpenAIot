using AfterService.DAL;
using AfterService.Model;
using AuthService;
using AuthService.DAL;
using AuthService.Model;
using ChannelUtility;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using EfficiencyService.Controller;
using EfficiencyService.DAL;
using EfficiencyService.Model;
using EfficiencyService.Model.Common;
using EfficiencyService.Model.Org;
using EfficiencyService.Model.Production;
using Google.Protobuf.WellKnownTypes;
using InfluxDB.Client.Api.Domain;
using IoTService.Business;
using IoTService.DAL;
using IoTService.Models;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Minio.DataModel;
using MyAccess.DB.Builder.WhereToSql;
using NodaTime;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Functions;
using Quartz;
using Quartz.Impl.AdoJobStore.Common;
using RabbitMQ.Client;
using SixLabors.Fonts;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label.Element;
using static NPOI.POIFS.Crypt.CryptoFunctions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EfficiencyService.Business
{
    public class ProductionBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private ProductionDAL _productionDAL;
        private CommonDAL _commonDAL;
        private PolicyDAL _policyDAL;
        private FactorDAL _factorDAL;
        private OrgConfDAL _orgConfDAL;
        private IotInfluxBLL _iotInflux;
        private IotDeviceDAL _deviceDAL;
        private IotWarningDAL _warningDAL;
        private UserDAL _userDAL;
        private CodeDAL _codeDAL;

        public ProductionBLL(ITAServiceProvider provider, CodeDAL codeDAL, UserDAL userDAL, IotDeviceDAL deviceDAL, IotWarningDAL warningDAL, ProductionDAL productionDAL, CommonDAL commonDAL, PolicyDAL policyDAL, FactorDAL factorDAL, OrgConfDAL orgConfDAL, IotInfluxBLL iotInflux, SnowflakeHelper snowflake)
        {
            _provider = provider;
            _snowflake = snowflake;
            _productionDAL = productionDAL;
            _commonDAL = commonDAL;
            _policyDAL = policyDAL;
            _factorDAL = factorDAL;
            _orgConfDAL = orgConfDAL;
            _iotInflux = iotInflux;
            _deviceDAL = deviceDAL;
            _warningDAL = warningDAL;
            _userDAL = userDAL;
            _codeDAL = codeDAL;
        }

        #region 生产数据采集
        /// <summary>
        /// 新增生产数据
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddProduction(In_Production in_Production)
        {
            T_Com_Product product = await _commonDAL.SelectProduct(in_Production.ProductId);
            if (product == null)
            {
                return BusResponse<string>.Error(500, "找不到产品");
            }

            T_Com_Facility facility = await _commonDAL.SelectFacility(in_Production.FacilityId);
            if (facility == null)
            {
                return BusResponse<string>.Error(500, "找不到设施");
            }
            DateTime ddate = DateTime.Now;
            if (!DateTime.TryParse(in_Production.DDate, out ddate))
            {
                return BusResponse<string>.Error(500, "日期错误");
            }

            var user = _provider.GetUser();

            T_Prod_Production production = new T_Prod_Production();
            production.Id = _snowflake.NextId().ToString();

            production.OrgId = in_Production.OrgId;
            production.ProductId = in_Production.ProductId;
            production.DDate = ddate;
            production.OutPut = in_Production.OutPut;
            production.FacilityId = in_Production.FacilityId;

            production.Price = product.ProductPrice;
            production.Unit = product.Unit;
            production.OutValue = Math.Round(in_Production.OutPut * production.Price, 2);

            production.del_flag = "0";
            production.createId = user.UserId;
            production.create_time = DateTime.Now;
            production.updateId = user.UserId;
            production.update_time = DateTime.Now;
            await _productionDAL.AddProduction(production);
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 导入生产数据
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ImportProduction(List<In_Production> productions)
        {
            var user = _provider.GetUser();
            List<T_Com_Facility> Facilitys = await _commonDAL.SelectFacilityTree(user.OrgId);
            In_ProductPageList query = new In_ProductPageList();
            query.OrgId = user.OrgId;
            List<T_Com_Product> Products = (await _commonDAL.SelectProductPage(query)).List;

            Dictionary<string, T_Com_Facility> dicFacilitys = new Dictionary<string, T_Com_Facility>();
            Dictionary<string, T_Com_Product> dicProducts = new Dictionary<string, T_Com_Product>();
            //先验证一遍
            foreach (var in_Production in productions)
            {
                if (!dicProducts.ContainsKey(in_Production.ProductId))
                {
                    bool flg = true;
                    foreach (var item in Products)
                    {
                        if (item.ProductName == in_Production.ProductId)
                        {
                            flg = false;
                            dicProducts.Add(in_Production.ProductId, item);
                        }
                    }
                    if (flg)
                    {
                        return BusResponse<string>.Error(500, "找不到产品：" + in_Production.ProductId);
                    }
                }
                if (!dicFacilitys.ContainsKey(in_Production.FacilityId))
                {
                    bool flg = true;
                    foreach (var item in Facilitys)
                    {
                        if (item.FacilityName == in_Production.FacilityId)
                        {
                            flg = false;
                            dicFacilitys.Add(in_Production.FacilityId, item);
                        }
                    }
                    if (flg)
                    {
                        return BusResponse<string>.Error(500, "找不到设施：" + in_Production.FacilityId);
                    }
                }

                DateTime ddate = DateTime.Now;
                if (!DateTime.TryParse(in_Production.DDate, out ddate))
                {
                    try
                    {
                        double x = double.Parse(in_Production.DDate);
                        DateTime.FromOADate(x);
                    }
                    catch (Exception)
                    {
                        return BusResponse<string>.Error(500, "日期错误");
                    }
                }
               
            }
           
            foreach (var in_Production in productions)
            {
                T_Com_Product product = dicProducts[in_Production.ProductId];

                T_Com_Facility facility = dicFacilitys[in_Production.FacilityId];

                DateTime ddate = DateTime.Now;
                if (!DateTime.TryParse(in_Production.DDate, out ddate))
                {
                    try
                    {
                        double x = double.Parse(in_Production.DDate);
                        ddate =DateTime.FromOADate(x);
                    }
                    catch (Exception)
                    {
                        return BusResponse<string>.Error(500, "日期错误");
                    }
                }

                T_Prod_Production production = new T_Prod_Production();
                production.Id = _snowflake.NextId().ToString();

                production.OrgId = user.OrgId;
                production.ProductId = product.Id;
                production.DDate = ddate;
                production.OutPut = in_Production.OutPut;
                production.FacilityId = facility.Id;

                production.Price = product.ProductPrice;
                production.Unit = product.Unit;
                production.OutValue = Math.Round(in_Production.OutPut * production.Price, 2);

                production.del_flag = "0";
                production.createId = user.UserId;
                production.create_time = DateTime.Now;
                production.updateId = user.UserId;
                production.update_time = DateTime.Now;
                await _productionDAL.AddProduction(production);
            }
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除生产数据
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteProduction(string Id)
        {
            await _productionDAL.DeleteProduction(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改生产数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateProduction(In_ProductionUpdate in_Production)
        {
            T_Prod_Production production = await _productionDAL.SelectProduction(in_Production.Id);
            if (production == null)
            {
                return BusResponse<int>.Error(500, "找不到记录");
            }

            T_Com_Product product = await _commonDAL.SelectProduct(production.ProductId);
            if (product == null)
            {
                return BusResponse<int>.Error(500, "找不到产品");
            }

            T_Com_Facility facility = await _commonDAL.SelectFacility(production.FacilityId);
            if (facility == null)
            {
                return BusResponse<int>.Error(500, "找不到设施");
            }

            var user = _provider.GetUser();

            production.OutPut = in_Production.OutPut;

            production.Price = product.ProductPrice;
            production.Unit = product.Unit;
            production.OutValue = Math.Round(in_Production.OutPut * production.Price, 2);

            production.updateId = user.UserId;
            production.update_time = DateTime.Now;
            await _productionDAL.UpdateProduction(production);
            return BusResponse<int>.Success();
        }

        /// <summary>
        /// 分页查询生产数据列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_Production>> SelectProductionList(InProductionPageList query)
        {
            //查询子设施
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }

            return await _productionDAL.SelectProductionPage(query);
        }

        /// <summary>
        /// 分页查询设施每日每月每年生产记录
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_ProductionDay>> SelectProductionDateList(InProductionDayPageList query)
        {
            if (!(query.DateType == "日" || query.DateType == "月" || query.DateType == "年"))
            {
                return PageObject<Out_ProductionDay>.Empty();
            }
            //查询子设施
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }

            return await _productionDAL.SelectProductionDatePage(query);
        }

        /// <summary>
        /// 查询生产数据
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Production> SelectProduction(string Id)
        {
            return await _productionDAL.SelectProduction(Id);
        }
        #endregion

        #region 设备能耗采集
        /// <summary>
        /// 新增设备能耗
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddEnergyHand(In_EnergyHand in_Energy)
        {
            //设备信息
            T_Com_Equipment equipment = await _commonDAL.SelectEquipment(in_Energy.EquipmentId);
            if (equipment == null)
            {
                return BusResponse<string>.Error(500, "找不到设备");
            }

            //计费政策
            T_Price_Policy policy = await _policyDAL.SelectPolicy(equipment.PolicyId);
            if (policy == null)
            {
                return BusResponse<string>.Error(500, "找不到计费标准");
            }

            //计费明细
            List<T_Price_PolicyDetil> policyDetils = await _policyDAL.SelectPolicyDetil(policy.Id);
            double price = 0;
            foreach (var item in policyDetils)
            {
                if (item.PolicyType == "2")//全天
                {
                    //全天价格,只有一条
                    T_Price_Period periods = (await _policyDAL.SelectPeriod(item.Id)).First();
                    if (periods == null)
                    {
                        return BusResponse<string>.Error(500, "找不到计费标准");
                    }
                    else
                    {
                        price = periods.Price;
                    }
                }
            }

            //排放因子
            T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
            if (factor == null)
            {
                return BusResponse<string>.Error(500, "找不到排放因子");
            }

            //能源类型
            T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
            if (factorType == null)
            {
                return BusResponse<string>.Error(500, "找不到能源类型");
            }

            var user = _provider.GetUser();

            T_Prod_Energy energy = new T_Prod_Energy();
            energy.Id = _snowflake.NextId().ToString();

            energy.OrgId = in_Energy.OrgId;
            energy.EquipmentId = in_Energy.EquipmentId;
            energy.DDate = DateTime.Parse(in_Energy.DDate);
            energy.InitVale = in_Energy.InitVale;
            energy.EndVale = in_Energy.EndVale;
            energy.UseVale = Math.Round(in_Energy.EndVale - in_Energy.InitVale,2);
            energy.CostVale = Math.Round(price * (in_Energy.EndVale - in_Energy.InitVale),2);

            energy.PolicyId = equipment.PolicyId;
            energy.PolicyName = policy.PolicyName;

            energy.FactorId = factorType.Id;
            energy.FactorName = factorType.TypeName;

            energy.DataSource = "1";
            energy.Memo = "-";
            energy.Unit = policy.Unit;
            energy.LageUnit = policy.LageUnit;
            energy.CarbonEmission = Math.Round(factor.AvgCalorific * (in_Energy.EndVale - in_Energy.InitVale),2);
            energy.ConvertCoal = Math.Round(factor.EqCoal * (in_Energy.EndVale - in_Energy.InitVale),2);
            energy.TimePeriod = "-";

            energy.del_flag = "0";
            energy.createId = user.UserId;
            energy.create_time = DateTime.Now;
            energy.updateId = user.UserId;
            energy.update_time = DateTime.Now;
            await _productionDAL.AddEnergy(energy);
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 新增设备能耗
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ImportEnergy(List<In_EnergyHand> list)
        {
            var user = _provider.GetUser();
            List<T_Com_Equipment> Equipments = await _commonDAL.SelectEquipmentList(user.OrgId);
            In_PolicyList query = new In_PolicyList();
            query.OrgId = user.OrgId;
            List<Out_Policy> Policys = (await _policyDAL.SelectPolicy(query)).List;

            Dictionary<string, T_Com_Equipment> dicEquipments = new Dictionary<string, T_Com_Equipment>();
            Dictionary<string, T_Price_Policy> dicPolicy = new Dictionary<string, T_Price_Policy>();
            Dictionary<string, List<T_Price_PolicyDetil>> dicDetils = new Dictionary<string, List<T_Price_PolicyDetil>>();
            Dictionary<string, T_ENG_Factor> dicFactor = new Dictionary<string, T_ENG_Factor>();
            Dictionary<string, T_ENG_FactorType> dicFactorType = new Dictionary<string, T_ENG_FactorType>();
            foreach (var in_Energy in list)
            {
                //设备信息
                T_Com_Equipment equipment = null;
                foreach (var item in Equipments)
                {
                    if (item.EquipmentName == in_Energy.EquipmentId)
                    {
                        equipment = item;
                        if (!dicEquipments.ContainsKey(item.EquipmentName))
                        {
                            dicEquipments.Add(item.EquipmentName, item);
                        }
                    }
                }
                if (equipment == null)
                {
                    return BusResponse<string>.Error(500, "找不到设备");
                }

                //计费政策
                T_Price_Policy policy = null;
                foreach (var item in Policys)
                {
                    if (item.Id == equipment.PolicyId)
                    {
                        policy = item;
                        if (!dicPolicy.ContainsKey(item.Id))
                        {
                            dicPolicy.Add(item.Id, item);
                        }
                    }
                }
                if (policy == null)
                {
                    return BusResponse<string>.Error(500, "找不到计费标准");
                }

                //计费明细
                List<T_Price_PolicyDetil> policyDetils = null;
                if (!dicDetils.ContainsKey(policy.Id))
                {
                    policyDetils = await _policyDAL.SelectPolicyDetil(policy.Id);
                    dicDetils.Add(policy.Id, policyDetils);
                }
                else
                {
                    policyDetils = dicDetils[policy.Id];
                }
                double price = 0;
                foreach (var item in policyDetils)
                {
                    if (item.PolicyType == "2")//全天
                    {
                        //全天价格,只有一条
                        T_Price_Period periods = (await _policyDAL.SelectPeriod(item.Id)).First();
                        if (periods == null)
                        {
                            return BusResponse<string>.Error(500, "找不到计费标准");
                        }
                        else
                        {
                            price = periods.Price;
                        }
                    }
                }

                //排放因子
                T_ENG_Factor factor = null;
                if (!dicFactor.ContainsKey(policy.FactorId))
                {
                    factor = await _factorDAL.SelectFactor(policy.FactorId);
                    dicFactor.Add(policy.FactorId, factor);
                }
                else
                {
                    factor = dicFactor[policy.FactorId];
                }
                if (factor == null)
                {
                    return BusResponse<string>.Error(500, "找不到排放因子");
                }

                //能源类型
                T_ENG_FactorType factorType = null;
                if (!dicFactorType.ContainsKey(policy.EnergyType))
                {
                    factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                    dicFactorType.Add(policy.EnergyType, factorType);
                }
                else
                {
                    factorType = dicFactorType[policy.EnergyType];
                }
                if (factorType == null)
                {
                    return BusResponse<string>.Error(500, "找不到能源类型");
                }

                DateTime ddate = DateTime.Now;
                if (!DateTime.TryParse(in_Energy.DDate, out ddate))
                {
                    try
                    {
                        double x = double.Parse(in_Energy.DDate);
                        DateTime.FromOADate(x);
                    }
                    catch (Exception)
                    {
                        return BusResponse<string>.Error(500, "日期错误");
                    }
                }
            }

            foreach (var in_Energy in list)
            {
                //设备信息
                T_Com_Equipment equipment = dicEquipments[in_Energy.EquipmentId];
                if (equipment == null)
                {
                    return BusResponse<string>.Error(500, "找不到设备");
                }

                //计费政策
                T_Price_Policy policy = dicPolicy[equipment.PolicyId];
                if (policy == null)
                {
                    return BusResponse<string>.Error(500, "找不到计费标准");
                }

                //计费明细
                List<T_Price_PolicyDetil> policyDetils = dicDetils[policy.Id];
                double price = 0;
                foreach (var item in policyDetils)
                {
                    if (item.PolicyType == "2")//全天
                    {
                        //全天价格,只有一条
                        T_Price_Period periods = (await _policyDAL.SelectPeriod(item.Id)).First();
                        if (periods == null)
                        {
                            return BusResponse<string>.Error(500, "找不到计费标准");
                        }
                        else
                        {
                            price = periods.Price;
                        }
                    }
                }

                //排放因子
                T_ENG_Factor factor = dicFactor[policy.FactorId];
                if (factor == null)
                {
                    return BusResponse<string>.Error(500, "找不到排放因子");
                }

                //能源类型
                T_ENG_FactorType factorType = dicFactorType[policy.EnergyType];
                if (factorType == null)
                {
                    return BusResponse<string>.Error(500, "找不到能源类型");
                }

                DateTime ddate = DateTime.Now;
                if (!DateTime.TryParse(in_Energy.DDate, out ddate))
                {
                    try
                    {
                        double x = double.Parse(in_Energy.DDate);
                        ddate = DateTime.FromOADate(x);
                    }
                    catch (Exception)
                    {
                        return BusResponse<string>.Error(500, "日期错误");
                    }
                }

                T_Prod_Energy energy = new T_Prod_Energy();
                energy.Id = _snowflake.NextId().ToString();

                energy.OrgId = user.OrgId;
                energy.EquipmentId = equipment.Id;
                energy.DDate = ddate;
                energy.InitVale = in_Energy.InitVale;
                energy.EndVale = in_Energy.EndVale;
                energy.UseVale = Math.Round(in_Energy.EndVale - in_Energy.InitVale, 2);
                energy.CostVale = Math.Round(price * (in_Energy.EndVale - in_Energy.InitVale), 2);

                energy.PolicyId = equipment.PolicyId;
                energy.PolicyName = policy.PolicyName;

                energy.FactorId = factorType.Id;
                energy.FactorName = factorType.TypeName;

                energy.DataSource = "1";
                energy.Memo = "-";
                energy.Unit = policy.Unit;
                energy.LageUnit = policy.LageUnit;
                energy.CarbonEmission = Math.Round(factor.AvgCalorific * (in_Energy.EndVale - in_Energy.InitVale), 2);
                energy.ConvertCoal = Math.Round(factor.EqCoal * (in_Energy.EndVale - in_Energy.InitVale), 2);
                energy.TimePeriod = "-";

                energy.del_flag = "0";
                energy.createId = user.UserId;
                energy.create_time = DateTime.Now;
                energy.updateId = user.UserId;
                energy.update_time = DateTime.Now;
                await _productionDAL.AddEnergy(energy);
            }
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除设备能耗
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteEnergy(string Id)
        {
            await _productionDAL.DeleteEnergy(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改设备能耗
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateEnergy(In_EnergyHandUpdate in_Energy)
        {
            T_Prod_Energy energy = await _productionDAL.SelectEnergy(in_Energy.Id);

            if (energy == null)
            {
                return BusResponse<int>.Error(500, "找不到记录");
            }

            //设备信息
            T_Com_Equipment equipment = await _commonDAL.SelectEquipment(energy.EquipmentId);
            if (equipment == null)
            {
                return BusResponse<int>.Error(500, "找不到设备");
            }

            //计费政策
            T_Price_Policy policy = await _policyDAL.SelectPolicy(energy.PolicyId);
            if (equipment == null)
            {
                return BusResponse<int>.Error(500, "找不到计费标准");
            }

            //计费明细
            List<T_Price_PolicyDetil> policyDetils = await _policyDAL.SelectPolicyDetil(policy.Id);
            double price = 0;
            foreach (var item in policyDetils)
            {
                if (item.PolicyType == "2")//全天
                {
                    //全天价格,只有一条
                    T_Price_Period periods = (await _policyDAL.SelectPeriod(item.Id)).First();
                    if (periods == null)
                    {
                        return BusResponse<int>.Error(500, "找不到计费标准");
                    }
                    else
                    {
                        price = periods.Price;
                    }
                }
            }

            //排放因子
            T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
            if (factor == null)
            {
                return BusResponse<int>.Error(500, "找不到排放因子");
            }

            //能源类型
            T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
            if (factorType == null)
            {
                return BusResponse<int>.Error(500, "找不到能源类型");
            }

            var user = _provider.GetUser();

            energy.InitVale = in_Energy.InitVale;
            energy.EndVale = in_Energy.EndVale;
            energy.UseVale = Math.Round(in_Energy.EndVale - in_Energy.InitVale,2);
            energy.CostVale = Math.Round(price * (in_Energy.EndVale - in_Energy.InitVale),2);

            energy.CarbonEmission = Math.Round(factor.EmissionFactor * (in_Energy.EndVale - in_Energy.InitVale),2);
            energy.ConvertCoal = Math.Round((factor.AvgCalorific * (in_Energy.EndVale - in_Energy.InitVale)) / factor.EqCoal,2);

            energy.updateId = user.UserId;
            energy.update_time = DateTime.Now;
            await _productionDAL.UpdateEnergy(energy);
            return BusResponse<int>.Success();
        }

        /// <summary>
        /// 分页查询设备能耗列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_Energy>> SelectEnergyList(InEnergyPageList query)
        {
            //查询子设施
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }

            return await _productionDAL.SelectEnergyPage(query);
        }

        /// <summary>
        /// 分页查询设备能耗列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_Energy>> SelectEnergyTimeList(InEnergyPageList query)
        {
            //查询子设施
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }

            return await _productionDAL.SelectEnergyPage(query);
        }

        /// <summary>
        /// 分页查询设备每日每月每年能耗采集记录
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_EnergyDay>> SelectEnergyDateList(InEnergyDayPageList query)
        {
            if (!(query.DateType == "日" || query.DateType == "月" || query.DateType == "年"))
            {
                 return PageObject<Out_EnergyDay>.Empty();
            }
            //查询子设施
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }

            return await _productionDAL.SelectEnergyDatePage(query);
        }

        private static string DeppIDS(string ParentId, List<T_Com_Facility> data)
        {
            string Ids = "";
            foreach (var item in data)
            {
                if (item.ParentId == ParentId)
                {
                    Ids += item.Id + ",";
                    Ids += DeppIDS(item.Id, data);
                }
            }
            return Ids;
        }

        /// <summary>
        /// 分页查询设施每日每月每年能耗采集记录
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_FacilityDay>> SelectFacilityDateList(InEnergyDayPageList query)
        {
            if (!(query.DateType == "日" || query.DateType == "月" || query.DateType == "年"))
            {
                return PageObject<Out_FacilityDay>.Empty();
            }
            string facilityIds = "";
            if (!string.IsNullOrEmpty(query.FacilityId) && query.OrgId != null)
            {
                List<T_Com_Facility> list = await _commonDAL.SelectFacilityTree((long)query.OrgId);

                facilityIds = DeppIDS(query.FacilityId, list);

                query.FacilityId = facilityIds + query.FacilityId;
            }
            return await _productionDAL.SelectFacilityEnergyDatePage(query);
        }

        


        /// <summary>
        /// 查询设备能耗
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Energy> SelectEnergy(string Id)
        {
            return await _productionDAL.SelectEnergy(Id);
        }

        /// <summary>
        /// 查询企业能流图
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_FacilityEnergy>> SeletEnergyTree(InEnergyTree query)
        {
            //设施列表
            List<T_Com_Facility> listFacility = await _commonDAL.SelectFacilityTree((long)query.OrgId);

            //能耗数据
            List<Out_EnergyList> listEnergies =  await _productionDAL.SelectEnergyList(query);

            //返回的能流图树
            List<Out_FacilityEnergy> data = EnergyTree("-", listFacility, listEnergies);

            return data;
        }

        private static List<Out_FacilityEnergy> EnergyTree(string ParentId, List<T_Com_Facility> listFacility, List<Out_EnergyList> listEnergies)
        {
            List<Out_FacilityEnergy> list = new List<Out_FacilityEnergy>();
            foreach (var item in listFacility)
            {
                if (item.ParentId == ParentId)
                {
                    Out_FacilityEnergy ft = new Out_FacilityEnergy();
                    ft.Id = item.Id;
                    ft.FacilityName = item.FacilityName;
                    ft.Manager = item.Manager;
                    ft.Contact = item.Contact;
                    ft.ParentId = item.ParentId;
                    ft.FacilityType = item.FacilityType;
                    ft.EquipmentType = false;
                    ft.UseVale = 0;
                    ft.CostVale = 0;
                    ft.CarbonEmission = 0;
                    ft.ConvertCoal = 0;
                    //计算设施下的设备的能耗
                    if (ft.FacilityType)
                    {
                        ft.Children = new List<Out_FacilityEnergy>();
                        Dictionary<string, Out_EquipmentEnergy> Equipments = new Dictionary<string, Out_EquipmentEnergy>();
                        List<string> arr = new List<string>();
                        foreach (var jtem in listEnergies)
                        {
                            if (jtem.FacilityId == item.Id)
                            {
                                ft.UseVale += jtem.UseVale;
                                ft.CostVale += jtem.CostVale;
                                ft.CarbonEmission += jtem.CarbonEmission;
                                ft.ConvertCoal += jtem.ConvertCoal;
                                ft.Unit = jtem.Unit;
                                ft.LageUnit = jtem.LageUnit;

                                Out_FacilityEnergy facilityEnergy = new Out_FacilityEnergy();
                                facilityEnergy.Id = jtem.EquipmentId;
                                facilityEnergy.FacilityName = jtem.EquipmentName;
                                facilityEnergy.Manager = "";
                                facilityEnergy.Contact = "";
                                facilityEnergy.ParentId = ft.Id;
                                facilityEnergy.FacilityType = ft.FacilityType;
                                facilityEnergy.FactorName = jtem.FactorName;
                                facilityEnergy.UseVale = Math.Round(jtem.UseVale,2);
                                facilityEnergy.CostVale = Math.Round(jtem.CostVale,2);
                                facilityEnergy.CarbonEmission = Math.Round(jtem.CarbonEmission,2);
                                facilityEnergy.ConvertCoal = Math.Round(jtem.ConvertCoal,2);
                                facilityEnergy.Memo = jtem.Memo;
                                facilityEnergy.Unit = jtem.Unit;
                                facilityEnergy.LageUnit = jtem.LageUnit;
                                facilityEnergy.EquipmentType = true;

                                if (arr.Contains(jtem.EquipmentId))
                                {
                                    foreach (var ptem in ft.Children)
                                    {
                                        if (ptem.Id == jtem.EquipmentId)
                                        {
                                            ptem.UseVale = Math.Round(ptem.UseVale + facilityEnergy.UseVale, 2);
                                            ptem.CostVale = Math.Round(ptem.CostVale + facilityEnergy.CostVale, 2);
                                            ptem.CarbonEmission = Math.Round(ptem.CarbonEmission + facilityEnergy.CarbonEmission, 2);
                                            ptem.ConvertCoal = Math.Round(ptem.ConvertCoal + facilityEnergy.ConvertCoal, 2);
                                        }
                                    }
                                }
                                else 
                                {
                                    arr.Add(jtem.EquipmentId);
                                    ft.Children.Add(facilityEnergy);
                                }
                                    
                                /*设备先不管了
                                //统计到设备
                                if (Equipments.ContainsKey(jtem.EquipmentId))
                                {
                                    Equipments[jtem.EquipmentId].UseVale = Math.Round(Equipments[jtem.EquipmentId].UseVale + jtem.UseVale, 2);
                                    Equipments[jtem.EquipmentId].CostVale = Math.Round(Equipments[jtem.EquipmentId].CostVale + jtem.CostVale, 2);
                                    Equipments[jtem.EquipmentId].CarbonEmission = Math.Round(Equipments[jtem.EquipmentId].CarbonEmission+ jtem.CarbonEmission, 2);
                                    Equipments[jtem.EquipmentId].ConvertCoal = Math.Round(Equipments[jtem.EquipmentId].ConvertCoal + jtem.ConvertCoal, 2);
                                }
                                else 
                                {
                                    Out_EquipmentEnergy equipmentEnergy = new Out_EquipmentEnergy();
                                    equipmentEnergy.Id = jtem.EquipmentId;
                                    equipmentEnergy.EquipmentName = jtem.EquipmentName;
                                    equipmentEnergy.UseVale = Math.Round(jtem.UseVale,2);
                                    equipmentEnergy.CostVale = Math.Round(jtem.CostVale,2);
                                    equipmentEnergy.Memo = jtem.Memo;
                                    equipmentEnergy.Unit = jtem.Unit;
                                    equipmentEnergy.LageUnit = jtem.LageUnit;
                                    equipmentEnergy.CarbonEmission = Math.Round(jtem.CarbonEmission,2);
                                    equipmentEnergy.ConvertCoal = Math.Round(jtem.ConvertCoal,2);

                                    Equipments.Add(jtem.EquipmentId, equipmentEnergy);
                                }
                                ft.Equipments = Equipments.Values.ToList();
                                */
                            }
                        }
                    }
                    else
                    {
                        //计算设施下的子设施的能耗
                        ft.Children = EnergyTree(item.Id, listFacility, listEnergies);
                        foreach (var jtem in ft.Children)
                        {
                            ft.UseVale = Math.Round(ft.UseVale + jtem.UseVale,2);
                            ft.CostVale = Math.Round(ft.CostVale + jtem.CostVale,2);
                            ft.CarbonEmission = Math.Round(ft.CarbonEmission + jtem.CarbonEmission,2);
                            ft.ConvertCoal = Math.Round(ft.ConvertCoal + jtem.ConvertCoal,2);
                            if (!string.IsNullOrEmpty(jtem.Unit))
                            {
                                ft.Unit = jtem.Unit;
                            }
                            if (!string.IsNullOrEmpty(jtem.LageUnit))
                            {
                                ft.LageUnit = jtem.LageUnit;
                            }
                        }
                    }
                    ft.UseVale = Math.Round(ft.UseVale, 2);
                    ft.CostVale = Math.Round(ft.CostVale, 2);
                    ft.CarbonEmission = Math.Round(ft.CarbonEmission, 2);
                    ft.ConvertCoal = Math.Round(ft.ConvertCoal, 2);
                    list.Add(ft);
                }
            }
            return list;
        }

        #endregion

        #region 测试每日生成计费数据
        /// <summary>
        /// 测试每日生成计费数据
        /// 弃用
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> TestEnergyDay(int Year, int Month, int Day, bool Execute, string equipmentId)
        {
            //所有绑定了物联网采集模块的设备
            List<T_Com_Equipment> equipments = new List<T_Com_Equipment>();

            List<T_Com_Equipment> AllEquipments = await _commonDAL.SelectEnergyEquipmentList();
            foreach (var item in AllEquipments)
            {
                if (item.Id == equipmentId)
                {
                    equipments.Add(item);
                }
            }

            //多个企业的上次查询记录
            Dictionary<string, T_Prod_Config> configs = new Dictionary<string, T_Prod_Config>();

            DateTime BeginTime = new DateTime(Year, Month, Day, 0, 0, 0, 0, DateTimeKind.Local);
            DateTime EndTime = new DateTime(Year, Month, Day, 23, 59, 59, 999, DateTimeKind.Local);

            foreach (var item in equipments)
            {
                //上次查询的记录
                T_Prod_Config config;
                if (configs.ContainsKey(item.OrgId.ToString()))
                {
                    config = configs[item.OrgId.ToString()];
                }
                else
                {
                    config = await _productionDAL.SelectConfig(item.OrgId.ToString());
                    if (config == null)
                    {
                        config = new T_Prod_Config();
                        config.Id = item.OrgId.ToString();
                        config.BeginTime = BeginTime;
                        config.EndTime = EndTime;
                        config.TotalUse = 0;
                        config.TotalTime = DateTime.Now;
                        await _productionDAL.AddConfig(config);
                    }
                    //if (config.EndTime == EndTime)//同一天已经生成过了，不重复
                    //{
                    //    continue;
                    //}
                    config.BeginTime = BeginTime;
                    config.EndTime = EndTime;
                    configs.Add(item.OrgId.ToString(), config);
                }

                //计费政策
                T_Price_Policy policy = await _policyDAL.SelectPolicy(item.PolicyId);

                if (policy == null)
                {
                    //没有绑定,下一个
                    continue;
                }
                else
                {
                    //计费政策明细
                    List<T_Price_PolicyDetil> policyDetil = await _policyDAL.SelectPolicyDetil(item.PolicyId);
                    var tmpOrgConf = await _orgConfDAL.Select(item.OrgId);

                    foreach (var itemDetil in policyDetil)
                    {
                        //生效月份
                        string[] months = itemDetil.PolicyMonth.Split(',');
                        //寻找当月的配置
                        if (months.Contains(EndTime.Month.ToString()))
                        {
                            if (tmpOrgConf == null)
                            {
                                continue;
                            }

                            //查询物联网采集的历史数据
                            List<DeviceProperty> historys = new List<DeviceProperty>();
                            if (Execute)
                            {
                                try
                                {
                                    In_HistoryListSync in_History = new In_HistoryListSync();
                                    in_History.Number = item.ThirdId;
                                    in_History.pageNum = 1;
                                    in_History.pageSize = int.MaxValue;
                                    in_History.BeginTime = BeginTime;
                                    in_History.EndTime = EndTime;
                                    in_History.Code = tmpOrgConf.ElecCode;
                                    historys = (await _iotInflux.SelectHistory(in_History)).List.Where(x => (double)x.Value > 0).OrderBy(x => x.UpdatedOn).ToList();
                                }
                                catch (Exception e)
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                for (int i = 0; i < 1440; i++)
                                {
                                    DeviceProperty property = new DeviceProperty();
                                    property.Value = i;
                                    property.UpdatedOn = BeginTime.AddMinutes(i);

                                    historys.Add(property);
                                }
                            }

                            if (itemDetil.PolicyType == "1")//分时
                            {
                                //分时价格
                                List<T_Price_Period> periods = (await _policyDAL.SelectPeriod(itemDetil.Id));
                                if (periods == null)
                                {
                                    //没绑定价格，下一个
                                    continue;
                                }

                                //分时时段
                                List<T_Price_Time> times = (await _policyDAL.SelectTime(itemDetil.Id));
                                if (times == null)
                                {
                                    //没绑定价格，下一个
                                    continue;
                                }

                                //排放因子
                                T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                if (factor == null)
                                {
                                    //没绑定排放因子，下一个
                                    continue;
                                }

                                //排放因子类型
                                T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                if (factorType == null)
                                {
                                    //没绑定排放因子类型，下一个
                                    continue;
                                }

                                foreach (var itemTime in times)
                                {
                                    DateTime start = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.StartTime);
                                    DateTime end = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.EndTime);
                                    if (EndTime.Hour == end.Hour && EndTime.Minute == end.Minute)
                                    {
                                        end = EndTime;
                                    }
                                    double initValue = 0;
                                    double endValue = 0;
                                    foreach (var itemHistory in historys)
                                    {
                                        if (itemHistory.UpdatedOn >= start && itemHistory.UpdatedOn <= end)
                                        {
                                            double result = 0;
                                            Double.TryParse(itemHistory.Value.ToString(), out result);

                                            if (initValue == 0)
                                            {
                                                initValue = result;
                                                endValue = result;
                                            }
                                            else
                                            {
                                                if (result > endValue)
                                                {
                                                    endValue = result;
                                                }
                                            }
                                        }
                                    }

                                    double price = 0;
                                    foreach (var itemPeriods in periods)
                                    {
                                        if (itemPeriods.TimePeriod == itemTime.TimePeriod)
                                        {
                                            price = itemPeriods.Price;
                                        }
                                    }


                                    //插入能耗记录
                                    T_Prod_Energy energy = new T_Prod_Energy();
                                    energy.Id = _snowflake.NextId().ToString();

                                    energy.OrgId = (long)item.OrgId;
                                    energy.EquipmentId = item.Id;
                                    energy.DDate = BeginTime;
                                    energy.UseVale = endValue - initValue;
                                    energy.CostVale = price * (endValue - initValue);
                                    energy.InitVale = initValue;
                                    energy.EndVale = endValue;

                                    energy.PolicyId = policy.Id;
                                    energy.PolicyName = policy.PolicyName;
                                    energy.FactorId = factorType.Id;
                                    energy.FactorName = factorType.TypeName;
                                    energy.Memo = itemTime.StartTime + "-" + itemTime.EndTime;
                                    energy.DataSource = "2";
                                    energy.Unit = policy.Unit;
                                    energy.LageUnit = policy.LageUnit;
                                    energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                    energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                    energy.TimePeriod = itemTime.TimePeriod;

                                    energy.del_flag = "0";
                                    energy.createId = 0;
                                    energy.create_time = DateTime.Now;
                                    energy.updateId = 0;
                                    energy.update_time = DateTime.Now;
                                    await _productionDAL.AddEnergy(energy);
                                }

                            }
                            else if (itemDetil.PolicyType == "2")//全天
                            {
                                //全天价格,只有一条
                                T_Price_Period periods = (await _policyDAL.SelectPeriod(itemDetil.Id)).First();
                                if (periods == null)
                                {
                                    continue;
                                }

                                //排放因子
                                T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                if (factor == null)
                                {
                                    continue;
                                }

                                //排放因子类型
                                T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                if (factorType == null)
                                {
                                    //没绑定排放因子类型，下一个
                                    continue;
                                }

                                double initValue = 0;
                                double endValue = 0;
                                foreach (var itemHistory in historys)
                                {
                                    double result = 0;
                                    Double.TryParse(itemHistory.Value.ToString(), out result);

                                    if (initValue == 0)
                                    {
                                        initValue = result;
                                        endValue = result;
                                    }
                                    else
                                    {
                                        if (result > endValue)
                                        {
                                            endValue = result;
                                        }
                                    }
                                }

                                //插入能耗记录
                                T_Prod_Energy energy = new T_Prod_Energy();
                                energy.Id = _snowflake.NextId().ToString();

                                energy.OrgId = (long)item.OrgId;
                                energy.EquipmentId = item.Id;
                                energy.DDate = BeginTime;
                                energy.UseVale = endValue - initValue;
                                energy.CostVale = periods.Price * (endValue - initValue);
                                energy.InitVale = initValue;
                                energy.EndVale = endValue;

                                energy.PolicyId = policy.Id;
                                energy.PolicyName = policy.PolicyName;
                                energy.FactorId = factorType.Id;
                                energy.FactorName = factorType.TypeName;
                                energy.Memo = "全天";
                                energy.DataSource = "2";
                                energy.Unit = policy.Unit;
                                energy.LageUnit = policy.LageUnit;
                                energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                energy.TimePeriod = "5";//5代表全天

                                energy.del_flag = "0";
                                energy.createId = 0;
                                energy.create_time = DateTime.Now;
                                energy.updateId = 0;
                                energy.update_time = DateTime.Now;
                                await _productionDAL.AddEnergy(energy);
                            }
                            else if (itemDetil.PolicyType == "3")//阶梯
                            {
                                //阶梯价格
                                List<T_Price_Tier> tiers = (await _policyDAL.SelectTier(itemDetil.Id));
                                if (tiers == null)
                                {
                                    continue;
                                }

                                //排放因子
                                T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                if (factor == null)
                                {
                                    continue;
                                }

                                //排放因子类型
                                T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                if (factorType == null)
                                {
                                    //没绑定排放因子类型，下一个
                                    continue;
                                }

                                double initValue = 0;
                                double endValue = 0;
                                foreach (var itemHistory in historys)
                                {
                                    double result = 0;
                                    Double.TryParse(itemHistory.Value.ToString(), out result);

                                    if (initValue == 0)
                                    {
                                        initValue = result;
                                        endValue = result;
                                    }
                                    else
                                    {
                                        if (result > endValue)
                                        {
                                            endValue = result;
                                        }
                                    }
                                }

                                InEnergyDayPageList query = new InEnergyDayPageList();
                                query.OrgId = item.OrgId;
                                query.DateType = "月";
                                query.FactorId = factorType.Id;
                                query.BeginDate = new DateTime(BeginTime.Year, BeginTime.Month, 1).ToString("yyyy-MM-dd");
                                query.EndDate = new DateTime(BeginTime.Year, BeginTime.Month, DateTime.DaysInMonth(BeginTime.Year, BeginTime.Month)).ToString("yyyy-MM-dd");
                                List<Out_EnergyDay> EnergyDays = (await _productionDAL.SelectEnergyDatePage(query)).List;

                                double totalUse = 0;
                                foreach (var days in EnergyDays)
                                {
                                    totalUse += days.UseVale;
                                }

                                T_Price_Tier tier1 = new T_Price_Tier();
                                T_Price_Tier tier2 = new T_Price_Tier();
                                T_Price_Tier tier3 = new T_Price_Tier();
                                foreach (var itemTier in tiers)
                                {
                                    if (itemTier.TierLevel == "1")
                                    {
                                        tier1 = itemTier;
                                    }
                                    else if (itemTier.TierLevel == "2")
                                    {
                                        tier2 = itemTier;
                                    }
                                    else if (itemTier.TierLevel == "3")
                                    {
                                        tier3 = itemTier;
                                    }
                                }
                                //第一阶梯
                                if (tier1.MinKwh <= totalUse && totalUse <= tier1.MaxKwh)
                                {
                                    //超出第二阶梯
                                    if (totalUse + (endValue - initValue) > tier2.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy1 = new T_Prod_Energy();
                                        energy1.Id = _snowflake.NextId().ToString();

                                        energy1.OrgId = (long)item.OrgId;
                                        energy1.EquipmentId = item.Id;
                                        energy1.DDate = BeginTime;
                                        energy1.UseVale = tier1.MaxKwh - initValue;
                                        energy1.CostVale = tier1.Price * (tier1.MaxKwh - initValue);
                                        energy1.InitVale = initValue;
                                        energy1.EndVale = tier1.MaxKwh;

                                        energy1.PolicyId = policy.Id;
                                        energy1.PolicyName = policy.PolicyName;
                                        energy1.FactorId = factorType.Id;
                                        energy1.FactorName = factorType.TypeName;
                                        energy1.DataSource = "2";
                                        energy1.Memo = "阶梯一";
                                        energy1.Unit = policy.Unit;
                                        energy1.LageUnit = policy.LageUnit;
                                        energy1.CarbonEmission = factor.EmissionFactor * (tier1.MaxKwh - initValue);
                                        energy1.ConvertCoal = (factor.AvgCalorific * (tier1.MaxKwh - initValue)) / factor.EqCoal;
                                        energy1.TimePeriod = "6";//阶梯一

                                        energy1.del_flag = "0";
                                        energy1.createId = 0;
                                        energy1.create_time = DateTime.Now;
                                        energy1.updateId = 0;
                                        energy1.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy1);

                                        //插入能耗记录
                                        T_Prod_Energy energy2 = new T_Prod_Energy();
                                        energy2.Id = _snowflake.NextId().ToString();

                                        energy2.OrgId = (long)item.OrgId;
                                        energy2.EquipmentId = item.Id;
                                        energy2.DDate = BeginTime;
                                        energy2.UseVale = tier2.MaxKwh - tier1.MaxKwh;
                                        energy2.CostVale = tier2.Price * (tier2.MaxKwh - tier1.MaxKwh);
                                        energy2.InitVale = tier1.MaxKwh;
                                        energy2.EndVale = tier2.MaxKwh;

                                        energy2.PolicyId = policy.Id;
                                        energy2.PolicyName = policy.PolicyName;
                                        energy2.FactorId = factorType.Id;
                                        energy2.FactorName = factorType.TypeName;
                                        energy2.DataSource = "2";
                                        energy2.Memo = "阶梯二";
                                        energy2.Unit = policy.Unit;
                                        energy2.LageUnit = policy.LageUnit;
                                        energy2.CarbonEmission = factor.EmissionFactor * (tier2.MaxKwh - tier1.MaxKwh);
                                        energy2.ConvertCoal = (factor.AvgCalorific * (tier2.MaxKwh - tier1.MaxKwh)) / factor.EqCoal;
                                        energy2.TimePeriod = "7";//阶梯二

                                        energy2.del_flag = "0";
                                        energy2.createId = 0;
                                        energy2.create_time = DateTime.Now;
                                        energy2.updateId = 0;
                                        energy2.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy2);

                                        //插入能耗记录
                                        T_Prod_Energy energy3 = new T_Prod_Energy();
                                        energy3.Id = _snowflake.NextId().ToString();

                                        energy3.OrgId = (long)item.OrgId;
                                        energy3.EquipmentId = item.Id;
                                        energy3.DDate = BeginTime;
                                        energy3.UseVale = endValue - tier2.MaxKwh;
                                        energy3.CostVale = tier2.Price * (endValue - tier2.MaxKwh);
                                        energy3.InitVale = tier2.MaxKwh;
                                        energy3.EndVale = endValue;

                                        energy3.PolicyId = policy.Id;
                                        energy3.PolicyName = policy.PolicyName;
                                        energy3.FactorId = factorType.Id;
                                        energy3.FactorName = factorType.TypeName;
                                        energy3.DataSource = "2";
                                        energy3.Memo = "阶梯三";
                                        energy3.Unit = policy.Unit;
                                        energy3.LageUnit = policy.LageUnit;
                                        energy3.CarbonEmission = factor.EmissionFactor * (endValue - tier2.MaxKwh);
                                        energy3.ConvertCoal = (factor.AvgCalorific * (endValue - tier2.MaxKwh)) / factor.EqCoal;
                                        energy3.TimePeriod = "8";//阶梯三

                                        energy3.del_flag = "0";
                                        energy3.createId = 0;
                                        energy3.create_time = DateTime.Now;
                                        energy3.updateId = 0;
                                        energy3.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy3);

                                    }
                                    //超出第一阶梯,不超出第二阶梯
                                    if (totalUse + (endValue - initValue) > tier1.MaxKwh && totalUse + (endValue - initValue) <= tier2.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy1 = new T_Prod_Energy();
                                        energy1.Id = _snowflake.NextId().ToString();

                                        energy1.OrgId = (long)item.OrgId;
                                        energy1.EquipmentId = item.Id;
                                        energy1.DDate = BeginTime;
                                        energy1.UseVale = tier1.MaxKwh - initValue;
                                        energy1.CostVale = tier1.Price * (tier1.MaxKwh - initValue);
                                        energy1.InitVale = initValue;
                                        energy1.EndVale = tier1.MaxKwh;

                                        energy1.PolicyId = policy.Id;
                                        energy1.PolicyName = policy.PolicyName;
                                        energy1.FactorId = factorType.Id;
                                        energy1.FactorName = factorType.TypeName;
                                        energy1.DataSource = "2";
                                        energy1.Memo = "阶梯一";
                                        energy1.Unit = policy.Unit;
                                        energy1.LageUnit = policy.LageUnit;
                                        energy1.CarbonEmission = factor.EmissionFactor * (tier1.MaxKwh - initValue);
                                        energy1.ConvertCoal = (factor.AvgCalorific * (tier1.MaxKwh - initValue)) / factor.EqCoal;
                                        energy1.TimePeriod = "6";//阶梯一

                                        energy1.del_flag = "0";
                                        energy1.createId = 0;
                                        energy1.create_time = DateTime.Now;
                                        energy1.updateId = 0;
                                        energy1.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy1);

                                        //插入能耗记录
                                        T_Prod_Energy energy2 = new T_Prod_Energy();
                                        energy2.Id = _snowflake.NextId().ToString();

                                        energy2.OrgId = (long)item.OrgId;
                                        energy2.EquipmentId = item.Id;
                                        energy2.DDate = BeginTime;
                                        energy2.UseVale = endValue - tier1.MaxKwh;
                                        energy2.CostVale = tier2.Price * (endValue - tier1.MaxKwh);
                                        energy2.InitVale = tier1.MaxKwh;
                                        energy2.EndVale = endValue;

                                        energy2.PolicyId = policy.Id;
                                        energy2.PolicyName = policy.PolicyName;
                                        energy2.FactorId = factorType.Id;
                                        energy2.FactorName = factorType.TypeName;
                                        energy2.DataSource = "2";
                                        energy2.Memo = "阶梯二";
                                        energy2.Unit = policy.Unit;
                                        energy2.LageUnit = policy.LageUnit;
                                        energy2.CarbonEmission = factor.EmissionFactor * (endValue - tier1.MaxKwh);
                                        energy2.ConvertCoal = (factor.AvgCalorific * (endValue - tier1.MaxKwh)) / factor.EqCoal;
                                        energy2.TimePeriod = "7";//阶梯二

                                        energy2.del_flag = "0";
                                        energy2.createId = 0;
                                        energy2.create_time = DateTime.Now;
                                        energy2.updateId = 0;
                                        energy2.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy2);
                                    }
                                    //不超出第一阶梯
                                    if (tier1.MinKwh <= totalUse + (endValue - initValue) && totalUse + (endValue - initValue) <= tier1.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy = new T_Prod_Energy();
                                        energy.Id = _snowflake.NextId().ToString();

                                        energy.OrgId = (long)item.OrgId;
                                        energy.EquipmentId = item.Id;
                                        energy.DDate = BeginTime;
                                        energy.UseVale = endValue - initValue;
                                        energy.CostVale = tier1.Price * (endValue - initValue);
                                        energy.InitVale = initValue;
                                        energy.EndVale = endValue;

                                        energy.PolicyId = policy.Id;
                                        energy.PolicyName = policy.PolicyName;
                                        energy.FactorId = factorType.Id;
                                        energy.FactorName = factorType.TypeName;
                                        energy.DataSource = "2";
                                        energy.Memo = "阶梯一";
                                        energy.Unit = policy.Unit;
                                        energy.LageUnit = policy.LageUnit;
                                        energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                        energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                        energy.TimePeriod = "6";//阶梯一

                                        energy.del_flag = "0";
                                        energy.createId = 0;
                                        energy.create_time = DateTime.Now;
                                        energy.updateId = 0;
                                        energy.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy);
                                    }
                                }
                                //第二阶梯
                                if (tier1.MaxKwh < totalUse && totalUse <= tier2.MaxKwh)
                                {
                                    //第二阶梯和第三阶梯交界
                                    if (tier2.MaxKwh < totalUse + (endValue - initValue))
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy1 = new T_Prod_Energy();
                                        energy1.Id = _snowflake.NextId().ToString();

                                        energy1.OrgId = (long)item.OrgId;
                                        energy1.EquipmentId = item.Id;
                                        energy1.DDate = BeginTime;
                                        energy1.UseVale = tier2.MaxKwh - initValue;
                                        energy1.CostVale = tier2.Price * (tier2.MaxKwh - initValue);
                                        energy1.InitVale = initValue;
                                        energy1.EndVale = tier2.MaxKwh;

                                        energy1.PolicyId = policy.Id;
                                        energy1.PolicyName = policy.PolicyName;
                                        energy1.FactorId = factorType.Id;
                                        energy1.FactorName = factorType.TypeName;
                                        energy1.DataSource = "2";
                                        energy1.Memo = "阶梯二";
                                        energy1.Unit = policy.Unit;
                                        energy1.LageUnit = policy.LageUnit;
                                        energy1.CarbonEmission = factor.EmissionFactor * (tier2.MaxKwh - initValue);
                                        energy1.ConvertCoal = (factor.AvgCalorific * (tier2.MaxKwh - initValue)) / factor.EqCoal;
                                        energy1.TimePeriod = "7";//阶梯二

                                        energy1.del_flag = "0";
                                        energy1.createId = 0;
                                        energy1.create_time = DateTime.Now;
                                        energy1.updateId = 0;
                                        energy1.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy1);

                                        //插入能耗记录
                                        T_Prod_Energy energy2 = new T_Prod_Energy();
                                        energy2.Id = _snowflake.NextId().ToString();

                                        energy2.OrgId = (long)item.OrgId;
                                        energy2.EquipmentId = item.Id;
                                        energy2.DDate = BeginTime;
                                        energy2.UseVale = endValue - tier2.MaxKwh;
                                        energy2.CostVale = tier3.Price * (endValue - tier2.MaxKwh);
                                        energy2.InitVale = tier2.MaxKwh;
                                        energy2.EndVale = endValue;

                                        energy2.PolicyId = policy.Id;
                                        energy2.PolicyName = policy.PolicyName;
                                        energy2.FactorId = factorType.Id;
                                        energy2.FactorName = factorType.TypeName;
                                        energy2.DataSource = "2";
                                        energy2.Memo = "阶梯三";
                                        energy2.Unit = policy.Unit;
                                        energy2.LageUnit = policy.LageUnit;
                                        energy2.CarbonEmission = factor.EmissionFactor * (endValue - tier2.MaxKwh);
                                        energy2.ConvertCoal = (factor.AvgCalorific * (endValue - tier2.MaxKwh)) / factor.EqCoal;
                                        energy2.TimePeriod = "8";//阶梯三

                                        energy2.del_flag = "0";
                                        energy2.createId = 0;
                                        energy2.create_time = DateTime.Now;
                                        energy2.updateId = 0;
                                        energy2.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy2);
                                    }
                                    //不超出第二阶梯
                                    if (totalUse + (endValue - initValue) > tier1.MaxKwh && totalUse + (endValue - initValue) <= tier2.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy = new T_Prod_Energy();
                                        energy.Id = _snowflake.NextId().ToString();

                                        energy.OrgId = (long)item.OrgId;
                                        energy.EquipmentId = item.Id;
                                        energy.DDate = BeginTime;
                                        energy.UseVale = endValue - initValue;
                                        energy.CostVale = tier2.Price * (endValue - initValue);
                                        energy.InitVale = initValue;
                                        energy.EndVale = endValue;

                                        energy.PolicyId = policy.Id;
                                        energy.PolicyName = policy.PolicyName;
                                        energy.FactorId = factorType.Id;
                                        energy.FactorName = factorType.TypeName;
                                        energy.DataSource = "2";
                                        energy.Memo = "阶梯二";
                                        energy.Unit = policy.Unit;
                                        energy.LageUnit = policy.LageUnit;
                                        energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                        energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                        energy.TimePeriod = "7";//阶梯二

                                        energy.del_flag = "0";
                                        energy.createId = 0;
                                        energy.create_time = DateTime.Now;
                                        energy.updateId = 0;
                                        energy.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy);
                                    }
                                }
                                //第三阶梯
                                if (tier2.MaxKwh < totalUse)
                                {
                                    //插入能耗记录
                                    T_Prod_Energy energy = new T_Prod_Energy();
                                    energy.Id = _snowflake.NextId().ToString();

                                    energy.OrgId = (long)item.OrgId;
                                    energy.EquipmentId = item.Id;
                                    energy.DDate = BeginTime;
                                    energy.UseVale = endValue - initValue;
                                    energy.CostVale = tier3.Price * (endValue - initValue);
                                    energy.InitVale = initValue;
                                    energy.EndVale = endValue;

                                    energy.PolicyId = policy.Id;
                                    energy.PolicyName = policy.PolicyName;
                                    energy.FactorId = factorType.Id;
                                    energy.FactorName = factorType.TypeName;
                                    energy.DataSource = "2";
                                    energy.Memo = "阶梯三";
                                    energy.Unit = policy.Unit;
                                    energy.LageUnit = policy.LageUnit;
                                    energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                    energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                    energy.TimePeriod = "8";//阶梯三

                                    energy.del_flag = "0";
                                    energy.createId = 0;
                                    energy.create_time = DateTime.Now;
                                    energy.updateId = 0;
                                    energy.update_time = DateTime.Now;
                                    await _productionDAL.AddEnergy(energy);
                                }
                                //更新累计值
                                configs[item.OrgId.ToString()].TotalUse = totalUse + (endValue - initValue);
                                configs[item.OrgId.ToString()].TotalTime = DateTime.Now;
                            }
                        }
                    }

                }
            }

            //更新查询记录
            foreach (var item in configs.Keys)
            {
                await _productionDAL.UpdateConfig(configs[item]);
            }

            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 生成电消耗数据
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ReCalculationEnergyDay(string beginDate, string endDate, long orgId)
        {
            //所有绑定了物联网采集模块的设备
            List<T_Com_Equipment> equipments = new List<T_Com_Equipment>();

            List<T_Com_Equipment> AllEquipments = await _commonDAL.SelectEnergyEquipmentList();
            foreach (var item in AllEquipments)
            {
                if (item.OrgId == orgId && item.ThirdId != "-")//同一个组织，并且绑定了第三方编码
                {
                    equipments.Add(item);
                }
            }

            DateTime dateBegin = DateTime.Parse(beginDate);//开始日期
            DateTime dateEnd = DateTime.Parse(endDate);//结束日期

            while (dateBegin <= dateEnd)
            {
                int Year = dateBegin.Year;
                int Month = dateBegin.Month;
                int Day = dateBegin.Day;

                DateTime BeginTime = new DateTime(Year, Month, Day, 0, 0, 0, 0, DateTimeKind.Local);
                DateTime EndTime = new DateTime(Year, Month, Day, 23, 59, 59, 999, DateTimeKind.Local);

                foreach (var item in equipments)
                {
                    //是否已生成历史数据，已生成跳过
                    InEnergyTree queryTree = new InEnergyTree();
                    queryTree.OrgId = orgId;
                    queryTree.EquipmentId = item.Id;
                    queryTree.BeginDate = dateBegin.ToString("yyyy-MM-dd");
                    queryTree.EndDate = dateBegin.ToString("yyyy-MM-dd");

                    //删除
                    await _productionDAL.DeleteEnergyList(item.Id, dateBegin);

                    //计费政策
                    T_Price_Policy policy = await _policyDAL.SelectPolicy(item.PolicyId);

                    if (policy == null)
                    {
                        //没有绑定,下一个
                        continue;
                    }
                    else
                    {
                        //计费政策明细
                        List<T_Price_PolicyDetil> policyDetil = await _policyDAL.SelectPolicyDetil(item.PolicyId);
                        var tmpOrgConf = await _orgConfDAL.Select(item.OrgId);

                        foreach (var itemDetil in policyDetil)
                        {
                            //生效月份
                            string[] months = itemDetil.PolicyMonth.Split(',');
                            //寻找当月的配置
                            if (months.Contains(EndTime.Month.ToString()))
                            {
                                if (tmpOrgConf == null)
                                {
                                    continue;
                                }

                                //查询物联网采集的历史数据
                                List<DeviceProperty> historys = new List<DeviceProperty>();

                                try
                                {
                                    In_HistoryListSync in_History = new In_HistoryListSync();
                                    in_History.Number = item.ThirdId;
                                    in_History.pageNum = 1;
                                    in_History.pageSize = int.MaxValue;
                                    in_History.BeginTime = BeginTime;
                                    in_History.EndTime = EndTime;
                                    in_History.Code = tmpOrgConf.ElecCode;
                                    historys = (await _iotInflux.SelectHistory(in_History)).List.Where(x => (double)x.Value > 0).OrderBy(x => x.UpdatedOn).ToList();
                                }
                                catch (Exception e)
                                {
                                    //continue;
                                    Random random = new Random();
                                    int n = 1;
                                    for (global::System.Int32 i = 0; i < 24; i++)
                                    {
                                        for (global::System.Int32 j = 0; j < 60; j++)
                                        {
                                            DeviceProperty ax = new DeviceProperty();
                                            n = n + 1;
                                            ax.Value = n;
                                            DateTime dateTime = new DateTime(Year, Month, Day, i, j, 1, 1, DateTimeKind.Local); ;
                                            ax.UpdatedOn = dateTime;
                                            historys.Add(ax);
                                        }
                                    }
                                }

                                if (itemDetil.PolicyType == "1")//分时
                                {
                                    //分时价格
                                    List<T_Price_Period> periods = (await _policyDAL.SelectPeriod(itemDetil.Id));
                                    if (periods == null)
                                    {
                                        //没绑定价格，下一个
                                        continue;
                                    }

                                    //分时时段
                                    List<T_Price_Time> times = (await _policyDAL.SelectTime(itemDetil.Id)).OrderBy(e => e.StartTime).ToList();
                                    if (times == null)
                                    {
                                        //没绑定价格，下一个
                                        continue;
                                    }

                                    //排放因子
                                    T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                    if (factor == null)
                                    {
                                        //没绑定排放因子，下一个
                                        continue;
                                    }

                                    //排放因子类型
                                    T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                    if (factorType == null)
                                    {
                                        //没绑定排放因子类型，下一个
                                        continue;
                                    }
                                    Out_Energy eng = await _productionDAL.SelectEnergyMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                    double initValue = 0;
                                    double endValue = 0;
                                    if (eng != null)
                                    {
                                        initValue = eng.EndVale;
                                        endValue = eng.EndVale;
                                    }
                                    foreach (var itemTime in times)
                                    {
                                        DateTime start = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.StartTime);
                                        DateTime end = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.EndTime);
                                        if (EndTime.Hour == end.Hour && EndTime.Minute == end.Minute)
                                        {
                                            end = EndTime;
                                        }
                                        initValue = endValue;
                                        foreach (var itemHistory in historys)
                                        {
                                            if (itemHistory.UpdatedOn >= start && itemHistory.UpdatedOn <= end)
                                            {
                                                double result = 0;
                                                Double.TryParse(itemHistory.Value.ToString(), out result);

                                                if (initValue == 0)
                                                {
                                                    initValue = result;
                                                    endValue = result;
                                                }
                                                else
                                                {
                                                    if (result > endValue)
                                                    {
                                                        endValue = result;
                                                    }
                                                }
                                            }
                                        }

                                        double price = 0;
                                        foreach (var itemPeriods in periods)
                                        {
                                            if (itemPeriods.TimePeriod == itemTime.TimePeriod)
                                            {
                                                price = itemPeriods.Price;
                                            }
                                        }


                                        //插入能耗记录
                                        T_Prod_Energy energy = new T_Prod_Energy();
                                        energy.Id = _snowflake.NextId().ToString();

                                        energy.OrgId = (long)item.OrgId;
                                        energy.EquipmentId = item.Id;
                                        energy.DDate = BeginTime;
                                        energy.UseVale = endValue - initValue;
                                        energy.CostVale = price * (endValue - initValue);
                                        energy.InitVale = initValue;
                                        energy.EndVale = endValue;

                                        energy.PolicyId = policy.Id;
                                        energy.PolicyName = policy.PolicyName;
                                        energy.FactorId = factorType.Id;
                                        energy.FactorName = factorType.TypeName;
                                        energy.Memo = itemTime.StartTime + "-" + itemTime.EndTime;
                                        energy.DataSource = "2";
                                        energy.Unit = policy.Unit;
                                        energy.LageUnit = policy.LageUnit;
                                        energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                        energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                        energy.TimePeriod = itemTime.TimePeriod;

                                        energy.del_flag = "0";
                                        energy.createId = 0;
                                        energy.create_time = DateTime.Now;
                                        energy.updateId = 0;
                                        energy.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy);
                                    }

                                    T_Prod_Energy_T eng_t = await _productionDAL.SelectEnergyTimeMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                    if (eng_t == null)
                                    {
                                        initValue = 0;
                                        endValue = 0;
                                    }
                                    else
                                    {
                                        initValue = eng_t.EndVale;
                                        endValue = eng_t.EndVale;
                                    } 
                                    await _productionDAL.DeleteEnergyTime(item.Id, BeginTime);
                                    foreach (var itemTime in times)
                                    {
                                        DateTime start = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.StartTime);
                                        DateTime end = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.EndTime);
                                        if (EndTime.Hour == end.Hour && EndTime.Minute == end.Minute)
                                        {
                                            end = EndTime;
                                        }
                                        double price = 0;
                                        foreach (var itemPeriods in periods)
                                        {
                                            if (itemPeriods.TimePeriod == itemTime.TimePeriod)
                                            {
                                                price = itemPeriods.Price;
                                            }
                                        }
                                        for (global::System.Int32 i = 0; i < 24; i++)
                                        {
                                            initValue = endValue;
                                            foreach (var itemHistory in historys)
                                            {
                                                if (itemHistory.UpdatedOn >= start && itemHistory.UpdatedOn <= end && itemHistory.UpdatedOn.Value.Hour == i)
                                                {
                                                    double result = 0;
                                                    Double.TryParse(itemHistory.Value.ToString(), out result);

                                                    if (initValue == 0)
                                                    {
                                                        initValue = result;
                                                        endValue = result;
                                                    }
                                                    else
                                                    {
                                                        if (result > endValue)
                                                        {
                                                            endValue = result;
                                                        }
                                                    }
                                                }
                                            }
                                            T_Prod_Energy_T energy_H = new T_Prod_Energy_T();
                                            energy_H.Id = _snowflake.NextId().ToString();

                                            energy_H.OrgId = (long)item.OrgId;
                                            energy_H.EquipmentId = item.Id;
                                            energy_H.DDate = BeginTime;
                                            energy_H.TTime = i.ToString().PadLeft(2, '0') + ":00-" + (i + 1).ToString().PadLeft(2, '0') + ":00";
                                            energy_H.UseVale = Math.Round(endValue - initValue, 2);
                                            energy_H.InitVale = initValue;
                                            energy_H.EndVale = endValue;
                                            energy_H.FactorId = factorType.Id;
                                            energy_H.FactorName = factorType.TypeName;
                                            energy_H.Unit = policy.Unit;
                                            energy_H.LageUnit = policy.LageUnit;
                                            energy_H.CostVale = Math.Round(energy_H.UseVale * price, 2);
                                            energy_H.CarbonEmission = Math.Round(energy_H.UseVale * factor.EmissionFactor, 2);
                                            energy_H.ConvertCoal = Math.Round((energy_H.UseVale * factor.AvgCalorific) / factor.EqCoal, 2);

                                            if (itemTime.TimePeriod == "1")
                                            {
                                                energy_H.TimePeriod = "尖";
                                            }
                                            else if (itemTime.TimePeriod == "2")
                                            {
                                                energy_H.TimePeriod = "峰";
                                            }
                                            else if (itemTime.TimePeriod == "3")
                                            {
                                                energy_H.TimePeriod = "平";
                                            }
                                            else if (itemTime.TimePeriod == "4")
                                            {
                                                energy_H.TimePeriod = "谷";
                                            }
                                            if (energy_H.UseVale > 0)
                                            {
                                                await _productionDAL.AddEnergyTime(energy_H);
                                            }
                                        }
                                    }
                                }
                                else if (itemDetil.PolicyType == "2")//全天
                                {
                                    //全天价格,只有一条
                                    T_Price_Period periods = (await _policyDAL.SelectPeriod(itemDetil.Id)).First();
                                    if (periods == null)
                                    {
                                        continue;
                                    }

                                    //排放因子
                                    T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                    if (factor == null)
                                    {
                                        continue;
                                    }

                                    //排放因子类型
                                    T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                    if (factorType == null)
                                    {
                                        //没绑定排放因子类型，下一个
                                        continue;
                                    }

                                    Out_Energy eng = await _productionDAL.SelectEnergyMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                    double initValue = 0;
                                    double endValue = 0;
                                    if (eng != null)
                                    {
                                        initValue = eng.EndVale;
                                        endValue = eng.EndVale;
                                    }
                                    foreach (var itemHistory in historys)
                                    {
                                        double result = 0;
                                        Double.TryParse(itemHistory.Value.ToString(), out result);

                                        if (initValue == 0)
                                        {
                                            initValue = result;
                                            endValue = result;
                                        }
                                        else
                                        {
                                            if (result > endValue)
                                            {
                                                endValue = result;
                                            }
                                        }
                                    }

                                    //插入能耗记录
                                    T_Prod_Energy energy = new T_Prod_Energy();
                                    energy.Id = _snowflake.NextId().ToString();

                                    energy.OrgId = (long)item.OrgId;
                                    energy.EquipmentId = item.Id;
                                    energy.DDate = BeginTime;
                                    energy.UseVale = endValue - initValue;
                                    energy.CostVale = periods.Price * (endValue - initValue);
                                    energy.InitVale = initValue;
                                    energy.EndVale = endValue;

                                    energy.PolicyId = policy.Id;
                                    energy.PolicyName = policy.PolicyName;
                                    energy.FactorId = factorType.Id;
                                    energy.FactorName = factorType.TypeName;
                                    energy.Memo = "全天";
                                    energy.DataSource = "2";
                                    energy.Unit = policy.Unit;
                                    energy.LageUnit = policy.LageUnit;
                                    energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                    energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                    energy.TimePeriod = "5";//5代表全天

                                    energy.del_flag = "0";
                                    energy.createId = 0;
                                    energy.create_time = DateTime.Now;
                                    energy.updateId = 0;
                                    energy.update_time = DateTime.Now;
                                    await _productionDAL.AddEnergy(energy);
                                }
                                else if (itemDetil.PolicyType == "3")//阶梯
                                {
                                    //阶梯价格
                                    List<T_Price_Tier> tiers = (await _policyDAL.SelectTier(itemDetil.Id));
                                    if (tiers == null)
                                    {
                                        continue;
                                    }

                                    //排放因子
                                    T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                    if (factor == null)
                                    {
                                        continue;
                                    }

                                    //排放因子类型
                                    T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                    if (factorType == null)
                                    {
                                        //没绑定排放因子类型，下一个
                                        continue;
                                    }

                                    double initValue = 0;
                                    double endValue = 0;
                                    foreach (var itemHistory in historys)
                                    {
                                        double result = 0;
                                        Double.TryParse(itemHistory.Value.ToString(), out result);

                                        if (initValue == 0)
                                        {
                                            initValue = result;
                                            endValue = result;
                                        }
                                        else
                                        {
                                            if (result > endValue)
                                            {
                                                endValue = result;
                                            }
                                        }
                                    }

                                    InEnergyDayPageList query = new InEnergyDayPageList();
                                    query.OrgId = item.OrgId;
                                    query.DateType = "月";
                                    query.FactorId = factorType.Id;
                                    query.BeginDate = new DateTime(BeginTime.Year, BeginTime.Month, 1).ToString("yyyy-MM-dd");
                                    query.EndDate = new DateTime(BeginTime.Year, BeginTime.Month, DateTime.DaysInMonth(BeginTime.Year, BeginTime.Month)).ToString("yyyy-MM-dd");
                                    List<Out_EnergyDay> EnergyDays = (await _productionDAL.SelectEnergyDatePage(query)).List;

                                    double totalUse = 0;
                                    foreach (var days in EnergyDays)
                                    {
                                        totalUse += days.UseVale;
                                    }

                                    T_Price_Tier tier1 = new T_Price_Tier();
                                    T_Price_Tier tier2 = new T_Price_Tier();
                                    T_Price_Tier tier3 = new T_Price_Tier();
                                    foreach (var itemTier in tiers)
                                    {
                                        if (itemTier.TierLevel == "1")
                                        {
                                            tier1 = itemTier;
                                        }
                                        else if (itemTier.TierLevel == "2")
                                        {
                                            tier2 = itemTier;
                                        }
                                        else if (itemTier.TierLevel == "3")
                                        {
                                            tier3 = itemTier;
                                        }
                                    }
                                    //第一阶梯
                                    if (tier1.MinKwh <= totalUse && totalUse <= tier1.MaxKwh)
                                    {
                                        //超出第二阶梯
                                        if (totalUse + (endValue - initValue) > tier2.MaxKwh)
                                        {
                                            //插入能耗记录
                                            T_Prod_Energy energy1 = new T_Prod_Energy();
                                            energy1.Id = _snowflake.NextId().ToString();

                                            energy1.OrgId = (long)item.OrgId;
                                            energy1.EquipmentId = item.Id;
                                            energy1.DDate = BeginTime;
                                            energy1.UseVale = tier1.MaxKwh - initValue;
                                            energy1.CostVale = tier1.Price * (tier1.MaxKwh - initValue);
                                            energy1.InitVale = initValue;
                                            energy1.EndVale = tier1.MaxKwh;

                                            energy1.PolicyId = policy.Id;
                                            energy1.PolicyName = policy.PolicyName;
                                            energy1.FactorId = factorType.Id;
                                            energy1.FactorName = factorType.TypeName;
                                            energy1.DataSource = "2";
                                            energy1.Memo = "阶梯一";
                                            energy1.Unit = policy.Unit;
                                            energy1.LageUnit = policy.LageUnit;
                                            energy1.CarbonEmission = factor.EmissionFactor * (tier1.MaxKwh - initValue);
                                            energy1.ConvertCoal = (factor.AvgCalorific * (tier1.MaxKwh - initValue)) / factor.EqCoal;
                                            energy1.TimePeriod = "6";//阶梯一

                                            energy1.del_flag = "0";
                                            energy1.createId = 0;
                                            energy1.create_time = DateTime.Now;
                                            energy1.updateId = 0;
                                            energy1.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy1);

                                            //插入能耗记录
                                            T_Prod_Energy energy2 = new T_Prod_Energy();
                                            energy2.Id = _snowflake.NextId().ToString();

                                            energy2.OrgId = (long)item.OrgId;
                                            energy2.EquipmentId = item.Id;
                                            energy2.DDate = BeginTime;
                                            energy2.UseVale = tier2.MaxKwh - tier1.MaxKwh;
                                            energy2.CostVale = tier2.Price * (tier2.MaxKwh - tier1.MaxKwh);
                                            energy2.InitVale = tier1.MaxKwh;
                                            energy2.EndVale = tier2.MaxKwh;

                                            energy2.PolicyId = policy.Id;
                                            energy2.PolicyName = policy.PolicyName;
                                            energy2.FactorId = factorType.Id;
                                            energy2.FactorName = factorType.TypeName;
                                            energy2.DataSource = "2";
                                            energy2.Memo = "阶梯二";
                                            energy2.Unit = policy.Unit;
                                            energy2.LageUnit = policy.LageUnit;
                                            energy2.CarbonEmission = factor.EmissionFactor * (tier2.MaxKwh - tier1.MaxKwh);
                                            energy2.ConvertCoal = (factor.AvgCalorific * (tier2.MaxKwh - tier1.MaxKwh)) / factor.EqCoal;
                                            energy2.TimePeriod = "7";//阶梯二

                                            energy2.del_flag = "0";
                                            energy2.createId = 0;
                                            energy2.create_time = DateTime.Now;
                                            energy2.updateId = 0;
                                            energy2.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy2);

                                            //插入能耗记录
                                            T_Prod_Energy energy3 = new T_Prod_Energy();
                                            energy3.Id = _snowflake.NextId().ToString();

                                            energy3.OrgId = (long)item.OrgId;
                                            energy3.EquipmentId = item.Id;
                                            energy3.DDate = BeginTime;
                                            energy3.UseVale = endValue - tier2.MaxKwh;
                                            energy3.CostVale = tier2.Price * (endValue - tier2.MaxKwh);
                                            energy3.InitVale = tier2.MaxKwh;
                                            energy3.EndVale = endValue;

                                            energy3.PolicyId = policy.Id;
                                            energy3.PolicyName = policy.PolicyName;
                                            energy3.FactorId = factorType.Id;
                                            energy3.FactorName = factorType.TypeName;
                                            energy3.DataSource = "2";
                                            energy3.Memo = "阶梯三";
                                            energy3.Unit = policy.Unit;
                                            energy3.LageUnit = policy.LageUnit;
                                            energy3.CarbonEmission = factor.EmissionFactor * (endValue - tier2.MaxKwh);
                                            energy3.ConvertCoal = (factor.AvgCalorific * (endValue - tier2.MaxKwh)) / factor.EqCoal;
                                            energy3.TimePeriod = "8";//阶梯三

                                            energy3.del_flag = "0";
                                            energy3.createId = 0;
                                            energy3.create_time = DateTime.Now;
                                            energy3.updateId = 0;
                                            energy3.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy3);

                                        }
                                        //超出第一阶梯,不超出第二阶梯
                                        if (totalUse + (endValue - initValue) > tier1.MaxKwh && totalUse + (endValue - initValue) <= tier2.MaxKwh)
                                        {
                                            //插入能耗记录
                                            T_Prod_Energy energy1 = new T_Prod_Energy();
                                            energy1.Id = _snowflake.NextId().ToString();

                                            energy1.OrgId = (long)item.OrgId;
                                            energy1.EquipmentId = item.Id;
                                            energy1.DDate = BeginTime;
                                            energy1.UseVale = tier1.MaxKwh - initValue;
                                            energy1.CostVale = tier1.Price * (tier1.MaxKwh - initValue);
                                            energy1.InitVale = initValue;
                                            energy1.EndVale = tier1.MaxKwh;

                                            energy1.PolicyId = policy.Id;
                                            energy1.PolicyName = policy.PolicyName;
                                            energy1.FactorId = factorType.Id;
                                            energy1.FactorName = factorType.TypeName;
                                            energy1.DataSource = "2";
                                            energy1.Memo = "阶梯一";
                                            energy1.Unit = policy.Unit;
                                            energy1.LageUnit = policy.LageUnit;
                                            energy1.CarbonEmission = factor.EmissionFactor * (tier1.MaxKwh - initValue);
                                            energy1.ConvertCoal = (factor.AvgCalorific * (tier1.MaxKwh - initValue)) / factor.EqCoal;
                                            energy1.TimePeriod = "6";//阶梯一

                                            energy1.del_flag = "0";
                                            energy1.createId = 0;
                                            energy1.create_time = DateTime.Now;
                                            energy1.updateId = 0;
                                            energy1.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy1);

                                            //插入能耗记录
                                            T_Prod_Energy energy2 = new T_Prod_Energy();
                                            energy2.Id = _snowflake.NextId().ToString();

                                            energy2.OrgId = (long)item.OrgId;
                                            energy2.EquipmentId = item.Id;
                                            energy2.DDate = BeginTime;
                                            energy2.UseVale = endValue - tier1.MaxKwh;
                                            energy2.CostVale = tier2.Price * (endValue - tier1.MaxKwh);
                                            energy2.InitVale = tier1.MaxKwh;
                                            energy2.EndVale = endValue;

                                            energy2.PolicyId = policy.Id;
                                            energy2.PolicyName = policy.PolicyName;
                                            energy2.FactorId = factorType.Id;
                                            energy2.FactorName = factorType.TypeName;
                                            energy2.DataSource = "2";
                                            energy2.Memo = "阶梯二";
                                            energy2.Unit = policy.Unit;
                                            energy2.LageUnit = policy.LageUnit;
                                            energy2.CarbonEmission = factor.EmissionFactor * (endValue - tier1.MaxKwh);
                                            energy2.ConvertCoal = (factor.AvgCalorific * (endValue - tier1.MaxKwh)) / factor.EqCoal;
                                            energy2.TimePeriod = "7";//阶梯二

                                            energy2.del_flag = "0";
                                            energy2.createId = 0;
                                            energy2.create_time = DateTime.Now;
                                            energy2.updateId = 0;
                                            energy2.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy2);
                                        }
                                        //不超出第一阶梯
                                        if (tier1.MinKwh <= totalUse + (endValue - initValue) && totalUse + (endValue - initValue) <= tier1.MaxKwh)
                                        {
                                            //插入能耗记录
                                            T_Prod_Energy energy = new T_Prod_Energy();
                                            energy.Id = _snowflake.NextId().ToString();

                                            energy.OrgId = (long)item.OrgId;
                                            energy.EquipmentId = item.Id;
                                            energy.DDate = BeginTime;
                                            energy.UseVale = endValue - initValue;
                                            energy.CostVale = tier1.Price * (endValue - initValue);
                                            energy.InitVale = initValue;
                                            energy.EndVale = endValue;

                                            energy.PolicyId = policy.Id;
                                            energy.PolicyName = policy.PolicyName;
                                            energy.FactorId = factorType.Id;
                                            energy.FactorName = factorType.TypeName;
                                            energy.DataSource = "2";
                                            energy.Memo = "阶梯一";
                                            energy.Unit = policy.Unit;
                                            energy.LageUnit = policy.LageUnit;
                                            energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                            energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                            energy.TimePeriod = "6";//阶梯一

                                            energy.del_flag = "0";
                                            energy.createId = 0;
                                            energy.create_time = DateTime.Now;
                                            energy.updateId = 0;
                                            energy.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy);
                                        }
                                    }
                                    //第二阶梯
                                    if (tier1.MaxKwh < totalUse && totalUse <= tier2.MaxKwh)
                                    {
                                        //第二阶梯和第三阶梯交界
                                        if (tier2.MaxKwh < totalUse + (endValue - initValue))
                                        {
                                            //插入能耗记录
                                            T_Prod_Energy energy1 = new T_Prod_Energy();
                                            energy1.Id = _snowflake.NextId().ToString();

                                            energy1.OrgId = (long)item.OrgId;
                                            energy1.EquipmentId = item.Id;
                                            energy1.DDate = BeginTime;
                                            energy1.UseVale = tier2.MaxKwh - initValue;
                                            energy1.CostVale = tier2.Price * (tier2.MaxKwh - initValue);
                                            energy1.InitVale = initValue;
                                            energy1.EndVale = tier2.MaxKwh;

                                            energy1.PolicyId = policy.Id;
                                            energy1.PolicyName = policy.PolicyName;
                                            energy1.FactorId = factorType.Id;
                                            energy1.FactorName = factorType.TypeName;
                                            energy1.DataSource = "2";
                                            energy1.Memo = "阶梯二";
                                            energy1.Unit = policy.Unit;
                                            energy1.LageUnit = policy.LageUnit;
                                            energy1.CarbonEmission = factor.EmissionFactor * (tier2.MaxKwh - initValue);
                                            energy1.ConvertCoal = (factor.AvgCalorific * (tier2.MaxKwh - initValue)) / factor.EqCoal;
                                            energy1.TimePeriod = "7";//阶梯二

                                            energy1.del_flag = "0";
                                            energy1.createId = 0;
                                            energy1.create_time = DateTime.Now;
                                            energy1.updateId = 0;
                                            energy1.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy1);

                                            //插入能耗记录
                                            T_Prod_Energy energy2 = new T_Prod_Energy();
                                            energy2.Id = _snowflake.NextId().ToString();

                                            energy2.OrgId = (long)item.OrgId;
                                            energy2.EquipmentId = item.Id;
                                            energy2.DDate = BeginTime;
                                            energy2.UseVale = endValue - tier2.MaxKwh;
                                            energy2.CostVale = tier3.Price * (endValue - tier2.MaxKwh);
                                            energy2.InitVale = tier2.MaxKwh;
                                            energy2.EndVale = endValue;

                                            energy2.PolicyId = policy.Id;
                                            energy2.PolicyName = policy.PolicyName;
                                            energy2.FactorId = factorType.Id;
                                            energy2.FactorName = factorType.TypeName;
                                            energy2.DataSource = "2";
                                            energy2.Memo = "阶梯三";
                                            energy2.Unit = policy.Unit;
                                            energy2.LageUnit = policy.LageUnit;
                                            energy2.CarbonEmission = factor.EmissionFactor * (endValue - tier2.MaxKwh);
                                            energy2.ConvertCoal = (factor.AvgCalorific * (endValue - tier2.MaxKwh)) / factor.EqCoal;
                                            energy2.TimePeriod = "8";//阶梯三

                                            energy2.del_flag = "0";
                                            energy2.createId = 0;
                                            energy2.create_time = DateTime.Now;
                                            energy2.updateId = 0;
                                            energy2.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy2);
                                        }
                                        //不超出第二阶梯
                                        if (totalUse + (endValue - initValue) > tier1.MaxKwh && totalUse + (endValue - initValue) <= tier2.MaxKwh)
                                        {
                                            //插入能耗记录
                                            T_Prod_Energy energy = new T_Prod_Energy();
                                            energy.Id = _snowflake.NextId().ToString();

                                            energy.OrgId = (long)item.OrgId;
                                            energy.EquipmentId = item.Id;
                                            energy.DDate = BeginTime;
                                            energy.UseVale = endValue - initValue;
                                            energy.CostVale = tier2.Price * (endValue - initValue);
                                            energy.InitVale = initValue;
                                            energy.EndVale = endValue;

                                            energy.PolicyId = policy.Id;
                                            energy.PolicyName = policy.PolicyName;
                                            energy.FactorId = factorType.Id;
                                            energy.FactorName = factorType.TypeName;
                                            energy.DataSource = "2";
                                            energy.Memo = "阶梯二";
                                            energy.Unit = policy.Unit;
                                            energy.LageUnit = policy.LageUnit;
                                            energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                            energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                            energy.TimePeriod = "7";//阶梯二

                                            energy.del_flag = "0";
                                            energy.createId = 0;
                                            energy.create_time = DateTime.Now;
                                            energy.updateId = 0;
                                            energy.update_time = DateTime.Now;
                                            await _productionDAL.AddEnergy(energy);
                                        }
                                    }
                                    //第三阶梯
                                    if (tier2.MaxKwh < totalUse)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy = new T_Prod_Energy();
                                        energy.Id = _snowflake.NextId().ToString();

                                        energy.OrgId = (long)item.OrgId;
                                        energy.EquipmentId = item.Id;
                                        energy.DDate = BeginTime;
                                        energy.UseVale = endValue - initValue;
                                        energy.CostVale = tier3.Price * (endValue - initValue);
                                        energy.InitVale = initValue;
                                        energy.EndVale = endValue;

                                        energy.PolicyId = policy.Id;
                                        energy.PolicyName = policy.PolicyName;
                                        energy.FactorId = factorType.Id;
                                        energy.FactorName = factorType.TypeName;
                                        energy.DataSource = "2";
                                        energy.Memo = "阶梯三";
                                        energy.Unit = policy.Unit;
                                        energy.LageUnit = policy.LageUnit;
                                        energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                        energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                        energy.TimePeriod = "8";//阶梯三

                                        energy.del_flag = "0";
                                        energy.createId = 0;
                                        energy.create_time = DateTime.Now;
                                        energy.updateId = 0;
                                        energy.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy);
                                    }
                                }

                                await _productionDAL.DeleteEnergyHour(item.Id, BeginTime);
                                try
                                {
                                    //排放因子类型
                                    T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                    if (factorType == null)
                                    {
                                        //没绑定排放因子类型，下一个
                                        continue;
                                    }
                                    T_Prod_Energy_H eng_h = await _productionDAL.SelectEnergyHourMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                    double initValue = 0;
                                    double endValue = 0;
                                    if (eng_h != null)
                                    {
                                        initValue = eng_h.EndVale;
                                        endValue = eng_h.EndVale;
                                    }
                                    for (global::System.Int32 i = 0; i < 24; i++)
                                    {
                                         initValue = endValue;
                                        //每小时
                                        foreach (var itemHistory in historys)
                                        {
                                            if (itemHistory.UpdatedOn.Value.Hour == i)
                                            {
                                                double result = 0;
                                                Double.TryParse(itemHistory.Value.ToString(), out result);

                                                if (initValue == 0)
                                                {
                                                    initValue = result;
                                                    endValue = result;
                                                }
                                                else
                                                {
                                                    if (result > endValue)
                                                    {
                                                        endValue = result;
                                                    }
                                                }
                                            }
                                        }
                                        T_Prod_Energy_H energy_H = new T_Prod_Energy_H();
                                        energy_H.Id = _snowflake.NextId().ToString();

                                        energy_H.OrgId = (long)item.OrgId;
                                        energy_H.EquipmentId = item.Id;
                                        energy_H.DDate = BeginTime;
                                        energy_H.TTime = i.ToString().PadLeft(2, '0') + ":00-" + (i + 1).ToString().PadLeft(2, '0') + ":00";
                                        energy_H.UseVale = Math.Round(endValue - initValue,2);
                                        energy_H.InitVale = initValue;
                                        energy_H.EndVale = endValue;
                                        energy_H.FactorId = factorType.Id;
                                        energy_H.FactorName = factorType.TypeName;
                                        energy_H.Unit = policy.Unit;
                                        energy_H.LageUnit = policy.LageUnit;

                                        await _productionDAL.AddEnergyHour(energy_H);
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                dateBegin = dateBegin.AddDays(1);
            }
            return BusResponse<string>.Success("生成成功");
        }


        /// <summary>
        /// 生成电消耗数据
        /// 弃用
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ReCalculationEnergyDayHour(string beginDate, string endDate, long orgId)
        {
            //所有绑定了物联网采集模块的设备
            List<T_Com_Equipment> equipments = new List<T_Com_Equipment>();

            List<T_Com_Equipment> AllEquipments = await _commonDAL.SelectEnergyEquipmentList();
            foreach (var item in AllEquipments)
            {
                if (item.OrgId == orgId && item.ThirdId != "-")//同一个组织，并且绑定了第三方编码
                {
                    equipments.Add(item);
                }
            }

            DateTime dateBegin = DateTime.Parse(beginDate);//开始日期
            DateTime dateEnd = DateTime.Parse(endDate);//结束日期

            while (dateBegin <= dateEnd)
            {
                int Year = dateBegin.Year;
                int Month = dateBegin.Month;
                int Day = dateBegin.Day;

                DateTime BeginTime = new DateTime(Year, Month, Day, 0, 0, 0, 0, DateTimeKind.Local);
                DateTime EndTime = new DateTime(Year, Month, Day, 23, 59, 59, 999, DateTimeKind.Local);

                foreach (var item in equipments)
                {
                    //计费政策
                    T_Price_Policy policy = await _policyDAL.SelectPolicy(item.PolicyId);

                    if (policy == null)
                    {
                        //没有绑定,下一个
                        continue;
                    }
                    else
                    {
                        //计费政策明细
                        List<T_Price_PolicyDetil> policyDetil = await _policyDAL.SelectPolicyDetil(item.PolicyId);
                        var tmpOrgConf = await _orgConfDAL.Select(item.OrgId);

                        foreach (var itemDetil in policyDetil)
                        {
                            //生效月份
                            string[] months = itemDetil.PolicyMonth.Split(',');
                            //寻找当月的配置
                            if (months.Contains(EndTime.Month.ToString()))
                            {
                                if (tmpOrgConf == null)
                                {
                                    continue;
                                }

                                //查询物联网采集的历史数据
                                List<DeviceProperty> historys = new List<DeviceProperty>();

                                try
                                {
                                    In_HistoryListSync in_History = new In_HistoryListSync();
                                    in_History.Number = item.ThirdId;
                                    in_History.pageNum = 1;
                                    in_History.pageSize = int.MaxValue;
                                    in_History.BeginTime = BeginTime;
                                    in_History.EndTime = EndTime;
                                    in_History.Code = tmpOrgConf.ElecCode;
                                    historys = (await _iotInflux.SelectHistory(in_History)).List.Where(x => (double)x.Value > 0).OrderBy(x => x.UpdatedOn).ToList();
                                }
                                catch (Exception e)
                                {
                                    continue;
                                    //Random random = new Random();
                                    //int n = 0;
                                    //for (global::System.Int32 i = 0; i < 24; i++)
                                    //{
                                    //    for (global::System.Int32 j = 0; j < 60; j++)
                                    //    {
                                    //        DeviceProperty ax = new DeviceProperty();
                                    //        n = n + 1;
                                    //        ax.Value = n;
                                    //        DateTime dateTime = new DateTime(Year, Month, Day, i, j, 1, 1, DateTimeKind.Local); ;
                                    //        ax.UpdatedOn = dateTime;
                                    //        historys.Add(ax);
                                    //    }
                                    //}
                                }

                                if (itemDetil.PolicyType == "1")//分时
                                {
                                    //分时价格
                                    List<T_Price_Period> periods = (await _policyDAL.SelectPeriod(itemDetil.Id));
                                    if (periods == null)
                                    {
                                        //没绑定价格，下一个
                                        continue;
                                    }

                                    //分时时段
                                    List<T_Price_Time> times = (await _policyDAL.SelectTime(itemDetil.Id));
                                    if (times == null)
                                    {
                                        //没绑定价格，下一个
                                        continue;
                                    }

                                    //排放因子
                                    T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                    if (factor == null)
                                    {
                                        //没绑定排放因子，下一个
                                        continue;
                                    }

                                    //排放因子类型
                                    T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                    if (factorType == null)
                                    {
                                        //没绑定排放因子类型，下一个
                                        continue;
                                    }

                                    await _productionDAL.DeleteEnergyTime(item.Id, BeginTime);
                                    foreach (var itemTime in times)
                                    {
                                        DateTime start = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.StartTime);
                                        DateTime end = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.EndTime);
                                        if (EndTime.Hour == end.Hour && EndTime.Minute == end.Minute)
                                        {
                                            end = EndTime;
                                        }
                                        double price = 0;
                                        foreach (var itemPeriods in periods)
                                        {
                                            if (itemPeriods.TimePeriod == itemTime.TimePeriod)
                                            {
                                                price = itemPeriods.Price;
                                            }
                                        }
                                        double initValue = 0;
                                        double endValue = 0;
                                        for (global::System.Int32 i = 0; i < 24; i++)
                                        {
                                            initValue = endValue;
                                            foreach (var itemHistory in historys)
                                            {
                                                if (itemHistory.UpdatedOn >= start && itemHistory.UpdatedOn <= end && itemHistory.UpdatedOn.Value.Hour == i)
                                                {
                                                    double result = 0;
                                                    Double.TryParse(itemHistory.Value.ToString(), out result);

                                                    if (initValue == 0)
                                                    {
                                                        initValue = result;
                                                        endValue = result;
                                                    }
                                                    else
                                                    {
                                                        if (result > endValue)
                                                        {
                                                            endValue = result;
                                                        }
                                                    }
                                                }
                                            }
                                            T_Prod_Energy_T energy_H = new T_Prod_Energy_T();
                                            energy_H.Id = _snowflake.NextId().ToString();

                                            energy_H.OrgId = (long)item.OrgId;
                                            energy_H.EquipmentId = item.Id;
                                            energy_H.DDate = BeginTime;
                                            energy_H.TTime = i.ToString().PadLeft(2, '0') + ":00-" + (i + 1).ToString().PadLeft(2, '0') + ":00";
                                            energy_H.UseVale = Math.Round(endValue - initValue, 2);
                                            energy_H.InitVale = initValue;
                                            energy_H.EndVale = endValue;
                                            energy_H.FactorId = factorType.Id;
                                            energy_H.FactorName = factorType.TypeName;
                                            energy_H.Unit = policy.Unit;
                                            energy_H.LageUnit = policy.LageUnit;
                                            energy_H.CostVale = Math.Round(energy_H.UseVale * price, 2);
                                            energy_H.CarbonEmission = Math.Round(energy_H.UseVale * factor.EmissionFactor, 2);
                                            energy_H.ConvertCoal = Math.Round((energy_H.UseVale * factor.AvgCalorific) / factor.EqCoal, 2);

                                            if (itemTime.TimePeriod == "1")
                                            {
                                                energy_H.TimePeriod = "尖";
                                            }
                                            else if (itemTime.TimePeriod == "2")
                                            {
                                                energy_H.TimePeriod = "峰";
                                            }
                                            else if (itemTime.TimePeriod == "3")
                                            {
                                                energy_H.TimePeriod = "平";
                                            }
                                            else if (itemTime.TimePeriod == "4")
                                            {
                                                energy_H.TimePeriod = "谷";
                                            }

                                            await _productionDAL.AddEnergyTime(energy_H);
                                        }
                                    }
                                }

                                await _productionDAL.DeleteEnergyHour(item.Id, BeginTime);
                                try
                                {
                                    //排放因子类型
                                    T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                    if (factorType == null)
                                    {
                                        //没绑定排放因子类型，下一个
                                        continue;
                                    }
                                    double initValue = 0;
                                    double endValue = 0;
                                    for (global::System.Int32 i = 0; i < 24; i++)
                                    {
                                        initValue = endValue;
                                        //每小时
                                        foreach (var itemHistory in historys)
                                        {
                                            if (itemHistory.UpdatedOn.Value.Hour == i)
                                            {
                                                double result = 0;
                                                Double.TryParse(itemHistory.Value.ToString(), out result);

                                                if (initValue == 0)
                                                {
                                                    initValue = result;
                                                    endValue = result;
                                                }
                                                else
                                                {
                                                    if (result > endValue)
                                                    {
                                                        endValue = result;
                                                    }
                                                }
                                            }
                                        }
                                        T_Prod_Energy_H energy_H = new T_Prod_Energy_H();
                                        energy_H.Id = _snowflake.NextId().ToString();

                                        energy_H.OrgId = (long)item.OrgId;
                                        energy_H.EquipmentId = item.Id;
                                        energy_H.DDate = BeginTime;
                                        energy_H.TTime = i.ToString().PadLeft(2, '0') + ":00-" + (i + 1).ToString().PadLeft(2, '0') + ":00";
                                        energy_H.UseVale = Math.Round(endValue - initValue, 2);
                                        energy_H.InitVale = initValue;
                                        energy_H.EndVale = endValue;
                                        energy_H.FactorId = factorType.Id;
                                        energy_H.FactorName = factorType.TypeName;
                                        energy_H.Unit = policy.Unit;
                                        energy_H.LageUnit = policy.LageUnit;

                                        await _productionDAL.AddEnergyHour(energy_H);
                                    }
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                dateBegin = dateBegin.AddDays(1);
            }
            return BusResponse<string>.Success("生成成功");
        }
        #endregion

        #region 定时执行能耗计算
        /// <summary>
        /// 定时执行能耗计算
        /// </summary>
        /// <returns></returns>
        public virtual async Task ExecuteEnergy(QuartzContext context)
        {
            //所有绑定了物联网采集模块的设备
            List<T_Com_Equipment> equipments = await _commonDAL.SelectEnergyEquipmentList();

            //多个企业的上次查询记录
            Dictionary<string, T_Prod_Config> configs = new Dictionary<string, T_Prod_Config>();

            //查询结束时间（使用调度时间，防止任务失火）
            var lastday = context.ScheduledFireTimeUtc.Value.AddDays(-1).LocalDateTime;
            DateTime BeginTime = new DateTime(lastday.Year, lastday.Month, lastday.Day, 0, 0, 0, 0, DateTimeKind.Local);
            DateTime EndTime = new DateTime(lastday.Year, lastday.Month, lastday.Day, 23, 59, 59, 999, DateTimeKind.Local);

            foreach (var item in equipments)
            {
                //上次查询的记录
                T_Prod_Config config;
                if (configs.ContainsKey(item.OrgId.ToString()))
                {
                    config = configs[item.OrgId.ToString()];
                }
                else
                {
                    config = await _productionDAL.SelectConfig(item.OrgId.ToString());
                    if (config == null)
                    {
                        config = new T_Prod_Config();
                        config.Id = item.OrgId.ToString();
                        config.BeginTime = BeginTime;
                        config.EndTime = EndTime;
                        config.TotalUse = 0;
                        config.TotalTime = DateTime.Now;
                        await _productionDAL.AddConfig(config);
                    }
                    if (config.EndTime == EndTime)//同一天已经生成过了，不重复
                    {
                        continue;
                    }
                    config.BeginTime = BeginTime;
                    config.EndTime = EndTime;
                    configs.Add(item.OrgId.ToString(), config);
                }
                //计费政策
                T_Price_Policy policy = await _policyDAL.SelectPolicy(item.PolicyId);

                if (policy == null)
                {
                    //没有绑定,下一个
                    continue;
                }
                else
                {
                    //计费政策明细
                    List<T_Price_PolicyDetil> policyDetil = await _policyDAL.SelectPolicyDetil(item.PolicyId);
                    var tmpOrgConf = await _orgConfDAL.Select(item.OrgId);

                    foreach (var itemDetil in policyDetil)
                    {
                        //生效月份
                        string[] months = itemDetil.PolicyMonth.Split(',');
                        //寻找当月的配置
                        if (months.Contains(EndTime.Month.ToString()))
                        {
                            if (tmpOrgConf == null)
                            {
                                continue;
                            }

                            //查询物联网采集的历史数据
                            List<DeviceProperty> historys = new List<DeviceProperty>();
                            try
                            {
                                //查询物联网采集的历史数据
                                In_HistoryListSync in_History = new In_HistoryListSync();
                                in_History.Number = item.ThirdId;
                                in_History.pageNum = 1;
                                in_History.pageSize = int.MaxValue;
                                in_History.BeginTime = BeginTime;
                                in_History.EndTime = EndTime;
                                in_History.Code = tmpOrgConf.ElecCode;
                                historys = (await _iotInflux.SelectHistory(in_History)).List.Where(x => (double)x.Value > 0).OrderBy(x => x.UpdatedOn).ToList();
                            }
                            catch (Exception e)
                            {
                                continue;
                            }

                            if (itemDetil.PolicyType == "1")//分时
                            {
                                //分时价格
                                List<T_Price_Period> periods = (await _policyDAL.SelectPeriod(itemDetil.Id));
                                if (periods == null)
                                {
                                    //没绑定价格，下一个
                                    continue;
                                }

                                //分时时段
                                List<T_Price_Time> times = (await _policyDAL.SelectTime(itemDetil.Id)).OrderBy(e => e.StartTime).ToList(); ;
                                if (times == null)
                                {
                                    //没绑定价格，下一个
                                    continue;
                                }

                                //排放因子
                                T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                if (factor == null)
                                {
                                    //没绑定排放因子，下一个
                                    continue;
                                }

                                //排放因子类型
                                T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                if (factorType == null)
                                {
                                    //没绑定排放因子类型，下一个
                                    continue;
                                }
                                Out_Energy eng = await _productionDAL.SelectEnergyMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                double initValue = 0;
                                double endValue = 0;
                                if (eng != null)
                                {
                                    initValue = eng.EndVale;
                                    endValue = eng.EndVale;
                                }
                                foreach (var itemTime in times)
                                {
                                    DateTime start = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.StartTime);
                                    DateTime end = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.EndTime);
                                    if (EndTime.Hour == end.Hour && EndTime.Minute == end.Minute)
                                    {
                                        end = EndTime;
                                    }
                                    initValue = endValue;
                                    foreach (var itemHistory in historys)
                                    {
                                        if (itemHistory.UpdatedOn >= start && itemHistory.UpdatedOn <= end)
                                        {
                                            double result = 0;
                                            Double.TryParse(itemHistory.Value.ToString(), out result);

                                            if (initValue == 0)
                                            {
                                                initValue = result;
                                                endValue = result;
                                            }
                                            else
                                            {
                                                if (result > endValue)
                                                {
                                                    endValue = result;
                                                }
                                            }
                                        }
                                    }

                                    double price = 0;
                                    foreach (var itemPeriods in periods)
                                    {
                                        if (itemPeriods.TimePeriod == itemTime.TimePeriod)
                                        {
                                            price = itemPeriods.Price;
                                        }
                                    }


                                    //插入能耗记录
                                    T_Prod_Energy energy = new T_Prod_Energy();
                                    energy.Id = _snowflake.NextId().ToString();

                                    energy.OrgId = (long)item.OrgId;
                                    energy.EquipmentId = item.Id;
                                    energy.DDate = BeginTime;
                                    energy.UseVale = endValue - initValue;
                                    energy.CostVale = price * (endValue - initValue);
                                    energy.InitVale = initValue;
                                    energy.EndVale = endValue;

                                    energy.PolicyId = policy.Id;
                                    energy.PolicyName = policy.PolicyName;
                                    energy.FactorId = factorType.Id;
                                    energy.FactorName = factorType.TypeName;
                                    energy.Memo = itemTime.StartTime + "-" + itemTime.EndTime;
                                    energy.DataSource = "2";
                                    energy.Unit = policy.Unit;
                                    energy.LageUnit = policy.LageUnit;
                                    energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                    energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                    energy.TimePeriod = itemTime.TimePeriod;

                                    energy.del_flag = "0";
                                    energy.createId = 0;
                                    energy.create_time = DateTime.Now;
                                    energy.updateId = 0;
                                    energy.update_time = DateTime.Now;
                                    await _productionDAL.AddEnergy(energy);
                                }

                                T_Prod_Energy_T eng_t = await _productionDAL.SelectEnergyTimeMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                if (eng_t == null)
                                {
                                    initValue = 0;
                                    endValue = 0;
                                }
                                else
                                {
                                    initValue = eng_t.EndVale;
                                    endValue = eng_t.EndVale;
                                }
                                await _productionDAL.DeleteEnergyTime(item.Id, BeginTime);
                                foreach (var itemTime in times)
                                {
                                    DateTime start = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.StartTime);
                                    DateTime end = DateTime.Parse(EndTime.ToString("yyyy-MM-dd ") + itemTime.EndTime);
                                    if (EndTime.Hour == end.Hour && EndTime.Minute == end.Minute)
                                    {
                                        end = EndTime;
                                    }
                                    double price = 0;
                                    foreach (var itemPeriods in periods)
                                    {
                                        if (itemPeriods.TimePeriod == itemTime.TimePeriod)
                                        {
                                            price = itemPeriods.Price;
                                        }
                                    }
                                    for (global::System.Int32 i = 0; i < 24; i++)
                                    {
                                        initValue = endValue;
                                        foreach (var itemHistory in historys)
                                        {
                                            if (itemHistory.UpdatedOn >= start && itemHistory.UpdatedOn <= end && itemHistory.UpdatedOn.Value.Hour == i)
                                            {
                                                double result = 0;
                                                Double.TryParse(itemHistory.Value.ToString(), out result);

                                                if (initValue == 0)
                                                {
                                                    initValue = result;
                                                    endValue = result;
                                                }
                                                else
                                                {
                                                    if (result > endValue)
                                                    {
                                                        endValue = result;
                                                    }
                                                }
                                            }
                                        }
                                        T_Prod_Energy_T energy_H = new T_Prod_Energy_T();
                                        energy_H.Id = _snowflake.NextId().ToString();

                                        energy_H.OrgId = (long)item.OrgId;
                                        energy_H.EquipmentId = item.Id;
                                        energy_H.DDate = BeginTime;
                                        energy_H.TTime = i.ToString().PadLeft(2, '0') + ":00-" + (i + 1).ToString().PadLeft(2, '0') + ":00";
                                        energy_H.UseVale = Math.Round(endValue - initValue, 2);
                                        energy_H.InitVale = initValue;
                                        energy_H.EndVale = endValue;
                                        energy_H.FactorId = factorType.Id;
                                        energy_H.FactorName = factorType.TypeName;
                                        energy_H.Unit = policy.Unit;
                                        energy_H.LageUnit = policy.LageUnit;
                                        energy_H.CostVale = Math.Round(energy_H.UseVale * price, 2);
                                        energy_H.CarbonEmission = Math.Round(energy_H.UseVale * factor.EmissionFactor, 2);
                                        energy_H.ConvertCoal = Math.Round((energy_H.UseVale * factor.AvgCalorific) / factor.EqCoal, 2);

                                        if (itemTime.TimePeriod == "1")
                                        {
                                            energy_H.TimePeriod = "尖";
                                        }
                                        else if (itemTime.TimePeriod == "2")
                                        {
                                            energy_H.TimePeriod = "峰";
                                        }
                                        else if (itemTime.TimePeriod == "3")
                                        {
                                            energy_H.TimePeriod = "平";
                                        }
                                        else if (itemTime.TimePeriod == "4")
                                        {
                                            energy_H.TimePeriod = "谷";
                                        }
                                        if (energy_H.UseVale > 0)
                                        {
                                            await _productionDAL.AddEnergyTime(energy_H);
                                        }
                                    }
                                }
                            }
                            else if (itemDetil.PolicyType == "2")//全天
                            {
                                //全天价格,只有一条
                                T_Price_Period periods = (await _policyDAL.SelectPeriod(itemDetil.Id)).First();
                                if (periods == null)
                                {
                                    continue;
                                }

                                //排放因子
                                T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                if (factor == null)
                                {
                                    continue;
                                }

                                //排放因子类型
                                T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                if (factorType == null)
                                {
                                    //没绑定排放因子类型，下一个
                                    continue;
                                }

                                Out_Energy eng = await _productionDAL.SelectEnergyMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                double initValue = 0;
                                double endValue = 0;
                                if (eng != null)
                                {
                                    initValue = eng.EndVale;
                                    endValue = eng.EndVale;
                                }
                                foreach (var itemHistory in historys)
                                {
                                    double result = 0;
                                    Double.TryParse(itemHistory.Value.ToString(), out result);

                                    if (initValue == 0)
                                    {
                                        initValue = result;
                                        endValue = result;
                                    }
                                    else
                                    {
                                        if (result > endValue)
                                        {
                                            endValue = result;
                                        }
                                    }
                                }

                                //插入能耗记录
                                T_Prod_Energy energy = new T_Prod_Energy();
                                energy.Id = _snowflake.NextId().ToString();

                                energy.OrgId = (long)item.OrgId;
                                energy.EquipmentId = item.Id;
                                energy.DDate = BeginTime;
                                energy.UseVale = endValue - initValue;
                                energy.CostVale = periods.Price * (endValue - initValue);
                                energy.InitVale = initValue;
                                energy.EndVale = endValue;

                                energy.PolicyId = policy.Id;
                                energy.PolicyName = policy.PolicyName;
                                energy.FactorId = factorType.Id;
                                energy.FactorName = factorType.TypeName;
                                energy.Memo = "全天";
                                energy.DataSource = "2";
                                energy.Unit = policy.Unit;
                                energy.LageUnit = policy.LageUnit;
                                energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                energy.TimePeriod = "5";//5代表全天

                                energy.del_flag = "0";
                                energy.createId = 0;
                                energy.create_time = DateTime.Now;
                                energy.updateId = 0;
                                energy.update_time = DateTime.Now;
                                await _productionDAL.AddEnergy(energy);
                            }
                            else if (itemDetil.PolicyType == "3")//阶梯
                            {
                                //阶梯价格
                                List<T_Price_Tier> tiers = (await _policyDAL.SelectTier(itemDetil.Id));
                                if (tiers == null)
                                {
                                    continue;
                                }

                                //排放因子
                                T_ENG_Factor factor = await _factorDAL.SelectFactor(policy.FactorId);
                                if (factor == null)
                                {
                                    continue;
                                }

                                //排放因子类型
                                T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                if (factorType == null)
                                {
                                    //没绑定排放因子类型，下一个
                                    continue;
                                }

                                double initValue = 0;
                                double endValue = 0;
                                foreach (var itemHistory in historys)
                                {
                                    double result = 0;
                                    Double.TryParse(itemHistory.Value.ToString(), out result);

                                    if (initValue == 0)
                                    {
                                        initValue = result;
                                        endValue = result;
                                    }
                                    else
                                    {
                                        if (result > endValue)
                                        {
                                            endValue = result;
                                        }
                                    }
                                }

                                InEnergyDayPageList query = new InEnergyDayPageList();
                                query.OrgId = item.OrgId;
                                query.DateType = "月";
                                query.FactorId = factorType.Id;
                                query.BeginDate = new DateTime(BeginTime.Year, BeginTime.Month, 1).ToString("yyyy-MM-dd");
                                query.EndDate = new DateTime(BeginTime.Year, BeginTime.Month, DateTime.DaysInMonth(BeginTime.Year, BeginTime.Month)).ToString("yyyy-MM-dd");
                                List<Out_EnergyDay> EnergyDays = (await _productionDAL.SelectEnergyDatePage(query)).List;

                                double totalUse = 0;
                                foreach (var days in EnergyDays)
                                {
                                    totalUse += days.UseVale;
                                }

                                T_Price_Tier tier1 = new T_Price_Tier();
                                T_Price_Tier tier2 = new T_Price_Tier();
                                T_Price_Tier tier3 = new T_Price_Tier();
                                foreach (var itemTier in tiers)
                                {
                                    if (itemTier.TierLevel == "1")
                                    {
                                        tier1 = itemTier;
                                    }
                                    else if (itemTier.TierLevel == "2")
                                    {
                                        tier2 = itemTier;
                                    }
                                    else if (itemTier.TierLevel == "3")
                                    {
                                        tier3 = itemTier;
                                    }
                                }
                                //第一阶梯
                                if (tier1.MinKwh <= totalUse && totalUse <= tier1.MaxKwh)
                                {
                                    //超出第二阶梯
                                    if (totalUse + (endValue - initValue) > tier2.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy1 = new T_Prod_Energy();
                                        energy1.Id = _snowflake.NextId().ToString();

                                        energy1.OrgId = (long)item.OrgId;
                                        energy1.EquipmentId = item.Id;
                                        energy1.DDate = BeginTime;
                                        energy1.UseVale = tier1.MaxKwh - initValue;
                                        energy1.CostVale = tier1.Price * (tier1.MaxKwh - initValue);
                                        energy1.InitVale = initValue;
                                        energy1.EndVale = tier1.MaxKwh;

                                        energy1.PolicyId = policy.Id;
                                        energy1.PolicyName = policy.PolicyName;
                                        energy1.FactorId = factorType.Id;
                                        energy1.FactorName = factorType.TypeName;
                                        energy1.DataSource = "2";
                                        energy1.Memo = "阶梯一";
                                        energy1.Unit = policy.Unit;
                                        energy1.LageUnit = policy.LageUnit;
                                        energy1.CarbonEmission = factor.EmissionFactor * (tier1.MaxKwh - initValue);
                                        energy1.ConvertCoal = (factor.AvgCalorific * (tier1.MaxKwh - initValue)) / factor.EqCoal;
                                        energy1.TimePeriod = "6";//阶梯一

                                        energy1.del_flag = "0";
                                        energy1.createId = 0;
                                        energy1.create_time = DateTime.Now;
                                        energy1.updateId = 0;
                                        energy1.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy1);

                                        //插入能耗记录
                                        T_Prod_Energy energy2 = new T_Prod_Energy();
                                        energy2.Id = _snowflake.NextId().ToString();

                                        energy2.OrgId = (long)item.OrgId;
                                        energy2.EquipmentId = item.Id;
                                        energy2.DDate = BeginTime;
                                        energy2.UseVale = tier2.MaxKwh - tier1.MaxKwh;
                                        energy2.CostVale = tier2.Price * (tier2.MaxKwh - tier1.MaxKwh);
                                        energy2.InitVale = tier1.MaxKwh;
                                        energy2.EndVale = tier2.MaxKwh;

                                        energy2.PolicyId = policy.Id;
                                        energy2.PolicyName = policy.PolicyName;
                                        energy2.FactorId = factorType.Id;
                                        energy2.FactorName = factorType.TypeName;
                                        energy2.DataSource = "2";
                                        energy2.Memo = "阶梯二";
                                        energy2.Unit = policy.Unit;
                                        energy2.LageUnit = policy.LageUnit;
                                        energy2.CarbonEmission = factor.EmissionFactor * (tier2.MaxKwh - tier1.MaxKwh);
                                        energy2.ConvertCoal = (factor.AvgCalorific * (tier2.MaxKwh - tier1.MaxKwh)) / factor.EqCoal;
                                        energy2.TimePeriod = "7";//阶梯二

                                        energy2.del_flag = "0";
                                        energy2.createId = 0;
                                        energy2.create_time = DateTime.Now;
                                        energy2.updateId = 0;
                                        energy2.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy2);

                                        //插入能耗记录
                                        T_Prod_Energy energy3 = new T_Prod_Energy();
                                        energy3.Id = _snowflake.NextId().ToString();

                                        energy3.OrgId = (long)item.OrgId;
                                        energy3.EquipmentId = item.Id;
                                        energy3.DDate = BeginTime;
                                        energy3.UseVale = endValue - tier2.MaxKwh;
                                        energy3.CostVale = tier2.Price * (endValue - tier2.MaxKwh);
                                        energy3.InitVale = tier2.MaxKwh;
                                        energy3.EndVale = endValue;

                                        energy3.PolicyId = policy.Id;
                                        energy3.PolicyName = policy.PolicyName;
                                        energy3.FactorId = factorType.Id;
                                        energy3.FactorName = factorType.TypeName;
                                        energy3.DataSource = "2";
                                        energy3.Memo = "阶梯三";
                                        energy3.Unit = policy.Unit;
                                        energy3.LageUnit = policy.LageUnit;
                                        energy3.CarbonEmission = factor.EmissionFactor * (endValue - tier2.MaxKwh);
                                        energy3.ConvertCoal = (factor.AvgCalorific * (endValue - tier2.MaxKwh)) / factor.EqCoal;
                                        energy3.TimePeriod = "8";//阶梯三

                                        energy3.del_flag = "0";
                                        energy3.createId = 0;
                                        energy3.create_time = DateTime.Now;
                                        energy3.updateId = 0;
                                        energy3.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy3);

                                    }
                                    //超出第一阶梯,不超出第二阶梯
                                    if (totalUse + (endValue - initValue) > tier1.MaxKwh && totalUse + (endValue - initValue) <= tier2.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy1 = new T_Prod_Energy();
                                        energy1.Id = _snowflake.NextId().ToString();

                                        energy1.OrgId = (long)item.OrgId;
                                        energy1.EquipmentId = item.Id;
                                        energy1.DDate = BeginTime;
                                        energy1.UseVale = tier1.MaxKwh - initValue;
                                        energy1.CostVale = tier1.Price * (tier1.MaxKwh - initValue);
                                        energy1.InitVale = initValue;
                                        energy1.EndVale = tier1.MaxKwh;

                                        energy1.PolicyId = policy.Id;
                                        energy1.PolicyName = policy.PolicyName;
                                        energy1.FactorId = factorType.Id;
                                        energy1.FactorName = factorType.TypeName;
                                        energy1.DataSource = "2";
                                        energy1.Memo = "阶梯一";
                                        energy1.Unit = policy.Unit;
                                        energy1.LageUnit = policy.LageUnit;
                                        energy1.CarbonEmission = factor.EmissionFactor * (tier1.MaxKwh - initValue);
                                        energy1.ConvertCoal = (factor.AvgCalorific * (tier1.MaxKwh - initValue)) / factor.EqCoal;
                                        energy1.TimePeriod = "6";//阶梯一

                                        energy1.del_flag = "0";
                                        energy1.createId = 0;
                                        energy1.create_time = DateTime.Now;
                                        energy1.updateId = 0;
                                        energy1.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy1);

                                        //插入能耗记录
                                        T_Prod_Energy energy2 = new T_Prod_Energy();
                                        energy2.Id = _snowflake.NextId().ToString();

                                        energy2.OrgId = (long)item.OrgId;
                                        energy2.EquipmentId = item.Id;
                                        energy2.DDate = BeginTime;
                                        energy2.UseVale = endValue - tier1.MaxKwh;
                                        energy2.CostVale = tier2.Price * (endValue - tier1.MaxKwh);
                                        energy2.InitVale = tier1.MaxKwh;
                                        energy2.EndVale = endValue;

                                        energy2.PolicyId = policy.Id;
                                        energy2.PolicyName = policy.PolicyName;
                                        energy2.FactorId = factorType.Id;
                                        energy2.FactorName = factorType.TypeName;
                                        energy2.DataSource = "2";
                                        energy2.Memo = "阶梯二";
                                        energy2.Unit = policy.Unit;
                                        energy2.LageUnit = policy.LageUnit;
                                        energy2.CarbonEmission = factor.EmissionFactor * (endValue - tier1.MaxKwh);
                                        energy2.ConvertCoal = (factor.AvgCalorific * (endValue - tier1.MaxKwh)) / factor.EqCoal;
                                        energy2.TimePeriod = "7";//阶梯二

                                        energy2.del_flag = "0";
                                        energy2.createId = 0;
                                        energy2.create_time = DateTime.Now;
                                        energy2.updateId = 0;
                                        energy2.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy2);
                                    }
                                    //不超出第一阶梯
                                    if (tier1.MinKwh <= totalUse + (endValue - initValue) && totalUse + (endValue - initValue) <= tier1.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy = new T_Prod_Energy();
                                        energy.Id = _snowflake.NextId().ToString();

                                        energy.OrgId = (long)item.OrgId;
                                        energy.EquipmentId = item.Id;
                                        energy.DDate = BeginTime;
                                        energy.UseVale = endValue - initValue;
                                        energy.CostVale = tier1.Price * (endValue - initValue);
                                        energy.InitVale = initValue;
                                        energy.EndVale = endValue;

                                        energy.PolicyId = policy.Id;
                                        energy.PolicyName = policy.PolicyName;
                                        energy.FactorId = factorType.Id;
                                        energy.FactorName = factorType.TypeName;
                                        energy.DataSource = "2";
                                        energy.Memo = "阶梯一";
                                        energy.Unit = policy.Unit;
                                        energy.LageUnit = policy.LageUnit;
                                        energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                        energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                        energy.TimePeriod = "6";//阶梯一

                                        energy.del_flag = "0";
                                        energy.createId = 0;
                                        energy.create_time = DateTime.Now;
                                        energy.updateId = 0;
                                        energy.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy);
                                    }
                                }
                                //第二阶梯
                                if (tier1.MaxKwh < totalUse && totalUse <= tier2.MaxKwh)
                                {
                                    //第二阶梯和第三阶梯交界
                                    if (tier2.MaxKwh < totalUse + (endValue - initValue))
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy1 = new T_Prod_Energy();
                                        energy1.Id = _snowflake.NextId().ToString();

                                        energy1.OrgId = (long)item.OrgId;
                                        energy1.EquipmentId = item.Id;
                                        energy1.DDate = BeginTime;
                                        energy1.UseVale = tier2.MaxKwh - initValue;
                                        energy1.CostVale = tier2.Price * (tier2.MaxKwh - initValue);
                                        energy1.InitVale = initValue;
                                        energy1.EndVale = tier2.MaxKwh;

                                        energy1.PolicyId = policy.Id;
                                        energy1.PolicyName = policy.PolicyName;
                                        energy1.FactorId = factorType.Id;
                                        energy1.FactorName = factorType.TypeName;
                                        energy1.DataSource = "2";
                                        energy1.Memo = "阶梯二";
                                        energy1.Unit = policy.Unit;
                                        energy1.LageUnit = policy.LageUnit;
                                        energy1.CarbonEmission = factor.EmissionFactor * (tier2.MaxKwh - initValue);
                                        energy1.ConvertCoal = (factor.AvgCalorific * (tier2.MaxKwh - initValue)) / factor.EqCoal;
                                        energy1.TimePeriod = "7";//阶梯二

                                        energy1.del_flag = "0";
                                        energy1.createId = 0;
                                        energy1.create_time = DateTime.Now;
                                        energy1.updateId = 0;
                                        energy1.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy1);

                                        //插入能耗记录
                                        T_Prod_Energy energy2 = new T_Prod_Energy();
                                        energy2.Id = _snowflake.NextId().ToString();

                                        energy2.OrgId = (long)item.OrgId;
                                        energy2.EquipmentId = item.Id;
                                        energy2.DDate = BeginTime;
                                        energy2.UseVale = endValue - tier2.MaxKwh;
                                        energy2.CostVale = tier3.Price * (endValue - tier2.MaxKwh);
                                        energy2.InitVale = tier2.MaxKwh;
                                        energy2.EndVale = endValue;

                                        energy2.PolicyId = policy.Id;
                                        energy2.PolicyName = policy.PolicyName;
                                        energy2.FactorId = factorType.Id;
                                        energy2.FactorName = factorType.TypeName;
                                        energy2.DataSource = "2";
                                        energy2.Memo = "阶梯三";
                                        energy2.Unit = policy.Unit;
                                        energy2.LageUnit = policy.LageUnit;
                                        energy2.CarbonEmission = factor.EmissionFactor * (endValue - tier2.MaxKwh);
                                        energy2.ConvertCoal = (factor.AvgCalorific * (endValue - tier2.MaxKwh)) / factor.EqCoal;
                                        energy2.TimePeriod = "8";//阶梯三

                                        energy2.del_flag = "0";
                                        energy2.createId = 0;
                                        energy2.create_time = DateTime.Now;
                                        energy2.updateId = 0;
                                        energy2.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy2);
                                    }
                                    //不超出第二阶梯
                                    if (totalUse + (endValue - initValue) > tier1.MaxKwh && totalUse + (endValue - initValue) <= tier2.MaxKwh)
                                    {
                                        //插入能耗记录
                                        T_Prod_Energy energy = new T_Prod_Energy();
                                        energy.Id = _snowflake.NextId().ToString();

                                        energy.OrgId = (long)item.OrgId;
                                        energy.EquipmentId = item.Id;
                                        energy.DDate = BeginTime;
                                        energy.UseVale = endValue - initValue;
                                        energy.CostVale = tier2.Price * (endValue - initValue);
                                        energy.InitVale = initValue;
                                        energy.EndVale = endValue;

                                        energy.PolicyId = policy.Id;
                                        energy.PolicyName = policy.PolicyName;
                                        energy.FactorId = factorType.Id;
                                        energy.FactorName = factorType.TypeName;
                                        energy.DataSource = "2";
                                        energy.Memo = "阶梯二";
                                        energy.Unit = policy.Unit;
                                        energy.LageUnit = policy.LageUnit;
                                        energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                        energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                        energy.TimePeriod = "7";//阶梯二

                                        energy.del_flag = "0";
                                        energy.createId = 0;
                                        energy.create_time = DateTime.Now;
                                        energy.updateId = 0;
                                        energy.update_time = DateTime.Now;
                                        await _productionDAL.AddEnergy(energy);
                                    }
                                }
                                //第三阶梯
                                if (tier2.MaxKwh < totalUse)
                                {
                                    //插入能耗记录
                                    T_Prod_Energy energy = new T_Prod_Energy();
                                    energy.Id = _snowflake.NextId().ToString();

                                    energy.OrgId = (long)item.OrgId;
                                    energy.EquipmentId = item.Id;
                                    energy.DDate = BeginTime;
                                    energy.UseVale = endValue - initValue;
                                    energy.CostVale = tier3.Price * (endValue - initValue);
                                    energy.InitVale = initValue;
                                    energy.EndVale = endValue;

                                    energy.PolicyId = policy.Id;
                                    energy.PolicyName = policy.PolicyName;
                                    energy.FactorId = factorType.Id;
                                    energy.FactorName = factorType.TypeName;
                                    energy.DataSource = "2";
                                    energy.Memo = "阶梯三";
                                    energy.Unit = policy.Unit;
                                    energy.LageUnit = policy.LageUnit;
                                    energy.CarbonEmission = factor.EmissionFactor * (endValue - initValue);
                                    energy.ConvertCoal = (factor.AvgCalorific * (endValue - initValue)) / factor.EqCoal;
                                    energy.TimePeriod = "8";//阶梯三

                                    energy.del_flag = "0";
                                    energy.createId = 0;
                                    energy.create_time = DateTime.Now;
                                    energy.updateId = 0;
                                    energy.update_time = DateTime.Now;
                                    await _productionDAL.AddEnergy(energy);
                                }
                                //更新累计值
                                configs[item.OrgId.ToString()].TotalUse = totalUse + (endValue - initValue);
                                configs[item.OrgId.ToString()].TotalTime = DateTime.Now;
                            }

                            await _productionDAL.DeleteEnergyHour(item.Id, BeginTime);
                            try
                            {
                                //排放因子类型
                                T_ENG_FactorType factorType = await _factorDAL.SelectFactorTypeInfo(policy.EnergyType);
                                if (factorType == null)
                                {
                                    //没绑定排放因子类型，下一个
                                    continue;
                                }
                                T_Prod_Energy_H eng_h = await _productionDAL.SelectEnergyHourMax(EndTime.ToString("yyyy-MM-dd"), item.Id);
                                double initValue = 0;
                                double endValue = 0;
                                if (eng_h != null)
                                {
                                    initValue = eng_h.EndVale;
                                    endValue = eng_h.EndVale;
                                }
                                for (global::System.Int32 i = 0; i < 24; i++)
                                {
                                    initValue = endValue;
                                    //每小时
                                    foreach (var itemHistory in historys)
                                    {
                                        if (itemHistory.UpdatedOn.Value.Hour == i)
                                        {
                                            double result = 0;
                                            Double.TryParse(itemHistory.Value.ToString(), out result);

                                            if (initValue == 0)
                                            {
                                                initValue = result;
                                                endValue = result;
                                            }
                                            else
                                            {
                                                if (result > endValue)
                                                {
                                                    endValue = result;
                                                }
                                            }
                                        }
                                    }
                                    T_Prod_Energy_H energy_H = new T_Prod_Energy_H();
                                    energy_H.Id = _snowflake.NextId().ToString();

                                    energy_H.OrgId = (long)item.OrgId;
                                    energy_H.EquipmentId = item.Id;
                                    energy_H.DDate = BeginTime;
                                    energy_H.TTime = i.ToString().PadLeft(2, '0') + ":00-" + (i + 1).ToString().PadLeft(2, '0') + ":00";
                                    energy_H.UseVale = Math.Round(endValue - initValue, 2);
                                    energy_H.InitVale = initValue;
                                    energy_H.EndVale = endValue;
                                    energy_H.FactorId = factorType.Id;
                                    energy_H.FactorName = factorType.TypeName;
                                    energy_H.Unit = policy.Unit;
                                    energy_H.LageUnit = policy.LageUnit;

                                    await _productionDAL.AddEnergyHour(energy_H);
                                }
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }

                }
            }

            //更新查询记录
            foreach (var item in configs.Keys)
            {
                await _productionDAL.UpdateConfig(configs[item]);
            }
        }
        #endregion

        #region 碳排分析
        /// <summary>
        /// 按排放类型分析报表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_CarbonReportType>> CarbonReportByType(In_CarbonReport query)
        {            
            List<Out_CarbonReportType> carbonReportType = new List<Out_CarbonReportType>();

            //能耗数据
            InEnergyTree inEnergyTree = new InEnergyTree();
            inEnergyTree.OrgId = query.OrgId;
            inEnergyTree.BeginDate = query.BeginDate;
            inEnergyTree.EndDate = query.EndDate;
            List<Out_EnergyList> listEnergies = await _productionDAL.SelectEnergyList(inEnergyTree);

            //产值数据
            InProductionPageList inProduction = new InProductionPageList();
            inProduction.OrgId = query.OrgId;
            inProduction.BeginDate = query.BeginDate;
            inProduction.EndDate = query.EndDate;
            List<Out_Production> listProductions = (await _productionDAL.SelectProductionPage(inProduction)).List;

            //所有设施
            List<T_Com_Facility> facilityList = await _commonDAL.SelectFacilityTree((long)query.OrgId);

            //把六大类型查出来
            List<Out_Class> out_Class = await _commonDAL.SelectClassAll();
            foreach (var item in out_Class)
            {
                Out_CarbonReportType carbonReport = new Out_CarbonReportType();
                carbonReport.Id = item.Id;
                carbonReport.ClassNo = item.ClassNo;
                carbonReport.ClassName = item.ClassName;
                carbonReport.RangeId = item.RangeId;
                //所有子类型
                List<T_OrgClass> OrgClasses = await _commonDAL.SelectOrgClass(query.OrgId, item.Id);

                Dictionary<string,List<T_OrgClassCarbonReport>> OrgClassCarbonReports = new Dictionary<string,List<T_OrgClassCarbonReport>>();
                foreach (var T_OrgClass in OrgClasses)
                {
                    if (!OrgClassCarbonReports.ContainsKey(T_OrgClass.SubClassName))
                    {
                        OrgClassCarbonReports.Add(T_OrgClass.SubClassName, new List<T_OrgClassCarbonReport>());
                    }

                    T_OrgClassCarbonReport reportDetil = new T_OrgClassCarbonReport();
                    reportDetil.Id = T_OrgClass.Id;
                    reportDetil.ClassId = T_OrgClass.ClassId;
                    reportDetil.SubClassId = T_OrgClass.SubClassId;
                    reportDetil.FacilityId = T_OrgClass.FacilityId;
                    reportDetil.FactorId = T_OrgClass.FactorId;
                    reportDetil.FactorType = T_OrgClass.FactorType;
                    reportDetil.DataSource = T_OrgClass.DataSource;
                    reportDetil.EquipmentIds = T_OrgClass.EquipmentIds;
                    reportDetil.SubClassName = T_OrgClass.SubClassName;
                    reportDetil.FacilityName = T_OrgClass.FacilityName;
                    reportDetil.FactorName = T_OrgClass.FactorName;
                    reportDetil.EmissionFactor = T_OrgClass.EmissionFactor;
                    reportDetil.TypeName = T_OrgClass.TypeName;
                    reportDetil.FactorUnit = T_OrgClass.FactorUnit;
                    reportDetil.ActivityUnit = T_OrgClass.ActivityUnit;
                    double UseVale = 0;
                    double CostVale = 0;
                    double CarbonEmission = 0;
                    double ConvertCoal = 0;
                    foreach (var jtem in listEnergies)
                    {
                        string[] deepFacility = (DeppIDS(reportDetil.FacilityId, facilityList) + reportDetil.FacilityId).Split(',');
                        if (deepFacility.Contains(jtem.FacilityId))//在设施下
                        {
                            if (jtem.FactorId == reportDetil.FactorType)//同一种能源
                            {
                                if (reportDetil.EquipmentIds.Split(',').Contains(jtem.EquipmentId))//设备被选中
                                {
                                    UseVale += jtem.UseVale;
                                    CostVale += jtem.CostVale;
                                    CarbonEmission += jtem.CarbonEmission;
                                    ConvertCoal += jtem.ConvertCoal;
                                }
                            }
                        }
                    }
                    reportDetil.UseVale = Math.Round(UseVale, 2);
                    reportDetil.CostVale = Math.Round(CostVale, 2);
                    reportDetil.CarbonEmission = Math.Round(CarbonEmission, 2);
                    reportDetil.ConvertCoal = Math.Round(ConvertCoal, 2);

                    //产值
                    double OutValue = 0;
                    string[] facilityIds = (DeppIDS(reportDetil.FacilityId, facilityList) + reportDetil.FacilityId).Split(',');
                    foreach (var jtem in listProductions)
                    {
                        if (facilityIds.Contains(jtem.FacilityId))//同一设施
                        {
                            OutValue += jtem.OutValue;
                        }
                    }
                    
                    reportDetil.OutValue = Math.Round(OutValue,2);

                    OrgClassCarbonReports[T_OrgClass.SubClassName].Add(reportDetil);
                }
                carbonReport.OrgClasses = OrgClassCarbonReports;

                carbonReportType.Add(carbonReport);
            }

            return carbonReportType;
        }

        /// <summary>
        /// 按排放范围分析报表
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_CarbonReportType>> CarbonReportByRange(In_CarbonReport query)
        {
            List<Out_CarbonReportType> carbonReportType = new List<Out_CarbonReportType>();
            Out_CarbonReportType one = new Out_CarbonReportType();
            one.ClassNo = "1";
            one.ClassName = "范围一";
            one.OrgClasses = new Dictionary<string, List<T_OrgClassCarbonReport>>();
            Out_CarbonReportType two = new Out_CarbonReportType();
            two.ClassNo = "2";
            two.ClassName = "范围二";
            two.OrgClasses = new Dictionary<string, List<T_OrgClassCarbonReport>>();
            Out_CarbonReportType three = new Out_CarbonReportType();
            three.ClassNo = "3";
            three.ClassName = "范围三";
            three.OrgClasses = new Dictionary<string, List<T_OrgClassCarbonReport>>();
            carbonReportType.Add(one);
            carbonReportType.Add(two);
            carbonReportType.Add(three);

            //能耗数据
            InEnergyTree inEnergyTree = new InEnergyTree();
            inEnergyTree.OrgId = query.OrgId;
            inEnergyTree.BeginDate = query.BeginDate;
            query.EndDate = query.EndDate;
            List<Out_EnergyList> listEnergies = await _productionDAL.SelectEnergyList(inEnergyTree);

            //产值数据
            InProductionPageList inProduction = new InProductionPageList();
            inProduction.OrgId = query.OrgId;
            inProduction.BeginDate = query.BeginDate;
            inProduction.EndDate = query.EndDate;
            List<Out_Production> listProductions = (await _productionDAL.SelectProductionPage(inProduction)).List;

            //所有设施
            List<T_Com_Facility> facilityList = await _commonDAL.SelectFacilityTree((long)query.OrgId);

            //把六大类型查出来
            List<Out_Class> out_Class = await _commonDAL.SelectClassAll();
            foreach (var item in out_Class)
            {
                //所有子类型
                List<T_OrgClass> OrgClasses = await _commonDAL.SelectOrgClass(query.OrgId, item.Id);

                foreach (var T_OrgClass in OrgClasses)
                {
                    foreach (var Rtem in carbonReportType)
                    {
                        if (Rtem.ClassNo == item.RangeId)
                        {
                            if (!Rtem.OrgClasses.ContainsKey(T_OrgClass.SubClassName))
                            {
                                Rtem.OrgClasses.Add(T_OrgClass.SubClassName, new List<T_OrgClassCarbonReport>());
                            }


                            T_OrgClassCarbonReport reportDetil = new T_OrgClassCarbonReport();
                            reportDetil.Id = T_OrgClass.Id;
                            reportDetil.ClassId = T_OrgClass.ClassId;
                            reportDetil.SubClassId = T_OrgClass.SubClassId;
                            reportDetil.FacilityId = T_OrgClass.FacilityId;
                            reportDetil.FactorId = T_OrgClass.FactorId;
                            reportDetil.FactorType = T_OrgClass.FactorType;
                            reportDetil.DataSource = T_OrgClass.DataSource;
                            reportDetil.EquipmentIds = T_OrgClass.EquipmentIds;
                            reportDetil.SubClassName = T_OrgClass.SubClassName;
                            reportDetil.FacilityName = T_OrgClass.FacilityName;
                            reportDetil.FactorName = T_OrgClass.FactorName;
                            reportDetil.EmissionFactor = T_OrgClass.EmissionFactor;
                            reportDetil.TypeName = T_OrgClass.TypeName;
                            reportDetil.FactorUnit = T_OrgClass.FactorUnit;
                            reportDetil.ActivityUnit = T_OrgClass.ActivityUnit;
                            double UseVale = 0;
                            double CostVale = 0;
                            double CarbonEmission = 0;
                            double ConvertCoal = 0;
                            foreach (var jtem in listEnergies)
                            {
                                string deepFacility = DeppIDS(reportDetil.FacilityId, facilityList) + reportDetil.FacilityId;
                                if (deepFacility.Split(',').Contains(jtem.FacilityId))//在设施下
                                {
                                    if (jtem.FactorId == reportDetil.FactorType)//同一种能源
                                    {
                                        if (reportDetil.EquipmentIds.Split(',').Contains(jtem.EquipmentId))//设备被选中
                                        {
                                            UseVale += jtem.UseVale;
                                            CostVale += jtem.CostVale;
                                            CarbonEmission += jtem.CarbonEmission;
                                            ConvertCoal += jtem.ConvertCoal;
                                        }
                                    }
                                }
                            }
                            reportDetil.UseVale = Math.Round(UseVale, 2);
                            reportDetil.CostVale = Math.Round(CostVale, 2);
                            reportDetil.CarbonEmission = Math.Round(CarbonEmission, 2);
                            reportDetil.ConvertCoal = Math.Round(ConvertCoal, 2);

                            //产值
                            double OutValue = 0;
                            string[] facilityIds = (DeppIDS(reportDetil.FacilityId, facilityList) + reportDetil.FacilityId).Split(',');
                            foreach (var jtem in listProductions)
                            {
                                if (facilityIds.Contains(jtem.FacilityId))//同一设施
                                {
                                    OutValue += jtem.OutValue;
                                }
                            }

                            reportDetil.OutValue = Math.Round(OutValue,2);

                            Rtem.OrgClasses[T_OrgClass.SubClassName].Add(reportDetil);
                        }
                    }
                }
            }

            return carbonReportType;
        }

        #endregion


        #region 首页设备分布
        /// <summary>
        /// 设备统计
        /// </summary>
        /// <param name="inCalEnergy"></param>
        /// <returns></returns>
        public async Task<Out_EquipmentStatistics> StatisticsInfo(InCalEnergy inCalEnergy)
        {
            Out_EquipmentStatistics res = new Out_EquipmentStatistics();
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);

            //if (orgId > 0)
            //{
            //    var staticInfo = await _deviceDAL.SelectStatusByOrgId(user, orgId);
            //    res.TotalCount = staticInfo.TotalCount;
            //    res.OnlineCount = staticInfo.OnlineCount;
            //    res.OfflineCount = staticInfo.OfflineCount;
            //    res.UnknowCount = staticInfo.UnknowCount;
            //    res.EventCount = await _warningDAL.SelectWaitCountByOrgId(user, orgId);
            //}
            //else
            //{
            //    List<string> keys = new List<string>();
            //    keys.Add(user.OrgId.ToString());
            //    res.OnlineCount = await _deviceDAL.Count(x => (x.OrgId == user.OrgId || SonSqlFun.FullSearch("OwnerOrgPath", keys) || x.UseOrgId == user.OrgId || x.UseUserId == user.UserId) && x.Online == 1);
            //    res.OfflineCount = await _deviceDAL.Count(x => (x.OrgId == user.OrgId || SonSqlFun.FullSearch("OwnerOrgPath", keys) || x.UseOrgId == user.OrgId || x.UseUserId == user.UserId) && x.Online == 0);
            //    res.UnknowCount = await _deviceDAL.Count(x => (x.OrgId == user.OrgId || SonSqlFun.FullSearch("OwnerOrgPath", keys) || x.UseOrgId == user.OrgId || x.UseUserId == user.UserId) && x.Online == 2);
            //    res.TotalCount = res.OnlineCount + res.OfflineCount + res.UnknowCount;
            //    res.EventCount = await _warningDAL.SelectWaitCount(user);
            //}

            res.EventCount = await _warningDAL.SelectWaitCountByOrgId(user, inCalEnergy.orgId);
            //设备总数
            List<T_Com_Equipment> equipments = await _commonDAL.SelectEquipmentList(inCalEnergy.orgId);
            List<MZ_IotDevice> iotDevices = await _commonDAL.SelectIotDeviceList();
            res.EquipemntCount = equipments.Count;
            res.DataCount = 0;
            res.TotalCount = 0;
            res.OnlineCount = 0;
            res.OfflineCount = 0;
            res.UnknowCount = 0;
            foreach (var item in equipments)
            {
                if (item.DataState == "1")
                {
                    res.DataCount++;
                }
                if (item.ThirdId != "-")
                {
                    res.TotalCount++;
                    bool flg = true;
                    foreach (var jtem in iotDevices)
                    {
                        if (jtem.DeviceNumber == item.ThirdId)
                        {
                            flg = false;
                            if (jtem.Online == 1)
                            {
                                res.OnlineCount++;
                            }
                            else if(jtem.Online == 0)
                            {
                                res.OfflineCount++;
                            }
                            else
                            {
                                res.UnknowCount++;
                            }
                            break;
                        }
                    }
                    if (flg)
                    {
                        res.UnknowCount++;
                    }
                }
            }
            res.EquipemntTypes = await _commonDAL.SelectEquipmentTypeList(inCalEnergy.orgId);

            //能耗数据
            InEnergyTree inEnergyTree = new InEnergyTree();
            inEnergyTree.OrgId = inCalEnergy.orgId;
            inEnergyTree.BeginDate = inCalEnergy.beginDate;
            inEnergyTree.EndDate = inCalEnergy.endDate;
            List<Out_EnergyList> listEnergies = await _productionDAL.SelectEnergyList(inEnergyTree);
            res.CostVale = 0;
            res.CarbonEmission = 0;
            res.ConvertCoal = 0;
            foreach (var item in listEnergies)
            {
                res.CostVale += item.CostVale;
                res.CarbonEmission += item.CarbonEmission;
                res.ConvertCoal += item.ConvertCoal;
                foreach (var jtem in res.EquipemntTypes)
                {
                    if (jtem.EnergyType == item.FactorId)
                    {
                        jtem.CarbonEmission = Math.Round(jtem.CarbonEmission + item.CarbonEmission, 2);
                        jtem.ConvertCoal = Math.Round(jtem.ConvertCoal + item.ConvertCoal, 2);
                        jtem.UseVale = Math.Round(jtem.UseVale + item.UseVale, 2);
                        jtem.Unit = item.Unit;
                        jtem.LageUnit = item.LageUnit;
                    }
                }
            }
            res.CostVale = Math.Round(res.CostVale,2);
            res.CarbonEmission = Math.Round(res.CarbonEmission,2);
            res.ConvertCoal = Math.Round(res.ConvertCoal,2);

            res.OutValue = 0;
            //产值数据
            InProductionPageList inProduction = new InProductionPageList();
            inProduction.OrgId = inCalEnergy.orgId;
            inProduction.BeginDate = inCalEnergy.beginDate;
            inProduction.EndDate = inCalEnergy.endDate;
            List<Out_Production> listProductions = (await _productionDAL.SelectProductionPage(inProduction)).List;
            foreach (var item in listProductions)
            {
                res.OutValue += item.OutValue;
            }
            res.OutValue = Math.Round(res.OutValue, 2);

            return res;
        }

        /// <summary>
        /// 告警信息类别
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_IotWarning>> WarningListPage(In_WarningListPage query, IUserInfo user)
        {
            List<T_Com_Equipment> equipments = await _commonDAL.SelectEnergyEquipmentList();
            List<MZ_IotDevice> iotDevices = await _commonDAL.SelectIotDeviceList();

            var tlist = await _warningDAL.SelectWithPage(query, user);

            var clearUsers = await _userDAL.NavigateDict(tlist.List, x => x.ClearId != null && x.ClearId > 0, x => x.ClearId.Value);
            foreach (var warn in tlist.List)
            {
                foreach (var jtem in iotDevices)//匹配物联设备信息
                {
                    if (jtem.Id == warn.DeviceId)
                    {
                        foreach (var ltem in equipments)//匹配设备信息
                        {
                            if (jtem.DeviceNumber == ltem.ThirdId)
                            {
                                warn.DeviceName = ltem.EquipmentName;//替换设备名称
                            }
                        }
                    }
                }
                if (warn.ClearId != null && warn.ClearId > 0)
                {
                    MZ_AdminInfo clearUser;
                    if (clearUsers.TryGetValue(warn.ClearId.Value, out clearUser))
                    {
                        warn.ClearUser = clearUser;
                    }
                }
            }
            return tlist;
        }
        #endregion

        #region 供应商
        /// <summary>
        /// 新增供应商
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddProvider(In_Prod_Provider in_Provider)
        {
            var user = _provider.GetUser();

            T_Prod_Provider t_provider = new T_Prod_Provider();
            t_provider.Id = _snowflake.NextId().ToString();

            t_provider.OrgId = in_Provider.OrgId;
            t_provider.ProviderCode = in_Provider.ProviderCode;
            t_provider.ProviderName = in_Provider.ProviderName;
            t_provider.ProviderAddress = in_Provider.ProviderAddress;
            t_provider.Contact = in_Provider.Contact;
            t_provider.Area = in_Provider.Area;
            t_provider.Manager = in_Provider.Manager;

            t_provider.del_flag = "0";
            t_provider.createId = user.UserId;
            t_provider.create_time = DateTime.Now;
            t_provider.updateId = user.UserId;
            t_provider.update_time = DateTime.Now;
            await _productionDAL.AddProvider(t_provider);
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 导入供应商
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ImportProvider(List<In_Prod_Provider> list)
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            var user = _provider.GetUser();
            List<MZ_Area> mZ_Areas = await _codeDAL.SelectCodeList();
           

            foreach (var in_Provider in list)
            {
                if (!string.IsNullOrEmpty(in_Provider.ProviderName))
                {
                    T_Prod_Provider t_provider = await _productionDAL.SelectProviderByProviderName(in_Provider.ProviderCode);

                    if (t_provider == null)
                    {
                        t_provider = new T_Prod_Provider();
                        t_provider.Id = _snowflake.NextId().ToString();

                        t_provider.OrgId = user.OrgId;
                        if (string.IsNullOrEmpty(in_Provider.ProviderCode))
                        {
                            t_provider.ProviderCode = await tmpredis.GenerateNumber("PV");
                        }
                        else
                        {
                            t_provider.ProviderCode = in_Provider.ProviderCode;
                        }
                        t_provider.ProviderName = in_Provider.ProviderName;
                        t_provider.ProviderAddress = in_Provider.ProviderAddress;
                        t_provider.Contact = in_Provider.Contact;
                        string[] arr = in_Provider.Area.Split(',');
                        List<string> arrs = new List<string>();
                        foreach (var item in arr)
                        {
                            foreach (var jtem in mZ_Areas)
                            {
                                if (jtem.Name == item)
                                {
                                    arrs.Add(jtem.Id);
                                    break;
                                }
                            }
                        }
                        t_provider.Area = string.Join(",", arrs);
                        t_provider.Manager = in_Provider.Manager;

                        t_provider.del_flag = "0";
                        t_provider.createId = user.UserId;
                        t_provider.create_time = DateTime.Now;
                        t_provider.updateId = user.UserId;
                        t_provider.update_time = DateTime.Now;
                        await _productionDAL.AddProvider(t_provider);
                    }
                    else
                    {
                        t_provider.ProviderName = in_Provider.ProviderName;
                        t_provider.ProviderAddress = in_Provider.ProviderAddress;
                        t_provider.Contact = in_Provider.Contact;
                        string[] arr = in_Provider.Area.Split(',');
                        List<string> arrs = new List<string>();
                        foreach (var item in arr)
                        {
                            foreach (var jtem in mZ_Areas)
                            {
                                if (jtem.Name == item)
                                {
                                    arrs.Add(jtem.Id);
                                    break;
                                }
                            }
                        }
                        t_provider.Area = string.Join(",", arrs);
                        t_provider.Manager = in_Provider.Manager;

                        t_provider.updateId = user.UserId;
                        t_provider.update_time = DateTime.Now;
                        await _productionDAL.UpdateProvider(t_provider);
                    }
                }
            }
            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteProvider(string Id)
        {
            await _productionDAL.DeleteProvider(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改供应商
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateProvider(In_Prod_Provider in_Provider)
        {
            T_Prod_Provider t_provider = await _productionDAL.SelectProvider(in_Provider.Id);
            if (t_provider == null)
            {
                return BusResponse<int>.Error(500, "找不到记录");
            }

            var user = _provider.GetUser();

            t_provider.ProviderCode = in_Provider.ProviderCode;
            t_provider.ProviderName = in_Provider.ProviderName;
            t_provider.ProviderAddress = in_Provider.ProviderAddress;
            t_provider.Contact = in_Provider.Contact;
            t_provider.Area = in_Provider.Area;
            t_provider.Manager = in_Provider.Manager;

            t_provider.updateId = user.UserId;
            t_provider.update_time = DateTime.Now;
            await _productionDAL.UpdateProvider(t_provider);
            return BusResponse<int>.Success();
        }
        /// <summary>
        /// 查询供应商
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_Provider> SelectProvider(string Id)
        {
            return await _productionDAL.SelectProvider(Id);
        }

        /// <summary>
        /// 分页查询供应商列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<T_Prod_Provider2>> SelectProviderList(In_Provider query)
        {
            return await _productionDAL.SelectProviderPage(query);
        }

        public virtual async Task<BusResponse<string>> GenerateNumber(string Id)
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return BusResponse<string>.Success(await tmpredis.GenerateNumber(Id));
        }
        #endregion

        #region 物料

        /// <summary>
        /// 查询供应商
        /// </summary>
        /// <returns></returns>
        public async Task<List<Out_Prod_Provider>> SelectMaterialProvider(string MaterialId)
        {
            return await _productionDAL.SelectMaterialProvider(MaterialId);
        }
        /// <summary>
        /// 新增物料
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddMaterial(In_Prod_Material In_Material)
        {
            var user = _provider.GetUser();

            T_Prod_Material t_Material = new T_Prod_Material();
            t_Material.Id = _snowflake.NextId().ToString();

            t_Material.OrgId = In_Material.OrgId;
            t_Material.MaterialCode = In_Material.MaterialCode;
            t_Material.MaterialName = In_Material.MaterialName;
            t_Material.MaterialType = In_Material.MaterialType;
            t_Material.MaterialUnit = In_Material.MaterialUnit;

            t_Material.del_flag = "0";
            t_Material.createId = user.UserId;
            t_Material.create_time = DateTime.Now;
            t_Material.updateId = user.UserId;
            t_Material.update_time = DateTime.Now;
            await _productionDAL.AddMaterial(t_Material);

            foreach (var item in In_Material.ProviderMaterials)
            {
                T_Prod_ProviderMaterial t_Prod_ProviderMaterial = new T_Prod_ProviderMaterial();
                t_Prod_ProviderMaterial.MaterialId = t_Material.Id;
                t_Prod_ProviderMaterial.ProviderId = item.ProviderId;
                t_Prod_ProviderMaterial.ProductModel = item.ProductModel;

                await _productionDAL.AddProviderMaterial(t_Prod_ProviderMaterial);
            }

            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除物料
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteMaterial(string Id)
        {
            await _productionDAL.DeleteMaterial(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改物料
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateMaterial(In_Prod_Material In_Material)
        {
            T_Prod_Material t_Material = await _productionDAL.SelectMaterial(In_Material.Id);
            if (t_Material == null)
            {
                return BusResponse<int>.Error(500, "找不到记录");
            }

            var user = _provider.GetUser();

            t_Material.MaterialCode = In_Material.MaterialCode;
            t_Material.MaterialName = In_Material.MaterialName;
            t_Material.MaterialType = In_Material.MaterialType;
            t_Material.MaterialUnit = In_Material.MaterialUnit;

            t_Material.updateId = user.UserId;
            t_Material.update_time = DateTime.Now;
            await _productionDAL.UpdateMaterial(t_Material);

            await _productionDAL.DeleteProviderMaterial(t_Material.Id);

            foreach (var item in In_Material.ProviderMaterials)
            {
                T_Prod_ProviderMaterial t_Prod_ProviderMaterial = new T_Prod_ProviderMaterial();
                t_Prod_ProviderMaterial.MaterialId = t_Material.Id;
                t_Prod_ProviderMaterial.ProviderId = item.ProviderId;
                t_Prod_ProviderMaterial.ProductModel = item.ProductModel;

                await _productionDAL.AddProviderMaterial(t_Prod_ProviderMaterial);
            }
            return BusResponse<int>.Success();
        }

        /// <summary>
        /// 查询物料
        /// </summary>
        /// <returns></returns>
        public async Task<Out_Material> SelectMaterial(string Id)
        {
            Out_Material material = await _productionDAL.SelectMaterial(Id);
            material.ProviderMaterials = await _productionDAL.SelectMaterialProvider(material.Id);

            return material;
        }

        /// <summary>
        /// 分页查询物料列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_Material>> SelectMaterialList(In_Material query)
        {
            return await _productionDAL.SelectMaterialPage(query);
        }

        /// <summary>
        /// 导入物料
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ImportMaterial(List<Inpotr_Material> list)
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            var user = _provider.GetUser();

            Dictionary<string, In_Prod_Material> In_Material = new Dictionary<string, In_Prod_Material>();
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.MaterialCode))
                {
                    item.MaterialCode = await tmpredis.GenerateNumber("MT");
                }
                if (In_Material.ContainsKey(item.MaterialCode))
                {
                    T_Prod_Provider provider = await _productionDAL.SelectProviderByProviderName(item.ProviderCode);
                    if (provider != null)
                    {
                        T_Prod_ProviderMaterial ProviderMaterial = new T_Prod_ProviderMaterial();
                        ProviderMaterial.MaterialId = In_Material[item.MaterialCode].Id;
                        ProviderMaterial.ProviderId = provider.Id;
                        ProviderMaterial.ProductModel = item.ProductModel;
                        In_Material[item.MaterialCode].ProviderMaterials.Add(ProviderMaterial);
                    }
                }
                else
                {
                    Out_Material material = await _productionDAL.SelectMaterialByMaterialCode(item.MaterialCode);

                    In_Prod_Material in_Prod = new In_Prod_Material();
                    in_Prod.MaterialCode = item.MaterialCode;
                    in_Prod.MaterialName = item.MaterialName;
                    in_Prod.MaterialType = item.MaterialType;
                    in_Prod.MaterialUnit = item.MaterialUnit;
                    if (material == null)
                    {
                        in_Prod.Id = _snowflake.NextId().ToString();
                        in_Prod.OrgId = user.OrgId;
                        in_Prod.MaterialCode = item.MaterialCode;
                    }
                    else
                    {
                        in_Prod.Id = material.Id;
                        in_Prod.OrgId = material.OrgId;
                    }
                    in_Prod.ProviderMaterials = new List<T_Prod_ProviderMaterial>();
                    T_Prod_Provider provider = await _productionDAL.SelectProviderByProviderName(item.ProviderCode);

                    if (provider != null)
                    {
                        T_Prod_ProviderMaterial ProviderMaterial = new T_Prod_ProviderMaterial();
                        ProviderMaterial.MaterialId = in_Prod.Id;
                        ProviderMaterial.ProviderId = provider.Id;
                        ProviderMaterial.ProductModel = item.ProductModel;
                        in_Prod.ProviderMaterials.Add(ProviderMaterial);
                    }
                    In_Material.Add(in_Prod.MaterialCode, in_Prod);
                }
            }

            foreach (var item in In_Material.Values)
            {
                T_Prod_Material material = await _productionDAL.SelectMaterial(item.Id);
                if (material == null)
                {
                    T_Prod_Material t_Material = new T_Prod_Material();
                    t_Material.Id = item.Id;

                    t_Material.OrgId = item.OrgId;
                    t_Material.MaterialCode = item.MaterialCode;
                    t_Material.MaterialName = item.MaterialName;
                    t_Material.MaterialType = item.MaterialType;
                    t_Material.MaterialUnit = item.MaterialUnit;

                    t_Material.del_flag = "0";
                    t_Material.createId = user.UserId;
                    t_Material.create_time = DateTime.Now;
                    t_Material.updateId = user.UserId;
                    t_Material.update_time = DateTime.Now;
                    await _productionDAL.AddMaterial(t_Material);

                    foreach (var jtem in item.ProviderMaterials)
                    {
                        T_Prod_ProviderMaterial t_Prod_ProviderMaterial = new T_Prod_ProviderMaterial();
                        t_Prod_ProviderMaterial.MaterialId = item.Id;
                        t_Prod_ProviderMaterial.ProviderId = jtem.ProviderId;
                        t_Prod_ProviderMaterial.ProductModel = jtem.ProductModel;

                        await _productionDAL.AddProviderMaterial(t_Prod_ProviderMaterial);
                    }
                }
                else
                {
                    material.MaterialCode = item.MaterialCode;
                    material.MaterialName = item.MaterialName;
                    material.MaterialType = item.MaterialType;
                    material.MaterialUnit = item.MaterialUnit;

                    material.updateId = user.UserId;
                    material.update_time = DateTime.Now;
                    await _productionDAL.UpdateMaterial(material);

                    await _productionDAL.DeleteProviderMaterial(item.Id);

                    foreach (var jtem in item.ProviderMaterials)
                    {
                        T_Prod_ProviderMaterial t_Prod_ProviderMaterial = new T_Prod_ProviderMaterial();
                        t_Prod_ProviderMaterial.MaterialId = item.Id;
                        t_Prod_ProviderMaterial.ProviderId = jtem.ProviderId;
                        t_Prod_ProviderMaterial.ProductModel = jtem.ProductModel;

                        await _productionDAL.AddProviderMaterial(t_Prod_ProviderMaterial);
                    }
                }
            }
            return BusResponse<string>.Success("新增成功");
        }
        #endregion

        #region 物料碳足迹
        /// <summary>
        /// 新增物料碳足迹
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> AddMaterialCarbon(In_Prod_MaterialCarbon In_Material)
        {
            Out_Material material = await _productionDAL.SelectMaterial(In_Material.MaterialId);
            if (material == null)
            {
                return BusResponse<string>.Error(500, "找不到物料编码");
            }
            material.ProviderMaterials = await _productionDAL.SelectMaterialProvider(material.Id);
            T_Prod_Provider provider = await _productionDAL.SelectProvider(In_Material.ProviderId);
            if (provider == null)
            {
                return BusResponse<string>.Error(500, "找不到供应商编码");
            }
            bool flg = true;
            foreach (var item in material.ProviderMaterials)
            {
                if (item.ProviderId == In_Material.ProviderId)
                {
                    flg = false;
                }
            }
            if (flg)
            {
                return BusResponse<string>.Error(500, "供应商不在物料的供应商列表");
            }
            if (!(In_Material.ProductBorder == "1" || In_Material.ProductBorder == "2" || In_Material.ProductBorder == "3"))
            {
                return BusResponse<string>.Error(500, "产品生命周期边界错误:1,2,3");
            }

            var user = _provider.GetUser();

            T_Prod_MaterialCarbon t_Material = new T_Prod_MaterialCarbon();
            t_Material.Id = _snowflake.NextId().ToString();
            
            t_Material.OrgId = (long)In_Material.OrgId;
            t_Material.MaterialId = In_Material.MaterialId;
            t_Material.ProviderId = In_Material.ProviderId;
            t_Material.ProductBorder = In_Material.ProductBorder;
            t_Material.CarbonEmission = In_Material.CarbonEmission;
            t_Material.CarbonUnit = In_Material.CarbonUnit;


            t_Material.del_flag = "0";
            t_Material.createId = user.UserId;
            t_Material.create_time = DateTime.Now;
            t_Material.updateId = user.UserId;
            t_Material.update_time = DateTime.Now;
            await _productionDAL.AddMaterialCarbon(t_Material);

            return BusResponse<string>.Success("新增成功");
        }

        /// <summary>
        /// 删除物料碳足迹
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> DeleteMaterialCarbon(string Id)
        {
            await _productionDAL.DeleteMaterialCarbon(Id);
            return BusResponse<int>.Success(0, "删除成功");
        }

        /// <summary>
        /// 修改物料碳足迹
        /// </summary>
        /// <returns></returns>
        public virtual async Task<BusResponse<int>> UpdateMaterialCarbon(In_Prod_MaterialCarbon In_Material)
        {
            Out_Material material = await _productionDAL.SelectMaterial(In_Material.MaterialId);
            if (material == null)
            {
                return BusResponse<int>.Error(500, "找不到物料编码");
            }
            material.ProviderMaterials = await _productionDAL.SelectMaterialProvider(material.Id);
            T_Prod_Provider provider = await _productionDAL.SelectProvider(In_Material.ProviderId);
            if (provider == null)
            {
                return BusResponse<int>.Error(500, "找不到供应商编码");
            }
            if (!(In_Material.ProductBorder == "1" || In_Material.ProductBorder == "2" || In_Material.ProductBorder == "3"))
            {
                return BusResponse<int>.Error(500, "产品生命周期边界错误:1,2,3");
            }
            bool flg = true;
            foreach (var item in material.ProviderMaterials)
            {
                if (item.ProviderId == In_Material.ProviderId)
                {
                    flg = false;
                }
            }
            if (flg)
            {
                return BusResponse<int>.Error(500, "供应商不在物料的供应商列表");
            }

            T_Prod_MaterialCarbon t_Material = await _productionDAL.SelectMaterialCarbon(In_Material.Id);
            if (t_Material == null)
            {
                return BusResponse<int>.Error(500, "找不到记录");
            }
            var user = _provider.GetUser();

            t_Material.OrgId = (long)In_Material.OrgId;
            t_Material.MaterialId = In_Material.MaterialId;
            t_Material.ProviderId = In_Material.ProviderId;
            t_Material.ProductBorder = In_Material.ProductBorder;
            t_Material.CarbonEmission = In_Material.CarbonEmission;
            t_Material.CarbonUnit = In_Material.CarbonUnit;

            t_Material.updateId = user.UserId;
            t_Material.update_time = DateTime.Now;
            await _productionDAL.UpdateMaterialCarbon(t_Material);

            return BusResponse<int>.Success();
        }

        /// <summary>
        /// 查询物料碳足迹信息
        /// </summary>
        /// <returns></returns>
        public async Task<T_Prod_MaterialCarbon> SelectMaterialCarbon(string Id)
        {
            T_Prod_MaterialCarbon material = await _productionDAL.SelectMaterialCarbon(Id);

            return material;
        }

        /// <summary>
        /// 分页查询物料碳足迹列表
        /// </summary>
        /// <returns></returns>
        public async Task<PageObject<Out_MaterialCarbon>> SelectMaterialCarbonList(In_MaterialCarbon query)
        {
            return await _productionDAL.SelectMaterialCarbonPage(query);
        }

        /// <summary>
        /// 导入物料
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<string>> ImportMaterialCarbon(List<Inport_MaterialCarbon> list)
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            var user = _provider.GetUser();

            if (list != null)
            {
                foreach (var item in list)
                {
                    T_Prod_Provider provider = await _productionDAL.SelectProviderByProviderName(item.ProviderCode);
                    if (provider == null)
                    {
                        return BusResponse<string>.Error(500, "找不到供应商编码：" + item.ProviderCode);
                    }

                    Out_Material material = await _productionDAL.SelectMaterialByMaterialCode(item.MaterialCode);
                    if (material == null)
                    {
                        return BusResponse<string>.Error(500, "找不到物料编码：" + item.MaterialCode);
                    }

                    if (!(item.ProductBorder == "1" || item.ProductBorder == "2" || item.ProductBorder == "3"))
                    {
                        return BusResponse<string>.Error(500, "产品生命周期边界错误:1,2,3");
                    }
                }
            }

            if (list != null)
            {
                foreach (var item in list)
                {
                    T_Prod_Provider provider = await _productionDAL.SelectProviderByProviderName(item.ProviderCode);
                    if (provider == null)
                    {
                        return BusResponse<string>.Error(500, "找不到供应商编码：" + item.ProviderCode);
                    }

                    Out_Material material = await _productionDAL.SelectMaterialByMaterialCode(item.MaterialCode);
                    if (material == null)
                    {
                        return BusResponse<string>.Error(500, "找不到物料编码：" + item.MaterialCode);
                    }

                    T_Prod_MaterialCarbon t_Material = await _productionDAL.SelectMaterialCarbon(item.Id);
                    if (t_Material == null)
                    {
                        t_Material = new T_Prod_MaterialCarbon();
                        t_Material.Id = _snowflake.NextId().ToString();

                        t_Material.OrgId = user.OrgId;
                        t_Material.MaterialId = material.Id;
                        t_Material.ProviderId = provider.Id;
                        t_Material.ProductBorder = item.ProductBorder;
                        t_Material.CarbonEmission = double.Parse(item.CarbonEmission);
                        t_Material.CarbonUnit = item.CarbonUnit;


                        t_Material.del_flag = "0";
                        t_Material.createId = user.UserId;
                        t_Material.create_time = DateTime.Now;
                        t_Material.updateId = user.UserId;
                        t_Material.update_time = DateTime.Now;
                        await _productionDAL.AddMaterialCarbon(t_Material);
                    }
                    else
                    {
                        t_Material.MaterialId = material.Id;
                        t_Material.ProviderId = provider.Id;
                        t_Material.ProductBorder = item.ProductBorder;
                        t_Material.CarbonEmission = double.Parse(item.CarbonEmission);
                        t_Material.CarbonUnit = item.CarbonUnit;

                        t_Material.updateId = user.UserId;
                        t_Material.update_time = DateTime.Now;
                        await _productionDAL.UpdateMaterialCarbon(t_Material);
                    }
                }
            }
            return BusResponse<string>.Success("新增成功");
        }
        #endregion

        #region 碳排计划表
        /// <summary>
        /// 查询碳排计划表
        /// </summary>
        /// <returns></returns>
        public async Task<In_CarbonPlan> SelectCarbonPlan(string Year, long OrgId)
        {
            In_CarbonPlan carbonPlan = new In_CarbonPlan();
            if (string.IsNullOrEmpty(Year))
            {
                return carbonPlan;
            }

            carbonPlan.Year = Year;
            carbonPlan.OrgId = OrgId;
            carbonPlan.Details = new List<In_CarbonPlanDetail>();

            In_CarbonPlanDetail CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "碳排量");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "碳排量";
                t_Prod_Carbon.Unit = "tCO₂e";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "碳排量");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);


            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "配额总量");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "配额总量";
                t_Prod_Carbon.Unit = "tCO₂e";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "配额总量");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "CCER总量");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "CCER总量";
                t_Prod_Carbon.Unit = "tCO₂e";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "CCER总量");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "能源需求");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "能源需求";
                t_Prod_Carbon.Unit = "标准煤";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "能源需求");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "总产值");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "总产值";
                t_Prod_Carbon.Unit = "万元";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "总产值");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            return carbonPlan;
        }
        /// <summary>
        /// 更新碳排计划表
        /// </summary>
        /// <returns></returns>
        public async Task<BusResponse<int>> UpdateCarbonPlan(In_CarbonPlan carbonPlan)
        {
            foreach (var item in carbonPlan.Details)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = carbonPlan.Year;
                t_Prod_Carbon.OrgId = carbonPlan.OrgId;
                t_Prod_Carbon.CarbonType = item.CarbonType;
                t_Prod_Carbon.Unit = item.Unit;
                t_Prod_Carbon.Granularity = item.Granularity;
                t_Prod_Carbon.Month1 = item.Month1;
                t_Prod_Carbon.Month2 = item.Month2;
                t_Prod_Carbon.Month3 = item.Month3;
                t_Prod_Carbon.Month4 = item.Month4;
                t_Prod_Carbon.Month5 = item.Month5;
                t_Prod_Carbon.Month6 = item.Month6;
                t_Prod_Carbon.Month7 = item.Month7;
                t_Prod_Carbon.Month8 = item.Month8;
                t_Prod_Carbon.Month9 = item.Month9;
                t_Prod_Carbon.Month10 = item.Month10;
                t_Prod_Carbon.Month11 = item.Month11;
                t_Prod_Carbon.Month12 = item.Month12;
                t_Prod_Carbon.YearTotal = item.YearTotal;
                await _productionDAL.UpdateCarbonPlan(t_Prod_Carbon);
            }
            return BusResponse<int>.Success();
        }
        /// <summary>
        /// 查询碳排计划表
        /// </summary>
        /// <returns></returns>
        public async Task<In_CarbonAsset> SelectCarbonAsset(string Year, long OrgId)
        {
            In_CarbonAsset carbonPlan = new In_CarbonAsset();
            if (string.IsNullOrEmpty(Year))
            {
                return carbonPlan;
            }

            carbonPlan.Year = Year;
            carbonPlan.OrgId = OrgId;
            carbonPlan.Details = new List<In_CarbonPlanDetail>();

            In_CarbonPlanDetail CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "碳排量");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "碳排量";
                t_Prod_Carbon.Unit = "tCO₂e";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "碳排量");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);


            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "配额总量");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "配额总量";
                t_Prod_Carbon.Unit = "tCO₂e";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "配额总量");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "CCER总量");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "CCER总量";
                t_Prod_Carbon.Unit = "tCO₂e";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "CCER总量");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "能源需求");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "能源需求";
                t_Prod_Carbon.Unit = "标准煤";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "能源需求");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "总产值");
            if (CarbonPlanDetail == null)
            {
                T_Prod_CarbonPlan t_Prod_Carbon = new T_Prod_CarbonPlan();
                t_Prod_Carbon.Year = Year;
                t_Prod_Carbon.OrgId = OrgId;
                t_Prod_Carbon.CarbonType = "总产值";
                t_Prod_Carbon.Unit = "万元";
                t_Prod_Carbon.Granularity = "按月";
                t_Prod_Carbon.Month1 = 0;
                t_Prod_Carbon.Month2 = 0;
                t_Prod_Carbon.Month3 = 0;
                t_Prod_Carbon.Month4 = 0;
                t_Prod_Carbon.Month5 = 0;
                t_Prod_Carbon.Month6 = 0;
                t_Prod_Carbon.Month7 = 0;
                t_Prod_Carbon.Month8 = 0;
                t_Prod_Carbon.Month9 = 0;
                t_Prod_Carbon.Month10 = 0;
                t_Prod_Carbon.Month11 = 0;
                t_Prod_Carbon.Month12 = 0;
                t_Prod_Carbon.YearTotal = 0;
                await _productionDAL.AddCarbonPlan(t_Prod_Carbon);
                CarbonPlanDetail = await _productionDAL.SelectCarbonPlan(Year, OrgId, "总产值");
            }
            carbonPlan.Details.Add(CarbonPlanDetail);

            InEnergyPageList query = new InEnergyPageList();
            query.BeginDate = Year + "-01-01";
            query.EndDate = Year + "-12-31";
            query.OrgId = OrgId;
            List<Out_Energy> Energys = (await _productionDAL.SelectEnergyPage(query)).List;
            foreach (var item in Energys)
            {
                if (item.DDate.Month == 1)
                {
                    carbonPlan.CarbonEmission1 = Math.Round(carbonPlan.CarbonEmission1 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 2)
                {
                    carbonPlan.CarbonEmission2 = Math.Round(carbonPlan.CarbonEmission2 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 3)
                {
                    carbonPlan.CarbonEmission3 = Math.Round(carbonPlan.CarbonEmission3 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 4)
                {
                    carbonPlan.CarbonEmission4 = Math.Round(carbonPlan.CarbonEmission4 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 5)
                {
                    carbonPlan.CarbonEmission5 = Math.Round(carbonPlan.CarbonEmission5 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 6)
                {
                    carbonPlan.CarbonEmission6 = Math.Round(carbonPlan.CarbonEmission6 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 7)
                {
                    carbonPlan.CarbonEmission7 = Math.Round(carbonPlan.CarbonEmission7 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 8)
                {
                    carbonPlan.CarbonEmission8 = Math.Round(carbonPlan.CarbonEmission8 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 9)
                {
                    carbonPlan.CarbonEmission9 = Math.Round(carbonPlan.CarbonEmission9 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 10)
                {
                    carbonPlan.CarbonEmission10 = Math.Round(carbonPlan.CarbonEmission10 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 11)
                {
                    carbonPlan.CarbonEmission11 = Math.Round(carbonPlan.CarbonEmission11 + item.CarbonEmission, 2);
                }
                if (item.DDate.Month == 12)
                {
                    carbonPlan.CarbonEmission12 = Math.Round(carbonPlan.CarbonEmission12 + item.CarbonEmission, 2);
                }
                carbonPlan.CarbonEmissionTotal = Math.Round(carbonPlan.CarbonEmissionTotal + item.CarbonEmission, 2);
            }

            return carbonPlan;
        }
        #endregion
    }
}
