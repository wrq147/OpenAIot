using Common;
using Common.Share;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace AuthService.Controller
{

    public class FileTemp : TANetController
    {
        private FileHelper _file;
        private IOptions<GeneralOption> _conf;
        public FileTemp(FileHelper file, IOptions<GeneralOption> conf)
        {
            _file = file;
            _conf = conf;
        }
        /// <summary>
        /// 上传临时文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Upload()
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            string tmpfilekey = this.Context.Request.Header["FileKey"];
            if (string.IsNullOrEmpty(tmpfilekey) || tmpfilekey != _conf.Value.tmp_filekey)
            {
                return this.Error<string>(11, "无权限上传临时文件");
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
            string rt = await _file.UploadTmpFile(form.Files[0]);
            return this.Success(rt);
        }
    }
}
