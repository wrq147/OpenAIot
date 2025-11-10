using System;

namespace TemplateAction.Label.Expression
{
    public abstract class AbstractOp : IOp
    {
        public IOp NextOp { get; set; }

        public virtual TAVar Add(TAVar a, TAVar b)
        {
            return NextOp.Add(a, b);
        }

        public virtual TAVar And(TAVar a, TAVar b)
        {
            return NextOp.And(a, b);
        }

        public virtual TAVar Div(TAVar a, TAVar b)
        {
            return NextOp.Div(a, b);
        }

        public virtual TAVar Eq(TAVar a, TAVar b)
        {
            return NextOp.Eq(a, b);
        }

        public virtual TAVar Ge(TAVar a, TAVar b)
        {
            return NextOp.Ge(a, b);
        }

        public virtual TAVar Gt(TAVar a, TAVar b)
        {
            return NextOp.Gt(a, b);
        }

        public virtual TAVar Le(TAVar a, TAVar b)
        {
            return NextOp.Le(a, b);
        }

        public virtual TAVar Lt(TAVar a, TAVar b)
        {
            return NextOp.Lt(a, b);
        }

        public virtual TAVar Mod(TAVar a, TAVar b)
        {
            return NextOp.Mod(a, b);
        }

        public virtual TAVar Mul(TAVar a, TAVar b)
        {
            return NextOp.Mul(a, b);
        }

        public virtual TAVar Ne(TAVar a, TAVar b)
        {
            return NextOp.Ne(a, b);
        }

        public virtual TAVar Neg(TAVar a)
        {
            return NextOp.Neg(a);
        }

        public virtual TAVar Or(TAVar a, TAVar b)
        {
            return NextOp.Or(a, b);
        }

        public virtual TAVar Reverse(TAVar a)
        {
            return NextOp.Reverse(a);
        }


        public virtual TAVar Sub(TAVar a, TAVar b)
        {
            return NextOp.Sub(a, b);
        }
    }
}
