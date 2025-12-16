using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace ModbusChannel
{
    public class ModbusService : BackgroundService
    {
        private IServiceProvider _provider;
        private List<SerialTask> _tasks;
        private RemoteTask _remote;
        private GpioTask _gpio;
        private TcpClientTask _tcp;
        public ModbusService(IServiceProvider provider)
        {
            _provider = provider;
            _tasks = new List<SerialTask>();
        }
        private string GetMachineId()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"ip link show | grep \\\"link/ether\\\" | awk '{{print $2}}'\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                string[] tarr = output.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                if (tarr.Length > 0)
                {
                    return tarr[^1].Replace(":", "").ToLower();
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {

                // 创建ProcessStartInfo对象
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe", // 指定命令解释器
                    RedirectStandardInput = true, // 允许写入输入流
                    RedirectStandardOutput = true, // 允许读取输出流
                    UseShellExecute = false, // 不使用系统外壳程序启动
                    CreateNoWindow = true // 不创建新窗口
                };
                string command = "wmic cpu get processorid";
                // 启动进程
                using (Process process = Process.Start(startInfo))
                {
                    // 写入命令
                    using (StreamWriter sw = process.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            // 写入命令
                            sw.WriteLine(command);
                        }
                    }
                    // 等待命令执行完成
                    process.WaitForExit();
                    // 读取命令的输出
                    string output = process.StandardOutput.ReadToEnd();
                    int P = output.IndexOf(command) + command.Length;
                    output = output.Substring(P, output.Length - P - 3);
                    string[] tmpsaa = output.Split("\r\n");
                    if (tmpsaa.Length > 2)
                    {
                        return tmpsaa[2].Replace("-", "").Replace("\r", "").Trim().ToLower();
                    }
                }
                return string.Empty;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var option = _provider.GetService<IOptions<ModbusOption>>();
            string machineId = GetMachineId();
            if (option.Value.net_way == 1)
            {
                foreach (var item in option.Value.serial_names)
                {
                    SerialTask t = new SerialTask(item, option.Value, _provider);
                    await t.Start(stoppingToken);
                    _tasks.Add(t);
                }
            }
            else if (option.Value.net_way == 2)
            {
                _tcp = new TcpClientTask(option.Value.tcp_ip, option.Value.tcp_port, option.Value.tcp_dtuid, option.Value.down_interval, option.Value.send_interval, _provider);
                await _tcp.Start(stoppingToken);
            }
            else
            {
                //启用串口服务
                if (option.Value.serial_names != null)
                {
                    foreach (var item in option.Value.serial_names)
                    {
                        SerialTask t = new SerialTask(item, option.Value, _provider);
                        await t.Start(stoppingToken);
                        _tasks.Add(t);
                    }
                }
                //启用tcp服务
                if (_tasks.Count == 0 && !string.IsNullOrEmpty(option.Value.tcp_dtuid))
                {
                    _tcp = new TcpClientTask(option.Value.tcp_ip, option.Value.tcp_port, option.Value.tcp_dtuid, option.Value.down_interval, option.Value.send_interval, _provider);
                    await _tcp.Start(stoppingToken);
                }
            }


            //启用gpio服务
            if (!string.IsNullOrEmpty(option.Value.gpio_dtuid))
            {
                _gpio = new GpioTask(option.Value.gpio_dtuid, _provider);
                await _gpio.Start(stoppingToken);
            }

            //启用远程服务
            _remote = new RemoteTask(_provider, option.Value, machineId);
            await _remote.Start(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var item in _tasks)
            {
                await item.Stop();
            }
            _tasks.Clear();

            if (_tcp != null)
            {
                await _tcp.Stop();
            }

            if (_gpio != null)
            {
                await _gpio.Stop(cancellationToken);
            }

            if (_remote != null)
            {
                await _remote.Stop(cancellationToken);
            }
            await base.StopAsync(cancellationToken);
        }

    }
}
