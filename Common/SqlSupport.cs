using MyAccess.Aop.DAL;
using MyAccess.MySql;
using MyAccess.Sqlite;
using System;
using System.Threading.Tasks;
using MyAccess.DB;

namespace Common
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class SqlSupport : DBSupport
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        public enum SqlType
        {
            MySql,
            Sqlite
        }
        private SqlType _t;

        public void SetSqlType(SqlType t)
        {
            _t = t;
        }
        public SqlType GetSqlType()
        {
            return _t;
        }
        private string _connectionStr;

        public SqlSupport(string connectionStr, SqlType sqlType = SqlType.MySql)
        {
            _t = sqlType;
            _connectionStr = connectionStr;
        }

        protected override DbHelp CreateDB()
        {
            switch (_t)
            {
                case SqlType.MySql:
                    return new MySqlHelp(_connectionStr);
                case SqlType.Sqlite:
                    return new SqliteHelp(_connectionStr);
                default:
                    return new MySqlHelp(_connectionStr);
            }
        }
    }
}
