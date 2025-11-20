using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class JoinTwoBuilder<A, B> : AbstractQueryBuilder<QueryOneBuilder<A>>
    {
        public JoinTwoBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }

        protected override QueryOneBuilder<A> This()
        {
            return new QueryOneBuilder<A>(this._sqlBuilder);
        }


        /// <summary>
        /// A、B、C左链接
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinThreeBuilder<A, B, C> LeftJoin<C>(Expression<Func<A, B, C, bool>> onCondi)
        {
            Type bEntityType = typeof(C);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty);
            string cc = this._sqlBuilder.GetOnByLambda<A, B, C>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinThreeBuilder<A, B, C>(this._sqlBuilder);
        }

        /// <summary>
        /// A、B、C右链接
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinThreeBuilder<A, B, C> RightJoin<C>(Expression<Func<A, B, C, bool>> onCondi)
        {
            Type bEntityType = typeof(C);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "right join");
            string cc = this._sqlBuilder.GetOnByLambda<A, B, C>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinThreeBuilder<A, B, C>(this._sqlBuilder);
        }

        /// <summary>
        /// A、B、C内链接
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinThreeBuilder<A, B, C> InnerJoin<C>(Expression<Func<A, B, C, bool>> onCondi)
        {
            Type bEntityType = typeof(C);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "inner join");
            string cc = this._sqlBuilder.GetOnByLambda<A, B, C>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinThreeBuilder<A, B, C>(this._sqlBuilder);
        }

        /// <summary>
        /// A、B、C全连接
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinThreeBuilder<A, B, C> FullJoin<C>(Expression<Func<A, B, C, bool>> onCondi)
        {
            Type bEntityType = typeof(C);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "full join");
            string cc = this._sqlBuilder.GetOnByLambda<A, B, C>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinThreeBuilder<A, B, C>(this._sqlBuilder);
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Count(Expression<Func<A, B, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B>(expression));
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0);
        }
        /// <summary>
        /// 查询数量（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<A, B, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B>(expression));
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0);
        }
        /// <summary>
        /// 判断是否存在指定记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Some(Expression<Func<A, B, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B>(expression)).Take(1);
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断是否存在指定记录（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<bool> SomeAsync(Expression<Func<A, B, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B>(expression)).Take(1);
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fields">表别名固定a、b</param>
        /// <returns></returns>
        public QueryOneBuilder<A> Where(Expression<Func<A, B, bool>> expression, string fields = null)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);
            if (string.IsNullOrEmpty(fields))
            {
                fields = GenerateFields(EntityType);
            }
            this.Append(string.Format("select {0} from {1} where ", fields, table)).Append(this._sqlBuilder.GetWhereByLambda<A, B>(expression));
            return This();
        }

    }
}
