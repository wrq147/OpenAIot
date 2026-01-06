using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using GB28181.Sys;
using Microsoft.Extensions.Logging;

namespace GB28181.App
{
    /// <summary>
    /// 设备信息
    /// </summary>
    // [Table(Name = "Device")]
    [DataContract]
    public class SvrDevice : INotifyPropertyChanged, ISIPAsset
    {
        private ILogger logger = AppState.logger;

        public const string XML_DOCUMENT_ELEMENT_NAME = "Device";
        public const string XML_ELEMENT_NAME = "svrDevice";

        public event PropertyChangedEventHandler PropertyChanged;

        private Guid _id;
        /// <summary>
        /// ID
        /// </summary>
        // [Column(Name = "ID", DbType = "varchar(36)", IsPrimaryKey = true, CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _deviceID;
        /// <summary>
        /// 编号(设备/下级平台)
        /// </summary>
        // [Column(Name = "DeviceID", DbType = "varchar(20)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string DeviceID
        {
            get { return _deviceID; }
            set { _deviceID = value; }
        }

        private string _name;
        /// <summary>
        /// 名称
        /// </summary>
        // [Column(Name = "Name", DbType = "varchar(50)", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(_name));
            }
        }

        private string _manufacture;
        /// <summary>
        /// 厂商
        /// </summary>
        // [Column(Name = "Manufacture", DbType = "varchar(50)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string Manufacture
        {
            get { return _manufacture; }
            set { _manufacture = value; }
        }

        private string _devType;
        /// <summary>
        /// 类型
        /// </summary>
        // [Column(Name = "DevType", DbType = "varchar(20)", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string DevType
        {
            get { return _devType; }
            set { _devType = value; }
        }

        private string _model;
        /// <summary>
        /// 型号
        /// </summary>
        // [Column(Name = "Model", DbType = "varchar(20)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        private string _owner;
        /// <summary>
        /// 所有者
        /// </summary>
        // [Column(Name = "Owner", DbType = "varchar(20)", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public string Owner
        {
            get { return _owner; }
            set
            {
                _owner = value;
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(_owner));
            }
        }

        private int _cvilCode;
        /// <summary>
        /// 城市码
        /// </summary>
        // [Column(Name = "CvilCode", DbType = "int", CanBeNull = false, UpdateCheck = UpdateCheck.Never)]
        [DataMember]
        public int CvilCode
        {
            get { return _cvilCode; }
            set { _cvilCode = value; }
        }

        private string _platformId;
        /// <summary>
        /// 平台ID
        /// </summary>
        public string PlatformId
        {
            get { return _platformId; }
            set { _platformId = value; }
        }

        private string _ip;
        /// <summary>
        /// IP地址(设备/下级平台)
        /// </summary>
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }
        private int _port;
        /// <summary>
        /// 端口号(设备/下级平台)
        /// </summary>
        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }
        private string _userName;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }
        private string _password;
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _onvifAddress;
        /// <summary>
        /// onvif地址
        /// </summary>
        public string ONVIFAddress
        {
            get { return _onvifAddress; }
            set { _onvifAddress = value; }
        }

        private string _onvifVersion;
        /// <summary>
        /// onvif版本
        /// </summary>
        public string ONVIFVersion
        {
            get { return _onvifVersion; }
            set { _onvifVersion = value; }
        }

        private int _isAnalyzer;
        /// <summary>
        /// 标准化(码流分析)
        /// </summary>
        public int IsAnalyzer
        {
            get { return _isAnalyzer; }
            set { _isAnalyzer = value; }
        }

        private int _isBackRecord;
        /// <summary>
        /// 是/否有录像
        /// </summary>
        public int IsBackRecord
        {
            get { return _isBackRecord; }
            set { _isBackRecord = value; }
        }


        private string _localIP;
        /// <summary>
        /// 本地IP
        /// </summary>
        public string LocalIP
        {
            get { return _localIP; }
            set { _localIP = value; }
        }
        private int _localPort;

        /// <summary>
        /// 本地端口
        /// </summary>
        public int LocalPort
        {
            get { return _localPort; }
            set { _localPort = value; }
        }
        private string _localID;

        /// <summary>
        /// 本地编码(国标ID)
        /// </summary>
        public string LocalID
        {
            get { return _localID; }
            set { _localID = value; }
        }
        private string _macAddress;
        /// <summary>
        /// mac地址
        /// </summary>
        public string MacAddress
        {
            get { return _macAddress; }
            set { _macAddress = value; }
        }

        private string _firmware;
        /// <summary>
        /// 固件版本
        /// </summary>
        public string Firmware
        {
            get { return _firmware; }
            set { _firmware = value; }
        }
        private int _channelNum;
        /// <summary>
        /// 通道数量
        /// </summary>
        public int ChannelNum
        {
            get { return _channelNum; }
            set { _channelNum = value; }
        }
        private int _status;
        /// <summary>
        /// 状态
        /// </summary>
        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }
        private string _reason;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }
        private DateTime _deviceTime;
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DeviceTime
        {
            get { return _deviceTime; }
            set { _deviceTime = value; }
        }
        private int _parental;
        /// <summary>
        /// 
        /// </summary>
        public int Parental
        {
            get { return _parental; }
            set { _parental = value; }
        }
        private int _parentID;
        /// <summary>
        /// 上级ID
        /// </summary>
        public int ParentID
        {
            get { return _parentID; }
            set { _parentID = value; }
        }
        private string _address;
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        private int _block;
        /// <summary>
        /// 锁定
        /// </summary>
        public int Block
        {
            get { return _block; }
            set { _block = value; }
        }
        private int _registerWay;
        /// <summary>
        /// 注册认证
        /// </summary>
        public int RegisterWay
        {
            get { return _registerWay; }
            set { _registerWay = value; }
        }
        private int _secrecy;
        /// <summary>
        /// 安全模式
        /// </summary>
        public int Secrecy
        {
            get { return _secrecy; }
            set { _secrecy = value; }
        }
        private DateTime _endTime;
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }
        private int _errCode;
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrCode
        {
            get { return _errCode; }
            set { _errCode = value; }
        }
        private int _certifiable;

        public int Certifiable
        {
            get { return _certifiable; }
            set { _certifiable = value; }
        }
        private int _certNum;

        public int CertNum
        {
            get { return _certNum; }
            set { _certNum = value; }
        }
        private string _uri;
        /// <summary>
        /// web地址
        /// </summary>
        public string URI
        {
            get { return _uri; }
            set { _uri = value; }
        }

        private int _webPort;
        /// <summary>
        /// web端口
        /// </summary>
        public int WebPort
        {
            get { return _webPort; }
            set { _webPort = value; }
        }
        private string _latitude;
        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }
        private string _longitude;
        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
        private DateTime _createTime;
        /// <summary>
        /// 接入时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return _createTime; }
            set { _createTime = value; }
        }
        private DateTime _updateTime;


        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateTime
        {
            get { return _updateTime; }
            set { _updateTime = value; }
        }

        public string ToXML()
        {
            return null;
        }

        public string ToXMLNoParent()
        {
            return null;
        }

        public string GetXMLElementName()
        {
            return XML_ELEMENT_NAME;
        }

        public string GetXMLDocumentElementName()
        {
            return XML_DOCUMENT_ELEMENT_NAME;
        }


        public Dictionary<Guid, object> Load(System.Xml.XmlDocument dom)
        {
            throw new NotImplementedException();
        }
    }
}
