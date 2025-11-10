using System;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Common;
using TemplateAction.Label.Element;
using TemplateAction.Label.Expression;
using TemplateAction.Label.Token;
namespace TemplateAction.Label
{
    public class TemplateDocument : Template
    {
        public const string LABEL_TYPE = "Document";
        public override string Type
        {
            get { return LABEL_TYPE; }
        }

        public TemplateDocument() : this(string.Empty) { }
        public TemplateDocument(string content)
        {
            try
            {
                //创建栈，用于保存父标签
                Stack<Template> stacks = new Stack<Template>();
                stacks.Push(this);
                TemplateSequence tsequence = new TemplateSequence(content);
                OnCreateLabel(stacks, tsequence);
            }
            catch (Exception ex)
            {
                throw new Exception("模版创建异常,来源:" + ex.Source + "异常信息:" + ex.Message);
            }

        }

        /// <summary>
        /// 解释标签的属性
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="lastToken"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected bool GetParam(TemplateSequence sequence, out BaseToken lastToken, out string name, out string value)
        {
            BaseToken token = sequence.Pop();
            if (token.Type == CharSetToken.TOKEN_TYPE)
            {
                name = token.Value<string>();
                token = sequence.Pop();
                if (token.Token == StructToken.EQUAL)
                {
                    token = sequence.Pop();
                    if (token.Type == StringToken.TOKEN_TYPE)
                    {
                        value = token.Value<string>();
                        lastToken = token;
                        return true;
                    }
                }
            }
            name = string.Empty;
            value = string.Empty;
            lastToken = token;
            return false;
        }

