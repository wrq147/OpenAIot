using MyAccess.DB;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
namespace MyAccess.MSSql
{
    public class SqlDbHelp : DbHelp
    {
        public override ICompatible CreateCompatible(SqlBuilder sql)
        {
            return new MSSqlCompatible(this, sql);
        }

        public SqlDbHelp(string connstr) : base(connstr)
        {
        }
        public SqlDbHelp(string host, string userName, string password, string dbName) :
            base(string.Format("Max Pool Size = 2048;Server={0};UID={1};PWD={2};DataBase={3};Pooling=true;", host, userName, password, dbName))
        {
        }

        protected override DbConnection CreateConnection()
        {
            return new SqlConnection();
        }
        protected override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }
        protected override string AutoDbParam(string name, object val, ParameterDirection direct)
        {
            if (!name.StartsWith("@"))
            {
                name = "@" + name;
            }
            SqlParameter dbParameter = new SqlParameter(name, val);
            dbParameter.Direction = direct;
            AddDbParameter(dbParameter);
            return name;
        }


        public string AddOutParam(string parameterName, SqlDbType dbType)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            SqlParameter dbParameter = new SqlParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            AddDbParameter(dbParameter);
            return parameterName;
        }
        public string AddInParam(string parameterName, SqlDbType dbType, object value)
        {
            if (!parameterName.StartsWith("@"))
            {
                parameterName = "@" + parameterName;
            }
            SqlParameter dbParameter = new SqlParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            AddDbParameter(dbParameter);
            return parameterName;
        }

    }
}