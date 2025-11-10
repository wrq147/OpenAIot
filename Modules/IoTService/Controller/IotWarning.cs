using AuthService.Controller;
using Common;
using Common.Share;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace IoTService.Controller
{
    public class IotWarning : AbstractLoginedController
    {
        private IotWarningBLL _warningBLL;
        private IotWarnConfigBLL _warningConfigBLL;
        public IotWarning(IotWarningBLL warningBLL, IotWarnConfigBLL warningConfigBLL)
        {
            _warningBLL = warningBLL;
            _warningConfigBLL = warningConfigBLL;
        }

        /// <summary>
        /// 报警列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<PageObject<MZ_IotWarning>>))]
        public async Task<AjaxResult> ListPage(In_WarningListPage query)
        {
            return this.Success(await _warningBLL.ListPage(query, GetUser()));
        }

       
        /// <summary>
        /// 报警信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotWarning>> Info(long id)
        {
            return (await _warningBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 通过工单编号获取报警信息
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotWarning>> InfoByNumber(string number)
        {
            return (await _warningBLL.InfoByNumber(number)).ToAjaxResult();
        }
        /// <summary>
        /// 手动处理报警（批量处理）
        /// </summary>
        /// <param name="devId">设备Id（为空则不过滤）</param>
        /// <param name="code">事件标识（为空则不过滤）</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Des("处理报警事件")]
        public async Task<DefaultAjaxResult<int>> Clear(string devId = "", string code = "", string remark = "")
        {
            if (string.IsNullOrEmpty(remark))
            {
                remark = "手动处理";
            }
            return (await _warningBLL.ClearWarning(devId, code, remark)).ToAjaxResult();
        }

        /// <summary>
        /// 手动处理报警（单个处理）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        [Des("处理报警事件")]
        public async Task<DefaultAjaxResult<int>> ClearOne(long id, string remark = "")
        {
            if (string.IsNullOrEmpty(remark))
            {
                remark = "手动处理";
            }
            return (await _warningBLL.ClearWarning(id, remark, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 报警配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotWarnConfig>> ConfigInfo(string pid)
        {
            var warnInfo = await _warningConfigBLL.WarnConfigInfoByPro(pid);
            if (warnInfo == null)
            {
                var user = GetUser();
                warnInfo = new MZ_IotWarnConfig();
                warnInfo.ProductId = pid;
                warnInfo.OrgId = user.OrgId;
                warnInfo.WarnFlowId = 0;
                warnInfo.WarnFlowInitJson = string.Empty;
                warnInfo.WarnFlowName = string.Empty;
            }
            if (warnInfo.WarnFlowId > 0)
            {
                warnInfo.WarnFlowName = await _warningBLL.QueryFlowTemplateName(warnInfo.WarnFlowId.Value);
            }
            return this.Success(warnInfo);
        }
        /// <summary>
        /// 保存报警配置信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> SaveConfig(MZ_IotWarnConfig data)
        {
            return (await _warningConfigBLL.SetWarnConfig(data,GetUser())).ToAjaxResult();
        }
    }
}
