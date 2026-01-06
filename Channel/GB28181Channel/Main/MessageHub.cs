using System;
using System.Collections.Generic;
using System.Text;
using GB28181.App;
using GB28181.Servers;
using GB28181.Servers.SIPMessages;
using GB28181.Sys;
using GB28181.Sys.XML;
using Microsoft.Extensions.Logging;
using SIPSorcery.SIP;
namespace GB28181.Server.Main
{
    public class MessageHub
    {
        private static readonly ILogger logger = AppState.logger;
        private DateTime _keepaliveTime;
        private readonly Queue<HeartBeatEndPoint> _keepAliveQueue = new Queue<HeartBeatEndPoint>();
        private readonly Queue<Catalog> _catalogQueue = new Queue<Catalog>();
        private readonly List<string> _deviceAlarmSubscribed = new List<string>();
        private ISipMessageCore _sipCoreMessageService;
        private ISIPMonitorCore _sIPMonitorCore;
        private ISIPRegistrarCore _registrarCore;

        public Dictionary<string, DeviceStatus> DeviceStatuses { get; } = new Dictionary<string, DeviceStatus>();

        public Dictionary<string, Catalog> Catalogs { get; } = new Dictionary<string, Catalog>();
        public Dictionary<string, SIPTransaction> GBSIPTransactions { get; } = new Dictionary<string, SIPTransaction>();

        private SIPAccount _SIPAccount;

        public MessageHub(ISipMessageCore sipCoreMessageService, ISIPMonitorCore sIPMonitorCore, ISIPRegistrarCore sipRegistrarCore)
        {
            _sipCoreMessageService = sipCoreMessageService;
            _sIPMonitorCore = sIPMonitorCore;
            _registrarCore = sipRegistrarCore;
            _registrarCore.DeviceAlarmSubscribe += OnDeviceAlarmSubscribeReceived;
            _registrarCore.DmsRegisterReceived += _sipRegistrarCore_RPCDmsRegisterReceived;
        }

        public void OnCatalogReceived(Catalog obj)
        {
            if (!Catalogs.ContainsKey(obj.DeviceID))
            {
                Catalogs.Add(obj.DeviceID, obj);
            }
            if (GBSIPTransactions.ContainsKey(obj.DeviceID))
            {
                SIPTransaction _SIPTransaction = GBSIPTransactions[obj.DeviceID];
                obj.DeviceList.Items.FindAll(item => item != null).ForEach(catalogItem =>
                {
                    var devCata = DevType.GetCataType(catalogItem.DeviceID);
                    if (devCata == DevCataType.Device)
                    {
                        _SIPTransaction.TransactionRequestFrom.URI.User = catalogItem.DeviceID;
                        string gbname = "GB_" + catalogItem.Name;
                        //string gbname = "gb" + _SIPTransaction.TransactionRequest.RemoteSIPEndPoint.Address.ToString();
                        if (!string.IsNullOrEmpty(catalogItem.ParentID) && !obj.DeviceID.Equals(catalogItem.DeviceID))
                        {
                            gbname = "GB_" + catalogItem.Name;
                        }

                        //query device info from db
                        string edit = IsDeviceExisted(catalogItem.DeviceID) ? "updated" : "added";

                        //Device Dms Register
                        DeviceDmsRegister(_SIPTransaction, gbname);

                    }
                });
            }
        }

        public void OnDeviceStatusReceived(SIPEndPoint arg1, DeviceStatus arg2)
        {
            DeviceStatuses.Remove(arg2.DeviceID);
            DeviceStatuses.Add(arg2.DeviceID, arg2);
        }

        internal void OnKeepaliveReceived(SIPEndPoint remoteEP, KeepAlive keapalive, string devId)
        {
            _keepaliveTime = DateTime.Now;
            var hbPoint = new HeartBeatEndPoint()
            {
                RemoteEP = remoteEP,
                Heart = keapalive,
                KeepaliveTime = _keepaliveTime
            };
            _keepAliveQueue.Enqueue(hbPoint);

            //HeartBeatStatuses.Remove(devId);
            //HeartBeatStatuses.Add(devId, hbPoint);
        }

        internal void OnServiceChanged(string msg, ServiceStatus state)
        {
            SetSIPService(msg, state);
        }

