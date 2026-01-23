using Common.Attr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
namespace IoTRulesService.Flow.Builder
{
    /// <summary>
    /// 表示一条有时间的数据流
    /// </summary>
    public class StreamData
    {
        public static StreamData Create(IDictionary<string, object> data)
        {
            return Create(data, DateTime.Now);
        }
        public static StreamData Create(IDictionary<string, object> data, DateTime time)
        {
            return new StreamData()
            {
                Data = data ?? new Dictionary<string, object>(),
                Time = MyAccess.Core.TypeConvert.Time2Unix(time)
            };
        }
        public IDictionary<string, object> Data { get; set; }
        public long Time { get; set; }
        public DateTime TimeStr
        {
            get { return MyAccess.Core.TypeConvert.Unix2Time(Time); }
        }
    }
}
