using ReactiveUI;
using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace AirJointUI.Models
{
    /// <summary>
    /// 公用顶部信息
    /// </summary>
    public class TopInfo : ReactiveObject
    {
        private static readonly object _lock = new object();
        private volatile static TopInfo _instance = null;
        public static TopInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TopInfo();
                        }
                    }
                }
                return _instance;
            }
        }
        private TopInfo() { }
        private bool _hasNet;
        /// <summary>
        /// 是否启用wifi
        /// </summary>
        public bool HasNet
        {
            get
            {
                return _hasNet;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _hasNet, value);
            }
        }
        private string _uiTime;
        /// <summary>
        /// 当前显示时间
        /// </summary>
        public string UITime
        {
            get
            {
                return _uiTime;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _uiTime, value);
            }
        }
        private string _date;
        public string UIDate
        {
            get
            {
                return _date;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _date, value);
            }
        }
        private string _time;
        public string UIDateTime
        {
            get
            {
                return _time;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref _time, value);
            }
        }
    }
}
