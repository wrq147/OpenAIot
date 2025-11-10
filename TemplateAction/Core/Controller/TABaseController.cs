using System;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    /// <summary>
    /// 基础控制器,所有控制器应该继承这个
    /// </summary>
    public abstract class TABaseController : IController
    {
        private TAAction mAction;
        protected ITAContext Context { get { return mAction.Context; } }
        protected ITARequest Request { get { return mAction.Context.Request; } }
        protected ITAResponse Response { get { return mAction.Context.Response; } }
        protected ITAServiceProvider ServiceProvider { get { return mAction.Context.Application.ServiceProvider; } }
        public virtual void Bind(TAAction handle)
        {
            mAction = handle;
        }
        protected void Define(string key)
        {
            mAction.AddGlobal(key, string.Empty);
        }
        /// <summary>
        /// 设置全局变量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        protected void SetGlobal(string key, object val)
        {
            mAction.AddGlobal(key, val);
        }
        /// <summary>
        /// 同步重定向
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        protected virtual IResult Redirect(string controller, string action, string ns = "")
        {
            if (string.IsNullOrEmpty(ns))
            {
                ns = mAction.NameSpace;
            }
            TAAction tmpac = mAction;
            TAActionBuilder builder = mAction.Context.Application.CreateTAActionBuilder(mAction.Context, ns, controller, action);
            IResult rt = builder.Excute();
            Bind(tmpac);
            return rt;
        }

        /// <summary>
        /// 异步重定向
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="ns"></param>
        /// <returns></returns>
        protected virtual async Task<IResult> RedirectAsync(string controller, string action, string ns = "")
        {
            if (string.IsNullOrEmpty(ns))
            {
                ns = mAction.NameSpace;
            }
            TAAction tmpac = mAction;
            TAActionBuilder builder = mAction.Context.Application.CreateTAActionBuilder(mAction.Context, ns, controller, action);
            IResult t = await builder.ExcuteAsync();
            Bind(tmpac);
            return t;
        }
        /// <summary>
        /// 返回默认视图
        /// </summary>
        /// <returns></returns>
        protected virtual ViewResult View()
        {
            return View(mAction.Controller, mAction.Action);
        }
        protected StreamResult Stream(string filename, byte[] data)
        {
            return new StreamResult(filename, data);
        }
        /// <summary>
        /// 返回指定视图
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="moduleNode"></param>
        /// <returns></returns>
        protected ViewResult View(string moduleName, string moduleNode)
        {
            return new ViewResult(moduleName, moduleNode);
        }
        protected virtual ViewResult View(string moduleNode)
        {
            return new ViewResult(mAction.Controller, moduleNode);
        }
        protected PngResult Png(byte[] data)
        {
            return new PngResult(data);
        }
        protected GifResult Gif(byte[] data)
        {
            return new GifResult(data);
        }
        protected FileResult File(string path)
        {
            return new FileResult(path);
        }

        protected TextResult Content(string content)
        {
            return new TextResult(content);
        }

        /// <summary>
        /// 控制器异常
        /// </summary>
        /// <param name="ex">异常类</param>
        /// <returns></returns>
        public virtual IResult Exception(int code, Exception ex)
        {
            return new TextResult(string.Format("{0}异常:{1}", ex.Source, ex.Message));
        }
        /// <summary>
        /// 路由执行
        /// </summary>
        /// <returns></returns>
        public virtual async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            MethodInfo method = ac.ActionNode.Method;
            object rt = method.Invoke(this, parameters);
            if (ac.ActionNode.Async)
            {
                Task t = rt as Task;
                await t;
                IResult waitRt = t.GetType().GetProperty("Result").GetValue(t, null) as IResult;
                return waitRt;
            }
            else
            {
                return rt as IResult;
            }

        }
        public TAAction IntentAction
        {
            get { return mAction; }
        }
    }
}
