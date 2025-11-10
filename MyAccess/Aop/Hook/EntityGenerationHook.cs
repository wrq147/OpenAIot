using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace MyAccess.Aop
{
    public class EntityGenerationHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }
        private bool IsSetter(MethodInfo method)
        {
            return method.IsSpecialName && method.Name.StartsWith("set_", StringComparison.Ordinal);
        }
        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return IsSetter(methodInfo) && methodInfo.IsPublic;
        }
    }
}
