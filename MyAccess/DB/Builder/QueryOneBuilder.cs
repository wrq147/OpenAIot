using MyAccess.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    /// <summary>
    /// 单个查询
    /// </summary>
    /// <typeparam name="X"></typeparam>
    public class QueryOneBuilder<X> : AbstractQueryBuilder<QueryOneBuilder<X>>
    {
        public QueryOneBuilder(SqlBuilder sqlBuilder):base(sqlBuilder)
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
            _sqlBuilder.Comparable.SqlPageTotal(page, size, orderby);
            var pagert = (await _sqlBuilder.DoAsync<DoQueryTwo<DoQuerySql<X>, DoQuerySql<int>>>());
            total.Value = pagert.Second.ToFirst();
            return pagert.First.ToList();
        }

    }
}
