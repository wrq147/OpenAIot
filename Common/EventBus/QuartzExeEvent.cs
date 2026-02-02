using System;
using System.Collections.Generic;
using TemplateAction.Core;

namespace Common.EventBus
{
    /// <summary>
    /// Quartz任务执行事件
    /// </summary>
    public class QuartzExeEvent : ResponseEvent
    {
        public const string EventKey = "EV.BUS.QUARTZ";
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string MethodParams { get; set; }
        public bool DisConcurrent { get; set; }
        public QuartzContext Context { get; set; }


        /// <summary>
        /// 获取method方法参数相关列表
        /// </summary>
        /// <returns></returns>
        public List<object> GetMethodParams()
        {
            string[] methodParams = this.MethodParams.Split(",", StringSplitOptions.RemoveEmptyEntries);
            List<object> classs = new List<object>();
            for (int i = 0; i < methodParams.Length; i++)
            {
                string str = methodParams[i].Trim();
                // String字符串类型，包含'
                if (str.StartsWith("'"))
                {
                    classs.Add(str.Replace("'", ""));
                }
                // boolean布尔类型，等于true或者false
                else if (string.Equals(str, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(str, "false", StringComparison.OrdinalIgnoreCase))
                {
                    classs.Add(bool.Parse(str));
                }
                // long长整形，包含L
                else if (str.StartsWith("L"))
                {
                    classs.Add(long.Parse(str.Replace("L", "")));
                }
                // double浮点类型，包含D
                else if (str.StartsWith("D"))
                {
                    classs.Add(double.Parse(str.Replace("D", "")));
                }
                else if (str == "$context")
                {
                    classs.Add(Context);
                }
                else if (str == "$null")
                {
                    classs.Add(null);
                }
                // 其他类型归类为整形
                else
                {
                    classs.Add(int.Parse(str));
                }
            }
            return classs;
        }
    }

    public class QuartzExeResponse : EvtResponse
    {
        public bool IsSuccess()
        {
            return this.Code == Constants.SUCCESS_CODE;
        }
        public static QuartzExeResponse Success()
        {
            QuartzExeResponse response = new QuartzExeResponse();
            response.Code = Constants.SUCCESS_CODE;
            response.IsDone = true;
            return response;
        }
        public static QuartzExeResponse Error(int code, string message)
        {
            QuartzExeResponse response = new QuartzExeResponse();
            response.Message = message;
            response.Code = code;
            response.IsDone = true;
            return response;
        }

    }
    public class QuartzContext
    {
        public DateTimeOffset? PreviousFireTimeUtc { get; set; }
        public DateTimeOffset? ScheduledFireTimeUtc { get; set; }
    }
}
