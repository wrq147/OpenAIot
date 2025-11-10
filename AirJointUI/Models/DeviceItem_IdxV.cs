using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace AirJointUI.Models
{
    public class DeviceItem_IdxV : ParamObject
    {
        public string Id { get; set; }
        private string _number;
        /// <summary>
        /// 设备第三方编码
        /// </summary>
        public string Number
        {
            get => _number;
            set => this.RaiseAndSetIfChanged(ref _number, value);
        }
        private int _runState;
        /// <summary>
        /// 运行状态
        /// </summary>
        public int RunState
        {
            get => _runState;
            set => this.RaiseAndSetIfChanged(ref _runState, value);
        }
        private bool _isRuning;
        public bool IsRuning
        {
            get => _isRuning;
            set => this.RaiseAndSetIfChanged(ref _isRuning, value);
        }
        private string _runText;
        public string RunText
        {
            get => _runText;
            set => this.RaiseAndSetIfChanged(ref _runText, value);
        }

        private bool _isOnline;
        public bool IsOnline
        {
            get => _isOnline;
            set => this.RaiseAndSetIfChanged(ref _isOnline, value);
        }

        private bool _jointControlState;
        /// <summary>
        /// 联控状态
        /// </summary>
        public bool JointControlState
        {
            get => _jointControlState;
            set => this.RaiseAndSetIfChanged(ref _jointControlState, value);
        }
        private string _description;
        /// <summary>
        /// 设备描述
        /// </summary>
        public string? Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        private string _name;
        public string? Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
        private string _runTimes;
        /// <summary>
        /// 运行时间（秒）
        /// </summary>
        public string RunTimes
        {
            get => _runTimes;
            set => this.RaiseAndSetIfChanged(ref _runTimes, value);
        }

        private string _stopTimes;
        /// <summary>
        /// 停机时间（秒）
        /// </summary>
        public string StopTimes
        {
            get => _stopTimes;
            set => this.RaiseAndSetIfChanged(ref _stopTimes, value);
        }


        private bool _enableShow;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool EnableShow
        {
            get => _enableShow;
            set => this.RaiseAndSetIfChanged(ref _enableShow, value);
        }
        private bool _enableStart;
        public bool EnableStart
        {
            get => _enableStart;
            set => this.RaiseAndSetIfChanged(ref _enableStart, value);
        }
        private bool _enableStop;
        public bool EnableStop
        {
            get => _enableStop;
            set => this.RaiseAndSetIfChanged(ref _enableStop, value);
        }

    }

}
