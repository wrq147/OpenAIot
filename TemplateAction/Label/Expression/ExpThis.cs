using System;
namespace TemplateAction.Label.Expression
{
    public class ExpThis : IExpressionItem
    {
        public TAVar Calculate(ITemplateContext context, object scope)
        {
            return new TAVar(context, VarType.Var);
        }
    }
}
