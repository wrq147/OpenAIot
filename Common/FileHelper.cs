using Common.Attr;
using Common.Share;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;


namespace Common
{
    public class FileHelper
    {
        private ITAServiceProvider _provider;
        private IOptions<GeneralOption> _option;
        private static string[] ImageType = { ".jpg", ".jpeg", ".gif", ".png", ".bmp" };
        private static Dictionary<string, string> ContentTypeMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {

                { "image/bmp" ,".bmp"},
                { "image/jpeg",".jpg" },
                { "image/png",".png" },
                { "application/msword",".doc" },
                { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ,".docx"},
                { "video/mpeg" ,".mpeg"},
                { "video/quicktime",".mov" },
                { "audio/mpeg",".mp3" },
                { "video/mp4",".mp4" },
                { "application/vnd.ms-powerpoint",".ppt" },
                { "application/vnd.openxmlformats-officedocument.presentationml.presentation",".pptx" },
                { "application/octet-stream",".rar" },
                { "application/x-zip-compressed" ,".zip"},
                { "application/pdf",".pdf" },
                { "application/vnd.ms-excel",".xls" }
            };
        public FileHelper(ITAServiceProvider provider, IOptions<GeneralOption> option)
        {
            _provider = provider;
            _option = option;
        }
        /// <summary>
        /// ContentType转扩展名
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public string ContentTypeToExt(string contentType)
        {
            string rt;
            if (ContentTypeMapping.TryGetValue(contentType, out rt))
            {
                return rt;
            }
            return string.Empty;
        }
        public async Task<string> UploadBase64(string base64Str)
        {
            base64Str = base64Str.Replace("data:image/gif;base64,", "").Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
            byte[] bytes = Convert.FromBase64String(base64Str);
            var data = base64Str.Substring(0, 5);
            string fileExt = string.Empty;
            switch (data.ToUpper())
            {
                case "IVBOR":
                    fileExt = ".png";
                    break;
                case "/9J/4":
                    fileExt = ".jpg";
                    break;
                case "R0lGO":
                    fileExt = ".gif";
                    break;
            }

            if (string.IsNullOrEmpty(_option.Value.minio_server))
            {

                Image image = Image.Load(bytes);

                IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
                string fileName = MyAccess.Core.StringTool.GetGUID() + fileExt;
                DateTime now = DateTime.Now;
                string saveurl = string.Format("/uploads/{0}/{1}/{2}", now.Year, now.ToString("MMdd"), fileName);
                string filepath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, saveurl);
                string dirPath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                image.Save(filepath);
                return saveurl;
            }
            else
            {
                var client = _provider.GetService<MinioHelper>();
                string fileName = DateTime.Now.ToString("yyyyMMdd") + "/" + MyAccess.Core.StringTool.GetGUID() + fileExt;
                return await client.UploadFile(bytes, fileName).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<byte[]> DownFile(string url)
        {
            var clientFactory = _provider.GetService<IHttpClientFactory>();
            using (var client = clientFactory.CreateClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        stream.CopyTo(memStream);
                        return memStream.ToArray();
                    }
                }
            }
        }

        /// <summary>
        /// 图片下载到本地
        /// </summary>
        /// <param name="url"></param>
        /// <param name="enableThumb"></param>
        /// <returns></returns>
        public async Task<string> DownFileToLocal(string url, bool enableThumb = true)
        {
            var bytes = await DownFile(url);
            var strurl = await UploadFile(bytes, ".jpg", false, enableThumb);
            return strurl;
        }


