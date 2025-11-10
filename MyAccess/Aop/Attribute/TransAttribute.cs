using Castle.DynamicProxy;
using System;
using MyAccess.DB;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MyAccess.Aop
{

    [AttributeUsage(AttributeTargets.Method)]
    public class TransAttribute : AbstractAopAttr
    {
        public TransAttribute()
        {
        }
        public override async Task ExcuteAsync(IDbHelp dbhelp, IInvocation invocation, IInvocationProceedInfo proceedInfo)
        {
            var attrib = invocation.Method.GetCustomAttribute<AsyncStateMachineAttribute>();
            bool issync = attrib == null;
            if (dbhelp != null)
            {
                bool disableTrans = BLLIntercept.ThreadDbHelp.Value != null && BLLIntercept.ThreadDbHelp.Value.ThreadDb != null;
                if (!disableTrans)
                {
                    if (issync)
                    {
                        dbhelp.BeginTran();
                    }
                    else
                    {
                        await dbhelp.BeginTranAsync();
                    }
                }

                try
                {
                    await Next.ExcuteAsync(dbhelp, invocation, proceedInfo);
                    if (!disableTrans)
                    {
                        if (issync)
                        {
                            dbhelp.Commit();
                        }
                        else
                        {
                            await dbhelp.CommitAsync();
                        }
                    }

                }
                finally
                {
                    if (!disableTrans && dbhelp.IsTrans())
                    {
                        if (issync)
                        {
                            dbhelp.RollBack();
                        }
                        else
                        {
                            await dbhelp.RollBackAsync();
                        }
                    }
                }
            }
            else
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await Next.ExcuteAsync(dbhelp, invocation, proceedInfo);

                    ITransReturn tr = Last.GetResult() as ITransReturn;
                    if (tr != null && !tr.IsSuccess())
                    {
                        return;
                    }
                    // 完成
                    if (issync)
                    {
                        scope.Complete();
                    }
                    else
                    {
                        await scope.CompleteAsync();
                    }
                }
            }

        }


    }
}
