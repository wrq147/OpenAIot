using System;

namespace TemplateAction.Label.Expression
{
    /// <summary>
    /// 判断算术运算时参数是否为null
    /// </summary>
    public class NullArithOp : AbstractOp
    {
        public override TAVar Ge(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                return new TAVar(false, VarType.Keyword);
            }
            else
            {
                return NextOp.Ge(a, b);
            }
        }

        public override TAVar Gt(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                return new TAVar(false, VarType.Keyword);
            }
            else
            {
                return NextOp.Gt(a, b);
            }
        }

        public override TAVar Le(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                return new TAVar(false, VarType.Keyword);
            }
            else
            {
                return NextOp.Le(a, b);
            }
        }

        public override TAVar Lt(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                return new TAVar(false, VarType.Keyword);
            }
            else
            {
                return NextOp.Lt(a, b);
            }
        }
        public override TAVar Eq(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                if (a.IsNull() && b.IsNull())
                {
                    return new TAVar(true, VarType.Keyword);
                }
                else
                {
                    return new TAVar(false, VarType.Keyword);
                }
            }
            else
            {
                return NextOp.Eq(a, b);
            }
        }
        public override TAVar Ne(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                if (a.IsNull() && b.IsNull())
                {
                    return new TAVar(false, VarType.Keyword);
                }
                else
                {
                    return new TAVar(true, VarType.Keyword);
                }
            }
            else
            {
                return NextOp.Ne(a, b);
            }
        }
        public override TAVar Reverse(TAVar a)
        {
            if (a.IsNull())
            {
                return new TAVar(true, VarType.Keyword);
            }
            else
            {
                return NextOp.Reverse(a);
            }
        }

        public override TAVar And(TAVar a, TAVar b)
        {
            TAVar newa = a;
            if (a.IsNull())
            {
                newa = new TAVar(false, VarType.Keyword);
            }
            TAVar newb = b;
            if (b.IsNull())
            {
                newb = new TAVar(false, VarType.Keyword);
            }
            return NextOp.And(newa, newb);
        }
        public override TAVar Or(TAVar a, TAVar b)
        {
            TAVar newa = a;
            if (a.IsNull())
            {
                newa = new TAVar(false, VarType.Keyword);
            }
            TAVar newb = b;
            if (b.IsNull())
            {
                newb = new TAVar(false, VarType.Keyword);
            }
            return NextOp.Or(newa, newb);
        }


        public override TAVar Add(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                throw new Exception("加法对象不能为null");
            }
            else
            {
                return NextOp.Add(a, b);
            }
        }

        public override TAVar Div(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                throw new Exception("除法对象不能为null");
            }
            else
            {
                return NextOp.Div(a, b);
            }
        }
        public override TAVar Mod(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                throw new Exception("取余对象不能为null");
            }
            else
            {
                return NextOp.Mod(a, b);
            }
        }
        public override TAVar Mul(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                throw new Exception("乘法对象不能为null");
            }
            else
            {
                return NextOp.Mul(a, b);
            }
        }


        public override TAVar Neg(TAVar a)
        {
            if (a.IsNull())
            {
                throw new Exception("负号对象不能为null");
            }
            else
            {
                return NextOp.Neg(a);
            }
        }

        public override TAVar Sub(TAVar a, TAVar b)
        {
            if (a.IsNull() || b.IsNull())
            {
                throw new Exception("减法对象不能为null");
            }
            else
            {
                return NextOp.Sub(a, b);
            }
        }

     
    }
}
