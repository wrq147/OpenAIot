using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    /// <summary>
    /// 启用数据验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TANetValidAttribute : NodeAttribute
    {
        private int _code;
        public TANetValidAttribute(int code = 600)
        {
            _code = code;
        }
        public override void Config(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            TANetNodeValidExt ext = new TANetNodeValidExt();
            ext.Code = _code;
            node.SetExtra<TANetNodeValidExt>(ext);
        }

    }
}
