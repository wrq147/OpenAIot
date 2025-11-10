using Common.Share;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Common
{
    public class MinioHelper
    {
        private IMinioClient _minioClient;
        private IOptions<GeneralOption> _conf;
        public MinioHelper(IOptions<GeneralOption> conf)
        {
            _conf = conf;
            _minioClient = new MinioClient().WithEndpoint(conf.Value.minio_server)
                    .WithCredentials(conf.Value.minio_access, conf.Value.minio_secret)
            .Build();
        }

        public async Task<string> UploadFile(byte[] bytes, string fileName, bool withDomin = false)
        {
            //上传文件
            try
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    stream.Position = 0;
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(_conf.Value.minio_bucket)
                        .WithObject(fileName)
                        .WithObjectSize(stream.Length)
                        .WithStreamData(stream);
                    await _minioClient.PutObjectAsync(putObjectArgs);
                    if (withDomin)
                    {
                        return _conf.Value.minio_url + "/" + _conf.Value.minio_bucket + "/" + fileName;
                    }
                    else
                    {
                        return "/" + _conf.Value.minio_bucket + "/" + fileName;
                    }
                }
            }
            catch (MinioException e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
                return string.Empty;
            }
        }

        public async Task RemoveFile(string url)
        {
            string tmpurl = url;
            if (tmpurl.StartsWith(_conf.Value.minio_url))
            {
                tmpurl = tmpurl.Substring(0, _conf.Value.minio_url.Length);
            }
            string fileName = tmpurl.Substring(0, _conf.Value.minio_bucket.Length + 2);
            var removeObjectArgs = new RemoveObjectArgs()
       .WithBucket(_conf.Value.minio_bucket)
       .WithObject(fileName);
            await _minioClient.RemoveObjectAsync(removeObjectArgs);
        }
    }
}
