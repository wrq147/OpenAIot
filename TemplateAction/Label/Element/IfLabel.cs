using System;
using System.Text;
using TemplateAction.Common;

namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 条件判断标签
    /// </summary>
    public class IfLabel : CollectionTemplate
    {
        public const string LABEL_TYPE = "if";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }
        protected bool ElseIf(ITemplateContext context)
        {
            if (context.CurrentLabel.ParamCount <= 0) return true;

            object result;
            if (!context.CurrentLabel.TryGetParam(TAUtility.CONDITION_EX, out result))
            {
                throw new Exception("else if参数不存在");
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

        protected virtual string OnFinalHtml(bool condition, ITemplateContext context)
        {
            StringBuilder tmpBuilder = new StringBuilder();
            if (condition)
            {
                LoopChild(context, (c) =>
                {
                    if (c is ElseLabel)
                    {
                        return false;
                    }
                    else
                    {
                        tmpBuilder.Append(c.MakeHtml(context));
                        return true;
                    }
                });
            }
            else
            {
                bool startApp = false;
                LoopChild(context, (c) =>
                {
                    if (c is ElseLabel)
                    {
                        if (startApp)
                        {
                            return false;
                        }
                        c.MakeHtml(context);
                        IProxyLabel pre = context.CurrentLabel;
                        context.CurrentLabel = new ProxyLabel(context, c);
                        startApp = ElseIf(context);
                        context.CurrentLabel = pre;
                    }
                    else
                    {
                        if (startApp)
                        {
                            tmpBuilder.Append(c.MakeHtml(context));
                        }
                    }
                    return true;
                });
            }

            return tmpBuilder.ToString();
        }

        /// <summary>
        /// 生成最终的HTML代码
        /// </summary>
        protected override string OnMake(ITemplateContext context)
        {
            try
            {
                object result;
                if (!context.CurrentLabel.TryGetParam(TAUtility.CONDITION_EX, out result))
                {
                    return "if的参数不存在";
                }
                if (result is bool)
                {
                    return OnFinalHtml((bool)result, context);
                }
                else
                {
                    return OnFinalHtml(result == null ? false : true, context);
                }
            
            }
            catch (Exception ex)
            {
                return "if标签异常！" + ex.Message;
            }
        }

    }
}
