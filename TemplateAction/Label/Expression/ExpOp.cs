using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateAction.Label.Expression
{
    public class ExpOp : IExpressionItem
    {
        private string _op;
        public ExpOp(string op)
        {
            _op = op;
        }
        public override string ToString()
        {
            return _op;
        }
        public TAVar Calculate(ITemplateContext context, object scope)
        {
            return null;
        }
    }
}
