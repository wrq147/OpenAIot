using System;
using System.Text;
using TemplateAction.Common;

namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 变量定义标签，将子标签数据定义成指定变量
    /// </summary>
    public class DefineLabel : CollectionTemplate
    {
        public const string LABEL_TYPE = "def";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }
        protected override string OnMake(ITemplateContext context)
        {
            try
            {
                string varName = context.CurrentLabel.GetParam(TAUtility.FUN_VAR, string.Empty);
                StringBuilder tmpBuilder = new StringBuilder();
                LoopChild(context, (c) =>
                {
                    tmpBuilder.Append(c.MakeHtml(context));
                    return true;
                });
                context.PushGlobal(varName, tmpBuilder.ToString());
                return string.Empty;
            }
            catch(Exception ex)
            {
                return "变量定义标签异常：" + ex.Message;
            }
          
        }
    }
}
