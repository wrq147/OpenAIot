// ============================================================================
// FileName: SIPDomain.cs
//
// Description:
// Maintains a list of domains and domain aliases that can be used by various
// SIP Server agents. For example allows a SIP Registrar or Proxy to check the 
// domain on an incoming request to see if it is serviced at this location.
//
// Author(s):
// Aaron Clauson
//
// History:
// 27 Jul 2008	Aaron Clauson	Created.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//


using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GB28181.Sys;
using System.Linq;
using System.Xml;
using SIPSorcery.Sys;
using Microsoft.Extensions.Logging;


#if !SILVERLIGHT
using System.Data;
#endif

namespace GB28181.App
{
    /// <remarks>
    /// The mechanism to load the SIP domain records differs for XML and SQL data stores. Because the domain and domain alias
    /// are hierarchical the XML stores the records in a nested node structure. With SQL the nested structure is not possible 
    /// and instead a flat table is used where the domain alias are stored as a delimited list in a single column field.
    /// </remarks>
    [DataContract]
    // // // [Column(Name = "sipdomains")]
    public class SIPDomain : ISIPAsset
    {
        private const char ALIAS_SEPERATOR_CHAR = ';';
        public const string XML_DOCUMENT_ELEMENT_NAME = "sipdomains";
        public const string XML_ELEMENT_NAME = "sipdomain";

        private static string m_newLine = AppState.NewLine;

        [DataMember]
        // // [Column(Name = "id", DbType = "varchar(36)", IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public Guid Id { get; set; }

        [DataMember]
        // // [Column(Name = "domain", DbType = "varchar(128)", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Domain { get; set; }

        [DataMember]
        // // [Column(Name = "owner", DbType = "varchar(32)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string Owner { get; set; }

        private DateTimeOffset? m_inserted;
        // // [Column(Name = "inserted", DbType = "datetimeoffset", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTimeOffset? Inserted
        {
            get { return m_inserted; }
            set
            {
                if (value != null)
                {
                    m_inserted = value.Value.ToUniversalTime();
                }
                else
                {
                    m_inserted = null;
                }
            }
        }

        // // [Column(Name = "aliaslist", DbType = "varchar(1024)", CanBeNull = true)]
        public string AliasList
        {
            get
            {
                if (Aliases != null && Aliases.Count > 0)
                {
                    string aliasList = null;

                    // Aliases.ForEach(item => aliasList = aliasList + item + ALIAS_SEPERATOR_CHAR);

                    aliasList = Aliases.Aggregate((stringist, item) => stringist + item + ALIAS_SEPERATOR_CHAR);

                    return aliasList;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Aliases = ParseAliases(value);
            }
        }

        public List<string> Aliases = new List<string>();

        public SIPDomain()
        { }

        public SIPDomain(string domain, string owner, List<string> aliases)
        {
            Id = Guid.NewGuid();
            Domain = domain;
            Owner = owner;
            Aliases = aliases;
            Inserted = DateTime.UtcNow;
        }

        public string ToXML()
        {
            string sipDomainXML =
                "  <" + XML_ELEMENT_NAME + ">" + m_newLine +
               ToXMLNoParent() + m_newLine +
                "  </" + XML_ELEMENT_NAME + ">" + m_newLine;

            return sipDomainXML;
        }

        public string ToXMLNoParent()
        {
            throw new NotImplementedException();
        }

        public string GetXMLElementName()
        {
            return XML_ELEMENT_NAME;
        }

        public string GetXMLDocumentElementName()
        {
            return XML_DOCUMENT_ELEMENT_NAME;
        }

        private List<string> ParseAliases(string aliasString)
        {
            if (aliasString.IsNullOrBlank()) return null;

            var aliases = aliasString.Split(ALIAS_SEPERATOR_CHAR);
            if (aliases == null || aliases.Length <= 0) return null;

            return aliases.Where(alias => !alias.IsNullOrBlank())
                     .Aggregate(new List<string>(), (resultList, alias) =>
              {
                  if (!resultList.Contains(alias.ToLower()))
                  {
                      resultList.Add(alias);
                  }
                  return resultList;
              });

        }

        public Dictionary<Guid, object> Load(XmlDocument dom)
        {
            throw new NotImplementedException();
        }
    }
}
