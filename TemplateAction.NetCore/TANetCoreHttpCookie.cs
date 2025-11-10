using Microsoft.AspNetCore.Http;
using System;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpCookie : ITACookie
    {
        private HttpContext _context;
        private string _name;
        private string _value;
        private DateTime _expires;
        private bool _secure;
        private bool _httpOnly;
        private string _path;
        private string _domain;
        /// <summary>
        /// 加密密钥
        /// </summary>
        private string _encodeKey;
        public TANetCoreHttpCookie(HttpContext context, string name, string encodeKey = null)
        {
            _context = context;
            if (!context.Request.Cookies.TryGetValue(name, out _value))
            {
                _value = null;
            }
            _name = name;
            _encodeKey = encodeKey;
            _httpOnly = false;
            _path = "/";
            _secure = false;
            _expires = new DateTime(0);
        }

        public DateTime Expires
        {
            get
            {
                return _expires;
            }
            set
            {
                _expires = value;
            }
        }

        public bool Secure
        {
            get
            {
                return _secure;
            }
            set
            {
                _secure = value;
            }
        }
        public bool HttpOnly
        {
            get { return _httpOnly; }
            set { _httpOnly = value; }
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string GetValue()
        {
            if (string.IsNullOrEmpty(_encodeKey))
            {
                return _value;
            }
            else
            {
                return TANetCoreCripter.Decrypt(_value, _encodeKey);
            }
        }


        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(_value);
        }


        public void SetValue(string value)
        {
            if (string.IsNullOrEmpty(_encodeKey))
            {
                _value = value;
            }
            else
            {
                _value = TANetCoreCripter.Encrypt(value, _encodeKey);
            }
        }
        public void SaveCookie()
        {
            CookieOptions ckop = new CookieOptions();
            if (_domain != null)
            {
                ckop.Domain = _domain;
            }
            ckop.HttpOnly = _httpOnly;
            ckop.Path = _path;
            ckop.Secure = _secure;
            if (_expires.Ticks != 0)
            {
                ckop.Expires = _expires;
            }
            _context.Response.Cookies.Append(_name, _value, ckop);
        }
    }
}
