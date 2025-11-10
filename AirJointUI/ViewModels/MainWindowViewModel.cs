using AirJointUI.Models;
using ReactiveUI;
using System;
using System.Net.NetworkInformation;
using System.Threading;
namespace AirJointUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _contentViewModel;
        private Timer _timer;
        private Timer _testNetTimer;
        private bool _testRun = false;
        private string[] _urls = new string[] { "125.124.98.180", "8.8.8.8", "iot.wookongcloud.com" };
        public ViewModelBase ContentViewModel
        {
            get => _contentViewModel;
            set => this.RaiseAndSetIfChanged(ref _contentViewModel, value);
        }
        public MainWindowViewModel()
        {
            // 创建一个新的计时器对象
            _timer = new Timer(new TimerCallback(UpdateTopTime), null, 0, 500);
            _testNetTimer = new Timer(new TimerCallback(UpdateTopNet), null, 0, 5000);
        }
        private void UpdateTopNet(object value)
        {
            if (_testRun) return;
            _testRun = true;
            try
            {
                Ping ping = new Ping();
                for (int i = 0; i < _urls.Length; i++)
                {
                    try
                    {
                        PingReply pingStatus = ping.Send(_urls[i], 500);
                        if (pingStatus.Status == IPStatus.Success)
                        {
                            TopInfo.Instance.HasNet = true;
                            return;
                        }
                    }
                    catch { }

                }
                TopInfo.Instance.HasNet = false;
            }
            finally
            {
                _testRun = false;
            }

        }
        private void UpdateTopTime(object value)
        {
            #region 更新当前时间
            TopInfo.Instance.UITime = DateTime.Now.ToString("yyyy.MM.dd  HH:mm:ss");
            TopInfo.Instance.UIDate = DateTime.Now.ToString("yyyy-MM-dd");
            TopInfo.Instance.UIDateTime = DateTime.Now.ToString("HH:mm:ss");
            #endregion

        }
    }
}
