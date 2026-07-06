using MyAccess.Core;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class JoinFiveBuilder<A, B, C, D, E> : QueryOneBuilder<A>
    {
        public JoinFiveBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }


        /// <summary>
        /// 将e映射到指定成员
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JoinFiveBuilder<A, B, C, D, E> MappingE(Expression<Func<A, object>> obj)
        {
            int preidx = DBMapping.GetIndexByPrefix("e");
            string name = ExpressionTool.GetMemberName(obj);
            _sqlBuilder.SubMaps[preidx] = name;
            return this;
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Count(Expression<Func<A, B, C, D, E, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D, E>(expression));
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0);
        }
        /// <summary>
        /// 查询数量（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<A, B, C, D, E, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D, E>(expression));
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0);
        }
        /// <summary>
        /// 判断是否存在指定记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Some(Expression<Func<A, B, C, D, E, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D, E>(expression)).Take(1);
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断是否存在指定记录（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<bool> SomeAsync(Expression<Func<A, B, C, D, E, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D, E>(expression)).Take(1);
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fields">表别名固定a、b、c、d、e</param>
        /// <returns></returns>
        public JoinFiveBuilder<A, B, C, D, E> Where(Expression<Func<A, B, C, D, E, bool>> expression, string fields = null)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);
            if (string.IsNullOrEmpty(fields))
            {
                fields = GenerateFields(EntityType);
            }
            this.Append(string.Format("select {0} from {1} where ", fields, table)).Append(this._sqlBuilder.GetWhereByLambda<A, B, C, D, E>(expression));
            return this;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="orderExp"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public QueryOneBuilder<A> OrderBy(Expression<Func<A, B, C, D, E, object>> orderExp, OrderByType t)
        {
            var torderstr = BuildOrderBySql(orderExp, t);
            if (!string.IsNullOrEmpty(torderstr))
            {
                this._sqlBuilder.Append(torderstr);
            }
            return this;
        }
        public List<A> ToPage(int page, int size, params OrderByModel<A, B, C, D, E>[] orders)
        {
            StringBuilder tmporder = new StringBuilder();
            foreach (var todm in orders)
            {
                var tmpss = BuildOrderBySql(todm.Expression, todm.OrderType);
                if (!string.IsNullOrEmpty(tmpss))
                {
                    tmporder.Append(tmpss);
                }
            }
            _sqlBuilder.Comparable.SqlPage(page, size, tmporder.ToString());
            return _sqlBuilder.Do<DoQuerySql<A>>().ToList();
        }
        public List<A> ToPage(int page, int size, ref int total, params OrderByModel<A, B, C, D, E>[] orders)
        {
            StringBuilder tmporder = new StringBuilder();
            foreach (var todm in orders)
            {
                var tmpss = BuildOrderBySql(todm.Expression, todm.OrderType);
                if (!string.IsNullOrEmpty(tmpss))
                {
                    tmporder.Append(tmpss);
                }
            }
            _sqlBuilder.Comparable.SqlPageTotal(page, size, tmporder.ToString());
            var pagert = _sqlBuilder.Do<DoQueryTwo<DoQuerySql<A>, DoQuerySql<int>>>();
            total = pagert.Second.ToFirst();
            return pagert.First.ToList();
        }
        public async Task<List<A>> ToPageAsync(int page, int size, params OrderByModel<A, B, C, D, E>[] orders)
        {
            StringBuilder tmporder = new StringBuilder();
            foreach (var todm in orders)
            {
                var tmpss = BuildOrderBySql(todm.Expression, todm.OrderType);
                if (!string.IsNullOrEmpty(tmpss))
                {
                    tmporder.Append(tmpss);
                }
            }
            _sqlBuilder.Comparable.SqlPage(page, size, tmporder.ToString());
            return (await _sqlBuilder.DoAsync<DoQuerySql<A>>()).ToList();
        }
        public async Task<List<A>> ToPageAsync(int page, int size, RefAsync<int> total, params OrderByModel<A, B, C, D, E>[] orders)
        {
            StringBuilder tmporder = new StringBuilder();
            foreach (var todm in orders)
            {
                var tmpss = BuildOrderBySql(todm.Expression, todm.OrderType);
                if (!string.IsNullOrEmpty(tmpss))
                {
                    tmporder.Append(tmpss);
                }
            }
            _sqlBuilder.Comparable.SqlPageTotal(page, size, tmporder.ToString());
            var pagert = (await _sqlBuilder.DoAsync<DoQueryTwo<DoQuerySql<A>, DoQuerySql<int>>>());
            total.Value = pagert.Second.ToFirst();
            return pagert.First.ToList();
        }
    }
}
