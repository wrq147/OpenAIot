using AuthService;
using Common;
using Common.Share;
using AfterService.Model;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AfterService.DAL
{
    public class DevPlaneTaskDAL : BaseRepository<MZ_PlaneTask>
    {
        public virtual async Task<Out_PlaneStatis> GetPlaneStatistics(IUserInfo user, string typeId)
        {
            return (await new SqlBuilder(help).Append("SELECT  SUM(CASE WHEN TaskStatus = 0 THEN 1 ELSE 0 END) AS doing,SUM(CASE WHEN TaskStatus = 1 THEN 1 ELSE 0 END) AS wait_work,SUM(CASE WHEN TaskStatus = 2 THEN 1 ELSE 0 END) AS working,SUM(CASE WHEN TaskStatus=3 THEN 1 ELSE 0 END) AS finish,SUM(CASE WHEN TaskStatus=4 THEN 1 ELSE 0 END) AS expired,SUM(CASE WHEN TaskStatus=5 THEN 1 ELSE 0 END) AS accepted,SUM(CASE WHEN TaskStatus=6 THEN 1 ELSE 0 END) AS noaccept,SUM(CASE WHEN TaskStatus=7 THEN 1 ELSE 0 END) AS invalid FROM mz_plane_task where OrgId=").AppendParam(user.OrgId).Append(" and PlanTypeId=").AppendParam(typeId).DoAsync<DoQuerySql<Out_PlaneStatis>>()).ToFirst();
        }
        public virtual async Task<List<Out_PlaneStatisItem>> GetPlaneStatisticsList(IUserInfo user, In_PlaneTaskStatisList query)
        {
            return (await new SqlBuilder(help).Append("SELECT  t.Name,p.PlanTypeId,SUM(CASE WHEN TaskStatus = 0 THEN 1 ELSE 0 END) AS doing,SUM(CASE WHEN TaskStatus = 1 THEN 1 ELSE 0 END) AS wait_work,SUM(CASE WHEN TaskStatus = 2 THEN 1 ELSE 0 END) AS working,SUM(CASE WHEN TaskStatus=3 THEN 1 ELSE 0 END) AS finish,SUM(CASE WHEN TaskStatus=4 THEN 1 ELSE 0 END) AS expired,SUM(CASE WHEN TaskStatus=5 THEN 1 ELSE 0 END) AS accepted,SUM(CASE WHEN TaskStatus=6 THEN 1 ELSE 0 END) AS noaccept,SUM(CASE WHEN TaskStatus=7 THEN 1 ELSE 0 END) AS invalid FROM mz_plane_task p inner join mz_plane_type t on p.PlanTypeId=t.Id where p.OrgId=").AppendParam(user.OrgId)
                .Then(!string.IsNullOrEmpty(query.PlaneTypeId), x =>
                {
                    x.Append(" and p.PlanTypeId=").AppendParam(query.PlaneTypeId);
                })
                .Then(query.CreaetStart != null, sql => {
                    sql.Append(" and p.StartOn >= ").AppendParam(query.CreaetStart);
                })
                .Then(query.CreaetEnd != null, sql => {
                    sql.Append(" and p.StartOn <= ").AppendParam(query.CreaetEnd);
                })
                .Append(" group by p.PlanTypeId")
                .DoAsync<DoQuerySql<Out_PlaneStatisItem>>()).ToList();
        }
        public virtual async Task<List<Out_DevTaskCount>> QueryDevTaskCount(string devId, IUserInfo user)
        {
            var dqs = await new SqlBuilder(help).Append("select Count(1) as Total,f.TemplateId from mz_plane_task t left join mz_flow f on t.FlowId=f.Id where TargetId=").AppendParam(devId).Append(" and f.Status=0 and EXISTS(select 1 from mz_flow_extension_attr a left join mz_flow_node n on a.ExecutionNodeId=n.Id where a.AttributeKey=").AppendParam("User/" + user.UserId + "/" + user.OrgId).Append(" and a.FlowId=t.FlowId and n.Status=5) group by f.TemplateId").DoAsync<DoQuerySql<Out_DevTaskCount>>();
            return dqs.ToList();
        }
        public virtual async Task<List<long>> QueryLeadersByDevice(long orgId, string deviceId)
        {
            var dqs = await new SqlBuilder(help).Append("select LeaderId from mz_room_device_v where OrgId=").AppendParam(orgId).Append(" and TargetId=").AppendParam(deviceId).DoAsync<DoQuerySql<long>>();
            return dqs.ToList().Distinct().ToList();
        }
        public virtual async Task<PageObject<Out_PlaneDay>> QueryDayPage(In_DevPlaneTask query, IUserInfo user)
        {
            List<string> keys = new List<string>();
            keys.Add(user.OrgId.ToString());
            var tsql = new SqlBuilder(help).Query<Out_PlaneDay>().Append("SELECT  t.TargetId,d.`Name` as DeviceName, GROUP_CONCAT(t.Id SEPARATOR ',') as TaskIds from mz_plane_task t left join mz_iot_device d on t.TargetId=d.Id where (d.OrgId=" + user.OrgId + " or ").FullSearch("d.OwnerOrgPath", keys).Append(")")
            .Then(query.UserId != null, sql =>
            {
                sql.Append(" AND t.UserId=").AppendParam(query.UserId);
            }).Append(" group by t.TargetId");
            return await tsql.GeneratePageObjectAsync(query, "");
        }
        public virtual async Task<PageObject<MZ_PlaneTask>> QueryPlanePage(In_DevPlaneTask query, Data_ServerTokenInfo user)
        {
            Expression<Func<MZ_PlaneTask, bool>> expression;
            if (string.IsNullOrEmpty(query.DeviceId))
            {
                List<string> keys = new List<string>();
                keys.Add(user.OrgId.ToString());
                expression = x => x.OrgId == user.OrgId || x.TargetDevice.OrgId == user.OrgId || SonSqlFun.FullSearch("b.OwnerOrgPath", keys);
            }
            else
            {
                expression = x => x.TargetId == query.DeviceId;
            }
            var scope = await user.GetScope(this.Provider, "/AfterService/DevPlaneTask/List");
            if (scope != null)
            {
                string scopestr = scope.GenerateFilter("DeptId", "UserId", string.Empty, false, false, false);
                if (!string.IsNullOrEmpty(scopestr))
                {
                    expression = expression.And(x => SonSqlFun.SqlCondition(scopestr));
                }
            }
            if (query.UserId != null)
            {
                expression = expression.And(x => x.UserId == query.UserId);
            }
            if (!string.IsNullOrEmpty(query.PlanTypeId))
            {
                expression = expression.And(x => x.PlanTypeId == query.PlanTypeId);
            }
            if (query.TaskStatus != null)
            {
                expression = expression.And(x => x.TaskStatus == query.TaskStatus);
            }
            if (query.FlowTemplateId != null)
            {
                expression = expression.And(x => x.FlowInfo.TemplateId == query.FlowTemplateId);
            }
            if (query.beginTime != null)
            {
                expression = expression.And(x => x.CreatedOn >= query.beginTime);
            }
            if (query.endTime != null)
            {
                expression = expression.And(x => x.CreatedOn <= query.endTime);
            }
            return await new SqlBuilder(help).Query<MZ_PlaneTask>().Include(x => x.TargetDevice, x => x.TargetId).Include(x => x.FlowInfo, x => x.FlowId).Where(expression).GeneratePageObjectAsync(query, "a.CreatedOn desc");
        }
    }
}
