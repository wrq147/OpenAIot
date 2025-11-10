using System;
namespace TemplateAction.Label.Expression
{
    public interface IExpressionItem
    {
        TAVar Calculate(ITemplateContext context,object scope);
    }
}
