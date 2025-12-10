using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using MyAccess.Aop;
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
        public virtual async Task<BusResponse<MZ_WorkTask>> Info(string id)
        {
            var info = await _workTaskDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_WorkTask>.Error(111, "生产任务不存在");
            }
            var wkorder = await _provider.GetService<WorkOrderDAL>().Select(info.WorkOrderId);
            if (wkorder != null)
            {
                info.WorkNumber = wkorder.WorkNumber;
            }
            var wkoper = await _provider.GetService<OperDAL>().Select(info.OperId);
            if (wkoper != null)
            {
                info.OperName = wkoper.OperName;
                info.AssignedUser = wkoper.AssignedUser;
            }
            var wkpro = await _provider.GetService<ProductDAL>().Select(info.ProductId);
            if (wkpro != null)
            {
                info.SkuNumber = wkpro.SkuNumber;
                info.ProductName = wkpro.ProductName;
            }

            var rtoper = await _provider.GetService<RouteOperDAL>().Select(info.RouteOperId);
            if (rtoper != null)
            {
                info.RouteOper = rtoper;
            }
            return BusResponse<MZ_WorkTask>.Success(info);
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
            if (oldTask.StartOn == null)
            {
                newTask.StartOn = report.StartWork;
            }

            if (reportTotalInfo.TotalGoodNum >= oldTask.PlanNum)
            {
                newTask.FinishOn = report.EndWork;
                newTask.IsFinish = true;
            }
            await _workTaskDAL.Update(newTask);

            //计算生产工单物料
            var proNum = (newTask.GoodNum + newTask.DefectNum) / oldTask.PropOf;
            var workBomDAL = _provider.GetService<WorkBomDAL>();
            await workBomDAL.UpdateUsedQuantity(report.WorkOrderId, report.OperId, proNum.Value);


            var workOrderDAL = _provider.GetService<WorkOrderDAL>();
            var workOrder = await workOrderDAL.Select(report.WorkOrderId);
            if (workOrder == null)
            {
                return;
            }
            if (workOrder.Status == 0)
            {
                //变更工单状态
                MZ_WorkOrder neworder = new MZ_WorkOrder();
                neworder.Id = workOrder.Id;
                neworder.Status = 1;
                neworder.StartOn = report.StartWork;
                await workOrderDAL.Update(neworder);
                await _provider.GetService<ProductPlanDAL>().StartPlane(workOrder.PlanId);
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
            var workBatchDAL = _provider.GetService<WorkBatchDAL>();
            var workBatchList = await workBatchDAL.SelectList(x => x.Id == report.BatchNo && x.OrgId == report.OrgId);
            if (workBatchList.Count == 0)
            {
                return;
            }
            if (workBatchList[0].IsFinish == true)
            {
                return;
            }
            MZ_ProductBatch proBatch = null;
            //判断是否为首次绑定通讯编码
            if (!string.IsNullOrEmpty(workBatchList[0].LNumber))
            {
                var workBatch = workBatchList[0];
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
                        var tmprsp = await BusUtility.Call("FromMesBatch", new
                        {
                            UserId = 2,
                            OrgId = product.OrgId,
                            PhotoUrl = product.PhotoUrl,
                            DeviceNumber = report.BatchNo,
                            ProductId = product.IOTProductId,
                            MesProductId = product.Id,
                            DeviceId = workBatch.LNumber,
                            Name = proBatch.BatchName
                        });
                        if (tmprsp.IsSuccess())
                        {
                            batchId = tmprsp.Result;
                        }
                        else
                        {
                            throw new Exception(tmprsp.Message);
                        }
                    }
                    proBatch.Id = batchId;
                    await proBatchDAL.Insert(proBatch);
                }
            }

            //如果是工艺的最后一个工序，则产品批次入库
            int routeOpCC = await _provider.GetService<RouteOperDAL>().Count(x => x.RouteId == workOrder.RouteId);
            var reportlist = await reportDAL.SelectList(x => x.WorkOrderId == workOrder.Id && x.BatchNo == report.BatchNo && x.Status == 2, "", "RouteOperId");
            int reportedcc = reportlist.Select(x => x.RouteOperId).Distinct().Count();
            if (reportedcc < routeOpCC)
            {
                return;
            }
            MZ_WorkBatch newbatch = new MZ_WorkBatch();
            newbatch.Id = report.BatchNo;
            newbatch.IsFinish = true;
            await workBatchDAL.Update(newbatch);

            if (proBatch == null)
            {
                var tmpbatchlist = await proBatchDAL.SelectList(x => x.Number == report.BatchNo && x.OrgId == report.OrgId);
                if (tmpbatchlist.Count > 0)
                {
                    proBatch = tmpbatchlist[0];
                }
                else
                {
                    proBatch = new MZ_ProductBatch();
                    proBatch.OrgId = product.OrgId;
                    proBatch.BatchName = product.ProductName + "【" + report.BatchNo + "】";
                    proBatch.PhotoUrl = product.PhotoUrl;
                    proBatch.Number = report.BatchNo;
                    proBatch.ProductId = product.Id;
                    proBatch.Id = _snowflakeHelper.NextId().ToString();
                    await proBatchDAL.Insert(proBatch);
                }
            }

            await workOrderDAL.IncreaseProgress(workOrder.Id, report.GoodNum.Value);
            if (workOrder.BatchCount < workOrder.Quantity && (workOrder.BatchCount + report.GoodNum.Value) >= workOrder.Quantity)
            {
                //结束工单
                MZ_WorkOrder neworder = new MZ_WorkOrder();
                neworder.Id = workOrder.Id;
                neworder.Status = 2;
                neworder.EndOn = report.EndWork;
                await workOrderDAL.Update(neworder);
                await _provider.GetService<ProductPlanDAL>().FinishPlane(workOrder.PlanId);
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
