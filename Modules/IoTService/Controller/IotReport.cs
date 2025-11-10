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
using TemplateAction.Route;

namespace IoTService.Controller
{
    /// <summary>
    /// Iot报表接口
    /// </summary>
    public class IotReport : AbstractLoginedController
    {
        private IotDeviceBLL _deviceBLL;
        public IotReport(IotDeviceBLL deviceBLL)
        {
            _deviceBLL = deviceBLL;
        }

        /// <summary>
        /// 设备数量统计
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_DeviceStatistics>> StatisticsInfo(long orgId = 0)
        {
            var res = await _deviceBLL.StatisticsInfo(orgId);
            return this.Success(res);
        }
        /// <summary>
        /// 设备状态数量统计
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_DeviceRunStatistics>>> RunStatisticsInfo(In_RunStatisticsInfo data)
        {
            var res = await _deviceBLL.RunStatisticsInfo(data);
            return this.Success(res);
        }
        /// <summary>
        /// 指定告警代码的数量统计
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Dictionary<string, int>>> WarnCount(string[] codes)
        {
            var res = await this.ServiceProvider.GetService<IotWarningBLL>().GetWarningCountByCode(codes, GetUser());
            return this.Success(res);
        }
    }
}
