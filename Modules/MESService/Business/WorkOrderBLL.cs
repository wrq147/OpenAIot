using Common.IdGenerator;
using Common.Share;
using FluentMigrator.Builders.Alter.Table;
using MESService.DAL;
using MESService.Model;
using Microsoft.Extensions.Logging;
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

        public virtual async Task<PageObject<MZ_WorkOrder>> SelectList(In_WorkOrderList query, IUserInfo user)
        {
            return await _workOrderDAL.SelectByPage(query, user.OrgId);
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
                    await RecursiveProduct(item.ProductId, need, item, plan, null, product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task RecursiveProduct(string productId, decimal num, MZ_ProductPlanItem planItem, MZ_ProductPlan plan, string parentWorkId, MZ_Product product)
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
                order.ParentWorkOrderId = parentWorkId ?? string.Empty;
                order.Id = _snowflake.NextId().ToString();
                order.OrgId = product.OrgId;
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
                await _workOrderDAL.Insert(order);

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
                    await RecursiveProduct(item.ProductId, need, planItem, plan, order.Id, null);
                }
            }
        }

        /// <summary>
        /// 定时生成工单任务
        /// </summary>
        /// <returns></returns>
        public async Task OrderToTask()
        {
            //提前6个小时生成任务
            var orderlist = await _workOrderDAL.SelectList(x => x.Status == 0 && DateTime.Now.AddHours(6) > x.PlannedStartOn);
            foreach (var order in orderlist)
            {
                MZ_WorkOrder neworder = new MZ_WorkOrder();
                var product = await _productDAL.Select(order.ProductId);
                if (product == null)
                {
                    neworder.Id = order.Id;
                    neworder.Status = 3;
                    neworder.CancelReason = $"工单的产品未关联任何工艺路线";
                    await _workOrderDAL.Update(neworder);
                    continue;
                }
                var routeOpers = await _routeOperDAL.SelectList(x => x.RouteId == product.Route);
                if (routeOpers.Count == 0)
                {
                    neworder.Id = order.Id;
                    neworder.Status = 3;
                    neworder.CancelReason = $"工单的工序未关联任何工艺路线";
                    await _workOrderDAL.Update(neworder);
                    continue;
                }
                List<MZ_WorkTask> tasks = new List<MZ_WorkTask>();
                foreach (var oper in routeOpers)
                {
                    MZ_WorkTask task = new MZ_WorkTask();
                    task.Id = _snowflake.NextId().ToString();
                    task.OrgId = order.OrgId;
                    task.PlanId = order.PlanId;
                    task.WorkOrderId = order.Id;
                    task.OperId = oper.OperId;
                    task.StartOn = order.PlannedStartOn;
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
                    task.StrExt1 = oper.StrExt1;
                    task.StrExt2 = oper.StrExt2;
                    task.StrExt3 = oper.StrExt3;
                    task.StrExt4 = oper.StrExt4;
                    task.StrExt5 = oper.StrExt5;
                    task.StrExt6 = oper.StrExt6;
                    task.StrExt7 = oper.StrExt7;
                    task.StrExt8 = oper.StrExt8;
                    task.StrExt9 = oper.StrExt9;
                    task.StrExt10 = oper.StrExt10;
                    task.StrExt11 = oper.StrExt11;
                    task.StrExt12 = oper.StrExt12;
                    task.StrExt13 = oper.StrExt13;
                    task.StrExt14 = oper.StrExt14;
                    task.StrExt15 = oper.StrExt15;
                    task.StrExt16 = oper.StrExt16;
                    task.StrExt17 = oper.StrExt17;
                    task.StrExt18 = oper.StrExt18;
                    task.StrExt19 = oper.StrExt19;
                    task.StrExt20 = oper.StrExt20;
                    task.StrExt21 = oper.StrExt21;
                    task.StrExt22 = oper.StrExt22;
                    task.StrExt23 = oper.StrExt23;
                    task.StrExt24 = oper.StrExt24;
                    task.StrExt25 = oper.StrExt25;
                    task.StrExt26 = oper.StrExt26;
                    task.StrExt27 = oper.StrExt27;
                    task.StrExt28 = oper.StrExt28;
                    task.StrExt29 = oper.StrExt29;
                    task.StrExt30 = oper.StrExt30;
                    task.NumExt1 = oper.NumExt1;
                    task.NumExt2 = oper.NumExt2;
                    task.NumExt3 = oper.NumExt3;
                    task.NumExt4 = oper.NumExt4;
                    task.NumExt5 = oper.NumExt5;
                    task.NumExt6 = oper.NumExt6;
                    task.NumExt7 = oper.NumExt7;
                    task.NumExt8 = oper.NumExt8;
                    task.NumExt9 = oper.NumExt9;
                    task.NumExt10 = oper.NumExt10;
                    tasks.Add(task);
                }

                await _workTaskDAL.Insert(tasks);
                neworder.Id = order.Id;
                neworder.Status = 1;
                await _workOrderDAL.Update(neworder);
            }
        }
    }
}
