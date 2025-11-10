using Common;
using Common.Share;
using Microsoft.AspNetCore.Hosting;
using ReportService.Business;
using ReportService.Models;
using System;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace ReportService.Controller
{
    /// <summary>
    /// 报表history路由用
    /// </summary>
    public class Preview : TANetController
    {

        /// <summary>
        /// 预览用
        /// </summary>
        /// <returns></returns>
        [Route("report/datav/datavView")]
        [HttpGet]
        public IResult DatavView()
        {
            IWebHostEnvironment env = this.ServiceProvider.GetService<IWebHostEnvironment>();
            string tpath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, "/index.html");
            TemplateDocument indexTemp = TemplateApp.Instance.LoadViewPage(tpath);
            if (indexTemp == null)
            {
                return new V404Result();
            }
            return new ViewResult(indexTemp.MakeHtml(this.IntentAction.TemplateContext));
        }
        /// <summary>
        /// 发布用
        /// </summary>
        /// <returns></returns>
        [Route("report/datav/datavRelease")]
        [HttpGet]
        public IResult DatavRelease()
        {
            IWebHostEnvironment env = this.ServiceProvider.GetService<IWebHostEnvironment>();
            string tpath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, "/index.html");
            TemplateDocument indexTemp = TemplateApp.Instance.LoadViewPage(tpath);
            if (indexTemp == null)
            {
                return new V404Result();
            }
            return new ViewResult(indexTemp.MakeHtml(this.IntentAction.TemplateContext));
        }
        /// <summary>
        /// 获取指定报表的当前更新时间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<long>> ReportUpdateTime(string id)
        {
            var reportBLL = this.ServiceProvider.GetService<ReportBLL>();
            var time = await reportBLL.GetReportUpdateTime(id);
            if (time == null)
            {
                return this.Error<long>(21, "更新时间不存在", 0);
            }
            return this.Success(MyAccess.Core.TypeConvert.Time2Unix(time.Value));
        }
        /// <summary>
        /// 获取报表信息
        /// </summary>
        /// <param name="id">报表Id</param>
        /// <param name="pwd">查看密码</param>
        /// <returns>返回代码1，则表示需要输入查看密码</returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_ShareReport>> CheckShare(string id, string pwd = "")
        {
            var shareBLL = this.ServiceProvider.GetService<ShareBLL>();
            var shareRsp = await shareBLL.Info(id);
            if (!shareRsp.IsSuccess())
            {
                return this.Error<Out_ShareReport>(11, shareRsp.Message);
            }
            var reportBLL = this.ServiceProvider.GetService<ReportBLL>();
            var tmpresport = await reportBLL.Info(shareRsp.Data.ReportId, null);
            if (!tmpresport.IsSuccess())
            {
                return this.Error<Out_ShareReport>(12, tmpresport.Message);
            }
            if (tmpresport.Data.Status != "2")
            {
                return this.Error<Out_ShareReport>(13, "报表未发布，无法使用");
            }
            //验证是否过期
            if (shareRsp.Data.ExpirationTime < DateTime.Now)
            {
                return this.Error<Out_ShareReport>(14, "分享链接已过期");
            }

            if (!string.IsNullOrEmpty(shareRsp.Data.UsingPassword))
            {
                if (string.IsNullOrEmpty(pwd))
                {
                    return this.Error<Out_ShareReport>(1, "请输入查看密码");
                }
                if (shareRsp.Data.UsingPassword.ToLower() != pwd.ToLower())
                {
                    return this.Error<Out_ShareReport>(16, "查看密码错误");
                }
            }

            var tmpshr = new Out_ShareReport();
            tmpshr.Share = shareRsp.Data;
            tmpshr.Report = tmpresport.Data;
            return this.Success(tmpshr);
        }
    }
}
