using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class DeleteBuilderWithWhere<X> : AbstractBuilder<DeleteBuilderWithWhere<X>>
    {
        public DeleteBuilderWithWhere(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        protected override DeleteBuilderWithWhere<X> This()
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
