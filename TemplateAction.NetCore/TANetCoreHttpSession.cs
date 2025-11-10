using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpSession : ITASession
    {
        private ISession _session;
        public TANetCoreHttpSession(ISession session)
        {
            _session = session;
        }

        public string SessionId
        {
            get { return _session.Id; }
        }

        public IEnumerable<string> Keys
        {
            get { return _session.Keys; }
        }

        public int? GetInt32(string name)
        {
            return _session.GetInt32(name);
        }

        public T GetObject<T>(string name) where T : class
        {
            return _session.GetObject<T>(name);
        }

        public string GetString(string name)
        {
            return _session.GetString(name);
        }

        public bool Remove(string name)
        {
            _session.Remove(name);
            return true;
        }

        public void SetInt32(string name, int value)
        {
            _session.SetInt32(name, value);
        }

        public void SetObject(string name, object value)
        {
            _session.SetObject(name, value);
        }

        public void SetString(string name, string value)
        {
            _session.SetString(name, value);
        }
    }
}