        /// <summary>
        /// 解释函数和变量
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="funname"></param>
        /// <param name="parent"></param>
        /// <param name="sequence"></param>
        /// <returns></returns>
        protected BaseToken ParseFunOrVar(string funname, TemplateSequence sequence, out object statement)
        {
            statement = null;
            BaseToken token = sequence.Pop();
            ExpLink targetlink = TemplateExpress.ParseLink(funname, sequence, ref token);
            if (StructToken.EQUAL.Equals(token.Token))
            {
                token = sequence.Pop();
                TemplateExpress exp = TemplateExpress.CreateFrom(sequence, ref token);
                if (exp.Count > 0)
                {
                    statement = ParseVar(exp, targetlink);
                }
            }
            else
            {
                //变量直接打印输出
                statement = ParseVar(TemplateExpress.CreateExpress(targetlink), null);
            }
            return token;
        }
        /// <summary>
        /// 解释变量赋值
        /// </summary>
        /// <param name="varname"></param>
        /// <param name="targetvar"></param>
        /// <param name="parent"></param>
        /// <param name="sequence"></param>
        protected Template ParseVar(object obj, ExpLink targetvar)
        {
            Template theLabel = new AssignLabel();
            if (targetvar != null)
            {
                theLabel.Param.AddParam(TAUtility.FUN_VAR, targetvar);
            }
            theLabel.Param.AddParam(TAUtility.ASSIGN_SRC, obj);
            return theLabel;
        }
        /// <summary>
        /// 解释@{}里面的语句
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sequence"></param>
        protected BaseToken ParseStatement(BaseToken token, TemplateSequence sequence, out object statement)
        {
            statement = null;
            string varname = string.Empty;
            if (token.Type == VarToken.TOKEN_TYPE)
            {
                varname = token.Value<string>();
                token = sequence.Pop();
                ExpLink targetlink = TemplateExpress.ParseLink(varname, sequence, ref token);
                if (StructToken.EQUAL.Equals(token.Token))
                {
                    token = sequence.Pop();
                    TemplateExpress exp = TemplateExpress.CreateFrom(sequence, ref token);
                    if (exp.Count > 0)
                    {
                        statement = ParseVar(exp, targetlink);
                    }
                }
                else
                {
                    //变量直接打印输出
                    statement = ParseVar(TemplateExpress.CreateExpress(targetlink), null);
                }
            }
            else if (token.Type == StringToken.TOKEN_TYPE)
            {
                //字符串可直接打印输出
                statement = ParseVar(token.GetValue(), null);
                token = sequence.Pop();
            }
            else if (StructToken.EQUAL.Equals(token.Token))
            {
                //=号输出的字符串不进行html编码
                token = sequence.Pop();
                TemplateExpress exp = TemplateExpress.CreateFrom(sequence, ref token);
                Template tout = ParseVar(exp, null);
                tout.Param.AddParam(TAUtility.HTML_ENCODE, false);
                statement = tout;
            }
            else if (token.Type == CharSetToken.TOKEN_TYPE)
            {
                string keyword = token.ToString();
                if (CharSetToken.IF.Equals(keyword) || CharSetToken.WHILE.Equals(keyword))
                {
                    //解释条件if()或while()
                    token = sequence.Pop();
                    if (StructToken.FUN_PARAM_L.Equals(token.Token))
                    {
                        token = sequence.Pop();
                        TemplateExpress exp = TemplateExpress.CreateFrom(sequence, ref token);
                        if (exp.Count > 0 && StructToken.FUN_PARAM_R.Equals(token.Token))
                        {
                            Template theLabel = GetLabel(keyword);
                            theLabel.Param.AddParam(TAUtility.CONDITION_EX, exp);
                            statement = theLabel;
                            token = sequence.Pop();
                        }
                    }
                }
                else if (CharSetToken.ELSE.Equals(keyword))
                {
                    //解释条件else if()
                    token = sequence.Pop();
                    if (CharSetToken.IF.Equals(token.ToString()))
                    {
                        token = sequence.Pop();
                        if (StructToken.FUN_PARAM_L.Equals(token.Token))
                        {
                            token = sequence.Pop();
                            TemplateExpress exp = TemplateExpress.CreateFrom(sequence, ref token);
                            if (exp.Count > 0 && StructToken.FUN_PARAM_R.Equals(token.Token))
                            {
                                Template theLabel = GetLabel(ElseLabel.LABEL_TYPE);
                                theLabel.Param.AddParam(TAUtility.CONDITION_EX, exp);
                                statement = theLabel;
                                token = sequence.Pop();
                            }
                        }
                    }
                    else
                    {
                        //解释条件else
                        Template theLabel = GetLabel(ElseLabel.LABEL_TYPE);
                        statement = theLabel;
                    }
                }
                else if (CharSetToken.FOR.Equals(keyword))
                {
                    //解释for(($a,$i) in $b)或for($a in $b)
                    token = sequence.Pop();
                    if (StructToken.FUN_PARAM_L.Equals(token.Token))
                    {
                        token = sequence.Pop();
                        if (token.Type == VarToken.TOKEN_TYPE || token.Type == CharSetToken.TOKEN_TYPE)
                        {
                            string tname = token.Value<string>();
                            token = sequence.Pop();
                            if (CharSetToken.IN.Equals(token.ToString()))
                            {
                                token = sequence.Pop();
                                TemplateExpress exp = TemplateExpress.CreateFrom(sequence, ref token);
                                if (exp.Count > 0 && StructToken.FUN_PARAM_R.Equals(token.Token))
                                {
                                    Template theLabel = GetLabel(LoopLabel.LABEL_TYPE);
                                    theLabel.Param.AddParam(TAUtility.FOR_FROM, exp);
                                    theLabel.Param.AddParam(TAUtility.FOR_NAME, tname);
                                    statement = theLabel;
                                    token = sequence.Pop();
                                }
                            }
                        }
                        else if (StructToken.FUN_PARAM_L.Equals(token.Token))
                        {
                            token = sequence.Pop();
                            if (token.Type == VarToken.TOKEN_TYPE || token.Type == CharSetToken.TOKEN_TYPE)
                            {
                                string tname = token.Value<string>();
                                token = sequence.Pop();
                                if (StructToken.FUN_PARAM_M.Equals(token.Token))
                                {
                                    token = sequence.Pop();
                                    if (token.Type == VarToken.TOKEN_TYPE || token.Type == CharSetToken.TOKEN_TYPE)
                                    {
                                        string tindex = token.Value<string>();
                                        token = sequence.Pop();
                                        if (StructToken.FUN_PARAM_R.Equals(token.Token))
                                        {
                                            token = sequence.Pop();
                                            if (CharSetToken.IN.Equals(token.ToString()))
                                            {
                                                token = sequence.Pop();
                                                TemplateExpress exp = TemplateExpress.CreateFrom(sequence, ref token);
                                                if (exp.Count > 0 && StructToken.FUN_PARAM_R.Equals(token.Token))
                                                {
                                                    Template theLabel = GetLabel(LoopLabel.LABEL_TYPE);
                                                    theLabel.Param.AddParam(TAUtility.FOR_FROM, exp);
                                                    theLabel.Param.AddParam(TAUtility.FOR_INDEX, tindex);
                                                    theLabel.Param.AddParam(TAUtility.FOR_NAME, tname);
                                                    statement = theLabel;
                                                    token = sequence.Pop();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (CharSetToken.END.Equals(keyword))
                {
                    statement = CharSetToken.END;
                    token = sequence.Pop();
                }
                else if (CharSetToken.BREAK.Equals(keyword))
                {
                    Template theLabel = GetLabel(BreakLabel.LABEL_TYPE);
                    statement = theLabel;
                    token = sequence.Pop();
                }
                else
                {
                    token = ParseFunOrVar(keyword, sequence, out statement);
                }
            }
            return token;
        }
        /// <summary>
        /// 解释标签
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sequence"></param>
        protected void OnCreateLabel(Stack<Template> stack, TemplateSequence sequence)
        {
            int _topIndex = 0;
            BaseToken token = null;
            int beginIndex = 0;
            do
            {
                _topIndex = sequence.Position;
                token = sequence.NextTemplate();
                beginIndex = sequence.Position - token.Length;
                bool successparse = false;
                if (token.Token == StructToken.BEGIN_START)
                {
                    Template tmpLabel = new TxTLabel(sequence.Substring(_topIndex, beginIndex - _topIndex));
                    stack.Peek().AddLable(tmpLabel);
                    //解释标签开始
                    token = sequence.Pop();
                    if (token.Type == CharSetToken.TOKEN_TYPE)
                    {
                        string _MyLT = token.ToString();
                        string tName;
                        string tVal;
                        Template theLabel = GetLabel(_MyLT);
                        while (GetParam(sequence, out token, out tName, out tVal))
                        {
                            theLabel.Param.AddParam(tName, tVal);
                        }

                        if (token.Token == StructToken.END_START_FINISH)
                        {
                            stack.Peek().AddLable(theLabel);
                            successparse = true;
                        }
                        else if (token.Token == StructToken.END_FINISH)
                        {
                            stack.Peek().AddLable(theLabel);
                            stack.Push(theLabel);
                            successparse = true;
                        }
                    }
                }
                else if (token.Token == StructToken.END_START)
                {
                    //解释标签结束
                    Template tmpLabel = new TxTLabel(sequence.Substring(_topIndex, beginIndex - _topIndex));
                    stack.Peek().AddLable(tmpLabel);
                    token = sequence.Pop();
                    if (token.Type == CharSetToken.TOKEN_TYPE)
                    {
                        string endMT = token.ToString();
                        token = sequence.Pop();
                        if (token.Token == StructToken.END_FINISH)
                        {
                            if (endMT == stack.Peek().Type)
                            {
                                stack.Pop();
                                successparse = true;
                            }
                        }
                    }
                }
                else if (token.Token == StructToken.BEGIN_FUN)
                {
                    //解释@{}
                    Template tmpLabel = new TxTLabel(sequence.Substring(_topIndex, beginIndex - _topIndex));
                    stack.Peek().AddLable(tmpLabel);
                    token = sequence.Pop();

                    if (token.Token == StructToken.SECTION_L)
                    {
                        token = sequence.Pop();
                        LinkedList<object> statements = new LinkedList<object>();
                        //解释语句
                        while (true)
                        {
                            object statement;
                            token = ParseStatement(token, sequence, out statement);
                            if (statement == null)
                            {
                                successparse = false;
                                break;
                            }
                            else
                            {
                                statements.AddLast(statement);
                            }
                            if (token.Token == StructToken.SECTION_R)
                            {
                                foreach (object t in statements)
                                {
                                    Statement2Stack(t, stack);
                                }
                                successparse = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        bool simple = false;
                        if (beginIndex == 0)
                        {
                            simple = true;
                        }
                        else
                        {
                            char prechr = sequence.Substring(beginIndex - 1, 1)[0];
                            if (char.IsPunctuation(prechr) || char.IsWhiteSpace(prechr) || prechr == '>' || prechr == '<')
                            {
                                simple = true;
                            }
                        }
                        if (simple)
                        {
                            //简写处理
                            object statement;
                            token = ParseStatement(token, sequence, out statement);
                            if (statement != null)
                            {
                                sequence.BackPosition(token.Length);
                                Statement2Stack(statement, stack);
                                successparse = true;
                            }
                        }

                    }
                }
                else
                {
                    break;
                }
                if (!successparse)
                {
                    Template tmpLabel = new TxTLabel(sequence.Substring(beginIndex, sequence.Position - beginIndex));
                    stack.Peek().AddLable(tmpLabel);
                }
            } while (true);
            Template tmpLabel2 = new TxTLabel(sequence.Substring(_topIndex, beginIndex - _topIndex));
            stack.Peek().AddLable(tmpLabel2);
        }
        protected void Statement2Stack(object t, Stack<Template> stack)
        {
            Template tlabel = t as Template;
            if (tlabel != null)
            {
                stack.Peek().AddLable(tlabel);
                if (IfLabel.LABEL_TYPE.Equals(tlabel.Type) ||
                    LoopLabel.LABEL_TYPE.Equals(tlabel.Type) ||
                    WhileLabel.LABEL_TYPE.Equals(tlabel.Type))
                {
                    stack.Push(tlabel);
                }
            }
            else
            {
                string endstr = t as string;
                if (endstr != null && CharSetToken.END.Equals(endstr))
                {
                    stack.Pop();
                }
            }
        }
        /// <summary>
        /// 返回标签实例
        /// </summary>
        /// <returns>根据标签类型返回标签实例</returns>
        protected Template GetLabel(string labelType)
        {
            Template tTmp;
            switch (labelType)
            {
                case IfLabel.LABEL_TYPE:
                    tTmp = new IfLabel();
                    break;
                case LoopLabel.LABEL_TYPE:
                    tTmp = new LoopLabel();
                    break;
                case IncludeLabel.LABEL_TYPE:
                    tTmp = new IncludeLabel();
                    break;
                case ElseLabel.LABEL_TYPE:
                    tTmp = new ElseLabel();
                    break;
                case DefineLabel.LABEL_TYPE:
                    tTmp = new DefineLabel();
                    break;
                case WhileLabel.LABEL_TYPE:
                    tTmp = new WhileLabel();
                    break;
                case BreakLabel.LABEL_TYPE:
                    tTmp = new BreakLabel();
                    break;
                default:
                    tTmp = new TxTLabel(labelType);
                    break;
            }

            return tTmp;
        }
        protected override string OnMake(ITemplateContext context)
        {
            StringBuilder tmpBuilder = new StringBuilder();
            foreach (Template child in mChilds)
            {
                tmpBuilder.Append(child.MakeHtml(context));
                if (context.BreakCount > 0)
                {
                    break;
                }
            }
            return tmpBuilder.ToString();
        }
    }
}
