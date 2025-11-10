using System;
using System.Data.Common;

namespace MyAccess.DB
{
    /// <summary>
    /// 命令执行的参数
    /// </summary>
    public struct ExcuteParam
    {
        public DbHelp Db;
        public DbCommand Command;
        public SqlBuilder Sql;
    }
}
