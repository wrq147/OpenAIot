using Common;
using Common.Share;
using MonitorService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorService.DAL
{
    public class HolidayOrgDAL : BaseRepository<MZ_HolidayOrg>
    {
        public virtual async Task<List<MZ_HolidayOrg>> GetHolidayOrgList(long orgId)
        {
            return await new SqlBuilder(help).Query<MZ_HolidayOrg>().LeftJoin<MZ_HolidayType>((a, b) => a.HolidayTypeId == b.Id).Where((a, b) => a.OrgId == orgId).ToListAsync();
        }
    }
}
