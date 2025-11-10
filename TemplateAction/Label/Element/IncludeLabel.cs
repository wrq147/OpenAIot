using System;
using System.Text;

namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 引入文件标签
    /// </summary>
    public class IncludeLabel : Template
    {
        public const string LABEL_TYPE = "in";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }

        protected override string OnMake(ITemplateContext context)
        {
            try
            {
                string src = context.CurrentLabel.GetParam("src", string.Empty);
                src = src.ToLower();
                return context.Include(src);
            }
            catch (Exception ex)
            {
                return "include标签解释异常:" + ex.Message;
            }
        }
    }
}
