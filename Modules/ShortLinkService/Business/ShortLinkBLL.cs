using Common.Share;
using ShortLinkService.DAL;
using ShortLinkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public virtual async Task<MZ_ShortLink> Info(string id)
        {
            return await _shortLinkDAL.Select(id);
        }
    }
}
