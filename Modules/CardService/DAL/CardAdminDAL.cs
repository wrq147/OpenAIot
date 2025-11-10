using AuthService.Model;
using Common;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace CardService.DAL
{
    public class CardAdminDAL : BaseDbSupport
    {
        public async Task<long> SelectUsingCardId(long uid)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Query<MZ_AdminExt>().Where(x => x.UserId == uid && x.ExtField == "CardId").ToFirstAsync();
                if (docmd == null)
                {
                    return 0;
                }
                return long.Parse(docmd.ExtValue);
            }
        }

    }
}
