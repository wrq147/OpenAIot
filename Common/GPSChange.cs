using System;

namespace Common
{
    public class GPSChange
    {
        private const double x_pi = 3.14159265358979324 * 3000.0 / 180.0;

        private const double a = 6378245.0;
        private const double ee = 0.00669342162296594323;
        public static GPSPoint WGS84_to_GCJ02(double wg_lat, double wg_lon)
        {
            double dLat = TransformLat(wg_lon - 105.0, wg_lat - 35.0);
            double dLon = TransformLon(wg_lon - 105.0, wg_lat - 35.0);
            double radLat = wg_lat / 180.0 * Math.PI;
            double magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            double sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * Math.PI);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * Math.PI);
            double newLat = wg_lat + dLat;
            double newLon = wg_lon + dLon;
            GPSPoint point = new GPSPoint();
            point.SetLng(newLon);
            point.SetLat(newLat);
            return point;
        }

        private static double TransformLat(double x, double y)
        {
            double ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(y * Math.PI) + 40.0 * Math.Sin(y / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(y / 12.0 * Math.PI) + 320 * Math.Sin(y * Math.PI / 30.0)) * 2.0 / 3.0;
            return ret;
        }

        private static double TransformLon(double x, double y)
        {
            double ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
            ret += (20.0 * Math.Sin(6.0 * x * Math.PI) + 20.0 * Math.Sin(2.0 * x * Math.PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(x * Math.PI) + 40.0 * Math.Sin(x / 3.0 * Math.PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(x / 12.0 * Math.PI) + 300.0 * Math.Sin(x / 30.0 * Math.PI)) * 2.0 / 3.0;
            return ret;
        }
        /// <summary>
        /// GCJ-02转换BD-09
        /// </summary>
        /// <param name="gg_lat">纬度</param>
        /// <param name="gg_lon">经度</param>
        /// <returns></returns>
        public static GPSPoint GCJ02_to_BD09(double gg_lat, double gg_lon)
        {
            GPSPoint point = new GPSPoint();
            double x = gg_lon, y = gg_lat;
            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * x_pi);
            double bd_lon = z * Math.Cos(theta) + 0.0065;
            double bd_lat = z * Math.Sin(theta) + 0.006;
            point.SetLat(bd_lat);
            point.SetLng(bd_lon);
            return point;
        }

        /// <summary>
        /// BD-09转换GCJ-02
        /// </summary>
        /// <param name="bd_lat">纬度</param>
        /// <param name="bd_lon">经度</param>
        /// <returns></returns>
        public static GPSPoint BD09_to_GCJ02(double bd_lat, double bd_lon)
        {
            GPSPoint point = new GPSPoint();
            double x = bd_lon - 0.0065, y = bd_lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_pi);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_pi);
            double gg_lon = z * Math.Cos(theta);
            double gg_lat = z * Math.Sin(theta);
            point.SetLat(gg_lat);
            point.SetLng(gg_lon);
            return point;
        }


    }

    public class GPSPoint
    {
        private double lat;// 纬度
        private double lng;// 经度

        public GPSPoint()
        {
        }

        public GPSPoint(double lng, double lat)
        {
            this.lng = lng;
            this.lat = lat;
        }


        public double GetLat()
        {
            return lat;
        }
        public void SetLat(double lat)
        {
            this.lat = lat;
        }
        public double GetLng()
        {
            return lng;
        }
        public void SetLng(double lng)
        {
            this.lng = lng;
        }


    }
}
