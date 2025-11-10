using Common;
using AfterService.Model;
using MyAccess.DB;
using NPOI.SS.Formula.Functions;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AfterService.DAL
{
    public class RoomDeviceDAL : BaseRepository<MZ_RoomDevice>
    {
        public virtual async Task<List<Out_DeviceWithRoome>> QueryDeivceWithRoomList(List<string> devIds)
        {
            var dqs = new SqlBuilder(help).Append("select Id as RoomId,Name as RoomName,TargetId,CategoryId,LeaderId,Helper from mz_room_device_v where TargetId in (").AppendParam(devIds).Append(")");
            return (await dqs.DoAsync<DoQuerySql<Out_DeviceWithRoome>>()).ToList();
        }
        public virtual async Task<List<Out_DeviceWithRoome>> QueryDeivceWithRoom(string devId)
        {
            var dqs = new SqlBuilder(help).Append("select Id as RoomId,Name as RoomName,TargetId,CategoryId,LeaderId,Helper from mz_room_device_v where TargetId=").AppendParam(devId);
            return (await dqs.DoAsync<DoQuerySql<Out_DeviceWithRoome>>()).ToList();
        }
        public virtual async Task<int> InsertOrIgnore(List<MZ_RoomDevice> list)
        {
            string tsql = new SqlBuilder(help).Insert(list).ToString();
            int startIndx = tsql.IndexOf("insert", StringComparison.OrdinalIgnoreCase);
            if (startIndx > -1)
            {
                tsql = tsql.Remove(startIndx, "insert".Length);
                tsql = "insert ignore " + tsql;
            }
            return (await new SqlBuilder(help).Append(tsql).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<int> ClearJunk(long orgId)
        {
            return (await new SqlBuilder(help).Append("delete from mz_room_device where not EXISTS(select Id from mz_iot_device where Id=mz_room_device.TargetId) and OrgId=").AppendParam(orgId).DoAsync<DoExecSql>()).RowCount;
        }
    }
}
