using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
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
    }
}
