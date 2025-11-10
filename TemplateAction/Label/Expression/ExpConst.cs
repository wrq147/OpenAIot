using System;

namespace TemplateAction.Label.Expression
{
    public class ExpConst : TAVar, IExpressionItem
    {
        public ExpConst(object var, VarType t) : base(var, t)
        {
        }
        public TAVar Calculate(ITemplateContext context, object scope)
        {
            return this;
        }
    }
}
