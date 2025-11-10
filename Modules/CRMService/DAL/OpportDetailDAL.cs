using AuthService;
using Common;
using Common.Share;
using CRMService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMService.DAL
{
    public class OpportDetailDAL : BaseRepository<MZ_OpportDetail>
    {
        public virtual async Task<decimal> CalMaybeTotalPrice(DateTime? from, DateTime? to, string periodId, IUserInfo user)
        {
            var sqlBuilder = new SqlBuilder(help).Query<decimal>().Append("select SUM(d.Quantity*d.Price) from mz_opport_detail d left join mz_opportunity o on d.OpportId=o.Id where o.del_flag='0'")
                .Then(user != null, x => x.Append(" and o.createId=").AppendParam(user.UserId))
                .Then(!string.IsNullOrEmpty(periodId), x => x.Append(" and o.Period=").AppendParam(periodId))
                .Then(from != null, x => x.Append(" and o.create_time>=").AppendParam(from))
                .Then(to != null, x => x.Append(" and o.create_time<").AppendParam(to));

            return (await sqlBuilder.DoAsync<DoQueryScalar>()).GetValueDecimal();
        }
    }
}
