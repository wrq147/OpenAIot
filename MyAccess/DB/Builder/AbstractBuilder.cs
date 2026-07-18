using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public abstract class AbstractBuilder<X> : ISqlBuilder<X> where X : ISqlBuilder<X>
    {
        protected SqlBuilder _sqlBuilder;
        public SqlBuilder Sql { get { return _sqlBuilder; } }
        public AbstractBuilder(SqlBuilder sqlBuilder)
        {
            _sqlBuilder = sqlBuilder;
        }
        protected abstract X This();

        public X FullSearch(string field, IEnumerable<string> words)
        {
            _sqlBuilder.FullSearch(field, words);
            return This();
        }

        public X Append(string value)
        {
            _sqlBuilder.Append(value);
            return This();
        }

        public X AppendLeft(string value)
        {
            _sqlBuilder.AppendLeft(value);
            return This();
        }

        public X AppendParam<T>(List<T> inlist)
        {
            _sqlBuilder.AppendParam(inlist);
            return This();
        }

        public X AppendParam<T>(T[] inarr)
        {
            _sqlBuilder.AppendParam(inarr);
            return This();
        }

        public X AppendParam(object value)
        {
            _sqlBuilder.AppendParam(value);
            return This();
        }


        public X Then(bool condition, Action<X> config)
        {
            X _this = This();
            if (condition)
            {
                config.Invoke(_this);
            }
            return _this;
        }

        public InsertBuilder<T> Insert<T>(T inserted)
        {
            return _sqlBuilder.Insert(inserted);
        }

        public InsertBuilder<T> Insert<T>(List<T> inserted)
        {
            return _sqlBuilder.Insert(inserted);
        }

        public InsertBuilder<T> Insert<T>(T[] inserted)
        {
            return _sqlBuilder.Insert(inserted);
        }

        public UpdateBuilder<T> Update<T>(T updated, string where = "")
        {
            return _sqlBuilder.Update(updated, where);
        }
        public UpdateBuilder<T> Update<T>(T updated, Expression<Func<T, bool>> expression)
        {
            return _sqlBuilder.Update(updated, expression);
        }
        public DeleteBuilderWithWhere<T> Delete<T>(string where = "")
        {
            return _sqlBuilder.Delete<T>(where);
        }
        public DeleteBuilderWithWhere<T> Delete<T>(Expression<Func<T, bool>> expression)
        {
            return _sqlBuilder.Delete<T>(expression);
        }
        public DeleteBuilder<T> Delete<T>()
        {
            return _sqlBuilder.Delete<T>();
        }
        public T Do<T>() where T : IDoCommand, new()
        {
            return _sqlBuilder.Do<T>();
        }

        public async Task<T> DoAsync<T>() where T : IDoCommand, new()
        {
            return await _sqlBuilder.DoAsync<T>();
        }
        /// <summary>
        /// 转换成sql字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _sqlBuilder.ToString();
        }
        /// <summary>
        /// 隐式转换成字符串
        /// </summary>
        /// <param name="sqlBuilder"></param>
        public static implicit operator string(AbstractBuilder<X> sqlBuilder)
        {
            return sqlBuilder.ToString();
        }
    }
}
