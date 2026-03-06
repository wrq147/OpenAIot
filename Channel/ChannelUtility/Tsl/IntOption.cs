using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace ChannelUtility.Tsl
{
    public class IntOption : BaseValueOption
    {
        public IntOption()
        {
            this.type = "int";
        }

        public int max { get; set; }
        public int min { get; set; }
        public string unit { get; set; }
        public float spacing { get; set; }
        public float multiple { get; set; }


        public int Limit(object input)
        {
            return Math.Min(max, Math.Max(min, Convert.ToInt32(input)));
        }
        public override object InnerRawTo(object input)
        {
            long tmpval = 0;
            if (min >= 0)
            {
                switch (input)
                {
                    case string sinput:
                        {
                            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sinput);
                            tmpval = BitConverter.ToInt64(bytes);
                        }
                        break;
                    case short hinput:
                        tmpval = (ushort)hinput;
                        break;
                    case int iinput:
                        tmpval = (uint)iinput;
                        break;
                    case long linput:
                        tmpval = linput;
                        break;
                    case double dbinput:
                        tmpval = Convert.ToInt64(dbinput);
                        break;
                }

            }
            else
            {
                switch (input)
                {
                    case string sinput:
                        {
                            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(sinput);
                            tmpval = BitConverter.ToInt64(bytes);
                        }
                        break;
                    case short hinput:
                        tmpval = hinput;
                        break;
                    case int iinput:
                        tmpval = iinput;
                        break;
                    case long linput:
                        tmpval = linput;
                        break;
                    case double dbinput:
                        tmpval = Convert.ToInt64(dbinput);
                        break;
                }
            }
            long val = Convert.ToInt64(spacing + tmpval * (multiple == 0 ? 1 : multiple));
            return (int)Math.Min(max, Math.Max(min, val));
        }
    }
}
