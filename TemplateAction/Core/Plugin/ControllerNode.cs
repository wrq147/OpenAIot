using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 控制器树节点
    /// </summary>
    public class ControllerNode : Node
    {
        private Type mType;
        public Type ControllerType
        {
            get { return mType; }
        }
        private string mPluginName;
        /// <summary>
        /// 所属模块名
        /// </summary>
        public string PluginName
        {
            get { return mPluginName; }
        }


        /// <summary>
        /// 是否执行过配置
        /// </summary>
        public bool IsConfiged
        {
            get;
            private set;
        }
        public ControllerNode(PluginObject plg, Type type)
        {
            mKey = type.Name.Replace("Controller", string.Empty, StringComparison.OrdinalIgnoreCase);
            mPluginName = plg.Name;
            mType = type;
            mDescript = mKey;
            this.IsConfiged = false;
            DesAttribute ad = (DesAttribute)type.GetCustomAttribute(typeof(DesAttribute));
            if (ad != null)
            {
                mDescript = ad.Des;
                mSort = ad.Sort;
            }

            //配置控制器注解
            IEnumerable<NodeAttribute> customAttrs = type.GetCustomAttributes<NodeAttribute>();
            foreach (NodeAttribute a in customAttrs)
            {
                a.Config(plg, this, null);
            }
            InitActions(plg);
            this.IsConfiged = true;
            foreach (NodeAttribute a in customAttrs)
            {
                a.Config(plg, this, null);
            }
        }

        /// <summary>
        /// 初始化动作节点
        /// </summary>
        private void InitActions(PluginObject plg)
        {
            MethodInfo[] methodArray = mType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo method in methodArray)
            {
                //判断方法返回值是否继承IResult
                if (!typeof(IResult).IsAssignableFrom(method.ReturnType))
                {
                    bool iacontinue = true;
                    if (method.ReturnType == typeof(Task))
                    {
                        iacontinue = false;

                    }
                    else if (method.ReturnType.IsGenericType)
                    {
                        if (method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                        {
                            if (method.ReturnType.GenericTypeArguments.Length > 0)
                            {
                                Type gtype = method.ReturnType.GenericTypeArguments[0];
                                if (typeof(IResult).IsAssignableFrom(gtype))
                                {
                                    iacontinue = false;
                                }
                            }
                        }
                    }
                    if (iacontinue)
                    {
                        continue;
                    }
                }
                if (method.IsVirtual || method.IsStatic || method.DeclaringType.Name.Equals("Object"))
                {
                    continue;
                }
                ActionNode an = new ActionNode(plg, this, method);
                AddChildNode(an.Key, an);
            }
        }
    }
}
