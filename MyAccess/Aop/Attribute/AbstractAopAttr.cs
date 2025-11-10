using Castle.DynamicProxy;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    public abstract class AbstractAopAttr : Attribute, IAopMiddleware
    {
        public IAopMiddleware Next { get; set; }
        public IResultMiddleware Last { get; set; }

        public abstract Task ExcuteAsync(IDbHelp dbhelp, IInvocation invocation, IInvocationProceedInfo proceedInfo);
    }
}
