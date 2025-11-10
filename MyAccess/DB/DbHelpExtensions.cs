using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public static class DbHelpExtensions
    {
        /// <summary>
        /// 合并同步执行两个命令
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="help"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DoQueryTwo<T1, T2> DoQueryTwo<T1, T2>(this DbHelp help, SqlBuilder sql) where T1 : IDoCommand, new() where T2 : IDoCommand, new()
        {
            return help.DoCommand<DoQueryTwo<T1, T2>>(sql);
        }

        /// <summary>
        /// 合并异步执行两个命令
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="help"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static async Task<DoQueryTwo<T1, T2>> DoQueryTwoAsync<T1, T2>(this DbHelp help, SqlBuilder sql) where T1 : IDoCommand, new() where T2 : IDoCommand, new()
        {
            return await help.DoCommandAsync<DoQueryTwo<T1, T2>>(sql);
        }
        /// <summary>
        /// 执行有返回值的存储过程（一个返回值）
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="help"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DoQueryOneStored<T1> DoQueryOneStored<T1>(this DbHelp help, SqlBuilder sql) where T1 : IDoResult<DbDataReader>, new()
        {
            return help.DoCommand<DoQueryOneStored<T1>>(sql);
        }
        /// <summary>
        /// 执行有返回值的存储过程（一个返回值，异步）
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="help"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static async Task<DoQueryOneStored<T1>> DoQueryOneStoredAsync<T1>(this DbHelp help, SqlBuilder sql) where T1 : IDoResult<DbDataReader>, new()
        {
            return await help.DoCommandAsync<DoQueryOneStored<T1>>(sql);
        }
        /// <summary>
        /// 执行有返回值的存储过程（两个返回值）
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="help"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DoQueryTwoStored<T1, T2> DoQueryTwoStored<T1, T2>(this DbHelp help, SqlBuilder sql) where T1 : IDoResult<DbDataReader>, new() where T2 : IDoResult<DbDataReader>, new()
        {
            return help.DoCommand<DoQueryTwoStored<T1, T2>>(sql);
        }
        /// <summary>
        /// 执行有返回值的存储过程（两个返回值，异步用）
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="help"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static async Task<DoQueryTwoStored<T1, T2>> DoQueryTwoStoredAsync<T1, T2>(this DbHelp help, SqlBuilder sql) where T1 : IDoResult<DbDataReader>, new() where T2 : IDoResult<DbDataReader>, new()
        {
            return await help.DoCommandAsync<DoQueryTwoStored<T1, T2>>(sql);
        }

    }
}
