using System;
using System.Collections.Generic;
namespace TemplateAction.Label.Expression
{
    public class OpMan
    {
        private LinkedList<IOp> linkedList = new LinkedList<IOp>();
        private volatile static OpMan mInstance = null;
        private static readonly object lockHelper = new object();
        private OpMan()
        {
            this.linkedList.AddLast(new ExceptionOp());
        }
        public static OpMan Instance()
        {
            if (mInstance == null)
            {
                lock (lockHelper)
                {
                    if (mInstance == null)
                    {
                        mInstance = new OpMan()
                            .AddLast<NullArithOp>()
                            .AddLast<ValueOp>();
                    }
                }
            }
            return mInstance;
        }
        /// <summary>
        /// 获取第一个单元
        /// </summary>
        internal IOp First
        {
            get
            {
                return this.linkedList.First.Value;
            }
        }
        internal IOp Last
        {
            get { return this.linkedList.Last.Value; }
        }
        /// <summary>
        /// 初始化各单元
        /// </summary>
        /// <returns></returns>
        private OpMan InitOps()
        {
            LinkedListNode<IOp> node = this.linkedList.First;
            while (node.Next != null)
            {
                node.Value.NextOp = node.Next.Value;
                node = node.Next;
            }
            return this;
        }
        /// <summary>
        /// 通过类型查找节点
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        private LinkedListNode<IOp> FindNode<T>() where T : IOp
        {
            LinkedListNode<IOp> node = this.linkedList.First;
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
        /// 是否已存在T类型的单元
        /// </summary>
        /// <typeparam name="T">单元类型</typeparam>
        /// <returns></returns>
        private bool ExistConvert<T>() where T : IOp
        {
            return this.FindNode<T>() != null;
        }



        /// <summary>
        /// 添加一个单元到最前面
        /// </summary>
        /// <typeparam name="T">单元类型</typeparam>
        /// <returns></returns>
        public OpMan AddFrist<T>() where T : IOp
        {
            if (this.ExistConvert<T>() == false)
            {
                T opitem = Activator.CreateInstance<T>();
                this.linkedList.AddFirst(opitem);
            }
            return InitOps();
        }

        /// <summary>
        /// 添加到指定单元之后
        /// </summary>
        /// <typeparam name="TSource">已存在的单元</typeparam>
        /// <typeparam name="TDest">新加入的单元</typeparam>
        /// <returns></returns>
        public OpMan AddBefore<TSource, TDest>()
            where TSource : IOp
            where TDest : IOp
        {
            LinkedListNode<IOp> node = this.FindNode<TSource>();
            if (node != null && this.ExistConvert<TDest>() == false)
            {
                TDest convert = Activator.CreateInstance<TDest>();
                this.linkedList.AddBefore(node, convert);
            }
            return InitOps();
        }

        /// <summary>
        /// 添加到指定单元之后
        /// </summary>
        /// <typeparam name="TSource">已存在的单元</typeparam>
        /// <typeparam name="TDest">新加入的单元</typeparam>
        /// <returns></returns>
        public OpMan AddAfter<TSource, TDest>()
            where TSource : IOp
            where TDest : IOp
        {
            LinkedListNode<IOp> node = this.FindNode<TSource>();
            if (node != null && this.ExistConvert<TDest>() == false)
            {
                TDest convert = Activator.CreateInstance<TDest>();
                this.linkedList.AddAfter(node, convert);
            }
            return InitOps();
        }

        /// <summary>
        /// 添加一个单元到末尾
        /// </summary>
        /// <typeparam name="T">转换单元类型</typeparam>
        /// <returns></returns>
        public OpMan AddLast<T>() where T : IOp
        {
            return this.AddBefore<ExceptionOp, T>();
        }

        /// <summary>
        /// 解绑一个转换单元
        /// </summary>
        /// <typeparam name="T">转换单元类型</typeparam>
        /// <returns></returns>
        public OpMan Remove<T>() where T : IOp
        {
            LinkedListNode<IOp> node = this.FindNode<T>();
            if (node != null)
            {
                this.linkedList.Remove(node);
            }
            return InitOps();
        }

    }
}
