using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelUtility.Tsl
{
    /// <summary>
    /// Geo地理位置类型
    /// </summary>
    public class GeoOption : BaseValueOption
    {
        /// <summary>
        /// 是否开启火星坐标系转换
        /// </summary>
        public bool usingGCJTo { get; set; }
        public GeoOption()
        {
            this.type = "geo";
        }
        protected override object InnerRawTo(object input)
        {
            if (input is string s)
            {
                string[] arr = s.Split(new char[] { ',', '_' });
                if (arr.Length > 1)
                {
                    Dictionary<string, object> rs = new Dictionary<string, object>();
                    var tmplng = Convert.ToDouble(arr[0]);
                    var tmplat = Convert.ToDouble(arr[1]);
                    if (usingGCJTo)
                    {
                        var newgps = UtilityTool.WGS84_to_GCJ02(tmplat, tmplng);
                        rs.Add("lng", newgps.GetLng());
                        rs.Add("lat", newgps.GetLat());
                        return rs;
                    }
                    else
                    {
                        rs.Add("lng", tmplng);
                        rs.Add("lat", tmplat);
                        return rs;
                    }

                }
            }
            else if (input.GetType().IsArray)
            {
                Array array = input as Array;
                if (array != null && array.Length > 1)
                {
                    Dictionary<string, object> rs = new Dictionary<string, object>();
                    var tmplng = Convert.ToDouble(array.GetValue(0));
                    var tmplat = Convert.ToDouble(array.GetValue(1));
                    if (usingGCJTo)
                    {
                        var newgps = UtilityTool.WGS84_to_GCJ02(tmplat, tmplng);
                        rs.Add("lng", newgps.GetLng());
                        rs.Add("lat", newgps.GetLat());
                        return rs;
                    }
                    else
                    {
                        rs.Add("lng", tmplng);
                        rs.Add("lat", tmplat);
                        return rs;
                    }
                }

            }
            return null;
        }
    }
}
