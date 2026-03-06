using System;
using System.Text;

namespace ChannelUtility.Tsl
{
    public class FloatOption : BaseValueOption
    {
        public FloatOption()
        {
            this.type = "float";
        }
        public double max { get; set; }
        public double min { get; set; }
        public string unit { get; set; }
        public double spacing { get; set; }
        public double multiple { get; set; }
        /// <summary>
        /// 小数点位数
        /// </summary>
        public int decimals { get; set; }


        public double Limit(object input)
        {
            if (decimals < 1) decimals = 1;
            if (decimals > 8) decimals = 8;
            double val;
            if (input is string sinput)
            {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sinput);
                val = BitConverter.ToDouble(bytes);
            }
            else if (input is float finput)
            {
                val = finput;
            }
            else if (input is double dbinput)
            {
                val = dbinput;
            }
            else
            {
                val = Convert.ToDouble(input);
            }

            val = Math.Min(max, Math.Max(min, val));
            double factor = Math.Pow(10, decimals);
            val = Math.Truncate(val * factor) / factor;
            return val;
        }
        public override object InnerRawTo(object input)
        {
            if (decimals < 1) decimals = 1;
            if (decimals > 8) decimals = 8;
            double val;
            if (input is string sinput)
            {
                byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sinput);
                val = BitConverter.ToDouble(bytes);
            }
            else if (input is float finput)
            {
                val = finput;
            }
            else if (input is double dbinput)
            {
                val = dbinput;
            }
            else
            {
                val = Convert.ToDouble(input);
            }
            val = spacing + val * (multiple == 0 ? 1.0 : multiple);
            val = Math.Min(max, Math.Max(min, val));
            double factor = Math.Pow(10, decimals);
            val = Math.Truncate(val * factor) / factor;
            return val;
        }
    }
}
