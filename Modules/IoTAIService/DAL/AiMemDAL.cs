using Common;
using IoTAIService.Models;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTAIService.DAL
{
    public class AiMemDAL : BaseRepository<MZ_AIMem>
    {
        public virtual async Task<List<Out_FaceCount>> GetFaceCount(long orgId)
        {
            return (await new SqlBuilder(help).Append("select HouseId,Count(1) as TotalFace from mz_ai_mem where OrgId=").AppendParam(orgId).Append(" group by HouseId")
                .DoAsync<DoQuerySql<Out_FaceCount>>()).ToList();
        }
    }
}
