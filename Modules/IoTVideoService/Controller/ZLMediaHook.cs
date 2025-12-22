using IoTVideoService.Business;
using IoTVideoService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.NetCore;

namespace IoTVideoService.Controller
{
    public class ZLMediaHook : TANetController
    {
        private FixVideoBLL _fixVideoBLL;
        public ZLMediaHook(FixVideoBLL fixVideoBLL)
        {
            _fixVideoBLL = fixVideoBLL;
        }
        public async Task<HookResult> StreamNotFound(In_HookNotFound data)
        {
            if (string.IsNullOrEmpty(data.stream))
            {
                return new HookResult(13, "stream不能为空");
            }
            await _fixVideoBLL.CollectVideo(data.stream);
            return new HookResult(0, "success");
        }
        public async Task<HookNoneRResult> StreamNoneReader(In_HookNoneReader data)
        {
            await _fixVideoBLL.DelVideo(data.stream);
            return new HookNoneRResult(0, true);
        }
    }
}
