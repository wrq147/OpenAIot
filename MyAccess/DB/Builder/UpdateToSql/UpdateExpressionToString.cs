using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder.UpdateToSql
{
    public class UpdateExpressionToString
    {
        /// <summary>
        /// 将一元成员运算表达式（如 it => it.Num + 1）转换为运算字符串（如 Num + 1）
        /// </summary>
        /// <typeparam name="T">表达式输入类型</typeparam>
        /// <typeparam name="TResult">表达式返回类型</typeparam>
        /// <param name="expression">待转换的表达式</param>
        /// <returns>简化后的运算字符串</returns>
        /// <exception cref="ArgumentException">不支持的表达式类型时抛出</exception>
        public static string Convert<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            // 提取表达式主体（it.Num + 1）
            var body = expression.Body;

            // 构建字符串拼接器
            var sb = new StringBuilder();

            // 解析表达式节点
            ParseExpressionNode(body, sb);

            return sb.ToString();
        }

        /// <summary>
        /// 递归解析表达式节点（支持二元运算、成员访问、常量、参数等）
        /// </summary>
        private static void ParseExpressionNode(Expression node, StringBuilder sb)
        {
            switch (node.NodeType)
            {
                // 处理二元运算（+、-、*、/ 等）
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    var binaryExpr = (BinaryExpression)node;
                    // 解析左操作数
                    ParseExpressionNode(binaryExpr.Left, sb);
                    // 添加运算符
                    sb.Append(GetOperatorSymbol(binaryExpr.NodeType));
                    // 解析右操作数
                    ParseExpressionNode(binaryExpr.Right, sb);
                    break;

                // 处理成员访问（如 it.Num）
                case ExpressionType.MemberAccess:
                    var memberExpr = (MemberExpression)node;
                    // 只保留成员名（Num），忽略参数（it）
                    sb.Append(memberExpr.Member.Name);
                    break;

                // 处理常量（如 1、"abc" 等）
                case ExpressionType.Constant:
                    var constantExpr = (ConstantExpression)node;
                    sb.Append(constantExpr.Value);
                    break;

                // 处理参数（如 it，直接忽略）
                case ExpressionType.Parameter:
                    // 参数本身无需拼接（如 it）
                    break;

                // 扩展：可添加更多表达式类型支持（如方法调用、一元运算等）
                default:
                    throw new ArgumentException($"不支持的表达式类型：{node.NodeType}");
            }
        }

        /// <summary>
        /// 将表达式运算符枚举转换为符号字符串
        /// </summary>
        private static string GetOperatorSymbol(ExpressionType type)
        {
            return type switch
            {
                ExpressionType.Add => " + ",
                ExpressionType.Subtract => " - ",
                ExpressionType.Multiply => " * ",
                ExpressionType.Divide => " / ",
                _ => throw new ArgumentException($"不支持的运算符：{type}")
            };
        }
    }
}
