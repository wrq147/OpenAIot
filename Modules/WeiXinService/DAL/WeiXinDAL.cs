using MyAccess.DB;
using System.Threading.Tasks;
using WeiXinService.Model;
using Common;

namespace WeiXinService.DAL
{
    public class WeiXinDAL : BaseDbSupport
    {
        public virtual async Task<MZ_WeiXin> SelectById(string unionId, string appId)
        {
            var docmd = await new SqlBuilder(help).Append("select * from mz_weixin where UnionId=").AppendParam(unionId).Append(" and AppId=").AppendParam(appId).DoAsync<DoQuerySql<MZ_WeiXin>>();
            return docmd.ToFirst();
        }
        public virtual async Task<MZ_WeiXin> SelectByUid(long uid,string appId)
        {
            var docmd=await new SqlBuilder(help).Append("select w.* from mz_weixin w left join mz_admin_wx x on w.UnionId=x.UnionId where x.Id=").AppendParam(uid).Append(" and w.AppId=").AppendParam(appId).DoAsync<DoQuerySql<MZ_WeiXin>>();
            return docmd.ToFirst();
        }
        public virtual async Task<int> Insert(MZ_WeiXin model)
        {
            try
            {
                var docmd = await new SqlBuilder(help).Insert(model).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
            catch
            {
                return -1;
            }

        }


    }
}