        /// <summary>
        /// 设置sip服务状态
        /// </summary>
        /// <param name="state">sip状态</param>
        private void SetSIPService(string msg, ServiceStatus state)
        {
            logger.LogDebug("SIP Service Status: " + msg + "," + state);
        }


        //设备信息查询回调函数
        private void DeviceInfoReceived(SIPEndPoint remoteEP, DeviceInfo device)
        {
        }

        //设备状态查询回调函数
        private void DeviceStatusReceived(SIPEndPoint remoteEP, DeviceStatus device)
        {
        }

        ///// <summary>
        ///// 录像查询回调
        ///// </summary>
        ///// <param name="record"></param>
        //internal void OnRecordInfoReceived(RecordInfo record)
        //{
        //    SetRecord(record);
        //}

        //private void SetRecord(RecordInfo record)
        //{
        //    foreach (var item in record.RecordItems.Items)
        //    {
        //    }
        //}

        //internal void OnNotifyCatalogReceived(NotifyCatalog notify)
        //{
        //    if (notify.DeviceList == null)
        //    {
        //        return;
        //    }
        //    new Action(() =>
        //    {
        //        foreach (var item in notify.DeviceList.Items)
        //        {
        //        }
        //    }).BeginInvoke(null, null);
        //}

        /// <summary>
        /// 报警订阅
        /// </summary>
        /// <param name="sIPTransaction"></param>
        /// <param name="sIPAccount"></param>
        internal void OnDeviceAlarmSubscribeReceived(SIPTransaction sIPTransaction)
        {
            try
            {
                string keyDeviceAlarmSubscribe = sIPTransaction.RemoteEndPoint.ToString() + " - " + sIPTransaction.TransactionRequestFrom.URI.User;
                //if (!_deviceAlarmSubscribed.Contains(keyDeviceAlarmSubscribe))
                //{
                //_sIPMonitorCore.DeviceControlResetAlarm(sIPTransaction.RemoteEndPoint, sIPTransaction.TransactionRequestFrom.URI.User);
                //logger.Debug("Device Alarm Reset: " + keyDeviceAlarmSubscribe);
                //_sIPMonitorCore.DeviceAlarmSubscribe(sIPTransaction.RemoteEndPoint, sIPTransaction.TransactionRequestFrom.URI.User);
                //logger.Debug("Device Alarm Subscribe: " + keyDeviceAlarmSubscribe);
                //_deviceAlarmSubscribed.Add(keyDeviceAlarmSubscribe);
                //}
            }
            catch (Exception ex)
            {
                logger.LogError("OnDeviceAlarmSubscribeReceived: " + ex.Message);
            }
        }
        /// <summary>
        /// 设备报警
        /// </summary>
        /// <param name="alarm"></param>
        internal void OnAlarmReceived(Alarm alarm)
        {

        }

