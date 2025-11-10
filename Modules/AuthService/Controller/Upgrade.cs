using Common.Share;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;
using TemplateAction.NetCore;
using Common.UserAgent;
using AuthService.Business;
using AuthService.Model;
using Microsoft.AspNetCore.Hosting;
using TemplateAction.Common;
using System.IO;

namespace AuthService.Controller
{
    /// <summary>
    /// App升级接口
    /// </summary>
    public class Upgrade : TANetController
    {
        private UpgradeBLL _upgradeBLL;
        public Upgrade(UpgradeBLL upgradeBLL)
        {
            _upgradeBLL = upgradeBLL;
        }
        private (long startByte, long endByte) GetRange(string rangeHeader, long fileSize)
        {
            if (rangeHeader.Length <= 6) return (0, fileSize);
            var ranges = rangeHeader[6..].Split("-");
            try
            {
                if (ranges[1].Length > 0)
                {
                    return (long.Parse(ranges[0]), long.Parse(ranges[1]));
                }
            }
            catch (Exception)
            {
                return (long.Parse(ranges[0]), fileSize - 1);
            }

            return (long.Parse(ranges[0]), fileSize - 1);
        }
        /// <summary>
        /// 最新App下载信息
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="styleId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Upgrade>> Info(long orgId = 0, string styleId = "")
        {
            string plat = "android";
            UserAgent ua = UserAgentHelper.Parse(Context.Request.UserAgent);
            if (ua != null)
            {
                if (ua.Platform == "Android")
                {
                    plat = "android";
                }
                else if (ua.Platform == "iOS")
                {
                    plat = "ios";
                }
            }

            var lastUp = await _upgradeBLL.SelectLast(0, plat, orgId, styleId);
            if (lastUp == null)
            {
                return this.Error<MZ_Upgrade>(22, $"{plat}版App不存在，请联系管理员添加");
            }
            return this.Success(lastUp);
        }
        /// <summary>
        /// App下载地址
        /// </summary>
        /// <param name="orgId">所属企业</param>
        /// <param name="styleId">所属主题</param>
        /// <returns></returns>
        [HttpGet]
        [Route("UpgradeDown")]
        public async Task<IResult> Down(long orgId = 0, string styleId = "")
        {
            string plat = "android";
            UserAgent ua = UserAgentHelper.Parse(Context.Request.UserAgent);
            if (ua != null)
            {
                if (ua.Platform == "Android")
                {
                    plat = "android";
                }
                else if (ua.Platform == "iOS")
                {
                    plat = "ios";
                }
            }

            var lastUp = await _upgradeBLL.SelectLast(0, plat, orgId, styleId);
            if (lastUp == null)
            {
                return this.Error<string>(22, $"{plat}版App不存在，请联系管理员添加");
            }

            IWebHostEnvironment env = this.ServiceProvider.GetService<IWebHostEnvironment>();
            string fullPath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, lastUp.UpUrl);


            // 检查文件是否存在
            if (!System.IO.File.Exists(fullPath))
            {
                return new V404Result();
            }

            // 获取文件信息
            var fileInfo = new FileInfo(fullPath);
            var fileSize = fileInfo.Length;

            // 检查是否支持断点续传
            var rangeHeader = Context.Request.Header.Get("Range");
            if (!string.IsNullOrEmpty(rangeHeader))
            {
                // 解析 Range 头部，获取断点续传的起始位置和结束位置
                var (startByte, endByte) = GetRange(rangeHeader, fileSize);

                // 设置响应头部
                Response.StatusCode = 206; // Partial Content
                Response.AppendHeader("Accept-Ranges", "bytes");
                Response.AppendHeader("Content-Range", $"bytes {startByte}-{endByte}/{fileSize}");

                // 设置响应内容
                var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                fileStream.Seek(startByte, SeekOrigin.Begin);
                using var fileStream0 = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                // 设置文件流的位置为起始字节位置
                fileStream0.Position = startByte;

                // 计算要读取的字节数
                var totalBytesToRead = endByte - startByte + 1;

                // 创建一个字节数组来存储读取的字节
                var buffer = new byte[totalBytesToRead];

                // 从文件流中读取字节
                var bytesRead = fileStream.Read(buffer, 0, (int)totalBytesToRead);

                // 创建 FileStreamResult 对象并设置相关属性
                var result = new StreamResult(fileInfo.Name, buffer);
                return result;
            }

            // 设置响应头部
            Response.AppendHeader("Accept-Ranges", "bytes");
            Response.AppendHeader("Content-Range", $"bytes {0}-{fileSize - 1}/{fileSize}");
            // 设置响应内容
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return new StreamResult(fileInfo.Name, fileBytes);
        }

        /// <summary>
        /// 检测是否需要更新
        /// </summary>
        /// <param name="appVersion">App版本</param>
        /// <param name="wgtVersion">wgt版本</param>
        /// <param name="orgId">所属企业</param>
        /// <param name="styleId">所属主题</param>
        /// <returns></returns>

        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Upgrade>> CheckVersion(string appVersion, string wgtVersion, long orgId = 0, string styleId = "")
        {
            if (string.IsNullOrEmpty(wgtVersion))
            {
                return this.Error<MZ_Upgrade>(15, "wgt版本号不能为空");
            }
            if (string.IsNullOrEmpty(appVersion))
            {
                return this.Error<MZ_Upgrade>(16, "App版本号不能为空");
            }
            string plat = "android";
            UserAgent ua = UserAgentHelper.Parse(Context.Request.UserAgent);
            if (ua.Platform == "Android")
            {
                plat = "android";
            }
            else if (ua.Platform == "iOS")
            {
                plat = "ios";
            }

            //查询最新App
            var appUp = await _upgradeBLL.SelectLast(0, plat, orgId, styleId);
            if (appUp == null)
            {
                return this.Success<MZ_Upgrade>();
            }
            bool canwgt = true;
            bool appgreater = MyAccess.Core.StringTool.VersionCompare(wgtVersion, appUp.UpVersion) > 0;
            if (appgreater && appUp.IsMandatory == true)
            {
                canwgt = false;
            }
            //查询是否可直接wgt升级
            if (canwgt)
            {
                var lastUp = await _upgradeBLL.SelectLast(1, plat, orgId, styleId);
                if (lastUp == null)
                {
                    if (!appgreater)
                    {
                        return this.Success<MZ_Upgrade>();
                    }
                }
                else
                {
                    if (MyAccess.Core.StringTool.VersionCompare(wgtVersion, lastUp.UpVersion) > 0)
                    {
                        //判断是否需要先升级app
                        if (MyAccess.Core.StringTool.VersionCompare(appVersion, lastUp.MinAppVersion) > 0)
                        {
                            return this.Success(appUp);
                        }
                        return this.Success(lastUp);
                    }
                    else
                    {
                        return this.Success<MZ_Upgrade>();
                    }
                }
            }


            if (MyAccess.Core.StringTool.VersionCompare(appVersion, appUp.UpVersion) > 0)
            {
                return this.Success(appUp);
            }
            return this.Success<MZ_Upgrade>();
        }
    }
}
