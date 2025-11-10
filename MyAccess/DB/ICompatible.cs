using MyAccess.DB.Builder;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 兼容处理
    /// </summary>
    public interface ICompatible
    {
        string FullSearch(string field, IEnumerable<string> words);
        void SqlPageTotal(int page, int size, string orderby);
        void SqlPage(int page, int size, string orderby);
        /// <summary>
        /// 执行插入并返回Id
        /// </summary>
        /// <param name="p"></param>
        /// <param name="idname"></param>
        /// <returns></returns>
        long DoExecReturnIdentity(ExcuteParam p, string idname);

        /// <summary>
        /// 异步执行插入并返回Id
        /// </summary>
        /// <param name="p"></param>
        /// <param name="idname"></param>
        /// <returns></returns>
        Task<long> DoExecReturnIdentityAsync(ExcuteParam p, string idname);
        /// <summary>
        /// sql取前几条
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="num"></param>
        void Take(int num);

        /// <summary>
        /// 添加分隔
        /// </summary>
        /// <param name="sql"></param>
        void AppendDivide();
        /// <summary>
        /// 多条语句用
        /// </summary>
        /// <param name="sql"></param>
        string MutiWrapSql(string sql);


        /// <summary>
        /// 添加或更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void CreateOrUpdate<T>(T entity);
    }
}
