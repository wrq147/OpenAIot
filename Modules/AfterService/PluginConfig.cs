using TemplateAction.NetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TemplateAction.Core;
using Common;
using Common.EventBus;
using AfterService.Business;
using AfterService.DAL;
using Common.DataAc;
using MonitorService.Business;
using MonitorService.Model;
using AfterService.Model;
using System;
using System.Collections.Generic;

namespace AfterService
{
    /// <summary>
    /// 售后服务模块
    /// </summary>
    public class PluginConfig : TANetCorePluginConfig
    {
        private ILogger<PluginConfig> _log;
        public override string[] DependOn => new string[] { "AuthService", "DeveloperService", "FlowService", "IoTService" };
        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<DevPlaneBLL>();
            services.AddBLL<DevPlaneTaskBLL>();
            services.AddBLL<KFDeviceBLL>();
            services.AddBLL<RoomBLL>();
            services.AddBLL<RoomCategoryBLL>();
            services.AddBLL<RoomDeviceBLL>();
            services.AddBLL<AfterActionBLL>();

            services.AddDAL<DevPlaneDAL>();
            services.AddDAL<DevPlaneTaskDAL>();
            services.AddDAL<KFDeviceDAL>();
            services.AddDAL<RoomCategoryDAL>();
            services.AddDAL<RoomDAL>();
            services.AddDAL<RoomDeviceDAL>();
            services.AddDAL<AfterActionDAL>();

        }
        private DA_Table tb3;
        protected override void Configure(ITAApplication app, PluginObject plg)
        {

            tb3 = new DA_Table()
            {
                name = "计划任务单",
                code = "mz_plane_task"
            };
            tb3.fields = new List<DA_Field>
                {
                    new DA_Field()
                    {
                        name = "任务单号",
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
                        name = "任务状态",
                        code = "TaskStatus",
                        type = "Enum",
                        used = 2,
                       options=new List<DA_Value>
                       {
                           new DA_Value()
                           {
                               name="待执行",
                               val="1"
                           },
                           new DA_Value()
                           {
                               name="执行中",
                               val="2"
                           },
                           new DA_Value()
                           {
                               name="已完成",
                               val="3"
                           },
                           new DA_Value()
                           {
                               name="已验收",
                               val="5"
                           },
                           new DA_Value()
                           {
                               name="验收失败",
                               val="6"
                           },
                           new DA_Value()
                           {
                               name="已作废",
                               val="7"
                           }
                       }
                    }
                };

            var redis = app.ServiceProvider.GetService<GeneralRedisHelper>();
            redis.HashSet("BusChange-Event", "AfterService", new List<DA_Table> { tb3 });


            if (Constants.General.quick_init != true)
            {
                TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
                {
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    //定时处理到期设备计划任务
                    string jhjobname = "ExecuteExpireTask";
                    string jhgroup = "SYSTEM";
                    if (!await jobBLL.ExistJob(jhjobname, jhgroup))
                    {
                        MZ_Job job = new MZ_Job();
                        job.concurrent = "1";
                        job.createId = 0;
                        job.create_time = DateTime.Now;
                        job.updateId = 0;
                        job.update_time = DateTime.Now;
                        job.cron_expression = "0 10 0 * * ?";
                        job.invoke_target = typeof(DevPlaneTaskBLL).FullName + ".ExpireExecute()";
                        job.job_group = jhgroup;
                        job.job_name = jhjobname;
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
                if (tb3.IsThisTable(paramdata))
                {
                    paramdata.TargetName = "计划任务单";
                    var res = await app.ServiceProvider.GetService<AfterActionBLL>().DoPlaneActionEvent(paramdata);
                    return CallResponse.Create(res);
                }
                return CallResponse.Next();
            });


            //监听业务事件
            plg.RegisterBus("DeviceEvent", async (bs) =>
            {
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<DeviceEventData>(bs.Params);
                await app.ServiceProvider.GetService<DevPlaneBLL>().EventToTask(evt);
            });

            plg.RegisterBus("IOTDeviceDel", async (bs) =>
            {
                try
                {
                    var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(bs.Params);
                    await app.ServiceProvider.GetService<RoomDeviceBLL>().ClearJunk(Convert.ToInt64(evt.OrgId));
                }
                catch { }
            });

            plg.RegisterBus("LeaveApply", async (bs) =>
            {
                //领用时分配设备房间
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(bs.Params);
                long torgId = Convert.ToInt64(evt.OrgId);
                long tleaderId = Convert.ToInt64(evt.LeaderId);
                string tDevIds = Convert.ToString(evt.DevIds);
                string[] tdevIdsArr = tDevIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    var tRoomlist = await app.ServiceProvider.GetService<RoomDAL>().SelectList(x => x.OrgId == torgId && x.LeaderId == tleaderId && x.AutoAdd == true);
                    var roomDeviceDAL = app.ServiceProvider.GetService<RoomDeviceDAL>();
                    foreach (var room in tRoomlist)
                    {
                        List<MZ_RoomDevice> troomDeviceList = new List<MZ_RoomDevice>();
                        foreach (var t in tdevIdsArr)
                        {
                            MZ_RoomDevice roomDevice = new MZ_RoomDevice();
                            roomDevice.Id = room.Id;
                            roomDevice.TargetId = t;
                            roomDevice.OrgId = room.OrgId;
                            troomDeviceList.Add(roomDevice);
                        }
                        await roomDeviceDAL.InsertOrIgnore(troomDeviceList);
                    }
                }
                catch { }
            });

            plg.RegisterBus("StockLeave", async (bs) =>
            {
                //出库时为房间分配设备
                var evt = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(bs.Params);
                long torgId = Convert.ToInt64(evt.OrgId);
                long ttargetOrgId = Convert.ToInt64(evt.TargetOrgId);
                string tDevIds = Convert.ToString(evt.DevIds);
                string[] tdevIdsArr = tDevIds.Split(',', StringSplitOptions.RemoveEmptyEntries);

                try
                {
                    var tRoomlist = await app.ServiceProvider.GetService<RoomDAL>().SelectList(x => x.OrgId == torgId && x.TargetOrgId == ttargetOrgId && x.AutoAdd == true);
                    var roomDeviceDAL = app.ServiceProvider.GetService<RoomDeviceDAL>();
                    foreach (var room in tRoomlist)
                    {
                        List<MZ_RoomDevice> troomDeviceList = new List<MZ_RoomDevice>();
                        foreach (var t in tdevIdsArr)
                        {
                            MZ_RoomDevice roomDevice = new MZ_RoomDevice();
                            roomDevice.Id = room.Id;
                            roomDevice.TargetId = t;
                            roomDevice.OrgId = room.OrgId;
                            troomDeviceList.Add(roomDevice);
                        }
                        await roomDeviceDAL.InsertOrIgnore(troomDeviceList);
                    }
                }
                catch { }
            });

        }
    }
}
