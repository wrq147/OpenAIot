// ============================================================================
// FileName: SIPAccount.cs
//
// Description:
// Represents a SIP account that holds authentication information and additional settings
// for SIP accounts.
//
// Author(s):
// Aaron Clauson
//
// History:
// 10 May 2008  Aaron Clauson   Created.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using GB28181.Net.RTP;
using GB28181.Sys;
using Microsoft.Extensions.Logging;
using SIPSorcery.Sys;

namespace GB28181.App
{
    /// <remarks>
    /// SIP account usernames can be treated by some SIP Sorcery server agents as domain name like structures where a username of
    /// "x.username" will match the "username" account for receiving calls. To facilitate this SIP accounts with a '.' character in them
    /// can only be created where the suffix "username" portion matches the Owner field. This allows users to create SIP accounts with '.'
    /// in them but will prevent a different user from being able to hijack an "x.username" account and caue unexpected behaviour.
    /// </remarks>
   // [Table(Name = "sipaccounts")]
    [DataContract]
    public class SIPAccount : INotifyPropertyChanged, ISIPAsset
    {
        public const string XML_DOCUMENT_ELEMENT_NAME = "sipaccounts";
        public const string XML_ELEMENT_NAME = "sipaccount";
        public const int PASSWORD_MIN_LENGTH = 6;
        public const int PASSWORD_MAX_LENGTH = 15;
        public const int USERNAME_MIN_LENGTH = 5;
        private const string BANNED_SIPACCOUNT_NAMES = "dispatcher";

        //public static readonly string SelectQuery = "select * from sipaccounts where sipusername = ?1 and sipdomain = ?2";
        // Only non-printable non-alphanumeric ASCII characters missing are ; \ and space. The semi-colon isn't accepted by 
        // Netgears and the space has the potential to create too much confusion with the users and \ with the system.
        public static readonly char[] NONAPLPHANUM_ALLOWED_PASSWORD_CHARS = new char[] { '!', '"', '$', '%', '&', '(', ')', '*', '+', ',', '.', '/', ':', '<', '=', '>', '?', '@', '[', ']', '^', '_', '`', '{', '|', '}', '~' };
        public static readonly string USERNAME_ALLOWED_CHARS = @"a-zA-Z0-9_\-\.";

        private static ILogger logger = AppState.logger;
        private static string m_newLine = AppState.NewLine;

        public static int TimeZoneOffsetMinutes;
        //  // [Column(Name = "id", DbType = "varchar(36)", IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public Guid Id { get; set; }

        private string m_owner;                 // The username of the account that owns this SIP account.
                                                //   // [Column(Name = "owner", DbType = "varchar(32)", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string Owner
        {
            get { return m_owner; }
            set
            {
                m_owner = value;
                NotifyPropertyChanged("Owner");
            }
        }

        #region 国标属性
        private IPAddress m_localIP;
        private ushort m_localPort;
        private IPAddress mediaIP;//流媒体IP
        private ushort mediaPort;////流媒体port
        private string m_sipUsername;
        private string m_sipPassword;
        private ProtocolType _msgProtocol;
        private ProtocolType _streamProtocol;
        private TcpConnectMode _tcpMode;
        private IPAddress m_OutputIP;

        /// <summary>
        /// GB Version
        /// </summary>
        public string GbVersion { get; set; }

        /// <summary>
        /// 本地国标编码
        /// </summary>
        public string LocalID { get; set; }

        /// <summary>
        /// 本地IP地址
        /// </summary>
        public IPAddress LocalIP
        {
            get { return m_localIP; }
            set { m_localIP = value; }
        }

        /// <summary>
        /// 本地端口号
        /// </summary>
        public ushort LocalPort
        {
            get { return m_localPort; }
            set { m_localPort = value; }
        }

        /// <summary>
        /// 流媒体IP
        /// </summary>
        public IPAddress MediaIP
        {
            get { return mediaIP; }
            set { mediaIP = value; }
        }


