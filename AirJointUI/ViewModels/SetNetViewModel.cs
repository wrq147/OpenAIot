using AirJointUI.Models;
using ReactiveUI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DynamicData;
using AirJointUI.Utils;

namespace AirJointUI.ViewModels
{
    /// <summary>
    /// 网络设置模型
    /// </summary>
    public class SetNetViewModel : ViewModelBase
    {
        private bool _enableNet;
        public bool EnableNet
        {
            get => _enableNet;
            set => this.RaiseAndSetIfChanged(ref _enableNet, value);
        }
        public ObservableCollection<ComboItem> SSIDList { get; set; }

        private int _netIndex;
        public int NetIndex
        {
            get => _netIndex;
            set => this.RaiseAndSetIfChanged(ref _netIndex, value);
        }

        private string _txtPassword;
        public string TxtPassword
        {
            get => _txtPassword;
            set => this.RaiseAndSetIfChanged(ref _txtPassword, value);
        }
        private string _txtRemoteCode;
        public string TxtRemoteCode
        {
            get => _txtRemoteCode;
            set => this.RaiseAndSetIfChanged(ref _txtRemoteCode, value);
        }
        public TopInfo Top
        {
            get { return TopInfo.Instance; }
        }
        private string _txtIp;
        public string TxtIp
        {
            get => _txtIp;
            set => this.RaiseAndSetIfChanged(ref _txtIp, value);
        }
        public SetNetViewModel()
        {
            SSIDList = new ObservableCollection<ComboItem>(new List<ComboItem>());
            _netIndex = 0;
        }

        private string HexToChars(string hex)
        {
            string pattern = @"\\x([0-9a-fA-F]{2})";
            MatchEvaluator evaluator = match =>
            {
                string hexValue = match.Groups[1].Value;
                char character = (char)Convert.ToInt32(hexValue, 16);
                return character.ToString();
            };

            return Regex.Replace(hex, pattern, evaluator);
        }
        public void GetWifiList()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"iw dev " + MyVMLocator.Instance.WifiName + " scan | egrep 'SSID'\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var ssids = new List<string>();
            foreach (var line in output.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("SSID:"))
                {
                    string tmpssid = line.Substring(line.IndexOf("SSID:") + "SSID:".Length).Trim();
                    if (!string.IsNullOrEmpty(tmpssid))
                    {
                        ssids.Add(HexToChars(tmpssid));
                    }
                }
            }
            ssids = ssids.Distinct().ToList();

            HashSet<string> tremove = SSIDList.Select(x => x.Name).ToHashSet<string>();
            foreach (var ssid in ssids)
            {
                tremove.Remove(ssid);
                if (!SSIDList.Any(x => x.Name == ssid))
                {
                    SSIDList.Add(new ComboItem()
                    {
                        Name = ssid,
                        Val = ssid
                    });
                }
            }

            var tmpcccll = SSIDList.Where(x => tremove.Contains(x.Name));
            if (tmpcccll.Count() > 0)
            {
                foreach (var rmm in tmpcccll)
                {
                    SSIDList.Remove(rmm);
                }
            }
        }
        private async Task<string> GetConnectedIp()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"ip addr show | awk '/inet / && /brd/ {print $2}' | cut -d/ -f1\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            await process.WaitForExitAsync();
            return output;

        }
        private async Task<string> GetCurSSID()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"iw " + MyVMLocator.Instance.WifiName + " link | grep SSID\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            await process.WaitForExitAsync();

            string curssid = null;
            foreach (var line in output.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains("SSID:"))
                {
                    string tmpssid = line.Substring(line.IndexOf("SSID:") + "SSID:".Length).Trim();
                    if (!string.IsNullOrEmpty(tmpssid))
                    {
                        curssid = HexToChars(tmpssid);
                    }
                }
            }
            return curssid;
        }
        public void CloseNet()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"ifconfig " + MyVMLocator.Instance.WifiName + " down\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }
        public void OpenNet()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"ifconfig " + MyVMLocator.Instance.WifiName + " up\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }
        private bool ConnectNet(string ssid, string pwd)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"nmcli device wifi connect " + ssid + " password " + pwd + " \"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (output.IndexOf("successfully") != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
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
        public async Task Init()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var curssid = await GetCurSSID();
                if (curssid == null)
                {
                    this.EnableNet = false;
                }
                else
                {
                    this.EnableNet = true;
                }

                for (int i = 0; i < SSIDList.Count; i++)
                {
                    var ssid = SSIDList[i];
                    if (ssid.Val == curssid)
                    {
                        this.NetIndex = i;
                    }
                }

                this.TxtIp = string.Format(I18NExt.Translate("IpAddress"), await GetConnectedIp());
            }
            else
            {
                this.TxtIp = string.Format(I18NExt.Translate("IpAddress"), "111.111.111.111");
            }

            this.TxtRemoteCode = string.Format(I18NExt.Translate("RemoteCode"), GetMachineId());
        }
        public bool Save()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (_enableNet)
                {
                    OpenNet();
                    if (!ConnectNet(this.SSIDList[_netIndex].Val, _txtPassword))
                    {
                        return ConnectNet(this.SSIDList[_netIndex].Val, _txtPassword);
                    }
                    return true;
                }
                else
                {
                    return true;
                }
            }
            return true;
        }
    }
}
