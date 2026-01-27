using Common;
using IoTVideoService.Models;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTVideoService.DAL
{
    public class RecordDAL : BaseRepository<MZ_IotRecord>
    {
        public virtual async Task<List<MZ_IotRecord>> GetUnCleanRecords(DateTime compareTime, int top)
        {
            var tlist = await new SqlBuilder(help).Query<MZ_IotRecord>()
                .Append("select * from mz_iot_record where not exists(select Id from mz_iot_recordlog where PlanId=mz_iot_record.Id and LogType='clean' and ExecTime>").AppendParam(compareTime).Append(")").Take(top)
                 .ToListAsync();
            return tlist;
        }
    }
}
