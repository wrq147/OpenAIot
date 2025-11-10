using System.Collections.Generic;
using TemplateAction.Label.Expression;

namespace TemplateAction.Label
{
    public class TAExpression
    {
        public struct TAOperator
        {
            public string oper;
            public int level;
            public TAOperator(string op, int lv)
            {
                oper = op;
                level = lv;
            }
        }
        protected ITemplateContext mContext;

        public TAExpression(ITemplateContext context)
        {
            mContext = context;
        }

        //分离优先级
        public static void CalPriorityExpress(LinkedList<IExpressionItem> express, Stack<TAOperator> operatorStack, TAOperator operator1)
        {
            if (operatorStack.Count > 0)
            {
                TAOperator tmpop = operatorStack.Peek();
                if (tmpop.oper == "(" || verifyOperatorPriority(operator1, tmpop))
                {
                    operatorStack.Push(operator1);
                }
                else
                {
                    express.AddLast(new ExpOp(operatorStack.Pop().oper));
                    operatorStack.Push(operator1);
                }
            }
            else
            {
                operatorStack.Push(operator1);
            }
        }


        /// <summary>
        /// 对转换成后序表达的式子进行计算
        /// 23#51#102#100#-#/#*#36#24#-#8#6#-#/#*#+"
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public TAVar CalculateParenthesesExpressionEx(LinkedList<IExpressionItem> expression)
        {
            //定义两个栈
            Stack<TAVar> operandList = new Stack<TAVar>();
            TAVar operand1;
            TAVar operand2;
            TAVar operand3;
            foreach (IExpressionItem oper in expression)
            {
                TAVar var = oper.Calculate(mContext, null);
                if (var != null)
                {
                    operandList.Push(var);
                }
                else
                {
                    string tmpoper = oper.ToString();
                    //两个操作数退栈和一个操作符退栈计算
                    if (operandList.Count >= 1)
                    {
                        operand2 = operandList.Pop();
                        TAVar dm = calculate(operand2, tmpoper);
                        if (dm != null)
                        {
                            operandList.Push(dm);
                        }
                        else
                        {
                            //双目
                            if (operandList.Count > 0)
                            {
                                operand1 = operandList.Pop();
                                dm = calculate(operand1, operand2, tmpoper);
                                if (dm != null)
                                {
                                    operandList.Push(dm);
                                }
                                else
                                {
                                    //三目
                                    if (operandList.Count > 0)
                                    {
                                        operand3 = operandList.Pop();
                                        dm = calculate(operand1, operand2, operand3, tmpoper);
                                        if (dm != null)
                                        {
                                            operandList.Push(dm);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return operandList.Pop();
        }
        private static bool verifyOperatorPriority(TAOperator Operator1, TAOperator Operator2)
        {
            //单目优先
            if (Operator1.level < Operator2.level)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 单目计算
        /// </summary>
        /// <param name="operand2"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        private TAVar calculate(TAVar op1, string operator2)
        {
            switch (operator2)
            {
                case "!":
                    {
                        return OpMan.Instance().First.Reverse(op1);
                    }
                case "d-":
                    {
                        return OpMan.Instance().First.Neg(op1);
                    }
                case "?":
                    return op1;
                default:
                    return null;
            }
        }
        /// <summary>
        /// 双目计算
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        private TAVar calculate(TAVar op1, TAVar op2, string operator2)
        {

            switch (operator2)
            {
                case "*":
                    {
                        return OpMan.Instance().First.Mul(op1, op2);
                    }
                case "/":
                    {
                        return OpMan.Instance().First.Div(op1, op2);
                    }
                case "+":
                    {
                        return OpMan.Instance().First.Add(op1, op2);
                    }
                case "-":
                    {
                        return OpMan.Instance().First.Sub(op1, op2);
                    }
                case ">":
                    {
                        return OpMan.Instance().First.Gt(op1, op2);
                    }
                case "<":
                    {
                        return OpMan.Instance().First.Lt(op1, op2);
                    }
                case ">=":
                    {
                        return OpMan.Instance().First.Ge(op1, op2);
                    }
                case "<=":
                    {
                        return OpMan.Instance().First.Le(op1, op2);
                    }
                case "!=":
                    {
                        return OpMan.Instance().First.Ne(op1, op2);
                    }
                case "==":
                    {
                        return OpMan.Instance().First.Eq(op1, op2);
                    }
                case "||":
                    {
                        return OpMan.Instance().First.Or(op1, op2);
                    }
                case "&&":
                    {
                        return OpMan.Instance().First.And(op1, op2);
                    }
                case "%":
                    {
                        return OpMan.Instance().First.Mod(op1, op2);
                    }
                default:
                    return null;
            }
        }
        private TAVar calculate(TAVar op1, TAVar op2, TAVar op3, string operator2)
        {
            if (operator2 == ":")
            {
                return op3.ToBoolean() ? op1 : op2;
            }
            return null;
        }
    }
}
