using System;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    internal class TAActionTemplateContext : AbstractTemplateContext
    {
        private string mNameSpace;
        private string mController;
        public TAActionTemplateContext(string ns,string controller)
        {
            mNameSpace = ns;
            mController = controller;
        }
        /// <summary>
        /// 模板Include语法调用
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public override string Include(string src)
        {
            string zPath = string.Empty;
            if (src.StartsWith("/"))
            {
                zPath = src;
            }
            else
            {
                zPath = "/" + mNameSpace + "/" + mController + "/" + src;
            }
            TemplateDocument indexTemp = TemplateApp.Instance.LoadViewPage(zPath);
            if (indexTemp == null)
            {
                return "视图不存在";
            }
            return indexTemp.MakeHtml(this);
        }
    }
}
