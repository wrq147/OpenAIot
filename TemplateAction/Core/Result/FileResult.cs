using System;
using System.IO;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    /// <summary>
    /// 文件输出
    /// </summary>
    public class FileResult : IResult
    {
        private string mPath;
        public string Path
        {
            get { return mPath; }
        }

        public FileResult(string path)
        {
            mPath = path;
        }

        public async Task Output(TAAction ac)
        {
            try
            {
                string ext = System.IO.Path.GetExtension(mPath);
                ac.Context.Response.ContentType = FileContentType.GetMimeType(ext);
                using (FileStream fsRead = new FileStream(mPath, FileMode.OpenOrCreate))
                {
                    byte[] heByte = new byte[fsRead.Length];
                    fsRead.Read(heByte, 0, heByte.Length);
                    await ac.Context.Response.BinaryWriteAsync(heByte);
                }
            }
            catch {
                await ac.Context.Response.WriteAsync("文件异常");
            }
        }
    }
}
