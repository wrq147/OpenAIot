using AuthService;
using AuthService.Controller;
using Common;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;


namespace IoTService.Controller
{
    /// <summary>
    /// 数据修正接口
    /// </summary>
    public class IotData : AbstractLoginedController
    {

        /// <summary>
        /// 重新计算属性规则
        /// </summary>
        /// <returns></returns>
        [About("/IoTService/IotData/Update")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> NoticeCalProp(In_CalProp data)
        {
            var winRule = this.ServiceProvider.GetService<IotWinRuleBLL>();
            await winRule.NoticeCalDevice(new Common.EventBus.QuartzContext()
            {
                PreviousFireTimeUtc = data.Time,
                ScheduledFireTimeUtc = data.Time
            });
            return this.Success(string.Empty);
        }

        /// <summary>
        /// 从一台设备拷贝数据到另一台设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/IoTService/IotData/Update")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> CopyData(In_CopyHis data)
        {
            var influxBLL = this.ServiceProvider.GetService<IotInfluxBLL>();
            var rs = await influxBLL.CopyDeviceHistory(data.SourceId, data.TargetId, data.Mapping, data.StartTime, data.EndTime);
            return rs.ToAjaxResult();
        }
    }
}
