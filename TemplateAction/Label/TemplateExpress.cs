using System;
using System.Collections.Generic;
using TemplateAction.Common;
using TemplateAction.Label.Element;
using TemplateAction.Label.Expression;
using TemplateAction.Label.Token;

namespace TemplateAction.Label
{
    /// <summary>
    /// 模板表达式
    /// </summary>
    public class TemplateExpress
    {
        LinkedList<IExpressionItem> _expressionList;
        public int Count
        {
            get { return _expressionList.Count; }
        }
        private TemplateExpress(LinkedList<IExpressionItem> link)
        {
            _expressionList = link;
        }
        public static TemplateExpress CreateExpress(IExpressionItem item)
        {
            LinkedList<IExpressionItem> expressionList = new LinkedList<IExpressionItem>();
            expressionList.AddLast(item);
            return new TemplateExpress(expressionList);
        }
        /// <summary>
        /// 创建变量链
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sequence"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static TemplateExpress CreateLink(string name, TemplateSequence sequence, ref BaseToken token)
        {
            LinkedList<IExpressionItem> expressionList = new LinkedList<IExpressionItem>();
            expressionList.AddLast(ParseLink(name, sequence, ref token));
            return new TemplateExpress(expressionList);
        }
        /// <summary>
        /// 解释函数
        /// </summary>
        /// <param name="funname"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        private static ExpFun ParseFun(string funname, TemplateSequence sequence)
        {
            BaseToken token;
            ExpFun tfun = new ExpFun(funname);
            while (true)
            {
                token = sequence.Pop();
                TemplateExpress exp = CreateFrom(sequence, ref token);
                if (exp.Count <= 0) break;
                tfun.AddParam(exp);
                if (!StructToken.FUN_PARAM_M.Equals(token.Token))
                {
                    break;
                }
            }
            if (StructToken.FUN_PARAM_R.Equals(token.Token))
            {
                return tfun;
            }
            return null;
        }
        /// <summary>
        /// 解释变量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sequence"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ExpVar ParseVar(string name, TemplateSequence sequence, ref BaseToken token)
        {
            ExpVar expvar = new ExpVar(name);
            if (StructToken.LM.Equals(token.Token))
            {
                //解释数组
                token = sequence.Pop();
                TemplateExpress exp = CreateFrom(sequence, ref token);
                if (exp.Count > 0 && StructToken.RM.Equals(token.Token))
                {
                    expvar.InitIdx(exp);
                    token = sequence.Pop();
                }
            }
            return expvar;
        }

