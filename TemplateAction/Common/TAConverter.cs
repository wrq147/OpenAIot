using System;
using System.Collections.Generic;
using TemplateAction.Common.Converts;

namespace TemplateAction.Common
{
    public class TAConverter
    {
        /// <summary>
        /// 转换器静态实例
        /// </summary>
        private volatile static TAConverter _instance = null;
        private static readonly object _lock = new object();
        /// <summary>
        /// 获取转换单元操控对象        
        /// </summary>
        private ContertItems Items { get; }
        public static TAConverter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TAConverter();
                        }
                    }
                }
                return _instance;
            }
        }
        public TAConverter()
        {
            this.Items = new ContertItems()
                .AddLast<NullConvert>()
                .AddLast<NoConvert>()
                .AddLast<NullableConvert>()
                .AddLast<TimeConvert>()
                .AddLast<SimpleContert>()
                .AddLast<DictionaryConvert>()
                .AddLast<ArrayConvert>()
                .AddLast<ListConvert>();
        }
        public static T Cast<T>(object value)
        {
            return Instance.Convert<T>(value);
        }
        public static T Cast<T>(object value, T def)
        {
            try
            {
                return Cast<T>(value);
            }
            catch (NotSupportedException)
            {
                return def;
            }
        }
        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <typeparam name="T">要转换的目标类型</typeparam>
        /// <param name="value">要转换的值</param>    
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>转换后的值</returns>
        public T Convert<T>(object value)
        {
            return (T)this.Convert(value, typeof(T));
        }

        /// <summary>
        /// 转换为目标类型
        /// </summary>
        /// <param name="value">要转换的值</param>
        /// <param name="targetType">要转换的目标类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="Exception"></exception>
        /// <returns>转换后的值</returns>
        public object Convert(object value, Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            return this.Items.First.Convert(value, targetType);
        }
        /// <summary>
        /// 转换并返回转换是否成功
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryConvert(object value, Type targetType, out object result)
        {
            try
            {
                result = Convert(value, targetType);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
        public class ContertItems
        {
            /// <summary>
            /// 转换单元列表
            /// </summary>           
            private readonly LinkedList<ITAConvert> linkedList = new LinkedList<ITAConvert>();

            /// <summary>
            /// 获取第一个转换单元
            /// </summary>
            internal ITAConvert First
            {
                get
                {
                    return this.linkedList.First.Value;
                }
            }

            /// <summary>
            /// 转换单元合集
            /// </summary>
            /// <param name="converter">转换器实例</param>
            public ContertItems()
            {
                this.linkedList.AddLast(new NotSupportedConvert());
            }

            /// <summary>
            /// 通过类型查找节点
            /// </summary>
            /// <typeparam name="T">类型</typeparam>
            /// <returns></returns>
            private LinkedListNode<ITAConvert> FindNode<T>() where T : ITAConvert
            {
                LinkedListNode<ITAConvert> node = this.linkedList.First;
                while (node != null)
                {
                    if (node.Value.GetType() == typeof(T))
                    {
                        return node;
                    }
                    node = node.Next;
                }
                return null;
            }

            /// <summary>
            /// 是否已存在T类型的转换单元
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            private bool ExistConvert<T>() where T : ITAConvert
            {
                return this.FindNode<T>() != null;
            }

            /// <summary>
            /// 初始化各转换单元
            /// </summary>
            /// <returns></returns>
            private ContertItems ReInitItems()
            {
                LinkedListNode<ITAConvert> node = this.linkedList.First;
                while (node.Next != null)
                {
                    node.Value.NextConvert = node.Next.Value;
                    node = node.Next;
                }
                return this;
            }

            /// <summary>
            /// 添加一个转换单元到最前面
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddFrist<T>() where T : ITAConvert
            {
                if (this.ExistConvert<T>() == false)
                {
                    T convert = Activator.CreateInstance<T>();
                    this.linkedList.AddFirst(convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">已存在的转换单元</typeparam>
            /// <typeparam name="TDest">新加入的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddBefore<TSource, TDest>()
                where TSource : ITAConvert
                where TDest : ITAConvert
            {
                LinkedListNode<ITAConvert> node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    TDest convert = Activator.CreateInstance<TDest>();
                    this.linkedList.AddBefore(node, convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加到指定转换单元之后
            /// </summary>
            /// <typeparam name="TSource">已存在的转换单元</typeparam>
            /// <typeparam name="TDest">新加入的转换单元</typeparam>
            /// <returns></returns>
            public ContertItems AddAfter<TSource, TDest>()
                where TSource : ITAConvert
                where TDest : ITAConvert
            {
                LinkedListNode<ITAConvert> node = this.FindNode<TSource>();
                if (node != null && this.ExistConvert<TDest>() == false)
                {
                    TDest convert = Activator.CreateInstance<TDest>();
                    this.linkedList.AddAfter(node, convert);
                }
                return this.ReInitItems();
            }

            /// <summary>
            /// 添加一个转换单元到末尾
            /// </summary>
            /// <typeparam name="T">转换单元类型</typeparam>
            /// <returns></returns>
            public ContertItems AddLast<T>() where T : ITAConvert
            {
                return this.AddBefore<NotSupportedConvert, T>();
            }



        }
    }
}
