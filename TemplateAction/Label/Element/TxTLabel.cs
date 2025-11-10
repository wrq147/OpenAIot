using System;

namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 非标签数据被解释成Text标签
    /// </summary>
    public class TxTLabel : Template
    {
        public const string LABEL_TYPE = "text";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }
        protected string mContent;
        public string Content
        {
            get { return mContent; }
        }
        public TxTLabel(string content)
        {
            mContent = content;
        }
        protected override string OnMake(ITemplateContext context)
        {
            return mContent;
        }
    }
}
