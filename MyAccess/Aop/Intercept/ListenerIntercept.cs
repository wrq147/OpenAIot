using Castle.DynamicProxy;
using System;

namespace MyAccess.Aop
{
    /// <summary>
    /// 事件分发器用拦截器
    /// </summary>
    public class ListenerIntercept : IInterceptor
    {
        private Action<string, string, object[]> _call;
        private string _name;
        public ListenerIntercept(Action<string, string, object[]> callFun)
        {
            _call = callFun;
        }
        public void Intercept(IInvocation invocation)
        {
            if (string.IsNullOrEmpty(_name))
            {
                string proxyname = invocation.Proxy.GetType().Name;
                _name = proxyname.Substring(0, proxyname.Length - 5);
            }
            _call(_name, invocation.Method.Name, invocation.Arguments);
        }
    }
}
