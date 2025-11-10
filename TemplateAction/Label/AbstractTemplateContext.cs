using System;
using System.Collections.Generic;
using TemplateAction.Common;

namespace TemplateAction.Label
{
    /// <summary>
    /// 标签的上下文
    /// </summary>
    public abstract class AbstractTemplateContext : ITemplateContext
    {
        protected Stack<string> mGlobalReplaceStack = new Stack<string>();
        protected Dictionary<string, object> mGlobalReplace = new Dictionary<string, object>();
        //执行当前标签的代理
        protected IProxyLabel mCurrentLabel;
        public IProxyLabel CurrentLabel
        {
            get { return mCurrentLabel; }
            set { mCurrentLabel = value; }
        }
        protected int mBreakCount;
        public int BreakCount
        {
            get
            {
                return mBreakCount;
            }
            set
            {
                mBreakCount = value;
            }
        }


        public void PushGlobal(string key, object value)
        {
            mGlobalReplace[key] = value;
            if (!mGlobalReplace.ContainsKey(key))
            {
                //标记局部变量
                mGlobalReplaceStack.Push(key);
            }
        }
        public void PopGlobal()
        {
            string tkey = mGlobalReplaceStack.Pop();
            mGlobalReplace.Remove(tkey);
        }
        public int StackCount()
        {
            return mGlobalReplaceStack.Count;
        }
        public T GetGlobal<T>(string key, T def)
        {
            object rtVal = null;
            if (mGlobalReplace.TryGetValue(key, out rtVal))
            {
                return TAConverter.Cast<T>(rtVal);
            }
            else
            {
                return def;
            }
        }

        public object GetGlobal(string key)
        {
            object rtVal = null;
            if (mGlobalReplace.TryGetValue(key, out rtVal))
            {
                return rtVal;
            }
            else
            {
                return null;
            }
        }

        public bool IsDefine(string key)
        {
            return mGlobalReplace.ContainsKey(key);
        }


        public abstract string Include(string src);
    }
}
