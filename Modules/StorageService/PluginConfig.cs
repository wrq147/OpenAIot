using TemplateAction.NetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TemplateAction.Core;
using Common;
using Common.EventBus;
using Common.DataAc;
using MonitorService.Business;
using MonitorService.Model;
using StorageService.Business;
using StorageService.Model;
using StorageService.DAL;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Common.Share;

namespace StorageService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        private ILogger<PluginConfig> _log;
        public override string[] DependOn => new string[] { "AuthService", "FlowService", "ProducerService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<ApplyBLL>();
            services.AddBLL<HouseBLL>();
            services.AddBLL<InventoryBLL>();
            services.AddBLL<StockBLL>();
            services.AddBLL<StorageActionBLL>();
            services.AddBLL<StReportBLL>();

            services.AddDAL<EnterDetailDAL>();
            services.AddDAL<EnterStockDAL>();
            services.AddDAL<InventoryDAL>();
            services.AddDAL<InventoryItemDAL>();
            services.AddDAL<InventoryUserDAL>();
            services.AddDAL<LeaveApplyDAL>();
            services.AddDAL<LeaveApplyDetailDAL>();
            services.AddDAL<LeaveDetailDAL>();
            services.AddDAL<LeaveStockDAL>();
            services.AddDAL<StockPileDAL>();
            services.AddDAL<StockRecordDAL>();
            services.AddDAL<StorageActionDAL>();
            services.AddDAL<StoreHouseDAL>();

            services.Configure<StorageOption>(config.GetSection("StorageService"));
        }
        private DA_Table tb1;
        private DA_Table tb2;
        private DA_Table tb4;
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            #region 可变动数据
            var redis = app.ServiceProvider.GetService<GeneralRedisHelper>();
            tb1 = new DA_Table()
            {
                name = "出库单",
                code = "mz_leave_stock"
            };
            tb1.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "出库单号",
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
                        name = "出库状态",
                        code = "Status",
                        type = "Enum",
                        used = 2,
                       options=new List<DA_Value>
                       {
                           new DA_Value()
                           {
                               name="出库成功",
                               val="2"
                           },
                           new DA_Value()
                           {
                               name="出库失败",
                               val="3"
                           }
                       }
                    }
                };


            tb2 = new DA_Table()
            {
                name = "入库单",
                code = "mz_enter_stock"
            };

            tb2.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "入库单号",
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
                        name = "入库状态",
                        code = "Status",
                        type = "Enum",
                        used = 2,
                       options=new List<DA_Value>
                       {
                           new DA_Value()
                           {
                               name="入库成功",
                               val="2"
                           },
                           new DA_Value()
                           {
                               name="待退货",
                               val="3"
                           }
                       }
                    }
                };



            tb4 = new DA_Table()
            {
                name = "出库申请单",
                code = "mz_leave_apply"
            };

            tb4.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "申请单号",
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
                        name = "申请状态",
                        code = "Status",
                        type = "Enum",
                        used = 2,
                       options=new List<DA_Value>
                       {
                           new DA_Value()
                           {
                               name="申请成功",
                               val="2"
                           },
                           new DA_Value()
                           {
                               name="申请失败",
                               val="3"
                           }
                       }
                    }
                };

            redis.HashSet("BusChange-Event", "StorageService", new List<DA_Table>{
                    tb1,
                    tb2,
                    tb4
                });

            #endregion


            var factory = app.ServiceProvider.GetService<ILoggerFactory>();
            _log = factory.CreateLogger<PluginConfig>();

            if (Constants.General.quick_init != true)
            {
                TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
                {
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    //定时发送库存告警提醒
                    string xxjobname = "StockPileWarnTask";
                    string xxgroup = "SYSTEM";
                    if (!await jobBLL.ExistJob(xxjobname, xxgroup))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "1";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 0 7,8,9,10,11,12,13,14,15,16,17,18,19,20,21 * * ?";
                        job.invoke_target = typeof(StockBLL).FullName + ".WarnExecute()";
                        job.job_group = xxgroup;
                        job.job_name = xxjobname;
                        job.misfire_policy = "2";
                        job.status = "0";

                        await jobBLL.InsertJob(job);
                    }
                });
            }

            //监听数据变动
            plg.RegisterCall("ChangeData", async (evt) =>
            {
                var paramdata = Newtonsoft.Json.JsonConvert.DeserializeObject<ActionChangeData>(evt.Params);
                if (tb1.IsThisTable(paramdata))
                {
                    var res = await app.ServiceProvider.GetService<StorageActionBLL>().DoStockActionEvent(paramdata);
                    return new CallResponse(res);
                }
                else if (tb2.IsThisTable(paramdata))
                {
                    var res = await app.ServiceProvider.GetService<StorageActionBLL>().DoStockActionEvent(paramdata);
                    return new CallResponse(res);
                }
                else if (tb4.IsThisTable(paramdata))
                {
                    var res = await app.ServiceProvider.GetService<StorageActionBLL>().DoApplyActionEvent(paramdata);
                    return new CallResponse(res);
                }
                return CallResponse.Next();
            });

            //监听业务事件
            plg.RegisterBus("AddFactory", async (bs) =>
            {
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(bs.Params);
                var tmpfactoryId = Convert.ToInt64(evt.FactoryId);
                await app.ServiceProvider.GetService<HouseBLL>().AddDefHouse(tmpfactoryId);
            });


            plg.RegisterBus("JoinBy", async (bs) =>
            {
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<Tmp_JoinEventData>(bs.Params);
                await app.ServiceProvider.GetService<HouseBLL>().JoinByOtherMod(evt);
            });

            plg.RegisterBus("ManualPile", async (bs) =>
            {
                var tUserId = bs.GetLong("UserId");
                var tOrgId = bs.GetLong("OrgId");
                var targetHouseId = bs.GetValue("ToHouseId");
                var detailList = bs.GetObject("List");
                if (string.IsNullOrEmpty(targetHouseId))
                {
                    //使用默认仓库
                    var tsysHouseList = await app.ServiceProvider.GetService<StoreHouseDAL>().SelectList(x => x.OrgId == tOrgId && x.IsSystem == 1 && x.Status == "1");
                    if (tsysHouseList.Count == 0)
                    {
                        return;
                    }
                    targetHouseId = tsysHouseList[0].Id;
                }

                var detailJArr = detailList as JArray;
                if (detailJArr.Count == 0)
                {
                    return;
                }
                ArtificialUser artificialUser = new ArtificialUser(tUserId, tOrgId);
                In_ManualStock manualParam = new In_ManualStock();
                manualParam.InDate = DateTime.Now;
                manualParam.HouseId = targetHouseId;
                manualParam.Remark = string.Empty;
                manualParam.List = new List<MZ_EnterDetail>();
                foreach (var item in detailJArr)
                {
                    var jobj = item as JObject;
                    MZ_EnterDetail detail = new MZ_EnterDetail();
                    detail.TargetType = jobj.Value<int>("TargetType");
                    detail.TargetId = jobj.Value<string>("TargetId");
                    detail.Price = jobj.Value<decimal>("Price");
                    detail.Quantity = jobj.Value<decimal>("Quantity");
                    manualParam.List.Add(detail);
                }

                await app.ServiceProvider.GetService<StockBLL>().ManualPile(manualParam, artificialUser);
            });
        }
    }
}
