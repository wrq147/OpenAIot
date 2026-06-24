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
                if (double.TryParse(sinput, out double dv))
                {
                    val = dv;
                }
                else
                {
                    val = min;
                }
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
            switch (input)
            {
                case string sinput:
                    {
                        if (double.TryParse(sinput, out double dv))
                        {
                            val = dv;
                        }
                        else
                        {
                            val = min;
                        }
                    }
                    break;
                case float finput:
                    {
                        val = finput;
                    }
                    break;
                case double dbinput:
                    {
                        val = dbinput;
                    }
                    break;
                case int iinput:
                    {
                        val = Convert.ToSingle(iinput);
                    }
                    break;
                case long linput:
                    {
                        val = Convert.ToDouble(linput);
                    }
                    break;
                default:
                    val = Convert.ToDouble(input);
                    break;
            }
            val = spacing + val * (multiple == 0 ? 1.0 : multiple);
            val = Math.Min(max, Math.Max(min, val));
            double factor = Math.Pow(10, decimals);
            val = Math.Truncate(val * factor) / factor;
            return val;
        }
    }
}
