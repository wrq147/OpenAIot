using Common;
using Common.Share;
using LLMService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LLMService.DAL
{
    public class KnowledgeDAL : BaseRepository<MZ_Knowledge>
    {
        public virtual async Task<PageObject<MZ_Knowledge>> SelectByPage(In_KnowledgeQuery query, IUserInfo user)
        {
            Expression<Func<MZ_Knowledge, bool>> expression;
            if (query.WithPublic == true)
            {
                expression = (kb) => kb.OrgId == user.OrgId || kb.OrgId == 0;
            }
            else
            {
                expression = (kb) => kb.OrgId == user.OrgId;
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expression = expression.And((kb) => kb.Name.Contains(query.Name));
            }

            if (query.Status.HasValue)
            {
                expression = expression.And((kb) => kb.Status == query.Status.Value);
            }

            var tmpSql = new SqlBuilder(help).Query<MZ_Knowledge>().Where(expression);
            var pageResult = await tmpSql.GeneratePageObjectAsync(query, "create_time desc");
            return pageResult;
        }


    }

}