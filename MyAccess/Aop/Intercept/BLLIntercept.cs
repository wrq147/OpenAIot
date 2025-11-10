using Castle.DynamicProxy;
using MyAccess.DB;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    /// <summary>
    /// 业务层拦截器
    /// </summary>
    public class BLLIntercept : AsyncInterceptorBase
    {
        public static AsyncLocal<BLLDbStore> ThreadDbHelp = new AsyncLocal<BLLDbStore>();
        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
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
            if (ThreadDbHelp.Value != null)
            {
                await new LastAopMiddleware(proceed).ExcuteAsync(null, invocation, proceedInfo);
            }
            else
            {
                if (firstNode != null)
                {
                    preNode.Next = lastNode;
                    await firstNode.ExcuteAsync(null, invocation, proceedInfo);
                }
                else
                {
                    await new LastAopMiddleware(proceed).ExcuteAsync(null, invocation, proceedInfo);
                }
            }
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
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
            if (ThreadDbHelp.Value != null)
            {
                await lastNode.ExcuteAsync(null, invocation, proceedInfo);
                return lastNode.Result;
            }
            else
            {
                if (firstNode != null)
                {
                    preNode.Next = lastNode;
                    await firstNode.ExcuteAsync(null, invocation, proceedInfo);
                    return lastNode.Result;
                }
                else
                {
                    await lastNode.ExcuteAsync(null, invocation, proceedInfo);
                    return lastNode.Result;
                }

            }

        }


    }

    public class BLLDbStore
    {
        public DbHelp ThreadDb { get; set; }
        public long StoreId { get; set; }
    }

    public class BLLTranScope : IDisposable
    {
        private bool _isComplete;
        private long _instanceId;
        private static long IncreaseId = 0;
        public BLLTranScope()
        {
            if (BLLIntercept.ThreadDbHelp.Value == null)
            {
                _isComplete = false;
                _instanceId = Interlocked.Increment(ref IncreaseId);
                BLLIntercept.ThreadDbHelp.Value = new BLLDbStore()
                {
                    StoreId = _instanceId
                };
            }
        }
        ~BLLTranScope()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
        }
        private void Dispose(bool disposing)
        {
            if (_isComplete == false)
            {
                RollBack();
            }
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }
        public void RollBack()
        {
            if (BLLIntercept.ThreadDbHelp.Value != null && BLLIntercept.ThreadDbHelp.Value.StoreId == _instanceId)
            {
                DbHelp db = BLLIntercept.ThreadDbHelp.Value.ThreadDb;
                if (db != null)
                {
                    db.RollBack();
                }
                BLLIntercept.ThreadDbHelp.Value = null;
            }
        }

        public void Complete()
        {
            if (BLLIntercept.ThreadDbHelp.Value != null && BLLIntercept.ThreadDbHelp.Value.StoreId == _instanceId)
            {
                DbHelp db = BLLIntercept.ThreadDbHelp.Value.ThreadDb;
                if (db != null)
                {
                    db.Commit();
                    _isComplete = true;
                }
                BLLIntercept.ThreadDbHelp.Value = null;
            }
        }
        public async Task CompleteAsync()
        {
            if (BLLIntercept.ThreadDbHelp.Value != null && BLLIntercept.ThreadDbHelp.Value.StoreId == _instanceId)
            {
                DbHelp db = BLLIntercept.ThreadDbHelp.Value.ThreadDb;
                if (db != null)
                {
                    await db.CommitAsync();
                    _isComplete = true;
                }
                BLLIntercept.ThreadDbHelp.Value = null;
            }
        }
    }
}
