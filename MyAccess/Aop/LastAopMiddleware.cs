using Castle.DynamicProxy;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.Aop
{

    public class LastAopMiddleware<T> : IResultMiddleware, IAopMiddleware
    {
        private Func<IInvocation, IInvocationProceedInfo, Task<T>> _proceed;
        public IAopMiddleware Next { get; set; }
        public IAopMiddleware Last { get; set; }
        public T Result { get; set; }
        public LastAopMiddleware(Func<IInvocation, IInvocationProceedInfo, Task<T>> proceed)
        {
            _proceed = proceed;
        }
        public async Task ExcuteAsync(IDbHelp dbhelp, IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            Result = await _proceed(invocation, proceedInfo).ConfigureAwait(false);
        }

        public object GetResult()
        {
            return Result;
        }
    }
    public class LastAopMiddleware : IResultMiddleware, IAopMiddleware
    {
        public IAopMiddleware Next { get; set; }
        public IAopMiddleware Last { get; set; }

        private Func<IInvocation, IInvocationProceedInfo, Task> _proceed;
        public LastAopMiddleware(Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            _proceed = proceed;
        }
        public async Task ExcuteAsync(IDbHelp dbhelp, IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            await _proceed(invocation, proceedInfo).ConfigureAwait(false);
        }

        public object GetResult()
        {
            return null;
        }
    }

}
