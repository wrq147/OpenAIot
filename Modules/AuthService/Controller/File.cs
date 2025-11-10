using Common;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace AuthService.Controller
{
    /// <summary>
    /// 文件处理API
    /// </summary>
    public class File : AbstractLoginedController
    {
        private FileHelper _file;
        private IOptions<GeneralOption> _conf;
        public File(FileHelper file, IOptions<GeneralOption> conf)
        {
            _file = file;
            _conf = conf;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="withDomain">返回路径是否带上域名</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Upload(bool withDomain = false)
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            string fileExt = Path.GetExtension(form.Files[0].FileName);
            if (string.IsNullOrEmpty(fileExt))
            {
                fileExt = _file.ContentTypeToExt(form.Files[0].ContentType);
            }
            string errorType = _conf.Value.limit_file_type.FirstOrDefault(m => m.Equals(fileExt, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(errorType))
            {
                return this.Error<string>(13, "非法的文件类型");
            }
            var file = form.Files[0];
            if (file.ContentLength > _conf.Value.limit_file_size * 1024 * 1024)
            {
                return this.Error<string>(14, string.Format("文件不能超过{0}MB", _conf.Value.limit_file_size));
            }
            string rt = await _file.UploadFile(form.Files[0], withDomain);
            return this.Success(rt);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id">文件路径</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Delete(string id)
        {
            if (await _file.DeleteFile(id))
            {
                return this.Success<string>();
            }
            else
            {
                return this.Error<string>(133, "删除失败");
            }
        }
    }
}
