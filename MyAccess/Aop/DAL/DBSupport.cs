using MyAccess.DB;
using System;
using System.Threading;

namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// DAL层
    /// </summary>
    public abstract class DBSupport
    {
        private static AsyncLocal<DbHelp> _dbHelp = new AsyncLocal<DbHelp>();
        public DbHelp help { get { return _dbHelp.Value; } }

        /// <summary>
        /// aop初始化当前DBHelp
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        internal DbHelp InitDB(DbHelp db)
        {
            if (db == null)
            {
                _dbHelp.Value = CreateDB();
            }
            else
            {
                _dbHelp.Value = db;
            }
            return _dbHelp.Value;
        }
        /// <summary>
        /// 创建数据库DBHelp
        /// </summary>
        /// <returns></returns>
        protected abstract DbHelp CreateDB();

    }
}
