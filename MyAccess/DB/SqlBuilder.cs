using MyAccess.Aop;
using MyAccess.Core;
using MyAccess.DB.Attr;
using MyAccess.DB.Builder;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// sql语句构建器
    /// </summary>
    public class SqlBuilder
    {
        /// <summary>
        /// 子对象
        /// </summary>
        private List<string> _subMaps;
        public List<string> SubMaps { get { return _subMaps; } }
        private List<string> _joinWays;
        public List<string> JoinWays { get { return _joinWays; } }
        private List<string> _subIdMaps;
        public List<string> SubIdMaps { get { return _subIdMaps; } }
        private List<Type> _subTypeMaps;
        public List<Type> SubTypeMaps { get { return _subTypeMaps; } }
        public const string NullSub = "$";
        public void AddJoin(string name, Type t, string condition, string way = "left join")
        {
            if (_subMaps == null)
            {
                _subMaps = new List<string>(8);
                _subIdMaps = new List<string>(8);
                _subTypeMaps = new List<Type>(8);
                _joinWays = new List<string>(8);
            }
            _subMaps.Add(name);
            _joinWays.Add(way);
            _subIdMaps.Add(condition);
            _subTypeMaps.Add(t);
        }
        public string GetPrefixOfMap(string name)
        {
            int index = _subMaps.IndexOf(name);
            return index != -1 ? DBMapping.GetSubPrefix(index) : "a";
        }
        private StringBuilder _sb;
        private DbHelp _db;
        private ICompatible _comparable;
        public ICompatible Comparable { get { return _comparable; } }
        /// <summary>
        /// 是否为多条语句
        /// </summary>
        private bool _mulSeq = false;
        internal void AppendDiv()
        {
            if (this._sb.Length > 0)
            {
                _mulSeq = true;
                _comparable.AppendDivide();
            }
        }
        public DbHelp Db
        {
            get { return _db; }
        }
        public SqlBuilder(DbHelp db)
        {
            _db = db;
            _sb = new StringBuilder();
            _comparable = _db.CreateCompatible(this);
        }
        public bool ReplaceLeftOne(string oldValue, string newValue)
        {
            string str = _sb.ToString();
            int startIndx = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
            if (startIndx > -1)
            {
                _sb.Remove(startIndx, oldValue.Length);
                _sb.Insert(startIndx, newValue);
                return true;
            }
            return false;
        }
        public bool ReplaceRightOne(string oldValue, string newValue)
        {
            string str = _sb.ToString();
            int startIndx = str.LastIndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
            if (startIndx > -1)
            {
                _sb.Remove(startIndx, oldValue.Length);
                _sb.Insert(startIndx, newValue);
                return true;
            }
            return false;
        }

        public SqlBuilder Append(string value)
        {
            _sb.Append(value);
            return this;
        }
        /// <summary>
        /// 最前面添加字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlBuilder AppendLeft(string value)
        {
            _sb.Insert(0, value);
            return this;
        }

        /// <summary>
        /// sql语句追加默认的in参数
        /// </summary>
        /// <param name="builder">列表</param>
        /// <returns></returns>
        public SqlBuilder AppendParam<T>(List<T> inlist)
        {
            return this.AppendParam(inlist.ToArray());
        }
        /// <summary>
        /// sql语句追加默认的in参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inarr">数组</param>
        /// <returns></returns>
        public SqlBuilder AppendParam<T>(T[] inarr)
        {
            string inwhere;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inarr.Length; i++)
            {
                sb.Append(',');
                sb.Append(_db.AddParam(inarr[i]));
            }
            inwhere = sb.ToString();
            if (inwhere.StartsWith(","))
            {
                inwhere = inwhere.Substring(1);
            }
            return this.Append(inwhere);
        }
        /// <summary>
        /// sql追加参数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SqlBuilder AppendParam(object value)
        {
            return this.Append(_db.AddParam(value));
        }
        /// <summary>
        /// sql追加全文搜索
        /// </summary>
        /// <param name="field"></param>
        /// <param name="words"></param>
        /// <returns></returns>
        public SqlBuilder FullSearch(string field, IEnumerable<string> words)
        {
            string tmpstr = this._comparable.FullSearch(field, words);
            return this.Append(tmpstr);
        }
        /// <summary>
        /// 在条件为true时,才会执行后面的
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public SqlBuilder Then(bool condition, Action<SqlBuilder> config)
        {
            if (condition)
            {
                config.Invoke(this);
            }
            return this;
        }


        /// <summary>
        /// 追加查询语句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public QueryBuilder<T> Query<T>()
        {
            AppendDiv();
            return new QueryBuilder<T>(this);
        }

        /// <summary>
        /// sql追加单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public InsertBuilder<T> Insert<T>(T inserted)
        {
            T[] tmpdata = { inserted };
            return this.Insert(tmpdata);
        }
        /// <summary>
        /// sql追加列表数据插入语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public InsertBuilder<T> Insert<T>(List<T> inserted)
        {
            return this.Insert(inserted.ToArray());
        }
        /// <summary>
        /// sql追加数组数据插入语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public InsertBuilder<T> Insert<T>(T[] inserted)
        {
            T[] tmpdata = inserted;
            if (tmpdata == null || tmpdata.Length == 0) return new InsertBuilder<T>(this);

            Type EntityType = typeof(T);
            TableNameAttribute tn = EntityType.GetCustomAttribute<TableNameAttribute>();
            string tablename = tn == null ? EntityType.Name : tn.Name;


            StringBuilder sbfields = new StringBuilder();
            StringBuilder sbvalues = new StringBuilder();


            PropertyInfo[] myProInfos = EntityType.GetProperties();

            for (int idx = 0; idx < tmpdata.Length; idx++)
            {
                T iitem = tmpdata[idx];
                sbvalues.Append(",(");
                bool isfirst = true;
                for (int i = 0; i < myProInfos.Length; i++)
                {
                    PropertyInfo pi = myProInfos[i];

                    if (pi.IsDefined(typeof(DataIgnoreAttribute)) || !DBMapping.IsMapping(pi.PropertyType))
                    {
                        continue;
                    }
                    IDAttribute idattr = (IDAttribute)pi.GetCustomAttribute(typeof(IDAttribute));
                    bool caninserted = true;
                    string seqName = null;
                    if (idattr != null)
                    {
                        if (string.IsNullOrEmpty(idattr.SeqName))
                        {
                            caninserted = !idattr.IsAuto;
                        }
                        else
                        {
                            caninserted = true;
                            seqName = idattr.SeqName;
                        }
                    }
                    if (caninserted)
                    {
                        if (idx == 0)
                        {
                            sbfields.Append(',');
                            sbfields.Append(pi.Name);
                        }
                        if (isfirst)
                        {
                            isfirst = false;
                        }
                        else
                        {
                            sbvalues.Append(',');
                        }
                        if (seqName == null)
                        {
                            sbvalues.Append(_db.AddParam(pi.GetValue(iitem)));
                        }
                        else
                        {
                            sbvalues.Append(seqName);
                        }
                    }
                }
                sbvalues.Append(')');
            }

            string rtfields = sbfields.ToString();
            if (rtfields.StartsWith(","))
            {
                rtfields = rtfields.Substring(1);
            }
            rtfields = "(" + rtfields + ")";
            string rtvalues = sbvalues.ToString();
            if (rtvalues.StartsWith(","))
            {
                rtvalues = rtvalues.Substring(1);
            }
            AppendDiv();
            this.Append(string.Format("insert into {0} {1} values {2}", tablename, rtfields, rtvalues));
            return new InsertBuilder<T>(this);
        }
        /// <summary>
        /// 追加更新语句
        /// </summary>
        /// <param name="updated"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public UpdateBuilder<T> Update<T>(T updated, string where = "")
        {
            Type EntityType = typeof(T);
            TableNameAttribute tn = EntityType.GetCustomAttribute<TableNameAttribute>();
            string tablename = tn == null ? EntityType.Name : tn.Name;

            StringBuilder sb = new StringBuilder();
            bool autowhere = string.IsNullOrEmpty(where);


            PropertyInfo[] myProInfos = EntityType.GetProperties();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                if (pi.IsDefined(typeof(DataIgnoreAttribute)) || !DBMapping.IsMapping(pi.PropertyType))
                {
                    continue;
                }
                bool canupdated = true;
                if (pi.IsDefined(typeof(IDAttribute)))
                {
                    if (autowhere)
                    {
                        canupdated = false;
                        if (string.IsNullOrEmpty(where))
                        {
                            where = pi.Name + "=" + _db.AddParam(pi.GetValue(updated));
                        }
                        else
                        {
                            where = where + " and " + pi.Name + "=" + _db.AddParam(pi.GetValue(updated));
                        }
                    }
                }
                if (canupdated)
                {
                    object val = pi.GetValue(updated);
                    if (val == null)
                    {
                        continue;
                    }
                    sb.Append(',');
                    sb.Append(pi.Name);
                    sb.Append('=');
                    sb.Append(_db.AddParam(pi.GetValue(updated)));
                }
            }
            string updatestr = sb.ToString();
            if (updatestr.StartsWith(","))
            {
                updatestr = updatestr.Substring(1);
            }


            string rtSQL = "update " + tablename + " set ";
            if (string.IsNullOrEmpty(where))
            {
                rtSQL += updatestr;
            }
            else
            {
                rtSQL += updatestr + " where " + where;
            }
            AppendDiv();
            this.Append(rtSQL);
            return new UpdateBuilder<T>(this);
        }
        public UpdateBuilder<T> Update<T>(T updated, Expression<Func<T, bool>> expression)
        {
            return Update<T>(updated, this.GetWhereByLambda(expression));
        }

        public UpdateColumnsBuilder<T> UpdateColumns<T>()
        {
            return new UpdateColumnsBuilder<T>(this);
        }
        public CreateOrUpdateBuilder<T> CreateOrUpdate<T>(T entity)
        {
            _comparable.CreateOrUpdate(entity);
            return new CreateOrUpdateBuilder<T>(this);
        }
        public DeleteBuilder<T> Delete<T>(string where)
        {
            return Delete<T>().Append(string.Format(" where {0}", where));
        }
        public DeleteBuilder<T> Delete<T>(Expression<Func<T, bool>> expression)
        {
            return Delete<T>(this.GetWhereByLambda(expression));
        }
        public DeleteBuilder<T> Delete<T>()
        {
            Type EntityType = typeof(T);
            TableNameAttribute tn = EntityType.GetCustomAttribute<TableNameAttribute>();
            string table = tn == null ? EntityType.Name : tn.Name;
            AppendDiv();
            this.Append(string.Format("delete from {0}", table));
            return new DeleteBuilder<T>(this);
        }
        public T Do<T>() where T : IDoCommand, new()
        {
            return _db.DoCommand<T>(this);
        }
        /// <summary>
        /// 异步执行sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> DoAsync<T>() where T : IDoCommand, new()
        {
            return await _db.DoCommandAsync<T>(this);
        }

        /// <summary>
        /// 转换成sql字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_mulSeq)
            {
                return _comparable.MutiWrapSql(_sb.ToString());
            }
            return _sb.ToString();
        }
        /// <summary>
        /// 隐式转换成字符串
        /// </summary>
        /// <param name="sqlBuilder"></param>
        public static implicit operator string(SqlBuilder sqlBuilder)
        {
            return sqlBuilder.ToString();
        }
    }

}
