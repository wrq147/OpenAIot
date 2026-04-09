using ChannelUtility;
using ChannelUtility.Buffers;
using ChannelUtility.Config;
using ChannelUtility.Message;
using ChannelUtility.Redis;
using FastTunnel.Core.Client;
using FastTunnel.Core.Config;
using FastTunnel.Core.Handlers.Client;
using FastTunnel.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusChannel
{
    public class RemoteTask
    {
        private string _machineId;
        private Task _mainTask;
        private bool _isAborted;
        private IMqttClient _client;
        private ModbusOption _option;
        private string[] _urls = new string[] { "125.124.98.180", "8.8.8.8", "iot.wookongcloud.com" };
        private long _connectTestTime = 0;
        private MqttClientFactory _factory = new MqttClientFactory();
        private CustomFastTunnelClient _fastTunnelClient;
        private IServiceProvider _provider;

        private Task _tunnelTask;
        public RemoteTask(IServiceProvider provider, ModbusOption option, string machineId)
        {
            _option = option;
            _machineId = machineId;
            _provider = provider;
        }
        public async Task Start(CancellationToken stoppingToken)
        {
            _mainTask = Task.Run(async () =>
            {
                while (!_isAborted)
                {
                    try
                    {
                        await Exe(stoppingToken);
                    }
                    catch (ThreadAbortException)
                    {
                        _isAborted = true;
                    }
                    catch (ThreadInterruptedException)
                    {
                        _isAborted = true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });

        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            _isAborted = true;

            if (_client != null)
            {
                await _client.DisconnectAsync();
                _client.Dispose();
                _client = null;
            }
            if (_process != null)
            {
                _process.Dispose();
                _process = null;
            }
            if (_fastTunnelClient != null)
            {
                await _fastTunnelClient.StopAsync(cancellationToken);
            }
            _tunnelTask = null;
        }
        private long _upallmsgTime = 0;
        private int _uptime = 0;
        private async Task Exe(CancellationToken stoppingToken)
        {
            long curll = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            if (_connectTestTime == 0)
            {
                _connectTestTime = curll - 1000000;
            }
            else
            {
                if (_client != null)
                {
                    if (!_client.IsConnected)
                    {
                        try
                        {
                            _client.Dispose();
                        }
                        catch { }
                        _client = null;
                        await _fastTunnelClient.StopAsync(stoppingToken);
                    }
                }
                var curdelta = (curll - _connectTestTime) / 1000;
                if (curdelta > 20)
                {
                    //每20秒尝试创建远程控制
                    if (!string.IsNullOrEmpty(_option.server_ip))
                    {
                        if (_client == null)
                        {
                            bool isnet = TestNet();
                            if (isnet)
                            {
                                await CreateRemote(stoppingToken);
                                //初始化上传间隔
                                var upmsgdetal = await _provider.GetService<GeneralRedisHelper>().StringGetAsync<string>("ModbusChannelUpMsgDeta");
                                if (!string.IsNullOrEmpty(upmsgdetal))
                                {
                                    _uptime = Convert.ToInt32(upmsgdetal);
                                }
                            }
                        }
                    }

                    _connectTestTime = curll;
                }
            }

            if (_upallmsgTime == 0)
            {
                _upallmsgTime = curll - 1000000;
            }
            else
            {
                if (_client != null)
                {
                    var curdelta = (curll - _upallmsgTime) / 1000;
                    if (_uptime > 0 && curdelta > _uptime)
                    {
                        await ExeSysCmd("$readprops all");
                        _upallmsgTime = curll;
                    }
                }
            }
            await Task.Delay(100);
        }
        private bool TestNet()
        {
            Ping ping = new Ping();
            for (int i = 0; i < _urls.Length; i++)
            {
                try
                {
                    PingReply pingStatus = ping.Send(_urls[i], 500);
                    if (pingStatus.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                catch { }

            }
            return false;
        }

        private async Task UpMsg(string msg)
        {
            var dates = Encoding.UTF8.GetBytes(msg);
            await _client.PublishBinaryAsync("wukong/up/" + _machineId, dates);
        }
        private async Task ExeSysCmd(string ss)
        {
            if (_client == null || !_client.IsConnected)
            {
                return;
            }
            if (ss.StartsWith("$upui"))
            {
                //$upui 下载路径  替换路径
                try
                {
                    await UpMsg("开始更新程序");
                    string[] uicmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    if (uicmds.Length < 2)
                    {
                        await UpMsg("命令需要传入参数");
                        return;
                    }
                    string localPath = "/tmp/tmpdown";
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }
                    localPath = localPath + Path.DirectorySeparatorChar + System.IO.Path.GetFileName(uicmds[1]);
                    if (File.Exists(localPath))
                    {
                        File.Delete(localPath);
                    }
                    await UpMsg("文件下载中...");
                    using (WebClient client = new WebClient())
                    {
                        await client.DownloadFileTaskAsync(new Uri(uicmds[1]), localPath);
                    }
                    await UpMsg("下载完成，正在更新程序");
                    ServerDownCmd("skill AirJointUI.Desk");
                    await Task.Delay(1000);
                    ServerDownCmd("skill myauto");
                    await Task.Delay(1000);
                    ServerDownCmd("skill App");
                    await Task.Delay(1000);

                    string targetPath = uicmds[2];
                    using (ZipArchive archive = ZipFile.OpenRead(localPath))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string updatefile = Path.Combine(targetPath, entry.FullName);
                            entry.ExtractToFile(updatefile, true);
                        }
                    }
                    if (File.Exists(localPath))
                    {
                        File.Delete(localPath);
                    }
                    await UpMsg("程序更新完成，正在重启");
                    ServerDownCmd("sudo reboot");
                }
                catch (Exception ex)
                {
                    await UpMsg("更新异常：" + ex.Message + ex.StackTrace);
                }
            }
            else if (ss.StartsWith("$kill"))
            {
                if (_process != null)
                {
                    _process.Kill();
                    _process.Dispose();
                    _process = null;
                }
            }
            else if (ss.StartsWith("$version"))
            {
                string[] uicmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (uicmds.Length < 2)
                {
                    await UpMsg("命令需要传入参数");
                    return;
                }
                string fileversion = File.ReadAllText(uicmds[1]);
                await UpMsg($"当前UI程序版本为：{fileversion}");
            }
            else if (ss.StartsWith("$downfile"))
            {
                string[] uicmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (uicmds.Length < 2)
                {
                    await UpMsg("命令需要传入参数");
                    return;
                }
                await UpMsg("开始下载" + Path.GetFileName(uicmds[1]));
                var tmpbytes = await File.ReadAllBytesAsync(uicmds[1]);
                await _client.PublishBinaryAsync("wukong/up/" + _machineId, tmpbytes);
                await UpMsg("下载完成");
            }
            else if (ss.StartsWith("$readprops"))
            {
                try
                {
                    //读属性到云端
                    string[] uicmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    string dtuid = uicmds[1];
                    var redis = _provider.GetService<GeneralRedisHelper>();
                    Dictionary<string, DevicePropertyValue> alldict = new Dictionary<string, DevicePropertyValue>();
                    if (dtuid == "all")
                    {
                        List<string> keylist = await redis.KeysAsync("Device:*");
                        foreach (string k in keylist)
                        {
                            string tmpdtuid = k.Substring(7);
                            var redisdict = await redis.HashGetAllAsync<string>("Device:" + tmpdtuid);
                            if (tmpdtuid == "wky111" || tmpdtuid == "wky222" || tmpdtuid == "wky333")
                            {
                                if (redisdict.Keys.Any(x => x.Contains("RunState")))
                                {
                                    continue;
                                }
                            }
                            if (redisdict != null && redisdict.Count > 0)
                            {
                                var dict = DevicePropertyValue.FromDictStr(redisdict);
                                foreach (var pp in dict)
                                {
                                    if (!pp.Key.StartsWith("$"))
                                    {
                                        alldict.Add(tmpdtuid + "_" + pp.Key, pp.Value);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var redisdict = await redis.HashGetAllAsync<string>("Device:" + dtuid);
                        if (redisdict != null && redisdict.Count > 0)
                        {
                            var dict = DevicePropertyValue.FromDictStr(redisdict);
                            foreach (var pp in dict)
                            {
                                if (!pp.Key.StartsWith("$"))
                                {
                                    alldict.Add(dtuid + "_" + pp.Key, pp.Value);
                                }
                            }
                        }
                    }

                    await UpMsg("$readprops " + System.Text.Json.JsonSerializer.Serialize(alldict, JsonMessageSerializerConfig.SerializeOptions));
                }
                catch(Exception ex)
                {
                    await UpMsg("更新异常：" + ex.Message + ex.StackTrace);
                }
               
            }
            else if (ss.StartsWith("$exefunc"))
            {
                //云端下发执行功能
                string[] uicmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                string dtuId = uicmds[1];
                bool ishex = Convert.ToBoolean(uicmds[2]);
                string cmdstr = uicmds[3];
                RawDataMessage rawdata = new RawDataMessage();
                rawdata.MessageId = Guid.NewGuid().ToString("N");
                if (ishex)
                {
                    rawdata.Data = FastBufferHelper.StrToToHex(cmdstr);
                }
                else
                {
                    rawdata.Data = Encoding.UTF8.GetBytes(cmdstr);
                }

                var eventBus = _provider.GetService<ClientBusProxy>();
                var tsl = await eventBus.GetTsl(null, dtuId);
                rawdata.DeviceId = dtuId;
                rawdata.ProductId = tsl.ProductId;
                await eventBus.DownRequestMessage(rawdata);
                await UpMsg("$exefunc success");
            }
            else if (ss.StartsWith("$setloop"))
            {
                string[] uicmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                await _provider.GetService<GeneralRedisHelper>().StringSetAsync<string>("ModbusChannelUpMsgDeta", uicmds[1]);
            }
            else if (ss.StartsWith("$getrules"))
            {
                try
                {
                    var ret = HttpApi.Instance.GetRuleList();
                    await UpMsg("$getrules " + ret);
                }
                catch (Exception ex)
                {
                    await UpMsg("$setrule {\"code\":999}");
                }
            }
            else if (ss.StartsWith("$setrule"))
            {
                try
                {
                    string[] uicmds = ss.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    long tid = Convert.ToInt64(uicmds[1]);
                    bool tbool = Convert.ToBoolean(uicmds[2]);
                    var ret = HttpApi.Instance.EnableRule(tid, tbool);
                    await UpMsg("$setrule " + ret);
                }
                catch(Exception ex)
                {
                    await UpMsg("$setrule {\"code\":999}");
                }
            }
        }
        private async Task MqttClient_MessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                //执行系统命令
                string ss = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                if (ss.StartsWith("$"))
                {
                    await ExeSysCmd(ss);
                }
                else
                {
                    ServerDownCmd(ss);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private Process _process = null;
        private ConcurrentQueue<string> _outputStrs = new ConcurrentQueue<string>();
        private int inTimer = 0;
        private async Task ExeUpDate()
        {
            bool needUp = false;
            if (Interlocked.Exchange(ref inTimer, 1) == 0)
            {
                await Task.Delay(100);
                StringBuilder tmpsb = new StringBuilder();
                while (_outputStrs.TryDequeue(out var buffer))
                {
                    tmpsb.Append(buffer + "\n");
                }
                await UpMsg(tmpsb.ToString());
                if (_outputStrs.Count > 0)
                {
                    needUp = true;
                }
                Interlocked.Exchange(ref inTimer, 0);
            }

            if (needUp)
            {
                Task tmp = ExeUpDate();
            }
        }
        private void ServerDownCmd(string command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (_process == null)
                {
                    _process = new Process();//进程   
                    _process.StartInfo.FileName = "bash";
                    _process.StartInfo.RedirectStandardOutput = true;
                    _process.StartInfo.UseShellExecute = false;
                    _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    _process.StartInfo.CreateNoWindow = true;
                    _process.StartInfo.RedirectStandardInput = true;
                    _process.StartInfo.RedirectStandardError = true;
                    _process.ErrorDataReceived += new DataReceivedEventHandler(async (sender, e) =>
                    {
                        await UpMsg(e.Data);
                    });
                    _process.OutputDataReceived += new DataReceivedEventHandler(async (sender, e) =>
                    {
                        _outputStrs.Enqueue(e.Data);
                        await ExeUpDate();
                    });
                    _process.Start();

                    _process.BeginErrorReadLine();
                    _process.BeginOutputReadLine();
                }

                // 写入命令
                _process.StandardInput.WriteLine(command);
                _process.StandardInput.Flush();
            }
            else
            {
                if (_process == null)
                {
                    _process = new Process();//进程   
                    _process.StartInfo.FileName = "cmd.exe";
                    _process.StartInfo.RedirectStandardOutput = true;
                    _process.StartInfo.UseShellExecute = false;
                    _process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    _process.StartInfo.CreateNoWindow = true;
                    _process.StartInfo.RedirectStandardInput = true;
                    _process.StartInfo.RedirectStandardError = true;
                    _process.ErrorDataReceived += new DataReceivedEventHandler(async (sender, e) =>
                    {
                        await UpMsg(e.Data);
                    });
                    _process.OutputDataReceived += new DataReceivedEventHandler(async (sender, e) =>
                    {
                        _outputStrs.Enqueue(e.Data);
                        await ExeUpDate();
                    });
                    _process.Start();

                    _process.BeginErrorReadLine();
                    _process.BeginOutputReadLine();
                }
                // 写入命令
                _process.StandardInput.WriteLine(command);
                _process.StandardInput.Flush();
            }
        }

        protected async Task TunnelStart(XRemoteInfo tmpServerInfo, CancellationToken stoppingToken)
        {
            //创建远程代理
            DefaultClientConfig clientConfig = new DefaultClientConfig();
            clientConfig.Server = new SuiDaoServer();
            clientConfig.Server.ServerAddr = tmpServerInfo.server_ip;
            clientConfig.Server.ServerPort = tmpServerInfo.server_port;
            clientConfig.Token = tmpServerInfo.server_token;
            clientConfig.Webs = new List<WebConfig>();
            clientConfig.Forwards = new List<ForwardConfig>();

            _fastTunnelClient = new CustomFastTunnelClient(_machineId, tmpServerInfo.server_devkey,
                _provider.GetService<ILogger<FastTunnelClient>>(),
                _provider.GetService<SwapHandler>(),
                       _provider.GetService<LogHandler>(),
                       clientConfig
                );
            await _fastTunnelClient.StartAsync(stoppingToken);
        }
        private async Task CreateRemote(CancellationToken stoppingToken)
        {
            XRemoteInfo tmpServerInfo = new XRemoteInfo();
            tmpServerInfo.server_ip = _option.server_ip;
            tmpServerInfo.server_port = _option.server_port;
            tmpServerInfo.server_token = _option.server_token;
            tmpServerInfo.server_devkey = _option.server_devkey;
            tmpServerInfo.mqtt_port = _option.mqtt_port;
            tmpServerInfo.mqtt_username = _option.mqtt_username;
            tmpServerInfo.mqtt_password = _option.mqtt_password;

            //创建远程控制
            _client = _factory.CreateMqttClient();
            _client.ApplicationMessageReceivedAsync += MqttClient_MessageReceived;
            var mqttClientOptions = new MqttClientOptionsBuilder()
.WithTcpServer(tmpServerInfo.server_ip, tmpServerInfo.mqtt_port)
.WithClientId(_machineId)
.WithKeepAlivePeriod(TimeSpan.FromSeconds(15))
.WithCredentials(tmpServerInfo.mqtt_username, tmpServerInfo.mqtt_password)
.Build();
            await _client.ConnectAsync(mqttClientOptions, stoppingToken);

            var mqttRecvOption = _factory.CreateSubscribeOptionsBuilder().WithTopicFilter(f =>
            {
                f.WithTopic("wukong/down/" + _machineId);
            }).Build();
            await _client.SubscribeAsync(mqttRecvOption);

            if (_tunnelTask == null)
            {
                _tunnelTask = Task.Run(async () =>
                {
                    await TunnelStart(tmpServerInfo, stoppingToken);
                });
            }

        }
    }
}