        /// <summary>
        /// 设备注册事件
        /// </summary>
        /// <param name="sipTransaction"></param>
        /// <param name="sIPAccount"></param>
        private void _sipRegistrarCore_RPCDmsRegisterReceived(SIPTransaction sipTransaction, GB28181.App.SIPAccount sIPAccount)
        {
            try
            {
                _SIPAccount = sIPAccount;
                string deviceid = sipTransaction.TransactionRequestFrom.URI.User;

                //GB SIPTransactions Dictionary
                GBSIPTransactions.Remove(deviceid);
                GBSIPTransactions.Add(deviceid, sipTransaction);

                //Device Catalog Query
                _sipCoreMessageService.DeviceCatalogQuery(deviceid);

                ////query device info from db
                //string edit = IsDeviceExisted(deviceid) ? "updated" : "added";

                ////Device Dms Register
                //DeviceDmsRegister(sipTransaction,"gb");

                ////Device Edit Event
                //DeviceEditEvent(deviceid, edit);
            }
            catch (Exception ex)
            {
                logger.LogError("_sipRegistrarCore_RPCDmsRegisterReceived Exception: " + ex.Message);
            }
        }
        private void DeviceDmsRegister(SIPTransaction sipTransaction, string gbname)
        {
            //try
            //{
            //    //Device insert into database
            //    Device _device = new Device();
            //    SIPRequest sipRequest = sipTransaction.TransactionRequest;
            //    _device.Guid = Guid.NewGuid().ToString();
            //    _device.IP = sipTransaction.TransactionRequest.RemoteSIPEndPoint.Address.ToString();//IPC
            //    _device.Name = gbname;
            //    _device.LoginUser.Add(new LoginUser() { LoginName = _SIPAccount.SIPUsername ?? "admin", LoginPwd = _SIPAccount.SIPPassword ?? "123456" });//same to GB config service
            //    _device.Port = Convert.ToUInt32(sipTransaction.TransactionRequest.RemoteSIPEndPoint.Port);//5060
            //    _device.GBID = sipTransaction.TransactionRequestFrom.URI.User;//42010000001180000184
            //    _device.PtzType = 0;
            //    _device.ProtocolType = 0;
            //    _device.ShapeType = ShapeType.Dome;
          
            //}
            //catch (Exception ex)
            //{
            //    logger.Error("DeviceDmsRegister Exception: " + ex.Message);
            //}
        }
        /// <summary>
        /// query device info from db
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        private bool IsDeviceExisted(string deviceid)
        {
            bool tf = false;
            ////var options = new List<ChannelOption> { new ChannelOption(ChannelOptions.MaxMessageLength, int.MaxValue) };
            ////   Channel channel = new Channel(EnvironmentVariables.DeviceManagementServiceAddress ?? "devicemanagementservice:8080", ChannelCredentials.Insecure);
            ////devicemanagementservice 是预留的服务标识(暂命名为设备管理服务).目前没有这个服务.
            ////需要你的微服务架构中实现一个设备资产以及一个配置管理服务(或者二合一的资源管服务)
            ////以达到两个目的：1、用来为当前GB服务提供启动配置，2、为GB收到注册的设备/平台信息，提供全平台的统一的存储服务.
            //var channel = GrpcChannel.ForAddress(EnvironmentVariables.DeviceManagementServiceAddress ?? "devicemanagementservice:8080"); //, ChannelCredentials.Insecure);
            //logger.Debug("Device Management Service Address: " + (EnvironmentVariables.DeviceManagementServiceAddress ?? "devicemanagementservice:8080"));
            //var client = new DevicesManager.DevicesManagerClient(channel);
            //QueryGBDeviceByGBIDsResponse rep = new QueryGBDeviceByGBIDsResponse();
            //QueryGBDeviceByGBIDsRequest req = new QueryGBDeviceByGBIDsRequest();
            //req.GbIds.Add(deviceid);
            //rep = client.QueryGBDeviceByGBIDs(req);
            //tf = rep.Devices.Count > 0;
            return tf;
        }


        //internal void OnDeviceStatusReceived(SIPEndPoint remoteEP, DeviceStatus device)
        //{
        //    var msg = "DeviceID:" + device.DeviceID +
        //         "\r\nResult:" + device.Result +
        //         "\r\nOnline:" + device.Online +
        //         "\r\nState:" + device.Status;
        //    new Action(() =>
        //    {
        //    }).Invoke();
        //}

        internal void OnDeviceInfoReceived(SIPEndPoint arg1, DeviceInfo arg2)
        {
            throw new NotImplementedException();
        }

        internal void OnMediaStatusReceived(SIPEndPoint arg1, MediaStatus arg2)
        {
            throw new NotImplementedException();
        }

        internal void OnPresetQueryReceived(SIPEndPoint arg1, PresetInfo arg2)
        {
            throw new NotImplementedException();
        }

        internal void OnDeviceConfigDownloadReceived(SIPEndPoint arg1, DeviceConfigDownload arg2)
        {
            throw new NotImplementedException();
        }
        internal void OnResponseCodeReceived(SIPResponseStatusCodesEnum status, string msg, SIPEndPoint remoteEP)
        {
            logger.LogDebug("OnResponseCodeReceived: " + msg);
        }
    }

    /// <summary>
    /// 心跳
    /// </summary>
    public class HeartBeatEndPoint
    {
        /// <summary>
        /// 远程终结点
        /// </summary>
        public SIPEndPoint RemoteEP { get; set; }

        /// <summary>
        /// 心跳周期
        /// </summary>
        public KeepAlive Heart { get; set; }

        public DateTime KeepaliveTime { get; set; }
    }

    public class Message
    {
        public Dictionary<string, string> Header { get; set; }
        public byte[] Body { get; set; }
    }
}