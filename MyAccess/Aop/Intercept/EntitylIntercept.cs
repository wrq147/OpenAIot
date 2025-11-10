using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace MyAccess.Aop
{
    /// <summary>
    /// 实体用拦截器
    /// </summary>
    public class EntitylIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            MethodInfo mi = invocation.MethodInvocationTarget;
            IBaseEntity be = invocation.InvocationTarget as IBaseEntity;
            if (be != null)
            {
                if (be.EnableRecord())
                {
                    PropertyInfo pi = invocation.TargetType.GetProperty(mi.Name.Substring(4));
                    if (pi != null)
                    {
                        be.AddProperty(pi);
                    }
                }
            }

            invocation.Proceed();
        }
    }
}
