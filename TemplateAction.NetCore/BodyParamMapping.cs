using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TemplateAction.Common;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public delegate object DecodeJson(string json, Type t);
    public delegate object DecodeXml(string xml, Type t);
    public class BodyParamMapping : IParamMapping
    {
        private DecodeJson _jsonFun;
        private DecodeXml _xmlFun;
        public BodyParamMapping(DecodeJson json, DecodeXml xml)
        {
            _jsonFun = json;
            _xmlFun = xml;
        }
        public BodyParamMapping(DecodeJson json) : this(json, DefaultXml) { }
        public BodyParamMapping(DecodeXml xml) : this(DefaultJson, xml) { }
        public BodyParamMapping() : this(DefaultJson, DefaultXml) { }

        internal static DecodeJson DefaultJson = (json, t) =>
        {
            return JsonSerializer.Deserialize(json, t);
        };
        internal static DecodeXml DefaultXml = (xml, t) =>
        {
            StringReader reader = new StringReader(xml);
            XmlSerializer serializer = new XmlSerializer(t);
            object rt = serializer.Deserialize(reader);
            reader.Close();
            return rt;
        };
        internal static async Task<object> BodyMapping(DecodeJson json, DecodeXml xml, TAAction ac, string key, Type t)
        {
            ITARequest req = ac.Context.Request;
            string contenttype = req.Header["content-type"];
            if (contenttype != null)
            {
                string encodingstr = "UTF-8";
                string ctstr = "";
                string[] tarr = contenttype.Split(";");

                if (tarr.Length > 0)
                {
                    ctstr = tarr[0].ToLower();
                    for (int i = 1; i < tarr.Length; i++)
                    {
                        string tmpstr = tarr[i].ToLower();
                        int tjidx = tmpstr.IndexOf("=");
                        string tmpkey = tmpstr.Substring(0, tjidx).Trim();
                        string tmpval = tmpstr.Substring(tjidx + 1, tmpstr.Length - tjidx - 1).Trim();
                        switch (tmpkey)
                        {
                            case "charset":
                                encodingstr = tmpval;
                                break;
                        }
                    }
                }
                else
                {
                    ctstr = contenttype.ToLower();
                }
                ctstr = ctstr.Trim();
                string tkey = "$$JsonParamObject";
                switch (ctstr)
                {
                    case "application/json":
                        {
                            StreamReader sr = new StreamReader(req.InputStream, Encoding.GetEncoding(encodingstr));
                            bool isObject = false;
                            string parseContent = null;
                            if (ac.Context.Items.Contains(tkey) || ac.ActionNode.Method.GetParameters().Length > 1)
                            {
                                isObject = true;
                            }
                            else
                            {
                                if (typeof(string).Equals(t) || t.IsValueType)
                                {
                                    isObject = true;
                                }
                                else if (t.IsArray || t.IsCollectible)
                                {
                                    parseContent = await sr.ReadToEndAsync();
                                    if (parseContent.TrimStart().StartsWith('{'))
                                    {
                                        isObject = true;
                                    }
                                }
                            }
                            if (isObject)
                            {
                                var sourceObj = ac.Context.Items[tkey] as IDictionary<string, JsonElement>;
                                if (sourceObj == null)
                                {
                                    if (parseContent == null)
                                    {
                                        parseContent = await sr.ReadToEndAsync();
                                    }
                                    if (string.IsNullOrEmpty(parseContent))
                                    {
                                        sourceObj = new Dictionary<string, JsonElement>();
                                    }
                                    else
                                    {
                                        sourceObj = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(parseContent);
                                    }
                                    ac.Context.Items[tkey] = sourceObj;
                                }
                                if (sourceObj != null)
                                {
                                    JsonElement outval;
                                    if (sourceObj.TryGetValue(key, out outval))
                                    {
                                        switch (outval.ValueKind)
                                        {
                                            case JsonValueKind.String:
                                                return outval.GetString();
                                            case JsonValueKind.Number:
                                            case JsonValueKind.True:
                                            case JsonValueKind.False:
                                                return TAConverter.Instance.Convert(outval.GetRawText(), t);
                                            default:
                                                return json(outval.GetRawText(), t);
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (parseContent == null)
                                {
                                    parseContent = await sr.ReadToEndAsync();
                                }
                                return json(parseContent, t);
                            }
                            break;
                        }
                    case "application/xml":
                        {
                            StreamReader sr = new StreamReader(req.InputStream, Encoding.GetEncoding(encodingstr));
                            if (typeof(string).Equals(t) || t.IsValueType || ac.Context.Items.Contains(tkey) || ac.ActionNode.Method.GetParameters().Length > 1)
                            {
                                var sourceObj = ac.Context.Items[tkey] as IDictionary<string, XmlElement>;
                                if (sourceObj == null)
                                {
                                    sourceObj = (IDictionary<string, XmlElement>)DefaultXml(await sr.ReadToEndAsync(), typeof(Dictionary<string, XmlElement>));
                                    ac.Context.Items[tkey] = sourceObj;
                                }
                                if (sourceObj != null)
                                {
                                    XmlElement outval;
                                    if (sourceObj.TryGetValue(key, out outval))
                                    {
                                        return xml(outval.InnerXml, t);
                                    }
                                }

                            }
                            else
                            {
                                return xml(await sr.ReadToEndAsync(), t);
                            }
                            break;
                        }
                }
            }
            return DBNull.Value;
        }
        public async Task<object> Mapping(LinkedListNode<IParamMapping> next, TAAction ac, string key, Type t)
        {
            object rt = await BodyMapping(_jsonFun, _xmlFun, ac, key, t);
            if (rt != DBNull.Value)
            {
                return rt;
            }
            if (next == null)
            {
                return DBNull.Value;
            }
            return await next.Value.Mapping(next.Next, ac, key, t);
        }
    }
}
