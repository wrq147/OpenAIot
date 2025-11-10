using ChannelUtility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModbusChannel
{
    public class TcpServerTask
    {
        private string _ip;
        private int _port;
        private int _downInterval;
        private IServiceProvider _provider;
        private TcpListener _tcpListener;

        private Task _mainTask;
        private bool _isAborted;
        public TcpServerTask(string ip, int port, int downInterval, IServiceProvider provider)
        {
            _ip = ip;
            _port = port;
            _downInterval = downInterval;
            _provider = provider;
        }
        public async Task Start(CancellationToken stoppingToken)
        {
            _tcpListener = new TcpListener(IPAddress.Parse(_ip), _port);
            // 启动服务器
            _tcpListener.Start();
            _mainTask = Task.Run(async () =>
            {
                while (!_isAborted)
                {
                    try
                    {
                        await Exe();
                    }
                    catch (TaskCanceledException)
                    {
                        _isAborted = true;
                    }
                    catch (ThreadAbortException)
                    {
                        _isAborted = true;
                    }
                    catch (ThreadInterruptedException)
                    {
                        _isAborted = true;
                    }
                    catch { }
                }
            });

        }
        private async Task Exe()
        {

            // 等待客户端连接
            TcpClient client = await _tcpListener.AcceptTcpClientAsync();
            Console.WriteLine("客户端已连接。");

            // 为每个客户端创建一个新的线程来读取信息
            Thread recvThread = new Thread(async () =>
            {
                await StartRecv(client);
            });
            //为每个客户端创建一个新的线程轮询下发数据
            Thread pollingThread = new Thread(async () =>
            {
                await StartSend(client);
            });

            recvThread.Start();
            pollingThread.Start();

        }
        private async Task StartSend(TcpClient client)
        {

        }
        private async Task StartRecv(TcpClient client)
        {
            try
            {

                // 获取与客户端的网络流
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[2048];
                int bytesRead;
                // 读取客户端发送的消息
                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理客户端时发生错误: {ex.Message}");
            }
            finally
            {
                // 关闭客户端连接
                client.Close();
                Console.WriteLine("客户端连接已关闭。");
            }
        }
        public async Task Stop()
        {

        }

    }
}
