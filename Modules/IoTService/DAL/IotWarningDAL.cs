using AuthService;
using Common;
using Common.Share;
using InfluxDB.Client.Api.Domain;
using IoTService.Models;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTService.DAL
{
    public class IotWarningDAL : BaseRepository<MZ_IotWarning>
    {
        public virtual async Task<long> InsertAndReturn(MZ_IotWarning entity)
        {
            return (await new SqlBuilder(help).Insert(entity).DoReturnIdentityAsync()).LastInsertedId;
        }
        public virtual async Task<int> SelectWaitCount(IUserInfo user)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_iot_warning w inner join mz_iot_device d on w.DeviceId=d.Id where w.Status=0 and w.OrgId=" + user.OrgId).DoAsync<DoQueryScalar>()).GetValueInt();
        }

        public virtual async Task<int> SelectWaitCountByOrgId(Data_ServerTokenInfo user, long orgId)
        {
            var scope = await user.GetScope(this.Provider, "/AfterService/Room/List");
            string scopestr = string.Empty;
            if (scope != null)
            {
                scopestr = scope.GenerateFilter("rd.DeptId", "rd.LeaderId", string.Empty, false, false, false);
            }
            List<string> keys = new List<string>();
            keys.Add(orgId.ToString());
            return (await new SqlBuilder(help).Append("select count(1) from mz_iot_warning w inner join mz_iot_device d on w.DeviceId=d.Id left join mz_room_device_v rd on d.Id=rd.TargetId where w.Status=0 and w.OrgId=" + user.OrgId + " and (").FullSearch("d.OwnerOrgPath", keys).Append(" or rd.TargetOrgId='" + orgId + "')" + scopestr).DoAsync<DoQueryScalar>()).GetValueInt();
        }
        public virtual async Task<PageObject<MZ_IotWarning>> SelectWithPage(In_WarningListPage query, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<MZ_IotWarning>().Append("select w.*,d.PhotoUrl as DevicePhotoUrl,d.Name as DeviceName,p.Name as ProductName from mz_iot_warning w inner join mz_iot_device d on w.DeviceId = d.Id left join mz_iot_product p on d.ProductId=p.Id where w.OrgId=" + user.OrgId)
            .Then(!string.IsNullOrEmpty(query.Name), sql =>
            {
                string tmpname = "%" + query.Name.SqlLikeFilter() + "%";
                sql.Append(" and (w.Name like ").AppendParam(tmpname).Append(" or d.Name like ").AppendParam(tmpname).Append(" or p.Name like ").AppendParam(tmpname).Append(")");
            })
            .Then(!string.IsNullOrEmpty(query.WarnCode), sql =>
            {
                sql.Append(" and w.Code=").AppendParam(query.WarnCode);
            })
            .Then(!string.IsNullOrEmpty(query.DeviceId), sql =>
            {
                sql.Append(" and w.DeviceId=").AppendParam(query.DeviceId);
            })
            .Then(!string.IsNullOrEmpty(query.DeviceNumber), sql =>
            {
                sql.Append(" and d.DeviceNumber=").AppendParam(query.DeviceNumber);
            })
            .Then(!string.IsNullOrEmpty(query.ProductId), sql =>
            {
                sql.Append(" and d.ProductId=").AppendParam(query.ProductId);
            })
            .Then(query.Status != null, sql =>
               {
                   sql.Append(" and w.Status=").AppendParam(query.Status);
               })
            .GeneratePageObjectAsync(query, "w.CreateOn desc");
        }
        public virtual async Task<List<string>> SelectWarningDeviceList(List<string> devIds, long orgId)
        {
            var dqs = await new SqlBuilder(help).Append("SELECT DeviceId from mz_iot_warning where OrgId=" + orgId + " and Status=0 and DeviceId in (").AppendParam(devIds).Append(") GROUP BY DeviceId HAVING Count(1)>0").DoAsync<DoQuerySql<string>>();
            return dqs.ToList();
        }
        public virtual async Task<string> SelectFlowTemplateName(long id)
        {
            var dqs = await new SqlBuilder(help).Append("select Name from mz_flow_template where Id=").AppendParam(id).DoAsync<DoQuerySql<string>>();
            return dqs.ToFirst();
        }
        public virtual async Task<Out_WarnDeviceInfo> SelectWarnDev(string number)
        {
            var dqs = await new SqlBuilder(help).Append("select d.DeviceId,w.Code from mz_iot_warning w left join mz_iot_device d on w.DeviceId=d.Id where w.WarnNumber=").AppendParam(number).DoAsync<DoQuerySql<Out_WarnDeviceInfo>>();
            return dqs.ToFirst();
        }
        public virtual async Task<List<Out_WarnDeviceInfo>> SelectWarnDevList(long orgId, byte status)
        {
            var dqs = await new SqlBuilder(help).Append("select d.DeviceId,w.Code from mz_iot_warning w left join mz_iot_device d on w.DeviceId=d.Id where w.OrgId=").AppendParam(orgId).Append(" and w.Status=").AppendParam(status).DoAsync<DoQuerySql<Out_WarnDeviceInfo>>();
            return dqs.ToList();
        }
        public virtual async Task<List<Out_WarnDeviceInfo>> SelectWarnDevList(long orgId, byte status, string code)
        {
            var dqs = await new SqlBuilder(help).Append("select d.DeviceId,w.Code from mz_iot_warning w left join mz_iot_device d on w.DeviceId=d.Id where w.OrgId=").AppendParam(orgId).Append(" and w.Status=").AppendParam(status).Append(" and w.Code=").AppendParam(code).DoAsync<DoQuerySql<Out_WarnDeviceInfo>>();
            return dqs.ToList();
        }
        public virtual async Task<List<Out_WarnDeviceInfo>> SelectWarnDevListById(long orgId, byte status, string devid)
        {
            var dqs = await new SqlBuilder(help).Append("select d.DeviceId,w.Code from mz_iot_warning w left join mz_iot_device d on w.DeviceId=d.Id where w.OrgId=").AppendParam(orgId).Append(" and w.Status=").AppendParam(status).Append(" and w.DeviceId=").AppendParam(devid).DoAsync<DoQuerySql<Out_WarnDeviceInfo>>();
            return dqs.ToList();
        }
        public virtual async Task<List<Out_WarnDeviceInfo>> SelectWarnDevListByAll(long orgId, byte status, string devid, string code)
        {
            var dqs = await new SqlBuilder(help).Append("select d.DeviceId,w.Code from mz_iot_warning w left join mz_iot_device d on w.DeviceId=d.Id where w.OrgId=").AppendParam(orgId).Append(" and w.Status=").AppendParam(status).Append(" and w.DeviceId=").AppendParam(devid).Append(" and w.Code=").AppendParam(code).DoAsync<DoQuerySql<Out_WarnDeviceInfo>>();
            return dqs.ToList();
        }
    }
}
