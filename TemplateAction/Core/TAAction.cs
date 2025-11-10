using System;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 代表Action
    /// </summary>
    public class TAAction : ITemplatePath, ILifetimeFactory
    {

        private string mNameSpace;
        public string NameSpace
        {
            get { return mNameSpace; }
        }
        private string mController;
        public string Controller
        {
            get { return mController; }
        }
        private string mAction;
        public string Action
        {
            get { return mAction; }
        }
        private ITAContext mContext;
        public ITAContext Context
        {
            get
            {
                return mContext;
            }
        }
        private ITemplateContext mTemplateContext;
        public ITemplateContext TemplateContext
        {
            get
            {
                _InitTemplateContext();
                return mTemplateContext;
            }
        }

        private TAObjectCollection _extparams;
        public TAObjectCollection ExtParams
        {
            get { return _extparams; }
        }
        private ControllerNode _controllerNode;
        public ControllerNode ControllerNode
        {
            get { return _controllerNode; }
        }
        private ActionNode _node;
        public ActionNode ActionNode
        {
            get { return _node; }
        }
        /// <summary>
        /// 获取或设置当前请求的异常处理
        /// </summary>
        public Func<int, Exception, IResult> ExceptionFun { get; internal set; }

        private static AsyncLocal<TAAction> _current = new AsyncLocal<TAAction>();
        /// <summary>
        /// 当前Action
        /// </summary>
        public static TAAction Current
        {
            get
            {
                return _current.Value;
            }
        }
        public TAAction(ITAContext context, ControllerNode controller, ActionNode action, TAObjectCollection ext)
        {
            mContext = context;
            mTemplateContext = null;
            _extparams = ext;
            _controllerNode = controller;
            _node = action;
            mNameSpace = controller.PluginName;
            mController = controller.Key;
            mAction = action.Key;
            _current.Value = this;
        }
        private void _InitTemplateContext()
        {
            if (mTemplateContext == null)
            {
                mTemplateContext = new TAActionTemplateContext(mNameSpace, mController);
            }
        }
        public void AddGlobal(string key, object value)
        {
            _InitTemplateContext();
            mTemplateContext.PushGlobal(key, value);
        }
        public T Global<T>(string key, T def)
        {
            _InitTemplateContext();
            return mTemplateContext.GetGlobal(key, def);
        }
        public bool IsDefine(string key)
        {
            _InitTemplateContext();
            return mTemplateContext.IsDefine(key);
        }
        /// <summary>
        /// 执行Action拦截器中间件
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> Excute()
        {
            try
            {
                return await mContext.Application.Filters.Excute(this);
            }
            catch (Exception ex)
            {
                return new TextResult(ex.Message);
            }

        }

        public object GetValue(IInstanceFactory instanceFactory, Type serviceType, ServiceDescriptor sd)
        {
            if (serviceType == typeof(ITAContext))
            {
                return mContext;
            }
            string tkey = serviceType.FullName;
            ProxyFactory factory = null;
            if (sd != null)
            {
                factory = sd.Factory;
                if (factory != null)
                {
                    tkey += sd.GetHashCode().ToString();
                }
            }

            if (mContext.Items.Contains(tkey))
            {
                return mContext.Items[tkey];
            }
            else
            {
                object target = instanceFactory.CreateServiceInstance(serviceType, factory, this);
                mContext.Items[tkey] = target;
                return target;
            }
        }
    }
}
