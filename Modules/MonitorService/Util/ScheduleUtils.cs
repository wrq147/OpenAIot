using Common;
using Common.EventBus;
using Common.Json;
using MonitorService.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MonitorService.Util
{
    public class ScheduleUtils
    {
        public const string TASK_CLASS_NAME = "TASK_CLASS_NAME";
        public const string TASK_PROPERTIES = "TASK_PROPERTIES";
        /// <summary>
        /// 构建任务触发对象
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobGroup"></param>
        /// <returns></returns>
        public static TriggerKey GetTriggerKey(long jobId, string jobGroup)
        {
            return new TriggerKey(TASK_CLASS_NAME + jobId, jobGroup);
        }

        /**
         * 构建任务键对象
         */
        public static JobKey GetJobKey(long jobId, string jobGroup)
        {
            return new JobKey(TASK_CLASS_NAME + jobId, jobGroup);
        }

        /// <summary>
        /// 设置定时任务策略
        /// </summary>
        /// <param name="job"></param>
        /// <param name="cb"></param>
        /// <returns></returns>
        public static CronScheduleBuilder HandleCronScheduleMisfirePolicy(MZ_Job job, CronScheduleBuilder cb)
        {
            switch (job.misfire_policy)
            {
                case "0":
                    //默认
                    return cb;
                case "1":
                    //重做错过的所有频率周期后,再按照正常的Cron频率依次执行
                    return cb.WithMisfireHandlingInstructionIgnoreMisfires();
                case "2":
                    //以当前时间为触发频率立刻触发一次执行,然后按照Cron频率依次执行
                    return cb.WithMisfireHandlingInstructionFireAndProceed();
                case "3":
                    //不触发立即执行,等待下次Cron触发频率到达时刻开始按照Cron频率依次执行
                    return cb.WithMisfireHandlingInstructionDoNothing();
                default:
                    throw new Exception("无效的任务策略 '" + job.misfire_policy + "'");
            }
        }

        /// <summary>
        /// 创建定时任务
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="job"></param>
        /// <returns></returns>
        public static async Task CreateScheduleJob(IScheduler scheduler, MZ_Job job)
        {
            bool isConcurrent = job.concurrent == "0";
            Type jobClass = isConcurrent ? typeof(QuartzJob) : typeof(QuartzDisallowConcurrentJob);
            // 构建job信息
            var jobKey = GetJobKey(job.job_id.Value, job.job_group);
            IJobDetail jobDetail = JobBuilder.Create(jobClass).WithIdentity(jobKey).Build();
            ITrigger trigger;
            if (string.IsNullOrEmpty(job.cron_expression) && !isConcurrent)
            {
                var triggetBuilder = TriggerBuilder.Create().WithIdentity(GetTriggerKey(job.job_id.Value, job.job_group)).StartNow()
           .WithSimpleSchedule(x => x
               .WithIntervalInSeconds(0)
               .RepeatForever());
                trigger = triggetBuilder.Build();
            }
            else
            {
                // 表达式调度构建器
                CronScheduleBuilder cronScheduleBuilder = CronScheduleBuilder.CronSchedule(job.cron_expression);
                cronScheduleBuilder = HandleCronScheduleMisfirePolicy(job, cronScheduleBuilder);

                // 按新的cronExpression表达式构建一个新的trigger
                var triggetBuilder = TriggerBuilder.Create().WithIdentity(GetTriggerKey(job.job_id.Value, job.job_group))
                        .WithSchedule(cronScheduleBuilder);
                trigger = triggetBuilder.Build();
            }

            // 放入参数，运行时的方法可以获取
            jobDetail.JobDataMap.Put(TASK_PROPERTIES, System.Text.Json.JsonSerializer.Serialize(job, MyDefaultTextJsonConfig.DefaultOptions));

            // 判断是否存在
            if (await scheduler.CheckExists(jobKey))
            {
                // 防止创建时存在数据问题 先移除，然后再执行创建操作
                await scheduler.DeleteJob(jobKey);
            }

            await scheduler.ScheduleJob(jobDetail, trigger);

            // 暂停任务
            if ("1".Equals(job.status))
            {
                await scheduler.PauseJob(jobKey);
            }
        }

        /// <summary>
        /// 执行指定类的方法
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="context"></param>
        /// <param name="job"></param>
        /// <param name="disConcurrent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task InvokeMethod(ITAServiceProvider provider, IJobExecutionContext context, MZ_Job job, bool disConcurrent)
        {
            if (job != null)
            {
                if (job.invoke_target.IsHttp())
                {
                    await HttpHelper.Instance.GetAsync(job.invoke_target);
                }
                else
                {
                    string className = ScheduleUtils.GetClassName(job.invoke_target);
                    string methodName = ScheduleUtils.GetMethodName(job.invoke_target);
                    string methodParams = job.invoke_target.SubstringBetween("(", ")");
                    QuartzContext quartzContext = new QuartzContext();
                    quartzContext.PreviousFireTimeUtc = context.PreviousFireTimeUtc;
                    quartzContext.ScheduledFireTimeUtc = context.ScheduledFireTimeUtc;

                    if (disConcurrent)
                    {
                        var tres = await BusUtility.TriggerWait(className, methodName, methodParams, quartzContext, job.job_id.Value);
                        if (!tres.IsSuccess())
                        {
                            throw new Exception(tres.Message);
                        }
                    }
                    else
                    {
                        await BusUtility.Trigger(className, methodName, methodParams, quartzContext, job.job_id.Value);
                    }
                }
            }
        }
        private static string GetClassName(string invokeTarget)
        {
            string beanName = invokeTarget.SubstringBefore("(");
            return beanName.SubstringBeforeLast(".");
        }
        private static string GetMethodName(string invokeTarget)
        {
            string methodName = invokeTarget.SubstringBefore("(");
            return methodName.SubstringAfterLast(".");
        }



        /// <summary>
        /// 返回下一个执行时间根据给定的Cron表达式
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <returns></returns>
        public static DateTime? GetNextExecution(string cronExpression)
        {
            try
            {
                CronExpression cron = new CronExpression(cronExpression);
                DateTime utcTime = cron.GetNextValidTimeAfter(DateTimeOffset.Now).Value.DateTime;
                return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Cron表达式转中文描述
        /// </summary>
        /// <param name="cronStr"></param>
        /// <returns></returns>
        public static string ToChineseDescription(string cronStr)
        {
            if (cronStr == null || cronStr.Length < 1)
            {
                return "cron表达式为空";
            }
            cronStr = cronStr.TrimEnd();

            var tmpCorns = cronStr.Split(' ');
            var sBuffer = new StringBuilder();
            bool hasYear = false;
            bool hasMonth = false;
            bool hasDay = false;
            bool hasHours = false;
            bool hasMinutes = false;
            bool hasSeconds = false;
            if (tmpCorns.Length == 7)
            {
                //解析年
                if (!tmpCorns[6].Equals("*"))
                {
                    hasYear = true;
                    sBuffer.Append(tmpCorns[6]).Append("年");
                }
                else
                {
                    sBuffer.Append("每年");
                }
            }
            if (tmpCorns.Length == 6 || tmpCorns.Length == 7)
            {
                //解析月
                if (!tmpCorns[4].Equals("*"))
                {
                    hasMonth = true;
                    sBuffer.Append(tmpCorns[4]).Append("月");
                }
                else
                {
                    sBuffer.Append("每月");
                }
                //解析周
                if (!tmpCorns[5].Equals("*") && !tmpCorns[5].Equals("?"))
                {

                    string tempCron = tmpCorns[5];
                    if (tempCron.EndsWith("L"))
                    {
                        tempCron = tmpCorns[5].TrimEnd('L');

                        sBuffer.Append("最后一个");
                    }
                    char[] tmpArray = tempCron.ToCharArray();

                    foreach (char tmp in tmpArray)
                    {
                        switch (tmp)
                        {
                            case '1':
                                sBuffer.Append("星期天");
                                break;

                            case '2':
                                sBuffer.Append("星期一");
                                break;

                            case '3':
                                sBuffer.Append("星期二");
                                break;

                            case '4':
                                sBuffer.Append("星期三");
                                break;

                            case '5':
                                sBuffer.Append("星期四");
                                break;

                            case '6':
                                sBuffer.Append("星期五");
                                break;

                            case '7':
                                sBuffer.Append("星期六");
                                break;

                            case '-':
                                sBuffer.Append("至");
                                break;

                            default:
                                sBuffer.Append(tmp);
                                break;
                        }
                    }
                }

                //解析日
                if (!tmpCorns[3].Equals("?"))
                {
                    if (!tmpCorns[3].Equals("*"))
                    {
                        hasDay = true;
                        sBuffer.Append(tmpCorns[3]).Append("日");
                    }
                    else
                    {
                        sBuffer.Append("每日");
                    }
                }

                //解析时
                if (!tmpCorns[2].Equals("*"))
                {
                    hasHours = true;
                    int xhgg = tmpCorns[2].IndexOf('/');
                    if (xhgg > -1)
                    {
                        string tmpsss = tmpCorns[2].Substring(0, xhgg);
                        if (tmpsss == "0")
                        {
                            sBuffer.Append("每" + tmpCorns[2].Substring(xhgg + 1) + "小时");
                        }
                        else
                        {
                            sBuffer.Append("每" + tmpCorns[2].Substring(xhgg + 1) + "小时，始于" + tmpCorns[2].Substring(0, xhgg) + "时");
                        }
                    }
                    else
                    {
                        sBuffer.Append(tmpCorns[2]).Append("时");
                    }
                }
                else
                {
                    sBuffer.Append("每时");
                }

                //解析分
                if (!tmpCorns[1].Equals("*"))
                {
                    hasMinutes = true;
                    sBuffer.Append(tmpCorns[1]).Append("分");
                }
                else
                {
                    sBuffer.Append("每分");
                }

                //解析秒
                if (!tmpCorns[0].Equals("*"))
                {
                    hasSeconds = true;
                    sBuffer.Append(tmpCorns[0]).Append("秒");
                }
                else
                {
                    sBuffer.Append("每秒");
                }
            }
            //return sBuffer.ToString();
            #region 解析优化

            string res = sBuffer.ToString();
            res = res.Replace("每月每日每时", "").Replace("0/", "");
            res = res.Replace("每月每日", "每日");
            res = res.Replace("时0分0秒", "时");
            res = res.Replace("时0分", "时");
            res = res.Replace("每分每秒", "");
            res = res.Replace("每分", "每");
            res = res.Replace("分0秒", "分");
            if (hasYear && hasMonth && hasDay && hasHours && hasMinutes && hasSeconds)
            {
                res = res.TrimStart('每');
            }
            else
            {
                res = "每" + res.TrimStart('每');
            }

            #endregion
            return res;
        }
    }
}
