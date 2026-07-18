using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder.WhereToSql
{
    internal sealed class ConditionBuilder : ExpressionVisitor
    {
        #region 属性

        /// <summary>
        /// 参数
        /// </summary>
        public OrderedDictionary Arguments { get; private set; }
        /// <summary>
        /// 输入参数数量
        /// </summary>
        public int InputParamCount { get; private set; }
        public ReadOnlyCollection<ParameterExpression> InputParams { get; private set; }

        public SqlBuilder builder { get; set; }

        /// <summary>
        ///  返回值
        /// </summary>
        public string Result
        {
            get
            {
                var str = string.Join(",", this.ConditionParts);
                return str;
            }
        }

        /// <summary>
        /// 数据
        /// </summary>
        private Stack<string> ConditionParts = new Stack<string>();
        #endregion

        #region 默认构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ConditionBuilder()
        {
            this.Arguments = new OrderedDictionary();
        }
        #endregion

        #region 执行生成

        /// <summary>
        /// 执行生成
        /// </summary>
        /// <param name="expression"></param>
        public void Build(Expression expression)
        {
            //初始化输入参数
            if (expression.NodeType == ExpressionType.Lambda)
            {
                LambdaExpression lambdaExpress = (LambdaExpression)expression;
                this.InputParamCount = lambdaExpress.Parameters.Count;
                this.InputParams = lambdaExpress.Parameters;
            }
            //前期处理
            PartialEvaluator evaluator = new PartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);

            this.Visit(evaluatedExpression);
        }
        #endregion


        #region 重写 二元操作符

        /// <summary>
        /// 重写 二元操作符
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected sealed override Expression VisitBinary(BinaryExpression node)
        {
            if (node != null)
            {
                this.Visit(node.Left);
                this.Visit(node.Right);

                string right = this.ConditionParts.Pop();
                string left = this.ConditionParts.Pop();

                string opr = ExpressionTool.Convert(node.NodeType);//操作符 
                if (right == "null")
                {
                    if (right == " =")
                    {
                        opr = string.Empty;
                        right = right.Substring(0, right.Length - 1) + " is null";
                    }
                    else if (right == "<>")
                    {
                        opr = string.Empty;
                        right = right.Substring(0, right.Length - 1) + " is not null";
                    }

                }
                string condition = string.Format("({0}{1}{2})", left, opr, right);
                this.ConditionParts.Push(condition);
            }
            return node;
        }
        #endregion

        #region 重写常量

        /// <summary>
        /// 重写常量
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected sealed override Expression VisitConstant(ConstantExpression node)
        {
            if (node != null)
            {
                var parName = this.builder.Db.AddParam(node.Value);
                this.Arguments.Add(parName, node.Value);
                this.ConditionParts.Push(parName);
            }
            return node;
        }
        #endregion

        #region 重写 字段 属性
        /// <summary>
        /// 重写 字段 属性
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>00.
        protected sealed override Expression VisitMember(MemberExpression node)
        {

            if (node != null)
            {
                PropertyInfo propertyInfo = node.Member as PropertyInfo;
                if (propertyInfo == null)
                {
                    return node;
                }
                string prefix = string.Empty;
                string propname = propertyInfo.Name;
                Expression preExpress;
                if (node.Expression.Type.Name == "Nullable`1")
                {
                    var tmpmemexp = node.Expression as MemberExpression;
                    propname = tmpmemexp?.Member?.Name;
                    preExpress = tmpmemexp.Expression;
                }
                else
                {
                    preExpress = node.Expression;
                }
                if (builder.SubMaps != null && builder.SubMaps.Count > 0)
                {
                    if (preExpress.NodeType == ExpressionType.MemberAccess)
                    {
                        prefix = (preExpress as MemberExpression)?.Member?.Name;
                        if (!string.IsNullOrEmpty(prefix))
                        {
                            prefix = builder.GetPrefixOfMap(prefix) + ".";
                        }
                    }
                    else
                    {
                        if (this.InputParamCount > 1)
                        {
                            for (int i = 0; i < this.InputParamCount; i++)
                            {
                                if (this.InputParams[i] == preExpress)
                                {
                                    if (i == 0)
                                    {
                                        prefix = "a.";
                                    }
                                    else
                                    {
                                        prefix = DBMapping.GetSubPrefix(i - 1) + ".";
                                    }
                                    break;
                                }
                            }
                        }
                        else
                        {
                            prefix = "a.";
                        }
                    }
                }

                //var obj = node.Update(Visit(node.Expression));
                this.ConditionParts.Push(prefix + propname);
            }
            else
            {
                return base.VisitMember(node);
            }
            return node;
        }
        #endregion

        #region 重写方法处理

        /// <summary>
        /// ConditionBuilder 并不支持生成Like操作，如 字符串的 StartsWith，Contains，EndsWith 并不能生成这样的SQL： Like ‘xxx%’, Like ‘%xxx%’ , Like ‘%xxx’ . 只要override VisitMethodCall 这个方法即可实现上述功能。
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected sealed override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node != null)
            {
                if (node.Method.DeclaringType.FullName == "MyAccess.DB.Builder.WhereToSql.SonSqlFun")
                {
                    switch (node.Method.Name)
                    {
                        case "SqlCondition":
                            switch (node.Arguments[0].NodeType)
                            {
                                case ExpressionType.Constant:
                                    this.ConditionParts.Push(((ConstantExpression)node.Arguments[0]).Value.ToString());
                                    break;
                                case ExpressionType.MemberAccess:
                                case ExpressionType.Add:
                                    {
                                        LambdaExpression lambda = Expression.Lambda(node.Arguments[0]);
                                        Delegate fn = lambda.Compile();
                                        var value = Expression.Constant(fn.DynamicInvoke(null), node.Arguments[0].Type);
                                        this.ConditionParts.Push(value.Value.ToString());
                                    }
                                    break;
                            }

                            return node;
                        case "SqlBuilderCondition":
                            switch (node.Arguments[0].NodeType)
                            {
                                case ExpressionType.MemberAccess:
                                case ExpressionType.Add:
                                    {
                                        LambdaExpression lambda = Expression.Lambda(node.Arguments[0]);
                                        Delegate fn = lambda.Compile();
                                        Action<SqlBuilder> sqlAC = fn.DynamicInvoke(null) as Action<SqlBuilder>;
                                        if (sqlAC != null)
                                        {
                                            var tmpsql = new SqlBuilder(builder.Db);
                                            sqlAC(tmpsql);
                                            this.ConditionParts.Push(tmpsql.ToString());
                                        }
                                    }
                                    break;
                            }

                            return node;
                        case "FullSearch":
                            string tmpfield = string.Empty;
                            switch (node.Arguments[0].NodeType)
                            {
                                case ExpressionType.Constant:
                                    tmpfield = ((ConstantExpression)node.Arguments[0]).Value.ToString();
                                    break;
                                case ExpressionType.MemberAccess:
                                case ExpressionType.Add:
                                    {
                                        LambdaExpression newlambda = Expression.Lambda(node.Arguments[0]);
                                        Delegate newfn = newlambda.Compile();
                                        ConstantExpression objExp = Expression.Constant(newfn.DynamicInvoke(null), node.Arguments[0].Type);
                                        tmpfield = objExp.Value.ToString();
                                    }
                                    break;
                            }
                            IEnumerable<string> words = null;
                            switch (node.Arguments[1].NodeType)
                            {
                                case ExpressionType.Constant:
                                    words = (IEnumerable<string>)((ConstantExpression)node.Arguments[1]).Value;
                                    break;
                                case ExpressionType.MemberAccess:
                                case ExpressionType.Add:
                                    {
                                        LambdaExpression newlambda = Expression.Lambda(node.Arguments[1]);
                                        Delegate newfn = newlambda.Compile();
                                        ConstantExpression objExp = Expression.Constant(newfn.DynamicInvoke(null), node.Arguments[1].Type);
                                        words = (IEnumerable<string>)objExp.Value;
                                    }
                                    break;
                            }
                            this.ConditionParts.Push(builder.Comparable.FullSearch(tmpfield, words));
                            return node;
                    }
                }

                Expression field = null;
                Expression par = null;
                if (node.ToString().StartsWith("value"))
                {
                    if (node.Object != null)
                    {
                        par = node.Object;
                        field = node.Arguments[0];
                    }
                    else
                    {
                        par = node.Arguments[0];
                        if (node.Arguments.Count > 1)
                        {
                            field = node.Arguments[1];
                        }
                    }
                }
                else
                {
                    if (node.Object != null)
                    {
                        field = node.Object;
                        if (node.Arguments.Count > 0)
                        {
                            par = node.Arguments[0];
                        }
                    }
                    else
                    {
                        field = node.Arguments[0];
                        if (node.Arguments.Count > 1)
                        {
                            par = node.Arguments[1];
                        }

                    }
                }
                if (field != null)
                {
                    this.Visit(field);
                }
                if (par != null)
                {
                    this.Visit(par);
                }

                var right = this.ConditionParts.Pop();
                var left = this.ConditionParts.Pop();

                MethodCall(node.Method.Name, left, right);
            }
            return node;
        }

        /// <summary>
        /// 自定义方法和公用方法处理
        /// </summary>
        /// <param name="methodName">方法名称</param>
        /// <param name="left">左测 一定是字段名称</param>
        /// <param name="right">右侧 一定是 this.Arguments的key</param>
        private void MethodCall(string methodName, string left, string right)
        {
            var value = this.Arguments[right];
            var format = "";

            #region 设置sql查询模板

            if (methodName == "Contains" && value is System.Collections.IList)
            {
                methodName = "ExIn";
            }
            if (methodName == "NotContains" && value is System.Collections.IList)
            {
                methodName = "ExNotIn";
            }

            switch (methodName)//系统级
            {
                case "StartsWith":
                    {
                        format = "({0} like concat({1},'%'))";
                        break;
                    }
                case "EndsWith":
                    {
                        format = "({0} like concat('%',{1}))";
                        break;
                    }
                case "Contains":
                    {
                        format = "({0} like concat('%',{1},'%'))";
                        break;
                    }
                case "NotContains":
                    {
                        format = "({0} not like concat('%',{1},'%'))";
                        break;
                    }
                case "Equals":
                    {
                        format = "({0} = {1} ";
                        break;
                    }
                case "ExIn":
                    {
                        format = "({0} in ({1}))";
                        break;
                    }
                case "ExNotIn":
                    {
                        format = "({0} not in ({1}))";
                        break;
                    }
                case "ExNotLike":
                    {
                        format = "({0} not like concat('%',{1},'%'))";
                        break;
                    }
                case "NotStartsWith":
                    {
                        format = "({0} not like concat({1},'%'))";
                        break;
                    }
                case "NotEndsWith":
                    {
                        format = "({0} not like concat('%',{1}))";
                        break;
                    }
            }
            #endregion

            #region 组装sql语句

            switch (methodName)
            {
                case "ExIn":
                case "ExNotIn":
                    {
                        this.Arguments.Remove(right);
                        var sb = new StringBuilder();
                        var ls = "";
                        foreach (var item in (value as System.Collections.IList))
                        {
                            var parName = this.builder.Db.AddParam(item);
                            sb.Append(ls + parName);
                            ls = ",";
                            this.Arguments.Add(parName, item);
                        }
                        if (!string.IsNullOrEmpty(sb.ToString()))
                        {
                            this.ConditionParts.Push(string.Format(format, left, sb));
                        }
                        else
                        {
                            if (methodName == "ExIn")
                            {
                                this.ConditionParts.Push(string.Format("1 = 0"));
                            }
                            else if (methodName == "ExNotIn")
                            {
                                this.ConditionParts.Push(string.Format("1 = 1"));
                            }
                        }
                        break;
                    }
                default:
                    {
                        this.ConditionParts.Push(string.Format(format, left, right));
                        break;
                    }
            }
            #endregion
        }

        #endregion 
    }
}
