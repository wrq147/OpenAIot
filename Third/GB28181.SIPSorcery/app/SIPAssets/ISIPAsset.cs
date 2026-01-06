using System;
using System.Collections.Generic;
using System.Xml;

namespace GB28181.App
{
    public interface ISIPAsset
    {
        Guid Id { get; set; }
        string ToXML();
        string ToXMLNoParent();
        string GetXMLElementName();
        string GetXMLDocumentElementName();
    }
}
