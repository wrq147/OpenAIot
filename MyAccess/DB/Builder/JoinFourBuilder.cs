using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class JoinFourBuilder<A, B, C, D> : QueryOneBuilder<A>
    {
        public JoinFourBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }


        /// <summary>
        /// A、B、C、D、E左链接
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFiveBuilder<A, B, C, D, E> LeftJoin<E>(Expression<Func<A, B, C, D, E, bool>> onCondi)
        {
            Type bEntityType = typeof(E);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty);
            string cc = this._sqlBuilder.GetOnByLambda<A, B, C, D, E>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFiveBuilder<A, B, C, D, E>(this._sqlBuilder);
        }

        /// <summary>
        /// A、B、C、D、E内链接
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFiveBuilder<A, B, C, D, E> InnerJoin<E>(Expression<Func<A, B, C, D, E, bool>> onCondi)
        {
            Type bEntityType = typeof(E);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "inner join");
            string cc = this._sqlBuilder.GetOnByLambda<A, B, C, D, E>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFiveBuilder<A, B, C, D, E>(this._sqlBuilder);
        }

        /// <summary>
        /// A、B、C、D、E全连接
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="onCondi"></param>
        /// <returns></returns>
        public JoinFiveBuilder<A, B, C, D, E> FullJoin<E>(Expression<Func<A, B, C, D, E, bool>> onCondi)
        {
            Type bEntityType = typeof(E);
            _sqlBuilder.AddJoin(SqlBuilder.NullSub, bEntityType, string.Empty, "full join");
            string cc = this._sqlBuilder.GetOnByLambda<A, B, C, D, E>(onCondi);
            _sqlBuilder.SubIdMaps[_sqlBuilder.SubIdMaps.Count - 1] = cc;
            return new JoinFiveBuilder<A, B, C, D, E>(this._sqlBuilder);
        }


        /// <summary>
        /// 将d映射到指定成员
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> MappingD(Expression<Func<A, object>> obj)
        {
            int preidx = DBMapping.GetIndexByPrefix("d");
            string name = ExpressionTool.GetMemberName(obj);
            _sqlBuilder.SubMaps[preidx] = name;
            return this;
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Count(Expression<Func<A, B, C, D, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D>(expression));
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0);
        }
        /// <summary>
        /// 查询数量（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<A, B, C, D, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D>(expression));
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0);
        }
        /// <summary>
        /// 判断是否存在指定记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Some(Expression<Func<A, B, C, D, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D>(expression)).Take(1);
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断是否存在指定记录（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<bool> SomeAsync(Expression<Func<A, B, C, D, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D>(expression)).Take(1);
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fields">表别名固定a、b、c、d</param>
        /// <returns></returns>
        public JoinFourBuilder<A, B, C, D> Where(Expression<Func<A, B, C, D, bool>> expression, string fields = null)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);
            if (string.IsNullOrEmpty(fields))
            {
                fields = GenerateFields(EntityType);
            }
            this.Append(string.Format("select {0} from {1} where ", fields, table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D>(expression));
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderExp"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public QueryOneBuilder<A> OrderBy(Expression<Func<A, B, C, D, object>> orderExp, OrderByType t)
        {
            BuildOrderBySql(orderExp, t);
            return this;
        }
    }
}
