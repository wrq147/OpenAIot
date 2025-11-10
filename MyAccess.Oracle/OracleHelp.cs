using System;
using System.Data.Common;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using MyAccess.DB;

namespace MyAccess.Oracle
{
    public class OracleHelp : DbHelp
    {
        public override ICompatible CreateCompatible(SqlBuilder sql)
        {
            return new OracleCompatible(this, sql);
        }

        public OracleHelp(string connstr) : base(connstr)
        {
        }
        public OracleHelp(string host, int port, string userName, string password, string dbName) :
            base(string.Format(@"Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))
	   (CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = {4}) ) );User ID={2};PassWord={3};", host, port, userName, password, dbName))
        {
        }
        protected override DbCommand CreateCommand()
        {
            return new OracleCommand();
        }

        protected override DbConnection CreateConnection()
        {
            return new OracleConnection();
        }

        protected override string AutoDbParam(string name, object val, ParameterDirection direct)
        {
            OracleParameter dbParameter = new OracleParameter(name, val);
            dbParameter.Direction = direct;
            AddDbParameter(dbParameter);
            return ":" + name;
        }


        public string AddOutParam(string parameterName, OracleDbType dbType)
        {
            OracleParameter dbParameter = new OracleParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.Output;
            AddDbParameter(dbParameter);
            return ":" + parameterName;
        }
        public string AddInParam(string parameterName, OracleDbType dbType, object value)
        {
            OracleParameter dbParameter = new OracleParameter(parameterName, dbType);
            dbParameter.Value = value;
            dbParameter.Direction = ParameterDirection.Input;
            AddDbParameter(dbParameter);
            return ":" + parameterName;
        }
        public string AddReturnParam(string parameterName, OracleDbType dbType)
        {
            OracleParameter dbParameter = new OracleParameter(parameterName, dbType);
            dbParameter.Direction = ParameterDirection.ReturnValue;
            AddDbParameter(dbParameter);
            return ":" + parameterName;
        }
        private int _addIdx = 0;
        public string GenerateReturnParam()
        {
            string parameterName = "RETURN_PARAM_AUTO_" + _addIdx;
            _addIdx++;
            return parameterName;
        }
    }
}
