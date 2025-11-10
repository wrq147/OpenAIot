using Common;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;

namespace AuthService.Controller
{
    [About]
    public class LoginiLog : AbstractLoginedController
    {
        private LoginLogBLL _loginLogBLL;
        public LoginiLog(LoginLogBLL loginLogBLL)
        {
            _loginLogBLL = loginLogBLL;
        }
        [About]
        [HttpGet]
        public async Task<AjaxResult> List(In_LoginLogList query)
        {
            return this.Success(await _loginLogBLL.SelectLogininforList(query));
        }
        [About]
        [HttpGet]
        public async Task<IResult> Export(In_LoginLogList query)
        {
            try
            {
                query.showAll = true;
                PageObject<MZ_LoginLog> page = await _loginLogBLL.SelectLogininforList(query);
                List<MZ_LoginLog> list = page.List;
                Dictionary<string, ParamRenderToExcel<MZ_LoginLog>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_LoginLog>>();
                FiedNames.Add("SysLogID", new ParamRenderToExcel<MZ_LoginLog>("日志主键"));
                FiedNames.Add("UserName", new ParamRenderToExcel<MZ_LoginLog>("用户名"));
                FiedNames.Add("IPAddress", new ParamRenderToExcel<MZ_LoginLog>("Ip地址"));
                FiedNames.Add("IPLocation", new ParamRenderToExcel<MZ_LoginLog>("登录地点"));
                FiedNames.Add("Status", new ParamRenderToExcel<MZ_LoginLog>("登录状态", x => x.Status == 0 ? "成功" : "失败"));
                FiedNames.Add("CreateDate", new ParamRenderToExcel<MZ_LoginLog>("登录时间", x => x.CreateDate == null ? "" : x.CreateDate.Value.ToString("yyyy-MM-dd HH:mm")));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_LoginLog>("登录日志表", list, FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }

        [HttpGet]
        [About()]
        public async Task<AjaxResult> Remove(long[] id)
        {
            return this.Success(await _loginLogBLL.DeleteLoginLogByIds(id));
        }
        [HttpGet]
        [About("Remove")]
        public async Task<AjaxResult> Clean()
        {
            return this.Success(await _loginLogBLL.CleanLoginLog());
        }
    }
}
