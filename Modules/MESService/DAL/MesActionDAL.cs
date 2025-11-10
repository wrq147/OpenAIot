using Common.DataAc;
using Common;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESService.DAL
{
    public class MesActionDAL : BaseDbSupport
    {
        public async Task<int> DoAsync(ActionChangeData evt, List<ActionCondition> list1, List<ActionInfo> list2)
        {
            using DbHelp db = CreateDB();
            return await evt.DoAsync(db, list1, list2);
        }

    }
}
