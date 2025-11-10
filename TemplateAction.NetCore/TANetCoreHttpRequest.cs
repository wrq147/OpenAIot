using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpRequest : ITARequest
    {
        private HttpRequest _request;
        private ITAObjectCollection _query;
        private ITAFormCollection _form;
        private NameValueCollection _header;
        private Uri _url;
        private Uri _urlReferrer;
        private IPAddress _serverip;
        private IPAddress _clientip;
        public TANetCoreHttpRequest(HttpContext context)
        {
            _request = context.Request;
            _header = new TANetCoreHttpHeader(_request.Headers);
            _query = new TANetCoreHttpQueryCollection(_request.Query);
        }
        public bool HasFormContentType
        {
            get { return _request.HasFormContentType; }
        }
        /// <summary>
        /// 异步读取form
        /// </summary>
        /// <returns></returns>
        public async Task<ITAFormCollection> ReadFormAsync()
        {
            if (_form == null)
            {
                if (_request.HasFormContentType)
                {
                    IFormCollection fc = await _request.ReadFormAsync();
                    int filecount = _request.Form.Files.Count;
                    TANetCoreHttpFile[] requestFiles = new TANetCoreHttpFile[filecount];
                    for (int i = 0; i < filecount; i++)
                    {
                        requestFiles[i] = new TANetCoreHttpFile(_request.Form.Files[i]);
                    }
                    _form = new TANetCoreHttpFormCollection(fc, requestFiles);
                }
                return _form;
            }
            else
            {
                return _form;
            }
        }
        public ITAObjectCollection Query
        {
            get { return _query; }
        }

        public NameValueCollection Header
        {
            get { return _header; }
        }

        public IPAddress ServerIP
        {
            get
            {
                if (_serverip == null)
                {
                    _serverip = _request.HttpContext.Connection.LocalIpAddress;
                }
                return _serverip;
            }

        }

        public int ServerPort
        {
            get { return _request.HttpContext.Connection.LocalPort; }
        }

        public IPAddress ClientIP
        {
            get
            {
                if (_clientip == null)
                {
                    _clientip = _request.HttpContext.Connection.RemoteIpAddress;
                }
                return _clientip;
            }
        }
        public Uri Url
        {
            get
            {
                if (_url == null)
                {
                    _url = new Uri(string.Format("{0}://{1}{2}{3}", _request.Scheme, _request.Host, _request.Path, _request.QueryString));
                }
                return _url;
            }
        }

        public Uri UrlReferrer
        {
            get
            {
                if (_urlReferrer == null)
                {
                    _urlReferrer = new Uri(_request.Headers["Referer"].ToString());
                }
                return _urlReferrer;
            }
        }

        public string Path
        {
            get { return _request.Path; }
        }

        public string HttpMethod
        {
            get { return _request.Method; }
        }

        public string UserAgent
        {
            get { return _request.Headers["User-Agent"].ToString(); }
        }

        public Stream InputStream
        {
            get { return _request.Body; }
        }
        public string ContentType
        {
            get { return _request.ContentType; }
        }
    }
}
