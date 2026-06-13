using System;
using System.Collections.Generic;
using System.Linq;
using SqlParser.Net;
using SqlParser.Net.Ast;
using SqlParser.Net.Ast.Expression;
using SqlParser.Net.Ast.Visitor;

namespace LLMService
{
    /// <summary>
    /// SqlParser.Net 原生AST SQL安全校验器
    /// </summary>
    public class AiSqlSecurityValidator
    {
        #region 配置项
        public HashSet<string> TableWhiteList { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public int MaxJoinCount { get; set; } = 3;
        public int MaxLimitRows { get; set; } = 500;
        public DbType DbDialect { get; set; } = DbType.MySql;
        #endregion

        public bool Validate(string sql, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (string.IsNullOrWhiteSpace(sql))
            {
                errorMsg = "SQL不能为空";
                return false;
            }

            SqlExpression ast;
            try
            {
                // 官方标准解析返回 SqlExpression
                ast = DbUtils.Parse(sql, DbDialect);
            }
            catch (Exception ex)
            {
                errorMsg = $"SQL语法解析失败：{ex.Message}";
                return false;
            }

            // 拦截UNION语句
            if (ast is SqlUnionQueryExpression)
            {
                errorMsg = "禁止使用UNION/UNION ALL合并多段查询结果";
                return false;
            }

            // 仅允许SELECT顶层语句
            if (ast is not SqlSelectExpression selectExpr)
            {
                string stmtType = ast switch
                {
                    SqlInsertExpression => "INSERT",
                    SqlUpdateExpression => "UPDATE",
                    SqlDeleteExpression => "DELETE",
                    _ => ast.GetType().Name
                };
                errorMsg = $"仅允许SELECT查询，禁止{stmtType}操作";
                return false;
            }

            SqlSelectQueryExpression query = (SqlSelectQueryExpression)selectExpr.Query;
            // 规则1：禁止 SELECT * 以及 t.*
            if (CheckContainsSelectAll(query.Columns))
            {
                errorMsg = "禁止使用SELECT * 或 表别名.*，必须显式指定字段";
                return false;
            }

            // 规则2：遍历AST提取所有表名
            var tableVisitor = new ExtractTableVisitor();
            selectExpr.Accept(tableVisitor);
            var tableNames = tableVisitor.TableNames.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (!tableNames.Any())
            {
                errorMsg = "未解析到任何数据表";
                return false;
            }

            foreach (var tbl in tableNames)
            {
                if (!TableWhiteList.Contains(tbl))
                {
                    errorMsg = $"数据表[{tbl}]不在访问白名单，禁止访问";
                    return false;
                }
            }

            // 规则3：统计JOIN表数量
            var joinCounter = new JoinTableCountVisitor();
            selectExpr.Accept(joinCounter);
            if (joinCounter.TableTotal > MaxJoinCount)
            {
                errorMsg = $"联表数量{joinCounter.TableTotal}，超过上限{MaxJoinCount}";
                return false;
            }

            // 规则4：禁止无WHERE全表扫描
            if (query.Where == null)
            {
                errorMsg = "必须携带WHERE过滤条件，禁止无限制全表查询";
                return false;
            }

            // 规则5：校验LIMIT分页行数
            if (query.Limit == null)
            {
                errorMsg = "必须显式添加LIMIT限制返回行数";
                return false;
            }
            long limitRows = Convert.ToInt64(((SqlNumberExpression)query.Limit.RowCount).Value);
            if (limitRows <= 0 || limitRows > MaxLimitRows)
            {
                errorMsg = $"LIMIT行数必须在1~{MaxLimitRows}之间，当前{limitRows}";
                return false;
            }

            // 规则6：禁止WITH CTE
            if (query.WithSubQuerys != null && query.WithSubQuerys.Any())
            {
                errorMsg = "禁止使用WITH CTE公用表表达式";
                return false;
            }


            return true;
        }

        private bool CheckContainsSelectAll(List<SqlSelectItemExpression> items)
        {
            foreach (var item in items)
            {
                if (item.Body is SqlAllColumnExpression)
                    return true;
                if (item.Body is SqlPropertyExpression prop && prop.Name.Value == "*")
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 内部Visitor：提取所有物理表名
        /// </summary>
        private class ExtractTableVisitor : BaseAstVisitor
        {
            public HashSet<string> TableNames { get; } = new(StringComparer.OrdinalIgnoreCase);
            public bool HasFromSubQuery { get; private set; }

            // 访问普通物理表
            public override SqlExpression VisitSqlTableExpression(SqlTableExpression expr, VisitContext context = null)
            {
                if (expr.Name != null)
                {
                    TableNames.Add(expr.Name.Value);
                }
                return base.VisitSqlTableExpression(expr, context);
            }

            // 访问嵌套子查询：FROM (SELECT ...) 标记违规
            public override SqlExpression VisitSqlSelectExpression(SqlSelectExpression expr, VisitContext context = null)
            {
                // 判断当前select是作为From子句的子查询
                if (context.Parent != null && context.Parent is SqlSelectQueryExpression q && q.From == expr)
                {
                    HasFromSubQuery = true;
                }
                return base.VisitSqlSelectExpression(expr, context);
            }
        }
        /// <summary>
        /// 统计所有参与联查的物理表总数
        /// </summary>
        private class JoinTableCountVisitor : BaseAstVisitor
        {
            public int TableTotal { get; private set; }

            public override SqlExpression VisitSqlTableExpression(SqlTableExpression expr, VisitContext context = null)
            {
                TableTotal++;
                return base.VisitSqlTableExpression(expr, context);
            }
        }

    }


}
