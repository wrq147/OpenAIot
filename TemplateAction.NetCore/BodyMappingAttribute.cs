using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class BodyMappingAttribute : AbstractMappingAttribute
    {
        private DecodeJson _json;
        private DecodeXml _xml;
        public BodyMappingAttribute(DecodeJson json, DecodeXml xml)
        {
            _json = json;
            _xml = xml;
        }
        public BodyMappingAttribute(DecodeJson json) : this(json, BodyParamMapping.DefaultXml) { }
        public BodyMappingAttribute(DecodeXml xml) : this(BodyParamMapping.DefaultJson, xml) { }
        public BodyMappingAttribute() : this(BodyParamMapping.DefaultJson, BodyParamMapping.DefaultXml) { }
        public override Task<object> Mapping(TAAction ac, string key, Type t)
        {
            return BodyParamMapping.BodyMapping(_json, _xml, ac, key, t);
        }
    }
}
