using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common.EventBus
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public static class BusUtility
    {
        /// <summary>
        /// 调用总线上的RPC
        /// </summary>
        /// <param name="name">方法名称</param>
        /// <param name="data">参数</param>
        /// <returns></returns>
        public static async Task<CallResponse> Call(string name, object data)
        {
            return await TAEventDispatcher.Instance.DispathWait<CallEvent, CallResponse>(CallEvent.EventKey, CallEvent.Create(name, data));
        }
        /// <summary>
        /// 触发总线上的事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task Dispatch(string name, object data)
        {
            await TAEventDispatcher.Instance.Dispatch(BusEvent.EventKey, BusEvent.Create(name, data));
        }

        /// <summary>
        /// 触发执行定时任务
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="methodParams"></param>
        /// <param name="context"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public static async Task Trigger(string className, string methodName, string methodParams, QuartzContext context, long jobId)
        {
            QuartzExeEvent exeEvt = new QuartzExeEvent();
            exeEvt.ClassName = className;
            exeEvt.MethodName = methodName;
            exeEvt.MethodParams = methodParams;
            exeEvt.DisConcurrent = false;
            exeEvt.Context = context;
            exeEvt.JobId = jobId;
            string moduleName = className.Substring(0, className.IndexOf('.'));
            await TAEventDispatcher.Instance.Dispatch($"{QuartzExeEvent.EventKey}.{moduleName}", exeEvt);
        }


        /// <summary>
        /// 触发执行等待定时任务
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="methodParams"></param>
        /// <param name="context"></param>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public static async Task<QuartzExeResponse> TriggerWait(string className, string methodName, string methodParams, QuartzContext context, long jobId)
        {
            QuartzExeEvent exeEvt = new QuartzExeEvent();
            exeEvt.ClassName = className;
            exeEvt.MethodName = methodName;
            exeEvt.MethodParams = methodParams;
            exeEvt.DisConcurrent = true;
            exeEvt.Context = context;
            exeEvt.JobId = jobId;
            string moduleName = className.Substring(0, className.IndexOf('.'));
            return await TAEventDispatcher.Instance.DispathWait<QuartzExeEvent, QuartzExeResponse>($"{QuartzExeEvent.EventKey}.{moduleName}", exeEvt);
        }
    }
}
