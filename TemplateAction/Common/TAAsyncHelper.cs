using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Common
{
    /// <summary>
    /// 同步调用异步的帮助类
    /// </summary>
    public static class TAAsyncHelper
    {

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            var res = func();
            return res.Result;
        }

        public static void RunSync(Func<Task> func)
        {
            var res = func();
            res.Wait();
        }

    }
}
