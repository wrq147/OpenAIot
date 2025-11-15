using AuthService.Controller;
using Common;
using Common.Share;
using EfficiencyService.Business;
using EfficiencyService.DAL;
using EfficiencyService.Model;
using EfficiencyService.Model.Common;
using EfficiencyService.Model.Production;
using InfluxDB.Client.Api.Domain;
using Minio.DataModel;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace EfficiencyService.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class Common: AbstractLoginedController
    {
        private CommonBLL _commonBLL;
        private PolicyBLL _policyBLL;
        private FactorBLL _factorBLL;
        private ProductionBLL _productionBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commonBLL"></param>
        public Common(CommonBLL commonBLL, PolicyBLL policyBLL, FactorBLL factorBLL, ProductionBLL productionBLL)
        {
            _commonBLL = commonBLL;
            _policyBLL = policyBLL;
            _factorBLL = factorBLL;
            _productionBLL = productionBLL;
        }

        #region 产品增删改查
        /// <summary>
        /// 新增产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddProduct(In_Product data)
        {

            return (await _commonBLL.AddProduct(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveProduct(string Id)
        {

            return (await _commonBLL.DeleteProduct(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新产品
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateProduct(In_Product data)
        {

            return (await _commonBLL.UpdateProduct(data)).ToAjaxResult();
        }

        /// <summary>
        /// 产品列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<T_Com_Product>>> ProductPageList(In_ProductPageList query)
        {
            return this.Success(await _commonBLL.SelectProductList(query));
        }

        /// <summary>
        /// 分页查询产品能效指标
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_ProductEnergy>>> SelectProductEnergyPage(In_ProductEnergyPageList query)
        {
            return this.Success(await _commonBLL.SelectProductEnergyPage(query));
        }

        /// <summary>
        /// 查询施设一段时间内的产品列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<T_Com_Product>>> FacilityProductList(InFacilityProductList query)
        {
            return this.Success(await _commonBLL.SelectFacilityProduct(query));
        }

        /// <summary>
        /// 产品信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<T_Com_Product>> ProductInfo(string Id)
        {
            return this.Success(await _commonBLL.SelectProduct(Id));
        }

        /// <summary>
        /// 导出产品信息表模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResult ExportProduct()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<T_Com_Product>> FiedNames = new Dictionary<string, ParamRenderToExcel<T_Com_Product>>();
                FiedNames.Add("Id", new ParamRenderToExcel<T_Com_Product>("编码"));
                FiedNames.Add("ProductModel", new ParamRenderToExcel<T_Com_Product>("产品型号"));
                FiedNames.Add("ProductName", new ParamRenderToExcel<T_Com_Product>("产品名称"));
                FiedNames.Add("ShortName", new ParamRenderToExcel<T_Com_Product>("产品简称或缩写"));
                FiedNames.Add("BrandName", new ParamRenderToExcel<T_Com_Product>("品牌"));
                FiedNames.Add("ProductType", new ParamRenderToExcel<T_Com_Product>("产品形态"));
                FiedNames.Add("ProductPrice", new ParamRenderToExcel<T_Com_Product>("零售单价"));
                FiedNames.Add("Unit", new ParamRenderToExcel<T_Com_Product>("单位"));
                FiedNames.Add("OnMarket", new ParamRenderToExcel<T_Com_Product>("是否上市"));
                FiedNames.Add("MarketTime", new ParamRenderToExcel<T_Com_Product>("上市时间"));
                FiedNames.Add("Memo", new ParamRenderToExcel<T_Com_Product>("产品说明"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<T_Com_Product>("产品信息表", new List<T_Com_Product>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导出产品信息表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResult> ExportProductList(In_ProductPageList query)
        {
            try
            {
                List<T_Com_Product> list = (await _commonBLL.SelectProductList(query)).List;

                Dictionary<string, ParamRenderToExcel<T_Com_Product>> FiedNames = new Dictionary<string, ParamRenderToExcel<T_Com_Product>>();
                FiedNames.Add("Id", new ParamRenderToExcel<T_Com_Product>("编码"));
                FiedNames.Add("ProductModel", new ParamRenderToExcel<T_Com_Product>("产品型号"));
                FiedNames.Add("ProductName", new ParamRenderToExcel<T_Com_Product>("产品名称"));
                FiedNames.Add("ShortName", new ParamRenderToExcel<T_Com_Product>("产品简称或缩写"));
                FiedNames.Add("BrandName", new ParamRenderToExcel<T_Com_Product>("品牌"));
                FiedNames.Add("ProductType", new ParamRenderToExcel<T_Com_Product>("产品形态"));
                FiedNames.Add("ProductPrice", new ParamRenderToExcel<T_Com_Product>("零售单价"));
                FiedNames.Add("Unit", new ParamRenderToExcel<T_Com_Product>("单位"));
                FiedNames.Add("OnMarket", new ParamRenderToExcel<T_Com_Product>("是否上市"));
                FiedNames.Add("MarketTime", new ParamRenderToExcel<T_Com_Product>("上市时间"));
                FiedNames.Add("Memo", new ParamRenderToExcel<T_Com_Product>("产品说明"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<T_Com_Product>("产品信息表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导入产品信息表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ImportProduct()
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("编码", new ParamImportToList("Id"));
            FiedNames.Add("产品型号", new ParamImportToList("ProductModel"));
            FiedNames.Add("产品名称", new ParamImportToList("ProductName"));
            FiedNames.Add("产品简称或缩写", new ParamImportToList("ShortName"));
            FiedNames.Add("品牌", new ParamImportToList("BrandName"));
            FiedNames.Add("产品形态", new ParamImportToList("ProductType"));
            FiedNames.Add("零售单价", new ParamImportToList("ProductPrice", val => Convert.ToDouble(val)));
            FiedNames.Add("单位", new ParamImportToList("Unit"));
            FiedNames.Add("是否上市", new ParamImportToList("OnMarket"));
            FiedNames.Add("上市时间", new ParamImportToList("MarketTime", val => Convert.ToDateTime(val)));
            FiedNames.Add("产品说明", new ParamImportToList("Memo"));

            List<In_Product> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<In_Product>(st, FiedNames);

            return (await _commonBLL.ImportProduct(list)).ToAjaxResult();
        }
        #endregion

        #region 设备增删改查
        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddEquipment(In_Equipment data)
        {

            return (await _commonBLL.AddEquipment(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveEquipment(string Id)
        {

            return (await _commonBLL.DeleteEquipment(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateEquipment(In_Equipment data)
        {

            return (await _commonBLL.UpdateEquipment(data)).ToAjaxResult();
        }

        /// <summary>
        /// 设备列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_Equipment>>> EquipmentPageList(In_EquipmentList query)
        {
            return this.Success(await _commonBLL.SelectEquipmentPage(query));
        }

        /// <summary>
        /// 设备信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Equipment>> EquipmentInfo(string Id)
        {
            Out_Equipment data = await _commonBLL.SelectEquipmentInfo(Id);
            return this.Success(data);
        }

        /// <summary>
        /// 设施代码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> EquipmentCode()
        {
            return (await _commonBLL.GenerateNumber("EQ")).ToAjaxResult();
        }
        #endregion

        #region 设施增删改查
        /// <summary>
        /// 新增设施
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddFacility(In_FacilityAdd data)
        {

            return (await _commonBLL.AddFacility(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除设施
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveFacility(string Id)
        {

            return (await _commonBLL.DeleteFacility(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新设施
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateFacility(In_FacilityEdit data)
        {

            return (await _commonBLL.UpdateFacility(data)).ToAjaxResult();
        }

        /// <summary>
        /// 设施列表
        /// </summary>
        /// <param name="OrgId">企业ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_Facility>>> FacilityTree(long OrgId)
        {
            return this.Success(await _commonBLL.SelectFacilityTree(OrgId));
        }

        /// <summary>
        /// 设施列表带设备
        /// </summary>
        /// <param name="OrgId">企业ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_Facility>>> SelectFacilityEquipmentTree(long OrgId)
        {
            return this.Success(await _commonBLL.SelectFacilityEquipmentTree(OrgId));
        }

        /// <summary>
        /// 设施信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<T_Com_Facility>> FacilityInfo(string Id)
        {
            return this.Success(await _commonBLL.SelectFacility(Id));
        }

        /// <summary>
        /// 设施绑定设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> FacilityBindEquipment(In_FacilityBind data)
        {

            return (await _commonBLL.AddFacilityBind(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除设施绑定设备
        /// </summary>
        /// <param name="Id">绑定Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveFacilityBindEquipment(string Id)
        {

            return (await _commonBLL.DeleteFacilityBind(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 设施代码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> FacilityCode()
        {
            return (await _commonBLL.GenerateNumber("FC")).ToAjaxResult();
        }
        #endregion

        #region 排放类型增删改查
        /// <summary>
        /// 新增排放类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddClass(In_Class data)
        {

            return (await _commonBLL.AddClass(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除排放类型
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveClass(string Id)
        {

            return (await _commonBLL.DeleteClass(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新排放类型 新增的子类型Id为空
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateClass(In_Class data)
        {

            return (await _commonBLL.UpdateClass(data)).ToAjaxResult();
        }

        /// <summary>
        /// 分页查询排放类型列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_Class>>> ClassPageList(In_ClassPageLis query)
        {
            return this.Success(await _commonBLL.SelectClassList(query));
        }

        /// <summary>
        /// 排放类型信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Class>> ClassInfo(string Id)
        {
            return this.Success(await _commonBLL.SelectClass(Id));
        }
        #endregion

        #region 碳排核算增删改
        /// <summary>
        /// 新增碳排核算  新增的子类型Id为空
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddOrgClass(In_OrgClass data)
        {

            return (await _commonBLL.AddOrgClass(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除碳排核算
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveOrgClass(string Id)
        {

            return (await _commonBLL.DeleteOrgClass(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新碳排核算
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateOrgClass(In_OrgClass data)
        {

            return (await _commonBLL.UpdateOrgClass(data)).ToAjaxResult();
        }

        /// <summary>
        /// 企业碳核算信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_OrgClass>>> OrgClassInfo(In_OrgClassList query)
        {
            return this.Success(await _commonBLL.SelectOrgClass(query));
        }
        #endregion

        #region 生命周期模型
        /// <summary>
        /// 新增产品生命周期模型
        /// </summary>
        /// <param name="in_Model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddModel(In_Model in_Model)
        {

            return (await _commonBLL.AddModel(in_Model)).ToAjaxResult();
        }

        /// <summary>
        /// 删除产品生命周期模型
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveModel(string Id)
        {

            return (await _commonBLL.DeleteModel(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新产品生命周期模型
        /// </summary>
        /// <param name="in_Model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateModel(In_Model in_Model)
        {

            return (await _commonBLL.UpdateModel(in_Model)).ToAjaxResult();
        }

        /// <summary>
        /// 分页查询产品生命周期模型
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_ModelList>>> ModelPageList(In_ModelPageLis query)
        {
            return this.Success(await _commonBLL.SelectModelList(query));
        }

        /// <summary>
        /// 生命周期模型信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Model>> ModelInfo(string Id)
        {
            return this.Success(await _commonBLL.SelectModelInfo(Id));
        }

        /// <summary>
        /// 查询生命周期边界
        /// </summary>
        /// <param name="ProductBorder">生命周期边界</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_link>>> SelectLink(string ProductBorder)
        {
            return this.Success(await _commonBLL.SelectLink(ProductBorder));
        }

        /// <summary>
        /// 查询输入
        /// </summary>
        /// <param name="FacilityId">设施编码，多个用逗号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<ShuRu>>> SelectShuRu(string FacilityId, long OrgId)
        {
            return this.Success(await _commonBLL.SelectShuRu(FacilityId, OrgId));
        }
        #endregion

        #region 碳排分析
        /// <summary>
        /// 新增产品碳足迹
        /// </summary>
        /// <param name="in_Model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddProductModel(In_ProductModel in_Model)
        {

            return (await _commonBLL.AddProductModel(in_Model)).ToAjaxResult();
        }

        /// <summary>
        /// 删除产品碳足迹
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveProductModel(string Id)
        {

            return (await _commonBLL.DeleteProductModel(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 修改产品碳足迹
        /// </summary>
        /// <param name="in_Model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateProductModel(In_ProductModel in_Model)
        {

            return (await _commonBLL.UpdateProductModel(in_Model)).ToAjaxResult();
        }

        /// <summary>
        /// 分页查询产品碳足迹详情
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_ProductModelList>>> ModelProductModelPage(In_ModelPageLis query)
        {
            return this.Success(await _commonBLL.SelectProductModelPage(query));
        }

        /// <summary>
        /// 查询产品碳足迹详情
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_ProductModelInfo>> SelectProductModelInfo(string Id)
        {
            return this.Success(await _commonBLL.SelectProductModelInfo(Id));
        }
        #endregion

        #region 小时数据
        /// <summary>
        /// 需量分析
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<T_Prod_Energy_Hour>>> SelectEnergyHour(InEnergyHour query)
        {
            return this.Success(await _commonBLL.SelectEnergyHour(query));
        }

        /// <summary>
        /// 用能策略推荐
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Energy_Strategy>> SelectEnergyStrategy(InEnergyHour query)
        {
            Out_Energy_Strategy strategy = new Out_Energy_Strategy();
            strategy.Policy = new List<Out_Energy_Strategy_D>();

            T_Prod_Energy_H energy_H = await _commonBLL.SelectEnergyTimeList3(query);
            strategy.TTime = energy_H.TTime;
            strategy.UseVale = energy_H.UseVale;

            int sTime = int.Parse(energy_H.TTime.Substring(0, 2)) * 60 + int.Parse(energy_H.TTime.Substring(3, 2));
            int eTime = int.Parse(energy_H.TTime.Substring(6, 2)) * 60 + int.Parse(energy_H.TTime.Substring(9, 2)); 

            In_Policy policy = await _policyBLL.SelectPolicy(energy_H.EquipmentId);
            if (policy == null)
            {
                return this.Error(888, "没有绑定计费标准", strategy);
            }
            Dictionary<string, Out_Energy_Strategy_D> dic = new Dictionary<string, Out_Energy_Strategy_D>();
            foreach (var item in policy.PolicyDetils)
            {
                if (item.PolicyMonth.Contains(DateTime.Parse(query.beginDate).Month.ToString()))
                {
                    foreach (var jtem in item.PriceTimes)
                    {
                        if (dic.ContainsKey(jtem.TimePeriod))
                        {
                            dic[jtem.TimePeriod].TTime.Add(jtem.StartTime + "-" + jtem.EndTime);

                            int sT = int.Parse(jtem.StartTime.Substring(0, 2)) * 60 + int.Parse(jtem.StartTime.Substring(3, 2));
                            int eT = int.Parse(jtem.EndTime.Substring(0, 2)) * 60 + int.Parse(jtem.EndTime.Substring(3, 2));
                            double price = 0;
                            if (jtem.TimePeriod == "1")
                            {
                                price = item.JianPrice;
                            }
                            if (jtem.TimePeriod == "2")
                            {
                                price = item.FengPrice;
                            }
                            if (jtem.TimePeriod == "3")
                            {
                                price = item.PingPrice;
                            }
                            if (jtem.TimePeriod == "4")
                            {
                                price = item.GuPrice;
                            }
                            if (sTime >= sT && sTime <= eT)
                            {
                                double toatl = 0;
                                if (eTime > eT)//超出
                                {
                                    toatl = eTime - sT;
                                }
                                else
                                {
                                    toatl = eTime - sTime;
                                }
                                strategy.SaveCost = Math.Round(strategy.SaveCost + strategy.UseVale * (price - item.GuPrice) * (toatl / 60));
                            }
                            else if (eTime >= sT && eTime <= eT)
                            {
                                double toatl = 0;
                                if (sTime < sT)//超出
                                {
                                    toatl = eTime - sT;
                                }
                                else
                                {
                                    toatl = eTime - sTime;
                                }
                                strategy.SaveCost = Math.Round(strategy.SaveCost + strategy.UseVale * (dic[jtem.TimePeriod].Price - item.GuPrice) * (toatl / 60), 0);
                            }
                        }
                        else
                        {
                            Out_Energy_Strategy_D l = new Out_Energy_Strategy_D();
                            l.TTime = new List<string>();
                            l.TTime.Add(jtem.StartTime + "-" + jtem.EndTime);
                            if (jtem.TimePeriod == "1")
                            {
                                l.PolicyType = "尖";
                                l.Price = item.JianPrice;
                            }
                            if (jtem.TimePeriod == "2")
                            {
                                l.PolicyType = "峰";
                                l.Price = item.FengPrice;
                            }
                            if (jtem.TimePeriod == "3")
                            {
                                l.PolicyType = "平";
                                l.Price = item.PingPrice;
                            }
                            if (jtem.TimePeriod == "4")
                            {
                                l.PolicyType = "谷";
                                l.Price = item.GuPrice;
                            }
                            int sT = int.Parse(jtem.StartTime.Substring(0, 2)) * 60 + int.Parse(jtem.StartTime.Substring(3, 2));
                            int eT = int.Parse(jtem.EndTime.Substring(0, 2)) * 60 + int.Parse(jtem.EndTime.Substring(3, 2));
                            if (sTime >= sT && sTime <= eT)
                            {
                                double toatl = 0;
                                if (eTime > eT)//超出
                                {
                                    toatl = eTime - sT;
                                }
                                else
                                {
                                    toatl = eTime - sTime;
                                }
                                strategy.SaveCost = Math.Round(strategy.SaveCost + strategy.UseVale * (l.Price - item.GuPrice) * (toatl/60));
                            }
                            else if (eTime >= sT && eTime <= eT)
                            {
                                double toatl = 0;
                                if (sTime < sT)//超出
                                {
                                    toatl = eTime - sT;
                                }
                                else
                                {
                                    toatl = eTime - sTime;
                                }
                                strategy.SaveCost = Math.Round(strategy.SaveCost + strategy.UseVale * (l.Price - item.GuPrice) * (toatl / 60), 0);
                            }


                            dic.Add(jtem.TimePeriod, l);
                        }
                    }
                   
                }
            }
            strategy.Policy = dic.Values.ToList();

            return this.Success(strategy);
        }

        /// <summary>
        /// 查询设备能耗时段
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Energy_T_Reuslt>>> SelectEnergyHourList(InEnergyHourList query)
        {
            List<Energy_T_Reuslt> reuslts = new List<Energy_T_Reuslt>();

            List<T_Prod_Energy_T> Energy_Ts = await _commonBLL.SelectEnergyTimeList(query);

            Out_Equipment equipment = await _commonBLL.SelectEquipmentInfo(query.EquipmentId);
            if (equipment == null)
            {
                return this.Error(888, "没有绑定设备", reuslts);
            }

            In_Policy policy = await _policyBLL.SelectPolicy(equipment.PolicyId);
            if (policy == null)
            {
                return this.Error(888, "没有绑定计费标准", reuslts);
            }
            if (!(query.TimeType == "日" || (query.TimeType == "月") || (query.TimeType == "年")))
            {
                return this.Error(888, "时间类型:日、月、年", reuslts);
            }

            Energy_T_Reuslt t_Jian = new Energy_T_Reuslt();
            t_Jian.TimePeriod = "尖";
            t_Jian.Unit = policy.Unit;
            t_Jian.LageUnit = policy.LageUnit;
            t_Jian.Details = new List<Energy_T_Reuslt_Detail>();
            Energy_T_Reuslt t_Feng = new Energy_T_Reuslt();
            t_Feng.TimePeriod = "峰";
            t_Feng.Unit = policy.Unit;
            t_Feng.LageUnit = policy.LageUnit;
            t_Feng.Details = new List<Energy_T_Reuslt_Detail>();
            Energy_T_Reuslt t_Ping = new Energy_T_Reuslt();
            t_Ping.TimePeriod = "平";
            t_Ping.Unit = policy.Unit;
            t_Ping.LageUnit = policy.LageUnit;
            t_Ping.Details = new List<Energy_T_Reuslt_Detail>();
            Energy_T_Reuslt t_Gu = new Energy_T_Reuslt();
            t_Gu.TimePeriod = "谷";
            t_Gu.Unit = policy.Unit;
            t_Gu.LageUnit = policy.LageUnit;
            t_Gu.Details = new List<Energy_T_Reuslt_Detail>();
            Energy_T_Reuslt t_Heji = new Energy_T_Reuslt();
            t_Heji.TimePeriod = "合计";
            t_Heji.Unit = policy.Unit;
            t_Heji.LageUnit = policy.LageUnit;
            t_Heji.Details = new List<Energy_T_Reuslt_Detail>();

            string month = DateTime.Parse(query.endDate).Month.ToString();
           
            foreach (var item in policy.PolicyDetils)
            {
                if (item.PolicyMonth.Split(',').Contains(month))
                {
                    t_Jian.Price = item.JianPrice;
                    t_Feng.Price = item.FengPrice;
                    t_Ping.Price = item.PingPrice;
                    t_Gu.Price = item.GuPrice;
                }
            }

            Dictionary<string, Energy_T_Reuslt_Detail> dics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Jiandics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Fengdics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Pingdics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Gudics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            foreach (var item in Energy_Ts)
            {
                if (item.TimePeriod == "尖")
                {
                    t_Jian.UseVale = Math.Round(t_Jian.UseVale + item.UseVale, 2);
                    t_Jian.CostVale = Math.Round(t_Jian.CostVale + item.CostVale, 2);
                    t_Jian.CarbonEmission = Math.Round(t_Jian.CarbonEmission + item.CarbonEmission, 2);
                    t_Jian.ConvertCoal = Math.Round(t_Jian.ConvertCoal + item.ConvertCoal, 2);
                   
                    string j = item.TTime.Substring(0, 2);
                    if (Jiandics.ContainsKey(j))
                    {
                        Jiandics[j].UseVale = Math.Round(Jiandics[j].UseVale + item.UseVale, 2);
                        Jiandics[j].CostVale = Math.Round(Jiandics[j].CostVale + item.CostVale, 2);
                        Jiandics[j].CarbonEmission = Math.Round(Jiandics[j].CarbonEmission + item.CarbonEmission, 2);
                        Jiandics[j].ConvertCoal = Math.Round(Jiandics[j].ConvertCoal + item.ConvertCoal, 2);
                    }
                    else
                    {
                        Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                        detail.TTime = j;
                        detail.UseVale = item.UseVale;
                        detail.CostVale = item.CostVale;
                        detail.CarbonEmission = item.CarbonEmission;
                        detail.ConvertCoal = item.ConvertCoal;
                        Jiandics.Add(j, detail);
                    }
                }
                if (item.TimePeriod == "峰")
                {
                    t_Feng.UseVale = Math.Round(t_Feng.UseVale + item.UseVale, 2);
                    t_Feng.CostVale = Math.Round(t_Feng.CostVale + item.CostVale, 2);
                    t_Feng.CarbonEmission = Math.Round(t_Feng.CarbonEmission + item.CarbonEmission, 2);
                    t_Feng.ConvertCoal = Math.Round(t_Feng.ConvertCoal + item.ConvertCoal, 2);
                    string f = item.TTime.Substring(0, 2);
                    if (Fengdics.ContainsKey(f))
                    {
                        Fengdics[f].UseVale = Math.Round(Fengdics[f].UseVale + item.UseVale, 2);
                        Fengdics[f].CostVale = Math.Round(Fengdics[f].CostVale + item.CostVale, 2);
                        Fengdics[f].CarbonEmission = Math.Round(Fengdics[f].CarbonEmission + item.CarbonEmission, 2);
                        Fengdics[f].ConvertCoal = Math.Round(Fengdics[f].ConvertCoal + item.ConvertCoal, 2);
                    }
                    else
                    {
                        Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                        detail.TTime = f;
                        detail.UseVale = item.UseVale;
                        detail.CostVale = item.CostVale;
                        detail.CarbonEmission = item.CarbonEmission;
                        detail.ConvertCoal = item.ConvertCoal;
                        Fengdics.Add(f, detail);
                    }
                }
                if (item.TimePeriod == "平")
                {
                    t_Ping.UseVale = Math.Round(t_Ping.UseVale + item.UseVale, 2);
                    t_Ping.CostVale = Math.Round(t_Ping.CostVale + item.CostVale, 2);
                    t_Ping.CarbonEmission = Math.Round(t_Ping.CarbonEmission + item.CarbonEmission, 2);
                    t_Ping.ConvertCoal = Math.Round(t_Ping.ConvertCoal + item.ConvertCoal, 2);
                    string p = item.TTime.Substring(0, 2);
                    if (Pingdics.ContainsKey(p))
                    {
                        Pingdics[p].UseVale = Math.Round(Pingdics[p].UseVale + item.UseVale, 2);
                        Pingdics[p].CostVale = Math.Round(Pingdics[p].CostVale + item.CostVale, 2);
                        Pingdics[p].CarbonEmission = Math.Round(Pingdics[p].CarbonEmission + item.CarbonEmission, 2);
                        Pingdics[p].ConvertCoal = Math.Round(Pingdics[p].ConvertCoal + item.ConvertCoal, 2);
                    }
                    else
                    {
                        Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                        detail.TTime = p;
                        detail.UseVale = item.UseVale;
                        detail.CostVale = item.CostVale;
                        detail.CarbonEmission = item.CarbonEmission;
                        detail.ConvertCoal = item.ConvertCoal;
                        Pingdics.Add(p, detail);
                    }
                }
                if (item.TimePeriod == "谷")
                {
                    t_Gu.EndVale = item.EndVale;
                    t_Gu.UseVale = Math.Round(t_Gu.UseVale + item.UseVale, 2);
                    t_Gu.CostVale = Math.Round(t_Gu.CostVale + item.CostVale, 2);
                    t_Gu.CarbonEmission = Math.Round(t_Gu.CarbonEmission + item.CarbonEmission, 2);
                    t_Gu.ConvertCoal = Math.Round(t_Gu.ConvertCoal + item.ConvertCoal, 2);
                    string g = item.TTime.Substring(0, 2);
                    if (Gudics.ContainsKey(g))
                    {
                        Gudics[g].UseVale = Math.Round(Gudics[g].UseVale + item.UseVale, 2);
                        Gudics[g].CostVale = Math.Round(Gudics[g].CostVale + item.CostVale, 2);
                        Gudics[g].CarbonEmission = Math.Round(Gudics[g].CarbonEmission + item.CarbonEmission, 2);
                        Gudics[g].ConvertCoal = Math.Round(Gudics[g].ConvertCoal + item.ConvertCoal, 2);
                    }
                    else
                    {
                        Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                        detail.TTime = g;
                        detail.UseVale = item.UseVale;
                        detail.CostVale = item.CostVale;
                        detail.CarbonEmission = item.CarbonEmission;
                        detail.ConvertCoal = item.ConvertCoal;
                        Gudics.Add(g, detail);
                    }
                }
                if (t_Heji.InitVale == 0)
                {
                    t_Heji.InitVale = item.InitVale;
                }
                t_Heji.EndVale = item.EndVale;
                t_Heji.UseVale = Math.Round(t_Heji.UseVale + item.UseVale, 2);
                t_Heji.CostVale = Math.Round(t_Heji.CostVale + item.CostVale, 2);
                t_Heji.CarbonEmission = Math.Round(t_Heji.CarbonEmission + item.CarbonEmission, 2);
                t_Heji.ConvertCoal = Math.Round(t_Heji.ConvertCoal + item.ConvertCoal, 2);
                string k = "";// item.TTime.Substring(0, 2);
                if (query.TimeType == "年")
                {
                    k = item.DDate.ToString("MM");
                }
                if (query.TimeType == "月")
                {
                    k = item.DDate.ToString("dd");
                }
                if (query.TimeType == "日")
                {
                    k = item.TTime.Substring(0, 2);
                }
                if (dics.ContainsKey(k))
                {
                    dics[k].UseVale = Math.Round(dics[k].UseVale + item.UseVale, 2);
                    dics[k].CostVale = Math.Round(dics[k].CostVale + item.CostVale, 2);
                    dics[k].CarbonEmission = Math.Round(dics[k].CarbonEmission + item.CarbonEmission, 2);
                    dics[k].ConvertCoal = Math.Round(dics[k].ConvertCoal + item.ConvertCoal, 2);
                }
                else
                {
                    Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                    detail.TTime = k;
                    detail.UseVale = item.UseVale;
                    detail.CostVale = item.CostVale;
                    detail.CarbonEmission = item.CarbonEmission;
                    detail.ConvertCoal = item.ConvertCoal;
                    dics.Add(k, detail);
                }
            }
            t_Heji.Details = dics.Values.ToList().OrderBy(e => e.TTime).ToList();
            t_Jian.Details = Jiandics.Values.ToList().OrderBy(e => e.TTime).ToList();
            t_Feng.Details = Fengdics.Values.ToList().OrderBy(e => e.TTime).ToList();
            t_Ping.Details = Pingdics.Values.ToList().OrderBy(e => e.TTime).ToList();
            t_Gu.Details = Gudics.Values.ToList().OrderBy(e => e.TTime).ToList();

            reuslts.Add(t_Jian);
            reuslts.Add(t_Feng);
            reuslts.Add(t_Ping);
            reuslts.Add(t_Gu);
            reuslts.Add(t_Heji);

            return this.Success(reuslts);
        }


        /// <summary>
        /// 查询单元能耗时段
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Energy_F_Reuslt>> SelectFacilityEnergyHourList(InEnergyHourList query)
        {
            Energy_F_Reuslt energy_F_Reuslt = new Energy_F_Reuslt();
            energy_F_Reuslt.Facilitys = new List<Energy_F_F>();
            energy_F_Reuslt.Times = new List<Energy_F_D>();
            energy_F_Reuslt.TimePeriods = new List<Energy_T_Reuslt>();
            Energy_T_Reuslt t_Jian = new Energy_T_Reuslt();
            t_Jian.TimePeriod = "尖";
            t_Jian.Details = new List<Energy_T_Reuslt_Detail>();
            Energy_T_Reuslt t_Feng = new Energy_T_Reuslt();
            t_Feng.TimePeriod = "峰";
            t_Feng.Details = new List<Energy_T_Reuslt_Detail>();
            Energy_T_Reuslt t_Ping = new Energy_T_Reuslt();
            t_Ping.TimePeriod = "平";
            t_Ping.Details = new List<Energy_T_Reuslt_Detail>();
            Energy_T_Reuslt t_Gu = new Energy_T_Reuslt();
            t_Gu.TimePeriod = "谷";
            t_Gu.Details = new List<Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Jiandics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Fengdics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Pingdics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            Dictionary<string, Energy_T_Reuslt_Detail> Gudics = new Dictionary<string, Energy_T_Reuslt_Detail>();
            if (string.IsNullOrEmpty(query.FacilityId))
            {
                return this.Error(888, "缺少设施编码", energy_F_Reuslt);
            }
            if (string.IsNullOrEmpty(query.FactorId))
            {
                return this.Error(888, "缺少排放因子", energy_F_Reuslt);
            }
            if (query.orgId == null)
            {
                return this.Error(888, "缺少企业ID", energy_F_Reuslt);
            }
            if (!(query.TimeType == "日" || (query.TimeType == "月") || (query.TimeType == "年")))
            {
                return this.Error(888, "时间类型:日、月、年", energy_F_Reuslt);
            }
            //时段明细
            Dictionary<string, Energy_F_D> Timekeys = new Dictionary<string, Energy_F_D>();

            string EquipmentId = "";
            //子设施
            List<T_Com_Facility> _Facilities = await _commonBLL.SelectFacilityByParentId(query.FacilityId);
            if (_Facilities == null)
            {
                InEnergyHourList queryPage = new InEnergyHourList();
                queryPage.FacilityId = query.FacilityId;
                queryPage.FactorId = query.FactorId;
                queryPage.beginDate = query.beginDate;
                queryPage.endDate = query.endDate;
                queryPage.orgId = query.orgId;
                List<T_Prod_Energy_T2> energies = await _commonBLL.SelectEnergyTimeList2(queryPage);
                Dictionary<string, Energy_F_F> dic = new Dictionary<string, Energy_F_F>();
                foreach (var item in energies)
                {
                    EquipmentId = item.EquipmentId;
                    if (item.TimePeriod == "尖")
                    {
                        t_Jian.Unit = item.Unit;
                        t_Jian.LageUnit = item.LageUnit;
                        t_Jian.UseVale = Math.Round(t_Jian.UseVale + item.UseVale, 2);
                        t_Jian.CostVale = Math.Round(t_Jian.CostVale + item.CostVale, 2);
                        t_Jian.CarbonEmission = Math.Round(t_Jian.CarbonEmission + item.CarbonEmission, 2);
                        t_Jian.ConvertCoal = Math.Round(t_Jian.ConvertCoal + item.ConvertCoal, 2);

                        string j = item.TTime.Substring(0, 2);
                        if (Jiandics.ContainsKey(j))
                        {
                            Jiandics[j].UseVale = Math.Round(Jiandics[j].UseVale + item.UseVale, 2);
                            Jiandics[j].CostVale = Math.Round(Jiandics[j].CostVale + item.CostVale, 2);
                            Jiandics[j].CarbonEmission = Math.Round(Jiandics[j].CarbonEmission + item.CarbonEmission, 2);
                            Jiandics[j].ConvertCoal = Math.Round(Jiandics[j].ConvertCoal + item.ConvertCoal, 2);
                        }
                        else
                        {
                            Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                            detail.TTime = j;
                            detail.UseVale = item.UseVale;
                            detail.CostVale = item.CostVale;
                            detail.CarbonEmission = item.CarbonEmission;
                            detail.ConvertCoal = item.ConvertCoal;
                            Jiandics.Add(j, detail);
                        }
                    }
                    if (item.TimePeriod == "峰")
                    {
                        t_Feng.Unit = item.Unit;
                        t_Feng.LageUnit = item.LageUnit;
                        t_Feng.UseVale = Math.Round(t_Feng.UseVale + item.UseVale, 2);
                        t_Feng.CostVale = Math.Round(t_Feng.CostVale + item.CostVale, 2);
                        t_Feng.CarbonEmission = Math.Round(t_Feng.CarbonEmission + item.CarbonEmission, 2);
                        t_Feng.ConvertCoal = Math.Round(t_Feng.ConvertCoal + item.ConvertCoal, 2);
                        string f = item.TTime.Substring(0, 2);
                        if (Fengdics.ContainsKey(f))
                        {
                            Fengdics[f].UseVale = Math.Round(Fengdics[f].UseVale + item.UseVale, 2);
                            Fengdics[f].CostVale = Math.Round(Fengdics[f].CostVale + item.CostVale, 2);
                            Fengdics[f].CarbonEmission = Math.Round(Fengdics[f].CarbonEmission + item.CarbonEmission, 2);
                            Fengdics[f].ConvertCoal = Math.Round(Fengdics[f].ConvertCoal + item.ConvertCoal, 2);
                        }
                        else
                        {
                            Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                            detail.TTime = f;
                            detail.UseVale = item.UseVale;
                            detail.CostVale = item.CostVale;
                            detail.CarbonEmission = item.CarbonEmission;
                            detail.ConvertCoal = item.ConvertCoal;
                            Fengdics.Add(f, detail);
                        }
                    }
                    if (item.TimePeriod == "平")
                    {
                        t_Ping.Unit = item.Unit;
                        t_Ping.LageUnit = item.LageUnit;
                        t_Ping.UseVale = Math.Round(t_Ping.UseVale + item.UseVale, 2);
                        t_Ping.CostVale = Math.Round(t_Ping.CostVale + item.CostVale, 2);
                        t_Ping.CarbonEmission = Math.Round(t_Ping.CarbonEmission + item.CarbonEmission, 2);
                        t_Ping.ConvertCoal = Math.Round(t_Ping.ConvertCoal + item.ConvertCoal, 2);
                        string p = item.TTime.Substring(0, 2);
                        if (Pingdics.ContainsKey(p))
                        {
                            Pingdics[p].UseVale = Math.Round(Pingdics[p].UseVale + item.UseVale, 2);
                            Pingdics[p].CostVale = Math.Round(Pingdics[p].CostVale + item.CostVale, 2);
                            Pingdics[p].CarbonEmission = Math.Round(Pingdics[p].CarbonEmission + item.CarbonEmission, 2);
                            Pingdics[p].ConvertCoal = Math.Round(Pingdics[p].ConvertCoal + item.ConvertCoal, 2);
                        }
                        else
                        {
                            Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                            detail.TTime = p;
                            detail.UseVale = item.UseVale;
                            detail.CostVale = item.CostVale;
                            detail.CarbonEmission = item.CarbonEmission;
                            detail.ConvertCoal = item.ConvertCoal;
                            Pingdics.Add(p, detail);
                        }
                    }
                    if (item.TimePeriod == "谷")
                    {
                        t_Gu.Unit = item.Unit;
                        t_Gu.LageUnit = item.LageUnit;
                        t_Gu.EndVale = item.EndVale;
                        t_Gu.UseVale = Math.Round(t_Gu.UseVale + item.UseVale, 2);
                        t_Gu.CostVale = Math.Round(t_Gu.CostVale + item.CostVale, 2);
                        t_Gu.CarbonEmission = Math.Round(t_Gu.CarbonEmission + item.CarbonEmission, 2);
                        t_Gu.ConvertCoal = Math.Round(t_Gu.ConvertCoal + item.ConvertCoal, 2);
                        string g = item.TTime.Substring(0, 2);
                        if (Gudics.ContainsKey(g))
                        {
                            Gudics[g].UseVale = Math.Round(Gudics[g].UseVale + item.UseVale, 2);
                            Gudics[g].CostVale = Math.Round(Gudics[g].CostVale + item.CostVale, 2);
                            Gudics[g].CarbonEmission = Math.Round(Gudics[g].CarbonEmission + item.CarbonEmission, 2);
                            Gudics[g].ConvertCoal = Math.Round(Gudics[g].ConvertCoal + item.ConvertCoal, 2);
                        }
                        else
                        {
                            Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                            detail.TTime = g;
                            detail.UseVale = item.UseVale;
                            detail.CostVale = item.CostVale;
                            detail.CarbonEmission = item.CarbonEmission;
                            detail.ConvertCoal = item.ConvertCoal;
                            Gudics.Add(g, detail);
                        }
                    }

                    energy_F_Reuslt.Unit = item.Unit;
                    energy_F_Reuslt.LageUnit = item.LageUnit;
                    energy_F_Reuslt.UseVale = Math.Round(energy_F_Reuslt.UseVale + item.UseVale, 2);
                    energy_F_Reuslt.CostVale = Math.Round(energy_F_Reuslt.CostVale + item.CostVale, 2);
                    energy_F_Reuslt.CarbonEmission = Math.Round(energy_F_Reuslt.CarbonEmission + item.CarbonEmission, 2);
                    energy_F_Reuslt.ConvertCoal = Math.Round(energy_F_Reuslt.ConvertCoal + item.ConvertCoal, 2);
                    if (dic.ContainsKey(item.EquipmentId))
                    {
                        dic[item.EquipmentId].UseVale = Math.Round(dic[item.EquipmentId].UseVale + item.UseVale, 2);
                    }
                    else
                    {
                        Energy_F_F energy_F_F = new Energy_F_F();
                        energy_F_F.FacilityName = item.EquipmentName;
                        energy_F_F.UseVale = Math.Round(item.UseVale, 2);
                        dic.Add(item.EquipmentId, energy_F_F);
                    }
                    if (query.TimeType == "年")
                    {
                        string ddate = item.DDate.ToString("yyyy-MM");
                        if (Timekeys.ContainsKey(ddate))
                        {
                            Timekeys[ddate].UseVale = Math.Round(Timekeys[ddate].UseVale + item.UseVale, 2);
                            Timekeys[ddate].CarbonEmission = Math.Round(Timekeys[ddate].CarbonEmission + item.CarbonEmission, 2);
                            Timekeys[ddate].ConvertCoal = Math.Round(Timekeys[ddate].ConvertCoal + item.ConvertCoal, 2);
                            Timekeys[ddate].CostVale = Math.Round(Timekeys[ddate].CostVale + item.CostVale, 2);
                        }
                        else
                        {
                            Energy_F_D energy_F_D = new Energy_F_D();
                            energy_F_D.TTime = item.DDate.ToString("MM");
                            energy_F_D.UseVale = Math.Round(item.UseVale, 2);
                            energy_F_D.CarbonEmission = Math.Round(item.CarbonEmission, 2);
                            energy_F_D.ConvertCoal = Math.Round(item.ConvertCoal, 2);
                            energy_F_D.CostVale = Math.Round(item.CostVale, 2);
                            Timekeys.Add(ddate, energy_F_D);
                        }
                    }
                    if (query.TimeType == "月")
                    {
                        string ddate = item.DDate.ToString("yyyy-MM-dd");
                        if (Timekeys.ContainsKey(ddate))
                        {
                            Timekeys[ddate].UseVale = Math.Round(Timekeys[ddate].UseVale + item.UseVale, 2);
                            Timekeys[ddate].CarbonEmission = Math.Round(Timekeys[ddate].CarbonEmission + item.CarbonEmission, 2);
                            Timekeys[ddate].ConvertCoal = Math.Round(Timekeys[ddate].ConvertCoal + item.ConvertCoal, 2);
                            Timekeys[ddate].CostVale = Math.Round(Timekeys[ddate].CostVale + item.CostVale, 2);
                        }
                        else
                        {
                            Energy_F_D energy_F_D = new Energy_F_D();
                            energy_F_D.TTime = item.DDate.ToString("dd");
                            energy_F_D.UseVale = Math.Round(item.UseVale, 2);
                            energy_F_D.CarbonEmission = Math.Round(item.CarbonEmission, 2);
                            energy_F_D.ConvertCoal = Math.Round(item.ConvertCoal, 2);
                            energy_F_D.CostVale = Math.Round(item.CostVale, 2);
                            Timekeys.Add(ddate, energy_F_D);
                        }
                    }
                    if (query.TimeType == "日")
                    {
                        string ddate = item.DDate.ToString("yyyy-MM-dd HH ") + item.TTime;
                        if (Timekeys.ContainsKey(ddate))
                        {
                            Timekeys[ddate].UseVale = Math.Round(Timekeys[ddate].UseVale + item.UseVale, 2);
                            Timekeys[ddate].CarbonEmission = Math.Round(Timekeys[ddate].CarbonEmission + item.CarbonEmission, 2);
                            Timekeys[ddate].ConvertCoal = Math.Round(Timekeys[ddate].ConvertCoal + item.ConvertCoal, 2);
                            Timekeys[ddate].CostVale = Math.Round(Timekeys[ddate].CostVale + item.CostVale, 2);
                        }
                        else
                        {
                            Energy_F_D energy_F_D = new Energy_F_D();
                            energy_F_D.TTime = item.TTime.Substring(0, 2);
                            energy_F_D.UseVale = Math.Round(item.UseVale, 2);
                            energy_F_D.CarbonEmission = Math.Round(item.CarbonEmission, 2);
                            energy_F_D.ConvertCoal = Math.Round(item.ConvertCoal, 2);
                            energy_F_D.CostVale = Math.Round(item.CostVale, 2);
                            Timekeys.Add(ddate, energy_F_D);
                        }
                    }
                }
                energy_F_Reuslt.Facilitys = dic.Values.ToList();
            }
            else
            {
                foreach (var jtem in _Facilities)
                {
                    InEnergyHourList queryPage = new InEnergyHourList();
                    queryPage.FacilityId = jtem.Id;
                    queryPage.FactorId = query.FactorId;
                    queryPage.beginDate = query.beginDate;
                    queryPage.endDate = query.endDate;
                    queryPage.orgId = query.orgId;
                    List<T_Prod_Energy_T2> energies = await _commonBLL.SelectEnergyTimeList2(queryPage);
                    Energy_F_F energy_F_F = new Energy_F_F();
                    energy_F_F.FacilityName = jtem.FacilityName;
                    foreach (var item in energies)
                    {
                        EquipmentId = item.EquipmentId;
                        if (item.TimePeriod == "尖")
                        {
                            t_Jian.Unit = item.Unit;
                            t_Jian.LageUnit = item.LageUnit;
                            t_Jian.UseVale = Math.Round(t_Jian.UseVale + item.UseVale, 2);
                            t_Jian.CostVale = Math.Round(t_Jian.CostVale + item.CostVale, 2);
                            t_Jian.CarbonEmission = Math.Round(t_Jian.CarbonEmission + item.CarbonEmission, 2);
                            t_Jian.ConvertCoal = Math.Round(t_Jian.ConvertCoal + item.ConvertCoal, 2);
                            string j = item.TTime.Substring(0, 2);
                            if (Jiandics.ContainsKey(j))
                            {
                                Jiandics[j].UseVale = Math.Round(Jiandics[j].UseVale + item.UseVale, 2);
                                Jiandics[j].CostVale = Math.Round(Jiandics[j].CostVale + item.CostVale, 2);
                                Jiandics[j].CarbonEmission = Math.Round(Jiandics[j].CarbonEmission + item.CarbonEmission, 2);
                                Jiandics[j].ConvertCoal = Math.Round(Jiandics[j].ConvertCoal + item.ConvertCoal, 2);
                            }
                            else
                            {
                                Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                                detail.TTime = j;
                                detail.UseVale = item.UseVale;
                                detail.CostVale = item.CostVale;
                                detail.CarbonEmission = item.CarbonEmission;
                                detail.ConvertCoal = item.ConvertCoal;
                                Jiandics.Add(j, detail);
                            }
                        }
                        if (item.TimePeriod == "峰")
                        {
                            t_Feng.Unit = item.Unit;
                            t_Feng.LageUnit = item.LageUnit;
                            t_Feng.UseVale = Math.Round(t_Feng.UseVale + item.UseVale, 2);
                            t_Feng.CostVale = Math.Round(t_Feng.CostVale + item.CostVale, 2);
                            t_Feng.CarbonEmission = Math.Round(t_Feng.CarbonEmission + item.CarbonEmission, 2);
                            t_Feng.ConvertCoal = Math.Round(t_Feng.ConvertCoal + item.ConvertCoal, 2);
                            string f = item.TTime.Substring(0, 2);
                            if (Fengdics.ContainsKey(f))
                            {
                                Fengdics[f].UseVale = Math.Round(Fengdics[f].UseVale + item.UseVale, 2);
                                Fengdics[f].CostVale = Math.Round(Fengdics[f].CostVale + item.CostVale, 2);
                                Fengdics[f].CarbonEmission = Math.Round(Fengdics[f].CarbonEmission + item.CarbonEmission, 2);
                                Fengdics[f].ConvertCoal = Math.Round(Fengdics[f].ConvertCoal + item.ConvertCoal, 2);
                            }
                            else
                            {
                                Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                                detail.TTime = f;
                                detail.UseVale = item.UseVale;
                                detail.CostVale = item.CostVale;
                                detail.CarbonEmission = item.CarbonEmission;
                                detail.ConvertCoal = item.ConvertCoal;
                                Fengdics.Add(f, detail);
                            }
                        }
                        if (item.TimePeriod == "平")
                        {
                            t_Ping.Unit = item.Unit;
                            t_Ping.LageUnit = item.LageUnit;
                            t_Ping.UseVale = Math.Round(t_Ping.UseVale + item.UseVale, 2);
                            t_Ping.CostVale = Math.Round(t_Ping.CostVale + item.CostVale, 2);
                            t_Ping.CarbonEmission = Math.Round(t_Ping.CarbonEmission + item.CarbonEmission, 2);
                            t_Ping.ConvertCoal = Math.Round(t_Ping.ConvertCoal + item.ConvertCoal, 2);
                            string p = item.TTime.Substring(0, 2);
                            if (Pingdics.ContainsKey(p))
                            {
                                Pingdics[p].UseVale = Math.Round(Pingdics[p].UseVale + item.UseVale, 2);
                                Pingdics[p].CostVale = Math.Round(Pingdics[p].CostVale + item.CostVale, 2);
                                Pingdics[p].CarbonEmission = Math.Round(Pingdics[p].CarbonEmission + item.CarbonEmission, 2);
                                Pingdics[p].ConvertCoal = Math.Round(Pingdics[p].ConvertCoal + item.ConvertCoal, 2);
                            }
                            else
                            {
                                Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                                detail.TTime = p;
                                detail.UseVale = item.UseVale;
                                detail.CostVale = item.CostVale;
                                detail.CarbonEmission = item.CarbonEmission;
                                detail.ConvertCoal = item.ConvertCoal;
                                Pingdics.Add(p, detail);
                            }
                        }
                        if (item.TimePeriod == "谷")
                        {
                            t_Gu.Unit = item.Unit;
                            t_Gu.LageUnit = item.LageUnit;
                            t_Gu.EndVale = item.EndVale;
                            t_Gu.UseVale = Math.Round(t_Gu.UseVale + item.UseVale, 2);
                            t_Gu.CostVale = Math.Round(t_Gu.CostVale + item.CostVale, 2);
                            t_Gu.CarbonEmission = Math.Round(t_Gu.CarbonEmission + item.CarbonEmission, 2);
                            t_Gu.ConvertCoal = Math.Round(t_Gu.ConvertCoal + item.ConvertCoal, 2);
                            string g = item.TTime.Substring(0, 2);
                            if (Gudics.ContainsKey(g))
                            {
                                Gudics[g].UseVale = Math.Round(Gudics[g].UseVale + item.UseVale, 2);
                                Gudics[g].CostVale = Math.Round(Gudics[g].CostVale + item.CostVale, 2);
                                Gudics[g].CarbonEmission = Math.Round(Gudics[g].CarbonEmission + item.CarbonEmission, 2);
                                Gudics[g].ConvertCoal = Math.Round(Gudics[g].ConvertCoal + item.ConvertCoal, 2);
                            }
                            else
                            {
                                Energy_T_Reuslt_Detail detail = new Energy_T_Reuslt_Detail();
                                detail.TTime = g;
                                detail.UseVale = item.UseVale;
                                detail.CostVale = item.CostVale;
                                detail.CarbonEmission = item.CarbonEmission;
                                detail.ConvertCoal = item.ConvertCoal;
                                Gudics.Add(g, detail);
                            }
                        }
                        energy_F_F.UseVale = Math.Round(energy_F_F.UseVale + item.UseVale, 2);
                        energy_F_Reuslt.Unit = item.Unit;
                        energy_F_Reuslt.LageUnit = item.LageUnit;
                        energy_F_Reuslt.UseVale = Math.Round(energy_F_Reuslt.UseVale + item.UseVale, 2);
                        energy_F_Reuslt.CostVale = Math.Round(energy_F_Reuslt.CostVale + item.CostVale, 2);
                        energy_F_Reuslt.CarbonEmission = Math.Round(energy_F_Reuslt.CarbonEmission + item.CarbonEmission, 2);
                        energy_F_Reuslt.ConvertCoal = Math.Round(energy_F_Reuslt.ConvertCoal + item.ConvertCoal, 2);
                        if (query.TimeType == "年")
                        {
                            string ddate = item.DDate.ToString("yyyy-MM");
                            if (Timekeys.ContainsKey(ddate))
                            {
                                Timekeys[ddate].UseVale = Math.Round(Timekeys[ddate].UseVale + item.UseVale, 2);
                                Timekeys[ddate].CarbonEmission = Math.Round(Timekeys[ddate].CarbonEmission + item.CarbonEmission, 2);
                                Timekeys[ddate].ConvertCoal = Math.Round(Timekeys[ddate].ConvertCoal + item.ConvertCoal, 2);
                                Timekeys[ddate].CostVale = Math.Round(Timekeys[ddate].CostVale + item.CostVale, 2);
                            }
                            else
                            {
                                Energy_F_D energy_F_D = new Energy_F_D();
                                energy_F_D.TTime = item.DDate.ToString("MM");
                                energy_F_D.UseVale = Math.Round(item.UseVale, 2);
                                energy_F_D.CarbonEmission = Math.Round(item.CarbonEmission, 2);
                                energy_F_D.ConvertCoal = Math.Round(item.ConvertCoal, 2);
                                energy_F_D.CostVale = Math.Round(item.CostVale, 2);
                                Timekeys.Add(ddate, energy_F_D);
                            }
                        }
                        if (query.TimeType == "月")
                        {
                            string ddate = item.DDate.ToString("yyyy-MM-dd");
                            if (Timekeys.ContainsKey(ddate))
                            {
                                Timekeys[ddate].UseVale = Math.Round(Timekeys[ddate].UseVale + item.UseVale, 2);
                                Timekeys[ddate].CarbonEmission = Math.Round(Timekeys[ddate].CarbonEmission + item.CarbonEmission, 2);
                                Timekeys[ddate].ConvertCoal = Math.Round(Timekeys[ddate].ConvertCoal + item.ConvertCoal, 2);
                                Timekeys[ddate].CostVale = Math.Round(Timekeys[ddate].CostVale + item.CostVale, 2);
                            }
                            else
                            {
                                Energy_F_D energy_F_D = new Energy_F_D();
                                energy_F_D.TTime = item.DDate.ToString("dd");
                                energy_F_D.UseVale = Math.Round(item.UseVale, 2);
                                energy_F_D.CarbonEmission = Math.Round(item.CarbonEmission, 2);
                                energy_F_D.ConvertCoal = Math.Round(item.ConvertCoal, 2);
                                energy_F_D.CostVale = Math.Round(item.CostVale, 2);
                                Timekeys.Add(ddate, energy_F_D);
                            }
                        }
                        if (query.TimeType == "日")
                        {
                            string ddate = item.DDate.ToString("yyyy-MM-dd HH ")+ item.TTime;
                            if (Timekeys.ContainsKey(ddate))
                            {
                                Timekeys[ddate].UseVale = Math.Round(Timekeys[ddate].UseVale + item.UseVale, 2);
                                Timekeys[ddate].CarbonEmission = Math.Round(Timekeys[ddate].CarbonEmission + item.CarbonEmission, 2);
                                Timekeys[ddate].ConvertCoal = Math.Round(Timekeys[ddate].ConvertCoal + item.ConvertCoal, 2);
                                Timekeys[ddate].CostVale = Math.Round(Timekeys[ddate].CostVale + item.CostVale, 2);
                            }
                            else
                            {
                                Energy_F_D energy_F_D = new Energy_F_D();
                                energy_F_D.TTime = item.TTime.Substring(0, 2);
                                energy_F_D.UseVale = Math.Round(item.UseVale, 2);
                                energy_F_D.CarbonEmission = Math.Round(item.CarbonEmission, 2);
                                energy_F_D.ConvertCoal = Math.Round(item.ConvertCoal, 2);
                                energy_F_D.CostVale = Math.Round(item.CostVale, 2);
                                Timekeys.Add(ddate, energy_F_D);
                            }
                        }
                    }
                    energy_F_Reuslt.Facilitys.Add(energy_F_F);
                }
            }

            string month = DateTime.Parse(query.endDate).Month.ToString();
            if (string.IsNullOrEmpty(EquipmentId))
            {
                In_PolicyList queryPolicy = new In_PolicyList();
                queryPolicy.OrgId = query.orgId;
                Out_Policy Opolicy = (await _policyBLL.SelectPolicyList(queryPolicy)).List.First();
                if (Opolicy != null)
                {
                    In_Policy policy = await _policyBLL.SelectPolicy(Opolicy.Id);
                    foreach (var item in policy.PolicyDetils)
                    {
                        if (item.PolicyMonth.Split(',').Contains(month))
                        {
                            t_Jian.Price = item.JianPrice;
                            t_Feng.Price = item.FengPrice;
                            t_Ping.Price = item.PingPrice;
                            t_Gu.Price = item.GuPrice;
                        }
                    }
                }
            }
            else
            {
                Out_Equipment equipment = await _commonBLL.SelectEquipmentInfo(EquipmentId);
                In_Policy policy = await _policyBLL.SelectPolicy(equipment.PolicyId);
                foreach (var item in policy.PolicyDetils)
                {
                    if (item.PolicyMonth.Split(',').Contains(month))
                    {
                        t_Jian.Price = item.JianPrice;
                        t_Feng.Price = item.FengPrice;
                        t_Ping.Price = item.PingPrice;
                        t_Gu.Price = item.GuPrice;
                    }
                }
            }

            t_Jian.Details = Jiandics.Values.ToList().OrderBy(e => e.TTime).ToList();
            t_Feng.Details = Fengdics.Values.ToList().OrderBy(e => e.TTime).ToList();
            t_Ping.Details = Pingdics.Values.ToList().OrderBy(e => e.TTime).ToList();
            t_Gu.Details = Gudics.Values.ToList().OrderBy(e => e.TTime).ToList();
            energy_F_Reuslt.Times = Timekeys.Values.ToList();
            energy_F_Reuslt.TimePeriods.Add(t_Jian);
            energy_F_Reuslt.TimePeriods.Add(t_Feng);
            energy_F_Reuslt.TimePeriods.Add(t_Ping);
            energy_F_Reuslt.TimePeriods.Add(t_Gu);

            return this.Success(energy_F_Reuslt);
        }
        #endregion
    }
}
