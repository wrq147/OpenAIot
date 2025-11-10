using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public interface IPluginLoader
    {
        /// <summary>
        /// 加载插件
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="path"></param>
        void LoadFrom(Assembly entry, string path);
        /// <summary>
        /// 热更新
        /// </summary>
        /// <param name="pathList"></param>
        void UpdateAssembly(List<string> pathList);
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="ass"></param>
        void UnloadAssembly(Assembly ass);
    }
}