        public static ExpLink ParseLink(string name, TemplateSequence sequence, ref BaseToken token)
        {
            ExpLink link = new ExpLink();
            if (StructToken.FUN_PARAM_L.Equals(token.Token))
            {
                //解释函数
                ExpFun fun = ParseFun(name, sequence);
                if (fun == null)
                {
                    throw new Exception("对象的指定函数不存在");
                }
                link.AddLink(fun);
                token = sequence.Pop();
            }
            else
            {
                link.AddLink(ParseVar(name, sequence, ref token));
            }
            while (StructToken.LINK.Equals(token.Token))
            {
                token = sequence.Pop();
                if (token.Type == CharSetToken.TOKEN_TYPE)
                {
                    string charname = token.Value<string>();
                    token = sequence.Pop();
                    if (StructToken.FUN_PARAM_L.Equals(token.Token))
                    {
                        //解释函数
                        ExpFun fun = ParseFun(charname, sequence);
                        if (fun == null)
                        {
                            throw new Exception("对象的指定函数不存在");
                        }
                        link.AddLink(fun);
                        token = sequence.Pop();
                    }
                    else
                    {
                        //解释变量
                        link.AddLink(ParseVar(charname, sequence, ref token));
                    }
                }
            }
            return link;
        }
        /// <summary>
        /// 创建复杂表达式
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static TemplateExpress CreateFrom(TemplateSequence sequence, ref BaseToken token)
        {
            Stack<TAExpression.TAOperator> operatorStack = new Stack<TAExpression.TAOperator>();
            //前一个表达式是否为操作码：操作码指各种符号，操作数指变量、字符串、数字
            bool isPreOp = true;
            //左括号数
            int lkhcount = 0;
            LinkedList<IExpressionItem> expressionList = new LinkedList<IExpressionItem>();
            while (true)
            {
                if (token.Type == StringToken.TOKEN_TYPE)
                {
                    if (!isPreOp) break;
                    expressionList.AddLast(new ExpConst(token.GetValue(), VarType.String));
                    isPreOp = false;
                    token = sequence.Pop();
                }
                else if (token.Type == NumberToken.TOKEN_TYPE)
                {
                    if (!isPreOp) break;
                    expressionList.AddLast(new ExpConst(token.GetValue(), VarType.Number));
                    isPreOp = false;
                    token = sequence.Pop();
                }
                else if (token.Type == VarToken.TOKEN_TYPE)
                {
                    if (!isPreOp) break;
                    expressionList.AddLast(ParseLink(token.Value<string>(), sequence, ref token));
                    isPreOp = false;
                }
                else if (token.Type == CharSetToken.TOKEN_TYPE)
                {
                    if (!isPreOp) break;
                    isPreOp = false;
                    bool needpop = true;
                    string charname = token.Value<string>();
                    switch (charname)
                    {
                        case CharSetToken.FALSE:
                            expressionList.AddLast(new ExpConst(false, VarType.Keyword));
                            break;
                        case CharSetToken.TRUE:
                            expressionList.AddLast(new ExpConst(true, VarType.Keyword));
                            break;
                        case CharSetToken.NULL:
                            expressionList.AddLast(new ExpConst(null, VarType.Keyword));
                            break;
                        case CharSetToken.THIS:
                            expressionList.AddLast(new ExpThis());
                            break;
                        default:
                            {
                                token = sequence.Pop();
                                //解释变量
                                needpop = false;
                                expressionList.AddLast(ParseLink(charname, sequence, ref token));
                            }
                            break;
                    }
                    if (needpop)
                    {
                        token = sequence.Pop();
                    }
                }
                else if (token.Type == StructToken.TOKEN_TYPE)
                {
                    bool isbreak = false;
                    string structstr = token.Token;
                    switch (structstr)
                    {
                        case StructToken.FUN_PARAM_L:
                            {
                                //取“(”处理
                                lkhcount++;
                                operatorStack.Push(new TAExpression.TAOperator(structstr, 0));
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.FUN_PARAM_R:
                            {
                                //取“)”处理
                                if (lkhcount <= 0)
                                {
                                    isbreak = true;
                                    break;
                                }
                                lkhcount--;
                                do
                                {
                                    if (operatorStack.Count <= 0) break;
                                    if (operatorStack.Peek().oper != "(")
                                    {
                                        expressionList.AddLast(new ExpOp(operatorStack.Pop().oper));
                                    }
                                    else
                                    {
                                        operatorStack.Pop();
                                        break;
                                    }

                                } while (true);
                                isPreOp = false;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.EEQ:
                        case StructToken.END_FINISH:
                        case StructToken.LESS:
                        case StructToken.LEQ:
                        case StructToken.GEQ:
                        case StructToken.NEQ:
                            {
                                TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 4));
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.OR:
                        case StructToken.AND:
                            {
                                TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 5));
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.NE:
                            {
                                TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 1));
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.MUL:
                        case StructToken.DIV:
                        case StructToken.MOD:
                            {
                                TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 2));
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.ADD:
                            {
                                TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 3));
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.SUB:
                            {
                                //判断负号是否为单目，为单目则更改符号
                                if (isPreOp)
                                {
                                    structstr = "d-";
                                    TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 1));
                                }
                                else
                                {
                                    TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 3));
                                }
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        case StructToken.Q:
                        case StructToken.COLON:
                            {
                                //三目运算符?:
                                TAExpression.CalPriorityExpress(expressionList, operatorStack, new TAExpression.TAOperator(structstr, 11));
                                isPreOp = true;
                                token = sequence.Pop();
                            }
                            break;
                        default:
                            isbreak = true;
                            break;
                    }
                    if (isbreak)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            while (operatorStack.Count != 0)
            {
                expressionList.AddLast(new ExpOp(operatorStack.Pop().oper));
            }
            return new TemplateExpress(expressionList);
        }
        /// <summary>
        /// 计算出结果
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public TAVar Calculate(ITemplateContext context)
        {
            TAExpression exp = new TAExpression(context);
            return exp.CalculateParenthesesExpressionEx(_expressionList);
        }
    }
}
