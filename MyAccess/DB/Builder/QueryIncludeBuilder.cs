using MyAccess.DB.Attr;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class QueryIncludeBuilder<A> : QueryOneBuilder<A>
    {
        public QueryIncludeBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        /// <summary>
        /// 添加子对象映射
        /// </summary>
        /// <typeparam name="TResult1"></typeparam>
        /// <typeparam name="TResult2"></typeparam>
        /// <param name="obj">子对象，例：x=>x.Obj</param>
        /// <param name="id">对象里的映射Id,x=>x.Id</param>
        /// <returns></returns>
        public QueryIncludeBuilder<A> Include<TResult1, TResult2>(Expression<Func<A, TResult1>> obj, Expression<Func<A, TResult2>> id)
        {
            string name = ExpressionTool.GetMemberName(obj);
            string idname = ExpressionTool.GetMemberName(id);
            _sqlBuilder.AddJoin(name, null, idname);
            return this;
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public int Count(Expression<Func<A, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda(expression));
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0);
        }
        /// <summary>
        /// 查询数量（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<int> CountAsync(Expression<Func<A, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select count(1) from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda(expression));
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0);
        }
        /// <summary>
        /// 判断是否存在指定记录
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Some(Expression<Func<A, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda(expression)).Take(1);
            return _sqlBuilder.Do<DoQueryScalar>().GetValueInt(0) > 0;
        }
        /// <summary>
        /// 判断是否存在指定记录（异步）
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public async Task<bool> SomeAsync(Expression<Func<A, bool>> expression)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            this.Append(string.Format("select 1 from {0} where ", table)).Append(this._sqlBuilder.GetWhereByLambda(expression)).Take(1);
            return (await _sqlBuilder.DoAsync<DoQueryScalar>()).GetValueInt(0) > 0;
        }

        /// <summary>
        /// 生成当前实体的条件查询语句
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public QueryOneBuilder<A> Where(Expression<Func<A, bool>> expression, string fields = null)
        {
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);
            if (string.IsNullOrEmpty(fields))
            {
                fields = GenerateFields(EntityType);
            }
            this.Append(string.Format("select {0} from {1} where ", fields, table)).Append(this._sqlBuilder.GetWhereByLambda(expression));
            return This();
        }
        /// <summary>
        /// 根据主键获取对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public A ToEntity(object key)
        {
            string idName = string.Empty;
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            PropertyInfo[] myProInfos = EntityType.GetProperties();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                if (pi.IsDefined(typeof(IDAttribute)))
                {
                    idName = pi.Name;
                    break;
                }
            }
            this.Append(string.Format("select * from {0} where {1}=", table, idName)).AppendParam(key);
            return _sqlBuilder.Do<DoQuerySql<A>>().ToFirst();
        }

        /// <summary>
        /// 根据主键获取对象（异步）
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<A> ToEntityAsync(object key)
        {
            string idName = string.Empty;
            Type EntityType = typeof(A);
            string table = GenerateTable(EntityType);

            PropertyInfo[] myProInfos = EntityType.GetProperties();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                if (pi.IsDefined(typeof(IDAttribute)))
                {
                    idName = pi.Name;
                    break;
                }
            }
            this.Append(string.Format("select * from {0} where {1}=", table, idName)).AppendParam(key);
            return (await _sqlBuilder.DoAsync<DoQuerySql<A>>()).ToFirst();
        }

        protected override QueryOneBuilder<A> This()
        {
            return this;
        }
    }
}
