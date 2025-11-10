using System;

namespace MyAccess.Core
{
    /// <summary>
    /// 经纬度计算工具
    /// </summary>
    public class LngLat
    {
        private const double EARTH_RADIUS = 6378137;//地球半径
        private double mLng;
        private double mLat;
        public double Lng
        {
            get { return mLng; }
        }
        public double Lat
        {
            get { return mLat; }
        }

        public LngLat(decimal lng,decimal lat)
        {
            mLng = Convert.ToDouble(lng);
            mLat = Convert.ToDouble(lat);
        }
        public LngLat(double lng,double lat)
        {
            mLng = lng;
            mLat = lat;
        }
        public NARect GetRectFromRange(double range)
        {
            double pi = 3.14159265;
            double latitude = Convert.ToDouble(mLat);
            double longitude = Convert.ToDouble(mLng);

            double degree = (24901 * 1609) / 360.0;
            double raidusMile = range;//20公里

            double dpmLat = 1 / degree;
            double radiusLat = Math.Abs(dpmLat * raidusMile);
            double minLat = latitude - radiusLat;
            double maxLat = latitude + radiusLat;

            double mpdLng = degree * Math.Cos(latitude * (pi / 180));
            double dpmLng = 1 / mpdLng;
            double radiusLng = Math.Abs(dpmLng * raidusMile);
            double minLng = longitude - radiusLng;
            double maxLng = longitude + radiusLng;
            return new NARect(minLng, maxLng, minLat, maxLat);
        }
        /// <summary>
        /// 度转弧度
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }
        /// <summary>
        /// 获取两经纬度的距离
        /// </summary>
        /// <param name="srcLng"></param>
        /// <param name="srcLat"></param>
        /// <param name="destLng"></param>
        /// <param name="destLat"></param>
        /// <returns></returns>
        public double GetDistance(double destLng, double destLat)
        {
            double radLat1 = rad(mLat);
            double radLat2 = rad(destLat);
            double a = radLat1 - radLat2;
            double b = rad(mLng) - rad(destLng);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
              Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
        public double GetDistance(decimal destLng, decimal destLat)
        {
            double lng = Convert.ToDouble(destLng);
            double lat = Convert.ToDouble(destLat);
            return GetDistance(lng, lat);
        }
    }
    public class NARect
    {
        private double mMinLng;
        private double mMaxLng;
        private double mMinLat;
        private double mMaxLat;
        public NARect(double minlng, double maxlng,double minlat,double maxlat)
        {
            mMinLng = minlng;
            mMaxLng = maxlng;
            mMinLat = minlat;
            mMaxLat = maxlat;
        }
        public double MinLat
        {
            get { return mMinLat; }
        }
        public double MaxLat
        {
            get { return mMaxLat; }
        }
        public double MinLng
        {
            get { return mMinLng; }
        }
        public double MaxLng
        {
            get { return mMaxLng; }
        }
    }

}
