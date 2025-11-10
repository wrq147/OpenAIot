using Common;
using FlowService.Model;
using MyAccess.DB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlowService.DAL
{
    public class FlowDeviceDAL : BaseDbSupport
    {
        public virtual async Task<Out_DeviceInfo>  QueryDeviceInfo(string id)
        {
            return (await new SqlBuilder(help).Append("select Id,OrgId,OwnerOrgId,UseOrgId,UseUserId from mz_iot_device where Id=").AppendParam(id).DoAsync<DoQuerySql<Out_DeviceInfo>>()).ToFirst();
        }
        public virtual async Task<List<Out_FlowDevice>> SelectFlowDeviceList(List<string> ids)
        {
            return (await new SqlBuilder(help).Append("select d.Id,d.DeviceNumber,d.ProductId,d.Name,d.DeviceId,p.NetworkWay,p.ModelTSL from mz_iot_device d left join mz_iot_product p on d.ProductId=p.Id where d.Id in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<Out_FlowDevice>>()).ToList();
        }
        public virtual async Task<List<long>> QueryLeadersByDevice(long orgId, string deviceId)
        {
            var dqs = await new SqlBuilder(help).Append("select LeaderId from mz_room_device_v where OrgId=").AppendParam(orgId).Append(" and TargetId=").AppendParam(deviceId).DoAsync<DoQuerySql<long>>();
            return dqs.ToList().Distinct().ToList();
        }
    }
}
