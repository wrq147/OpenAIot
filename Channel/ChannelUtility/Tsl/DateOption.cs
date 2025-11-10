using System;

namespace ChannelUtility.Tsl
{
    public class DateOption : BaseValueOption
    {
        public DateOption()
        {
            this.type = "date";
        }
        /// <summary>
        /// 格式化字符串
        /// </summary>
        public string format { get; set; }
        protected override object InnerRawTo(object input)
        {
            if (input is string)
            {
                DateTime now;
                if (DateTime.TryParse((string)input, out now))
                {
                    DateTimeOffset dto = new DateTimeOffset(now);
                    return dto.ToUnixTimeMilliseconds();
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return Convert.ToInt64(input);
            }

        }
    }
}
