using MyAccess.Aop;
using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Attr;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace MyAccess.Sqlite
{
    public class SqliteCompatible : ICompatible
    {
        private SqliteHelp _db;
        private SqlBuilder _sql;
        public SqliteCompatible(SqliteHelp db, SqlBuilder sql)
        {
            _db = db;
            _sql = sql;
        }
        public string FullSearch(string field, IEnumerable<string> words)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            int i = 0;
            foreach (string word in words)
            {
                if (i == 0)
                {
                    sb.Append(field + " like ('%" + StringTool.SqlLikeFilter(word) + "%')");
                }
                else
                {
                    sb.Append(" or " + field + " like ('%" + StringTool.SqlLikeFilter(word) + "%')");
                }
                ++i;
            }
            sb.Append(")");
            return sb.ToString();
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
            int start_row = (page - 1) * size;
            if (start_row < 0) start_row = 0;
            if (!string.IsNullOrEmpty(orderby))
            {
                _sql.Append(" order by " + orderby);
            }
            _sql.Append(string.Format(" limit {0} offset {1}", size, (page - 1) * size));
        }
        public void Take(int num)
        {
            _sql.Append(" limit " + num);
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
            p.Command.CommandText = p.Sql;
            var insertedId = p.Command.ExecuteScalar();
            return insertedId == null ? 0 : (long)insertedId;
        }
        public async Task<long> DoExecReturnIdentityAsync(ExcuteParam p, string idname)
        {
            p.Command.CommandType = CommandType.Text;
            p.Command.CommandText = p.Sql + ";SELECT last_insert_rowid()";
            var insertedId = await p.Command.ExecuteScalarAsync();
            return insertedId == null ? 0 : (long)insertedId;
        }

        public void CreateOrUpdate<T>(T entity)
        {
            Type EntityType = typeof(T);
            TableNameAttribute tn = EntityType.GetCustomAttribute<TableNameAttribute>();
            string tablename = tn == null ? EntityType.Name : tn.Name;


            PropertyInfo[] myProInfos = EntityType.GetProperties();
            T iitem = entity;
            _sql.Append(string.Format("INSERT INTO {0}", tablename));

            StringBuilder sbfields = new StringBuilder();
            StringBuilder sbvalues = new StringBuilder();

            List<string> ids = new List<string>();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                if (pi.IsDefined(typeof(DataIgnoreAttribute)) || !DBMapping.IsMapping(pi.PropertyType))
                {
                    continue;
                }
                IDAttribute idattr = (IDAttribute)pi.GetCustomAttribute(typeof(IDAttribute));
                bool caninserted = true;
                if (idattr != null)
                {
                    ids.Add(pi.Name);
                    if (string.IsNullOrEmpty(idattr.SeqName))
                    {
                        caninserted = !idattr.IsAuto;
                    }
                    else
                    {
                        caninserted = true;
                    }
                }
                if (caninserted)
                {
                    sbfields.Append(',');
                    sbfields.Append(pi.Name);
                    sbvalues.Append(',');
                    sbvalues.Append(_db.AddParam(pi.GetValue(iitem)));
                }
            }


            string rtfields = sbfields.ToString();
            if (rtfields.StartsWith(","))
            {
                rtfields = rtfields.Substring(1);
            }
            rtfields = "(" + rtfields + ")";
            string rtvalues = sbvalues.ToString();
            if (rtvalues.StartsWith(","))
            {
                rtvalues = rtvalues.Substring(1);
            }
            rtvalues = "(" + rtvalues + ")";
            _sql.Append(" " + rtfields + " values " + rtvalues);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                if (pi.IsDefined(typeof(DataIgnoreAttribute)) || !DBMapping.IsMapping(pi.PropertyType))
                {
                    continue;
                }
                if (!pi.IsDefined(typeof(IDAttribute)))
                {
                    object val = pi.GetValue(entity);
                    if (val == null)
                    {
                        continue;
                    }
                    sb.Append(',');
                    sb.Append(pi.Name);
                    sb.Append('=');
                    sb.Append(_db.AddParam(pi.GetValue(entity)));
                }
            }
            string updatestr = sb.ToString();
            if (updatestr.StartsWith(","))
            {
                updatestr = updatestr.Substring(1);
            }

            _sql.Append(" ON CONFLICT(" + string.Join(',', ids) + ") DO UPDATE set ");
            _sql.Append(updatestr);


        }
    }
}
