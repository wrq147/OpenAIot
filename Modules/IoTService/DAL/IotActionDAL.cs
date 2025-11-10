using Common;
using Common.DataAc;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTService.DAL
{
    public class IotActionDAL : BaseDbSupport
    {
        public async Task<int> DoAsync(ActionChangeData evt, List<ActionCondition> list1, List<ActionInfo> list2)
        {
            using DbHelp db = CreateDB();
            return await evt.DoAsync(db, list1, list2);
        }
    }
}
