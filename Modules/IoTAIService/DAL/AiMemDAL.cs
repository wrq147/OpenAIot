using Common;
using Common.Share;
using IoTAIService.Models;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Label;

namespace IoTAIService.DAL
{
    public class AiMemDAL : BaseRepository<MZ_AIMem>
    {
        public virtual async Task<PageObject<MZ_AIMem>> FacePage(In_FaceList data, IUserInfo user)
        {
            Expression<Func<MZ_AIMem, bool>> expression = x => x.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(data.Name))
            {
                expression = expression.And(x => x.MemInfo.RealName.Contains(data.Name));
            }
            var tsql = new SqlBuilder(help).Query<MZ_AIMem>().Include(x => x.MemInfo, x => x.MemId).Include(x => x.HouseInfo, x => x.HouseId);
            return await tsql.Where(expression).GeneratePageObjectAsync(data, "a.CreatedOn desc");
        }

        public virtual async Task<List<Out_FaceCount>> GetFaceCount(long orgId)
        {
            return (await new SqlBuilder(help).Append("select HouseId,Count(1) as TotalFace from mz_ai_mem where OrgId=").AppendParam(orgId).Append(" group by HouseId")
                .DoAsync<DoQuerySql<Out_FaceCount>>()).ToList();
        }
        public virtual async Task<List<MZ_AIMem>> SelectFaceMem(List<long> ids)
        {
            var tsql = new SqlBuilder(help).Query<MZ_AIMem>().Include(x => x.MemInfo, x => x.MemId).Where(x => ids.Contains(x.MemId.Value));
            return await tsql.ToListAsync();
        }
    }
}
