using System;
using System.Collections;
using System.Text;
using TemplateAction.Common;

namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 循环处理标签
    /// </summary>
    public class LoopLabel : CollectionTemplate
    {
        public const string LABEL_TYPE = "loop";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }

        /// <summary>
        /// 解析自定义标签内容
        /// </summary>
        protected override string OnMake(ITemplateContext context)
        {
            StringBuilder tmpBuilder = new StringBuilder();
            string _name = string.Empty;
            string _index = string.Empty;
            try
            {
                _name = context.CurrentLabel.GetParam(TAUtility.FOR_NAME, string.Empty);//子项数据别名
                if (string.IsNullOrEmpty(_name))
                {
                    return "name不能为空";
                }
                _index = context.CurrentLabel.GetParam(TAUtility.FOR_INDEX, string.Empty);
                IEnumerable loopList;
                object result;
                //数据源是指定数据名
                if (context.CurrentLabel.TryGetParam(TAUtility.FOR_FROM, out result))
                {
                    loopList = result as IEnumerable;
                    if (loopList == null)
                    {
                        return "数据来源不能为空";
                    }
                }
                else
                {
                    return "数据地址错误";
                }



                //解释数据
                int i = 0;
                foreach (object item in loopList)
                {
                    LoopChild(context, (c) =>
                    {
                        tmpBuilder.Append(c.MakeHtml(context));
                        return true;
                    }, () =>
                    {
                        GenerateReplace(context, item, i, _name, _index);
                    });
                    if (context.BreakCount > 0)
                    {
                        context.BreakCount--;
                        break;
                    }
                    ++i;
                }


                return tmpBuilder.ToString();
            }
            catch (Exception ex)
            {
                return "循环标签出错！" + ex.Message;
            }
        }
        protected void GenerateReplace(ITemplateContext context, object dr, int i, string name, string index)
        {
            context.PushGlobal(name, dr);
            context.PushGlobal(index, i);
        }
    }
}

