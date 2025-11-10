using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Builder;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Common.Share
{
    public class BaseQueryParam
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? beginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 分页号
        /// </summary>
        public int pageNum { get; set; }
        /// <summary>
        /// 分页大小（当showAll为true,pageSize大于0则返回分页
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 是否不返回总记录数
        /// </summary>
        public bool showAll { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string orderByColumn { get; set; }
        /// <summary>
        /// asc表示升序，desc表示降序
        /// </summary>
        public string isAsc { get; set; }
        /// <summary>
        /// 获取排序
        /// </summary>
        /// <param name="defualtOrderBy"></param>
        /// <returns></returns>
        public string GetOrderBy(string defualtOrderBy = "")
        {
            string orderby = defualtOrderBy;
            if (!string.IsNullOrEmpty(this.orderByColumn))
            {
                if (!Regex.IsMatch(this.orderByColumn, "^[A-Za-z ]+$"))
                {
                    return string.Empty;
                }
                this.orderByColumn = StringHelper.SqlLikeFilter(this.orderByColumn);
                if (this.isAsc == null)
                {
                    return orderby;
                }
                orderby = this.orderByColumn + " " + (this.isAsc.ToLower().StartsWith("asc") ? string.Empty : "desc");
            }
            return orderby;
        }
    }
    public static class QuerySqlBuilderExtensions
    {
        public static async Task<PageObject<T>> GeneratePageObjectAsync<T>(this QueryOneBuilder<T> sql, BaseQueryParam query, string defualtOrderBy = "")
        {
            string orderby = defualtOrderBy;
            if (!string.IsNullOrEmpty(query.orderByColumn))
            {
                orderby = query.orderByColumn + " " + (query.isAsc.ToLower().StartsWith("asc") ? string.Empty : "desc");
            }
            if (query.pageSize > 0)
            {
                if (query.showAll)
                {
                    var tlist = await sql.ToPageAsync(query.pageNum, query.pageSize, orderby);
                    return new PageObject<T>()
                    {
                        List = tlist,
                        Total = tlist.Count
                    };
                }
                else
                {
                    RefAsync<int> total = 0;
                    var tlist = await sql.ToPageAsync(query.pageNum, query.pageSize, total, orderby);
                    return new PageObject<T>()
                    {
                        List = tlist,
                        Total = total.Value
                    };
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(orderby))
                {
                    sql.Append(" order by " + orderby);
                }
                var tlist = (await sql.DoAsync<DoQuerySql<T>>()).ToList();
                return new PageObject<T>()
                {
                    List = tlist,
                    Total = tlist.Count
                };
            }

        }
        public static PageObject<T> GeneratePageObject<T>(this QueryOneBuilder<T> sql, BaseQueryParam query, string defualtOrderBy = "")
        {
            string orderby = defualtOrderBy;
            if (!string.IsNullOrEmpty(query.orderByColumn))
            {
                orderby = query.orderByColumn + " " + (query.isAsc.ToLower().StartsWith("asc") ? string.Empty : "desc");
            }
            if (query.pageSize > 0)
            {
                if (query.showAll)
                {
                    var tlist = sql.ToPage(query.pageNum, query.pageSize, orderby);
                    return new PageObject<T>()
                    {
                        List = tlist,
                        Total = tlist.Count
                    };
                }
                else
                {
                    int total = 0;
                    var tlist = sql.ToPage(query.pageNum, query.pageSize, ref total, orderby);
                    return new PageObject<T>()
                    {
                        List = tlist,
                        Total = total
                    };
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(orderby))
                {
                    sql.Append(" order by " + orderby);
                }
                var tlist = sql.Do<DoQuerySql<T>>().ToList();
                return new PageObject<T>()
                {
                    List = tlist,
                    Total = tlist.Count
                };
            }

        }
    }
}
