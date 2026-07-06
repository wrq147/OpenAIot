using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAccess.DB.Builder
{
    public class CreateOrUpdateBuilder<X> : AbstractBuilder<CreateOrUpdateBuilder<X>>
    {
        public CreateOrUpdateBuilder(SqlBuilder sqlBuilder) : base(sqlBuilder)
        {
        }
        protected override CreateOrUpdateBuilder<X> This()
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
