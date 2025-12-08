using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Builder;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.Oracle
{
    public class OracleCompatible : ICompatible
    {
        private OracleHelp _db;
        private SqlBuilder _sql;
        public OracleCompatible(OracleHelp db, SqlBuilder sql)
        {
            _db = db;
            _sql = sql;
        }
        public string GetFieldSign()
        {
            return "\"";
        }
        public string FullSearch(string field, IEnumerable<string> words)
        {
            return "CONTAINS(" + field + ", '" + StringTool.SqlLikeFilter(string.Join(" OR ", words)) + "') > 0";
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
            if (!string.IsNullOrEmpty(orderby))
            {
                _sql.Append(" order by " + orderby);
            }
            _sql.ReplaceLeftOne("select", "select * from(select A.*, rownum RowIndex from(select");
            _sql.Append(") A) where RowIndex BETWEEN " + ((page - 1) * size + 1) + " and " + (size * page));
        }
        public void Take(int num)
        {
            if (!_sql.ReplaceRightOne("where", "rownum<=" + num))
            {
                _sql.Append(" where rownum<=" + num);
            }
        }
        public void AppendDivide()
        {
            _sql.Append(";");
        }
        public string MutiWrapSql(string sql)
        {
            return "begin " + sql + ";end;";
        }

        public long DoExecReturnIdentity(ExcuteParam p, string idname)
        {
            if (!string.IsNullOrEmpty(idname))
            {
                OracleHelp oracleHelp = ((OracleHelp)p.Db);
                string autoname = oracleHelp.GenerateReturnParam();
                string paramname = oracleHelp.AddReturnParam(autoname, OracleDbType.Long);
                string tmpsql = p.Sql;
                tmpsql += " returning " + idname + " into " + paramname;
                p.Command.CommandType = CommandType.Text;
                p.Command.CommandText = tmpsql;
                p.Command.ExecuteNonQuery();
                return Convert.ToInt64(p.Command.Parameters[autoname].Value);
            }
            return 0;
        }
        public async Task<long> DoExecReturnIdentityAsync(ExcuteParam p, string idname)
        {
            if (!string.IsNullOrEmpty(idname))
            {
                OracleHelp oracleHelp = ((OracleHelp)p.Db);
                string autoname = oracleHelp.GenerateReturnParam();
                string paramname = oracleHelp.AddReturnParam(autoname, OracleDbType.Long);
                string tmpsql = p.Sql;
                tmpsql += " returning " + idname + " into " + paramname;
                p.Command.CommandType = CommandType.Text;
                p.Command.CommandText = tmpsql;
                await p.Command.ExecuteNonQueryAsync();
                return Convert.ToInt64(p.Command.Parameters[autoname].Value);
            }
            return 0;
        }

        public void CreateOrUpdate<T>(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
