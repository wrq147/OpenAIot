using AuthService;
using AuthService.Controller;
using Common;
using Common.Share;
using StorageService.Business;
using StorageService.Model;
using IoTService.Models;
using ProducerService.Business;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using ProducerService.Model;

namespace StorageService.Controller
{
    /// <summary>
    /// 库存管理API
    /// </summary>
    public class Stock : AbstractLoginedController
    {
        private StockBLL _stockBLL;
        public Stock(StockBLL stockBLL)
        {
            _stockBLL = stockBLL;
        }
        /// <summary>
        /// 库存列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_StockPie>>> List(In_PileList query)
        {
            return this.Success(await _stockBLL.SelectList(query));
        }
        /// <summary>
        /// 设置库存预警值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/StorageService/House/WarnList")]
        public async Task<DefaultAjaxResult<string>> SetPileWarn(List<In_UpdatePileItem> data)
        {
            return (await _stockBLL.SetPileWarn(data)).ToAjaxResult();
        }
        /// <summary>
        /// 获取库存的记录
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_StockRecord>>> RecordList(In_StockRecordPage query)
        {
            return this.Success(await _stockBLL.SelectStockRecordByPage(query));
        }
        /// <summary>
        /// 导出库存数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/List")]
        [HttpGet]
        public async Task<IResult> Export(In_PileList query)
        {
            try
            {
                query.showAll = true;
                PageObject<Out_StockPie> page = await _stockBLL.SelectList(query);
                List<Out_StockPie> list = page.List;
                Dictionary<string, ParamRenderToExcel<Out_StockPie>> FiedNames = new Dictionary<string, ParamRenderToExcel<Out_StockPie>>();
                FiedNames.Add("DeviceNumber", new ParamRenderToExcel<Out_StockPie>("唯一编号"));
                FiedNames.Add("Name", new ParamRenderToExcel<Out_StockPie>("物品名称"));
                FiedNames.Add("TargetType", new ParamRenderToExcel<Out_StockPie>("存储类型", x => x.TargetType == 0 ? "半成品" : "成品"));
                FiedNames.Add("StoreName", new ParamRenderToExcel<Out_StockPie>("所在仓库"));
                FiedNames.Add("PhotoUrl", new ParamRenderToExcel<Out_StockPie>("预览图片"));
                FiedNames.Add("Quantity", new ParamRenderToExcel<Out_StockPie>("存储数量"));
                FiedNames.Add("LockQuantity", new ParamRenderToExcel<Out_StockPie>("锁定数量"));
                FiedNames.Add("Unit", new ParamRenderToExcel<Out_StockPie>("单位"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<Out_StockPie>("库存", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }


        /// <summary>
        /// 出库列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/Leave")]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_LeaveStock>>> LeaveList(In_LeaveList query)
        {
            return this.Success(await _stockBLL.SelectLeaveList(query));
        }
        /// <summary>
        /// 入库列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/Enter")]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_EnterStock>>> EnterList(In_EnterList query)
        {
            return this.Success(await _stockBLL.SelectEnterList(query));
        }



        /// <summary>
        /// 入库单导出
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/List")]
        [HttpGet]
        public async Task<IResult> ExportEnter(In_EnterList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_EnterStock> page = await _stockBLL.SelectEnterList(query);
                List<MZ_EnterStock> list = page.List;
                Dictionary<string, ParamRenderToExcel<MZ_EnterStock>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_EnterStock>>();
                FiedNames.Add("StockNumber", new ParamRenderToExcel<MZ_EnterStock>("入库单号"));
                FiedNames.Add("FromName", new ParamRenderToExcel<MZ_EnterStock>("来源企业"));
                FiedNames.Add("FromHouseName", new ParamRenderToExcel<MZ_EnterStock>("所出仓库"));
                FiedNames.Add("ToHouseName", new ParamRenderToExcel<MZ_EnterStock>("所入仓库"));
                FiedNames.Add("EnterMethod", new ParamRenderToExcel<MZ_EnterStock>("入库方式", way =>
                {
                    switch (way.EnterMethod)
                    {
                        case 0:
                            return "出库";
                        case 1:
                            return "退货";
                        case 2:
                            return "调拨";
                        case 3:
                            return "手动";
                    }
                    return "";
                }));
                FiedNames.Add("Status", new ParamRenderToExcel<MZ_EnterStock>("单据状态", way =>
                {
                    switch (way.Status)
                    {
                        case 0:
                            return "待提交";
                        case 1:
                            return "待审批";
                        case 2:
                            return "入库成功";
                        case 3:
                            return "入库失败";
                    }
                    return "";
                }));
                FiedNames.Add("Creater", new ParamRenderToExcel<MZ_EnterStock>("制单人"));
                FiedNames.Add("InDate", new ParamRenderToExcel<MZ_EnterStock>("入库时间", x => x.InDate == null ? "" : x.InDate.Value.ToString("yyyy-MM-dd HH:mm:ss")));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_EnterStock>("入库单", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
        /// <summary>
        /// 通过工单编号获取出库单详情
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_LeaveStock>> LeaveInfoByNumber(string number)
        {
            return (await _stockBLL.LeaveInfoByNumber(number)).ToAjaxResult();
        }
        /// <summary>
        /// 获取出库单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Stock/Leave")]
        public async Task<DefaultAjaxResult<MZ_LeaveStock>> LeaveInfo(string id)
        {
            return (await _stockBLL.LeaveInfo(id)).ToAjaxResult();
        }
        /// <summary>
        /// 通过工单编号获取入库单详情
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_EnterStock>> EnterInfoByNumber(string number)
        {
            return (await _stockBLL.EnterInfoByNumber(number)).ToAjaxResult();
        }

        /// <summary>
        /// 生成入库单的初始化表单数据
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Dictionary<string, object>>> EnterFormData(MZ_EnterStock stock)
        {
            return (await _stockBLL.CreateEnterTaskForm(stock)).ToAjaxResult();
        }
        /// <summary>
        /// 生成出库单的初始化表单数据
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Dictionary<string, object>>> LeaveFormData(MZ_LeaveStock stock)
        {
            return (await _stockBLL.CreateLeaveTaskForm(stock)).ToAjaxResult();
        }

        /// <summary>
        /// 获取入库单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Stock/Enter")]
        public async Task<DefaultAjaxResult<MZ_EnterStock>> EnterInfo(string id)
        {
            return (await _stockBLL.EnterInfo(id)).ToAjaxResult();
        }
        /// <summary>
        /// 查询入库单的关联的出库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Stock/Leave")]
        public async Task<DefaultAjaxResult<MZ_LeaveStock>> LeaveBySource(string id)
        {
            return (await _stockBLL.SelectStockBySource(id)).ToAjaxResult();
        }
        /// <summary>
        /// 生成出库单编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Stock/Leave")]
        public async Task<DefaultAjaxResult<string>> GenerateCKNumber()
        {
            return this.Success(await _stockBLL.GenerateCKNumber());
        }
        /// <summary>
        /// 生成入库单编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Stock/Enter")]
        public async Task<DefaultAjaxResult<string>> GenerateRKNumber()
        {
            return this.Success(await _stockBLL.GenerateRKNumber());
        }
        /// <summary>
        /// 新增出库单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/Leave")]
        [HttpPost]
        [Des("新增一条出库单")]
        public async Task<DefaultAjaxResult<string>> AddLeave(MZ_LeaveStock data)
        {
            return (await _stockBLL.AddLeave(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改出库单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/Leave")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> EditLeave(MZ_LeaveStock data)
        {
            return (await _stockBLL.EditLeave(data)).ToAjaxResult();
        }

        /// <summary>
        /// 出库提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowId"></param>
        /// <returns>返回值为1表示时表示需要提交审核单，返回2时表示待出库，需要调用出库接口</returns>
        [About("/StorageService/Stock/Leave")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Submit(string id, long flowId)
        {
            return (await _stockBLL.Submit(GetUser(), id, flowId)).ToAjaxResult();
        }
        /// <summary>
        /// 出库提交（同时提交审核表单）
        /// </summary>
        /// <param name="data"></param>
        /// <returns>返回值为1表示时表示需要提交审核单，返回2时表示待出库，需要调用出库接口</returns>
        [About("/StorageService/Stock/Leave")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> SubmitModel(In_SubmitStock data)
        {
            return (await _stockBLL.SubmitModel(GetUser(), data)).ToAjaxResult();
        }
        /// <summary>
        /// 扫码入库（自动上级出库）
        /// </summary>
        /// <param name="ipt"></param>
        /// <returns>返回入库单号</returns>
        [HttpPost]
        [About("/StorageService/Stock/Enter")]
        [Des("设备或耗材入库")]
        public async Task<DefaultAjaxResult<string>> ScanEnterPile(In_ManualStock ipt)
        {
            return (await _stockBLL.ScanEnterPile(ipt, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 保存入库单
        /// </summary>
        /// <param name="ipt"></param>
        /// <returns>返回当前入库单状态rns>
        [HttpPost]
        [About("/StorageService/Stock/Enter")]
        [Des("设备或耗材入库")]
        public async Task<DefaultAjaxResult<string>> ManualPile(In_ManualStock ipt)
        {
            return (await _stockBLL.ManualPile(ipt, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 提交入库单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flowId"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/StorageService/Stock/Enter")]
        public async Task<DefaultAjaxResult<int>> SubmitManualPile(string id, long flowId)
        {
            return (await _stockBLL.SubmitManualPile(GetUser(), id, flowId)).ToAjaxResult();
        }

        /// <summary>
        /// 提交入库单（同时提交审核表单）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/StorageService/Stock/Enter")]
        public async Task<DefaultAjaxResult<int>> SubmitManualPileModel(In_SubmitManualPile data)
        {
            return (await _stockBLL.SubmitManualPileModel(GetUser(), data)).ToAjaxResult();
        }
        /// <summary>
        /// 撤销入库单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/Enter")]
        [HttpPost]
        [Des("撤销一条入库单")]
        public async Task<DefaultAjaxResult<int>> CancelEnter(In_StockCancel data)
        {
            return (await _stockBLL.CancelEnter(data)).ToAjaxResult();
        }
        /// <summary>
        /// 撤销入库单（同时提交审核表单）
        /// </summary>
        /// <returns></returns>
        [About("/StorageService/Stock/Enter")]
        [HttpPost]
        [Des("撤销一条入库单")]
        public async Task<DefaultAjaxResult<int>> CancelEnterModel(In_StockCancelModel data)
        {
            return (await _stockBLL.CancelEnterModel(data)).ToAjaxResult();
        }
        /// <summary>
        /// 撤销出库单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/StorageService/Stock/Leave")]
        [HttpPost]
        [Des("撤销一条出库单")]
        public async Task<DefaultAjaxResult<int>> CancelLeave(string id)
        {
            return (await _stockBLL.CancelLeave(id)).ToAjaxResult();
        }
        /// <summary>
        /// 可入库批次列表查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<V_ProductBatch>>> EnterDevList(In_EnterDevList query)
        {
            return this.Success(await _stockBLL.SelectDevList(query));
        }

        /// <summary>
        /// 入库扫码用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_Item>> InStockByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return this.Error<Out_Item>(13, "编码不能为空");
            }
            var batchBLL = this.ServiceProvider.GetService<ProductBatchBLL>();
            var user = GetUser();
            var batchInfo = await batchBLL.SelectVByNumber(user.OrgId, key);
            if (batchInfo == null)
            {
                return this.Error<Out_Item>(12, "编码不存在");
            }
            else
            {
                Out_Item item = new Out_Item();
                item.TargetId = batchInfo.Id;
                item.DeviceNumber = batchInfo.Number;
                item.Name = batchInfo.BatchName;
                item.Price = batchInfo.Price;
                item.Quantity = 1;
                item.TargetType = 0;
                item.PhotoUrl = batchInfo.PhotoUrl;
                item.Unit = batchInfo.Unit;
                return this.Success(item);
            }

        }
        /// <summary>
        /// 出库扫码用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_Item>> OutStockByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return this.Error<Out_Item>(13, "编码不能为空");
            }
            return this.Success(await _stockBLL.SelectItemByKey(key, GetUser()));
        }
    }
}
