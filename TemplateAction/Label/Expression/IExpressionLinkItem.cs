using System;
namespace TemplateAction.Label.Expression
{
    public interface IExpressionLinkItem: IExpressionItem
    {
        void Assign(ITemplateContext context, object scope, object result);
    }
}
