using Common;
using Common.Share;
using LLMService.Controller;
using LLMService.Model;
using MyAccess.DB;
using SqlParser.Net.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LLMService.DAL
{
    public class ArticleDAL : BaseRepository<MZ_Article>
    {

        public virtual async Task<PageObject<MZ_Article>> SelectByPage(In_ArticleQuery query, IUserInfo user)
        {
            Expression<Func<MZ_Article, bool>> expression = (a) => a.Kb.OrgId == 0 || a.Kb.OrgId == user.OrgId;
            if (query.KbId.HasValue)
            {
                expression = expression.And((a, kb) => a.kb_id == query.KbId.Value);
            }
            if (!string.IsNullOrEmpty(query.Title))
            {
                expression = expression.And((a, kb) => a.title.Contains(query.Title));
            }
            if (query.Status.HasValue)
            {
                expression = expression.And((a, kb) => a.status == query.Status.Value);
            }
            if (query.DocType.HasValue)
            {
                expression = expression.And((a, kb) => a.doc_type == query.DocType.Value);
            }

            var tmpSql = new SqlBuilder(help).Query<T_Article>()
                .LeftJoin<T_KnowledgeBase>((a, kb) => a.kb_id == kb.id)
                .Where(expression, "a.*,kb.name as KbName");
            var pageResult = await tmpSql.GeneratePageObjectAsync(query, "a.create_time desc");

            return pageResult;
        }


        public virtual async Task<int> IncrementViewCount(string id)
        {
            SqlBuilder sql = new SqlBuilder(help);
            return await sql.UpdateColumns<MZ_Article>()
                .SetColum(a => a.ViewCount, a => a.ViewCount + 1)
                .Where(a => a.Id == id)
                .DoAsync();
        }


    }


}