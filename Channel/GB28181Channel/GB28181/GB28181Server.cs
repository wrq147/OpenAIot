using GB28181Channel.GB28181.DTO;
using GB28181Channel.GB28181.Enum;
using GB28181Channel.GB28181.Event;
using GB28181Channel.GB28181.Interface;
using SIPSorcery.Net;
using SIPSorcery.SIP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;

namespace GB28181Channel.GB28181
{
    /// <summary>
    /// GB28181服务器核心类
    /// </summary>
    public class GB28181Server : IDisposable
    {
        #region 完整事件体系
        public event Func<object?, DeviceRegisteredEventArgs, Task> DeviceRegistered;
        public event Func<object?, DeviceOfflineEventArgs, Task> DeviceOffline;
        public event Func<object?, CatalogReceivedEventArgs, Task> CatalogReceived;
        public event Func<object?, AlarmReceivedEventArgs, Task> AlarmReceived;
        public event Func<object?, StreamPlayEventArgs, Task> StreamPlayed;
        #endregion

        // 核心字段
        private readonly SIPTransport _sipTransport;
        private readonly IDeviceStorage _deviceStorage;
        private readonly IMediaHandler _mediaHandler;
        private readonly ConcurrentDictionary<string, DateTime> _heartbeatMap = new ConcurrentDictionary<string, DateTime>();
        private readonly System.Timers.Timer _heartbeatTimer;
        private readonly GB28181Version _protocolVersion;
        private readonly string _serverId;
        private readonly int _sipPort;
        private readonly string _serverIp;
        private readonly int _heartbeatTimeout = 100;
        private readonly int _defaultRtpPort = 58200;
        // 传输协议（枚举类型）
        private readonly SIPTransportProtocol _transportProtocol;

        private int _cseqCounter = 1;
        private readonly object _cseqLock = new object();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serverIp">服务器IP</param>
        /// <param name="sipPort">SIP端口（UDP/TCP共用）</param>
        /// <param name="serverId">服务器ID</param>
        /// <param name="protocolVersion">GB28181协议版本</param>
        /// <param name="deviceStorage">设备存储接口</param>
        /// <param name="mediaHandler">媒体处理接口</param>
        /// <param name="transportProtocol">SIP传输协议（默认：同时使用UDP+TCP）</param>
        public GB28181Server(string serverIp, int sipPort, string serverId,
                             GB28181Version protocolVersion, IDeviceStorage deviceStorage, IMediaHandler mediaHandler,
                             SIPTransportProtocol transportProtocol = SIPTransportProtocol.Both)
        {
            _serverIp = serverIp ?? throw new ArgumentNullException(nameof(serverIp));
            _sipPort = sipPort;
            _serverId = serverId ?? throw new ArgumentNullException(nameof(serverId));
            _protocolVersion = protocolVersion;
            _deviceStorage = deviceStorage ?? throw new ArgumentNullException(nameof(deviceStorage));
            _mediaHandler = mediaHandler ?? throw new ArgumentNullException(nameof(mediaHandler));
            _transportProtocol = transportProtocol;

            _sipTransport = new SIPTransport();
            var localEP = new IPEndPoint(IPAddress.Parse(_serverIp), _sipPort);

            // 根据枚举值绑定对应的传输通道
            switch (_transportProtocol)
            {
                case SIPTransportProtocol.UdpOnly:
                    var udpChannel = new SIPUDPChannel(localEP);
                    _sipTransport.AddSIPChannel(udpChannel);
                    break;
                case SIPTransportProtocol.TcpOnly:
                    var tcpChannel = new SIPTCPChannel(localEP);
                    _sipTransport.AddSIPChannel(tcpChannel);
                    break;
                case SIPTransportProtocol.Both:
                    // 同时绑定UDP和TCP通道
                    var udpChannelBoth = new SIPUDPChannel(localEP);
                    _sipTransport.AddSIPChannel(udpChannelBoth);
                    var tcpChannelBoth = new SIPTCPChannel(localEP);
                    _sipTransport.AddSIPChannel(tcpChannelBoth);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(transportProtocol), "不支持的传输协议类型");
            }

            // 注册SIP请求处理器（UDP/TCP请求通用处理）
            _sipTransport.SIPTransportRequestReceived += OnSIPRequestReceived;

