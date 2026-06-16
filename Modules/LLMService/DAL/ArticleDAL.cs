using Common;
using Common.Share;
using JiebaNet.Segmenter;
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
    public class ArticleDAL : BaseRepository<MZ_Article>
    {
        public virtual async Task<List<Out_ArticleItem>> SelectArticleItems(string kbId)
        {
            return await new SqlBuilder(help).Query<Out_ArticleItem>().Where(x => x.KbId == kbId, "Id,KbId,Title,ColumnId").ToListAsync();
        }

        public virtual async Task<PageObject<MZ_Article>> SelectByPage(In_ArticleQuery query, IUserInfo user)
        {
            Expression<Func<MZ_Article, bool>> expression = (a) => a.Kb.OrgId == 0 || a.Kb.OrgId == user.OrgId;
            if (!string.IsNullOrEmpty(query.KbId))
            {
                expression = expression.And((a) => a.KbId == query.KbId);
            }
            if (!string.IsNullOrEmpty(query.Key))
            {
                var keys = new JiebaSegmenter().CutForSearch(query.Key);
                expression = expression.And((a) => SonSqlFun.FullSearch("KeyWords", keys));
            }
            if (query.IsPublic == true)
            {
                expression = expression.And((a) => a.Kb.Status == 1);
            }

            var tmpSql = new SqlBuilder(help).Query<MZ_Article>()
                .Include(a => a.Kb, a => a.KbId).Include(a => a.KbColumn, a => a.ColumnId)
                .Where(expression);
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