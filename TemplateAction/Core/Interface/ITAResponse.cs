using System.IO;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public interface ITAResponse
    {
        int StatusCode { get; set; }
        string StatusDescription { get; set; }
        string ContentType { get; set; }
        void Clear();
        Task WriteAsync(string s);
        Task BinaryWriteAsync(byte[] buffer);
        Stream OutputStream { get; }
        void AppendHeader(string name, string value);
        /// <summary>
        /// 客户端重定向
        /// </summary>
        /// <param name="url"></param>
        void Redirect(string url);
    }
}
