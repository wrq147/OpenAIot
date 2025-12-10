using Common;
using Common.DataAc;
using Common.EventBus;
using Common.Share;
using EasyNetQ;
using IoTService.Business;
using IoTService.DAL;
using IoTService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MonitorService.Business;
using MonitorService.Model;
using System;
using System.Collections.Generic;
using TemplateAction.Core;
using TemplateAction.NetCore;

namespace IoTService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "AuthService" };


        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<IotProductBLL>();
            services.AddBLL<IotClassBLL>();
            services.AddBLL<IotDeviceBLL>();
            services.AddBLL<IotUpdateBLL>();
            services.AddBLL<IotWarningBLL>();
            services.AddBLL<IotTagBLL>();
            services.AddBLL<IotCardBLL>();
            services.AddBLL<IotConfigBLL>();
            services.AddBLL<IotScriptBLL>();
            services.AddBLL<IotExceptBLL>();
            services.AddBLL<IotWarnConfigBLL>();
            services.AddSingleton<IotInfluxBLL>();
            services.AddBLL<IotHisSourceBLL>();
            services.AddBLL<IotCodeBLL>();
            services.AddBLL<IotWinRuleBLL>();


            services.AddDAL<IotClassDAL>();
            services.AddDAL<IotProductDAL>();
            services.AddDAL<IotDeviceDAL>();
            services.AddDAL<IotDeviceTagDAL>();
            services.AddDAL<IotUpdateDAL>();
            services.AddDAL<IotWarningDAL>();
            services.AddDAL<IotCardDAL>();
            services.AddDAL<IotConfigDAL>();
            services.AddDAL<IotScriptDAL>();
            services.AddDAL<IotExceptDAL>();
            services.AddDAL<IotActionDAL>();
            services.AddDAL<IotHisSourceDAL>();
            services.AddDAL<IotCodeDAL>();
            services.AddDAL<IotCodeGroupDAL>();
            services.AddDAL<IotWinRuleDAL>();
            services.AddDAL<IotWarnConfigDAL>();

            services.AddSingleton<IotRedisHelper>();
            services.AddSingleton<ServerBusProxy>();


            services.Configure<IotOption>(config.GetSection("IoTService"));
        }
        private DA_Table tb1;
        protected override async void Configure(ITAApplication app, PluginObject plg)
        {

            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                if (Constants.General.quick_init != true)
                {
                    //添加定时同步设备
                    string devjobname = "DeviceSystemUpdate";
                    string devgroup = "SYSTEM";
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    if (!await jobBLL.ExistJob(devjobname, devgroup))
                    {
                        MZ_Job devjob = new MZ_Job();
                        devjob.concurrent = "1";
                        devjob.createId = 0;
                        devjob.create_time = DateTime.Now;
                        devjob.updateId = 0;
                        devjob.update_time = DateTime.Now;
                        devjob.cron_expression = "0/10 * * * * ?";
                        devjob.invoke_target = typeof(IotDeviceBLL).FullName + ".SyncDevice()";
                        devjob.job_group = devgroup;
                        devjob.job_name = devjobname;
                        devjob.misfire_policy = "2";
                        devjob.status = "0";

                        await jobBLL.InsertJob(devjob);
                    }


                    //添加定时删除更新日志
                    string clearupdateLog = "ClearUpdateLog";
                    string cleargroup = "SYSTEM";
                    if (!await jobBLL.ExistJob(clearupdateLog, cleargroup))
                    {
                        MZ_Job warnjob = new MZ_Job();
                        warnjob.concurrent = "1";
                        warnjob.createId = 0;
                        warnjob.create_time = DateTime.Now;
                        warnjob.updateId = 0;
                        warnjob.update_time = DateTime.Now;
                        warnjob.cron_expression = "0 0 3 * * ?";
                        warnjob.invoke_target = typeof(IotDeviceBLL).FullName + ".ClearSyncDeviceLog()";
                        warnjob.job_group = cleargroup;
                        warnjob.job_name = clearupdateLog;
                        warnjob.misfire_policy = "2";
                        warnjob.status = "0";

                        await jobBLL.InsertJob(warnjob);
                    }

                    //添加定时删除过期告警
                    string warnjobname = "DeviceWarnClearTask";
                    string warngroup = "SYSTEM";
                    if (!await jobBLL.ExistJob(warnjobname, warngroup))
                    {
                        MZ_Job warnjob = new MZ_Job();
                        warnjob.concurrent = "1";
                        warnjob.createId = 0;
                        warnjob.create_time = DateTime.Now;
                        warnjob.updateId = 0;
                        warnjob.update_time = DateTime.Now;
                        warnjob.cron_expression = "0 0 3 * * ?";
                        warnjob.invoke_target = typeof(IotWarningBLL).FullName + ".ClearOverWarning()";
                        warnjob.job_group = warngroup;
                        warnjob.job_name = warnjobname;
                        warnjob.misfire_policy = "2";
                        warnjob.status = "0";

                        await jobBLL.InsertJob(warnjob);
                    }

                    //添加定时同步物联网卡信息
                    string cardjobname = "IotCardSyncTask";
                    string cardgroup = "SYSTEM";
                    if (!await jobBLL.ExistJob(cardjobname, cardgroup))
                    {
                        MZ_Job cardjob = new MZ_Job();
                        cardjob.concurrent = "1";
                        cardjob.createId = 0;
                        cardjob.create_time = DateTime.Now;
                        cardjob.updateId = 0;
                        cardjob.update_time = DateTime.Now;
                        cardjob.cron_expression = "0 0 3 * * ?";
                        cardjob.invoke_target = typeof(IotCardBLL).FullName + ".SyncCard()";
                        cardjob.job_group = cardgroup;
                        cardjob.job_name = cardjobname;
                        cardjob.misfire_policy = "2";
                        cardjob.status = "0";

                        await jobBLL.InsertJob(cardjob);
                    }


                    #region 添加定时属性规则

                    string winrulejobname = "IotWinRuleSyncTask";
                    string winrulegroup = "SYSTEM";

                    if (!await jobBLL.ExistJob(winrulejobname, winrulegroup))
                    {
                        MZ_Job devjob = new MZ_Job();
                        devjob.concurrent = "1";
                        devjob.createId = 0;
                        devjob.create_time = DateTime.Now;
                        devjob.updateId = 0;
                        devjob.update_time = DateTime.Now;
                        devjob.cron_expression = "0 0 * * * ?";
                        devjob.invoke_target = typeof(IotWinRuleBLL).FullName + ".CalDevice($context)";
                        devjob.job_group = winrulegroup;
                        devjob.job_name = winrulejobname;
                        devjob.misfire_policy = "1";
                        devjob.status = "0";

                        await jobBLL.InsertJob(devjob);
                    }

                    #endregion
                }


                var generalOption = app.ServiceProvider.GetService<IOptions<GeneralOption>>();
                if (!string.IsNullOrEmpty(generalOption.Value.event_bus_conn))
                {
                    var bus = app.ServiceProvider.GetService<RabbitScope>().Bus;
                    await bus.PubSub.SubscribeAsync<string>("RuleNode" + MyAccess.Core.StringTool.GetGUID(), (msg) =>
                    {
                        app.ServiceProvider.GetService<ServerBusProxy>().UpdateUpList();
                    }, cfg =>
                    {
                        cfg.WithTopic("/RuleNode.Change");
                        cfg.WithAutoDelete(true);
                    });

                    //设置规则执行节点
                    app.ServiceProvider.GetService<ServerBusProxy>().RegNode();
                }


            });




            #region 可变动数据
            var redis = app.ServiceProvider.GetService<GeneralRedisHelper>();
            tb1 = new DA_Table()
            {
                name = "告警工单",
                code = "mz_iot_warning"
            };
            tb1.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "告警单号",
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
                        name = "工单状态",
                        code = "Status",
                        type = "Enum",
                        used = 2,
                       options=new List<DA_Value>
                       {
                           new DA_Value()
                           {
                               name="待处理",
                               val="0"
                           },
                           new DA_Value()
                           {
                               name="已处理",
                               val="1"
                           },
                           new DA_Value()
                           {
                               name="待派工",
                               val="2"
                           }
                       }
                    },
                    new DA_Field()
                    {
                        name = "处理备注",
                        code = "ClearRemark",
                        type = "Text",
                        used = 2
                    },
                };


            redis.HashSet("BusChange-Event", "IoTService", new List<DA_Table> { tb1 });
            #endregion

            //监听数据变动
            plg.RegisterCall("ChangeData", async (evt) =>
            {
                var paramdata = evt.To<ActionChangeData>();
                if (tb1.IsThisTable(paramdata))
                {
                    var res = await app.ServiceProvider.GetService<IotActionBLL>().DoActionEvent(paramdata);
                    return CallResponse.CreateFrom(res);
                }
                return CallResponse.Next();
            });



            plg.RegisterBus("UpdateIotOrg", async (bs) =>
            {
                string tid = bs.GetValue("Id");
                long tOwnerOrgId = bs.GetLong("OwnerOrgId");
                long tUseOrgId = bs.GetLong("UseOrgId");
                long tUseUserId = bs.GetLong("UseUserId");

                MZ_IotDevice tmpdevice = new MZ_IotDevice();
                tmpdevice.Id = tid;
                tmpdevice.OwnerOrgId = tOwnerOrgId;
                tmpdevice.UseOrgId = tUseOrgId;
                tmpdevice.UseUserId = tUseUserId;
                var iotDevDAL = app.ServiceProvider.GetService<IotDeviceDAL>();
                await iotDevDAL.Update(tmpdevice);

                string tmpdtuid = await iotDevDAL.IdToDtuId(tid);
                if (tmpdtuid != null)
                {
                    IotRedisHelper iotredis = app.ServiceProvider.GetService<IotRedisHelper>();
                    await iotredis.HashDeleteAsync("Device:" + tmpdtuid, "$DeviceOrgIds");
                }

            });


            plg.RegisterBus("BatchIotOrg", async (bs) =>
            {
                var evt = bs.To<BatchIotOrgParams>();
                var deviceDAL = app.ServiceProvider.GetService<IotDeviceDAL>();
                if (evt.OwnerOrgId != null)
                {
                    await deviceDAL.UpdateBatchOwnerOrgId(evt.Ids, evt.OwnerOrgId.Value, evt.UseOrgId == 0);
                    if (evt.ClearOwnerOrgId != null && evt.ClearOwnerOrgId > 0)
                    {
                        //清除设备的路径信息
                        await deviceDAL.ClearOwnerOrgPath(evt.Ids, evt.ClearOwnerOrgId.Value);
                    }
                }
                else if (evt.UseOrgId != null)
                {
                    await deviceDAL.UpdateUseOrgId(evt.Ids, evt.UseOrgId.Value);
                }
                IotRedisHelper iotredis = app.ServiceProvider.GetService<IotRedisHelper>();
                var dtuIds = await deviceDAL.IdsToDtuIds(evt.Ids);
                foreach (var dtuId in dtuIds)
                {
                    await redis.HashDeleteAsync("Device:" + dtuId, "$DeviceOrgIds");
                }
            });

            //触发更新设备的关键词
            plg.RegisterBus("UpdateDeviceKeywords", async (bs) =>
            {
                var tOrgId = bs.GetLong("OrgId");
                await app.ServiceProvider.GetService<IotDeviceBLL>().UpdateDeviceKeywords(tOrgId);
            });


            //同步Mes批次
            plg.RegisterCall("FromMesBatch", async (bs) =>
            {
                var tUserId = bs.GetLong("UserId");
                var tOrgId = bs.GetLong("OrgId");
                var tPhotoUrl = bs.GetValue("PhotoUrl");
                var tDeviceNumber = bs.GetValue("DeviceNumber");
                var tProductId = bs.GetValue("ProductId");
                var tMesProductId = bs.GetValue("MesProductId");
                var tDeviceId = bs.GetValue("DeviceId");
                var tName = bs.GetValue("Name");
                ArtificialUser artificialUser = new ArtificialUser(tUserId, tOrgId);
                var tdevlist = await app.ServiceProvider.GetService<IotDeviceDAL>().SelectList(x => x.DeviceNumber == tDeviceNumber);
                if (tdevlist.Count > 0)
                {
                    MZ_IotDevice dev = new MZ_IotDevice();
                    dev.Id = tdevlist[0].Id;
                    dev.PhotoUrl = tPhotoUrl;
                    dev.ProductId = tProductId;
                    dev.MesProductId = tMesProductId;
                    dev.DeviceId = tDeviceId;
                    dev.Name = tName;
                    var res = await app.ServiceProvider.GetService<IotDeviceBLL>().Update(dev, artificialUser, tdevlist[0], false);
                    return CallResponse.CreateFrom(res);
                }
                else
                {
                    MZ_IotDevice dev = new MZ_IotDevice();
                    dev.PhotoUrl = tPhotoUrl;
                    dev.DeviceNumber = tDeviceNumber;
                    dev.ProductId = tProductId;
                    dev.MesProductId = tMesProductId;
                    dev.DeviceId = tDeviceId;
                    dev.Name = tName;
                    var res = await app.ServiceProvider.GetService<IotDeviceBLL>().Insert(dev, artificialUser, false);
                    return CallResponse.CreateFrom(res);
                }

            });

        }

    }
}
