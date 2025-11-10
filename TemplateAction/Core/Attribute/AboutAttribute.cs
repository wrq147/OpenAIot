using System;
namespace TemplateAction.Core
{
    /// <summary>
    /// 动作关联
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AboutAttribute : NodeControllerAttribute
    {
        /// <summary>
        /// 关联值
        /// </summary>
        private string _About;

        public AboutAttribute(string about = null)
        {
            _About = about;
        }
        private class ControllerAbout
        {
            public string About { get; set; }
        }


        public override void ConfigAction(PluginObject plg, ControllerNode controller, ActionNode node)
        {
            string taboutmodule = null;
            ControllerAbout controllerAbout = controller.GetExtra<ControllerAbout>();
            if (controllerAbout != null)
            {
                taboutmodule = controllerAbout.About;
            }

            if (_About != null)
            {
                if (taboutmodule == null)
                {
                    node.AboutCode = _About;
                }
                else
                {
                    if (_About.StartsWith("/"))
                    {
                        node.AboutCode = _About;

                    }
                    else
                    {
                        if (taboutmodule.EndsWith("/"))
                        {
                            node.AboutCode = taboutmodule + _About;
                        }
                        else
                        {
                            node.AboutCode = taboutmodule + "/" + _About;
                        }
                    }
                }
            }
            else
            {
                if (taboutmodule == null)
                {
                    node.AboutCode = string.Format("/{0}/{1}/{2}", plg.Name, controller.Key, node.Key);
                }
                else
                {
                    if (taboutmodule.EndsWith("/"))
                    {
                        node.AboutCode = taboutmodule + node.Key;
                    }
                    else
                    {
                        node.AboutCode = taboutmodule + "/" + node.Key;
                    }

                }
            }
        }

        public override void ConfigBeforeAction(PluginObject plg, ControllerNode controller)
        {
            if (_About == null)
            {
                controller.SetExtra(new ControllerAbout()
                {
                    About = string.Format("/{0}/{1}", plg.Name, controller.Key)
                });

            }
            else
            {
                controller.SetExtra(new ControllerAbout()
                {
                    About = _About
                });
            }
        }

        public override void ConfigAfterAction(PluginObject plg, ControllerNode controller)
        {
        }
    }
}
