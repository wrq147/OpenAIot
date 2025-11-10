using AuthService.Model;
using Common;
using Common.Share;
using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.DAL
{
    public class UpgradeDAL : BaseRepository<MZ_Upgrade>
    {
        public virtual async Task<PageObject<MZ_Upgrade>> SelectByPage(In_UpgradeList query)
        {
            RefAsync<int> total = 0;
            Expression<Func<MZ_Upgrade, bool>> expression = x => true;
            if (!string.IsNullOrEmpty(query.SearchKey))
            {
                expression = expression.And(x => x.UpVersion.Contains(query.SearchKey) || x.Title.Contains(query.SearchKey) || x.UpContent.Contains(query.SearchKey));
            }
            if (query.OrgId != null)
            {
                expression = expression.And(x => x.OrgId == query.OrgId);
            }
            if (query.StyleId != null)
            {
                query.StyleId = query.StyleId.Trim();
                expression = expression.And(x => x.StyleId == query.StyleId);
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.CreatedOn <= query.endTime);
            }
            return await SelectPage(expression, query, string.Empty);
        }
    }
}
