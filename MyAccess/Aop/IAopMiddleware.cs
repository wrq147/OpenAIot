using Castle.DynamicProxy;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    public interface IAopMiddleware
    {
        IAopMiddleware Next { get; set; }
        public Task ExcuteAsync(IDbHelp dbhelp, IInvocation invocation, IInvocationProceedInfo proceedInfo);
    }
    public interface IResultMiddleware
    {
        object GetResult();
    }
}
