using Common;
using Common.Share;
using AfterService.Model;
using IoTService.Models;
using MyAccess.DB;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace AfterService.DAL
{
    public class DevPlaneDAL : BaseRepository<MZ_PlaneType>
    {
        public virtual async Task<List<ObjectItem>> SelectPlaneTypeNames(IUserInfo user)
        {
            return (await new SqlBuilder(help).Append("select Name as name,Id as value from mz_plane_type where OrgId=").AppendParam(user.OrgId).DoAsync<DoQuerySql<ObjectItem>>()).ToList();
        }
        public virtual async Task<List<Out_PlaneFlowItem>> DeviceFlowList(string appendSql, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<Out_PlaneFlowItem>().Append("select DISTINCT t.FlowTemplateId,f.Icon,f.Background,f.Name as FlowTemplateName from mz_plane_type t left join mz_flow_template f on t.FlowTemplateId=f.Id where (t.OrgId=").AppendParam(user.OrgId).Append(" or t.IsFilterLeader=0)").Append(appendSql).ToListAsync();
        }
        public virtual async Task<List<MZ_IotDevice>> SelectPlaneDeviceList(string id)
        {
            return await new SqlBuilder(help).Query<MZ_IotDevice>().Append("select d.* from mz_plane_target t left join mz_iot_device d on t.TargetId=d.Id where t.TargetType=0 and t.PlaneId=").AppendParam(id).ToListAsync();
        }
        public virtual async Task<List<Out_KFProtocalName>> SelectPlaneProductList(string id)
        {
            return await new SqlBuilder(help).Query<Out_KFProtocalName>().Append("select p.Id,p.OrgId,p.Name,p.PhotoUrl from mz_plane_target t left join mz_product p  on t.TargetId=p.Id where t.TargetType=1 and t.PlaneId=").AppendParam(id).ToListAsync();
        }
        public virtual async Task<List<MZ_Room>> SelectPlaneRoomList(string id)
        {
            return await new SqlBuilder(help).Query<MZ_Room>().Append("select * from mz_room where Id in (select TargetId from mz_plane_target where PlaneId=").AppendParam(id).Append(" and TargetType=2)").ToListAsync();
        }
        public virtual async Task<List<MZ_IotDevice>> SelectPlaneProductDeviceList(string id)
        {
            return await new SqlBuilder(help).Query<MZ_IotDevice>().Append("select * from mz_iot_device where EXISTS(select 1 from mz_plane_target where PlaneId=").AppendParam(id).Append(" and TargetType=1 and TargetId=mz_iot_device.ProductId)").ToListAsync();
        }
        public virtual async Task<List<MZ_IotDevice>> SelectPlaneRoomDeviceList(string id)
        {
            return await new SqlBuilder(help).Query<MZ_IotDevice>().Append("select d.* from mz_room_device_v rd left join mz_iot_device d on rd.TargetId=d.Id where rd.Id in (select TargetId from mz_plane_target where PlaneId=").AppendParam(id).Append(" and TargetType=2)").ToListAsync();
        }
        public virtual async Task<bool> ExistPlaneDevice(string planeId, string targetId)
        {
            return await new SqlBuilder(help).Query<MZ_PlaneTarget>().SomeAsync(x => x.PlaneId == planeId && x.TargetType == 0 && x.TargetId == targetId);
        }
        public virtual async Task<bool> ExistPlaneProduct(string planeId, string targetId)
        {
            return await new SqlBuilder(help).Query<MZ_PlaneTarget>().SomeAsync(x => x.PlaneId == planeId && x.TargetType == 1 && x.TargetId == targetId);
        }
        public virtual async Task<bool> ExistPlaneRoom(string planeId, string targetId)
        {
            return await new SqlBuilder(help).Query<MZ_PlaneTarget>().SomeAsync(x => x.PlaneId == planeId && x.TargetType == 2 && x.TargetId == targetId);
        }
        public virtual async Task<int> AddPlaneDevice(List<MZ_PlaneTarget> list)
        {
            return await new SqlBuilder(help).Insert(list).DoAsync();
        }
        public virtual async Task<int> RemovePlaneDevice(string planeId)
        {
            return await new SqlBuilder(help).Delete<MZ_PlaneTarget>(x => x.PlaneId == planeId).DoAsync();
        }
        public virtual async Task<int> AddPlaneEvent(List<MZ_PlaneEvent> list)
        {
            return await new SqlBuilder(help).Insert(list).DoAsync();
        }
        public virtual async Task<int> RemovePlaneEvent(string planeId)
        {
            return await new SqlBuilder(help).Delete<MZ_PlaneEvent>(x => x.PlaneId == planeId).DoAsync();
        }
        public virtual async Task<List<MZ_PlaneEvent>> QueryPlaneEvents(string planeId)
        {
            return await new SqlBuilder(help).Query<MZ_PlaneEvent>().Where(x => x.PlaneId == planeId).ToListAsync();
        }
        public virtual async Task<List<MZ_PlaneEvent>> QueryPlaneEventsByEvt(long orgId, long ownOrgId, long usingOrgId, string evt)
        {
            return await new SqlBuilder(help).Query<MZ_PlaneEvent>().Where(x => (x.OrgId == orgId || x.OrgId == ownOrgId || x.OrgId == usingOrgId) && x.EventId == evt).ToListAsync();
        }
        public virtual async Task<List<MZ_PlaneTarget>> QueryPlaneTargets(string planeId)
        {
            return await new SqlBuilder(help).Query<MZ_PlaneTarget>().Where(x => x.PlaneId == planeId).ToListAsync();
        }
        public virtual async Task<PageObject<MZ_IotDevice>> DevListPage(In_PlaneDevList query)
        {
            var tsql = new SqlBuilder(help).Query<MZ_IotDevice>().Append("select d.*,p.ProductName,rd.Name as RoomName from mz_iot_device d inner join mz_product_batch b on d.Id=b.Id left join mz_product p on b.ProductId=p.Id left join mz_room_device_v rd on d.Id=rd.TargetId");
            tsql = tsql.Append(" where (");
            var targets = await QueryPlaneTargets(query.Id);
            var tdevIds = targets.Where(x => x.TargetType == 0).Select(x => x.TargetId).ToList();
            var tproIds = targets.Where(x => x.TargetType == 1).Select(x => x.TargetId).ToList();
            var troomIds = targets.Where(x => x.TargetType == 2).Select(x => x.TargetId).ToList();
            bool hascc = false;
            if (tdevIds.Count > 0)
            {
                tsql = tsql.Append("d.Id in (").AppendParam(tdevIds).Append(")");
                hascc = true;
            }
            if (tproIds.Count > 0)
            {
                if (hascc)
                {
                    tsql = tsql.Append(" or ");
                }
                tsql = tsql.Append("p.Id in (").AppendParam(tproIds).Append(")");
                hascc = true;
            }
            if (troomIds.Count > 0)
            {
                if (hascc)
                {
                    tsql = tsql.Append(" or ");
                }
                tsql = tsql.Append("rd.Id in (").AppendParam(troomIds).Append(")");
                hascc = true;
            }
            tsql = tsql.Append(")");
            return await tsql.Then(query.beginTime != null, sq => sq.Append(" and d.CreateOn >= ").AppendParam(query.beginTime))
                            .Then(!string.IsNullOrEmpty(query.Key), sq =>
                            {
                                var tmpkey = query.Key.SqlLikeFilter();
                                sq.Append(" and (d.DeviceNumber like ").AppendParam("%" + tmpkey + "%").Append(" or d.DeviceId like ").AppendParam("%" + tmpkey + "%").Append(" or d.Name like ").AppendParam("%" + tmpkey + "%").Append(")");
                            })
      .Then(query.endTime != null, sq => sq.Append(" and d.CreateOn <= ").AppendParam(query.endTime))
      .GeneratePageObjectAsync(query, "d.CreateOn desc");
        }

    }
}
