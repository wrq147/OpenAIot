using System;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class UpdateBuilder<X> : AbstractBuilder<UpdateBuilder<X>>
    {
        public UpdateBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        protected override UpdateBuilder<X> This()
        {
            return this;
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
