using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyAccess.DB.Builder.WhereToSql
{
    public static class SonSqlFun
    {
        /// <summary>
        /// 字符串的sql not like用
        /// </summary>
        /// <param name="ipt"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool NotContains(this string ipt, string target)
        {
            return true;
        }
        public static bool NotStartsWith(this string ipt, string target)
        {
            return true;
        }
        public static bool NotEndsWith(this string ipt, string target)
        {
            return true;
        }
        /// <summary>
        /// 数组或列表的sql not in用
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="ipt"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool NotContains<TSource>(this IEnumerable<TSource> ipt, TSource target)
        {
            return true;
        }
        /// <summary>
        /// 追加Sql条件
        /// </summary>
        /// <returns></returns>
        public static bool SqlCondition(string sql)
        {
            return true;
        }
        /// <summary>
        /// 追加SqlBuilder条件
        /// </summary>
        /// <param name="ac"></param>
        /// <returns></returns>
        public static bool SqlBuilderCondition(Action<SqlBuilder> ac)
        {
            return true;
        }
        /// <summary>
        /// 追加全文搜索条件
        /// </summary>
        /// <param name="field"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        public static bool FullSearch(string field, IEnumerable<string> words)
        {
            return true;
        }
    }

}