            // 初始化心跳检查定时器
            _heartbeatTimer = new System.Timers.Timer(30 * 1000);
            _heartbeatTimer.Elapsed += OnHeartbeatCheck;
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        public void Start()
        {
            try
            {
                _heartbeatTimer.Start();
                Console.WriteLine($"[GB28181 Server] 启动成功");

                // 根据枚举值输出协议信息
                var protocolDesc = _transportProtocol switch
                {
                    SIPTransportProtocol.UdpOnly => "UDP",
                    SIPTransportProtocol.TcpOnly => "TCP",
                    SIPTransportProtocol.Both => "UDP/TCP",
                    _ => "未知"
                };

                Console.WriteLine($"监听地址：{_serverIp}:{_sipPort} | 传输协议：{protocolDesc} | GB28181版本：{_protocolVersion}");
                Console.WriteLine($"默认RTP端口：{_defaultRtpPort} | 心跳超时：{_heartbeatTimeout}秒");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"启动失败：{ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 处理所有SIP请求（UDP/TCP通用）
        /// </summary>
        private async Task OnSIPRequestReceived(SIPEndPoint localEP, SIPEndPoint remoteEP, SIPRequest req)
        {
            try
            {
                // 输出请求的传输协议类型（UDP/TCP）
                var transportType = localEP.Protocol == SIPProtocolsEnum.udp ? "UDP" : "TCP";
                Console.WriteLine($"[SIP请求] {req.Method} | {remoteEP} | 传输协议：{transportType}");

                var remoteIpEP = new IPEndPoint(remoteEP.Address, remoteEP.Port);

                switch (req.Method)
                {
                    case SIPMethodsEnum.REGISTER:
                        await HandleRegister(req, remoteIpEP);
                        break;
                    case SIPMethodsEnum.MESSAGE:
                        await HandleMessage(req, remoteIpEP);
                        break;
                    case SIPMethodsEnum.INVITE:
                        await HandleInvite(req, remoteIpEP);
                        break;
                    case SIPMethodsEnum.BYE:
                        await HandleBye(req, remoteIpEP);
                        break;
                    default:
                        var resp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.NotImplemented, "Method not supported");
                        await _sipTransport.SendResponseAsync(resp);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理SIP请求失败：{ex.Message}");
                var errorResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.InternalServerError, "Server error");
                await _sipTransport.SendResponseAsync(errorResp);
            }
        }

        #region 核心业务处理（支持扩展）
        /// <summary>
        /// 处理设备注册
        /// </summary>
        private async Task HandleRegister(SIPRequest req, IPEndPoint remoteEP)
        {
            var deviceId = req.Header.From.FromURI.User;
            Console.WriteLine($"[注册请求] {deviceId} @ {remoteEP}");

            try
            {
                bool isUnregisterRequest = req.Header.Expires == 0 ||
                              (req.Header.Contact != null && req.Header.Contact.Count > 0 && req.Header.Contact[0].Expires == 0);

                if (isUnregisterRequest)
                {
                    await HandleDeviceUnregister(deviceId, remoteEP, req);
                    return;
                }
                // 检查是否携带Authorization头（设备是否已响应认证挑战）
                if (req.Header.AuthenticationHeaders == null || req.Header.AuthenticationHeaders.Count == 0)
                {
                    // 无认证信息，返回401 Unauthorized，发起认证挑战
                    var challengeResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Unauthorised, "Unauthorized");

                    // 构造WWW-Authenticate头（SIP Digest认证标准）
                    var nonce = Guid.NewGuid().ToString("N"); // 随机挑战值
                    var realm = _serverId; // 认证域，通常用服务器ID
                    var sipDigest = new SIPAuthorisationDigest(SIPAuthorisationHeadersEnum.WWWAuthenticate, DigestAlgorithmsEnum.MD5);
                    sipDigest.Qop = "auth";
                    sipDigest.Nonce = nonce;
                    sipDigest.Realm = realm;
                    challengeResp.Header.AuthenticationHeaders = new List<SIPAuthenticationHeader> { new SIPAuthenticationHeader(sipDigest) };

                    await _sipTransport.SendResponseAsync(challengeResp);
                    Console.WriteLine($"[认证挑战] {deviceId}：发送401认证请求");
                    return;
                }
                else
                {
                    // 有Authorization头，验证密码
                    var authHeader = req.Header.AuthenticationHeaders[0];
                    var realm = authHeader.SIPDigest.Realm;
                    var nonce = authHeader.SIPDigest.Nonce;
                    var response = authHeader.SIPDigest.Response; // 设备生成的认证摘要


                    string tpassword = await _deviceStorage.GetDevicePassword(deviceId);
                    if (string.IsNullOrEmpty(tpassword))
                    {
                        var unauthorizedResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Unauthorised, "The password cannot be empty");
                        await _sipTransport.SendResponseAsync(unauthorizedResp);
                        Console.WriteLine($"[注册失败] {deviceId}：密码不能为空");
                        return;
                    }
                    // 计算期望的认证摘要（与设备侧算法一致）

                    var sipExpectedDigest = authHeader.SIPDigest.CopyOf();
                    sipExpectedDigest.Password = tpassword;
                    sipExpectedDigest.RequestType = "REGISTER";
                    var expectedResponse = sipExpectedDigest.GetDigest();

                    // 验证摘要是否匹配
                    if (response != expectedResponse)
                    {
                        var unauthorizedResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Unauthorised, "Password error");
                        await _sipTransport.SendResponseAsync(unauthorizedResp);
                        Console.WriteLine($"[注册失败] {deviceId}：密码验证失败");
                        return;
                    }

                    // 3. 密码验证通过，完成注册流程
                    var deviceInfo = new DeviceInfo
                    {
                        DeviceId = deviceId,
                        DeviceIp = remoteEP.Address.ToString(),
                        DevicePort = remoteEP.Port,
                        RegisterTime = DateTime.Now,
                        LastHeartbeatTime = DateTime.Now,
                        ProtocolVersion = _protocolVersion
                    };

                    if (req.Header.Contact != null && req.Header.Contact.Count > 0)
                    {
                        deviceInfo.DevicePort = Convert.ToInt32(req.Header.Contact[0].ContactURI.HostPort);
                    }

                    _heartbeatMap.AddOrUpdate(deviceId, DateTime.Now, (key, oldValue) => DateTime.Now);

                    var okResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
                    okResp.Header.Expires = _protocolVersion == GB28181Version.V2016 ? 3600 : 7200;
                    okResp.Header.UnknownHeaders.Add("X-GB28181-Version: " + _protocolVersion.ToString());
                    await _sipTransport.SendResponseAsync(okResp);

                    await OnDeviceRegistered(new DeviceRegisteredEventArgs
                    {
                        Device = deviceInfo,
                        OriginalRequest = req,
                        RemoteEndPoint = remoteEP
                    });

                    Console.WriteLine($"[注册成功] {deviceId}：密码验证通过");
                }
            }
            catch (Exception ex)
            {
                var errorResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.InternalServerError, "Register failed");
                await _sipTransport.SendResponseAsync(errorResp);

