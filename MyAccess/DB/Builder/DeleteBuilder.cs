using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class DeleteBuilder<X> : DeleteBuilderWithWhere<X>
    {
        public DeleteBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        private void AppendIn<T>(T[] ids)
        {
            string idName = string.Empty;
            PropertyInfo[] myProInfos = typeof(X).GetProperties();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                if (pi.IsDefined(typeof(IDAttribute)))
                {
                    idName = pi.Name;
                    break;
                }
            }
            if (ids.Length > 1)
            {
                _sqlBuilder.Append(" where " + idName + " in (").AppendParam(ids).Append(")");
            }
            else if (ids.Length == 1)
            {
                _sqlBuilder.Append(" where " + idName + "=").AppendParam(ids[0]);
            }

        }
        public int Do<T>(T key)
        {
            T[] ids = { key };
            AppendIn(ids);
            return _sqlBuilder.Do<DoExecSql>().RowCount;
        }
        public async Task<int> DoAsync<T>(T key)
        {
            T[] ids = { key };
            AppendIn(ids);
            return (await _sqlBuilder.DoAsync<DoExecSql>()).RowCount;
        }
        public int Do<T>(List<T> ids)
        {
            AppendIn(ids.ToArray());
            return _sqlBuilder.Do<DoExecSql>().RowCount;
        }
        public async Task<int> DoAsync<T>(List<T> ids)
        {
            AppendIn(ids.ToArray());
            return (await _sqlBuilder.DoAsync<DoExecSql>()).RowCount;
        }
        public int Do<T>(T[] ids)
        {
            AppendIn(ids);
            return _sqlBuilder.Do<DoExecSql>().RowCount;
        }
        public async Task<int> DoAsync<T>(T[] ids)
        {
            AppendIn(ids);
            return (await _sqlBuilder.DoAsync<DoExecSql>()).RowCount;
        }

    }
}
