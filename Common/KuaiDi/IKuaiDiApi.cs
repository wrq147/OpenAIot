using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.KuaiDi
{
    public interface IKuaiDiApi
    {
        Task<KDResult<List<TrackItem>>> QueryTrack(string com, string number, string phone);
    }
}
