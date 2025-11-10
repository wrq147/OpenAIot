using System;

namespace TemplateAction.Common.Converts
{
    /// <summary>
    /// DateTime与Long互相转换,Long值可在Java中使用
    /// </summary>
    public class TimeConvert : ITAConvert
    {
        public ITAConvert NextConvert { get; set; }

        public object Convert(object value, Type targetType)
        {
            if (value.GetType() == typeof(DateTime))
            {
                if (targetType == typeof(long))
                {
                    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    TimeSpan toNow = (DateTime)value - dtStart;
                    return toNow.Ticks / 10000;
                }
            }
            else
            {
                if (targetType == typeof(DateTime))
                {
                    if (value.GetType() == typeof(long))
                    {
                        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                        TimeSpan toNow = new TimeSpan((long)value * 10000);
                        DateTime dtResult = dtStart.Add(toNow);
                        return dtResult;
                    }
                }
            }
            return this.NextConvert.Convert(value, targetType);
        }
    }
}
