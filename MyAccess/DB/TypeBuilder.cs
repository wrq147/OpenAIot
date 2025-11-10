using System;
using System.Reflection.Emit;
using System.Collections.Concurrent;
namespace MyAccess.DB
{
    /// <summary>
    /// IL动态创建实例类
    /// </summary>
    public class TypeBuilder
    {
        /// <summary>
        /// 缓存创建方法
        /// </summary>
        private static ConcurrentDictionary<Type, Delegate> _buildMethodCache = new ConcurrentDictionary<Type, Delegate>();
        /// <summary>
        /// 生成实例创建方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static Delegate BuildMethodCreateInstance<T>()
        {
            Type type = typeof(T);
            DynamicMethod dm = new DynamicMethod(string.Format("_{0:N}", Guid.NewGuid()), type, null);
            var gen = dm.GetILGenerator();
            gen.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            gen.Emit(OpCodes.Ret);
            return dm.CreateDelegate(typeof(Func<T>));
        }
        /// <summary>
        /// 生成实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            Type type = typeof(T);
            var func = _buildMethodCache.GetOrAdd(type, (t) =>
            {
                return BuildMethodCreateInstance<T>();
            });
            Func<T> invokeFunc = func as Func<T>;
            if (invokeFunc != null)
            {
                return invokeFunc.Invoke();
            }
            return default(T);
        }
    }
}
