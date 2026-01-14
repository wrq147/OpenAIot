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
using System.Text;
using System.Threading.Channels;
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
        public event Func<object?, AlarmReceivedEventArgs, Task> AlarmReceived;
        public event Func<object?, StreamPlayEventArgs, Task> StreamPlayed;
        public event Func<object?, PresetListReceivedEventArgs, Task> PresetListReceived;
        public event Func<object?, PTZEventOkArgs, Task> PTZEventOk;
        #endregion

        // 核心字段
        private readonly SIPTransport _sipTransport;
        private readonly IDeviceStorage _deviceStorage;
        private readonly ConcurrentDictionary<string, DateTime> _heartbeatMap = new ConcurrentDictionary<string, DateTime>();
        private readonly System.Timers.Timer _heartbeatTimer;
        private readonly GB28181Version _protocolVersion;
        private readonly string _serverId;
        private readonly int _sipPort;
        private readonly string _serverIp;
        private readonly int _heartbeatTimeout = 100;
        // 传输协议（枚举类型）
        private readonly SIPTransportProtocol _transportProtocol;

        // 用于存储主动请求的会话信息，便于响应匹配
        private readonly ConcurrentDictionary<string, RequestContext> _requestContextMap = new ConcurrentDictionary<string, RequestContext>();



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
                             GB28181Version protocolVersion, IDeviceStorage deviceStorage,
                             SIPTransportProtocol transportProtocol = SIPTransportProtocol.Both)
        {
            _serverIp = serverIp ?? throw new ArgumentNullException(nameof(serverIp));
            _sipPort = sipPort;
            _serverId = serverId ?? throw new ArgumentNullException(nameof(serverId));
            _protocolVersion = protocolVersion;
            _deviceStorage = deviceStorage ?? throw new ArgumentNullException(nameof(deviceStorage));
            _transportProtocol = transportProtocol;

            _sipTransport = new SIPTransport();
            var localEP = new IPEndPoint(IPAddress.Any, _sipPort);

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
            // 注册SIP响应处理器
            _sipTransport.SIPTransportResponseReceived += OnSIPResponseReceived;

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
                Console.WriteLine($"心跳超时：{_heartbeatTimeout}秒");
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


                switch (req.Method)
                {
                    case SIPMethodsEnum.REGISTER:
                        await HandleRegister(req, remoteEP);
                        break;
                    case SIPMethodsEnum.MESSAGE:
                        await HandleMessage(req, remoteEP);
                        break;
                    case SIPMethodsEnum.INVITE:
                        await HandleInvite(req, remoteEP);
                        break;
                    case SIPMethodsEnum.BYE:
                        await HandleBye(req, remoteEP);
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

        /// <summary>
        /// 处理所有SIP响应
        /// </summary>
        private async Task OnSIPResponseReceived(SIPEndPoint localEP, SIPEndPoint remoteEP, SIPResponse resp)
        {
            try
            {
                // 输出响应的传输协议类型（UDP/TCP）
                var transportType = localEP.Protocol == SIPProtocolsEnum.udp ? "UDP" : "TCP";
                Console.WriteLine($"[SIP响应] {resp.Status} {resp.ReasonPhrase} | CallID: {resp.Header.CallId} | 传输协议：{transportType}");

                // 根据CallId查找对应的请求上下文
                if (_requestContextMap.TryGetValue(resp.Header.CallId, out var requestContext))
                {
                    switch (requestContext.RequestType)
                    {
                        case nameof(SIPMethodsEnum.INVITE):
                            await HandleInviteResponse(resp, remoteEP, requestContext);
                            break;
                        case nameof(SIPMethodsEnum.MESSAGE):
                            await HandleMessageResponse(resp, remoteEP, requestContext);
                            break;
                        case nameof(SIPMethodsEnum.BYE):
                            await HandleByeResponse(resp, remoteEP, requestContext);
                            break;
                        default:
                            Console.WriteLine($"[SIP响应] 未处理的响应类型: {requestContext.RequestType}");
                            break;
                    }


                }
                else
                {
                    // 处理没有上下文的响应（如设备主动发起的INVITE响应）
                    if (resp.Header.CSeqMethod == SIPMethodsEnum.INVITE)
                    {
                        await HandleUncontextualInviteResponse(resp, remoteEP);
                    }
                    else
                    {
                        Console.WriteLine($"[SIP响应] 未找到对应的请求上下文 CallID: {resp.Header.CallId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理SIP响应失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 处理INVITE请求的响应（主动拉流）
        /// </summary>
        private async Task HandleInviteResponse(SIPResponse resp, SIPEndPoint remoteEP, RequestContext requestContext)
        {
            var channelId = requestContext.ChannelId;
            var deviceId = requestContext.DeviceId;

            var device = _deviceStorage.GetDevice(deviceId);
            if (device == null)
            {
                return;
            }
            var channelList = _deviceStorage.GetChannelsByDeviceId(deviceId);
            var channelInfo = channelList.Where(x => x.ChannelId == channelId).FirstOrDefault();
            if (channelInfo == null)
            {
                return;
            }
            try
            {
                if (resp.Status == SIPResponseStatusCodesEnum.Ok)
                {
                    // 解析设备返回的SDP
                    if (!string.IsNullOrEmpty(resp.Body))
                    {
                        var sdp = SDP.ParseSDPDescription(resp.Body);
                        var media = sdp.Media.FirstOrDefault();
                        if (media != null)
                        {
                            Console.WriteLine($"[主动拉流成功] 设备={deviceId} 通道={channelId} 远程RTP端口={media.Port}");

                            // 更新通道会话状态
                            channelInfo.SessionStatus = StreamState.Playing;
                            channelInfo.RemoteRtpPort = media.Port;

                            // 触发点播成功事件
                            await OnStreamPlayed(new StreamPlayEventArgs
                            {
                                Params = new PlaybackParams
                                {
                                    ChannelId = channelId,
                                    DeviceId = deviceId,
                                    RemoteIp = remoteEP.Address.ToString(),
                                    RemoteRtpPort = media.Port,
                                    Ssrc = channelInfo.Ssrc,
                                    IsLive = true
                                },
                                IsSuccess = true,
                                SessionId = resp.Header.CallId,
                                Message = "主动拉流成功"
                            });

                            var ackReq = CreateAckRequest(resp, remoteEP);
                            var dstEnd = new SIPEndPoint(device.TransportProtocol, IPAddress.Parse(device.DeviceIp), device.DevicePort);
                            await _sipTransport.SendRequestAsync(dstEnd, ackReq);
                        }
                    }
                }
                else if ((int)resp.Status >= 400)
                {
                    // 拉流失败
                    Console.WriteLine($"[主动拉流失败] 设备={deviceId} 通道={channelId} 状态码={resp.Status} 原因={resp.ReasonPhrase}");

                    // 更新通道会话状态
                    channelInfo.SessionStatus = StreamState.Failed;

                    // 触发点播失败事件
                    await OnStreamPlayed(new StreamPlayEventArgs
                    {
                        Params = new PlaybackParams { ChannelId = channelId, DeviceId = deviceId, Ssrc = channelInfo.Ssrc },
                        IsSuccess = false,
                        SessionId = resp.Header.CallId,
                        Message = $"主动拉流失败: {resp.ReasonPhrase}"
                    });
                }
                else if (resp.Status == SIPResponseStatusCodesEnum.Trying || resp.Status == SIPResponseStatusCodesEnum.Ringing)
                {
                    // 临时响应，仅记录日志
                    Console.WriteLine($"[主动拉流中] 设备={deviceId} 通道={channelId} 状态码={resp.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理INVITE响应失败：设备={deviceId} 通道={channelId} 错误={ex.Message}");
            }
            finally
            {
                // 对于最终响应（2xx/4xx/5xx/6xx），清理上下文
                if ((int)resp.Status >= 200)
                {
                    _requestContextMap.TryRemove(resp.Header.CallId, out _);
                }
            }
        }

        /// <summary>
        /// 处理没有上下文的INVITE响应（设备主动推流）
        /// </summary>
        private async Task HandleUncontextualInviteResponse(SIPResponse resp, SIPEndPoint remoteEP)
        {
            try
            {
                if (resp.Status == SIPResponseStatusCodesEnum.Ok && !string.IsNullOrEmpty(resp.Body))
                {
                    Console.WriteLine($"[设备主动推流响应] {remoteEP} SDP={resp.Body.Substring(0, Math.Min(100, resp.Body.Length))}...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理无上下文INVITE响应失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 处理MESSAGE请求的响应（控制命令/目录查询）
        /// </summary>
        private async Task HandleMessageResponse(SIPResponse resp, SIPEndPoint remoteEP, RequestContext requestContext)
        {
            var deviceId = requestContext.DeviceId;
            var device = _deviceStorage.GetDevice(deviceId);
            if (device == null)
            {
                return;
            }
            try
            {
                switch (requestContext.RequestType)
                {
                    case "PTZControl":
                        {
                            await OnPTZEventOk(new PTZEventOkArgs
                            {
                                DeviceId = deviceId,
                                MessageId = (string)requestContext.ExtraData,
                                IsSuccess = resp.Status == SIPResponseStatusCodesEnum.Ok,
                                Reason = resp.ReasonPhrase
                            });
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理MESSAGE响应失败：设备={deviceId} 错误={ex.Message}");
            }
            finally
            {
                // 清理MESSAGE请求上下文
                _requestContextMap.TryRemove(resp.Header.CallId, out _);
            }
        }

        /// <summary>
        /// 处理BYE请求的响应（停止拉流）
        /// </summary>
        private async Task HandleByeResponse(SIPResponse resp, SIPEndPoint remoteEP, RequestContext requestContext)
        {
            var channelId = requestContext.ChannelId;
            var deviceId = requestContext.DeviceId;
            var device = _deviceStorage.GetDevice(deviceId);
            if (device == null)
            {
                return;
            }
            var channelList = _deviceStorage.GetChannelsByDeviceId(deviceId);
            var channelInfo = channelList.Where(x => x.ChannelId == channelId).FirstOrDefault();
            if (channelInfo == null)
            {
                return;
            }
            try
            {
                if (resp.Status == SIPResponseStatusCodesEnum.Ok)
                {
                    Console.WriteLine($"[停止拉流成功] 设备={deviceId} 通道={channelId}");

                    // 更新通道会话状态
                    channelInfo.SessionStatus = StreamState.Stopped;

                    // 触发停止推流事件
                    await OnStreamPlayed(new StreamPlayEventArgs
                    {
                        Params = new PlaybackParams { ChannelId = channelId, DeviceId = deviceId, Ssrc = channelInfo.Ssrc },
                        IsSuccess = false,
                        SessionId = resp.Header.CallId,
                        Message = "停止拉流成功"
                    });
                }
                else
                {
                    Console.WriteLine($"[停止拉流失败] 设备={deviceId} 通道={channelId} 状态码={resp.Status} 原因={resp.ReasonPhrase}");

                    // 触发停止推流失败事件
                    await OnStreamPlayed(new StreamPlayEventArgs
                    {
                        Params = new PlaybackParams { ChannelId = channelId, DeviceId = deviceId, Ssrc = channelInfo.Ssrc },
                        IsSuccess = false,
                        SessionId = resp.Header.CallId,
                        Message = $"停止拉流失败: {resp.ReasonPhrase}"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理BYE响应失败：设备={deviceId} 通道={channelId} 错误={ex.Message}");
            }
            finally
            {
                // 清理BYE请求上下文
                _requestContextMap.TryRemove(resp.Header.CallId, out _);
            }
        }

        #region 核心业务处理（支持扩展）
        /// <summary>
        /// 通用认证处理
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        private async Task<AuthResult> HandleAuth(SIPRequest req)
        {
            AuthResult rs = new AuthResult();
            rs.IsSuccess = false;
            var deviceId = req.Header.From.FromURI.User;
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
                return rs;
            }
            else
            {
                // 有Authorization头，验证密码
                var authHeader = req.Header.AuthenticationHeaders[0];
                var response = authHeader.SIPDigest.Response;

                var curDevice = _deviceStorage.GetDevice(deviceId);
                string tpassword;
                if (curDevice == null || string.IsNullOrEmpty(curDevice.Password))
                {
                    tpassword = await _deviceStorage.GetDevicePassword(deviceId);
                }
                else
                {
                    tpassword = curDevice.Password;
                }
                rs.Password = tpassword;
                if (string.IsNullOrEmpty(tpassword))
                {
                    var unauthorizedResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Unauthorised, "The password cannot be empty");
                    await _sipTransport.SendResponseAsync(unauthorizedResp);
                    Console.WriteLine($"[认证失败] {deviceId}：密码不能为空");
                    return rs;
                }
                // 计算期望的认证摘要（与设备侧算法一致）
                var sipExpectedDigest = authHeader.SIPDigest.CopyOf();
                sipExpectedDigest.Password = tpassword;
                sipExpectedDigest.RequestType = req.Method.ToString();
                var expectedResponse = sipExpectedDigest.GetDigest();

                // 验证摘要是否匹配
                if (response != expectedResponse)
                {
                    var unauthorizedResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Unauthorised, "Password error");
                    await _sipTransport.SendResponseAsync(unauthorizedResp);
                    Console.WriteLine($"[认证失败] {deviceId}：密码验证失败");
                    return rs;
                }
                rs.IsSuccess = true;
                return rs;
            }
        }
        /// <summary>
        /// 处理设备注册
        /// </summary>
        private async Task HandleRegister(SIPRequest req, SIPEndPoint remoteEP)
        {
            var deviceId = req.Header.From.FromURI.User;
            try
            {
                bool isUnregisterRequest = req.Header.Expires == 0 ||
                              (req.Header.Contact != null && req.Header.Contact.Count > 0 && req.Header.Contact[0].Expires == 0);

                if (isUnregisterRequest)
                {
                    await HandleDeviceUnregister(deviceId, remoteEP, req);
                    return;
                }

                var authRs = await HandleAuth(req);
                if (!authRs.IsSuccess)
                {
                    return;
                }
                if (req.Header.Vias.Via.Count == 0)
                {
                    return;
                }
                // 密码验证通过，完成注册流程
                var deviceInfo = new DeviceInfo
                {
                    DeviceId = deviceId,
                    Password = authRs.Password,
                    DeviceIp = remoteEP.Address.ToString(),
                    DevicePort = remoteEP.Port,
                    RegisterTime = DateTime.Now,
                    LastHeartbeatTime = DateTime.Now,
                    ProtocolVersion = _protocolVersion,
                    TransportProtocol = remoteEP.Protocol,
                };

                if (req.Header.Contact != null && req.Header.Contact.Count > 0)
                {
                    deviceInfo.DevicePort = Convert.ToInt32(req.Header.Contact[0].ContactURI.HostPort);
                }

                _heartbeatMap.AddOrUpdate(deviceId, DateTime.Now, (key, oldValue) => DateTime.Now);

                var okResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
                okResp.Header.Expires = _protocolVersion == GB28181Version.V2016 ? 3600 : 7200;
                okResp.Header.UserAgent = "X-GB28181-Version: " + _protocolVersion.ToString();
                await _sipTransport.SendResponseAsync(okResp);

                await OnDeviceRegistered(new DeviceRegisteredEventArgs
                {
                    Device = deviceInfo,
                    OriginalRequest = req,
                    RemoteEndPoint = remoteEP
                });

                Console.WriteLine($"[注册成功] {deviceId}");

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
        private async Task HandleDeviceUnregister(string deviceId, SIPEndPoint remoteEP, SIPRequest req)
        {
            try
            {
                var authRs = await HandleAuth(req);
                if (!authRs.IsSuccess)
                {
                    return;
                }
                // 回复200 OK确认注销
                var okResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "Unregistered successfully");
                okResp.Header.Expires = 0; // 明确标识注销
                okResp.Header.UserAgent = "X-GB28181-Version: " + _protocolVersion.ToString();
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
        private async Task HandleMessage(SIPRequest req, SIPEndPoint remoteEP)
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
                else if (body.Contains("PresetQuery"))
                {
                    await HandlePresetQueryMessage(deviceId, remoteEP, req);
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
        private async Task HandleKeepaliveMessage(string deviceId, SIPEndPoint remoteEP, SIPRequest req)
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
        private async Task HandlePresetQueryMessage(string deviceId, SIPEndPoint remoteEP, SIPRequest req)
        {
            var device = _deviceStorage.GetDevice(deviceId);
            if (device == null)
            {
                return;
            }
            string snid;
            var presetList = GB28181Util.ParsePresetListXml(req.Body, deviceId, out snid);
            device.PresetList = presetList;
            if (snid != null)
            {
                if (_requestContextMap.TryRemove(snid, out var requestContext))
                {
                    await OnPresetListReceived(new PresetListReceivedEventArgs
                    {
                        DeviceId = deviceId,
                        PresetList = presetList,
                        MessageId = (string)requestContext.ExtraData
                    });
                    return;
                }
            }
            await OnPresetListReceived(new PresetListReceivedEventArgs
            {
                DeviceId = deviceId,
                PresetList = presetList,
                MessageId = null
            });
        }
        /// <summary>
        /// 处理目录消息
        /// </summary>
        private async Task HandleCatalogMessage(string deviceId, SIPEndPoint remoteEP, SIPRequest req)
        {
            var xmlDoc = XDocument.Parse(req.Body);
            var channels = new List<ChannelInfo>();
            var device = _deviceStorage.GetDevice(deviceId);
            if (device == null)
            {
                return;
            }
            var deviceList = xmlDoc.Descendants("DeviceList").FirstOrDefault();
            if (deviceList != null)
            {
                int i = 0;
                foreach (var deviceNode in deviceList.Descendants("Item"))
                {
                    var channel = new ChannelInfo
                    {
                        Index = i,
                        PushKey = device.VideoData.Item.PushKey + "_" + i,
                        DeviceId = deviceId,
                        ChannelId = deviceNode.Element("DeviceID")?.Value ?? string.Empty,
                        ChannelName = deviceNode.Element("Name")?.Value ?? string.Empty,
                        Manufacturer = deviceNode.Element("Manufacturer")?.Value ?? string.Empty,
                        Model = deviceNode.Element("Model")?.Value ?? string.Empty,
                        Status = deviceNode.Element("Status")?.Value ?? "ON",
                        SessionStatus = StreamState.None
                    };

                    channels.Add(channel);

                    ++i;
                }
            }

            var response = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
            await _sipTransport.SendResponseAsync(response);

            // 触发目录事件
            await OnCatalogReceived(new CatalogReceivedEventArgs
            {
                DeviceId = deviceId,
                Channels = channels
            });

            Console.WriteLine($"[目录更新] {deviceId} 共{channels.Count}个通道");
        }

        /// <summary>
        /// 处理报警消息
        /// </summary>
        private async Task HandleAlarmMessage(string deviceId, SIPEndPoint remoteEP, SIPRequest req)
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

            // 触发报警事件
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
        private async Task HandleInvite(SIPRequest req, SIPEndPoint remoteEP)
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
                    IsLive = true
                };

                // 回复100 Trying
                var tryingResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Trying, "Trying");
                await _sipTransport.SendResponseAsync(tryingResp);

                // 生成SDP（通过IMediaHandler扩展）
                var ssrc = GB28181Util.GetPlaySsrc(_serverId);
                var sdpResp = GB28181Util.BuildGB28181SDP(_serverId, _serverIp, playbackParams.RemoteRtpPort, ssrc);

                // 回复200 OK
                var okResp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
                okResp.Body = sdpResp;
                okResp.Header.ContentType = "application/sdp";
                await _sipTransport.SendResponseAsync(okResp);

                // 触发点播事件
                await OnStreamPlayed(new StreamPlayEventArgs
                {
                    Params = playbackParams,
                    IsSuccess = true,
                    SessionId = req.Header.CallId,
                    Message = "点播成功"
                });

                Console.WriteLine($"[点播成功] {channelId} SessionID: {req.Header.CallId} RTP端口：{playbackParams.RemoteRtpPort}");
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
        private async Task HandleBye(SIPRequest req, SIPEndPoint remoteEP)
        {
            var channelId = req.Header.From.FromURI.User;
            var sessionId = req.Header.CallId;
            string devId = channelId.Substring(0, 20);
            var device = _deviceStorage.GetDevice(devId);
            if (device == null)
            {
                return;
            }
            var channelList = _deviceStorage.GetChannelsByDeviceId(devId);
            var channelInfo = channelList.Where(x => x.ChannelId == channelId).FirstOrDefault();
            if (channelInfo == null)
            {
                return;
            }

            var resp = SIPResponse.GetResponse(req, SIPResponseStatusCodesEnum.Ok, "OK");
            await _sipTransport.SendResponseAsync(resp);

            await OnStreamPlayed(new StreamPlayEventArgs
            {
                Params = new PlaybackParams { ChannelId = channelId },
                IsSuccess = false,
                SessionId = sessionId,
                Message = "停止推流成功"
            });

            Console.WriteLine($"[停止推流] {channelId} SessionID: {sessionId} 成功");
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

        #region 扩展功能
        /// <summary>
        /// 获取设备通道的预置位列表
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="messageId">回复的消息Id</param>
        /// <returns>是否发送成功</returns>
        public async Task<bool> GetPresetList(string deviceId, string messageId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceId))
                {
                    throw new ArgumentException("设备ID不能为空");
                }

                var device = _deviceStorage.GetDevice(deviceId);
                if (device == null)
                {
                    Console.WriteLine($"[获取预置位列表] 设备{deviceId}不存在");
                    return false;
                }

                // 生成GB28181标准的预置位查询XML
                string tsnid;
                var presetQueryXml = GB28181Util.GeneratePresetQueryXml(deviceId, out tsnid);

                // 创建SIP MESSAGE请求
                var sipRequest = CreateSIPMessageRequest(
                    deviceId,
                    device.DeviceIp,
                    device.DevicePort,
                    presetQueryXml,
                    device.TransportProtocol);

                // 保存请求上下文（携带预置位查询参数）
                _requestContextMap.TryAdd(tsnid, new RequestContext
                {
                    RequestType = "PresetQuery",
                    DeviceId = deviceId,
                    ChannelId = string.Empty,
                    RequestTime = DateTime.Now,
                    ExtraData = messageId
                });

                // 发送预置位查询请求
                var dstEnd = new SIPEndPoint(device.TransportProtocol, IPAddress.Parse(device.DeviceIp), device.DevicePort);
                await _sipTransport.SendRequestAsync(dstEnd, sipRequest);

                Console.WriteLine($"[获取预置位列表] 请求已发送：设备={deviceId} CallID={sipRequest.Header.CallId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[获取预置位列表] 发送失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 发送PTZ控制命令
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

                var controlXml = GB28181Util.GeneratePTZControlXml(@params);
                var sipRequest = CreateSIPMessageRequest(device.DeviceId, device.DeviceIp, device.DevicePort, controlXml, device.TransportProtocol);

                // 保存请求上下文
                _requestContextMap.TryAdd(sipRequest.Header.CallId, new RequestContext
                {
                    RequestType = "PTZControl",
                    DeviceId = device.DeviceId,
                    ChannelId = @params.ChannelId,
                    RequestTime = DateTime.Now,
                    ExtraData = @params.MessageId
                });

                // 异步发送PTZ控制命令
                var dstEnd = new SIPEndPoint(device.TransportProtocol, IPAddress.Parse(device.DeviceIp), device.DevicePort);
                await _sipTransport.SendRequestAsync(dstEnd, sipRequest);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 主动请求设备推流（核心方法）
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="channelId">通道ID</param>
        /// <param name="rtpPort">RTP端口</param>
        /// <returns>是否成功</returns>
        public async Task<bool> StartActiveStream(string deviceId, string channelId, int rtpPort)
        {
            // 获取设备信息
            var device = _deviceStorage.GetDevice(deviceId);
            if (device == null)
            {
                return false;
            }

            var channelList = _deviceStorage.GetChannelsByDeviceId(deviceId);
            var channelInfo = channelList.Where(x => x.ChannelId == channelId).FirstOrDefault();
            if (channelInfo == null)
            {
                return false;
            }

            string sessionId = Guid.NewGuid().ToString("N");
            // 构造符合GB28181标准的SDP
            var ssrc = GB28181Util.GetPlaySsrc(_serverId);
            var sdp = GB28181Util.BuildGB28181SDP(_serverId, _serverIp, rtpPort, ssrc);
            channelInfo.InviteTime = DateTime.Now;
            channelInfo.SessionStatus = StreamState.Inviting;
            channelInfo.Ssrc = ssrc;
            // 构造SIP INVITE请求
            var inviteRequest = CreateInviteRequest(device, channelInfo, sessionId, sdp, ssrc);

            // 保存请求上下文
            _requestContextMap.TryAdd(sessionId, new RequestContext
            {
                RequestType = nameof(SIPMethodsEnum.INVITE),
                DeviceId = deviceId,
                ChannelId = channelId,
                RequestTime = DateTime.Now
            });

            // 发送INVITE请求到设备
            try
            {
                var dstEnd = new SIPEndPoint(device.TransportProtocol, IPAddress.Parse(device.DeviceIp), device.DevicePort);
                await _sipTransport.SendRequestAsync(dstEnd, inviteRequest);
                Console.WriteLine($"[主动拉流] INVITE已发送：设备={deviceId} 通道={channelId} 会话={sessionId} RTP端口={rtpPort}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送失败：{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 停止主动拉流
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task<bool> StopActiveStream(string deviceId, string channelId)
        {
            // 获取设备信息
            var device = _deviceStorage.GetDevice(deviceId);
            if (device == null)
            {
                return false;
            }
            var channelList = _deviceStorage.GetChannelsByDeviceId(deviceId);
            var channelInfo = channelList.Where(x => x.ChannelId == channelId).FirstOrDefault();
            if (channelInfo == null)
            {
                return false;
            }
            string sessionId = Guid.NewGuid().ToString("N");
            // 构造BYE请求
            var toUri = new SIPURI(channelId, $"{device.DeviceIp}:{device.DevicePort}", null, SIPSchemesEnum.sip);
            var fromUri = new SIPURI(_serverId, $"{_serverIp}:{_sipPort}", null, SIPSchemesEnum.sip);
            var byeRequest = new SIPRequest(SIPMethodsEnum.BYE, toUri);
            byeRequest.Header = new SIPHeader();
            string viaBranch = $"z9hG4bK-{Guid.NewGuid():N}";
            var viaHeader = new SIPViaHeader(_serverIp, _sipPort, viaBranch, device.TransportProtocol);
            byeRequest.Header.Vias.Via.Add(viaHeader);
            byeRequest.Header.CallId = sessionId;
            byeRequest.Header.From = new SIPFromHeader(null, fromUri, sessionId);
            byeRequest.Header.To = new SIPToHeader(null, toUri, null);
            byeRequest.Header.CSeq = GB28181Util.GenerateCSeq();
            byeRequest.Header.CSeqMethod = SIPMethodsEnum.BYE;
            byeRequest.Header.MaxForwards = 70;

            // 保存请求上下文
            _requestContextMap.TryAdd(sessionId, new RequestContext
            {
                RequestType = nameof(SIPMethodsEnum.BYE),
                DeviceId = deviceId,
                ChannelId = channelId,
                RequestTime = DateTime.Now
            });

            // 发送BYE请求
            try
            {
                var dstEnd = new SIPEndPoint(device.TransportProtocol, IPAddress.Parse(device.DeviceIp), device.DevicePort);
                await _sipTransport.SendRequestAsync(dstEnd, byeRequest);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送失败：{ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// 发送目录查询请求到设备（核心方法）
        /// </summary>
        /// <param name="device">设备信息</param>
        public async Task SendCatalogQuery(DeviceInfo device)
        {
            // 1. 构造符合GB28181标准的Catalog查询XML
            var catalogXml = GB28181Util.GenerateCatalogQueryXml(device.DeviceId, _protocolVersion);

            // 2. 创建SIP MESSAGE请求
            var sipRequest = CreateSIPMessageRequest(
                device.DeviceId,
                device.DeviceIp,
                device.DevicePort,
                catalogXml, device.TransportProtocol);

            //// 保存请求上下文
            //_requestContextMap.TryAdd(sipRequest.Header.CallId, new RequestContext
            //{
            //    RequestType = "Catalog",
            //    DeviceId = device.DeviceId,
            //    ChannelId = string.Empty,
            //    RequestTime = DateTime.Now
            //});

            // 3. 异步发送请求
            var dstEnd = new SIPEndPoint(device.TransportProtocol, IPAddress.Parse(device.DeviceIp), device.DevicePort);
            await _sipTransport.SendRequestAsync(dstEnd, sipRequest);
        }
        /// <summary>
        /// 创建SIP MESSAGE请求
        /// </summary>
        private SIPRequest CreateSIPMessageRequest(string deviceId, string deviceIp, int devicePort, string body, SIPProtocolsEnum protocol)
        {
            var toUri = new SIPURI(deviceId, $"{deviceIp}:{devicePort}", null, SIPSchemesEnum.sip);
            var fromUri = new SIPURI(_serverId, $"{_serverIp}:{_sipPort}", null, SIPSchemesEnum.sip);

            string randTag = Guid.NewGuid().ToString("N");
            string callId = Guid.NewGuid().ToString("N");
            string viaBranch = $"z9hG4bK-{Guid.NewGuid():N}";

            var request = new SIPRequest(SIPMethodsEnum.MESSAGE, toUri);
            request.Header = new SIPHeader();
            var viaHeader = new SIPViaHeader(_serverIp, _sipPort, viaBranch, protocol);
            request.Header.Vias.Via.Add(viaHeader);
            request.Header.From = new SIPFromHeader(null, fromUri, randTag);
            request.Header.To = new SIPToHeader(null, toUri, null);
            request.Header.CallId = callId;
            request.Header.CSeq = GB28181Util.GenerateCSeq();
            request.Header.CSeqMethod = SIPMethodsEnum.MESSAGE;
            request.Header.MaxForwards = 70;
            request.Header.UserAgent = $"X-GB28181-Version: {_protocolVersion}";
            request.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromUri)
            };
            request.Body = body;
            request.Header.ContentType = "Application/MANSCDP+xml";
            request.Header.ContentLength = request.BodyBuffer.Length;
            return request;
        }

        /// <summary>
        /// 构造INVITE请求
        /// </summary>
        private SIPRequest CreateInviteRequest(DeviceInfo device, ChannelInfo channel, string sessionId, string sdp, string ssrc)
        {
            // 请求URI：通道ID@设备IP:端口
            var toUri = new SIPURI(channel.ChannelId, $"{device.DeviceIp}:{device.DevicePort}", null, SIPSchemesEnum.sip);
            var fromUri = new SIPURI(_serverId, $"{_serverIp}:{_sipPort}", null, SIPSchemesEnum.sip);
            var inviteRequest = new SIPRequest(SIPMethodsEnum.INVITE, toUri);
            // SIP头域
            inviteRequest.Header = new SIPHeader();
            inviteRequest.Header.CallId = sessionId;
            inviteRequest.Header.From = new SIPFromHeader(null, fromUri, sessionId);
            inviteRequest.Header.To = new SIPToHeader(null, toUri, null);
            inviteRequest.Header.CSeq = GB28181Util.GenerateCSeq();
            inviteRequest.Header.CSeqMethod = SIPMethodsEnum.INVITE;
            inviteRequest.Header.MaxForwards = 70;
            string viaBranch = $"z9hG4bK-{Guid.NewGuid():N}";
            var viaHeader = new SIPViaHeader(_serverIp, _sipPort, viaBranch, device.TransportProtocol);
            inviteRequest.Header.Vias.Via.Add(viaHeader);
            inviteRequest.Header.Contact = new List<SIPContactHeader>()
            {
                new SIPContactHeader(null, fromUri)
            };
            inviteRequest.Header.Subject = $"{channel.ChannelId}:{ssrc},{_serverId}:0";

            // 携带SDP
            inviteRequest.Body = sdp;
            inviteRequest.Header.ContentType = "APPLICATION/SDP";
            inviteRequest.Header.ContentLength = inviteRequest.BodyBuffer.Length;
            inviteRequest.Header.UserAgent = $"X-GB28181-Version: {_protocolVersion}";
            return inviteRequest;
        }
        /// <summary>
        /// 构造ACK请求
        /// </summary>
        /// <param name="inviteResp">INVITE响应</param>
        /// <param name="remoteEP">设备端点</param>
        private SIPRequest CreateAckRequest(SIPResponse inviteResp, SIPEndPoint remoteEP)
        {
            // 1. 构造ACK请求，必须复用INVITE响应的核心头域
            var ackRequest = new SIPRequest(SIPMethodsEnum.ACK, inviteResp.Header.To.ToURI);
            ackRequest.Header = new SIPHeader();
            ackRequest.Header.CallId = inviteResp.Header.CallId;
            ackRequest.Header.From = inviteResp.Header.From;
            ackRequest.Header.To = inviteResp.Header.To;
            ackRequest.Header.CSeq = inviteResp.Header.CSeq;
            ackRequest.Header.CSeqMethod = SIPMethodsEnum.ACK;

            // 2. 构造Via头（和INVITE请求保持一致）
            string viaBranch = $"z9hG4bK-{Guid.NewGuid():N}";
            var viaHeader = new SIPViaHeader(_serverIp, _sipPort, viaBranch, remoteEP.Protocol);
            ackRequest.Header.Vias.Via.Add(viaHeader);

            // 3. 其他必要头域
            ackRequest.Header.MaxForwards = 70;
            ackRequest.Header.UserAgent = $"X-GB28181-Version: {_protocolVersion}";

            return ackRequest;
        }
        #endregion

        #region 事件触发方法

        protected virtual async Task OnDeviceRegistered(DeviceRegisteredEventArgs e)
        {
            _deviceStorage.SaveDevice(e.Device);
            if (DeviceRegistered != null)
            {
                await DeviceRegistered(this, e);
            }
        }


        protected virtual async Task OnDeviceOffline(DeviceOfflineEventArgs e)
        {
            if (DeviceOffline != null)
            {
                await DeviceOffline(this, e);
            }
            _deviceStorage.RemoveDevice(e.DeviceId);
        }

        protected virtual async Task OnCatalogReceived(CatalogReceivedEventArgs e)
        {
            await _deviceStorage.SaveChannels(e.DeviceId, e.Channels);
        }

        protected virtual async Task OnAlarmReceived(AlarmReceivedEventArgs e)
        {
            if (AlarmReceived != null)
            {
                await AlarmReceived(this, e);
            }
        }

        protected virtual async Task OnStreamPlayed(StreamPlayEventArgs e)
        {
            if (!e.IsSuccess && !string.IsNullOrEmpty(e.Params.Ssrc))
            {
                GB28181Util.ReleaseSsrc(e.Params.Ssrc);
            }
            if (StreamPlayed != null)
            {
                await StreamPlayed(this, e);
            }
        }

        protected virtual async Task OnPresetListReceived(PresetListReceivedEventArgs e)
        {
            if (PresetListReceived != null)
            {
                await PresetListReceived(this, e);
            }
        }
        protected virtual async Task OnPTZEventOk(PTZEventOkArgs e)
        {
            if (PTZEventOk != null)
            {
                await PTZEventOk(this, e);
            }
        }

        #endregion

        #region 资源释放
        public void Dispose()
        {
            _heartbeatTimer.Stop();
            _heartbeatTimer.Dispose();

            _sipTransport.Shutdown();
            _sipTransport.Dispose();

            // 清理请求上下文
            _requestContextMap.Clear();

            Console.WriteLine("[GB28181 Server] 已停止");
        }
        #endregion
    }

}