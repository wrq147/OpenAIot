using AuthService;
using Common;
using Common.Share;
using DiscussService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Builder.WhereToSql;
namespace DiscussService.DAL
{
    public class CommentDAL : BaseRepository<MZ_Comment>
    {
        public virtual async Task<PageObject<MZ_Comment>> SelectByPage(In_CommentList query, Data_ServerTokenInfo user)
        {
            Expression<Func<MZ_Comment, bool>> expression = x => x.del_flag == "0";
            if (!string.IsNullOrEmpty(query.TargetType))
            {
                expression = expression.And(x => x.Subject.TargetType == query.TargetType);
            }
            if (!string.IsNullOrEmpty(query.TargetId))
            {
                expression = expression.And(x => x.Subject.TargetId == query.TargetId);
            }
            else
            {
                if (query.IsMy == true)
                {
                    //我的评论
                    expression = expression.And(x => x.UserId == user.UserId);
                }
                else if (query.IsMyReply == true)
                {
                    //回复我的
                    expression = expression.And(x => (x.ParentCommentUserId == user.UserId || x.Subject.createId == user.UserId));
                }
                else
                {
                    expression = expression.And(x => x.OrgId == user.OrgId);
                }
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.CreateOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.CreateOn <= query.endTime);
            }
            return await new SqlBuilder(help).Query<MZ_Comment>().Include(x => x.Subject, x => x.SubjectId).Include(y => y.UserInfo, y => y.UserId).Where(expression).GeneratePageObjectAsync(query, string.Empty);
        }
    }
}
