using System;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    /// <summary>
    /// 两个查询
    /// </summary>
    /// <typeparam name="X"></typeparam>
    /// <typeparam name="Y"></typeparam>
    public class QueryTwoBulider<X, Y> : AbstractQueryBuilder<QueryTwoBulider<X, Y>>
    {
        public QueryTwoBulider(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        protected override QueryTwoBulider<X, Y> This()
        {
            return this;
        }
        public DoQueryTwo<DoQuerySql<X>, DoQuerySql<Y>> Do()
        {
            return _sqlBuilder.Do<DoQueryTwo<DoQuerySql<X>, DoQuerySql<Y>>>();
        }
        public async Task<DoQueryTwo<DoQuerySql<X>, DoQuerySql<Y>>> DoAsync()
        {
            return await _sqlBuilder.DoAsync<DoQueryTwo<DoQuerySql<X>, DoQuerySql<Y>>>();
        }
    }
}
