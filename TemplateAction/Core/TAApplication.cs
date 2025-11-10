using System;
using System.Reflection;

namespace TemplateAction.Core
{
    /// <summary>
    /// 控制台程序等应用
    /// </summary>
    public class TAApplication : TAAbstractApplication
    {
        public TAAbstractApplication UsePluginPath(string path)
        {
            base.SetPluginPath(path);
            return this;
        }

        protected override void BeforeInit()
        {
            //激活配置文件
            TAEventDispatcher.Instance.DispathLoadBefore(this);
        }
        public TAApplication Init(string rootpath)
        {
            InitApplication(rootpath, Assembly.GetEntryAssembly());
            return this;
        }

    }
}
