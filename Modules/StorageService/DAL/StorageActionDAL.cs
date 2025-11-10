using Common;
using Common.DataAc;
using MyAccess.DB;
using StorageService.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorageService.DAL
{
    public class StorageActionDAL : BaseDbSupport
    {
        public async Task<int> DoAsync(ActionChangeData evt, List<ActionCondition> list1, List<ActionInfo> list2)
        {
            using DbHelp db = CreateDB();
            return await evt.DoAsync(db, list1, list2);
        }

        public async Task<Tmp_CustomerInfo> SelectCustomerById(string id)
        {
            using DbHelp db = CreateDB();
            var dqs = await new SqlBuilder(db).Append("select Id,OrgId,CustomerNumber,CustomerName,CustomerType,BindOrgId from mz_customer where Id=").AppendParam(id).DoAsync<DoQuerySql<Tmp_CustomerInfo>>();
            return dqs.ToFirst();
        }

    }
}
