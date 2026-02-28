using ChannelUtility;
using ChannelUtility.Message;
using IoTAIService.AIProject;
using MessagePack;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTAIService
{
    public class ReliableAISubscriber : IDisposable
    {
        private SubscriberSocket _subscriber;
        private readonly string _connectAddress;
        private bool _isConnected;
        private CancellationTokenSource _cts;
        private ITAServiceProvider _provider;
        // 新增：定义Thread对象，用于管理订阅线程
        private Thread _receiveThread;

        public ReliableAISubscriber(ITAServiceProvider serviceProvider, IOptions<IoTAIOption> option)
        {
            _provider = serviceProvider;
            _connectAddress = option.Value.FramePushConn;
            _isConnected = false;
            _cts = new CancellationTokenSource();
            _receiveThread = null; // 初始化线程对象
        }

        /// <summary>
        /// 启动订阅者（改为使用Thread）
        /// </summary>
        public void StartReceiving()
        {
            // 防止重复启动线程
            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                return;
            }

            // 创建并启动新线程
            _receiveThread = new Thread(ReceiveLoop);
            // 将线程设置为后台线程，避免阻止程序退出
            _receiveThread.IsBackground = true;
            _receiveThread.Start();
        }

        /// <summary>
        /// 订阅消息的循环逻辑（线程执行的核心方法）
        /// </summary>
        private void ReceiveLoop()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    if (!_isConnected)
                    {
                        // 同步调用异步的重连方法（Thread中无法直接await）
                        ReconnectAsync().GetAwaiter().GetResult();
                    }
                    if (_isConnected)
                    {
                        // 改用TryReceiveFrameBytes+超时，避免永久阻塞
                        if (_subscriber.TryReceiveFrameBytes(TimeSpan.FromSeconds(2), out byte[] frameData) && frameData != null)
                        {
                            var options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
                            var aiFrame = MessagePackSerializer.Deserialize<AIDetectRequestMeesage>(frameData, options);
                            List<AIConfigData> configList = null;
                            if (!string.IsNullOrEmpty(aiFrame.Configs))
                            {
                                configList = System.Text.Json.JsonSerializer.Deserialize<List<AIConfigData>>(aiFrame.Configs, JsonMessageSerializerConfig.DefaultOptions);
                            }
                            // 同步调用异步的消息处理方法
                            _provider.GetService<AIProjectManager>().MessageHandler(aiFrame, configList).GetAwaiter().GetResult();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"接收异常：{ex.ToString()}");
                    _isConnected = false;
                    // 避免高频循环，线程休眠1秒（注意处理取消令牌）
                    if (!_cts.Token.WaitHandle.WaitOne(1000))
                    {
                        break; // 令牌已取消，退出循环
                    }
                }
            }
        }

        /// <summary>
        /// 重连逻辑（保持原有逻辑，适配Thread调用）
        /// </summary>
        private async Task ReconnectAsync()
        {
            int retry = 0;
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    _subscriber?.Dispose();
                    _subscriber = new SubscriberSocket();
                    _subscriber.Connect(_connectAddress);
                    _subscriber.SubscribeToAnyTopic();
                    Console.WriteLine("订阅者重连成功");
                    _isConnected = false;
                    break; // 重连成功后退出重连循环
                }
                catch (Exception ex)
                {
                    _isConnected = false;
                    retry++;
                    Console.WriteLine($"重连失败（第{retry}次）：{ex.Message}");
                }
                // 重连间隔1秒，处理取消令牌
                await Task.Delay(1000, _cts.Token);
            }
        }

        /// <summary>
        /// 停止订阅者（优雅退出）
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            _isConnected = false;
            // 等待线程退出（最多等待5秒，避免卡死）
            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Join(TimeSpan.FromSeconds(5));
            }
            Console.WriteLine("订阅者已停止");
        }

        /// <summary>
        /// 完整释放资源
        /// </summary>
        public void Dispose()
        {
            Stop();
            _subscriber?.Dispose();
            _cts?.Dispose();
            _receiveThread = null; // 清空线程对象
        }
    }
}