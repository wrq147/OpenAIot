using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TemplateAction.NetCore
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TANetWebApiResponseAttribute : Attribute
    {
        public TANetWebApiResponseAttribute(Type type)
        {
            this.Type = type;
        }

        public TANetWebApiResponseAttribute(Type type, HttpStatusCode statusCode = HttpStatusCode.OK, string description = null)
            : this(type)
        {
            Description = description;
            this.StatusCode = (int)statusCode;
        }


        public int StatusCode { get; private set; }

        public string Description { get; set; }

        public Type Type { get; set; }
    }
}
