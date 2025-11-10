using System;

namespace TemplateAction.Label.Expression
{
    public class ExceptionOp : AbstractOp
    {
        public override TAVar Add(TAVar a, TAVar b)
        {
            throw new Exception("对象无法进行加法运算");
        }


        public override TAVar Div(TAVar a, TAVar b)
        {
            throw new Exception("对象无法进行除法运算");
        }

        public override TAVar Eq(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Ge(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Gt(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Le(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Lt(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Mul(TAVar a, TAVar b)
        {
            throw new Exception("对象无法进行乘法运算");
        }

        public override TAVar Ne(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Neg(TAVar a)
        {
            throw new Exception("对象无法带负号");
        }
        public override TAVar And(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Or(TAVar a, TAVar b)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Reverse(TAVar a)
        {
            return new TAVar(false, VarType.Keyword);
        }

        public override TAVar Sub(TAVar a, TAVar b)
        {
            throw new Exception("对象无法进行减法运算");
        }

    }
}
