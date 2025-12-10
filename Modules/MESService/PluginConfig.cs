using AuthService.Fields;
using Common;
using Common.DataAc;
using Common.EventBus;
using MESService.Business;
using MESService.DAL;
using MESService.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MonitorService.Business;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace MESService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService", "FlowService", "ProducerService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<BomBLL>();
            services.AddBLL<ConfigBLL>();
            services.AddBLL<DefectBLL>();
            services.AddBLL<MesActionBLL>();
            services.AddBLL<OperBLL>();
            services.AddBLL<PlanBLL>();
            services.AddBLL<ReportBLL>();
            services.AddBLL<RouteBLL>();
            services.AddBLL<WorkOrderBLL>();
            services.AddBLL<WorkTaskBLL>();
            services.AddBLL<BatchDevHisBLL>();
            services.AddBLL<WorkBatchBLL>();

            services.AddDAL<BomHeaderDAL>();
            services.AddDAL<BomLineDAL>();
            services.AddDAL<DefectDAL>();
            services.AddDAL<OperDAL>();
            services.AddDAL<RouteDAL>();
            services.AddDAL<RouteOperDAL>();
            services.AddDAL<ProductPlanDAL>();
            services.AddDAL<ProductPlanItemDAL>();
            services.AddDAL<FactoryMesDAL>();
            services.AddDAL<MesActionDAL>();
            services.AddDAL<WorkOrderDAL>();
            services.AddDAL<WorkBomDAL>();
            services.AddDAL<WorkReportDAL>();
            services.AddDAL<WorkBatchDAL>();
            services.AddDAL<WorkTaskDAL>();
            services.AddDAL<BatchDevHisDAL>();
            services.AddDAL<WorkDefectDAL>();
        }
        private DA_Table tb1;
        private DA_Table tb2;
        protected override void Configure(ITAApplication app, PluginObject plg)
        {

            #region 可变动数据
            var redis = app.ServiceProvider.GetService<GeneralRedisHelper>();
            tb1 = new DA_Table()
            {
                name = "生产计划",
                code = "mz_product_plan"
            };
            tb1.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "生产计划单号",
                        code = "@Number",
                        type = "Text",
                        used = 1,
                        formlist = new List<DA_Value>
                        {
                           new DA_Value()
                           {
                               name="发起的单号",
                               val="@from"
                           }
                        }
                    },
                    new DA_Field()
                    {
                        name = "生产计划状态",
                        code = "Status",
                        type = "Enum",
                        used = 2,
                       options=new List<DA_Value>
                       {
                           new DA_Value()
                           {
                               name="待执行",
                               val="2"
                           },
                           new DA_Value()
                           {
                               name="已驳回",
                               val="6"
                           }
                       }
                    }
                };

            tb2 = new DA_Table()
            {
                name = "生产报工",
                code = "mz_work_report"
            };
            tb2.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "生产报工单号",
                        code = "@Number",
                        type = "Text",
                        used = 1,
                        formlist = new List<DA_Value>
                        {
                           new DA_Value()
                           {
                               name="发起的单号",
                               val="@from"
                           }
                        }
                    },
                    new DA_Field()
                    {
                        name = "生产报工状态",
                        code = "Status",
                        type = "Enum",
                        used = 2,
                       options=new List<DA_Value>
                       {
                           new DA_Value()
                           {
                               name="已审核",
                               val="2"
                           },
                           new DA_Value()
                           {
                               name="已驳回",
                               val="4"
                           }
                       }
                    }
                };

            redis.HashSet("BusChange-Event", "MESService", new List<DA_Table> { tb1, tb2 });

            #endregion

            #region 固有字段
            List<FieldBase> reportfields = new List<FieldBase>();
            reportfields.Add(new TextField()
            {
                mapid = "LNumber",
                name = "通讯编号",
                type = "文本"
            });
            redis.HashSet("FixedFields", "报工", reportfields);
            #endregion

            //监听数据变动
            plg.RegisterCall("ChangeData", async (evt) =>
            {
                var paramdata = evt.To<ActionChangeData>();
                if (tb1.IsThisTable(paramdata))
                {
                    var res = await app.ServiceProvider.GetService<MesActionBLL>().DoPlanActionEvent(paramdata);
                    return CallResponse.CreateFrom(res);
                }
                else if (tb2.IsThisTable(paramdata))
                {
                    var res = await app.ServiceProvider.GetService<MesActionBLL>().DoReportActionEvent(paramdata);
                    return CallResponse.CreateFrom(res);
                }
                return CallResponse.Next();
            });





        }
    }
}
