using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    public interface ITARequest
    {
        bool HasFormContentType { get; }
        /// <summary>
        /// 异步获取form
        /// </summary>
        /// <returns></returns>
        Task<ITAFormCollection> ReadFormAsync();
        ITAObjectCollection Query { get;}
        NameValueCollection Header { get; }
        IPAddress ServerIP { get; }
        int ServerPort { get; }
        IPAddress ClientIP { get; }

        string Path { get; }
        string HttpMethod { get; }
        string UserAgent { get; }

        Stream InputStream { get; }
        Uri Url { get; }
        Uri UrlReferrer { get; }
        string ContentType { get; }
    }
}
