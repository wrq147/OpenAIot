
namespace TemplateAction.Label.Element
{
    /// <summary>
    /// 退出循环标签
    /// </summary>
    public class BreakLabel : Template
    {
        public const string LABEL_TYPE = "break";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }
   
        protected override string OnMake(ITemplateContext context)
        {
            context.BreakCount++;
            return string.Empty;
        }
    }
}
