using System;

namespace TemplateAction.Label.Expression
{
    public class ValueOp : AbstractOp
    {
        public override TAVar Ge(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) >= Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) >= Convert.ToDouble(b.Value), VarType.Number);
            }
            else if (a.Value is DateTime && b.Value is DateTime)
            {
                return new TAVar(Convert.ToDateTime(a.Value) >= Convert.ToDateTime(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) >= Convert.ToInt64(b.Value), VarType.Number);
            }
        }

        public override TAVar Gt(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) > Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) > Convert.ToDouble(b.Value), VarType.Number);
            }
            else if (a.Value is DateTime && b.Value is DateTime)
            {
                return new TAVar(Convert.ToDateTime(a.Value) > Convert.ToDateTime(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) > Convert.ToInt64(b.Value), VarType.Number);
            }
        }

        public override TAVar Le(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) <= Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) <= Convert.ToDouble(b.Value), VarType.Number);
            }
            else if (a.Value is DateTime && b.Value is DateTime)
            {
                return new TAVar(Convert.ToDateTime(a.Value) <= Convert.ToDateTime(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) <= Convert.ToInt64(b.Value), VarType.Number);
            }
        }

        public override TAVar Lt(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) < Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) < Convert.ToDouble(b.Value), VarType.Number);
            }
            else if (a.Value is DateTime && b.Value is DateTime)
            {
                return new TAVar(Convert.ToDateTime(a.Value) < Convert.ToDateTime(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) < Convert.ToInt64(b.Value), VarType.Number);
            }
        }
        public override TAVar Eq(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) == Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.ValueType.IsPrimitive && b.ValueType.IsPrimitive)
            {
                if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
                {
                    return new TAVar(Convert.ToDouble(a.Value) == Convert.ToDouble(b.Value), VarType.Number);
                }
                else
                {
                    return new TAVar(Convert.ToInt64(a.Value) == Convert.ToInt64(b.Value), VarType.Number);
                }
            }
            else
            {
                return new TAVar(a.Value.Equals(b.Value), VarType.Keyword);
            }

        }
        public override TAVar Ne(TAVar a, TAVar b)
        {
            return Reverse(this.Eq(a, b));
        }
        public override TAVar Reverse(TAVar a)
        {
            return new TAVar(!a.ToBoolean(), VarType.Keyword);
        }

        public override TAVar And(TAVar a, TAVar b)
        {
            if (a.ToBoolean() && b.ToBoolean())
            {
                return new TAVar(true, VarType.Keyword);
            }
            else
            {
                return new TAVar(false, VarType.Keyword);
            }
        }
        public override TAVar Or(TAVar a, TAVar b)
        {
            if (a.ToBoolean() || b.ToBoolean())
            {
                return new TAVar(true, VarType.Keyword);
            }
            else
            {
                return new TAVar(false, VarType.Keyword);
            }
        }
        public override TAVar Add(TAVar a, TAVar b)
        {
            if (a.Value is string || b.Value is string)
            {
                return new TAVar(Convert.ToString(a.Value) + Convert.ToString(b.Value), VarType.String);
            }
            else if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) + Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) + Convert.ToDouble(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) + Convert.ToInt64(b.Value), VarType.Number);
            }
        }
        public override TAVar Mod(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) % Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) % Convert.ToDouble(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) % Convert.ToInt64(b.Value), VarType.Number);
            }
        }
        public override TAVar Div(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) / Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) / Convert.ToDouble(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) / Convert.ToInt64(b.Value), VarType.Number);
            }
        }


        public override TAVar Mul(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) * Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) * Convert.ToDouble(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) * Convert.ToInt64(b.Value), VarType.Number);
            }
        }

        public override TAVar Sub(TAVar a, TAVar b)
        {
            if (a.Value is decimal || b.Value is decimal)
            {
                return new TAVar(Convert.ToDecimal(a.Value) - Convert.ToDecimal(b.Value), VarType.Number);
            }
            else if (a.Value is double || b.Value is double || a.Value is float || b.Value is float)
            {
                return new TAVar(Convert.ToDouble(a.Value) - Convert.ToDouble(b.Value), VarType.Number);
            }
            else
            {
                return new TAVar(Convert.ToInt64(a.Value) - Convert.ToInt64(b.Value), VarType.Number);
            }
        }

        public override TAVar Neg(TAVar a)
        {
            if (a.Value is decimal)
            {
                return new TAVar(-Convert.ToDecimal(a.Value), VarType.Number);
            }
            else if (a.Value is double || a.Value is float)
            {
                return new TAVar(-Convert.ToDouble(a.Value), VarType.Number);
            }
            else
            {
                return new TAVar(-Convert.ToInt64(a.Value), VarType.Number);
            }
        }
      
    }
}
