using System;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class ViewResult : IResult
    {
        private string mHtml;
        private string mModule;
        private string mNode;
        public override string ToString()
        {
            return mHtml;
        }
        public ViewResult(string html)
        {
            mHtml = html;
        }
        public ViewResult() { }
        public ViewResult(string controller, string action)
        {
            mModule = controller;
            mNode = action;
        }

        /// <summary>
        /// 生成View
        /// </summary>
        /// <returns></returns>
        public string ModuleToString(TAAction ac, string module, string node)
        {
            TASiteApplication tdata = ac.Context.Application;
            string zPath = "/" + ac.NameSpace + "/" + module + "/" + node + TAUtility.FILE_EXT;
            TemplateDocument indexTemp = TemplateApp.Instance.LoadViewPage(zPath);
            if (indexTemp == null)
            {
                return "视图不存在";
            }

            return indexTemp.MakeHtml(ac.TemplateContext);
        }
        public async Task Output(TAAction ac)
        {
            if (mHtml == null)
            {
                if (mModule == null)
                {
                    mModule = ac.Controller;
                }
                if (mNode == null)
                {
                    mNode = ac.Action;
                }
                mHtml = ModuleToString(ac, mModule, mNode);
            }
            ac.Context.Response.ContentType = "text/html";
            await ac.Context.Response.WriteAsync(mHtml);
        }
    }
}
