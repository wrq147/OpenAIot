using Common.Share;
using Microsoft.Extensions.Options;
using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Attr;
using MyAccess.DB.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace Common
{
    /// <summary>
    /// 数据库仓储
    /// </summary>
    public class BaseRepository<T> : SqlSupport, IRepository
    {
        public ITAServiceProvider Provider { get; set; }

        public BaseRepository() : base(Constants.General.connstr)
        {
            if (!string.IsNullOrEmpty(Constants.General.sqltype))
            {
                this.SetSqlType(Enum.Parse<SqlType>(Constants.General.sqltype));
            }
        }
        public DbHelp GetUsingDbHelp()
        {
            if (this.help == null)
            {
                return CreateDB();
            }
            else
            {
                return this.help;
            }
        }
        public virtual async Task<PageObject<T>> SelectPage(Expression<Func<T, bool>> expression, BaseQueryParam query, string orderby)
        {
            string tmporder = query.GetOrderBy(orderby);
            using (DbHelp db = GetUsingDbHelp())
            {
                if (query.pageSize > 0)
                {
                    if (query.showAll)
                    {
                        var tlist = await new SqlBuilder(db).Query<T>().Where(expression).ToPageAsync(query.pageNum, query.pageSize, tmporder);
                        return new PageObject<T>()
                        {
                            List = tlist,
                            Total = tlist.Count
                        };
                    }
                    else
                    {
                        RefAsync<int> total = 0;
                        var tlist = await new SqlBuilder(db).Query<T>().Where(expression).ToPageAsync(query.pageNum, query.pageSize, total, tmporder);
                        return new PageObject<T>()
                        {
                            List = tlist,
                            Total = total
                        };
                    }
                }
                else
                {
                    var tlist = await SelectList(expression, tmporder);
                    return new PageObject<T>()
                    {
                        List = tlist,
                        Total = tlist.Count
                    };
                }

            }
        }
        /// <summary>
        /// 分页（返回全部数量）
        /// </summary>
        /// <param name="expression">条件过滤</param>
        /// <param name="page">当前查询的页</param>
        /// <param name="size">页大小</param>
        /// <param name="total">返回的总数量</param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> SelectPage(Expression<Func<T, bool>> expression, int page, int size, RefAsync<int> total, string orderby)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Query<T>().Where(expression).ToPageAsync(page, size, total, orderby);
            }
        }
        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="orderby"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> SelectPage(Expression<Func<T, bool>> expression, int page, int size, string orderby)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Query<T>().Where(expression).ToPageAsync(page, size, orderby);
            }
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="orderby"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public virtual async Task<List<T>> SelectList(Expression<Func<T, bool>> expression, string orderby = "", string fields = "")
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Query<T>().Where(expression, fields).Then(!string.IsNullOrEmpty(orderby), x => x.Append(" order by " + orderby)).ToListAsync();
            }
        }
        /// <summary>
        /// 通用Id查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> Select(object id)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Query<T>().ToEntityAsync(id);
            }
        }
        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<bool> Some(Expression<Func<T, bool>> expression)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Query<T>().SomeAsync(expression);
            }
        }
        /// <summary>
        /// 获取数量
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> Count(Expression<Func<T, bool>> expression)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Query<T>().CountAsync(expression);
            }
        }
        /// <summary>
        /// 通用添加
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> Insert(T entity)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Insert(entity).DoAsync();
            }
        }
        /// <summary>
        /// 通用批量添加
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task<int> Insert(List<T> list)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Insert(list).DoAsync();
            }
        }
        /// <summary>
        /// 通用更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> Update(T entity)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Update(entity).DoAsync();
            }
        }
        /// <summary>
        /// 条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> Update(T entity, Expression<Func<T, bool>> expression)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Update(entity, expression).DoAsync();
            }
        }
        /// <summary>
        /// 通用多个删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteIn<X>(List<X> ids)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Delete<T>().DoAsync(ids);
            }
        }
        /// <summary>
        /// 通用删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<int> Delete<X>(X id)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Delete<T>().DoAsync(id);
            }
        }
        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual async Task<int> Delete(Expression<Func<T, bool>> expression)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).Delete(expression).DoAsync();
            }
        }

        /// <summary>
        /// 存在则更新，不存在则新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual async Task<int> CreateOrUpdate(T entity)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                return await new SqlBuilder(db).CreateOrUpdate(entity).DoAsync();
            }
        }

        /// <summary>
        /// 生成导航字典
        /// </summary>
        /// <typeparam name="TSource">源表</typeparam>
        /// <typeparam name="TResult">Id的类型</typeparam>
        /// <param name="sourcelist">源列表</param>
        /// <param name="predicate">源表过滤条件</param>
        /// <param name="idname">源表中的关联字段</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<TResult, T>> NavigateDict<TSource, TResult>(List<TSource> sourcelist, Func<TSource, bool> predicate, Expression<Func<TSource, TResult>> idname)
        {
            using (DbHelp db = GetUsingDbHelp())
            {
                var sqlBuilder = new SqlBuilder(db);
                Type EntityType = typeof(T);
                TableNameAttribute tn = EntityType.GetCustomAttribute<TableNameAttribute>();
                string tablename = tn == null ? EntityType.Name : tn.Name;
                sqlBuilder.Append("select * from " + tablename + " where ");

                var tlist = sourcelist.Where(predicate);
                string propname = ExpressionTool.GetMemberName(idname);
                PropertyInfo sourceProp = typeof(TSource).GetProperty(propname);
                List<TResult> objlist = new List<TResult>();
                foreach (var source in tlist)
                {
                    objlist.Add((TResult)sourceProp.GetValue(source));
                }

                if (objlist.Count > 0)
                {
                    PropertyInfo idProp = null;
                    PropertyInfo[] properties = EntityType.GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        IDAttribute attribute = property.GetCustomAttribute<IDAttribute>();
                        if (attribute != null && property.IsDefined(typeof(IDAttribute)))
                        {
                            idProp = property;
                            sqlBuilder.Append(property.Name + " in (").AppendParam(objlist).Append(")");
                            break;
                        }
                    }

                    var tmplist = (await sqlBuilder.DoAsync<DoQuerySql<T>>()).ToList();

                    var dict = new Dictionary<TResult, T>();
                    foreach (var item in tmplist)
                    {
                        TResult tmpval = (TResult)idProp.GetValue(item);
                        if (!dict.ContainsKey(tmpval))
                        {
                            dict.Add(tmpval, item);
                        }
                    }
                    return dict;
                }


            }

            return new Dictionary<TResult, T>();
        }
    }
}
