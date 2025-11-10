using IoTRulesService.Flow.Node;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using TemplateAction.Core;

namespace IoTRulesService.Flow.Builder.Step
{
    public class DelayStep : RuleflowStep
    {
        public DelayProps props { get; set; }
        public override async Task Run(RuleExecutionContext context)
        {
            if (context.StartIndex == this.Index)
            {
                await context.ExcuteNext(RuleResult.Next());
                return;
            }
            else
            {
                if (context.IsDebug)
                {
                    await context.Print("开始延时执行");
                }
                DateTime overTime = DateTime.Now;
                if (props.type == "FIXED")
                {
                    int detalmm = 1000;
                    if (props.time < 1000)
                    {
                        detalmm = props.time;
                    }
                    if (props.unit == "S")
                    {
                        //按秒
                        overTime = overTime.AddSeconds(detalmm);
                    }
                    else if (props.unit == "MS")
                    {
                        //毫秒
                        await Task.Delay(detalmm);
                        await context.ExcuteNext(RuleResult.Next());
                        return;
                    }
                    else
                    {
                        //按分
                        overTime = overTime.AddMinutes(detalmm);
                    }

                }
                else if (props.type == "AUTO")
                {
                    DateTime minTime = DateTime.Now.AddSeconds(10);
                    // 尝试将时间字符串转换为 TimeSpan 对象
                    if (TimeSpan.TryParse(props.dateTime, out TimeSpan timeSpan))
                    {
                        // 获取当前日期
                        DateTime currentDate = DateTime.Now.Date;
                        // 组合当前日期和解析得到的时间部分
                        DateTime combinedDateTime = currentDate + timeSpan;

                        // 比较组合后的时间是否小于当前时间
                        if (combinedDateTime < minTime)
                        {
                            // 如果小于当前时间，则使用明天的日期
                            overTime = currentDate.AddDays(1) + timeSpan;
                        }
                        else
                        {
                            // 如果大于等于当前时间，则使用当天的日期
                            overTime = combinedDateTime;
                        }
                    }
                    else
                    {
                        // 如果时间字符串无法解析，则用默认时间片
                        overTime = minTime;
                    }
                }
                else
                {
                    await context.Print("未知的延时类型");
                }

                context.Provider.GetService<RuleWheelRuner>().PushConcurrentTask(context.Source.DeviceId, async () =>
                {
                    if (context.IsDebug)
                    {
                        await context.Print("延时执行").ConfigureAwait(false);
                    }
                    await context.ExcuteNext(RuleResult.Next()).ConfigureAwait(false);
                }, overTime - DateTime.Now);
            }
        }
    }
}
