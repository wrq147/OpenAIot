using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class ApiExplorerOptions
    {

        public Func<ControllerNode, ActionNode,string> RoutePathFun{ get; set; }

    }
}
