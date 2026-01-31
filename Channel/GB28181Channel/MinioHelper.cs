using Microsoft.Extensions.Options;
using Minio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB28181Channel
{
    public class MinioHelper
    {
        private IMinioClient _minioClient;
        private IOptions<GB28181Option> _conf;
        public MinioHelper(IOptions<GB28181Option> conf)
        {
            _conf = conf;
            _minioClient = new MinioClient().WithEndpoint(conf.Value.minio_server)
                    .WithCredentials(conf.Value.minio_access, conf.Value.minio_secret)
            .Build();
        }
        public async void UploadFile(string path, string fileName)
        {
            using (FileStream readStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                await this.UploadFile(readStream, fileName);
            }
            File.Delete(path);
        }
        private async Task UploadFile(Stream stream, string fileName)
        {
            //上传文件
            try
            {
                stream.Position = 0;
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(_conf.Value.minio_bucket)
                    .WithObject(fileName)
                    .WithObjectSize(stream.Length)
                    .WithStreamData(stream);
                await _minioClient.PutObjectAsync(putObjectArgs);
            }
            catch (Exception e)
            {
                Console.WriteLine("File Upload Error: {0}", e.Message);
            }
        }

        public async Task RemoveFile(string fileName)
        {
            var removeObjectArgs = new RemoveObjectArgs()
       .WithBucket(_conf.Value.minio_bucket)
       .WithObject(fileName);
            await _minioClient.RemoveObjectAsync(removeObjectArgs);
        }
    }
}
