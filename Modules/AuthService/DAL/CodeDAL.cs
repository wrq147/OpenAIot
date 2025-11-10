using AuthService.Model;
using Common;
using MyAccess.Core;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthService.DAL
{
    /// <summary>
    /// 各种代码数据层
    /// </summary>
    public class CodeDAL : BaseDbSupport
    {
        public async Task<List<MZ_Area>> SelectCodeList()
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select Id,ParentId,Name from mz_area order by Id asc")
                    .DoAsync<DoQuerySql<MZ_Area>>()).ToList();
            }
        }

        public async Task<List<MZ_Area>> SelectCodeListByCode(List<string> codes)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_area where Id in (").AppendParam(codes).Append(")")
                    .DoAsync<DoQuerySql<MZ_Area>>()).ToList();
            }
        }
        public async Task<MZ_Area> SelectArea(string code)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_area where Id=").AppendParam(code)
                    .DoAsync<DoQuerySql<MZ_Area>>()).ToFirst();
            }
        }
        public async Task<MZ_Area> SelectAreaByLatLng(double lng, double lat, int level)
        {
            using (DbHelp db = CreateDB())
            {
                string pointstr = $"POINT({lng} {lat})";

                string geohash = GeoHash.Encode(lat, lng, level);

                string geolikestr = $"g.GeoHash like '{geohash}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, 1 })}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, -1 })}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, -1 })}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, 1 })}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, 0 })}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 0, 1 })}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, 0 })}%'";
                geolikestr += $" or g.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 0, -1 })}%'";
                return (await new SqlBuilder(db).Append("select e.* from mz_area_geo g inner join mz_area e on g.id=e.Id where e.LevelType='3' and (" + geolikestr + ") and ST_Intersects(g.polygon,ST_GeomFromText('" + pointstr + "',0))=1")
                    .DoAsync<DoQuerySql<MZ_Area>>()).ToFirst();
            }
        }
        public async Task<MZ_Area> SelectAreaByCity(string pro, string city)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select Id,ParentId,Name,Lng,Lat from mz_area where Province=").AppendParam(pro).Append(" and City=").AppendParam(city).Append(" and LevelType='3'")
                    .DoAsync<DoQuerySql<MZ_Area>>()).ToFirstOrDefault(null);
            }
        }
        public async Task<MZ_Area> SelectAreaByDistrict(string pro, string city, string district)
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select Id,ParentId,Name,Lng,Lat from mz_area where Province=").AppendParam(pro).Append(" and City=").AppendParam(city).Append(" and District=").AppendParam(district)
                    .DoAsync<DoQuerySql<MZ_Area>>()).ToFirstOrDefault(null);
            }
        }
        public async Task<List<MZ_Industry>> SelectIndustryList()
        {
            using (DbHelp db = CreateDB())
            {
                return (await new SqlBuilder(db).Append("select * from mz_industry order by Sort asc")
                    .DoAsync<DoQuerySql<MZ_Industry>>()).ToList();
            }
        }
    }
}
