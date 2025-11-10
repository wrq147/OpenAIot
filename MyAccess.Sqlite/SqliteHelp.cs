using System.Data.Common;
using System.Data;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;
using System;
using MyAccess.DB;
namespace MyAccess.Sqlite
{
    /// <summary>
    /// Sqlite数据库访问类
    /// </summary>
    public class SqliteHelp : DbHelp
    {
        public override ICompatible CreateCompatible(SqlBuilder sql)
        {
            return new SqliteCompatible(this, sql);
        }
        public SqliteHelp(string connstr) : base(connstr)
        {
        }
        public override void BeginTran() { }
        public override Task BeginTranAsync() { return Task.CompletedTask; }
        protected override DbCommand CreateCommand()
        {
            return new SqliteCommand();
        }

        protected override DbConnection CreateConnection()
        {
            return new SqliteConnection();
        }


        protected override string AutoDbParam(string name, object val, ParameterDirection direct)
        {
            if (!name.StartsWith("@"))
            {
                name = "@" + name;
            }
            if (val == null)
            {
                val = DBNull.Value;
            }
            SqliteParameter dbParameter = new SqliteParameter(name, val);
            dbParameter.Direction = direct;
            AddDbParameter(dbParameter);
            return name;
        }


        public string AddOutParam(string parameterName, SqliteType dbType)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            SqliteParameter dbParameter = new SqliteParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            AddDbParameter(dbParameter);
            return parameterName;
        }
        public string AddInParam(string parameterName, SqliteType dbType, object value)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            if (value == null)
            {
                value = DBNull.Value;
            }
            SqliteParameter dbParameter = new SqliteParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            AddDbParameter(dbParameter);
            return parameterName;
        }
    }
}
