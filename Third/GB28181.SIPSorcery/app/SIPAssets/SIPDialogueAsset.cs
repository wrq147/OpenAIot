using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using GB28181.Sys;
using SIPSorcery.SIP;
using SIPSorcery.Sys;

namespace GB28181.App
{
    // // [Table(Name = "sipdialogues")]
    public class SIPDialogueAsset : ISIPAsset
    {
        public const string XML_DOCUMENT_ELEMENT_NAME = "sipdialogues";
        public const string XML_ELEMENT_NAME = "sipdialogue";

        private static string m_newLine = AppState.NewLine;

        [IgnoreDataMember]
        public SIPDialogue SIPDialogue;

        // // [Column(Name = "id", DbType = "varchar(36)", IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public Guid Id
        {
            get { return SIPDialogue.Id; }
            set { SIPDialogue.Id = value; }
        }

        // // [Column(Name = "owner", DbType = "varchar(32)", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Owner
        {
            get { return SIPDialogue.Owner; }
            set { SIPDialogue.Owner = value; }
        }

        // // [Column(Name = "adminmemberid", DbType = "varchar(32)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string AdminMemberId
        {
            get { return SIPDialogue.AdminMemberId; }
            set { SIPDialogue.AdminMemberId = value; }
        }

        //// // [Column(Name = "dialogueid", DbType = "varchar(256)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        //public string DialogueId {
        //   get { return SIPDialogue.DialogueId; }
        //    set { SIPDialogue.DialogueId = value; }
        //}

        // // [Column(Name = "localtag", DbType = "varchar(64)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string LocalTag
        {
            get { return SIPDialogue.LocalTag; }
            set { SIPDialogue.LocalTag = value; }
        }

        // // [Column(Name = "remotetag", DbType = "varchar(64)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string RemoteTag
        {
            get { return SIPDialogue.RemoteTag; }
            set { SIPDialogue.RemoteTag = value; }
        }

        // // [Column(Name = "callid", DbType = "varchar(128)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string CallId
        {
            get { return SIPDialogue.CallId; }
            set { SIPDialogue.CallId = value; }
        }

        // // [Column(Name = "cseq", DbType = "int", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public int CSeq
        {
            get { return SIPDialogue.CSeq; }
            set { SIPDialogue.CSeq = value; }
        }

        // // [Column(Name = "bridgeid", DbType = "varchar(36)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string BridgeId
        {
            get { return SIPDialogue.BridgeId.ToString(); }
            set { SIPDialogue.BridgeId = (!value.IsNullOrBlank()) ? new Guid(value) : Guid.Empty; }
        }

        // // [Column(Name = "remotetarget", DbType = "varchar(256)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string RemoteTarget
        {
            get { return SIPDialogue.RemoteTarget.ToString(); }
            set { SIPDialogue.RemoteTarget = (!value.IsNullOrBlank()) ? SIPURI.ParseSIPURI(value) : null; }
        }

        [IgnoreDataMember]
        // // [Column(Name = "localuserfield", DbType = "varchar(512)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string LocalUserField
        {
            get { return SIPDialogue.LocalUserField.ToString(); }
            set { SIPDialogue.LocalUserField = (!value.IsNullOrBlank()) ? SIPUserField.ParseSIPUserField(value) : null; }
        }

        [DataMember]
        // // [Column(Name = "remoteuserfield", DbType = "varchar(512)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string RemoteUserField
        {
            get { return SIPDialogue.RemoteUserField.ToString(); }
            set { SIPDialogue.RemoteUserField = (!value.IsNullOrBlank()) ? SIPUserField.ParseSIPUserField(value) : null; }
        }

        // // [Column(Name = "proxysipsocket", DbType = "varchar(64)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string ProxySIPSocket
        {
            get { return SIPDialogue.ProxySendFrom; }
            set { SIPDialogue.ProxySendFrom = value; }
        }

        [IgnoreDataMember]
        // // [Column(Name = "routeset", DbType = "varchar(512)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string RouteSet
        {
            get { return (SIPDialogue.RouteSet != null) ? SIPDialogue.RouteSet.ToString() : null; }
            set { SIPDialogue.RouteSet = (!value.IsNullOrBlank()) ? SIPRouteSet.ParseSIPRouteSet(value) : null; }
        }

