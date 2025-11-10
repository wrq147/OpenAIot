using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    /// <summary>
    /// 表达式工具
    /// </summary>
    public class ExpressionTool
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Convert(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " and ";
                case ExpressionType.Equal:
                    return " =";
                case ExpressionType.GreaterThan:
                    return " >";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    return null;
            }
        }
        /// <summary>
        /// 获取对象成员名称
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetMemberName(Expression expression)
        {
            if (expression is LambdaExpression)
            {
                expression = (expression as LambdaExpression).Body;
            }
            if (expression is UnaryExpression)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            var member = expression as MemberExpression;
            if (member == null)
            {
                return null;
            }
            if (member.Expression.Type.Name == "Nullable`1")
            {
                return (member.Expression as MemberExpression)?.Member?.Name;
            }
            return member?.Member?.Name;
        }
    }
}
