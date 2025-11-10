using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpResponse : ITAResponse
    {
        private HttpResponse _response;
        public TANetCoreHttpResponse(HttpContext context)
        {
            _response = context.Response;
        }
        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }

        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public Stream OutputStream
        {
            get { return _response.Body; }
        }

        public string StatusDescription
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public void AppendHeader(string name, string value)
        {
            _response.Headers[name] = value;
        }

        public async Task BinaryWriteAsync(byte[] buffer)
        {
            await _response.WriteAsync(buffer);
        }
        public void Clear()
        {
            _response.Clear();
        }

        public void Redirect(string url)
        {
            _response.Redirect(url);
        }
        public async Task WriteAsync(string s)
        {
            await _response.WriteAsync(s);
        }
    }
}
