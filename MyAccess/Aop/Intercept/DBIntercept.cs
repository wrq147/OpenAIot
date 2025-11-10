using Castle.DynamicProxy;
using MyAccess.Aop.DAL;
using MyAccess.DB;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    /// <summary>
    /// 数据链路层拦截器
    /// </summary>
    public class DBIntercept : AsyncInterceptorBase
    {
        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            DBSupport support = invocation.InvocationTarget as DBSupport;
            if (support != null)
            {
                var attrib = invocation.Method.GetCustomAttribute<AsyncStateMachineAttribute>();
                DbHelp dbHelp = null;
                DbHelp threadDB = null;
                if (BLLIntercept.ThreadDbHelp.Value != null)
                {
                    threadDB = BLLIntercept.ThreadDbHelp.Value.ThreadDb;
                    dbHelp = support.InitDB(threadDB);
                    if (threadDB == null)
                    {
                        if (attrib != null)
                        {
                            await dbHelp.BeginTranAsync();
                        }
                        else
                        {
                            dbHelp.BeginTran();
                        }
                        if (dbHelp.IsTrans())
                        {
                            BLLIntercept.ThreadDbHelp.Value.ThreadDb = dbHelp;
                            threadDB = dbHelp;
                        }
                    }
                }
                else
                {
                    dbHelp = support.InitDB(null);
                }

                AbstractAopAttr[] Attributes = (AbstractAopAttr[])invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractAopAttr), true);
                IAopMiddleware firstNode = null;
                IAopMiddleware preNode = null;
                LastAopMiddleware lastNode = new LastAopMiddleware(proceed);
                foreach (AbstractAopAttr attribute in Attributes)
                {
                    attribute.Last = lastNode;
                    if (firstNode == null)
                    {
                        firstNode = attribute;
                        preNode = attribute;
                    }
                    else
                    {
                        preNode.Next = attribute;
                        preNode = attribute;
                    }
                }


                if (firstNode != null)
                {
                    preNode.Next = lastNode;
                    await firstNode.ExcuteAsync(dbHelp, invocation, proceedInfo);
                    if (threadDB == null)
                    {
                        if (attrib != null)
                        {
                            await dbHelp?.CloseAsync();
                        }
                        else
                        {
                            dbHelp?.Close();
                        }
                    }

                }
                else
                {
                    await lastNode.ExcuteAsync(dbHelp, invocation, proceedInfo);
                    if (threadDB == null)
                    {
                        if (attrib != null)
                        {
                            await dbHelp?.CloseAsync();
                        }
                        else
                        {
                            dbHelp?.Close();
                        }
                    }
                }
            }
            else
            {
                await proceed(invocation, proceedInfo);
            }


        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            DBSupport support = invocation.InvocationTarget as DBSupport;
            if (support != null)
            {
                var attrib = invocation.Method.GetCustomAttribute<AsyncStateMachineAttribute>();
                DbHelp dbHelp = null;
                DbHelp threadDB = null;
                if (BLLIntercept.ThreadDbHelp.Value != null)
                {
                    threadDB = BLLIntercept.ThreadDbHelp.Value.ThreadDb;
                    dbHelp = support.InitDB(threadDB);
                    if (threadDB == null)
                    {
                        if (attrib != null)
                        {
                            await dbHelp.BeginTranAsync();
                        }
                        else
                        {
                            dbHelp.BeginTran();
                        }
                        if (dbHelp.IsTrans())
                        {
                            BLLIntercept.ThreadDbHelp.Value.ThreadDb = dbHelp;
                            threadDB = dbHelp;
                        }
                    }
                }
                else
                {
                    dbHelp = support.InitDB(null);
                }
                AbstractAopAttr[] Attributes = (AbstractAopAttr[])invocation.MethodInvocationTarget.GetCustomAttributes(typeof(AbstractAopAttr), true);
                IAopMiddleware firstNode = null;
                IAopMiddleware preNode = null;
                LastAopMiddleware<TResult> lastNode = new LastAopMiddleware<TResult>(proceed);
                foreach (AbstractAopAttr attribute in Attributes)
                {
                    attribute.Last = lastNode;
                    if (firstNode == null)
                    {
                        firstNode = attribute;
                        preNode = attribute;
                    }
                    else
                    {
                        preNode.Next = attribute;
                        preNode = attribute;
                    }
                }

         
                if (firstNode != null)
                {
                    preNode.Next = lastNode;
                    await firstNode.ExcuteAsync(dbHelp, invocation, proceedInfo);
                    if (threadDB == null)
                    {
                        if (attrib != null)
                        {
                            await dbHelp?.CloseAsync();
                        }
                        else
                        {
                            dbHelp?.Close();
                        }
                    }
                    return lastNode.Result;
                }
                else
                {
                    await lastNode.ExcuteAsync(dbHelp, invocation, proceedInfo);
                    if (threadDB == null)
                    {
                        if (attrib != null)
                        {
                            await dbHelp?.CloseAsync();
                        }
                        else
                        {
                            dbHelp?.Close();
                        }
                    }
                    return lastNode.Result;
                }
            }
            else
            {
                return await proceed(invocation, proceedInfo);
            }

        }

    }
}
