using AuthService;
using Common;
using Common.Share;
using Microsoft.Extensions.Options;
using MyAccess.Core;
using MyAccess.DB.Builder.WhereToSql;
using ShortLinkService.DAL;
using ShortLinkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace ShortLinkService.Business
{
    public class ShortLinkBLL
    {
        private ITAServiceProvider _provider;
        private ShortLinkDAL _shortLinkDAL;
        public ShortLinkBLL(ITAServiceProvider provider, ShortLinkDAL shortLinkDAL)
        {
            _provider = provider;
            _shortLinkDAL = shortLinkDAL;
        }

        public virtual async Task<BusResponse<string>> Delete(string[] ids)
        {
            await _shortLinkDAL.DeleteIn(ids.ToList());
            return BusResponse<string>.Success();
        }
        public virtual async Task<PageObject<MZ_ShortLink>> SelectList(In_ShortLinkList query, IUserInfo user)
        {
            Expression<Func<MZ_ShortLink, bool>> expression;
            if (user.OrgId == 1)
            {
                expression = x => true;
            }
            else
            {
                expression = x => x.OrgId == user.OrgId;
            }
            if (!string.IsNullOrEmpty(query.Key))
            {
                expression = expression.And(x => x.Url.Contains(query.Key) || x.Id == query.Key);
            }
            var tpage = await _shortLinkDAL.SelectPage(expression, query, "CreatedOn desc");
            var orgIds = tpage.List.Select(x => x.OrgId).ToList();
            if (orgIds.Count > 0)
            {
                var orgs = await _provider.GetService<OrgDAL>().SelectList(x => orgIds.Contains(x.Id));
                foreach (var item in tpage.List)
                {
                    item.OrgName = orgs.Where(x => x.Id == item.OrgId).FirstOrDefault()?.OrgName;
                }
            }
            return tpage;
        }

        public virtual async Task<MZ_ShortLink> Info(string id)
        {
            return await _shortLinkDAL.Select(id);
        }
        public virtual async Task<BusResponse<string>> Add(string url, long orgId)
        {

            long tmpval = await _provider.GetService<GeneralRedisHelper>().StringIncrementLongAsync("SL_VAL");
            string tmpstr = $"{tmpval}{(Utility.ThreadLocalRandom.Value.Next(9999)).ToString().PadLeft(4, '0')}";
            MZ_ShortLink data = new MZ_ShortLink();
            data.Id = ShortCodeGenerator.Encode(Convert.ToInt64(tmpstr));
            data.CreatedOn = DateTime.Now;
            data.OrgId = orgId;
            data.Url = url;
            await _shortLinkDAL.Insert(data);
            var generOption = _provider.GetService<IOptions<GeneralOption>>();
            return BusResponse<string>.Success(generOption.Value.url + "/s/" + data.Id);

        }
    }
}
