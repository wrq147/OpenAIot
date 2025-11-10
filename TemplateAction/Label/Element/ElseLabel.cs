
namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 条件判断标签的else子标签
    /// </summary>
    public class ElseLabel : Template
    {
        public const string LABEL_TYPE = "else";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }
 
        protected override string OnMake(ITemplateContext context)
        {
            return string.Empty;
        }
    }
}
