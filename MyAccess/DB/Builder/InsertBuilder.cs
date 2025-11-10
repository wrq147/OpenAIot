using MyAccess.DB.Attr;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class InsertBuilder<X> : AbstractBuilder<InsertBuilder<X>>
    {
        public InsertBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        protected override InsertBuilder<X> This()
        {
            return this;
        }

        private string FindIdName()
        {
            string idName = string.Empty;
            Type EntityType = typeof(X);
            PropertyInfo[] myProInfos = EntityType.GetProperties();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                if (pi.IsDefined(typeof(IDAttribute)))
                {
                    idName = pi.Name;
                    break;
                }
            }
            return idName;
        }
        /// <summary>
        /// 执行DoExecReturnIdentity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DoExecReturnIdentity DoReturnIdentity()
        {
            return _sqlBuilder.Db.DoCommand(new DoExecReturnIdentity(_sqlBuilder, FindIdName()));
        }
        /// <summary>
        /// 执行DoExecReturnIdentity（异步）
        /// </summary>
        /// <returns></returns>
        public async Task<DoExecReturnIdentity> DoReturnIdentityAsync()
        {
            return await _sqlBuilder.Db.DoCommandAsync(new DoExecReturnIdentity(_sqlBuilder, FindIdName()));
        }
        public int Do()
        {
            return _sqlBuilder.Do<DoExecSql>().RowCount;
        }
        public async Task<int> DoAsync()
        {
            return (await _sqlBuilder.DoAsync<DoExecSql>()).RowCount;
        }
    }
}
