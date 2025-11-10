using System;
using System.Collections.Generic;

namespace TemplateAction.Label.Expression
{
    public class ExpLink : IExpressionItem
    {
        private LinkedList<IExpressionLinkItem> _children = new LinkedList<IExpressionLinkItem>();
        public ExpLink() { }
        public void AddLink(IExpressionLinkItem exp)
        {
            _children.AddLast(exp);
        }
        /// <summary>
        /// 赋值
        /// </summary>
        /// <param name="result"></param>
        public void Assign(ITemplateContext context, object result)
        {
            if (_children.Count > 0)
            {
                object scope = null;
                LinkedListNode<IExpressionLinkItem> node = _children.First;
                while (node != _children.Last)
                {
                    scope = node.Value.Calculate(context, scope);
                }
                node.Value.Assign(context, scope, result);
            }
        }
        public TAVar Calculate(ITemplateContext context, object scope)
        {
            scope = null;
            foreach (IExpressionItem item in _children)
            {
                scope = item.Calculate(context, scope).Value;
            }
            return new TAVar(scope, VarType.Var);
        }
    }
}
