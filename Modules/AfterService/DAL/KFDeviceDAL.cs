using AuthService;
using Common;
using Common.Share;
using AfterService.Model;
using IoTService.Models;
using MyAccess.DB;
using System.Threading.Tasks;
using System.Collections.Generic;
using JiebaNet.Segmenter;
using MyAccess.Core;
using Microsoft.OpenApi.Writers;
using InfluxDB.Client.Api.Domain;

namespace AfterService.DAL
{
    public class KFDeviceDAL : BaseRepository<MZ_IotDevice>
    {
        public virtual async Task<PageObject<Out_KFProtocalName>> SelectKFProtocalList(long kfOrgId, In_KFProtocalPage query)
        {
            List<string> keys = new List<string>();
            keys.Add(kfOrgId.ToString());
            return await new SqlBuilder(help).Query<Out_KFProtocalName>()
                .Append("select Id,OrgId,Name,PhotoUrl from mz_iot_product where EXISTS(select 1 from mz_iot_device where mz_iot_product.Id=ProductId and (OrgId=" + kfOrgId + " or ").FullSearch("OwnerOrgPath", keys).Append(" or UseOrgId=" + kfOrgId + "))")
                .Then(!string.IsNullOrEmpty(query.Name), x =>
                {
                    x.Append(" and Name like ").AppendParam("%" + query.Name.SqlLikeFilter() + "%");
                })
                .Then(query.Pids != null && query.Pids.Length > 0, x =>
                {
                    x.Append(" and Id in (").AppendParam(query.Pids).Append(")");
                })
                .GeneratePageObjectAsync(query, "");
        }
        public virtual async Task<PageObject<Out_KFProductName>> SelectKFProductList(long kfOrgId, In_KFProductPage query)
        {
            List<string> keys = new List<string>();
            keys.Add(kfOrgId.ToString());
            return await new SqlBuilder(help).Query<Out_KFProductName>()
                .Append("select Id,OrgId,ProductName,PhotoUrl from mz_product where EXISTS(select 1 from mz_iot_device d inner join mz_product_batch b on d.Id=b.Id where mz_product.Id=b.ProductId and (d.OrgId=" + kfOrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(" or d.UseOrgId=" + kfOrgId + "))")
                .Then(!string.IsNullOrEmpty(query.Name), x =>
                {
                    x.Append(" and Name like ").AppendParam("%" + query.Name.SqlLikeFilter() + "%");
                })
                .Then(query.Pids != null && query.Pids.Length > 0, x =>
                {
                    x.Append(" and Id in (").AppendParam(query.Pids).Append(")");
                })
                .GeneratePageObjectAsync(query, "");
        }
        public virtual async Task<List<MZ_IotDevice>> SelectKFDeviceList(Data_ServerTokenInfo user, long kfOrgId)
        {
            List<string> keys = new List<string>();
            keys.Add(kfOrgId.ToString());
            List<string> keystwo = new List<string>();
            keystwo.Add(user.OrgId.ToString());
            return await new SqlBuilder(help).Query<MZ_IotDevice>().Append("select * from mz_iot_device where ").FullSearch("OwnerOrgPath", keys).Append(" and (OrgId=" + user.OrgId + " or ").FullSearch("OwnerOrgPath", keystwo).Append(" or UseOrgId=" + user.OrgId + ")").ToListAsync();
        }
        public virtual async Task<List<Out_KfDevice>> SelectKFDeviceListByRange(In_DevRangeList query, IUserInfo user, DataScope scope)
        {
            string geohash = GeoHash.Encode(query.Lat, query.Lng, query.level);
            string geolikestr = $"d.GeoHash like '{geohash}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, 1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, -1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, -1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, 1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, 0 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 0, 1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, 0 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 0, -1 })}%'";

            var tsql = new SqlBuilder(help).Append("select d.* from mz_iot_device d inner join mz_area a on d.AreaCode=a.Id left join mz_room_device_v rd on d.Id=rd.TargetId where ");
            tsql.Append("(d.UseUserId=" + user.UserId);
            if (user.OrgId > 0)
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                tsql.Append(" or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(" or d.UseOrgId=" + user.OrgId);
            }
            tsql.Append(")");
            if (query.NetStatus != null)
            {
                tsql.Append(" and d.Online=").AppendParam(query.NetStatus);
            }
            if (!string.IsNullOrEmpty(query.DState))
            {
                tsql.Append(" and d.DState=").AppendParam(query.DState);
            }
            string scopestr = string.Empty;
            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsql.Comparable.FullSearch("rd.Helper", keys));
                tsql.Append(scopestr);
            }