        /// <summary>
        /// 流媒体port
        /// </summary>
        public ushort MediaPort
        {
            get { return mediaPort; }
            set { mediaPort = value; }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public string SIPUsername
        {
            get { return m_sipUsername; }
            set
            {
                m_sipUsername = value;
                NotifyPropertyChanged("SIPUsername");
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string SIPPassword
        {
            get { return m_sipPassword; }
            set
            {
                m_sipPassword = value;
                NotifyPropertyChanged("SIPPassword");
            }
        }

        /// <summary>
        /// 消息信令协议(TCP/UDP)
        /// </summary>
        public ProtocolType MsgProtocol
        {
            get { return _msgProtocol; }
            set { _msgProtocol = value; }
        }

        /// <summary>
        /// 媒体流协议(TCP/UDP)
        /// </summary>
        public ProtocolType StreamProtocol
        {
            get { return _streamProtocol; }
            set { _streamProtocol = value; }
        }

        /// <summary>
        /// tcp模式(active/passive)
        /// </summary>
        public TcpConnectMode TcpMode
        {
            get { return _tcpMode; }
            set { _tcpMode = value; }
        }

        /// <summary>
        /// 消息信令字符编码
        /// </summary>
        public string MsgEncode { get; set; }


        /// <summary>
        /// 对外输出IP地址
        /// </summary>
        public IPAddress OutputIP { get => m_OutputIP; set => m_OutputIP = value; }

        #endregion


        // // [Column(Name = "adminmemberid", DbType = "varchar(32)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string AdminMemberId { get; set; }    // If set it designates this asset as a belonging to a user with the matching adminid.

        private string m_sipDomain;
        // // [Column(Name = "sipdomain", DbType = "varchar(128)", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string SIPDomain
        {
            get { return m_sipDomain; }
            set
            {
                m_sipDomain = value;
                NotifyPropertyChanged("SIPDomain");
            }
        }

        private bool m_sendNATKeepAlives;
        //  // [Column(Name = "sendnatkeepalives", DbType = "bit", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public bool SendNATKeepAlives
        {
            get { return m_sendNATKeepAlives; }
            set
            {
                m_sendNATKeepAlives = value;
                NotifyPropertyChanged("SendNATKeepAlives");
            }
        }

        private bool m_isIncomingOnly;          // For SIP accounts that can only be used to receive incoming calls.
                                                //  // [Column(Name = "isincomingonly", DbType = "bit", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public bool IsIncomingOnly
        {
            get { return m_isIncomingOnly; }
            set
            {
                m_isIncomingOnly = value;
                NotifyPropertyChanged("IsIncomingOnly");
            }
        }

        private string m_outDialPlanName;       // The dialplan that will be used for outgoing calls.
                                                // // [Column(Name = "outdialplanname", DbType = "varchar(64)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string OutDialPlanName
        {
            get { return m_outDialPlanName; }
            set
            {
                m_outDialPlanName = value;
                NotifyPropertyChanged("OutDialPlanName");
            }
        }

        private string m_inDialPlanName;        // The dialplan that will be used for incoming calls. If this field is empty incoming calls will be forwarded to the account's current bindings.
                                                // // [Column(Name = "indialplanname", DbType = "varchar(64)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string InDialPlanName
        {
            get { return m_inDialPlanName; }
            set
            {
                m_inDialPlanName = value;
                NotifyPropertyChanged("InDialPlanName");
            }
        }

        private bool m_isUserDisabled;              // Allows owning user disabling of accounts.
                                                    // // [Column(Name = "isuserdisabled", DbType = "bit", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public bool IsUserDisabled
        {
            get { return m_isUserDisabled; }
            set
            {
                m_isUserDisabled = value;
                NotifyPropertyChanged("IsUserDisabled");
                NotifyPropertyChanged("IsDisabled");
            }
        }

        private bool m_isAdminDisabled;              // Allows administrative disabling of accounts.
                                                     // // [Column(Name = "isadmindisabled", DbType = "bit", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public bool IsAdminDisabled
        {
            get { return m_isAdminDisabled; }
            set
            {
                m_isAdminDisabled = value;
                NotifyPropertyChanged("IsAdminDisabled");
                NotifyPropertyChanged("IsDisabled");
            }
        }

        private string m_adminDisabledReason;
        // // [Column(Name = "admindisabledreason", DbType = "varchar(256)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string AdminDisabledReason
        {
            get { return m_adminDisabledReason; }
            set
            {
                m_adminDisabledReason = value;
                NotifyPropertyChanged("AdminDisabledReason");
            }
        }

        private string m_networkId;                 // SIP accounts with the ame network id will not have their Contact headers or SDP mangled for private IP address.
                                                    // // [Column(Name = "networkid", DbType = "varchar(16)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string NetworkId
        {
            get { return m_networkId; }
            set
            {
                m_networkId = value;
                NotifyPropertyChanged("NetworkId");
            }
        }

        private string m_ipAddressACL;              // A regular expression that acts as an IP address Access Control List for SIP request authorisation.
                                                    // // [Column(Name = "ipaddressacl", DbType = "varchar(256)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string IPAddressACL
        {
            get { return m_ipAddressACL; }
            set
            {
                m_ipAddressACL = value;
                NotifyPropertyChanged("IPAddressACL");
            }
        }

        private DateTimeOffset m_inserted;
        //  // [Column(Name = "inserted", DbType = "datetimeoffset", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public DateTimeOffset Inserted
        {
            get { return m_inserted; }
            set { m_inserted = value.ToUniversalTime(); }
        }

        private bool m_isSwitchboardEnabled = true;
        //// [Column(Name = "isswitchboardenabled", DbType = "bit", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public bool IsSwitchboardEnabled
        {
            get { return m_isSwitchboardEnabled; }
            set
            {
                m_isSwitchboardEnabled = value;
                NotifyPropertyChanged("IsSwitchboardEnabled");
            }
        }

        private bool m_dontMangleEnabled = false;
        // // [Column(Name = "dontmangleenabled", DbType = "bit", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public bool DontMangleEnabled
        {
            get { return m_dontMangleEnabled; }
            set
            {
                m_dontMangleEnabled = value;
                NotifyPropertyChanged("DontMangleEnabled");
            }
        }

        private string m_avatarURL;
        //  // [Column(Name = "avatarurl", DbType = "varchar(1024)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string AvatarURL
        {
            get { return m_avatarURL; }
            set
            {
                m_avatarURL = value;
                NotifyPropertyChanged("AvatarURL");
            }
        }

        private string m_accountCode;
        // // [Column(Name = "accountcode", DbType = "varchar(36)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string AccountCode
        {
            get { return m_accountCode; }
            set
            {
                m_accountCode = value;
                NotifyPropertyChanged("AccountCode");
            }
        }

        private string m_description;
        // // [Column(Name = "description", DbType = "varchar(1024)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string Description
        {
            get { return m_description; }
            set
            {
                m_description = value;
                NotifyPropertyChanged("Description");
            }
        }

        public DateTimeOffset InsertedLocal
        {
            get { return Inserted.AddMinutes(TimeZoneOffsetMinutes); }
        }

        public bool IsDisabled
        {
            get { return m_isUserDisabled || m_isAdminDisabled; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SIPAccount() { }

        public string ToXML()
        {
            string sipAccountXML =
                " <" + XML_ELEMENT_NAME + ">" + m_newLine +
               ToXMLNoParent() + m_newLine +
                " </" + XML_ELEMENT_NAME + ">" + m_newLine;

            return sipAccountXML;
        }

        public string ToXMLNoParent()
        {
            string sipAccountXML =
                "  <id>" + Id + "</id>" + m_newLine +
                "  <owner>" + m_owner + "</owner>" + m_newLine +
                "  <sipusername>" + m_sipUsername + "</sipusername>" + m_newLine +
                "  <sippassword>" + m_sipPassword + "</sippassword>" + m_newLine +
                "  <sipdomain>" + m_sipDomain + "</sipdomain>" + m_newLine +
                "  <sendnatkeepalives>" + m_sendNATKeepAlives + "</sendnatkeepalives>" + m_newLine +
                "  <isincomingonly>" + m_isIncomingOnly + "</isincomingonly>" + m_newLine +
                "  <outdialplanname>" + m_outDialPlanName + "</outdialplanname>" + m_newLine +
                "  <indialplanname>" + m_inDialPlanName + "</indialplanname>" + m_newLine +
                "  <isuserdisabled>" + m_isUserDisabled + "</isuserdisabled>" + m_newLine +
                "  <isadmindisabled>" + m_isAdminDisabled + "</isadmindisabled>" + m_newLine +
                "  <disabledreason>" + m_adminDisabledReason + "</disabledreason>" + m_newLine +
                "  <networkid>" + m_networkId + "</networkid>" + m_newLine +
                "  <ipaddressacl>" + SafeXML.MakeSafeXML(m_ipAddressACL) + "</ipaddressacl>" + m_newLine +
                "  <inserted>" + m_inserted.ToString("o") + "</inserted>" + m_newLine +
                "  <isswitchboardenabled>" + m_isSwitchboardEnabled + "</isswitchboardenabled>" + m_newLine +
                "  <dontmangleenabled>" + m_dontMangleEnabled + "</dontmangleenabled>" + m_newLine +
                "  <avatarurl>" + m_avatarURL + "</avatarurl>";

            return sipAccountXML;
        }

        public string GetXMLElementName()
        {
            return XML_ELEMENT_NAME;
        }

        public string GetXMLDocumentElementName()
        {
            return XML_DOCUMENT_ELEMENT_NAME;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}