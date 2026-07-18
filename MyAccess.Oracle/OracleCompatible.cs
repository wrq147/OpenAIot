using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Attr;
using MyAccess.DB.Builder;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
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
            Type entityType = typeof(T);
            // 1. 获取表名
            TableNameAttribute tableAttr = entityType.GetCustomAttribute<TableNameAttribute>();
            string tableName = tableAttr == null ? entityType.Name : tableAttr.Name;
            string quote = GetFieldSign();

            PropertyInfo[] props = entityType.GetProperties();
            // 存储主键列名、插入字段、更新字段
            List<string> pkCols = new List<string>();
            List<(PropertyInfo Prop, string Col)> insertCols = new List<(PropertyInfo, string)>();
            List<(PropertyInfo Prop, string Col)> updateCols = new List<(PropertyInfo, string)>();

            foreach (var pi in props)
            {
                // 跳过忽略字段、不支持数据库映射的类型
                if (pi.IsDefined(typeof(DataIgnoreAttribute)) || !DBMapping.IsMapping(pi.PropertyType))
                    continue;

                // 获取列映射名
                ColumnByAttribute colAttr = pi.GetCustomAttribute<ColumnByAttribute>();
                string colName = colAttr == null ? pi.Name : colAttr.ColumnName;
                IDAttribute idAttr = pi.GetCustomAttribute<IDAttribute>();

                // 判断是否为主键
                if (idAttr != null)
                {
                    pkCols.Add(colName);
                    // 自增主键不需要传入值，跳过插入；序列/手动主键需要插入
                    bool canInsert = string.IsNullOrEmpty(idAttr.SeqName) ? !idAttr.IsAuto : true;
                    if (canInsert)
                        insertCols.Add((pi, colName));
                }
                else
                {
                    // 普通字段：插入+更新都参与
                    insertCols.Add((pi, colName));
                    updateCols.Add((pi, colName));
                }
            }

            // 校验必须存在主键
            if (pkCols.Count == 0)
                throw new InvalidOperationException($"实体 {entityType.Name} 未标记 [ID] 主键，无法执行CreateOrUpdate");

            StringBuilder mergeSb = new StringBuilder();

            // MERGE INTO 目标表
            mergeSb.AppendLine($"MERGE INTO {quote}{tableName}{quote} t");
            // USING 数据源（DUAL 单行虚拟表）
            mergeSb.AppendLine("USING (");
            List<string> selectItems = new List<string>();
            foreach (var (prop, col) in insertCols)
            {
                object val = prop.GetValue(entity);
                string param = _db.AddParam(val);
                selectItems.Add($"{param} AS {quote}{col}{quote}");
            }
            mergeSb.Append("    SELECT " + string.Join(",", selectItems));
            mergeSb.AppendLine(" FROM DUAL");
            mergeSb.AppendLine(") s");

            // ON 主键匹配条件（多主键联合匹配）
            List<string> onConditions = new List<string>();
            foreach (var pk in pkCols)
            {
                onConditions.Add($"t.{quote}{pk}{quote} = s.{quote}{pk}{quote}");
            }
            mergeSb.AppendLine($"ON ({string.Join(" AND ", onConditions)})");

            // 匹配成功：UPDATE SET
            if (updateCols.Count > 0)
            {
                mergeSb.AppendLine("WHEN MATCHED THEN");
                List<string> setItems = new List<string>();
                foreach (var (_, col) in updateCols)
                {
                    setItems.Add($"t.{quote}{col}{quote} = s.{quote}{col}{quote}");
                }
                mergeSb.AppendLine("UPDATE SET " + string.Join(",", setItems));
            }

            // 未匹配：INSERT
            mergeSb.AppendLine("WHEN NOT MATCHED THEN");
            List<string> insertFieldNames = insertCols.Select(x => $"{quote}{x.Col}{quote}").ToList();
            List<string> insertSourceNames = insertCols.Select(x => $"s.{quote}{x.Col}{quote}").ToList();
            mergeSb.AppendLine($"INSERT ({string.Join(",", insertFieldNames)})");
            mergeSb.AppendLine($"VALUES ({string.Join(",", insertSourceNames)})");

            // 写入SqlBuilder
            _sql.Append(mergeSb.ToString());
        }
    }
}
