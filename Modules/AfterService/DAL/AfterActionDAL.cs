using Common.DataAc;
using Common;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace AfterService.DAL
{
    public class AfterActionDAL : BaseDbSupport
    {
        public async Task<int> DoAsync(ActionChangeData evt, List<ActionCondition> list1, List<ActionInfo> list2)
        {
            return await evt.DoAsync(CreateDB(), list1, list2);
        }
    }
}
