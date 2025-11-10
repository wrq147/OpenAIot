using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    public class InvitLinkHelper
    {
        public static string GenerateLink(ITAServiceProvider provider, long orgId)
        {
            string cont = "invit," + orgId + "," + DateTime.Now.AddDays(7).ToString("yyyy-MM-dd HH:mm:ss");
            string signkey = MyAccess.Core.Crypter.SHA1(cont + provider.GetService<IOptions<GeneralOption>>().Value.secret_key, System.Text.Encoding.UTF8);
            return MyAccess.Core.Crypter.EncodeBase64(cont + "," + signkey, System.Text.Encoding.UTF8);
        }
        public static long ParseLink(ITAServiceProvider provider, string code)
        {
            string seckey = provider.GetService<IOptions<GeneralOption>>().Value.secret_key;
            string cont = MyAccess.Core.Crypter.DecodeBase64(code, System.Text.Encoding.UTF8);
            string[] arr = cont.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 4)
            {
                throw new Exception("邀请码格式错误");
            }
            string signkey = MyAccess.Core.Crypter.SHA1(arr[0] + "," + arr[1] + "," + arr[2] + seckey, System.Text.Encoding.UTF8);
            if (arr[3] != signkey)
            {
                throw new Exception("邀请码格式错误");
            }
            long orgId = long.Parse(arr[1]);
            DateTime over = Convert.ToDateTime(arr[2]);
            if (over <= DateTime.Now)
            {
                throw new Exception("邀请码已过期");
            }
            return orgId;
        }
    }
}