                Console.WriteLine($"[注册失败] {deviceId}：{ex.Message}");
            }
        }
        /// <summary>
        /// 处理设备主动注销
        /// </summary>
        private async Task HandleDeviceUnregister(string deviceId, IPEndPoint remoteEP, SIPRequest req)
        {
            try
            {
                // 回复200 OK确认注销
                var okResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "Unregistered successfully");
                okResp.Header.Expires = 0; // 明确标识注销
                okResp.Header.UnknownHeaders.Add("X-GB28181-Version: " + _protocolVersion.ToString());
                await _sipTransport.SendResponseAsync(okResp);

                // 触发注销事件
                var offlineTime = DateTime.Now;
                var offlineArgs = new DeviceOfflineEventArgs
                {
                    DeviceId = deviceId,
                    OfflineTime = offlineTime,
                    LastHeartbeat = offlineTime,
                    Reason = "设备主动注销"
                };

                // 触发通用的离线事件
                await OnDeviceOffline(offlineArgs);

                // 清理心跳映射表
                _heartbeatMap.TryRemove(deviceId, out _);
                Console.WriteLine($"[注销成功] {deviceId}：设备主动注销");
            }
            catch (Exception ex)
            {
                var errorResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.InternalServerError, "Unregister failed");
                await _sipTransport.SendResponseAsync(errorResp);

                Console.WriteLine($"[注销失败] {deviceId}：{ex.Message}");
            }
        }
        /// <summary>
        /// 处理MESSAGE请求（心跳/目录/报警）
        /// </summary>
        private async Task HandleMessage(SIPRequest req, IPEndPoint remoteEP)
        {
            var deviceId = req.Header.From.FromURI.User;
            var body = req.Body ?? string.Empty;

            try
            {
                if (body.Contains("Keepalive"))
                {
                    await HandleKeepaliveMessage(deviceId, remoteEP, req);
                }
                else if (body.Contains("Catalog"))
                {
                    await HandleCatalogMessage(deviceId, remoteEP, req);
                }
                else if (body.Contains("Alarm"))
                {
                    await HandleAlarmMessage(deviceId, remoteEP, req);
                }
                else
                {
                    var response = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
                    await _sipTransport.SendResponseAsync(response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[处理MESSAGE失败] {deviceId}：{ex.Message}");
                var errorResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.InternalServerError, "Message handle failed");
                await _sipTransport.SendResponseAsync(errorResp);
            }
        }

        /// <summary>
        /// 处理心跳消息
        /// </summary>
        private async Task HandleKeepaliveMessage(string deviceId, IPEndPoint remoteEP, SIPRequest req)
        {
            bool needRegDevice = !_heartbeatMap.ContainsKey(deviceId);
            _heartbeatMap.AddOrUpdate(deviceId, DateTime.Now, (key, oldValue) => DateTime.Now);

            if (needRegDevice)
            {
                // 通知设备重新注册：返回401 Unauthorized响应，携带认证挑战
                var unauthorizedResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Unauthorised, "Need re-register");

                // 构造WWW-Authenticate头（与注册阶段的认证逻辑一致）
                var nonce = Guid.NewGuid().ToString("N"); // 随机挑战值
                var realm = _serverId; // 认证域，与注册阶段保持一致
                var sipDigest = new SIPAuthorisationDigest(SIPAuthorisationHeadersEnum.WWWAuthenticate, DigestAlgorithmsEnum.MD5);
                sipDigest.Qop = "auth";
                sipDigest.Nonce = nonce;
                sipDigest.Realm = realm;
                unauthorizedResp.Header.AuthenticationHeaders = new List<SIPAuthenticationHeader> { new SIPAuthenticationHeader(sipDigest) };

                await _sipTransport.SendResponseAsync(unauthorizedResp);
                Console.WriteLine($"[心跳检测] {deviceId} @ {remoteEP} 未注册，要求重新注册");
            }
            else
            {
                var response = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
                await _sipTransport.SendResponseAsync(response);
                Console.WriteLine($"[心跳] {deviceId} @ {remoteEP}");
            }
        }

        /// <summary>
        /// 处理目录消息
        /// </summary>
        private async Task HandleCatalogMessage(string deviceId, IPEndPoint remoteEP, SIPRequest req)
        {
            var xmlDoc = XDocument.Parse(req.Body);
            var channels = new List<ChannelInfo>();

            var deviceList = xmlDoc.Descendants("DeviceList").FirstOrDefault();
            if (deviceList != null)
            {
                foreach (var deviceNode in deviceList.Descendants("Device"))
                {
                    var channel = new ChannelInfo
                    {
                        ChannelId = deviceNode.Element("DeviceID")?.Value ?? string.Empty,
                        ChannelName = deviceNode.Element("DeviceName")?.Value ?? string.Empty,
                        DeviceId = deviceId,
                        Manufacturer = deviceNode.Element("Manufacturer")?.Value ?? string.Empty,
                        Model = deviceNode.Element("Model")?.Value ?? string.Empty,
                        Status = deviceNode.Element("Status")?.Value ?? "ON"
                    };

                    if (!string.IsNullOrEmpty(channel.ChannelId))
                        channels.Add(channel);
                }
            }

            var response = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
            await _sipTransport.SendResponseAsync(response);

            // 触发目录事件（支持扩展）
            await OnCatalogReceived(new CatalogReceivedEventArgs
            {
                DeviceId = deviceId,
                Channels = channels,
                OriginalXml = xmlDoc
            });

            Console.WriteLine($"[目录更新] {deviceId} 共{channels.Count}个通道");
        }

        /// <summary>
        /// 处理报警消息
        /// </summary>
        private async Task HandleAlarmMessage(string deviceId, IPEndPoint remoteEP, SIPRequest req)
        {
            var xmlDoc = XDocument.Parse(req.Body);
            var alarmInfo = new AlarmInfo
            {
                AlarmId = Guid.NewGuid().ToString(),
                DeviceId = deviceId,
                ChannelId = xmlDoc.Element("Alarm")?.Element("ChannelID")?.Value ?? deviceId,
                AlarmType = xmlDoc.Element("Alarm")?.Element("AlarmType")?.Value ?? "Unknown",
                AlarmTime = DateTime.Now,
                AlarmDescription = xmlDoc.Element("Alarm")?.Element("Description")?.Value ?? string.Empty,
                IsHandled = false
            };

            var response = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
            await _sipTransport.SendResponseAsync(response);

            // 触发报警事件（支持扩展）
            await OnAlarmReceived(new AlarmReceivedEventArgs
            {
                Alarm = alarmInfo,
                OriginalXml = xmlDoc
            });

            Console.WriteLine($"[报警接收] {deviceId} - {alarmInfo.AlarmType}：{alarmInfo.AlarmDescription}");
        }

        /// <summary>
        /// 处理点播请求
        /// </summary>
        private async Task HandleInvite(SIPRequest req, IPEndPoint remoteEP)
        {
            var channelId = req.Header.To.ToURI.User;
            Console.WriteLine($"[点播请求] {channelId} @ {remoteEP}");

            try
            {
                var sdp = SDP.ParseSDPDescription(req.Body);
                var rtpPort = sdp.Media.First().Port;

                var playbackParams = new PlaybackParams
                {
                    ChannelId = channelId,
                    DeviceId = channelId.Substring(0, 20),
                    RemoteIp = remoteEP.Address.ToString(),
                    RemoteRtpPort = rtpPort,
                    LocalRtpPort = _defaultRtpPort,
                    IsLive = true
                };

                // 回复100 Trying
                var tryingResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Trying, "Trying");
                await _sipTransport.SendResponseAsync(tryingResp);

                // 生成SDP（通过IMediaHandler扩展）
                var sdpResp = _mediaHandler.GenerateSDP(playbackParams);

                // 启动RTP接收（通过IMediaHandler扩展）
                var sessionId = _mediaHandler.StartRtpReceiver(playbackParams);

                // 回复200 OK
                var okResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
                okResp.Body = sdpResp;
                okResp.Header.ContentType = "application/sdp";
                await _sipTransport.SendResponseAsync(okResp);

                // 触发点播事件（支持扩展）
                await OnStreamPlayed(new StreamPlayEventArgs
                {
                    Params = playbackParams,
                    IsSuccess = true,
                    SessionId = sessionId,
                    Message = "点播成功"
                });

                Console.WriteLine($"[点播成功] {channelId} SessionID: {sessionId} RTP端口：{playbackParams.LocalRtpPort}");
            }
            catch (Exception ex)
            {
                var errorResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.BadRequest, "Invite failed");
                await _sipTransport.SendResponseAsync(errorResp);

                await OnStreamPlayed(new StreamPlayEventArgs
                {
                    Params = new PlaybackParams { ChannelId = channelId },
                    IsSuccess = false,
                    Message = $"点播失败：{ex.Message}"
                });

                Console.WriteLine($"[点播失败] {channelId}：{ex.Message}");
            }
        }

        /// <summary>
        /// 处理停止推流
        /// </summary>
        private async Task HandleBye(SIPRequest req, IPEndPoint remoteEP)
        {
            var channelId = req.Header.From.FromURI.User;
            var sessionId = req.Header.CallId;

            // 停止RTP接收（通过IMediaHandler扩展）
            var stopResult = _mediaHandler.StopRtpReceiver(sessionId);

            var resp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
            await _sipTransport.SendResponseAsync(resp);

            await OnStreamPlayed(new StreamPlayEventArgs
            {
                Params = new PlaybackParams { ChannelId = channelId },
                IsSuccess = stopResult,
                SessionId = sessionId,
                Message = stopResult ? "停止推流成功" : "停止推流失败"
            });

            Console.WriteLine($"[停止推流] {channelId} SessionID: {sessionId} {(stopResult ? "成功" : "失败")}");
        }

        /// <summary>
        /// 心跳检查（检测设备离线）
        /// </summary>
        private async void OnHeartbeatCheck(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            var offlineDevices = new List<string>();

            // 遍历ConcurrentDictionary的快照，无需锁
            foreach (var kvp in _heartbeatMap)
            {
                var deviceId = kvp.Key;
                var lastHeartbeat = kvp.Value;

                if ((now - lastHeartbeat).TotalSeconds > _heartbeatTimeout)
                {
                    offlineDevices.Add(deviceId);

                    await OnDeviceOffline(new DeviceOfflineEventArgs
                    {
                        DeviceId = deviceId,
                        OfflineTime = now,
                        LastHeartbeat = lastHeartbeat,
                        Reason = $"心跳超时（{_heartbeatTimeout}秒）"
                    });

                    Console.WriteLine($"[设备离线] {deviceId} - {_heartbeatTimeout}秒心跳超时");
                }
            }

            // 批量移除离线设备，使用TryRemove原子操作
            foreach (var deviceId in offlineDevices)
            {
                _heartbeatMap.TryRemove(deviceId, out _);
            }
        }
        #endregion

        #region 扩展功能（PTZ控制）
        /// <summary>
        /// 发送PTZ控制命令（支持扩展）
        /// </summary>
        public async Task<bool> SendPTZControl(PTZControlParams @params)
        {
            try
            {
                if (@params == null || string.IsNullOrEmpty(@params.DeviceId))
                {
                    throw new ArgumentException("PTZ控制参数无效");
                }

                var device = _deviceStorage.GetDevice(@params.DeviceId);
                if (device == null)
                {
                    Console.WriteLine("设备不存在");
                    return false;
                }

                var controlXml = GeneratePTZControlXml(@params);
                var sipRequest = CreateSIPMessageRequest(device.DeviceId, device.DeviceIp, device.DevicePort, controlXml);

                // 异步发送PTZ控制命令（自动适配服务器启用的传输协议）
                _ = _sipTransport.SendRequestAsync(sipRequest);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 生成PTZ控制XML
        /// </summary>
        private string GeneratePTZControlXml(PTZControlParams @params)
        {
            var xml = new XDocument(
                new XElement("Control",
                    new XElement("CmdType", "DeviceControl"),
                    new XElement("SN", new Random().Next(1000, 9999)),
                    new XElement("DeviceID", @params.ChannelId),
                    new XElement("PTZCmd", @params.CommandType.ToString()),
                    new XElement("Speed", @params.Speed),
                    new XElement("PresetID", @params.PresetId)
                )
            );

            return xml.ToString(SaveOptions.DisableFormatting);
        }

        /// <summary>
        /// 创建SIP MESSAGE请求
        /// </summary>
        private SIPRequest CreateSIPMessageRequest(string deviceId, string deviceIp, int devicePort, string body)
        {
            var toUri = SIPURI.ParseSIPURI($"{deviceId}@{deviceIp}:{devicePort}");
            var fromUri = SIPURI.ParseSIPURI($"{_serverId}@{_serverIp}:{_sipPort}");

            var request = new SIPRequest(SIPMethodsEnum.MESSAGE, toUri);
            request.Header.From = new SIPFromHeader(_serverId, fromUri, Guid.NewGuid().ToString());
            request.Header.To = new SIPToHeader(_serverId, toUri, null);
            request.Header.CallId = Guid.NewGuid().ToString();

            // 正确构造CSeq（线程安全自增）
            int currentCseq;
            lock (_cseqLock)
            {
                currentCseq = _cseqCounter++;
                // 防止溢出，重置为1
                if (_cseqCounter > int.MaxValue - 1000)
                {
                    _cseqCounter = 1;
                }
            }
            request.Header.CSeq = _cseqCounter;
            request.Header.MaxForwards = 70;
            request.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(_serverId, SIPURI.ParseSIPURI($"{_serverId}@{_serverIp}:{_sipPort}"))
            };
            request.Body = body;
            request.Header.ContentType = "application/xml";

            return request;
        }
        #endregion

        #region 事件触发方法

        protected virtual async Task OnDeviceRegistered(DeviceRegisteredEventArgs e)
        {
            _deviceStorage.SaveDevice(e.Device);
            DeviceRegistered?.Invoke(this, e);
        }


        protected virtual async Task OnDeviceOffline(DeviceOfflineEventArgs e)
        {
            _deviceStorage.RemoveDevice(e.DeviceId);
            DeviceOffline?.Invoke(this, e);
        }

        protected virtual async Task OnCatalogReceived(CatalogReceivedEventArgs e)
        {
            _deviceStorage.SaveChannels(e.Channels);
            CatalogReceived?.Invoke(this, e);
        }

        protected virtual async Task OnAlarmReceived(AlarmReceivedEventArgs e)
        {
            AlarmReceived?.Invoke(this, e);
        }

        protected virtual async Task OnStreamPlayed(StreamPlayEventArgs e)
        {
            StreamPlayed?.Invoke(this, e);
        }

        #endregion

        #region 资源释放
        public void Dispose()
        {
            _heartbeatTimer.Stop();
            _heartbeatTimer.Dispose();

            _sipTransport.Shutdown();
            _sipTransport.Dispose();

            Console.WriteLine("[GB28181 Server] 已停止");
        }
        #endregion
    }
}
