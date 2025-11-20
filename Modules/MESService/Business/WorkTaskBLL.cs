using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class WorkTaskBLL
    {
        private ITAServiceProvider _provider;
        private WorkTaskDAL _workTaskDAL;
        private SnowflakeHelper _snowflakeHelper;
        public WorkTaskBLL(ITAServiceProvider provider, WorkTaskDAL workTaskDAL, SnowflakeHelper snowflakeHelper)
        {
            _provider = provider;
            _workTaskDAL = workTaskDAL;
            _snowflakeHelper = snowflakeHelper;
        }
        public virtual async Task<PageObject<MZ_WorkTask>> SelectList(In_WorkTaskList query, IUserInfo user)
        {
            return await _workTaskDAL.SelectByPage(query, user.OrgId);
        }

        public virtual async Task ResetTaskInfo(string taskId, MZ_WorkReport report)
        {
            var oldTask = await _workTaskDAL.Select(taskId);
            if (oldTask == null)
            {
                return;
            }

            ////计算生产任务
            var reportDAL = _provider.GetService<WorkReportDAL>();
            var reportTotalInfo = await reportDAL.SelectTotal(taskId);
            MZ_WorkTask newTask = new MZ_WorkTask();
            newTask.Id = taskId;
            newTask.GoodNum = reportTotalInfo.TotalGoodNum;
            newTask.DefectNum = reportTotalInfo.TotalDefectNum;
            newTask.WorkTimeTotal = reportTotalInfo.TotalWorkTime;
            if (reportTotalInfo.TotalGoodNum >= oldTask.PlanNum)
            {
                newTask.FinishOn = DateTime.Now;
                newTask.IsFinish = true;
            }
            await _workTaskDAL.Update(newTask);

            //计算生产工单物料
            var proNum = (newTask.GoodNum + newTask.DefectNum) / oldTask.PropOf;
            var workBomDAL = _provider.GetService<WorkBomDAL>();
            await workBomDAL.UpdateUsedQuantity(report.WorkOrderId, report.OperId, proNum.Value);


            var workOrder = await _provider.GetService<WorkOrderDAL>().Select(report.WorkOrderId);
            if (workOrder == null)
            {
                return;
            }
            var route = await _provider.GetService<RouteDAL>().Select(workOrder.RouteId);
            if (route == null)
            {
                return;
            }
            var product = await _provider.GetService<ProductDAL>().Select(workOrder.ProductId);
            if (product == null)
            {
                return;
            }
            var proBatchDAL = _provider.GetService<ProductBatchDAL>();
            var workBatch = await _provider.GetService<WorkBatchDAL>().Select(report.BatchNo);
            MZ_ProductBatch proBatch = null;
            //判断是否为首次绑定通讯编码
            if (workBatch != null && !string.IsNullOrEmpty(workBatch.LNumber))
            {
                if (!await proBatchDAL.Some(x => x.Number == workBatch.Id))
                {
                    proBatch = new MZ_ProductBatch();
                    //生成产品批次
                    proBatch.OrgId = product.OrgId;
                    proBatch.BatchName = product.ProductName + "【" + report.BatchNo + "】";
                    proBatch.PhotoUrl = product.PhotoUrl;
                    proBatch.Number = report.BatchNo;
                    proBatch.ProductId = product.Id;

                    string batchId;
                    if (string.IsNullOrEmpty(product.IOTProductId))
                    {
                        batchId = _snowflakeHelper.NextId().ToString();
                    }
                    else
                    {
                        //关联了物联产品，则生成对应的物联设备
                        var tmprsp = await BusUtility.Call("SaveIotDevice", new
                        {
                            UserId = 2,
                            OrgId = product.OrgId,
                            PhotoUrl = product.PhotoUrl,
                            DeviceNumber = report.BatchNo,
                            ProductId = product.IOTProductId,
                            DeviceId = workBatch.LNumber,
                            Name = proBatch.BatchName
                        });
                        var brs = tmprsp.GetResult<BusResponse<string>>();
                        batchId = brs.Data;
                    }
                    proBatch.Id = batchId;
                    await proBatchDAL.Insert(proBatch);
                }
            }

            //如果是工艺的最后一个工序，则生成产品批次并入库
            var routeOperList = await _provider.GetService<RouteOperDAL>().SelectList(x => x.RouteId == workOrder.RouteId);
            var lastRoute = routeOperList.OrderByDescending(x => x.Sequence).FirstOrDefault();
            if (lastRoute == null)
            {
                return;
            }
            if (lastRoute.OperId != report.OperId)
            {
                return;
            }
            if (proBatch == null)
            {
                var tmpbatchlist = await proBatchDAL.SelectList(x => x.Number == report.BatchNo);
                if (tmpbatchlist.Count > 0)
                {
                    proBatch = tmpbatchlist[0];
                }
                else
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(route.ToHouseId))
            {
                //生成产品入库单
                var tItemList = new List<object>();
                tItemList.Add(new
                {
                    TargetType = product.ProductLabel == "U" ? 0 : 1,
                    TargetId = proBatch.Id,
                    Price = product.Price,
                    Quantity = report.GoodNum
                });
                await BusUtility.Dispatch("ManualPile", new
                {
                    UserId = 2,
                    OrgId = product.OrgId,
                    ToHouseId = route.ToHouseId,
                    List = tItemList
                });
            }

        }
    }
}
