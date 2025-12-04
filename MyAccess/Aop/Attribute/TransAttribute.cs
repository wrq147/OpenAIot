using AspectCore.DynamicProxy;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace MyAccess.Aop
{
    public class TransAttribute : AbstractInterceptorAttribute
    {
        public static AsyncLocal<BLLDbStore> ThreadDbHelp = new AsyncLocal<BLLDbStore>();
        private T GetTargetInstance<T>(AspectContext context) where T : class
        {
            if (context == null)
                return null;

            // 场景 1：直接拦截类（非接口）→ context.Implementation 就是目标实例
            if (context.Implementation is T target)
                return target;

            // 场景 2：拦截接口（通过 DI 注册接口+实现类）→ context.Proxy 是真实实现类实例
            if (context.Proxy is T proxyTarget)
                return proxyTarget;

            return null;
        }
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            DBSupport support = GetTargetInstance<DBSupport>(context);
            bool issync = context.IsAsync();
            if (support != null && support.help != null)
            {
                bool disableTrans = ThreadDbHelp.Value != null && ThreadDbHelp.Value.ThreadDb != null;
                if (!disableTrans)
                {
                    if (issync)
                    {
                        support.help.BeginTran();
                    }
                    else
                    {
                        await support.help.BeginTranAsync();
                    }
                }

                try
                {
                    await next(context);
                    if (!disableTrans)
                    {
                        if (issync)
                        {
                            support.help.Commit();
                        }
                        else
                        {
                            await support.help.CommitAsync();
                        }
                    }
                }
                finally
                {
                    if (!disableTrans && support.help.IsTrans())
                    {
                        if (issync)
                        {
                            support.help.RollBack();
                        }
                        else
                        {
                            await support.help.RollBackAsync();
                        }
                    }
                }
            }
            else
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    await next(context);
                    if (context.ImplementationMethod.IsReturnValueTask())
                    {
                        ITransReturn tr = context.ReturnValue as ITransReturn;
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
}
