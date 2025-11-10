using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// 动作树节点
    /// </summary>
    public class ActionNode : Node
    {
        private MethodInfo mMethod;
        public MethodInfo Method
        {
            get { return mMethod; }
        }

        private byte _allowHttpMethod;
        public byte AllowHttpMethod
        {
            get { return _allowHttpMethod; }
        }

        /// <summary>
        /// 判断httpmethod是否可使用该节点
        /// </summary>
        /// <param name="httpmethod"></param>
        /// <returns></returns>
        public bool JudgeHttpMethod(string httpmethod)
        {
            if (_allowHttpMethod == 0) return true;
            httpmethod = httpmethod.ToLower();
            switch (httpmethod)
            {
                case "get":
                    return (_allowHttpMethod & (byte)NodeHttpMethod.Get) != 0;
                case "post":
                    return (_allowHttpMethod & (byte)NodeHttpMethod.Post) != 0;
                case "put":
                    return (_allowHttpMethod & (byte)NodeHttpMethod.Put) != 0;
                case "delete":
                    return (_allowHttpMethod & (byte)NodeHttpMethod.Delete) != 0;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 增加允许的HttpMethod
        /// </summary>
        /// <param name="httpMethod"></param>
        public void AddHttpMethod(NodeHttpMethod httpMethod)
        {
            _allowHttpMethod |= (byte)httpMethod;
        }
        private bool _async;
        /// <summary>
        /// 判断是否为异步方法
        /// </summary>
        public bool Async
        {
            get { return _async; }
        }
        /// <summary>
        /// 关联代码
        /// </summary>
        public string AboutCode { get; set; }

        public ActionNode(PluginObject plg, ControllerNode controller, MethodInfo method)
        {
            _allowHttpMethod = 0;
            mKey = method.Name;
            mMethod = method;
            mDescript = mKey;
            //判断是否为异步
            var attrib = method.GetCustomAttribute<AsyncStateMachineAttribute>();
            _async = attrib != null;

            //描述注解
            DesAttribute ad = (DesAttribute)method.GetCustomAttribute(typeof(DesAttribute));
            if (ad != null)
            {
                mDescript = ad.Des;
                mSort = ad.Sort;
            }

            //配置注解
            IEnumerable<NodeAttribute> customAttrs = method.GetCustomAttributes<NodeAttribute>();
            foreach (NodeAttribute a in customAttrs)
            {
                a.Config(plg, controller, this);
            }

        }
    }

    public enum NodeHttpMethod : byte
    {
        Get = 1,
        Post = 2,
        Delete = 4,
        Put = 8
    }

}
