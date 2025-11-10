
using System;
using TemplateAction.Common;
using TemplateAction.Label.Expression;

namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 赋值标签
    /// </summary>
    public class AssignLabel : Template
    {
        public const string LABEL_TYPE = "assign";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }
        public AssignLabel() { }
        protected override string OnMake(ITemplateContext context)
        {
            try
            {
                object tar;
                if (!context.CurrentLabel.TryGetParam(TAUtility.FUN_VAR, out tar, false))
                {
                    //开始输出语句处理
                    object result;
                    if (context.CurrentLabel.TryGetParam(TAUtility.ASSIGN_SRC, out result))
                    {
                        if (result == null) return string.Empty;
                        if(result is string)
                        {
                            //字符串默认html编码输出
                            if(context.CurrentLabel.GetParam<bool>(TAUtility.HTML_ENCODE, true))
                            {
                                return System.Net.WebUtility.HtmlEncode((string)result);
                            }
                            else
                            {
                                return (string)result;
                            }
                        }
                        else
                        {
                            return result.ToString();
                        }
                    }
                }
                else
                {
                    //开始赋值语句处理
                    object result;
                    if (context.CurrentLabel.TryGetParam(TAUtility.ASSIGN_SRC, out result))
                    {
                        ExpLink link = tar as ExpLink;
                        if (link != null)
                        {
                            link.Assign(context, result);
                            return string.Empty;
                        }
                        else
                        {
                            return "赋值语句目标格式错误";
                        }
                    }
                }
                return "=赋值语句的源数据错误";
            }
            catch (Exception ex)
            {
                return "赋值标签异常：" + ex.Message;
            }
        }
    }
}
