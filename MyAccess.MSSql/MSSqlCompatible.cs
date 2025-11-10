using MyAccess.Core;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MyAccess.MSSql
{
    public class MSSqlCompatible : ICompatible
    {
        private SqlDbHelp _db;
        private SqlBuilder _sql;
        public MSSqlCompatible(SqlDbHelp db, SqlBuilder sql)
        {
            _db = db;
            _sql = sql;
        }
        public string FullSearch(string field, IEnumerable<string> words)
        {
            return "CONTAINS(" + field + ", '" + StringTool.SqlLikeFilter(string.Join(" OR ", words)) + "')";
        }
        public void SqlPageTotal(int page, int size, string orderby)
        {
            string tmpsql = _sql;
            int startIdx = tmpsql.IndexOf("from", StringComparison.OrdinalIgnoreCase);
            SqlPage(page, size, orderby);
            _sql.Append(";select count(*) " + tmpsql.Substring(startIdx));
        }
        public void SqlPage(int page, int size, string orderby)
        {
            if (string.IsNullOrEmpty(orderby))
            {
                _sql.ReplaceLeftOne("select", "select * from (select ROW_NUMBER() OVER(ORDER BY GetDate())AS RowIndex,");
                _sql.Append(") TP where RowIndex BETWEEN " + ((page - 1) * size + 1) + " and " + (size * page));
            }
            else
            {
                _sql.ReplaceLeftOne("select", "select * from (select ROW_NUMBER() OVER(order by " + orderby + ")AS RowIndex,");
                _sql.Append(") TP where RowIndex BETWEEN " + ((page - 1) * size + 1) + " and " + (size * page));
            }
        }
        public void Take(int num)
        {
            _sql.ReplaceLeftOne("select", "select top " + num);
        }

        public void AppendDivide()
        {
            _sql.Append(";");
        }
        public string MutiWrapSql(string sql)
        {
            return sql;
        }
        public long DoExecReturnIdentity(ExcuteParam p, string idname)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql + ";SELECT SCOPE_IDENTITY()";
            object rt = p.Command.ExecuteScalar();
            return Convert.ToInt64(rt);
        }

        public async Task<long> DoExecReturnIdentityAsync(ExcuteParam p, string idname)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql + ";SELECT SCOPE_IDENTITY()";
            object rt = await p.Command.ExecuteScalarAsync();
            return Convert.ToInt64(rt);
        }

        public void CreateOrUpdate<T>(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
