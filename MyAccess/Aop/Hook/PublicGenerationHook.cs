using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace MyAccess.Aop
{
    public class PublicGenerationHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return methodInfo.IsPublic;
        }
    }
}
