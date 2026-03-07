using ChannelUtility.Message;
using Common;
using Common.EventBus;
using IoTRulesService.DataParser;
using IoTService.Business;
using IoTVideoService.Business;
using IoTVideoService.DAL;
using IoTVideoService.Models;
using IoTVideoService.PlanUtil;
using Microsoft.Extensions.Configuration;
using MonitorService.Business;
using MonitorService.Model;
using MonitorService.Util;
using System;
using TemplateAction.Core;
using TemplateAction.NetCore;
namespace IoTVideoService
{
    public class PluginConfig : TANetCorePluginConfig
    {
        public override string[] DependOn => new string[] { "IoTService", "IoTRulesService" };


        protected override void ConfigureServices(IConfiguration config, IServiceCollection services)
        {
            services.AddBLL<VideoSourceBLL>();
            services.AddBLL<PtzBLL>();
            services.AddBLL<RecordBLL>();

            services.AddDAL<RecordDAL>();
            services.AddDAL<RecordFileDAL>();
            services.AddDAL<RecordLogDAL>();
            services.AddDAL<VideoSourceDAL>();
            services.AddDAL<RecordKeyDAL>();

            services.AddSingleton<PlanConcurrentJob>();
            services.Configure<VideoOption>(config.GetSection("IoTVideoService"));
        }
        private ITAServiceProvider _provider;
        protected override void Configure(ITAApplication app, PluginObject plg)
        {
            _provider = app.ServiceProvider;
            TAEventDispatcher.Instance.RegisterPluginAllLoad(async (evt) =>
            {
                if (Constants.General.quick_init != true)
                {
                    //添加定时清除视频数据
                    string cacjobname = "VideoCleanService";
                    string cacgroup = "SYSTEM";
                    var jobBLL = app.ServiceProvider.GetService<JobBLL>();
                    if (!await jobBLL.ExistJob(cacjobname, cacgroup))
                    {
                        MZ_Job devjob = new MZ_Job();
                        devjob.concurrent = "0";
                        devjob.createId = 0;
                        devjob.create_time = DateTime.Now;
                        devjob.updateId = 0;
                        devjob.update_time = DateTime.Now;
                        devjob.cron_expression = "0 10 1 * * ?";
                        devjob.invoke_target = typeof(VideoSourceBLL).FullName + ".TimerClean()";
                        devjob.job_group = cacgroup;
                        devjob.job_name = cacjobname;
                        devjob.misfire_policy = "2";
                        devjob.status = "0";

                        await jobBLL.InsertJob(devjob);
                    }


                }

                //初始化录像计划定时器
                await PlanSchedule.InitScheduler(app.ServiceProvider);
            });

            plg.RegisterQuartzTask();
            app.ServiceProvider.GetService<MessageRunner>().OtherMessageListener += MessageHandler;
        }

        private async Task MessageHandler(BaseDeviceMessage msg)
        {
            switch (msg.MsgType)
            {
                case "MediaNF":
                    {
                        MediaNotFoundMessage nfmsg = (MediaNotFoundMessage)msg;
                        await _provider.GetService<VideoSourceBLL>().CollectVideo(nfmsg);
                    }
                    break;
                case "MediaNR":
                    {
                        MediaNotReaderMessage nrmsg = (MediaNotReaderMessage)msg;
                        await _provider.GetService<VideoSourceBLL>().DelVideo(nrmsg);
                    }
                    break;
                case "MediaCH":
                    {
                        MediaChannelMessage mcmsg = (MediaChannelMessage)msg;
                        await _provider.GetService<VideoSourceBLL>().InitChannels(mcmsg.UserName, mcmsg.DeviceId, mcmsg.Channels);
                    }
                    break;
                case "MediaUser":
                    {
                        var videoSourceBLL = _provider.GetService<VideoSourceBLL>();
                        MediaUserVerifyMessage uvmsg = (MediaUserVerifyMessage)msg;
                        var tpassword = await videoSourceBLL.GB28181Login(uvmsg.UserName);
                        if (tpassword == null)
                        {
                            await videoSourceBLL.ResponseVerifyResult(uvmsg.MessageId, string.Empty);
                        }
                        else
                        {
                            await videoSourceBLL.ResponseVerifyResult(uvmsg.MessageId, tpassword);
                        }
                    }
                    break;
                case "MediaFile":
                    {
                        MediaRecordFileMessage rfile = (MediaRecordFileMessage)msg;
                        await _provider.GetService<RecordBLL>().InsertRecordFile(rfile);
                    }
                    break;
                case "MediaKey":
                    {
                        MediaKeyMessage keyMessage = (MediaKeyMessage)msg;

                        MZ_IotRecordKey key = new MZ_IotRecordKey();
                        key.VideoKey = keyMessage.VideoKey;
                        key.KeyDate = keyMessage.KeyDate;
                        key.EvtDes = keyMessage.EvtDes;
                        key.FilePath = keyMessage.FilePath;
                        await _provider.GetService<RecordBLL>().InsertKey(key);
                    }
                    break;
                case "NodeOn":
                    {
                        NodeOnlineMessage nodeMsg = (NodeOnlineMessage)msg;
                        await _provider.GetService<VideoSourceBLL>().InitFixNode(nodeMsg.DeviceId);
                    }
                    break;
            }
        }
    }
}
