using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.KuaiDi.Com
{
    /// <summary>
    /// 顺风接口
    /// </summary>
    public class SFApi : IKuaiDiApi
    {
        public async Task<KDResult<List<TrackItem>>> QueryTrack(string com, string number, string phone)
        {
            string url = $"https://www.sf-express.com/sf-service-web/service/bills/{number}/routes?app=bill&lang=cn&region=cn&translate=&mobile={phone}";
            await HttpHelper.Instance.GetAsync(url);

            return null;
        }

    }
}
