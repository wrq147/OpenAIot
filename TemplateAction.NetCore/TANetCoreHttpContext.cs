using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpContext : ITAContext
    {
        private TANetCoreHttpApplication _app;
        private ITASession _tasession;
        private ITARequest _request;
        private ITAResponse _response;
        private HttpContext _context;
        private IDictionary _dicItems;
        public TANetCoreHttpContext(HttpContext context)
        {
            _app = context.Features.Get<TANetCoreHttpApplication>();
            ISessionFeature sessionFeature = context.Features.Get<ISessionFeature>();
            if (sessionFeature != null)
            {
                //创建Session
                _tasession = new TANetCoreHttpSession(context.Session);
            }
            _context = context;
            _request = new TANetCoreHttpRequest(context);
            _response = new TANetCoreHttpResponse(context);
            _dicItems = new Hashtable();
        }
        public TASiteApplication Application
        {
            get { return _app; }
        }

        public string Version
        {
            get { return string.Empty; }
        }

        public ITARequest Request { get { return _request; } }

        public ITAResponse Response { get { return _response; } }

        public IDictionary Items
        {
            get { return _dicItems; }
        }

        public ITASession Session
        {
            get { return _tasession; }
        }

        public string MapPath(string path)
        {
            IWebHostEnvironment env = _context.RequestServices.GetService<IWebHostEnvironment>();
            return TAUtility.RelativeToAbsolutePath(env.WebRootPath, path);
        }

        public string UrlDecode(string str, Encoding encoding)
        {
            return HttpUtility.UrlDecode(str.Replace("+", "%2b"), encoding);
        }

        public ITACookie CreateCookie(string name)
        {
            return new TANetCoreHttpCookie(_context, name);
        }

        public ITACookie CreateCookie(string name, string encodekey)
        {
            return new TANetCoreHttpCookie(_context, name, encodekey);
        }

        public bool ExistCookie(string name)
        {
            return _context.Request.Cookies.ContainsKey(name);
        }
        public void SaveCookie(ITACookie cookie)
        {
            ((TANetCoreHttpCookie)cookie).SaveCookie();
        }

    }
}