            tsql.Append(" and (" + geolikestr + ")");
            return (await tsql.DoAsync<DoQuerySql<Out_KfDevice>>()).ToList();
        }
        public virtual async Task<List<Out_DevAreaData>> SelectKFDevAreaDataListByRange(In_DevRangeAreaList query, IUserInfo user, DataScope scope)
        {
            string geohash = GeoHash.Encode(query.Lat, query.Lng, query.level);
            string geolikestr = $"d.GeoHash like '{geohash}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, 1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, -1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, -1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, 1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 1, 0 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 0, 1 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { -1, 0 })}%'";
            geolikestr += $" or d.GeoHash like '{GeoHash.Neighbor(geohash, new[] { 0, -1 })}%'";

            string leftnum = "Left(d.AreaCode,2) as AC";
            if (query.GroupBy == "City")
            {
                leftnum = "Left(d.AreaCode,4) as AC";
            }
            else if (query.GroupBy == "District")
            {
                leftnum = "d.AreaCode as AC";
            }


            var tsql = new SqlBuilder(help).Append("select count(1) as Count," + leftnum + " from mz_iot_device d inner join mz_area a on d.AreaCode=a.Id left join mz_room_device_v rd on d.Id=rd.TargetId where ");
            tsql.Append("(d.UseUserId=" + user.UserId);
            if (user.OrgId > 0)
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                tsql.Append(" or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(" or d.UseOrgId=" + user.OrgId);
            }
            tsql.Append(")");
            if (query.NetStatus != null)
            {
                tsql.Append(" and d.Online=").AppendParam(query.NetStatus);
            }
            if (!string.IsNullOrEmpty(query.DState))
            {
                tsql.Append(" and d.DState=").AppendParam(query.DState);
            }
            string scopestr = string.Empty;
            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsql.Comparable.FullSearch("rd.Helper", keys));
                tsql.Append(scopestr);
            }

            tsql.Append(" and (" + geolikestr + ")");
            if (query.GroupBy == "City")
            {
                tsql.Append(" and a.LevelType>1");
            }
            else if (query.GroupBy == "District")
            {
                tsql.Append(" and a.LevelType=3");
            }
            tsql.Append(" group by AC");
            var tmplist = (await tsql.DoAsync<DoQuerySql<Out_DevAreaData>>()).ToList();
            foreach (var tmp in tmplist)
            {
                if (tmp.AreaCode.Length == 2)
                {
                    tmp.AreaCode += "0000";
                }
                else if (tmp.AreaCode.Length == 4)
                {
                    tmp.AreaCode += "00";
                }
            }
            return tmplist;
        }
        public virtual async Task<List<Out_DevAreaData>> SelectKFDevAreaDataList(string parentPath, IUserInfo user, DataScope scope)
        {
            string leftnum = "Left(d.AreaCode,2) as AC";
            string filtersql = "";
            if (!string.IsNullOrEmpty(parentPath))
            {
                if (parentPath.IndexOf(",") == -1)
                {
                    leftnum = "Left(d.AreaCode,4) as AC";
                    filtersql = " and a.LevelType>1";
                }
                else
                {
                    leftnum = "d.AreaCode as AC";
                    filtersql = " and a.LevelType=3";
                }
            }

            var tsql = new SqlBuilder(help).Append("select count(1) as Count," + leftnum + " from mz_iot_device d inner join mz_area a on d.AreaCode=a.Id left join mz_room_device_v rd on d.Id=rd.TargetId where ");
            tsql.Append("(d.UseUserId=" + user.UserId);
            if (user.OrgId > 0)
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                tsql.Append(" or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(" or d.UseOrgId=" + user.OrgId);
            }
            tsql.Append(")");
            string scopestr = string.Empty;
            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsql.Comparable.FullSearch("rd.Helper", keys));
                tsql.Append(scopestr);
            }


            if (!string.IsNullOrEmpty(parentPath))
            {
                tsql.Append(" and a.ParentPath like concat(").AppendParam(parentPath).Append(",',%')");
            }
            tsql.Append(filtersql);
            tsql.Append(" group by AC");
            var tmplist = (await tsql.DoAsync<DoQuerySql<Out_DevAreaData>>()).ToList();
            foreach (var tmp in tmplist)
            {
                if (tmp.AreaCode.Length == 2)
                {
                    tmp.AreaCode += "0000";
                }
                else if (tmp.AreaCode.Length == 4)
                {
                    tmp.AreaCode += "00";
                }
            }
            return tmplist;
        }
        public virtual async Task<int> SelectKFDevAreaDStateCount(string parentPath, string dstate, IUserInfo user, DataScope scope)
        {
            var tsql = new SqlBuilder(help).Append("select count(1) as Count from mz_iot_device d inner join mz_area a on d.AreaCode=a.Id left join mz_room_device_v rd on d.Id=rd.TargetId where ");
            tsql.Append("(d.UseUserId=" + user.UserId);
            if (user.OrgId > 0)
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                tsql.Append(" or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(" or d.UseOrgId=" + user.OrgId);
            }
            tsql.Append(")");
            string scopestr = string.Empty;
            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsql.Comparable.FullSearch("rd.Helper", keys));
                tsql.Append(scopestr);
            }

            if (!string.IsNullOrEmpty(parentPath))
            {
                tsql.Append(" and a.ParentPath like concat(").AppendParam(parentPath).Append(",',%')");
            }

            if (!string.IsNullOrEmpty(dstate))
            {
                tsql.Append(" and d.DState=").AppendParam(dstate);
            }
            return (await tsql.DoAsync<DoQueryScalar>()).GetValueInt(0);
        }
        public virtual async Task<Out_DevAreaInfo> SelectKFDevAreaInfo(string parentPath, IUserInfo user, DataScope scope)
        {
            var tsql = new SqlBuilder(help).Append("select sum(case when d.Online = 0 THEN 1 ELSE 0 END) AS offCount,sum(case when d.Online = 1 THEN 1 ELSE 0 END) AS onCount,sum(case when d.Online = 2 THEN 1 ELSE 0 END) AS unCount from mz_iot_device d inner join mz_area a on d.AreaCode=a.Id left join mz_room_device_v rd on d.Id=rd.TargetId where ");
            tsql.Append("(d.UseUserId=" + user.UserId);
            if (user.OrgId > 0)
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                tsql.Append(" or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(" or d.UseOrgId=" + user.OrgId);
            }
            tsql.Append(")");
            string scopestr = string.Empty;
            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsql.Comparable.FullSearch("rd.Helper", keys));
                tsql.Append(scopestr);
            }

            if (!string.IsNullOrEmpty(parentPath))
            {
                tsql.Append(" and a.ParentPath like concat(").AppendParam(parentPath).Append(",',%')");
            }
            return (await tsql.DoAsync<DoQuerySql<Out_DevAreaInfo>>()).ToFirst();
        }
        public virtual async Task<PageObject<Out_KfDevice>> SelectWithGroupPage(In_KFDevListPage query, IUserInfo user, DataScope scope)
        {

            MZ_RoomCategory curcategory = null;
            if (!string.IsNullOrEmpty(query.RoomCategory))
            {
                curcategory = await new SqlBuilder(help).Query<MZ_RoomCategory>().Where(x => x.Id == query.RoomCategory).ToFirstAsync();
            }
            var tsql = new SqlBuilder(help).Query<Out_KfDevice>().Append("select d.*,p.ProductName,rd.Name as RoomName from mz_iot_device d inner join mz_product_batch b on d.Id=b.Id left join mz_product p on b.ProductId=p.Id left join mz_room_device_v rd on d.Id=rd.TargetId where ");
            tsql.Append("(d.UseUserId=" + user.UserId);

            if (user.OrgId > 0)
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                tsql.Append(" or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(" or d.UseOrgId=" + user.OrgId);
            }
            tsql.Append(")");

            if (query.TargetOrgId != null)
            {
                List<string> keys = new List<string>();
                keys.Add(query.TargetOrgId.ToString());
                tsql.Append(" and (").FullSearch("d.OwnerOrgPath", keys).Append(" or rd.TargetOrgId='" + query.TargetOrgId + "')");
            }

            string scopestr = string.Empty;
            if (scope != null)
            {
                List<string> keys = new List<string>();
                keys.Add(user.UserId.ToString());
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", "rd.LeaderId=" + user.UserId + " or " + tsql.Sql.Comparable.FullSearch("rd.Helper", keys));
                tsql.Append(scopestr);
            }

            return await tsql.Then(query.Online != null, sq => sq.Append(" and d.Online=").AppendParam(query.Online))
            .Then(query.ProductId != null, sq => sq.Append(" and b.ProductId=").AppendParam(query.ProductId))
            .Then(query.ProductList != null && query.ProductList.Length > 0, sq => sq.Append(" and b.ProductId in (").AppendParam(query.ProductList).Append(")"))
            .Then(!string.IsNullOrEmpty(query.Key), sq =>
            {
                var tmpkey = query.Key.SqlLikeFilter();
                var keyarr = new JiebaSegmenter().CutForSearch(query.Key);
                sq.Append(" and (d.DeviceNumber like ").AppendParam(tmpkey + "%")
                .Append(" or d.DeviceId like ").AppendParam(tmpkey + "%")
                .Append(" or d.Name like ").AppendParam("%" + tmpkey + "%")
                .Append(" or ").FullSearch("d.KeyWords", keyarr)
                .Append(")");
            })
            .Then(!string.IsNullOrEmpty(query.Keywords), sq =>
            {
                var keyarr = new JiebaSegmenter().CutForSearch(query.Keywords);
                sq.Append(" and ").FullSearch("d.KeyWords", keyarr);
            })
            .Then(!string.IsNullOrEmpty(query.DeviceNumber), sq => sq.Append(" and d.DeviceNumber like ").AppendParam("%" + query.DeviceNumber.SqlLikeFilter() + "%"))
            .Then(!string.IsNullOrEmpty(query.DeviceId), sq => sq.Append(" and d.DeviceId like ").AppendParam("%" + query.DeviceId.SqlLikeFilter() + "%"))
            .Then(!string.IsNullOrEmpty(query.RoomId), sq =>
            {
                sq.Append(" and rd.Id=").AppendParam(query.RoomId);
            })
            .Then(query.FilterRoom == true, sq =>
            {
                sq.Append(" and (rd.OrgId<>").AppendParam(user.OrgId).Append(" or rd.OrgId is null)");
            })
            .Then(curcategory != null, sq =>
            {
                sq.Append(" and EXISTS(select Id from mz_room_category where mz_room_category.OrgId=" + user.OrgId + " and rd.CategoryId=Id and Path like ").AppendParam(curcategory.Path + "%").Append(")");
            })
            .Then(!string.IsNullOrEmpty(query.DState), sq => sq.Append(" and d.DState like ").AppendParam(query.DState + "%"))
            .Then(query.beginTime != null, sq => sq.Append(" and d.CreateOn >= ").AppendParam(query.beginTime))
            .Then(query.endTime != null, sq => sq.Append(" and d.CreateOn <= ").AppendParam(query.endTime))
            .GeneratePageObjectAsync(query, "d.CreateOn desc");
        }
    }
}