        // // [Column(Name = "cdrid", DbType = "varchar(36)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string CDRId
        {
            get { return SIPDialogue.CDRId.ToString(); }
            set { SIPDialogue.CDRId = (!value.IsNullOrBlank()) ? new Guid(value) : Guid.Empty; }
        }

        [DataMember]
        // // [Column(Name = "calldurationlimit", DbType = "int", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public int CallDurationLimit
        {
            get { return SIPDialogue.CallDurationLimit; }
            set { SIPDialogue.CallDurationLimit = value; }
        }

        [DataMember]
        // // [Column(Name = "inserted", DbType = "datetimeoffset", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public DateTimeOffset Inserted
        {
            get { return SIPDialogue.Inserted; }
            set { SIPDialogue.Inserted = value.DateTime; }
        }

        [DataMember]
        // // [Column(Name = "hangupat", DbType = "datetimeoffset", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public DateTimeOffset? HangupAt
        {
            get
            {
                if (CallDurationLimit != 0)
                {
                    return Inserted.AddSeconds(CallDurationLimit);
                }
                else
                {
                    return null;
                }
            }
            set { }     // The hangup time is stored in the database for info. It is calculated from inserted and calldurationlimit and does not need a setter.
        }

        [DataMember]
        // // [Column(Name = "transfermode", DbType = "varchar(16)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string TransferMode
        {
            get { return SIPDialogue.TransferMode.ToString(); }
            set
            {
                if (!value.IsNullOrBlank())
                {
                    SIPDialogue.TransferMode = (SIPDialogueTransferModesEnum)Enum.Parse(typeof(SIPDialogueTransferModesEnum), value, true);
                }
            }
        }

        // // [Column(Name = "direction", DbType = "varchar(3)", IsPrimaryKey = false, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        public string Direction
        {
            get { return SIPDialogue.Direction.ToString(); }
            set
            {
                if (!value.IsNullOrBlank())
                {
                    SIPDialogue.Direction = (SIPCallDirection)Enum.Parse(typeof(SIPCallDirection), value, true);
                }
            }
        }

        // // [Column(Name = "sdp", DbType = "varchar(2048)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string SDP
        {
            get { return SIPDialogue.SDP; }
            set { SIPDialogue.SDP = value; }
        }

        // // [Column(Name = "remotesdp", DbType = "varchar(2048)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string RemoteSDP
        {
            get { return SIPDialogue.RemoteSDP; }
            set { SIPDialogue.RemoteSDP = value; }
        }

        // // [Column(Name = "switchboarddescription", DbType = "varchar(1024)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string SwitchboardDescription
        {
            get { return SIPDialogue.SwitchboardDescription; }
            set { SIPDialogue.SwitchboardDescription = value; }
        }

        // // [Column(Name = "switchboardcallerdescription", DbType = "varchar(1024)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string SwitchboardCallerDescription
        {
            get { return SIPDialogue.SwitchboardCallerDescription; }
            set { SIPDialogue.SwitchboardCallerDescription = value; }
        }

        // [Column(Name = "switchboardowner", DbType = "varchar(1024)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string SwitchboardOwner
        {
            get { return SIPDialogue.SwitchboardOwner; }
            set { SIPDialogue.SwitchboardOwner = value; }
        }

        // // [Column(Name = "SwitchboardLineName", DbType = "varchar(128)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string SwitchboardLineName
        {
            get { return SIPDialogue.SwitchboardLineName; }
            set { SIPDialogue.SwitchboardLineName = value; }
        }

        // // [Column(Name = "CRMPersonName", DbType = "varchar(256)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string CRMPersonName
        {
            get { return SIPDialogue.CRMPersonName; }
            set { SIPDialogue.CRMPersonName = value; }
        }

        // // [Column(Name = "CRMCompanyName", DbType = "varchar(256)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string CRMCompanyName
        {
            get { return SIPDialogue.CRMCompanyName; }
            set { SIPDialogue.CRMCompanyName = value; }
        }

        // // [Column(Name = "CRMPictureURL", DbType = "varchar(1024)", IsPrimaryKey = false, CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public string CRMPictureURL
        {
            get { return SIPDialogue.CRMPictureURL; }
            set { SIPDialogue.CRMPictureURL = value; }
        }

        public SIPDialogueAsset()
        {
            SIPDialogue = new SIPDialogue();
        }

        public SIPDialogueAsset(SIPDialogue sipDialogue)
        {
            SIPDialogue = sipDialogue;
        }

