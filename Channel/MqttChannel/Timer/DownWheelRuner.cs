using ChannelUtility;
using ChannelUtility.Message;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MqttChannel.Timer
{
    /// <summary>
    /// 防下发过快定时器
    /// </summary>
    public class DownWheelRuner
    {
        List<HashedWheelTimer> _schedulers;
        public DownWheelRuner(int runcount = 3)
        {
            _schedulers = new List<HashedWheelTimer>();
            for (int i = 0; i < runcount; i++)
            {
                var tmpsche = new HashedWheelTimer(TimeSpan.FromMilliseconds(100), 1024, 0);
                _schedulers.Add(tmpsche);
            }

        }
        public void PushConcurrentTask(RawDataMessage msg, Func<RawDataMessage, Task> ac)
        {
            PushConcurrentTask(msg, ac, TimeSpan.Zero);
        }
        public void PushConcurrentTask(RawDataMessage msg, Func<RawDataMessage, Task> ac, TimeSpan ts)
        {
            int curidx = Math.Abs(msg.DeviceId.GetHashCode() % _schedulers.Count);
            _schedulers[curidx].NewTimeout(new DownTask(msg, ac), ts);
        }
        public void StopAll()
        {
            foreach (var sch in _schedulers)
            {
                sch.Stop();
            }
        }
    }

    public class DownTask : TimerTask
    {
        private Func<RawDataMessage, Task> _ac;
        private RawDataMessage _msg;
        public DownTask(RawDataMessage msg, Func<RawDataMessage, Task> ac)
        {
            _msg = msg;
            _ac = ac;
        }
        public void Run(IWheelTimeout timeout)
        {
            var res = _ac(_msg);
            res.Wait();
        }
    }
}
