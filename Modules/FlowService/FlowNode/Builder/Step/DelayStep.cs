using FlowService.Business;
using MonitorService.Model;
using System;
using System.Threading.Tasks;

namespace FlowService.FlowNode.Builder.Step
{
    /// <summary>
    /// 延时
    /// </summary>
    public class DelayStep : WorkflowStep
    {
        public DelayProps props { get; set; }
        public override async Task<ExecutionResult> Run(StepExecutionContext context)
        {
            if (context.ExecutionPointer.EventPublished)
            {
                return await ExecutionResult.Next();
            }
            else
            {
                DateTime overTime = DateTime.Now;
                if (props.type == "FIXED")
                {

                    if (props.unit == "D")
                    {
                        //按天
                        overTime = overTime.AddDays(props.time);
                    }
                    else if (props.unit == "H")
                    {
                        //按时
                        overTime = overTime.AddHours(props.time);
                    }
                    else
                    {
                        //按分
                        overTime = overTime.AddMinutes(props.time);
                    }

                }
                else if (props.type == "AUTO")
                {
                    DateTime minTime = DateTime.Now.AddSeconds(15);
                    if (string.IsNullOrEmpty(props.dateTime))
                    {
                        overTime = minTime;
                    }
                    else
                    {
                        overTime = Convert.ToDateTime(props.dateTime);
                        if (overTime < minTime)
                        {
                            overTime = minTime;
                        }
                    }
                }
                else
                {
                    throw new Exception("未知的延时类型");
                }
                string jobname = "DelayOver_" + context.ExecutionPointer.Id;
                string group = "DEFAULT";
                MZ_Job job = new MZ_Job();
                job.concurrent = "0";
                job.createId = 0;
                job.create_time = DateTime.Now;
                job.updateId = 0;
                job.update_time = DateTime.Now;
                job.cron_expression = string.Format("{0} {1} {2} {3} {4} ? {5}", overTime.Second, overTime.Minute, overTime.Hour, overTime.Day, overTime.Month, overTime.Year);
                job.invoke_target = typeof(TaskBLL).FullName + ".ScheduleDelay(L" + context.ExecutionPointer.Id + ")";
                job.job_group = group;
                job.job_name = jobname;
                job.misfire_policy = "1";
                job.status = "0";
                context.JobList.Add(job);
                var eventKey = context.ExecutionPointer.Id.ToString();
                return await ExecutionResult.WaitForEvent(eventKey);
            }
        }
    }
}
