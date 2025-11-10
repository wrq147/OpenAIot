using AuthService;
using Common;
using Common.Share;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.DAL
{
    public class PrintTemplateDAL : BaseRepository<MZ_PrintTemplate>
    {
        public virtual async Task<PageObject<MZ_PrintTemplate>> SelectByPage(In_PrintList query, Data_ServerTokenInfo user)
        {
            Expression<Func<MZ_PrintTemplate, bool>> expression = x => x.OrgId == 0 || x.OrgId == user.OrgId;
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.create_time >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.create_time <= query.endTime);
            }
            if (!string.IsNullOrEmpty(query.DataId))
            {
                expression = expression.And(x => x.DataId == query.DataId);
            }

            return await new SqlBuilder(help).Query<MZ_PrintTemplate>().Where(expression, "Id,OrgId,Name,PaperDirection,PaperMarginTop,PaperMarginBottom,Background,FontFamily,LineHeight,PaperName,PaperWidth,PaperHeight,DataId,createId,create_time,updateId,update_time")
                .GeneratePageObjectAsync(query, "create_time desc");
        }
    }
}
