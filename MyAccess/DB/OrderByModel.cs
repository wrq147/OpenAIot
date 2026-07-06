using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class AbstractOrderByModel
    {
        protected LambdaExpression _expression;
        public LambdaExpression Expression { get { return _expression; } }

        protected OrderByType _t;
        public OrderByType OrderType { get { return _t; } }
    }
    public class OrderByModel<A> : AbstractOrderByModel
    {
        public OrderByModel(Expression<Func<A, object>> orderExp, OrderByType t)
        {
            _expression = orderExp;
            _t = t;
        }
    }
    public class OrderByModel<A, B> : AbstractOrderByModel
    {
        public OrderByModel(Expression<Func<A, B, object>> orderExp, OrderByType t)
        {
            _expression = orderExp;
            _t = t;
        }
    }
    public class OrderByModel<A, B, C> : AbstractOrderByModel
    {
        public OrderByModel(Expression<Func<A, B, C, object>> orderExp, OrderByType t)
        {
            _expression = orderExp;
            _t = t;
        }
    }
    public class OrderByModel<A, B, C, D> : AbstractOrderByModel
    {
        public OrderByModel(Expression<Func<A, B, C, D, object>> orderExp, OrderByType t)
        {
            _expression = orderExp;
            _t = t;
        }
    }
    public class OrderByModel<A, B, C, D, E> : AbstractOrderByModel
    {
        public OrderByModel(Expression<Func<A, B, C, D, E, object>> orderExp, OrderByType t)
        {
            _expression = orderExp;
            _t = t;
        }
    }
}