        /// <summary>
        /// 上传文件（直接数据）
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fileExt"></param>
        /// <param name="withDomain"></param>
        /// <param name="enableThumb"></param>
        /// <returns></returns>
        public async Task<string> UploadFile(byte[] data, string fileExt, bool withDomain, bool enableThumb = true)
        {
            if (string.IsNullOrEmpty(_option.Value.minio_server))
            {
                Stream newstr = new MemoryStream(data);
                IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
                string fileName = MyAccess.Core.StringTool.GetGUID() + fileExt;
                DateTime now = DateTime.Now;
                string saveurl = string.Format("/uploads/{0}/{1}/{2}", now.Year, now.ToString("MMdd"), fileName);
                string filepath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, saveurl);
                string dirPath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                //判断是否为图片
                string errorType = ImageType.FirstOrDefault(m => m.Equals(fileExt, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(errorType) && _option.Value.enable_thumb && enableThumb)
                {
                    //生成缩略图
                    string thumburl = string.Format("/uploads/{0}/{1}/s_{2}", now.Year, now.ToString("MMdd"), fileName);
                    string thumbpath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, thumburl);

                    ImageHelper img = new ImageHelper(newstr);
                    await img.CompressSaveAsync(500, 500, thumbpath);
                }

                //保存原文件
                using (FileStream fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                {
                    await newstr.CopyToAsync(fileStream);
                }
                if (withDomain)
                {
                    var generalOption = _provider.GetService<IOptions<GeneralOption>>();
                    return generalOption.Value.url + saveurl;
                }
                else
                {
                    return saveurl;
                }
            }
            else
            {
                Stream st = new MemoryStream(data);
                byte[] bytes = new byte[st.Length];
                await st.ReadAsync(bytes, 0, bytes.Length);
                string fileName = DateTime.Now.ToString("yyyyMMdd") + "/" + MyAccess.Core.StringTool.GetGUID() + fileExt;
                var client = _provider.GetService<MinioHelper>();
                return await client.UploadFile(bytes, fileName, withDomain);
            }
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="withDomain"></param>
        /// <returns></returns>
        public async Task<string> UploadFile(IRequestFile file, bool withDomain)
        {
            if (string.IsNullOrEmpty(_option.Value.minio_server))
            {
                IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
                string fileExt = Path.GetExtension(file.FileName);
                if (string.IsNullOrEmpty(fileExt))
                {
                    fileExt = ContentTypeToExt(file.ContentType);
                }
                string fileName = MyAccess.Core.StringTool.GetGUID() + fileExt;
                DateTime now = DateTime.Now;
                string saveurl = string.Format("/uploads/{0}/{1}/{2}", now.Year, now.ToString("MMdd"), fileName);
                string filepath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, saveurl);
                string dirPath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                //判断是否为图片
                string errorType = ImageType.FirstOrDefault(m => m.Equals(fileExt, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(errorType) && _option.Value.enable_thumb)
                {
                    //生成缩略图
                    string thumburl = string.Format("/uploads/{0}/{1}/s_{2}", now.Year, now.ToString("MMdd"), fileName);
                    string thumbpath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, thumburl);
                    ImageHelper img = new ImageHelper(file.OpenReadStream());
                    await img.CompressSaveAsync(500, 500, thumbpath);
                }

                //保存原文件
                await file.SaveAsAsync(filepath);
                if (withDomain)
                {
                    var generalOption = _provider.GetService<IOptions<GeneralOption>>();
                    return generalOption.Value.url + saveurl;
                }
                else
                {
                    return saveurl;
                }
            }
            else
            {
                using (Stream st = file.OpenReadStream())
                {
                    byte[] bytes = new byte[st.Length];
                    await st.ReadAsync(bytes, 0, bytes.Length);
                    string ext = Path.GetExtension(file.FileName);
                    if (string.IsNullOrEmpty(ext))
                    {
                        ext = ContentTypeToExt(file.ContentType);
                    }
                    var client = _provider.GetService<MinioHelper>();
                    string fileName = DateTime.Now.ToString("yyyyMMdd") + "/" + MyAccess.Core.StringTool.GetGUID() + ext;
                    return await client.UploadFile(bytes, fileName, withDomain);
                }
            }

        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<bool> DeleteFile(string path)
        {
            if (!string.IsNullOrEmpty(_option.Value.minio_url) && (path.StartsWith(_option.Value.minio_url) || path.StartsWith("/" + _option.Value.minio_bucket)))
            {
                var client = _provider.GetService<MinioHelper>();
                await client.RemoveFile(path);
                return true;
            }
            else
            {
                try
                {
                    IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
                    if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        path = new Uri(path).AbsolutePath;
                    }
                    string filepath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, path);
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<string> UploadTmpFile(IRequestFile file)
        {
            IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
            string fileExt = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(fileExt))
            {
                fileExt = ContentTypeToExt(file.ContentType);
            }
            string fileName = MyAccess.Core.StringTool.GetGUID() + fileExt;
            string saveurl = string.Format("/tmpfiles/{0}", fileName);
            string filepath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, saveurl);
            string dirPath = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            //保存原文件
            await file.SaveAsAsync(filepath);
            var generalOption = _provider.GetService<IOptions<GeneralOption>>();
            return generalOption.Value.url + saveurl;
        }
        /// <summary>
        /// 每日定时清除临时文件
        /// </summary>
        /// <returns></returns>
        public async Task ClearUpTmpFiles()
        {
            IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
            string dirPath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, "/tmpfiles");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            foreach (var file in Directory.GetFiles(dirPath))
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < DateTime.Now.AddHours(-2))
                {
                    fileInfo.Delete();
                }
            }
        }
        /// <summary>
        /// 通过存储的图片url创建rgb24格式的Image
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Image<Rgb24>> CreateRgb24FromUrl(string url)
        {
            if (string.IsNullOrEmpty(_option.Value.minio_server))
            {
                IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
                string filepath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, url);
                return Image.Load<Rgb24>(filepath);
            }
            else
            {
                string downurl = _option.Value.minio_url + url;
                using (var stream = await HttpHelper.Instance.GetStreamAsync(downurl))
                {
                    return Image.Load<Rgb24>(stream);
                }
            }
        }
        private static byte[] ImageRgb24ToBytes(Image<Rgb24> image)
        {
            int byteCount = image.Width * image.Height * 3;
            byte[] result = new byte[byteCount];

            int currentIndex = 0;
            var pixelMemoryGroup = image.GetPixelMemoryGroup();
            for (int i = 0; i < pixelMemoryGroup.Count; i++)
            {
                var pixelMemory = pixelMemoryGroup[i];
                // 复制当前内存块的字节到结果数组
                var pixelBytes = MemoryMarshal.AsBytes(pixelMemory.Span);
                MemoryMarshal.AsBytes(pixelMemory.Span).CopyTo(result.AsSpan(currentIndex));
                currentIndex += pixelBytes.Length;
            }
            return result;
        }
        public async Task<string> UploadRgb24File(Image<Rgb24> file)
        {
            if (string.IsNullOrEmpty(_option.Value.minio_server))
            {
                IWebHostEnvironment env = _provider.GetService<IWebHostEnvironment>();
                string fileExt = ".jpg";
                string fileName = MyAccess.Core.StringTool.GetGUID() + fileExt;
                DateTime now = DateTime.Now;
                string saveurl = string.Format("/uploads/{0}/{1}/{2}", now.Year, now.ToString("MMdd"), fileName);
                string filepath = TAUtility.RelativeToAbsolutePath(env.WebRootPath, saveurl);
                string dirPath = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                await file.SaveAsJpegAsync(filepath);
                return saveurl;
            }
            else
            {
                var result = ImageRgb24ToBytes(file);
                string ext = ".jpg";
                var client = _provider.GetService<MinioHelper>();
                string fileName = DateTime.Now.ToString("yyyyMMdd") + "/" + MyAccess.Core.StringTool.GetGUID() + ext;
                return await client.UploadFile(result, fileName);
            }

        }
    }

}
