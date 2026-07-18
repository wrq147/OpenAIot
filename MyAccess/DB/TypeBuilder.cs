using System;
using System.Linq.Expressions;
using System.Reflection;
namespace MyAccess.DB
{
    /// <summary>
    /// 无参实例创建器
    /// </summary>
    public static class TypeBuilder
    {
        /// <summary>
        /// 泛型静态缓存：每个T对应一个创建委托，仅初始化一次
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        private static class CreatorCache<T>
        {
            // 静态委托：编译期绑定，无反射开销
            public static readonly Func<T> InstanceCreator = CreateCreator();
            /// <summary>
            /// 初始化创建委托（仅执行一次）
            /// </summary>
            private static Func<T> CreateCreator()
            {
                Type type = typeof(T);

                // 1. 值类型：直接返回default(T)，无需构造函数
                if (type.IsValueType)
                {
                    return () => default(T);
                }

                // 2. 引用类型：校验无参构造函数并构建Expression委托
                ConstructorInfo? ctor = type.GetConstructor(Type.EmptyTypes);

                if (ctor == null)
                {
                    throw new MissingMethodException(type.FullName, "无公共无参构造函数，无法创建实例");
                }

                NewExpression newExpr = Expression.New(ctor);
                Expression<Func<T>> lambda = Expression.Lambda<Func<T>>(newExpr);
                return lambda.Compile();
            }
        }

        /// <summary>
        /// 生成实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            return CreatorCache<T>.InstanceCreator.Invoke();
        }
    }
}
