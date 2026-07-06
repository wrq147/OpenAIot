using MyAccess.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyAccess.DB.Builder
{
    /// <summary>
    /// 单个查询
    /// </summary>
    /// <typeparam name="X"></typeparam>
    public class QueryOneBuilder<X> : AbstractQueryBuilder<QueryOneBuilder<X>>
    {
        public QueryOneBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        protected override QueryOneBuilder<X> This()
        {
            return this;
        }

        public QueryTwoBulider<X, T> Query<T>()
        {
            _sqlBuilder.AppendDiv();
            return new QueryTwoBulider<X, T>(_sqlBuilder);
        }
        public List<X> ToList()
        {
            return _sqlBuilder.Do<DoQuerySql<X>>().ToList();
        }
        public async Task<List<X>> ToListAsync()
        {
            return (await _sqlBuilder.DoAsync<DoQuerySql<X>>()).ToList();
        }
        public X ToFirst()
        {
            return _sqlBuilder.Do<DoQuerySql<X>>().ToFirst();
        }
        public async Task<X> ToFirstAsync()
        {
            return (await _sqlBuilder.DoAsync<DoQuerySql<X>>()).ToFirst();
        }
        public X ToFirst(X def)
        {
            return _sqlBuilder.Do<DoQuerySql<X>>().ToFirstOrDefault(def);
        }
        public async Task<X> ToFirstAsync(X def)
        {
            return (await _sqlBuilder.DoAsync<DoQuerySql<X>>()).ToFirstOrDefault(def);
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public List<X> ToPage(int page, int size, string orderby = "")
        {
            _hasOrderBy = true;
            _sqlBuilder.Comparable.SqlPage(page, size, orderby);
            return _sqlBuilder.Do<DoQuerySql<X>>().ToList();
        }
        /// <summary>
        /// 分页并返回总数
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public List<X> ToPage(int page, int size, ref int total, string orderby = "")
        {
            _hasOrderBy = true;
            _sqlBuilder.Comparable.SqlPageTotal(page, size, orderby);
            var pagert = _sqlBuilder.Do<DoQueryTwo<DoQuerySql<X>, DoQuerySql<int>>>();
            total = pagert.Second.ToFirst();
            return pagert.First.ToList();
        }
        /// <summary>
        /// 分页（异步）
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public async Task<List<X>> ToPageAsync(int page, int size, string orderby = "")
        {
            _hasOrderBy = true;
            _sqlBuilder.Comparable.SqlPage(page, size, orderby);
            return (await _sqlBuilder.DoAsync<DoQuerySql<X>>()).ToList();
        }

        /// <summary>
        /// 分页并返回总数（异步）
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="total"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public async Task<List<X>> ToPageAsync(int page, int size, RefAsync<int> total, string orderby = "")
        {
            _hasOrderBy = true;
            _sqlBuilder.Comparable.SqlPageTotal(page, size, orderby);
            var pagert = (await _sqlBuilder.DoAsync<DoQueryTwo<DoQuerySql<X>, DoQuerySql<int>>>());
            total.Value = pagert.Second.ToFirst();
            return pagert.First.ToList();
        }
        private bool _hasOrderBy = false;
        public QueryOneBuilder<X> OrderBy(Expression<Func<X, object>> orderExp, OrderByType t)
        {
            if (_hasOrderBy)
            {
                _sqlBuilder.Append(",");
            }
            else
            {
                _sqlBuilder.Append(" order by ");
            }
            string torderby = BuildOrderBySql(orderExp, t);
            _sqlBuilder.Append(torderby);
            return this;
        }
        protected string BuildOrderBySql(LambdaExpression exp, OrderByType t)
        {
            string prefix = string.Empty;

            Expression current = exp.Body;
            if (current is UnaryExpression un)
            {
                current = un.Operand;
            }
            if (current is not MemberExpression member)
            {
                return string.Empty;
            }

            int paramIndex = -1;
            string fieldName = member.Member.Name;
            if (member.Expression is MemberExpression tmpmem)
            {
                int tidx = _sqlBuilder.SubMaps.IndexOf(fieldName);
                if (tidx == -1)
                {
                    return string.Empty;
                }
                paramIndex = tidx + 1;
                fieldName = tmpmem.Member.Name;
            }
            else
            {
                ParameterExpression paramNode = member.Expression as ParameterExpression;
                if (_sqlBuilder.SubMaps != null && _sqlBuilder.SubMaps.Count > 0)
                {
                    for (int i = 0; i < exp.Parameters.Count; i++)
                    {
                        if (ReferenceEquals(exp.Parameters[i], paramNode))
                        {
                            paramIndex = i;
                            break;
                        }
                    }
                }
            }

            if (paramIndex != -1)
            {
                if (paramIndex == 0)
                {
                    prefix = "a.";
                }
                else
                {
                    prefix = DBMapping.GetSubPrefix(paramIndex - 1) + ".";
                }
            }
            string sort = t == OrderByType.Asc ? "ASC" : "DESC";
            return $"{prefix}{fieldName} {sort}";
        }
    }
}