        public string ToXML()
        {
            string dialogueXML =
                " <" + XML_ELEMENT_NAME + ">" + m_newLine +
                ToXMLNoParent() +
                " </" + XML_ELEMENT_NAME + ">" + m_newLine;

            return dialogueXML;
        }

        public string ToXMLNoParent()
        {
            string hanupAtStr = (HangupAt != null) ? HangupAt.Value.ToString("o") : null;

            string dialogueXML =
                 "  <id>" + SIPDialogue.Id + "</id>" + m_newLine +
                 "  <owner>" + SIPDialogue.Owner + "</owner>" + m_newLine +
                 "  <adminmemberid>" + SIPDialogue.AdminMemberId + "</adminmemberid>" + m_newLine +
                 "  <localtag>" + SIPDialogue.LocalTag + "</localtag>" + m_newLine +
                 "  <remotetag>" + SIPDialogue.RemoteTag + "</remotetag>" + m_newLine +
                 "  <callid>" + SIPDialogue.CallId + "</callid>" + m_newLine +
                 "  <cseq>" + SIPDialogue.CSeq + "</cseq>" + m_newLine +
                 "  <bridgeid>" + SIPDialogue.BridgeId + "</bridgeid>" + m_newLine +
                 "  <remotetarget>" + SafeXML.MakeSafeXML(SIPDialogue.RemoteTarget.ToString()) + "</remotetarget>" + m_newLine +
                 "  <localuserfield>" + SafeXML.MakeSafeXML(SIPDialogue.LocalUserField.ToString()) + "</localuserfield>" + m_newLine +
                 "  <remoteuserfield>" + SafeXML.MakeSafeXML(SIPDialogue.RemoteUserField.ToString()) + "</remoteuserfield>" + m_newLine +
                 "  <routeset>" + SafeXML.MakeSafeXML(RouteSet) + "</routeset>" + m_newLine +
                 "  <proxysipsocket>" + SafeXML.MakeSafeXML(ProxySIPSocket) + "</proxysipsocket>" + m_newLine +
                 "  <cdrid>" + SIPDialogue.CDRId + "</cdrid>" + m_newLine +
                 "  <calldurationlimit>" + SIPDialogue.CallDurationLimit + "</calldurationlimit>" + m_newLine +
                 "  <inserted>" + Inserted.ToString("dd MMM yyyy HH:mm:ss zz") + "</inserted>" + m_newLine +
                 "  <hangupat>" + hanupAtStr + "</hangupat>" + m_newLine +
                 "  <transfermode>" + TransferMode + "</transfermode>" + m_newLine +
                 "  <direction>" + Direction + "</direction>" + m_newLine +
                 "  <sdp>" + SafeXML.MakeSafeXML(SDP) + "</sdp>" + m_newLine +
                 "  <remotesdp>" + SafeXML.MakeSafeXML(RemoteSDP) + "</remotesdp>" + m_newLine +
                 //"  <switchboardcallerdescription>" + SafeXML.MakeSafeXML(SIPDialogue.SwitchboardCallerDescription) + "</switchboardcallerdescription>" + m_newLine +
                 //"  <switchboarddescription>" + SafeXML.MakeSafeXML(SIPDialogue.SwitchboardDescription) + "</switchboarddescription>" + m_newLine +
                 "  <switchboardowner>" + SafeXML.MakeSafeXML(SIPDialogue.SwitchboardOwner) + "</switchboardowner>" + m_newLine +
                 "  <switchboardlinename>" + SafeXML.MakeSafeXML(SIPDialogue.SwitchboardLineName) + "</switchboardlinename>" + m_newLine +
                 "  <crmpersonname>" + SafeXML.MakeSafeXML(SIPDialogue.CRMPersonName) + "</crmpersonname>" + m_newLine +
                 "  <crmcompanyname>" + SafeXML.MakeSafeXML(SIPDialogue.CRMCompanyName) + "</crmcompanyname>" + m_newLine +
                 "  <crmpictureurl>" + SafeXML.MakeSafeXML(SIPDialogue.CRMPictureURL) + "</crmpicutureurl>" + m_newLine;

            return dialogueXML;
        }

        public string GetXMLElementName()
        {
            return XML_ELEMENT_NAME;
        }

        public string GetXMLDocumentElementName()
        {
            return XML_DOCUMENT_ELEMENT_NAME;
        }


        public Dictionary<Guid, object> Load(XmlDocument dom)
        {
            throw new NotImplementedException();
        }
    }
}
