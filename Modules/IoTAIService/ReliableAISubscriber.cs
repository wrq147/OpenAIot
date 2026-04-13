using ChannelUtility.Message;
using Common.Json;
using IoTAIService.AIProject;
using MessagePack;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TemplateAction.Core;

namespace IoTAIService
{
    public class ReliableAISubscriber : IDisposable
    {
        private PullSocket _subSocket;
        private readonly string _connectAddress;
        private CancellationTokenSource _cts;
        private ITAServiceProvider _provider;
        /// <summary>
        /// 接收线程
        /// </summary>
        private Thread _receiveThread;

        public ReliableAISubscriber(ITAServiceProvider serviceProvider, IOptions<IoTAIOption> option)
        {
            _provider = serviceProvider;
            _connectAddress = option.Value.AIBind;
            _cts = new CancellationTokenSource();
        }


        /// <summary>
        /// 启动所有订阅连接（每个连接一个独立线程）
        /// </summary>
        public void StartReceiving()
        {
            _subSocket = new PullSocket();
            _subSocket.Bind(_connectAddress);
            _receiveThread = new Thread(() => ReceiveLoop());
            _receiveThread.IsBackground = true;
            _receiveThread.Start();
        }


        /// <summary>
        /// 消息接收循环
        /// </summary>
        private void ReceiveLoop()
        {
            AITaskRuner runner = _provider.GetService<AITaskRuner>();
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    // 非阻塞接收消息（超时2秒）
                    if (_subSocket.TryReceiveFrameBytes(TimeSpan.FromSeconds(2), out byte[] frameData) && frameData != null)
                    {
                        // 反序列化消息
                        var options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);
                        var aiFrame = MessagePackSerializer.Deserialize<AIDetectRequestMeesage>(frameData, options);

                        List<AIConfigData> configList = null;
                        if (!string.IsNullOrEmpty(aiFrame.Configs))
                        {
                            configList = System.Text.Json.JsonSerializer.Deserialize<List<AIConfigData>>(aiFrame.Configs, MyDefaultTextJsonConfig.DefaultOptions);
                        }

                        // 处理消息
                        runner.PushConcurrentTask(aiFrame.DeviceId, async () =>
                        {
                            await _provider.GetService<AIProjectManager>().MessageHandler(aiFrame, configList);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"订阅接收异常: {ex.ToString()}");
                    // 休眠1秒避免高频重连（响应取消令牌）
                    if (!_cts.Token.WaitHandle.WaitOne(1000))
                        break;
                }
            }

            Console.WriteLine($"AI订阅连接接收循环退出");
        }

        /// <summary>
        /// 释放所有资源
        /// </summary>
        public void Dispose()
        {
            // 取消所有线程的令牌
            _cts.Cancel();

            if (_receiveThread != null && _receiveThread.IsAlive)
            {
                _receiveThread.Join(TimeSpan.FromSeconds(5));
            }
            Console.WriteLine("订阅连接已停止");
            _cts?.Dispose();
        }

    }


}