using AfterService.Business;
using AuthService.Controller;
using Common;
using Common.Share;
using EfficiencyService.Business;
using EfficiencyService.Model;
using EfficiencyService.Model.Common;
using IoTService.Models;
using Minio.DataModel;
using NPOI.SS.Formula.Functions;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace EfficiencyService.Controller
{
    public class Production: AbstractLoginedController
    {
        private ProductionBLL _productionBLL;
        private CommonBLL _commonBLL;

        public Production(ProductionBLL productionBLL, CommonBLL commonBLL)
        {
            _productionBLL = productionBLL;
            _commonBLL = commonBLL;
        }

        #region 生产数据采集
        /// <summary>
        /// 新增生产数据采集
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddProduction(In_Production data)
        {

            return (await _productionBLL.AddProduction(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除生产数据采集
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveProduction(string Id)
        {

            return (await _productionBLL.DeleteProduction(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新生产数据采集
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateProduction(In_ProductionUpdate data)
        {

            return (await _productionBLL.UpdateProduction(data)).ToAjaxResult();
        }

        /// <summary>
        /// 生产数据采集列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_Production>>> ProductionPageList(InProductionPageList query)
        {
            return this.Success(await _productionBLL.SelectProductionList(query));
        }

        /// <summary>
        /// 设施生产数据采集统计列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_ProductionDay>>> ProductionPageDateList(InProductionDayPageList query)
        {
            return this.Success(await _productionBLL.SelectProductionDateList(query));
        }

        /// <summary>
        /// 生产数据采集信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Production>> ProductionInfo(string Id)
        {
            return this.Success(await _productionBLL.SelectProduction(Id));
        }

        /// <summary>
        /// 导出生产数据采集表模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResult ExportProduction()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<In_Production>> FiedNames = new Dictionary<string, ParamRenderToExcel<In_Production>>();
                FiedNames.Add("FacilityId", new ParamRenderToExcel<In_Production>("设施名称"));
                FiedNames.Add("ProductId", new ParamRenderToExcel<In_Production>("产品名称"));
                FiedNames.Add("DDate", new ParamRenderToExcel<In_Production>("统计日期"));
                FiedNames.Add("OutPut", new ParamRenderToExcel<In_Production>("产量"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<In_Production>("生产数据采集表", new List<In_Production>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导入生产数据采集表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ImportProduction()
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("设施名称", new ParamImportToList("FacilityId"));
            FiedNames.Add("产品名称", new ParamImportToList("ProductId"));
            FiedNames.Add("统计日期", new ParamImportToList("DDate"));
            FiedNames.Add("产量", new ParamImportToList("OutPut", val => Convert.ToDouble(val)));
            List<In_Production> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<In_Production>(st, FiedNames);

            return (await _productionBLL.ImportProduction(list)).ToAjaxResult();
        }
        #endregion

        #region 设备能耗数据采集
        /// <summary>
        /// 新增手抄设备能耗数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddEnergyHand(In_EnergyHand data)
        {

            return (await _productionBLL.AddEnergyHand(data)).ToAjaxResult();
        }

        /// <summary>
        /// 导出设备能耗数据采集表模板
        /// </summary>
        /// <returns></returns>
        //[About("/EfficiencyService/Production/ExportEnergy")]
        [HttpGet]
        public IResult ExportEnergy()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<In_EnergyHand>> FiedNames = new Dictionary<string, ParamRenderToExcel<In_EnergyHand>>();
                FiedNames.Add("EquipmentId", new ParamRenderToExcel<In_EnergyHand>("设备名称"));
                FiedNames.Add("DDate", new ParamRenderToExcel<In_EnergyHand>("统计日期"));
                FiedNames.Add("InitVale", new ParamRenderToExcel<In_EnergyHand>("表码期初值"));
                FiedNames.Add("EndVale", new ParamRenderToExcel<In_EnergyHand>("表码期末值"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<In_EnergyHand>("设备能耗数据采集表", new List<In_EnergyHand>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导入设备能耗数据采集表
        /// </summary>
        /// <returns></returns>
        //[About("/EfficiencyService/Production/ImoprtEnergy")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ImportEnergy()
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("设备名称", new ParamImportToList("EquipmentId"));
            FiedNames.Add("统计时间", new ParamImportToList("DDate"));
            FiedNames.Add("表码期初值", new ParamImportToList("InitVale", val => Convert.ToDouble(val)));
            FiedNames.Add("表码期末值", new ParamImportToList("EndVale", val => Convert.ToDouble(val)));
            List<In_EnergyHand> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<In_EnergyHand>(st, FiedNames);

            return (await _productionBLL.ImportEnergy(list)).ToAjaxResult();
        }

        /// <summary>
        /// 删除设备能耗数据采集
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveEnergy(string Id)
        {

            return (await _productionBLL.DeleteEnergy(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新设备能耗数据采集
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateEnergy(In_EnergyHandUpdate data)
        {
            return (await _productionBLL.UpdateEnergy(data)).ToAjaxResult();
        }

        /// <summary>
        /// 设备能耗数据采集列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_Energy>>> EnergyPageList(InEnergyPageList query)
        {
            return this.Success(await _productionBLL.SelectEnergyList(query));
        }

        /// <summary>
        /// 设备能耗数据采集列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_EnergyDay>>> EnergyPageDateList(InEnergyDayPageList query)
        {
            return this.Success(await _productionBLL.SelectEnergyDateList(query));
        }

        /// <summary>
        /// 导出设备能耗数据采集列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResult> ExportEnergy(InEnergyDayPageList query)
        {
            try
            {
                List<In_EnergyHand> list = new List<In_EnergyHand>();//(await _productionBLL.SelectEnergyDateList(query)).List;

                Dictionary<string, ParamRenderToExcel<In_EnergyHand>> FiedNames = new Dictionary<string, ParamRenderToExcel<In_EnergyHand>>();
                FiedNames.Add("EquipmentId", new ParamRenderToExcel<In_EnergyHand>("设备名称"));
                FiedNames.Add("DDate", new ParamRenderToExcel<In_EnergyHand>("统计时间"));
                FiedNames.Add("InitVale", new ParamRenderToExcel<In_EnergyHand>("表码期初值"));
                FiedNames.Add("EndVale", new ParamRenderToExcel<In_EnergyHand>("表码期末值"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<In_EnergyHand>("设备能耗数据采集列表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 能耗数据采集列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_FacilityDay>>> FacilityEnergyPageDateList(InEnergyDayPageList query)
        {
            return this.Success(await _productionBLL.SelectFacilityDateList(query));
        }

        /// <summary>
        /// 能效对标
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_Bench>>> BenchEnergyDate(BenchEnergyDate query)
        {
            List<Out_Bench> list = new List<Out_Bench>();

            if (!(query.DateType == "日" || query.DateType == "月" || query.DateType == "年"))
            {
                return this.Error<List<Out_Bench>>(500, "日期类型错误");
            }

            if (query.BenchType == "单元对标" || query.BenchType == "设备对标")
            {
                string[] facilityIds = query.Ids.Split(',');
                foreach (var item in facilityIds)
                {
                    Out_Bench bench = new Out_Bench();
                    bench.Details = new List<Out_BenchDetail>();
                    //能耗数据
                    InEnergyDayPageList EnergyQuery = new InEnergyDayPageList();
                    EnergyQuery.OrgId = query.OrgId;
                    EnergyQuery.FactorId = query.FactorId;
                    if (query.DateType == "年")
                    {
                        EnergyQuery.DateType = "月";
                    }
                    else if (query.DateType == "月")
                    {
                        EnergyQuery.DateType = "日";
                    }
                    else if (query.DateType == "日")
                    {
                        EnergyQuery.DateType = "日";
                    }
                    EnergyQuery.BeginDate = query.BeginDate;
                    EnergyQuery.EndDate = query.EndDate;
                    if (query.BenchType == "单元对标")
                    {
                        EnergyQuery.FacilityId = item;
                        T_Com_Facility facility = await _commonBLL.SelectFacility(item);
                        if (facility == null)
                        {
                            return this.Error<List<Out_Bench>>(500, "单元不存在");
                        }
                        bench.FacilityName = facility.FacilityName;
                    }
                    else if (query.BenchType == "设备对标")
                    {
                        EnergyQuery.EquipmentId = item;
                        Out_Equipment equipment = await _commonBLL.SelectEquipmentInfo(item);
                        if (equipment == null)
                        {
                            return this.Error<List<Out_Bench>>(500, "设备不存在");
                        }
                        bench.FacilityName = equipment.EquipmentName;
                    }

                    List<Out_FacilityDay> days = (await _productionBLL.SelectFacilityDateList(EnergyQuery)).List;

                    foreach (var jtem in days)
                    {
                        if (!string.IsNullOrEmpty(bench.FactorName))
                        {
                            if (bench.FactorName != jtem.FactorName)
                            {
                                return this.Error<List<Out_Bench>>(500, "存在多个能源，请选择能源");
                            }
                        }
                        bench.FactorName = jtem.FactorName;
                        bench.Unit = jtem.Unit;
                        bench.LageUnit = jtem.LageUnit;
                        bench.UseVale = Math.Round(bench.UseVale + jtem.UseVale, 2);
                        bench.CostVale = Math.Round(bench.CostVale + jtem.CostVale, 2);
                        bench.CarbonEmission = Math.Round(bench.CarbonEmission + jtem.CarbonEmission, 2);
                        bench.ConvertCoal = Math.Round(bench.ConvertCoal + jtem.ConvertCoal, 2);
                    }

                    //产品数据
                    InProductionDayPageList productionQuery = new InProductionDayPageList();
                    productionQuery.OrgId = query.OrgId;
                    if (query.DateType == "年")
                    {
                        productionQuery.DateType = "月";
                    }
                    else if (query.DateType == "月")
                    {
                        productionQuery.DateType = "日";
                    }
                    else if (query.DateType == "日")
                    {
                        productionQuery.DateType = "日";
                    }
                    productionQuery.BeginDate = query.BeginDate;
                    productionQuery.EndDate = query.EndDate;
                    productionQuery.ProductId = query.ProductId;

                    List<Out_ProductionDay> productions;
                    if (string.IsNullOrEmpty(query.ProductId))
                    {
                        productions = new List<Out_ProductionDay>();
                    }
                    else
                    {
                        productions = (await _productionBLL.SelectProductionDateList(productionQuery)).List;
                    }
                    foreach (var jtem in productions)
                    {
                        if (!string.IsNullOrEmpty(bench.ProductId))
                        {
                            if (bench.ProductId != jtem.ProductId)
                            {
                                return this.Error<List<Out_Bench>>(500, "存在多个产品，请选择产品");
                            }
                        }
                        bench.ProductId = jtem.ProductId;
                        bench.ProductName = jtem.ProductName;
                        bench.ProductUnit = jtem.Unit;
                        bench.OutPut = Math.Round(bench.OutPut + jtem.OutPut, 2);
                        bench.OutValue = Math.Round(bench.OutValue + jtem.OutValue, 2);
                    }
                    if (bench.OutPut > 0)
                    {
                        bench.UseEfficiency = Math.Round(bench.UseVale / bench.OutPut, 2);
                        bench.CostEfficiency = Math.Round(bench.CostVale / bench.OutPut, 2);
                    }
                    DateTime time = DateTime.Parse(query.BeginDate);
                    if (query.DateType == "年")
                    {
                        DateTime startDate = new DateTime(time.Year, 1, 1);
                        DateTime endDate = startDate.AddMonths(12);

                        for (DateTime date = startDate; date <= endDate; date = date.AddMonths(1))
                        {
                            Out_BenchDetail benchDetail = new Out_BenchDetail();
                            benchDetail.FacilityName = bench.FacilityName;
                            benchDetail.FactorName = bench.FactorName;
                            benchDetail.Unit = bench.Unit;
                            benchDetail.LageUnit = bench.LageUnit;
                            benchDetail.DDate = date.ToString("yyyy-MM");
                            benchDetail.ProductId = bench.ProductId;
                            benchDetail.ProductName = bench.ProductName;
                            benchDetail.ProductUnit = bench.ProductUnit;
                            foreach (var jtem in days)
                            {
                                if (jtem.DDate == benchDetail.DDate)
                                {
                                    benchDetail.UseVale = Math.Round(benchDetail.UseVale + jtem.UseVale, 2);
                                    benchDetail.CostVale = Math.Round(benchDetail.CostVale + jtem.CostVale, 2);
                                    benchDetail.CarbonEmission = Math.Round(benchDetail.CarbonEmission + jtem.CarbonEmission, 2);
                                    benchDetail.ConvertCoal = Math.Round(benchDetail.ConvertCoal + jtem.ConvertCoal, 2);
                                }
                            }
                            foreach (var jtem in productions)
                            {
                                if (jtem.DDate == benchDetail.DDate)
                                {
                                    benchDetail.OutPut = Math.Round(benchDetail.OutPut + jtem.OutPut, 2);
                                    benchDetail.OutValue = Math.Round(benchDetail.OutValue + jtem.OutValue, 2);
                                }
                            }
                            if (benchDetail.OutPut > 0)
                            {
                                benchDetail.UseEfficiency = Math.Round(benchDetail.UseVale / benchDetail.OutPut, 2);
                                benchDetail.CostEfficiency = Math.Round(benchDetail.CostVale / benchDetail.OutPut, 2);
                            }
                            bench.Details.Add(benchDetail);
                        }
                    }
                    else if (query.DateType == "月")
                    {
                        DateTime startDate = new DateTime(time.Year, time.Month, 1);
                        DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            Out_BenchDetail benchDetail = new Out_BenchDetail();
                            benchDetail.FactorName = bench.FactorName;
                            benchDetail.Unit = bench.Unit;
                            benchDetail.LageUnit = bench.LageUnit;
                            benchDetail.DDate = date.ToString("yyyy-MM-dd");
                            benchDetail.ProductId = bench.ProductId;
                            benchDetail.ProductName = bench.ProductName;
                            benchDetail.ProductUnit = bench.ProductUnit;
                            foreach (var jtem in days)
                            {
                                if (jtem.DDate == benchDetail.DDate)
                                {
                                    benchDetail.UseVale = Math.Round(benchDetail.UseVale + jtem.UseVale, 2);
                                    benchDetail.CostVale = Math.Round(benchDetail.CostVale + jtem.CostVale, 2);
                                    benchDetail.CarbonEmission = Math.Round(benchDetail.CarbonEmission + jtem.CarbonEmission, 2);
                                    benchDetail.ConvertCoal = Math.Round(benchDetail.ConvertCoal + jtem.ConvertCoal, 2);
                                }
                            }
                            foreach (var jtem in productions)
                            {
                                if (jtem.DDate == benchDetail.DDate)
                                {
                                    benchDetail.OutPut = Math.Round(benchDetail.OutPut + jtem.OutPut, 2);
                                    benchDetail.OutValue = Math.Round(benchDetail.OutValue + jtem.OutValue, 2);
                                }
                            }
                            if (benchDetail.OutPut > 0)
                            {
                                benchDetail.UseEfficiency = Math.Round(benchDetail.UseVale / benchDetail.OutPut, 2);
                                benchDetail.CostEfficiency = Math.Round(benchDetail.CostVale / benchDetail.OutPut, 2);
                            }
                            bench.Details.Add(benchDetail);
                        }
                    }
                    else if (query.DateType == "日")
                    {

                    }

                    list.Add(bench);
                }
            }
            else
            {
                return this.Error<List<Out_Bench>>(500, "对标类型错误");
            }
           

            return this.Success(list);
        }

        /// <summary>
        /// 导出能耗数据采集列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResult> ExportFacilityEnergy(InEnergyDayPageList query)
        {
            try
            {
                List<Out_FacilityDay> list = (await _productionBLL.SelectFacilityDateList(query)).List;
               
                Dictionary<string, ParamRenderToExcel<Out_FacilityDay>> FiedNames = new Dictionary<string, ParamRenderToExcel<Out_FacilityDay>>();
                FiedNames.Add("FacilityName", new ParamRenderToExcel<Out_FacilityDay>("单元名称"));
                FiedNames.Add("FactorName", new ParamRenderToExcel<Out_FacilityDay>("能源类型"));
                FiedNames.Add("DDate", new ParamRenderToExcel<Out_FacilityDay>("统计时间"));
                FiedNames.Add("UseVale", new ParamRenderToExcel<Out_FacilityDay>("能源消耗量"));
                FiedNames.Add("Unit", new ParamRenderToExcel<Out_FacilityDay>("单位"));
                FiedNames.Add("CostVale", new ParamRenderToExcel<Out_FacilityDay>("成本"));
                FiedNames.Add("Memo", new ParamRenderToExcel<Out_FacilityDay>("数据来源"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<Out_FacilityDay>("能耗数据采集列表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }


        /// <summary>
        /// 设备能耗数据采集信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Energy>> EnergyInfo(string Id)
        {
            return this.Success(await _productionBLL.SelectEnergy(Id));
        }

        /// <summary>
        /// 企业能流图
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_FacilityEnergy>>> EnergyTree(InEnergyTree query)
        {
            return this.Success(await _productionBLL.SeletEnergyTree(query));
        }

        /// <summary>
        /// 测试每日生成计费数据
        /// </summary>
        /// <param name="Year">年</param>
        /// <param name="Month">月</param>
        /// <param name="Day">日</param>
        /// <param name="Execute">true：真实数据  false:模拟数据</param>
        /// <param name="equipmentId">设备ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> TestEnergyDay(int Year, int Month, int Day, bool Execute, string equipmentId)
        {
            return (await _productionBLL.TestEnergyDay(Year, Month, Day, Execute, equipmentId)).ToAjaxResult();
        }
        /// <summary>
        /// 生成电表历史数据
        /// </summary>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="orgId">企业编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ReCalculationEnergyDay(InCalEnergy inCalEnergy)
        {
            return (await _productionBLL.ReCalculationEnergyDay(inCalEnergy.beginDate, inCalEnergy.endDate, inCalEnergy.orgId)).ToAjaxResult();
        }
        /// <summary>
        /// 生成电表小时历史数据
        /// </summary>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="orgId">企业编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ReCalculationEnergyDayHour(InCalEnergy inCalEnergy)
        {
            return (await _productionBLL.ReCalculationEnergyDayHour(inCalEnergy.beginDate, inCalEnergy.endDate, inCalEnergy.orgId)).ToAjaxResult();
        }
        
        #endregion

        #region 碳排分析
        /// <summary>
        /// 按排放类型能碳分析报表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_CarbonReportType>>> CarbonReportByType(In_CarbonReport query)
        {
            return this.Success(await _productionBLL.CarbonReportByType(query));
        }
        /// <summary>
        /// 按排放范围能碳分析报表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_CarbonReportType>>> CarbonReportByRange(In_CarbonReport query)
        {
            return this.Success(await _productionBLL.CarbonReportByRange(query));
        }
        #endregion

        #region 首页设备分布
        /// <summary>
        /// 设备数量统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_EquipmentStatistics>> StatisticsInfo(InCalEnergy inCalEnergy)
        {
            var res = await _productionBLL.StatisticsInfo(inCalEnergy);
            return this.Success(res);
        }

        /// <summary>
        /// 报警列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<MZ_IotWarning>>))]
        public async Task<AjaxResult> WarningListPage(In_WarningListPage query)
        {
            return this.Success(await _productionBLL.WarningListPage(query, GetUser()));
        }
        #endregion

        #region 供应商
        /// <summary>
        /// 供应商代码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ProviderCode()
        {
            return (await _productionBLL.GenerateNumber("PV")).ToAjaxResult();
        }
        /// <summary>
        /// 新增供应商
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddProvider(In_Prod_Provider data)
        {

            return (await _productionBLL.AddProvider(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveProvider(string Id)
        {

            return (await _productionBLL.DeleteProvider(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新供应商
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateProvider(In_Prod_Provider data)
        {
            return (await _productionBLL.UpdateProvider(data)).ToAjaxResult();
        }

        /// <summary>
        /// 分页查询供应商列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<T_Prod_Provider2>>> ProviderPageList(In_Provider query)
        {
            return this.Success(await _productionBLL.SelectProviderList(query));
        }

        /// <summary>
        /// 查询供应商
        /// </summary>
        /// <param name="Id">供应商编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<T_Prod_Provider>> Provider(string Id)
        {
            return this.Success(await _productionBLL.SelectProvider(Id));
        }

        /// <summary>
        /// 导出供应商模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResult ExportProvider()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<In_Prod_Provider>> FiedNames = new Dictionary<string, ParamRenderToExcel<In_Prod_Provider>>();
                FiedNames.Add("ProviderCode", new ParamRenderToExcel<In_Prod_Provider>("供应商代码"));
                FiedNames.Add("ProviderName", new ParamRenderToExcel<In_Prod_Provider>("供应商名称"));
                FiedNames.Add("Manager", new ParamRenderToExcel<In_Prod_Provider>("联系人"));
                FiedNames.Add("Contact", new ParamRenderToExcel<In_Prod_Provider>("联系电话"));
                FiedNames.Add("Area", new ParamRenderToExcel<In_Prod_Provider>("所属区域"));
                FiedNames.Add("ProviderAddress", new ParamRenderToExcel<In_Prod_Provider>("供应商地址"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<In_Prod_Provider>("供应商", new List<In_Prod_Provider>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导出供应商列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResult> ExportProviderList(In_Provider query)
        {
            try
            {
                List<T_Prod_Provider2> list = (await _productionBLL.SelectProviderList(query)).List;
                List<In_Prod_Provider1> Providers = new List<In_Prod_Provider1>();
                foreach (var item in list)
                {
                    In_Prod_Provider1 provider1 = new In_Prod_Provider1();
                    provider1.ProviderCode = item.ProviderCode;
                    provider1.ProviderName = item.ProviderName;
                    provider1.Manager = item.Manager;
                    provider1.Contact = item.Contact;
                    if (!string.IsNullOrEmpty(item.Province))
                    {
                        provider1.Area = item.Province;
                        if (!string.IsNullOrEmpty(item.City))
                        {
                            provider1.Area = item.Province + "," + item.City;
                            if (!string.IsNullOrEmpty(item.District))
                            {
                                provider1.Area = item.Province + "," + item.City + "," + item.District;
                            }
                        }
                    }
                    provider1.ProviderAddress = item.ProviderAddress;
                    Providers.Add(provider1);
                }
                Dictionary<string, ParamRenderToExcel<In_Prod_Provider1>> FiedNames = new Dictionary<string, ParamRenderToExcel<In_Prod_Provider1>>();
                FiedNames.Add("ProviderCode", new ParamRenderToExcel<In_Prod_Provider1>("供应商代码"));
                FiedNames.Add("ProviderName", new ParamRenderToExcel<In_Prod_Provider1>("供应商名称"));
                FiedNames.Add("Manager", new ParamRenderToExcel<In_Prod_Provider1>("联系人"));
                FiedNames.Add("Contact", new ParamRenderToExcel<In_Prod_Provider1>("联系电话"));
                FiedNames.Add("Area", new ParamRenderToExcel<In_Prod_Provider1>("所属区域"));
                FiedNames.Add("ProviderAddress", new ParamRenderToExcel<In_Prod_Provider1>("供应商地址"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<In_Prod_Provider1>("供应商", Providers, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导入供应商
        /// </summary>
        /// <returns></returns>
        //[About("/EfficiencyService/Production/ImoprtProduction")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ImportProvider()
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("供应商代码", new ParamImportToList("ProviderCode"));
            FiedNames.Add("供应商名称", new ParamImportToList("ProviderName"));
            FiedNames.Add("联系人", new ParamImportToList("Manager"));
            FiedNames.Add("联系电话", new ParamImportToList("Contact"));
            FiedNames.Add("所属区域", new ParamImportToList("Area"));
            FiedNames.Add("供应商地址", new ParamImportToList("ProviderAddress"));
            List<In_Prod_Provider> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<In_Prod_Provider>(st, FiedNames);

            return (await _productionBLL.ImportProvider(list)).ToAjaxResult();
        }
        #endregion

        #region 物料
        /// <summary>
        /// 查询物料的供应商
        /// </summary>
        /// <param name="MaterialId">物料编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_Prod_Provider>>> MaterialProvider(string MaterialId)
        {
            return this.Success(await _productionBLL.SelectMaterialProvider(MaterialId));
        }
        /// <summary>
        /// 物料代码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> MaterialCode()
        {
            return (await _productionBLL.GenerateNumber("MT")).ToAjaxResult();
        }

        /// <summary>
        /// 新增物料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddMaterial(In_Prod_Material data)
        {

            return (await _productionBLL.AddMaterial(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除物料
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveMaterial(string Id)
        {

            return (await _productionBLL.DeleteMaterial(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 更新物料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateMaterial(In_Prod_Material data)
        {
            return (await _productionBLL.UpdateMaterial(data)).ToAjaxResult();
        }
        
        /// <summary>
        /// 分页查询物料列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_Material>>> MaterialPageList(In_Material query)
        {
            return this.Success(await _productionBLL.SelectMaterialList(query));
        }

        /// <summary>
        /// 查询物料
        /// </summary>
        /// <param name="Id">物料编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_Material>> Material(string Id)
        {
            return this.Success(await _productionBLL.SelectMaterial(Id));
        }

        /// <summary>
        /// 导出物料模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResult ExportMaterial()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<Inpotr_Material>> FiedNames = new Dictionary<string, ParamRenderToExcel<Inpotr_Material>>();
                FiedNames.Add("MaterialCode", new ParamRenderToExcel<Inpotr_Material>("物料代码"));
                FiedNames.Add("MaterialName", new ParamRenderToExcel<Inpotr_Material>("物料名称"));
                FiedNames.Add("MaterialType", new ParamRenderToExcel<Inpotr_Material>("物料类型"));
                FiedNames.Add("MaterialUnit", new ParamRenderToExcel<Inpotr_Material>("物料单位"));
                FiedNames.Add("ProviderCode", new ParamRenderToExcel<Inpotr_Material>("供应商代码"));
                FiedNames.Add("ProductModel", new ParamRenderToExcel<Inpotr_Material>("供应商规格型号"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<Inpotr_Material>("物料列表", new List<Inpotr_Material>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导出供物料列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResult> ExportMaterialList(In_Material query)
        {
            try
            {
                List<Out_Material> list = (await _productionBLL.SelectMaterialList(query)).List;
                List<Inpotr_Material> Material = new List<Inpotr_Material>();
                foreach (var item in list)
                {
                    List<Out_Prod_Provider> Providers = await _productionBLL.SelectMaterialProvider(item.Id);

                    if (Providers.Count > 0)
                    {
                        foreach (var jtem in Providers)
                        {
                            Inpotr_Material inpotr_Material = new Inpotr_Material();
                            inpotr_Material.MaterialCode = item.MaterialCode;
                            inpotr_Material.MaterialName = item.MaterialName;
                            inpotr_Material.MaterialType = item.MaterialType;
                            inpotr_Material.MaterialUnit = item.MaterialUnit;

                            inpotr_Material.ProviderCode = jtem.ProviderCode;
                            inpotr_Material.ProductModel = jtem.ProductModel;

                            Material.Add(inpotr_Material);
                        }
                    }
                    else
                    {
                        Inpotr_Material inpotr_Material = new Inpotr_Material();
                        inpotr_Material.MaterialCode = item.MaterialCode;
                        inpotr_Material.MaterialName = item.MaterialName;
                        inpotr_Material.MaterialType = item.MaterialType;
                        inpotr_Material.MaterialUnit = item.MaterialUnit;

                        Material.Add(inpotr_Material);
                    }
                }
                Dictionary<string, ParamRenderToExcel<Inpotr_Material>> FiedNames = new Dictionary<string, ParamRenderToExcel<Inpotr_Material>>();

                FiedNames.Add("MaterialCode", new ParamRenderToExcel<Inpotr_Material>("物料代码"));
                FiedNames.Add("MaterialName", new ParamRenderToExcel<Inpotr_Material>("物料名称"));
                FiedNames.Add("MaterialType", new ParamRenderToExcel<Inpotr_Material>("物料类型"));
                FiedNames.Add("MaterialUnit", new ParamRenderToExcel<Inpotr_Material>("物料单位"));
                FiedNames.Add("ProviderCode", new ParamRenderToExcel<Inpotr_Material>("供应商代码"));
                FiedNames.Add("ProductModel", new ParamRenderToExcel<Inpotr_Material>("供应商规格型号"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<Inpotr_Material>("物料列表", Material, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
        /// <summary>
        /// 导入物料
        /// </summary>
        /// <returns></returns>
        //[About("/EfficiencyService/Production/ImoprtProduction")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ImportMaterial()
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("物料代码", new ParamImportToList("MaterialCode"));
            FiedNames.Add("物料名称", new ParamImportToList("MaterialName"));
            FiedNames.Add("物料类型", new ParamImportToList("MaterialType"));
            FiedNames.Add("物料单位", new ParamImportToList("MaterialUnit"));
            FiedNames.Add("供应商代码", new ParamImportToList("ProviderCode"));
            FiedNames.Add("供应商规格型号", new ParamImportToList("ProductModel"));
            List<Inpotr_Material> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<Inpotr_Material>(st, FiedNames);

            return (await _productionBLL.ImportMaterial(list)).ToAjaxResult();
        }
        #endregion

        #region 物料碳足迹
        /// <summary>
        /// 新增物料碳足迹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddMaterialCarbon(In_Prod_MaterialCarbon data)
        {
            return (await _productionBLL.AddMaterialCarbon(data)).ToAjaxResult();
        }

        /// <summary>
        /// 删除物料碳足迹
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveMaterialCarbon(string Id)
        {

            return (await _productionBLL.DeleteMaterialCarbon(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 修改物料碳足迹
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateMaterialCarbon(In_Prod_MaterialCarbon data)
        {
            return (await _productionBLL.UpdateMaterialCarbon(data)).ToAjaxResult();
        }

        /// <summary>
        /// 分页查询物料碳足迹列表
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_MaterialCarbon>>> MaterialCarbonPageList(In_MaterialCarbon query)
        {
            return this.Success(await _productionBLL.SelectMaterialCarbonList(query));
        }

        /// <summary>
        /// 查询物料碳足迹信息
        /// </summary>
        /// <param name="Id">编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<T_Prod_MaterialCarbon>> MaterialCarbon(string Id)
        {
            return this.Success(await _productionBLL.SelectMaterialCarbon(Id));
        }
        /// <summary>
        /// 导出物料碳足迹模板
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResult ExportMaterialCarbon()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<Out_MaterialCarbon>> FiedNames = new Dictionary<string, ParamRenderToExcel<Out_MaterialCarbon>>();
                FiedNames.Add("Id", new ParamRenderToExcel<Out_MaterialCarbon>("碳足迹编码"));
                FiedNames.Add("MaterialCode", new ParamRenderToExcel<Out_MaterialCarbon>("物料代码"));
                FiedNames.Add("ProviderCode", new ParamRenderToExcel<Out_MaterialCarbon>("供应商代码"));
                FiedNames.Add("ProductBorder", new ParamRenderToExcel<Out_MaterialCarbon>("产品生命周期边界"));
                FiedNames.Add("CarbonEmission", new ParamRenderToExcel<Out_MaterialCarbon>("碳排放量"));
                FiedNames.Add("CarbonUnit", new ParamRenderToExcel<Out_MaterialCarbon>("碳排放单位"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<Out_MaterialCarbon>("料碳足迹列表", new List<Out_MaterialCarbon>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        /// <summary>
        /// 导出物料碳足迹列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResult> ExportMaterialCarbonList(In_MaterialCarbon query)
        {
            try
            {
                List<Out_MaterialCarbon> list = (await _productionBLL.SelectMaterialCarbonList(query)).List;
                Dictionary<string, ParamRenderToExcel<Out_MaterialCarbon>> FiedNames = new Dictionary<string, ParamRenderToExcel<Out_MaterialCarbon>>();
                FiedNames.Add("Id", new ParamRenderToExcel<Out_MaterialCarbon>("碳足迹编码"));
                FiedNames.Add("MaterialCode", new ParamRenderToExcel<Out_MaterialCarbon>("物料代码"));
                FiedNames.Add("ProviderCode", new ParamRenderToExcel<Out_MaterialCarbon>("供应商代码"));
                FiedNames.Add("ProductBorder", new ParamRenderToExcel<Out_MaterialCarbon>("产品生命周期边界"));
                FiedNames.Add("CarbonEmission", new ParamRenderToExcel<Out_MaterialCarbon>("碳排放量"));
                FiedNames.Add("CarbonUnit", new ParamRenderToExcel<Out_MaterialCarbon>("碳排放单位"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<Out_MaterialCarbon>("料碳足迹列表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
        /// <summary>
        /// 导入物料碳足迹
        /// </summary>
        /// <returns></returns>
        //[About("/EfficiencyService/Production/ImoprtProduction")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ImportMaterialCarbon()
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("碳足迹编码", new ParamImportToList("Id"));
            FiedNames.Add("物料代码", new ParamImportToList("MaterialCode"));
            FiedNames.Add("供应商代码", new ParamImportToList("ProviderCode"));
            FiedNames.Add("产品生命周期边界", new ParamImportToList("ProductBorder"));
            FiedNames.Add("碳排放量", new ParamImportToList("CarbonEmission"));
            FiedNames.Add("碳排放单位", new ParamImportToList("CarbonUnit"));
            List<Inport_MaterialCarbon> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<Inport_MaterialCarbon>(st, FiedNames);

            return (await _productionBLL.ImportMaterialCarbon(list)).ToAjaxResult();
        }
        #endregion

        #region 碳排计划
        /// <summary>
        /// 查询碳排计划
        /// </summary>
        /// <param name="data">查询参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<In_CarbonPlan>> SelectCarbonPlan(In_Prod_CarbonPlan data)
        {
            return this.Success(await _productionBLL.SelectCarbonPlan(data.Year, data.OrgId));
        }

        /// <summary>
        /// 更新碳排计划
        /// </summary>
        /// <param name="data">查询参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> UpdateCarbonPlan(In_CarbonPlan data)
        {
            return (await _productionBLL.UpdateCarbonPlan(data)).ToAjaxResult();
        }

        /// <summary>
        /// 查询碳排资产
        /// </summary>
        /// <param name="data">查询参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<In_CarbonAsset>> SelectCarbonAsset(In_Prod_CarbonPlan data)
        {
            return this.Success(await _productionBLL.SelectCarbonAsset(data.Year, data.OrgId));
        }

        #endregion
    }
}
