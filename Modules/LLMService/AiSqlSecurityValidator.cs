using Mysqlx.Expr;
using NPOI.SS.Formula.Functions;
using SqlParser.Net;
using SqlParser.Net.Ast;
using SqlParser.Net.Ast.Expression;
using SqlParser.Net.Ast.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LLMService
{
    /// <summary>
    /// SqlParser.Net 原生AST SQL安全校验器
    /// </summary>
    public static class AiSqlSecurityValidator
    {
        #region 配置项
        private const int MaxJoinCount = 10;
        private const DbType DbDialect = DbType.MySql;
        #endregion

        public static bool Validate(string sql, Dictionary<string, string> tableWhiteList, out List<string> tbs, out string errorMsg)
        {
            tbs = new List<string>();
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


            // 规则1：遍历AST提取所有表名
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
                if (!tableWhiteList.ContainsKey(tbl))
                {
                    errorMsg = $"数据表[{tbl}]不在访问白名单，禁止访问";
                    return false;
                }
            }
            tbs = tableNames;

            // 规则2：统计JOIN表数量
            if (tableVisitor.TableTotal > MaxJoinCount)
            {
                errorMsg = $"联表数量{tableVisitor.TableTotal}，超过上限{MaxJoinCount}";
                return false;
            }

            SqlSelectQueryExpression query = (SqlSelectQueryExpression)selectExpr.Query;
            // 规则3：禁止无WHERE全表扫描
            if (query.Where == null)
            {
                errorMsg = "必须携带WHERE过滤条件，禁止无限制全表查询";
                return false;
            }

            // 规则4：校验LIMIT分页行数
            if (query.Limit == null)
            {
                errorMsg = "必须显式添加LIMIT限制返回行数";
                return false;
            }


            // 规则5：禁止WITH CTE
            if (query.WithSubQuerys != null && query.WithSubQuerys.Any())
            {
                errorMsg = "禁止使用WITH CTE公用表表达式";
                return false;
            }

            // 规则6：交验OrgId过滤
            if (!tableVisitor.HasValidOrgCondition)
            {
                errorMsg = "查询语句超过了企业的查询权限";
                return false;
            }
            return true;
        }
        private class NoLimitVisitor : SqlGenerationAstVisitor
        {
            public NoLimitVisitor(DbType t) : base(t)
            {
            }
            public override SqlExpression VisitSqlLimitExpression(SqlLimitExpression sqlLimitExpression, VisitContext context = null)
            {
                return sqlLimitExpression;
            }
        }

        /// <summary>
        /// 内部Visitor：提取所有物理表名
        /// </summary>
        private class ExtractTableVisitor : IAstVisitor
        {
            public int TableTotal { get; private set; }
            public HashSet<string> TableNames { get; } = new(StringComparer.OrdinalIgnoreCase);
            public bool HasValidOrgCondition { get; private set; } = false;
            public SqlExpression VisitSqlAllColumnExpression(SqlAllColumnExpression sqlAllColumnExpression, VisitContext context = null)
            {
                return sqlAllColumnExpression;
            }

            public SqlExpression VisitSqlAllExpression(SqlAllExpression sqlAllExpression, VisitContext context = null)
            {
                sqlAllExpression.Body?.Accept(this);
                return sqlAllExpression;
            }

            public SqlExpression VisitSqlAnyExpression(SqlAnyExpression sqlAnyExpression, VisitContext context = null)
            {
                sqlAnyExpression.Body?.Accept(this);
                return sqlAnyExpression;
            }

            public SqlExpression VisitSqlArrayExpression(SqlArrayExpression sqlArrayExpression, VisitContext context = null)
            {
                if (sqlArrayExpression.Items != null)
                {
                    foreach (var item in sqlArrayExpression.Items)
                    {
                        item.Accept(this);
                    }
                }

                return sqlArrayExpression;
            }

            public SqlExpression VisitSqlArrayIndexExpression(SqlArrayIndexExpression sqlArrayIndexExpression, VisitContext context = null)
            {
                sqlArrayIndexExpression.Body?.Accept(this);
                sqlArrayIndexExpression.Index?.Accept(this);
                return sqlArrayIndexExpression;
            }

            public SqlExpression VisitSqlArraySliceExpression(SqlArraySliceExpression sqlArraySliceExpression, VisitContext context = null)
            {
                sqlArraySliceExpression.Body?.Accept(this);
                sqlArraySliceExpression.StartIndex?.Accept(this);
                sqlArraySliceExpression.EndIndex?.Accept(this);
                return sqlArraySliceExpression;
            }

            public SqlExpression VisitSqlAtTimeZoneExpression(SqlAtTimeZoneExpression sqlAtTimeZoneExpression, VisitContext context = null)
            {
                sqlAtTimeZoneExpression.TimeZone?.Accept(this);
                return sqlAtTimeZoneExpression;
            }

            public SqlExpression VisitSqlBetweenAndExpression(SqlBetweenAndExpression sqlBetweenAndExpression, VisitContext context = null)
            {
                sqlBetweenAndExpression.Begin?.Accept(this);
                sqlBetweenAndExpression.End?.Accept(this);
                sqlBetweenAndExpression.Body?.Accept(this);
                return sqlBetweenAndExpression;
            }

            public SqlExpression VisitSqlBinaryExpression(SqlBinaryExpression sqlBinaryExpression, VisitContext context = null)
            {
                sqlBinaryExpression.Left?.Accept(this);
                sqlBinaryExpression.Right?.Accept(this);
                sqlBinaryExpression.Collate?.Accept(this);
                if (sqlBinaryExpression.Operator == SqlBinaryOperator.EqualTo)
                {
                    string col = TryGetColumnName(sqlBinaryExpression.Left);
                    if (col.Equals("OrgId", StringComparison.OrdinalIgnoreCase))
                        HasValidOrgCondition = true;
                }
                return sqlBinaryExpression;
            }

            public SqlExpression VisitSqlBoolExpression(SqlBoolExpression sqlBoolExpression, VisitContext context = null)
            {
                return sqlBoolExpression;
            }

            public SqlExpression VisitSqlCaseExpression(SqlCaseExpression sqlCaseExpression, VisitContext context = null)
            {
                if (sqlCaseExpression.Items != null)
                {
                    foreach (var item in sqlCaseExpression.Items)
                    {
                        item.Accept(this);
                    }
                }

                sqlCaseExpression.Value?.Accept(this);
                return sqlCaseExpression;
            }

            public SqlExpression VisitSqlCaseItemExpression(SqlCaseItemExpression sqlCaseItemExpression, VisitContext context = null)
            {
                sqlCaseItemExpression.Value?.Accept(this);
                return sqlCaseItemExpression;
            }

            public SqlExpression VisitSqlCollateExpression(SqlCollateExpression sqlCollateExpression, VisitContext context = null)
            {
                sqlCollateExpression.Body?.Accept(this);
                return sqlCollateExpression;
            }

            public SqlExpression VisitSqlConnectByExpression(SqlConnectByExpression sqlConnectByExpression, VisitContext context = null)
            {
                sqlConnectByExpression.Body?.Accept(this);
                sqlConnectByExpression.OrderBy?.Accept(this);
                sqlConnectByExpression.StartWith?.Accept(this);
                return sqlConnectByExpression;
            }

            public SqlExpression VisitSqlDeleteExpression(SqlDeleteExpression sqlDeleteExpression, VisitContext context = null)
            {
                sqlDeleteExpression.Body?.Accept(this);
                sqlDeleteExpression.Table?.Accept(this);
                sqlDeleteExpression.Where?.Accept(this);

                return sqlDeleteExpression;
            }

            public SqlExpression VisitSqlExistsExpression(SqlExistsExpression sqlExistsExpression, VisitContext context = null)
            {
                sqlExistsExpression.Body?.Accept(this);
                return sqlExistsExpression;
            }

            public SqlExpression VisitSqlFunctionCallExpression(SqlFunctionCallExpression sqlFunctionCallExpression, VisitContext context = null)
            {
                if (sqlFunctionCallExpression.Arguments != null)
                {
                    foreach (var item in sqlFunctionCallExpression.Arguments)
                    {
                        item.Accept(this);
                    }
                }

                sqlFunctionCallExpression.CaseAsTargetType?.Accept(this);
                sqlFunctionCallExpression.Collate?.Accept(this);
                sqlFunctionCallExpression.FromSource?.Accept(this);
                sqlFunctionCallExpression.Name?.Accept(this);
                sqlFunctionCallExpression.Over?.Accept(this);
                sqlFunctionCallExpression.WithinGroup?.Accept(this);

                return sqlFunctionCallExpression;
            }

            public SqlExpression VisitSqlGroupByExpression(SqlGroupByExpression sqlGroupByExpression, VisitContext context = null)
            {
                sqlGroupByExpression.Having?.Accept(this);
                if (sqlGroupByExpression.Items != null)
                {
                    foreach (var item in sqlGroupByExpression.Items)
                    {
                        item.Accept(this);
                    }
                }

                return sqlGroupByExpression;
            }

            public SqlExpression VisitSqlHintExpression(SqlHintExpression sqlHintExpression, VisitContext context = null)
            {
                sqlHintExpression.Body?.Accept(this);
                return sqlHintExpression;
            }

            public SqlExpression VisitSqlIdentifierExpression(SqlIdentifierExpression sqlIdentifierExpression, VisitContext context = null)
            {
                sqlIdentifierExpression.Collate?.Accept(this);
                return sqlIdentifierExpression;
            }

            public SqlExpression VisitSqlInExpression(SqlInExpression sqlInExpression, VisitContext context = null)
            {
                sqlInExpression.Body?.Accept(this);
                sqlInExpression.SubQuery?.Accept(this);
                if (sqlInExpression.TargetList != null)
                {
                    foreach (var item in sqlInExpression.TargetList)
                    {
                        item.Accept(this);
                    }
                }

                return sqlInExpression;
            }

            public SqlExpression VisitSqlInsertExpression(SqlInsertExpression sqlInsertExpression, VisitContext context = null)
            {
                if (sqlInsertExpression.Columns != null)
                {
                    foreach (var item in sqlInsertExpression.Columns)
                    {
                        item.Accept(this);
                    }
                }

                sqlInsertExpression.FromSelect?.Accept(this);
                sqlInsertExpression.Returning?.Accept(this);
                sqlInsertExpression.Table?.Accept(this);
                if (sqlInsertExpression.ValuesList != null)
                {
                    foreach (var top in sqlInsertExpression.ValuesList)
                    {
                        foreach (var bt in top)
                        {
                            bt.Accept(this);
                        }
                    }
                }
                if (sqlInsertExpression.WithSubQuerys != null)
                {
                    foreach (var sb in sqlInsertExpression.WithSubQuerys)
                    {
                        sb.Accept(this);
                    }
                }
                return sqlInsertExpression;
            }

            public SqlExpression VisitSqlIntervalExpression(SqlIntervalExpression sqlIntervalExpression, VisitContext context = null)
            {
                sqlIntervalExpression.Body?.Accept(this);
                sqlIntervalExpression.Unit?.Accept(this);
                return sqlIntervalExpression;
            }

            public SqlExpression VisitSqlJoinTableExpression(SqlJoinTableExpression sqlJoinTableExpression, VisitContext context = null)
            {
                sqlJoinTableExpression.Conditions?.Accept(this);
                sqlJoinTableExpression.Left?.Accept(this);
                sqlJoinTableExpression.Right?.Accept(this);

                return sqlJoinTableExpression;
            }

            public SqlExpression VisitSqlLimitExpression(SqlLimitExpression sqlLimitExpression, VisitContext context = null)
            {
                sqlLimitExpression.Offset?.Accept(this);
                sqlLimitExpression.RowCount?.Accept(this);

                return sqlLimitExpression;
            }

            public SqlExpression VisitSqlNotExpression(SqlNotExpression sqlNotExpression, VisitContext context = null)
            {
                sqlNotExpression.Body?.Accept(this);

                return sqlNotExpression;
            }

            public SqlExpression VisitSqlNullExpression(SqlNullExpression sqlNullExpression, VisitContext context = null)
            {
                return sqlNullExpression;
            }

            public SqlExpression VisitSqlNumberExpression(SqlNumberExpression sqlNumberExpression, VisitContext context = null)
            {
                return sqlNumberExpression;
            }

            public SqlExpression VisitSqlOrderByExpression(SqlOrderByExpression sqlOrderByExpression, VisitContext context = null)
            {
                if (sqlOrderByExpression.Items != null)
                {
                    foreach (var item in sqlOrderByExpression.Items)
                    {
                        item.Accept(this);
                    }
                }

                return sqlOrderByExpression;
            }

            public SqlExpression VisitSqlOrderByItemExpression(SqlOrderByItemExpression sqlOrderByItemExpression, VisitContext context = null)
            {
                sqlOrderByItemExpression.Body?.Accept(this);
                return sqlOrderByItemExpression;
            }

            public SqlExpression VisitSqlOverExpression(SqlOverExpression sqlOverExpression, VisitContext context = null)
            {
                sqlOverExpression.OrderBy?.Accept(this);
                sqlOverExpression.PartitionBy?.Accept(this);

                return sqlOverExpression;
            }

            public SqlExpression VisitSqlPartitionByExpression(SqlPartitionByExpression sqlPartitionByExpression, VisitContext context = null)
            {
                if (sqlPartitionByExpression.Items != null)
                {
                    foreach (var item in sqlPartitionByExpression.Items)
                    {
                        item.Accept(this);
                    }
                }

                return sqlPartitionByExpression;
            }

            public SqlExpression VisitSqlPivotTableExpression(SqlPivotTableExpression sqlPivotTableExpression, VisitContext context = null)
            {
                sqlPivotTableExpression.Alias?.Accept(this);
                sqlPivotTableExpression.For?.Accept(this);
                sqlPivotTableExpression.FunctionCall?.Accept(this);
                if (sqlPivotTableExpression.In != null)
                {
                    foreach (var item in sqlPivotTableExpression.In)
                    {
                        item.Accept(this);
                    }
                }

                sqlPivotTableExpression.SubQuery?.Accept(this);

                return sqlPivotTableExpression;
            }

            public SqlExpression VisitSqlPropertyExpression(SqlPropertyExpression sqlPropertyExpression, VisitContext context = null)
            {
                sqlPropertyExpression.Collate?.Accept(this);
                sqlPropertyExpression.Name?.Accept(this);
                sqlPropertyExpression.Table?.Accept(this);

                return sqlPropertyExpression;
            }

            public SqlExpression VisitSqlReferenceTableExpression(SqlReferenceTableExpression sqlReferenceTableExpression, VisitContext context = null)
            {
                sqlReferenceTableExpression.Alias?.Accept(this);
                sqlReferenceTableExpression.FunctionCall?.Accept(this);

                return sqlReferenceTableExpression;
            }

            public SqlExpression VisitSqlRegexExpression(SqlRegexExpression sqlRegexExpression, VisitContext context = null)
            {
                sqlRegexExpression.Body?.Accept(this);
                sqlRegexExpression.Collate?.Accept(this);
                sqlRegexExpression.RegEx?.Accept(this);

                return sqlRegexExpression;
            }

            public SqlExpression VisitSqlReturningExpression(SqlReturningExpression sqlReturningExpression, VisitContext context = null)
            {
                if (sqlReturningExpression.IntoVariables != null)
                {
                    foreach (var tmpitem in sqlReturningExpression.IntoVariables)
                    {
                        tmpitem.Accept(this);
                    }
                }
                if (sqlReturningExpression.Items != null)
                {
                    foreach (var item in sqlReturningExpression.Items)
                    {
                        item.Accept(this);
                    }
                }


                return sqlReturningExpression;
            }

            public SqlExpression VisitSqlSelectExpression(SqlSelectExpression sqlSelectExpression, VisitContext context = null)
            {

                sqlSelectExpression.Alias?.Accept(this);

                sqlSelectExpression.Query?.Accept(this);

                sqlSelectExpression.OrderBy?.Accept(this);

                sqlSelectExpression.Limit?.Accept(this);
                return sqlSelectExpression;
            }

            public SqlExpression VisitSqlSelectItemExpression(SqlSelectItemExpression sqlSelectItemExpression, VisitContext context = null)
            {
                sqlSelectItemExpression.Body?.Accept(this);
                sqlSelectItemExpression.Alias?.Accept(this);

                return sqlSelectItemExpression;
            }

            public SqlExpression VisitSqlSelectQueryExpression(SqlSelectQueryExpression sqlSelectQueryExpression, VisitContext context = null)
            {
                sqlSelectQueryExpression.Into?.Accept(this);
                sqlSelectQueryExpression.Where?.Accept(this);
                sqlSelectQueryExpression.ConnectBy?.Accept(this);
                sqlSelectQueryExpression.From?.Accept(this);
                sqlSelectQueryExpression.OrderBy?.Accept(this);
                sqlSelectQueryExpression.Limit?.Accept(this);
                sqlSelectQueryExpression.Top?.Accept(this);
                return sqlSelectQueryExpression;
            }

            public SqlExpression VisitSqlStringExpression(SqlStringExpression sqlStringExpression, VisitContext context = null)
            {
                sqlStringExpression.Collate?.Accept(this);

                return sqlStringExpression;
            }

            public SqlExpression VisitSqlTableExpression(SqlTableExpression sqlTableExpression, VisitContext context = null)
            {
                sqlTableExpression.Alias?.Accept(this);
                sqlTableExpression.Database?.Accept(this);
                sqlTableExpression.DbLink?.Accept(this);
                if (sqlTableExpression.Hints != null)
                {
                    foreach (var item in sqlTableExpression.Hints)
                    {
                        item.Accept(this);
                    }
                }

                sqlTableExpression.Name?.Accept(this);
                sqlTableExpression.Schema?.Accept(this);
                if (sqlTableExpression.Name != null)
                {
                    TableNames.Add(sqlTableExpression.Name.Value);
                }
                TableTotal++;
                return sqlTableExpression;
            }

            public SqlExpression VisitSqlTimeUnitExpression(SqlTimeUnitExpression sqlTimeUnitExpression, VisitContext context = null)
            {
                return sqlTimeUnitExpression;
            }

            public SqlExpression VisitSqlTopExpression(SqlTopExpression sqlTopExpression, VisitContext context = null)
            {
                sqlTopExpression.Body?.Accept(this);
                return sqlTopExpression;
            }

            public SqlExpression VisitSqlUnionQueryExpression(SqlUnionQueryExpression sqlUnionQueryExpression, VisitContext context = null)
            {
                sqlUnionQueryExpression.Left?.Accept(this);
                sqlUnionQueryExpression.Right?.Accept(this);

                return sqlUnionQueryExpression;
            }

            public SqlExpression VisitSqlUpdateExpression(SqlUpdateExpression sqlUpdateExpression, VisitContext context = null)
            {
                sqlUpdateExpression.From?.Accept(this);
                if (sqlUpdateExpression.Items != null)
                {
                    foreach (var item in sqlUpdateExpression.Items)
                    {
                        item.Accept(this);
                    }
                }

                sqlUpdateExpression.Table?.Accept(this);
                sqlUpdateExpression.Where?.Accept(this);
                if (sqlUpdateExpression.WithSubQuerys != null)
                {
                    foreach (var witem in sqlUpdateExpression.WithSubQuerys)
                    {
                        witem.Accept(this);
                    }
                }

                return sqlUpdateExpression;
            }

            public SqlExpression VisitSqlVariableExpression(SqlVariableExpression sqlVariableExpression, VisitContext context = null)
            {
                sqlVariableExpression.Collate?.Accept(this);
                return sqlVariableExpression;
            }

            public SqlExpression VisitSqlWithinGroupExpression(SqlWithinGroupExpression sqlWithinGroupExpression, VisitContext context = null)
            {
                sqlWithinGroupExpression.OrderBy?.Accept(this);
                return sqlWithinGroupExpression;
            }

            public SqlExpression VisitSqlWithSubQueryExpression(SqlWithSubQueryExpression sqlWithSubQueryExpression, VisitContext context = null)
            {
                sqlWithSubQueryExpression.Alias?.Accept(this);
                if (sqlWithSubQueryExpression.Columns != null)
                {
                    foreach (var item in sqlWithSubQueryExpression.Columns)
                    {
                        item.Accept(this);
                    }
                }

                sqlWithSubQueryExpression.FromSelect?.Accept(this);
                return sqlWithSubQueryExpression;
            }


            private string TryGetColumnName(SqlExpression expr)
            {
                if (expr is SqlPropertyExpression prop)
                    return prop.Name.Value;
                if (expr is SqlIdentifierExpression id)
                    return id.Value;
                return null;
            }


        }

    }


}
