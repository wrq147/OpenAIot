using AuthService.Model;
using Common;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.DAL
{
    public class OrgExtDAL : BaseRepository<MZ_OrgExt>
    {
        public virtual async Task<int> InsertOrUpdate(MZ_OrgExt ext)
        {
            return (await new SqlBuilder(help).Insert(ext).Append(" ON DUPLICATE KEY UPDATE ExtValue=").AppendParam(ext.ExtValue)
    .DoAsync<DoExecSql>()).RowCount;
        }
    }
}
