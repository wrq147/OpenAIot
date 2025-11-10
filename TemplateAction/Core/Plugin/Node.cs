using System;
using System.Collections.Generic;

namespace TemplateAction.Core
{
    public abstract class Node
    {
        protected string mDescript = string.Empty;
        public string Descript
        {
            get { return mDescript; }
        }

        protected int mSort;
        public int Sort
        {
            get
            {
                return mSort;
            }
        }

        /// <summary>
        /// 节点键
        /// </summary>
        protected string mKey = string.Empty;
        public string Key
        {
            get { return mKey; }
        }


        protected Dictionary<string, Node> mChildrens = new Dictionary<string, Node>();
        public Dictionary<string, Node> Childrens
        {
            get { return mChildrens; }
        }

        public Node GetChildNode(string key)
        {
            Node rtVal = null;
            if (mChildrens.TryGetValue(key.ToLower(), out rtVal))
            {
                return rtVal;
            }
            return null;
        }
        public void AddChildNode(string key, Node n)
        {
            mChildrens[key.ToLower()] = n;
        }

        private Dictionary<string, object> _extra;
        /// <summary>
        /// 添加额外信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public void SetExtra<T>(T obj) where T : class
        {
            if (_extra == null)
            {
                _extra = new Dictionary<string, object>();
            }
            _extra[typeof(T).FullName] = obj;
        }
        /// <summary>
        /// 获取额外信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetExtra<T>() where T : class
        {
            if (_extra == null) return null;
            object rt;
            if (_extra.TryGetValue(typeof(T).FullName, out rt))
            {
                return (T)rt;
            }
            else
            {
                return null;
            }
        }
    }
}
