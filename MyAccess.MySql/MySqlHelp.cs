using System;
using System.Data.Common;
using System.Data;
using MySql.Data.MySqlClient;
using MyAccess.DB;

namespace MyAccess.MySql
{
    /// <summary>
    /// mysql数据库访问类
    /// </summary>
    public class MySqlHelp : DbHelp
    {
        public override ICompatible CreateCompatible(SqlBuilder sql)
        {
            return new MySqlCompatible(this, sql);
        }
        public MySqlHelp(string connstr) : base(connstr)
        {
        }
        public MySqlHelp(string host, string userName, string password, string dbName) :
            base(string.Format("server={0};user id={1};password={2};database={3};", host, userName, password, dbName))
        {
        }
        protected override DbCommand CreateCommand()
        {
            return new MySqlCommand();
        }

        protected override DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }


        protected override string AutoDbParam(string name, object val, ParameterDirection direct)
        {
            if (!name.StartsWith("@"))
            {
                name = "@" + name;
            }
            MySqlParameter dbParameter = new MySqlParameter(name, val);
            dbParameter.Direction = direct;
            AddDbParameter(dbParameter);
            return name;
        }


        public string AddOutParam(string parameterName, MySqlDbType dbType)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            MySqlParameter dbParameter = new MySqlParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            AddDbParameter(dbParameter);
            return parameterName;
        }
        public string AddInParam(string parameterName, MySqlDbType dbType, object value)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            MySqlParameter dbParameter = new MySqlParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            AddDbParameter(dbParameter);
            return parameterName;
        }
    }
}
