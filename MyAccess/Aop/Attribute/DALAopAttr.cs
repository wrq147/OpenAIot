using AspectCore.DynamicProxy;
using MyAccess.DB;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.Aop.Attribute
{
    public class DALAopAttr : AbstractInterceptorAttribute
    {
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
            if (support == null)
            {
                await next(context); // 非 DBSupport 实例，直接执行目标方法
                return;
            }

            // 1. 数据库连接初始化、事务控制
            DbHelp dbHelp = null;
            DbHelp threadDB = null;
            bool issync = context.IsAsync();
            if (TransAttribute.ThreadDbHelp.Value != null)
            {
                threadDB = TransAttribute.ThreadDbHelp.Value.ThreadDb;
                dbHelp = support.InitDB(threadDB);

                if (threadDB == null)
                {
                    if (issync)
                        await dbHelp.BeginTranAsync();
                    else
                        dbHelp.BeginTran();

                    if (dbHelp.IsTrans())
                    {
                        TransAttribute.ThreadDbHelp.Value.ThreadDb = dbHelp;
                        threadDB = dbHelp;
                    }
                }
            }
            else
            {
                dbHelp = support.InitDB(null);
            }

            try
            {
                await next(context);
            }
            finally
            {
                //关闭数据库连接
                if (threadDB == null)
                {
                    if (issync)
                        await dbHelp?.CloseAsync();
                    else
                        dbHelp?.Close();
                }
            }
        }
    }
}
