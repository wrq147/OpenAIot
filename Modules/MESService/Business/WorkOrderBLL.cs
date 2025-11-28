using AuthService;
using Common;
using Common.IdGenerator;
using Common.Share;
using FluentMigrator.Builders.Alter.Table;
using MESService.DAL;
using MESService.Model;
using Microsoft.Extensions.Logging;
using NPOI.POIFS.Properties;
using ProducerService.DAL;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class WorkOrderBLL
    {
        private ITAServiceProvider _provider;
        private ProductDAL _productDAL;
        private BomLineDAL _bomLineDAL;
        private SnowflakeHelper _snowflake;
        private WorkOrderDAL _workOrderDAL;
        private WorkBomDAL _workBomDAL;
        private WorkTaskDAL _workTaskDAL;
        private RouteOperDAL _routeOperDAL;
        private ILogger<WorkOrderBLL> _logger;
        public WorkOrderBLL(ITAServiceProvider provider, ProductDAL productDAL, BomLineDAL bomLineDAL, WorkOrderDAL workOrderDAL, WorkBomDAL workBomDAL,
            RouteOperDAL routeOperDAL, WorkTaskDAL workTaskDAL, SnowflakeHelper snowflake, ILoggerFactory factory)
        {
            _provider = provider;
            _productDAL = productDAL;
            _bomLineDAL = bomLineDAL;
            _workOrderDAL = workOrderDAL;
            _workBomDAL = workBomDAL;
            _routeOperDAL = routeOperDAL;
            _workTaskDAL = workTaskDAL;
            _snowflake = snowflake;
            _logger = factory.CreateLogger<WorkOrderBLL>();
        }
        public virtual async Task<BusResponse<MZ_WorkOrder>> Info(string id)
        {
            var info = await _workOrderDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_WorkOrder>.Error(111, "生产工单不存在");
            }
            info.ProdInfo = await _provider.GetService<ProductDAL>().Select(info.ProductId);
            info.PlanInfo = await _provider.GetService<ProductPlanDAL>().Select(info.PlanId);
            return BusResponse<MZ_WorkOrder>.Success(info);
        }
        public virtual async Task<PageObject<MZ_WorkOrder>> SelectList(In_WorkOrderList query, IUserInfo user)
        {
            string parentPath = string.Empty;
            if (!string.IsNullOrEmpty(query.ParentId))
            {
                MZ_WorkOrder parentOrder = await _workOrderDAL.Select(query.ParentId);
                if (parentOrder != null)
                {
                    parentPath = parentOrder.ParentPath;
                }
            }

            var pagelist = await _workOrderDAL.SelectByPage(query, user.OrgId, parentPath);
            var parentOrderIds = pagelist.List.Where(x => !string.IsNullOrEmpty(x.ParentWorkOrderId)).Select(x => x.ParentWorkOrderId).ToList();
            if (parentOrderIds.Count > 0)
            {
                var parentOrders = (await _workOrderDAL.SelectListByIds(parentOrderIds)).ToDictionary(x => x.Id);
                foreach (var order in pagelist.List)
                {
                    if (!string.IsNullOrEmpty(order.ParentWorkOrderId))
                    {
                        if (parentOrders.TryGetValue(order.ParentWorkOrderId, out var parentOrder))
                        {
                            order.ParentWorkInfo = parentOrder;
                        }
                    }
                }
            }


            return pagelist;
        }
        public virtual async Task<string> GenerateNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("GD");
        }
        /// <summary>
        /// 通过生产计划生成生产工单
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        public virtual async Task GenerateWorkOrder(MZ_ProductPlan plan)
        {
            try
            {
                foreach (var item in plan.Items)
                {
                    var product = await _productDAL.Select(item.ProductId);
                    var need = product.Total.Value * item.Quantity.Value;
                    await RecursiveProduct(item.ProductId, need, item, plan, product, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task RecursiveProduct(string productId, decimal num, MZ_ProductPlanItem planItem, MZ_ProductPlan plan, MZ_Product product, MZ_WorkOrder parentWorkOrder)
        {
            var snowflake = _provider.GetService<SnowflakeHelper>();
            var routeBLL = _provider.GetService<RouteBLL>();
            if (product == null)
            {
                product = await _productDAL.Select(productId);
            }
            if (product != null && product.ProductFrom == "自制")
            {
                //生成工单
                MZ_WorkOrder order = new MZ_WorkOrder();
                order.Id = _snowflake.NextId().ToString();
                if (parentWorkOrder == null)
                {
                    order.ParentWorkOrderId = string.Empty;
                    order.ParentPath = order.Id + ",";
                }
                else
                {
                    order.ParentWorkOrderId = parentWorkOrder.Id;
                    order.ParentPath = parentWorkOrder.ParentPath + order.Id + ",";
                }
                order.OrgId = product.OrgId;
                order.WorkNumber = await GenerateNumber();
                order.PlanId = planItem.PlanId;
                order.Status = 0;
                order.Priority = plan.Priority;
                order.OverTime = plan.OverTime;
                order.Quantity = num;
                order.ProductId = product.Id;
                order.RouteId = product.Route;
                order.CreatedOn = order.UpdatedOn = DateTime.Now;
                order.PlannedStartOn = planItem.PlannedStartOn;
                order.PlannedEndOn = planItem.PlannedEndOn;
                order.CancelReason = string.Empty;
                order.BatchCount = 0;
                await _workOrderDAL.Insert(order);
                await OrderToTask(order);

                var tbomLineList = await _bomLineDAL.SelectList(x => x.ParentProductId == productId);
                foreach (var item in tbomLineList)
                {
                    //生成工单bom
                    decimal need = item.Quantity.Value * num;
                    MZ_WorkBom workBom = new MZ_WorkBom();
                    workBom.Id = _snowflake.NextId().ToString();
                    workBom.NeedQuantity = need;
                    workBom.Quantity = item.Quantity;
                    workBom.OrgId = order.OrgId;
                    workBom.ProductId = item.ProductId;
                    workBom.WorkOrderId = order.Id;
                    workBom.OperId = item.ProcessStepId;
                    await _workBomDAL.Insert(workBom);

                    //生成子工单
                    await RecursiveProduct(item.ProductId, need, planItem, plan, null, order);
                }
            }
        }

        /// <summary>
        /// 生成工单任务
        /// </summary>
        /// <returns></returns>
        private async Task OrderToTask(MZ_WorkOrder order)
        {
            var product = await _productDAL.Select(order.ProductId);
            if (product == null)
            {
                MZ_WorkOrder neworder = new MZ_WorkOrder();
                neworder.Id = order.Id;
                neworder.Status = 3;
                neworder.CancelReason = $"工单的产品未关联任何工艺路线";
                await _workOrderDAL.Update(neworder);
                await _provider.GetService<ProductPlanDAL>().CancelPlane(order.PlanId);
                return;
            }
            var routeOpers = await _routeOperDAL.SelectList(x => x.RouteId == product.Route);
            if (routeOpers.Count == 0)
            {
                MZ_WorkOrder neworder = new MZ_WorkOrder();
                neworder.Id = order.Id;
                neworder.Status = 3;
                neworder.CancelReason = $"工单的工序未关联任何工艺路线";
                await _workOrderDAL.Update(neworder);
                await _provider.GetService<ProductPlanDAL>().CancelPlane(order.PlanId);
                return;
            }
            List<MZ_WorkTask> tasks = new List<MZ_WorkTask>();
            foreach (var oper in routeOpers)
            {
                MZ_WorkTask task = new MZ_WorkTask();
                task.Id = _snowflake.NextId().ToString();
                task.OrgId = order.OrgId;
                task.PlanId = order.PlanId;
                task.WorkOrderId = order.Id;
                task.ProductId = order.ProductId;
                task.OperId = oper.OperId;
                task.RouteOperId = oper.Id;
                task.CreatedOn = DateTime.Now;
                task.StartOn = null;
                task.FinishOn = null;
                task.IsFinish = false;
                task.Priority = order.Priority;
                task.OverTime = order.OverTime;
                task.PropOf = oper.PropOf;
                task.WorkTime = oper.WorkTime;
                task.WorkTimeTotal = 0;
                task.PlanNum = oper.PropOf * order.Quantity;
                task.GoodNum = 0;
                task.DefectNum = 0;
                task.Sequence = oper.Sequence;
                task.Remark = string.Empty;
                tasks.Add(task);
            }

            await _workTaskDAL.Insert(tasks);
        }
    }
}
