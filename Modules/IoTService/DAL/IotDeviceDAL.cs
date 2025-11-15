using AuthService;
using Common;
using Common.Share;
using FluentMigrator.Runner.Generators.SQLite;
using IoTService.Models;
using JiebaNet.Segmenter;
using MonitorService.Hardware;
using MyAccess.DB;
using NPOI.POIFS.Crypt.Agile;
using Org.BouncyCastle.Crypto;
using Quartz.Impl.AdoJobStore.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.DAL
{
    public class IotDeviceDAL : BaseRepository<MZ_IotDevice>
    {
        public virtual async Task<List<MZ_IotDevice>> SelectDevicesByIdx(string productId, int idx)
        {
            return await new SqlBuilder(help).Query<MZ_IotDevice>().Where(x => x.ProductId == productId && x.DeviceUpIdx == idx, "Id,DeviceId").ToListAsync();
        }
        public virtual async Task<int> SelectDStatusByOrgId(Data_ServerTokenInfo user, long orgId, string dStatus)
        {
            var scope = await user.GetScope(this.Provider, "/AfterService/Room/List");
            string scopestr = string.Empty;
            if (scope != null)
            {
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", string.Empty, false, false, false);
            }



            List<string> keys = new List<string>();
            keys.Add(orgId.ToString());
            List<string> xxkeeys = new List<string>();
            xxkeeys.Add(user.OrgId.ToString());
            return await new SqlBuilder(help).Query<int>().Append("select count(1) from  mz_iot_device d left join mz_room_device_v rd on d.Id=rd.TargetId where d.DState=").AppendParam(dStatus).Append(" and (d.UseUserId=" + user.UserId + " or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", xxkeeys).Append(" or d.UseOrgId=" + user.OrgId + ") and (")
                .FullSearch("d.OwnerOrgPath", keys).Append(" or rd.TargetOrgId='" + orgId + "')" + scopestr).ToFirstAsync();
        }
        public virtual async Task<Out_DevStatus> SelectStatusByOrgId(Data_ServerTokenInfo user, long orgId)
        {
            var scope = await user.GetScope(this.Provider, "/AfterService/Room/List");
            string scopestr = string.Empty;
            if (scope != null)
            {
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", string.Empty, false, false, false);
            }



            List<string> keys = new List<string>();
            keys.Add(orgId.ToString());
            List<string> tmpsss = new List<string>();
            tmpsss.Add(user.OrgId.ToString());
            return await new SqlBuilder(help).Query<Out_DevStatus>().Append("select sum(A) as TotalCount,sum(B) as OnlineCount,sum(C) as OfflineCount,sum(D) as UnknowCount from (select 1 as A,case when d.Online=1 then 1 else 0 end as B,case when d.Online=0 then 1 else 0 end as C,case when d.Online=2 then 1 else 0 end as D from mz_iot_device d left join mz_room_device_v rd on d.Id=rd.TargetId where (d.UseUserId=" + user.UserId + " or d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", tmpsss).Append(" or d.UseOrgId=" + user.OrgId + ") and (")
                .FullSearch("d.OwnerOrgPath", keys).Append(" or rd.TargetOrgId='" + orgId + "')" + scopestr + ") as ss").ToFirstAsync();
        }
        public virtual async Task<PageObject<MZ_IotDevice>> SelectWithGroupPage(In_DeviceListPage query, string groupPath, string classPath, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<MZ_IotDevice>().Append("select d.*,g.GroupName,p.Name as ProductName from mz_iot_device d left join mz_iot_group g on d.GroupId = g.Id left join mz_iot_product_v p on d.ProductId=p.Id where d.OrgId=").AppendParam(user.OrgId)
            .Then(!string.IsNullOrEmpty(groupPath), sq => sq.Append(" and g.Path like ").AppendParam(groupPath + "%"))
            .Then(!string.IsNullOrEmpty(classPath), sq => sq.Append(" and p.Path like ").AppendParam(classPath + "%"))
            .Then(query.Online != null, sq => sq.Append(" and d.Online=").AppendParam(query.Online))
            .Then(!string.IsNullOrEmpty(query.DState), sq => sq.Append(" and d.DState=").AppendParam(query.DState))
            .Then(query.ProductId != null, sq => sq.Append(" and d.ProductId=").AppendParam(query.ProductId))
            .Then(query.Ids != null && query.Ids.Length > 0, sq => sq.Append(" and d.Id in (").AppendParam(query.Ids).Append(")"))
            .Then(query.DtuIds != null && query.DtuIds.Length > 0, sq => sq.Append(" and d.DeviceId in (").AppendParam(query.DtuIds).Append(")"))
            .Then(query.ProductList != null && query.ProductList.Length > 0, sq => sq.Append(" and d.ProductId in (").AppendParam(query.ProductList).Append(")"))
            .Then(!string.IsNullOrEmpty(query.DeviceId), sq => sq.Append(" and d.DeviceId like ").AppendParam(query.DeviceId.SqlLikeFilter() + "%"))
            .Then(!string.IsNullOrEmpty(query.DeviceNumber), sq => sq.Append(" and d.DeviceNumber like ").AppendParam(query.DeviceNumber.SqlLikeFilter() + "%"))
            .Then(query.Numbers != null && query.Numbers.Length > 0, sq => sq.Append(" and d.DeviceNumber in (").AppendParam(query.Numbers).Append(")"))
            .Then(!string.IsNullOrEmpty(query.Key), sq =>
            {
                string tkey = query.Key.SqlLikeFilter();
                var keyarr = new JiebaSegmenter().CutForSearch(query.Key);
                sq.Append(" and (d.DeviceId like ").AppendParam(tkey + "%")
                .Append(" or d.DeviceNumber like ").AppendParam(tkey + "%")
                .Append(" or d.Name like ").AppendParam("%" + tkey + "%")
                .Append(" or ").FullSearch("d.KeyWords", keyarr)
                .Append(")");
            })
            .Then(!string.IsNullOrEmpty(query.Keywords), sq =>
            {
                var keyarr = new JiebaSegmenter().CutForSearch(query.Keywords);
                sq.Append(" and ").FullSearch("d.KeyWords", keyarr);
            })
            .Then(query.HasDeviceId != null, sq =>
            {
                if (query.HasDeviceId == true)
                {
                    sq.Append(" and d.DeviceId<>''");
                }
                else
                {
                    sq.Append(" and d.DeviceId=''");
                }
            })
            .Then(query.TagConditions != null && query.TagConditions.Count > 0, sq =>
            {
                foreach (var condi in query.TagConditions)
                {

                    switch (condi.optionType)
                    {
                        case "enum":
                        case "string":
                            {
                                sq.Append(" and EXISTS(select Id from mz_iot_device_tag where d.Id=Id and Code=").AppendParam(condi.code);
                                string cval = (condi.val ?? "").ToString();
                                if (condi.compare == "eq")
                                {
                                    sq.Append(" and Value=").AppendParam(cval);
                                }
                                else if (condi.compare == "ne")
                                {
                                    sq.Append(" and Value<>").AppendParam(cval);
                                }
                                else if (condi.compare == "like")
                                {
                                    sq.Append(" and Value like ").AppendParam("%" + cval + "%");
                                }
                                sq.Append(")");
                            }
                            break;
                        case "int":
                            {
                                sq.Append(" and EXISTS(select Id from mz_iot_device_tag where d.Id=Id and Code=").AppendParam(condi.code);
                                long cval = Convert.ToInt64(condi.val);
                                switch (condi.compare)
                                {
                                    case "eq":
                                        sq.Append(" and NumValue=").AppendParam(cval);
                                        break;
                                    case "ne":
                                        sq.Append(" and NumValue<>").AppendParam(cval);
                                        break;
                                    case "gt":
                                        sq.Append(" and NumValue>").AppendParam(cval);
                                        break;
                                    case "lt":
                                        sq.Append(" and NumValue<").AppendParam(cval);
                                        break;
                                    case "ge":
                                        sq.Append(" and NumValue>=").AppendParam(cval);
                                        break;
                                    case "le":
                                        sq.Append(" and NumValue<=").AppendParam(cval);
                                        break;
                                }
                                sq.Append(")");
                            }
                            break;
                        case "float":
                            {
                                sq.Append(" and EXISTS(select Id from mz_iot_device_tag where d.Id=Id and Code=").AppendParam(condi.code);
                                double cval = Convert.ToDouble(condi.val);
                                switch (condi.compare)
                                {
                                    case "eq":
                                        sq.Append(" and NumValue=").AppendParam(cval);
                                        break;
                                    case "ne":
                                        sq.Append(" and NumValue<>").AppendParam(cval);
                                        break;
                                    case "gt":
                                        sq.Append(" and NumValue>").AppendParam(cval);
                                        break;
                                    case "lt":
                                        sq.Append(" and NumValue<").AppendParam(cval);
                                        break;
                                    case "ge":
                                        sq.Append(" and NumValue>=").AppendParam(cval);
                                        break;
                                    case "le":
                                        sq.Append(" and NumValue<=").AppendParam(cval);
                                        break;
                                }
                                sq.Append(")");
                            }
                            break;
                        case "geo":
                            break;
                        case "date":
                            {

                                DateTime? valDate = null;
                                if (condi.val is string valstr)
                                {
                                    DateTime tmpdate;
                                    if (DateTime.TryParse(valstr, out tmpdate))
                                    {
                                        valDate = tmpdate;
                                    }
                                }
                                else
                                {
                                    long valll = Convert.ToInt64(condi.val);
                                    valDate = MyAccess.Core.TypeConvert.Unix2Time(valll);
                                }
                                if (valDate == null)
                                {
                                    break;
                                }
                                else
                                {
                                    sq.Append(" and EXISTS(select Id from mz_iot_device_tag where d.Id=Id and Code=").AppendParam(condi.code);
                                    string clientTZ = TAAction.Current.Context.Request.Header["TZ"];
                                    if (!string.IsNullOrEmpty(clientTZ))
                                    {
                                        int tz;
                                        if (int.TryParse(clientTZ, out tz))
                                        {
                                            valDate = TimeZoneInfo.ConvertTimeFromUtc(valDate.Value.AddMinutes(tz), TimeZoneInfo.Local);
                                        }
                                    }
                                    long tmpnumval = MyAccess.Core.TypeConvert.Time2Unix(valDate.Value);
                                    switch (condi.compare)
                                    {
                                        case "gt":
                                            sq.Append(" and NumValue>").AppendParam(tmpnumval);
                                            break;
                                        case "lt":
                                            sq.Append(" and NumValue<").AppendParam(tmpnumval);
                                            break;
                                        case "ge":
                                            sq.Append(" and NumValue>=").AppendParam(tmpnumval);
                                            break;
                                        case "le":
                                            sq.Append(" and NumValue<=").AppendParam(tmpnumval);
                                            break;
                                    }
                                    sq.Append(")");
                                }

                            }
                            break;
                        case "boolean":
                            {
                                sq.Append(" and EXISTS(select Id from mz_iot_device_tag where d.Id=Id and Code=").AppendParam(condi.code);
                                string cval = Convert.ToBoolean(condi.val) ? "true" : "false";
                                if (condi.compare == "eq")
                                {
                                    sq.Append(" and Value=").AppendParam(cval);
                                }
                                else if (condi.compare == "ne")
                                {
                                    sq.Append(" and Value<>").AppendParam(cval);
                                }
                                sq.Append(")");
                            }
                            break;
                    }

                }

            })
            .Then(query.beginTime != null, sq => sq.Append(" and d.CreateOn >= ").AppendParam(query.beginTime))
            .Then(query.endTime != null, sq => sq.Append(" and d.CreateOn <= ").AppendParam(query.endTime))
            .GeneratePageObjectAsync(query, "d.Id desc");
        }
        /// <summary>
        /// 查询指定产品的所有设备Id列表
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> SelectIdList(string productId)
        {
            return (await new SqlBuilder(help).Append("select Id from mz_iot_device where ProductId=").AppendParam(productId).DoAsync<DoQuerySql<string>>()).ToList();
        }
        /// <summary>
        /// 查询指定产品的所有在线设备
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public virtual async Task<List<string>> SelectDtuIdListByOnline(string productId)
        {
            return (await new SqlBuilder(help).Append("select DeviceId from mz_iot_device where Online=1 and ProductId=").AppendParam(productId).DoAsync<DoQuerySql<string>>()).ToList();
        }
        /// <summary>
        /// 指定分组路径下面是否有设备
        /// </summary>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistDevice(string groupPath)
        {
            var rs = await new SqlBuilder(help).Query<MZ_IotDevice>().Append("select d.Id from mz_iot_device d left join mz_iot_group g on d.GroupId = g.Id where g.Path like ")
       .AppendParam(groupPath + '%').Append(" limit 1").ToFirstAsync();
            return rs != null ? true : false;
        }


        /// <summary>
        /// 批量更新拥有者
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="newOrgId"></param>
        /// <param name="clearUse"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateBatchOwnerOrgId(List<string> ids, long newOrgId, bool clearUse)
        {
            var dtuIds = await IdsToDtuIds(ids);
            IotRedisHelper redis = this.Provider.GetService<IotRedisHelper>();
            int result;
            if (clearUse)
            {
                var des = await new SqlBuilder(help).Append("update mz_iot_device set UseOrgId=0,UseUserId=0,OwnerOrgId=" + newOrgId + ",OwnerOrgPath=concat(SUBSTRING_INDEX(OwnerOrgPath,',',-25),'" + newOrgId + ",') where Id in (").AppendParam(ids).Append(")").DoAsync<DoExecSql>();
                result = des.RowCount;
            }
            else
            {
                var des = await new SqlBuilder(help).Append("update mz_iot_device set OwnerOrgId=" + newOrgId + ",OwnerOrgPath=concat(SUBSTRING_INDEX(OwnerOrgPath,',',-25),'" + newOrgId + ",') where Id in (").AppendParam(ids).Append(")").DoAsync<DoExecSql>();
                result = des.RowCount;
            }

            foreach (var dtuId in dtuIds)
            {
                await redis.HashDeleteAsync("Device:" + dtuId, "$DeviceOrgIds");
            }
            return result;
        }
        /// <summary>
        /// 批量清除设备经过的组织路径
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<int> ClearOwnerOrgPath(List<string> ids, long orgId)
        {
            var des = await new SqlBuilder(help).Append("update mz_iot_device set OwnerOrgPath=REPLACE(OwnerOrgPath,'" + orgId + ",','') where Id in (").AppendParam(ids).Append(")").DoAsync<DoExecSql>();
            return des.RowCount;
        }
        /// <summary>
        /// 批量更改设备的使用者组织
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateUseOrgId(List<string> ids, long orgId)
        {
            var dtuIds = await IdsToDtuIds(ids);
            IotRedisHelper redis = this.Provider.GetService<IotRedisHelper>();

            MZ_IotDevice device = new MZ_IotDevice();
            device.UseOrgId = orgId;
            var result = await new SqlBuilder(help).Update(device, x => ids.Contains(x.Id)).DoAsync();

            foreach (var dtuId in dtuIds)
            {
                await redis.HashDeleteAsync("Device:" + dtuId, "$DeviceOrgIds");
            }
            return result;
        }
        public virtual async Task SetNeedUpdateKeywords(string path)
        {
            await new SqlBuilder(help).Append("update mz_iot_device set NeedUpdateKey=1 where EXISTS(select Id from mz_iot_product_v where Id=mz_iot_device.ProductId and Path like '" + path + "%')").DoAsync<DoExecSql>();
        }
        public virtual async Task UpdateKeywords(List<MZ_IotDevice> devices)
        {
            var tmpsql = new SqlBuilder(help).Append(@"UPDATE mz_iot_device
SET NeedUpdateKey=0,KeyWords = CASE Id");
            List<string> ids = new List<string>();
            foreach (var item in devices)
            {
                ids.Add(item.Id);
                tmpsql.Append(" WHEN '" + item.Id + "' THEN ").AppendParam(item.KeyWords);
            }
            tmpsql.Append(" END WHERE Id IN (").AppendParam(ids).Append(")");
            await tmpsql.DoAsync<DoExecSql>();
        }
        /// <summary>
        /// Id转通讯id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<List<string>> IdsToDtuIds(List<string> ids)
        {
            var des = new SqlBuilder(help).Append("select DeviceId from mz_iot_device where Id in (").AppendParam(ids).Append(")");
            return (await des.DoAsync<DoQuerySql<string>>()).ToList();
        }
        public virtual async Task<string> IdToDtuId(string id)
        {
            var des = new SqlBuilder(help).Append("select DeviceId from mz_iot_device where Id=").AppendParam(id);
            var tmplist = (await des.DoAsync<DoQuerySql<string>>()).ToList();
            if (tmplist.Count > 0)
            {
                return tmplist[0];
            }
            else
            {
                return null;
            }
        }
    }
}
