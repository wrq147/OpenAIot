using AirJointUI.Api;
using AirJointUI.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AirJointUI.ViewModels
{
    /// <summary>
    /// 串口配置模型
    /// </summary>
    public class SetSerialViewModel : ViewModelBase
    {
        private string _txtSendInterval = "1000";
        public string TxtSendInterval
        {
            get => _txtSendInterval;
            set => this.RaiseAndSetIfChanged(ref _txtSendInterval, value);
        }
        private string _txtDownInterval = "2000";
        public string TxtDownInterval
        {
            get => _txtDownInterval;
            set => this.RaiseAndSetIfChanged(ref _txtDownInterval, value);
        }
        private string _txtTurnOff = "10";
        public string TxtTurnOff
        {
            get => _txtTurnOff;
            set => this.RaiseAndSetIfChanged(ref _txtTurnOff, value);
        }
        private bool _enPort;
        public bool EnablePort
        {
            get => _enPort;
            set => this.RaiseAndSetIfChanged(ref _enPort, value);
        }
        private bool _enHoriz;
        public bool EnableHoriz
        {
            get => _enHoriz;
            set => this.RaiseAndSetIfChanged(ref _enHoriz, value);
        }
        private int _netWay;
        public int NetWay
        {
            get => _netWay;
            set => this.RaiseAndSetIfChanged(ref _netWay, value);
        }
        private bool _showIp;
        public bool ShowIp
        {
            get => _showIp;
            set => this.RaiseAndSetIfChanged(ref _showIp, value);
        }
        private string _txtIpPort;
        public string TxtIpPort
        {
            get => _txtIpPort;
            set => this.RaiseAndSetIfChanged(ref _txtIpPort, value);
        }
        private int _timeZoneVal = 0;
        public int TimeZoneVal
        {
            get => _timeZoneVal;
            set => this.RaiseAndSetIfChanged(ref _timeZoneVal, value);
        }
        public List<string> Items { get; set; }
        public SetSerialViewModel()
        {
            Items = new List<string>
            {
                "Asia/Chita",
                "Asia/Irkutsk",
                "Asia/Novosibirsk",
                "Asia/Seoul",
                "Asia/Shanghai",
                "Asia/Tashkent",
                "Asia/Tokyo",
                "Asia/Vladivostok",
                "America/New_York",
                "Europe/Berlin",
                "Europe/London",
                "Europe/Moscow",
                "Europe/Paris",
                "Europe/Samara"
            };
        }
        public async Task Init()
        {
            //初始化通信方式
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo parentDirectoryInfo = Directory.GetParent(currentDirectory);
            if (parentDirectoryInfo != null)
            {
                try
                {
                    string tmpfilepath = Path.Combine(parentDirectoryInfo.FullName, "channel", "appsettings.json");
                    string jsonContent = File.ReadAllText(tmpfilepath);
                    var jsonObject = JsonNode.Parse(jsonContent); ;
                    var mdop = jsonObject["ModbusOption"];
                    this.NetWay = mdop["net_way"].GetValue<int>();
                    if (this.NetWay == 2)
                    {
                        this.ShowIp = true;
                    }
                    else
                    {
                        this.ShowIp = false;
                    }
                    this.TxtIpPort = mdop["tcp_ip"].GetValue<string>() + " " + mdop["tcp_port"].GetValue<int>();

                    this.TxtDownInterval = mdop["down_interval"].GetValue<int>().ToString();
                }
                catch { }
            }


            //初始化下发间隔
            var chinfo = await RedisApi.GetOnlyChannelInfo();
            if (chinfo.code == 0)
            {
                TxtSendInterval = chinfo.data.SendInterval.ToString();
            }
            else
            {
                TxtSendInterval = "1000";
            }


            //初始化黑屏时间
            string blankTime = "0";
            if (File.Exists("bktime"))
            {
                blankTime = File.ReadAllText("bktime");
            }
            TxtTurnOff = blankTime;

            //初始化屏幕方向
            string dirval = "Horiz";
            if (File.Exists("dirf"))
            {
                dirval = File.ReadAllText("dirf");
            }
            if (dirval == "Port")
            {
                this.EnablePort = true;
                this.EnableHoriz = false;
            }
            else
            {
                this.EnablePort = false;
                this.EnableHoriz = true;
            }

            var tmpzone = getTimeZone();
            this.TimeZoneVal = this.Items.IndexOf(tmpzone);
        }
        public void setTimeZone()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"timedatectl set-timezone " + this.Items[this.TimeZoneVal] + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
        }
        private string getTimeZone()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = "-c \"timedatectl | grep 'Time zone' | awk -F': ' '{print $2}' | awk '{print $1}'\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output.Trim();
            }
            else
            {
                return "Asia/Shanghai";
            }
        }
        public bool Save()
        {
            File.WriteAllText("bktime", TxtTurnOff);

            string txtPath = "/etc/X11/xorg.conf";
            if (this.EnablePort)
            {
                File.WriteAllText(txtPath, @"
Section ""Device""
    Identifier ""Allwinner Graphics""
    Driver ""fbdev""
    Option ""Rotate"" ""CCW""
EndSection");
                File.WriteAllText("dirf", "Port");
            }
            else if (this.EnableHoriz)
            {
                File.WriteAllText(txtPath, string.Empty);
                File.WriteAllText("dirf", "Horiz");
            }


            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo parentDirectoryInfo = Directory.GetParent(currentDirectory);
            if (parentDirectoryInfo != null)
            {
                try
                {
                    string tmpfilepath = Path.Combine(parentDirectoryInfo.FullName, "channel", "appsettings.json");
                    string jsonContent = File.ReadAllText(tmpfilepath);
                    var jsonObject = JsonNode.Parse(jsonContent);

                    var mdop = jsonObject["ModbusOption"];
                    mdop["net_way"] = this.NetWay;

                    if (this.NetWay == 2 && !string.IsNullOrEmpty(this.TxtIpPort))
                    {
                        string pattern = @"[:\s,;]";
                        Regex regex = new Regex(pattern);
                        string[] parts = regex.Split(this.TxtIpPort);
                        if (parts.Length > 1)
                        {
                            mdop["tcp_ip"] = parts[0];
                            mdop["tcp_port"] = Convert.ToInt32(parts[^1]);
                        }
                    }

                    mdop["down_interval"] = Convert.ToInt32(this.TxtDownInterval);

                    string updatedJson = jsonObject.ToJsonString();
                    File.WriteAllText(tmpfilepath, updatedJson);
                }
                catch { }
            }


            setTimeZone();

            ChannelInfo info = new ChannelInfo();
            info.SendInterval = Convert.ToInt32(_txtSendInterval);
            if (info.SendInterval > 25000)
            {
                info.SendInterval = 25000;
            }
            var svrs = RedisApi.ChgOnlyChannel(info);
            if (svrs != null && svrs.code == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
