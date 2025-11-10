using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class JoinThreeBuilder<A, B, C> : AbstractQueryBuilder<QueryOneBuilder<A>>
    {
        public JoinThreeBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        protected override QueryOneBuilder<A> This()
        {
            return new QueryOneBuilder<A>(this._sqlBuilder);
        }
        /// <summary>
        /// A与D左链接
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> LeftJoin<D>(Expression<Func<A, D, bool>> onCondi)
        {
            Type bEntityType = typeof(D);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty);
            string cc = this._sqlBuilder.GetOnByLambda<A, D>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFourBuilder<A, B, C, D>(this._sqlBuilder);
        }
        /// <summary>
        /// B与D左链接
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> LeftJoin<D>(Expression<Func<B, D, bool>> onCondi)
        {
            Type bEntityType = typeof(D);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty);
            string cc = this._sqlBuilder.GetOnByLambda<B, D>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFourBuilder<A, B, C, D>(this._sqlBuilder);
        }
        /// <summary>
        /// C与D左链接
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> LeftJoin<D>(Expression<Func<C, D, bool>> onCondi)
        {
            Type bEntityType = typeof(D);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty);
            string cc = this._sqlBuilder.GetOnByLambda<C, D>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFourBuilder<A, B, C, D>(this._sqlBuilder);
        }
        /// <summary>
        /// A与D内链接
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> InnerJoin<D>(Expression<Func<A, D, bool>> onCondi)
        {
            Type bEntityType = typeof(D);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "inner join");
            string cc = this._sqlBuilder.GetOnByLambda<A, D>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFourBuilder<A, B, C, D>(this._sqlBuilder);
        }
        /// <summary>
        /// B与D内链接
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> InnerJoin<D>(Expression<Func<B, D, bool>> onCondi)
        {
            Type bEntityType = typeof(D);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "inner join");
            string cc = this._sqlBuilder.GetOnByLambda<B, D>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFourBuilder<A, B, C, D>(this._sqlBuilder);
        }
        /// <summary>
        /// C与D内链接
        /// </summary>
        /// <typeparam name="D"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> InnerJoin<D>(Expression<Func<C, D, bool>> onCondi)
        {
            Type bEntityType = typeof(D);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "inner join");
            string cc = this._sqlBuilder.GetOnByLambda<C, D>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFourBuilder<A, B, C, D>(this._sqlBuilder);
        }
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Count(Expression<Func<A, B, C, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C>(expression));
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0);
        }
        /// <summary>
        /// 查询数量（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<A, B, C, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C>(expression));
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0);
        }
        /// <summary>
        /// 判断是否存在指定记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Some(Expression<Func<A, B, C, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C>(expression)).Take(1);
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断是否存在指定记录（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<bool> SomeAsync(Expression<Func<A, B, C, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C>(expression)).Take(1);
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fields">表别名固定a、b、c</param>
        /// <returns></returns>
        public QueryOneBuilder<A> Where(Expression<Func<A, B, C, bool>> expression, string fields = null)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);
            if (string.IsNullOrEmpty(fields))
            {
                fields = GenerateFields(EntityType);
            }
            this.Append(string.Format("select {0} from {1} where ", fields, table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C>(expression));
            return This();
        }
    }
}
