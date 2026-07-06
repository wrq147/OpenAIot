using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyAccess.DB.Builder.WhereToSql
{
    /// <summary>
    /// 扩展条件表达式
    /// </summary>
    public static class ExpressionBuilderExtends
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)  
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first  
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression   
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<A, bool>> And<A>(this Expression<Func<A, bool>> first, Expression<Func<A, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<A, bool>> Or<A>(this Expression<Func<A, bool>> first, Expression<Func<A, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
        public static Expression<Func<A, B, bool>> And<A, B>(this Expression<Func<A, B, bool>> first, Expression<Func<A, B, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<A, B, bool>> Or<A, B>(this Expression<Func<A, B, bool>> first, Expression<Func<A, B, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
        public static Expression<Func<A, B, C, bool>> And<A, B, C>(this Expression<Func<A, B, C, bool>> first, Expression<Func<A, B, C, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<A, B, C, bool>> Or<A, B, C>(this Expression<Func<A, B, C, bool>> first, Expression<Func<A, B, C, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
        public static Expression<Func<A, B, C, D, bool>> And<A, B, C, D>(this Expression<Func<A, B, C, D, bool>> first, Expression<Func<A, B, C, D, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<A, B, C, D, bool>> Or<A, B, C, D>(this Expression<Func<A, B, C, D, bool>> first, Expression<Func<A, B, C, D, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
        public static Expression<Func<A, B, C, D, E, bool>> And<A, B, C, D, E>(this Expression<Func<A, B, C, D, E, bool>> first, Expression<Func<A, B, C, D, E, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<A, B, C, D, E, bool>> Or<A, B, C, D, E>(this Expression<Func<A, B, C, D, E, bool>> first, Expression<Func<A, B, C, D, E, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }
}
