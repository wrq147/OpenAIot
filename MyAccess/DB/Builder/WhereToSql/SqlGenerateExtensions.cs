using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder.WhereToSql
{
    public static class SqlGenerateExtensions
    {
        /// <summary>
        /// 生成SQL-Where语句(一个输入参数)
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        public static string GetWhereByLambda<A>(this SqlBuilder sqlbuilder, Expression<Func<A, bool>> expression)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.builder = sqlbuilder;
            conditionBuilder.Build(expression);
            return conditionBuilder.Result;
        }
        /// <summary>
        /// 生成SQL-Where语句(两个输入参数)
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="sqlbuilder"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetWhereByLambda<A, B>(this SqlBuilder sqlbuilder, Expression<Func<A, B, bool>> expression)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.builder = sqlbuilder;
            conditionBuilder.Build(expression);
            return conditionBuilder.Result;
        }
        /// <summary>
        /// 生成SQL-Where语句(三个输入参数)
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="sqlbuilder"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetWhereByLambda<A, B, C>(this SqlBuilder sqlbuilder, Expression<Func<A, B, C, bool>> expression)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.builder = sqlbuilder;
            conditionBuilder.Build(expression);
            return conditionBuilder.Result;
        }
        /// <summary>
        /// 生成SQL-Where语句(四个输入参数)
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <param name="sqlbuilder"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetWhereByLambda<A, B, C, D>(this SqlBuilder sqlbuilder, Expression<Func<A, B, C, D, bool>> expression)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.builder = sqlbuilder;
            conditionBuilder.Build(expression);
            return conditionBuilder.Result;
        }
        /// <summary>
        /// 生成SQL-Where语句(五个输入参数)
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <typeparam name="D"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="sqlbuilder"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetWhereByLambda<A, B, C, D, E>(this SqlBuilder sqlbuilder, Expression<Func<A, B, C, D, E, bool>> expression)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.builder = sqlbuilder;
            conditionBuilder.Build(expression);
            return conditionBuilder.Result;
        }
        /// <summary>
        /// 生成SQL的On条件
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <param name="sqlbuilder"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetOnByLambda<A, B>(this SqlBuilder sqlbuilder, Expression<Func<A, B, bool>> expression)
        {
            ConditionBuilder conditionBuilder = new ConditionBuilder();
            conditionBuilder.builder = sqlbuilder;
            conditionBuilder.Build(expression);
            return conditionBuilder.Result;
        }
    }
}
