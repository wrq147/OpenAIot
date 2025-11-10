using System;
using System.Text;
using TemplateAction.Common;

namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 循环标签
    /// </summary>
    public class WhileLabel : CollectionTemplate
    {
        public const string LABEL_TYPE = "while";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }
        protected bool Calculate(ITemplateContext context)
        {
            object result;
            if (!context.CurrentLabel.TryGetParam(TAUtility.CONDITION_EX, out result))
            {
                throw new Exception("while的参数不存在");
            }
            if (result is bool)
            {
                return (bool)result;
            }
            else
            {
                return result == null ? false : true;
            }
        }

        protected override string OnMake(ITemplateContext context)
        {
            try
            {
                StringBuilder tmpBuilder = new StringBuilder();
                while (Calculate(context))
                {
                    LoopChild(context, (c) =>
                    {
                        tmpBuilder.Append(c.MakeHtml(context));
                        return true;
                    });
                    if (context.BreakCount > 0)
                    {
                        context.BreakCount--;
                        break;
                    }
                }
                return tmpBuilder.ToString();
            }
            catch (Exception ex)
            {
                return "循环标签异常:" + ex.Message;
            }
        }
    }
}
