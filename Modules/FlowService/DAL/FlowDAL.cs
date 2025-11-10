using AuthService;
using Common;
using Common.Share;
using FlowService.Model;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FlowService.DAL
{
    public class FlowDAL : BaseDbSupport
    {
        /// <summary>
        /// 添加工作流实例
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        public virtual async Task<long> InsertFlowAsync(MZ_Flow flow)
        {
            //新增节点
            foreach (MZ_Flow_Node kv in flow.ExecutionNodes)
            {
                await new SqlBuilder(help).Insert(kv).DoAsync();
                //新增节点扩展
                foreach (var attr in kv.ExtensionAttributes)
                {
                    await new SqlBuilder(help).Insert(attr).DoAsync();
                }
            }

            return await new SqlBuilder(help).Insert(flow)
                .DoAsync();
        }
        public virtual async Task<int> UpdateFlowAsync(MZ_Flow flow)
        {
            if (flow.ExecutionNodes != null)
            {
                //更新节点
                foreach (MZ_Flow_Node kv in flow.ExecutionNodes)
                {
                    await new SqlBuilder(help).Append("INSERT INTO mz_flow_node(Id,FlowId,StepId,StepName,ParentId,Active,StartTime,EndTime,EventKey,EventPublished,Outcome,Status) VALUE(")
                        .AppendParam(kv.Id).Append(",").AppendParam(kv.FlowId).Append(",").AppendParam(kv.StepId).Append(",").AppendParam(kv.StepName).Append(",").AppendParam(kv.ParentId).Append(",").AppendParam(kv.Active)
                        .Append(",").AppendParam(kv.StartTime).Append(",").AppendParam(kv.EndTime).Append(",").AppendParam(kv.EventKey).Append(",").AppendParam(kv.EventPublished).Append(",").AppendParam(kv.Outcome)
                        .Append(",").AppendParam(kv.Status).Append(") ON DUPLICATE KEY UPDATE Active=").AppendParam(kv.Active).Append(",EndTime=").AppendParam(kv.EndTime).Append(",EventKey=").AppendParam(kv.EventKey).Append(",EventPublished=").AppendParam(kv.EventPublished)
                        .Append(",Outcome=").AppendParam(kv.Outcome).Append(",Status=").AppendParam(kv.Status).DoAsync<DoExecSql>();

                    //更新节点扩展
                    foreach (var attr in kv.ExtensionAttributes)
                    {
                        if (attr.Id == null)
                        {
                            await new SqlBuilder(help).Insert(attr).DoAsync();
                        }
                        else
                        {
                            await new SqlBuilder(help).Update(attr).DoAsync();
                        }
                    }
                }
            }

            return await new SqlBuilder(help).Update(flow).DoAsync();
        }

        /// <summary>
        /// 获取指定工作流实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<MZ_Flow> Select(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_flow where Id = ").AppendParam(id).DoAsync<DoQuerySql<MZ_Flow>>()).ToFirst();
        }
        public virtual async Task<MZ_FlowTemplate> SelectTemplateByFlowId(long id)
        {
            return (await new SqlBuilder(help).Append("select t.* from mz_flow f left join mz_flow_template t on f.TemplateId=t.Id where f.Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_FlowTemplate>>()).ToFirst();
        }
        public virtual async Task<List<Out_FlowTemplate>> SelectTemplateByFlowIdList(List<long> ids)
        {
            return (await new SqlBuilder(help).Append("select f.Id as FlowId,t.* from mz_flow f left join mz_flow_template t on f.TemplateId=t.Id where f.Id in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<Out_FlowTemplate>>()).ToList();
        }
        public virtual async Task<int> SelectCount(long templateId, long uid)
        {
            return await new SqlBuilder(help).Query<MZ_Flow>().CountAsync(x => x.TemplateId == templateId && x.createId == uid);
        }
        /// <summary>
        /// 标记删除指定流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteFlowAsync(long id)
        {
            return (await new SqlBuilder(help).Append("update mz_flow set del_flag = '2' where Id = ").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
        }

        /// <summary>
        /// 根据流程Id获取待处理任务
        /// </summary>
        /// <param name="flowIds"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<List<Out_FlowTodoItem>> GetTodoTaskListByIds(List<long> flowIds, IUserInfo user)
        {
            if (flowIds.Count <= 0)
            {
                return new List<Out_FlowTodoItem>();
            }
            var tsql = new SqlBuilder(help).Query<Out_FlowTodoItem>().Append("select a.*,n.StepName,n.StartTime,f.TemplateId,f.FlowName,f.createId from mz_flow_extension_attr a left join mz_flow_node n on a.ExecutionNodeId=n.Id left join mz_flow f on a.FlowId=f.Id where a.AttributeKey=").AppendParam("User/" + user.UserId + "/" + user.OrgId).Append(" and a.FlowId in (").AppendParam(flowIds).Append(") and n.Status=5 and f.Status=0");
            return await tsql.ToListAsync();
        }
        /// <summary>
        /// 获取待处理任务
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<Out_FlowTodoItem>> GetTodoTaskList(In_TodoList query, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<Out_FlowTodoItem>().Append("select a.*,n.StepName,n.StartTime,f.TemplateId,f.FlowName,f.createId from mz_flow_extension_attr a left join mz_flow_node n on a.ExecutionNodeId=n.Id left join mz_flow f on a.FlowId=f.Id where a.AttributeKey=").AppendParam("User/" + user.UserId + "/" + user.OrgId).Append(" and n.Status=5 and f.Status=0")
                 .Then(query.beginTime != null, sq => sq.Append(" and n.StartTime >= ").AppendParam(query.beginTime))
                 .Then(query.endTime != null, sq => sq.Append(" and n.StartTime <= ").AppendParam(query.endTime))
                 .Then(!string.IsNullOrEmpty(query.key), sq => sq.Append(" and f.FlowName like ").AppendParam("%" + query.key.SqlLikeFilter() + "%"))
                 .GeneratePageObjectAsync(query, "n.StartTime desc");
        }
        /// <summary>
        /// 获取我发起的流程
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_Flow>> GetMyProcessList(In_MyProcess query, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<MZ_Flow>().Append("select f.Id,f.OrgId,f.TemplateId,f.Description,f.FlowName,f.GroupId,f.Status,f.create_time,f.update_time,f.FinishTime,g.Name as GroupName from mz_flow f left join mz_flow_group g on f.GroupId=g.Id where f.del_flag='0' and f.IsEmbed=0 and f.createId=").AppendParam(user.UserId).Append(" and f.OrgId=").AppendParam(user.OrgId)
     .Then(query.name != null, sq => sq.Append(" AND f.FlowName like concat('%', ").AppendParam(query.name).Append(", '%')"))
     .Then(query.status != null, sq => sq.Append(" AND f.Status = ").AppendParam(query.status))
     .Then(query.templateId != null, sq => sq.Append(" AND f.TemplateId = ").AppendParam(query.templateId))
                 .Then(query.beginTime != null, sq => sq.Append(" and f.create_time >= ").AppendParam(query.beginTime))
                  .Then(query.endTime != null, sq => sq.Append(" and f.create_time <= ").AppendParam(query.endTime))
     .GeneratePageObjectAsync(query, "f.update_time desc");
        }

        /// <summary>
        /// 查询已办任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<Out_FinishedItem>> GetFinishedList(In_FinishedList query, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<Out_FinishedItem>().Append("select a.*,n.StepName,n.StartTime,n.EndTime,f.TemplateId,f.FlowName,f.createId from mz_flow_extension_attr a left join mz_flow_node n on a.ExecutionNodeId=n.Id left join mz_flow f on n.FlowId=f.Id where a.AttributeKey=").AppendParam("User/" + user.UserId + "/" + user.OrgId).Append(" and n.Status=3 and f.Status=2")
                 .Then(query.beginTime != null, sq => sq.Append(" and n.StartTime >= ").AppendParam(query.beginTime))
                 .Then(query.endTime != null, sq => sq.Append(" and n.StartTime <= ").AppendParam(query.endTime))
                 .Then(!string.IsNullOrEmpty(query.key), sq => sq.Append(" and f.FlowName like ").AppendParam("%" + query.key.SqlLikeFilter() + "%"))
        .GeneratePageObjectAsync(query, "n.StartTime desc");
        }

        /// <summary>
        /// 查询抄送列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<Out_CSItem>> GetCSList(In_CSList query, IUserInfo user)
        {
            return await new SqlBuilder(help).Query<Out_CSItem>().Append("select a.*,n.StepName,n.StartTime,f.TemplateId,f.FlowName,f.createId from mz_flow_extension_attr a left join mz_flow_node n on a.ExecutionNodeId=n.Id left join mz_flow f on n.FlowId=f.Id where a.AttributeKey=").AppendParam("CC/" + user.UserId + "/" + user.OrgId)
                 .Then(query.beginTime != null, sq => sq.Append(" and n.StartTime >= ").AppendParam(query.beginTime))
                 .Then(query.endTime != null, sq => sq.Append(" and n.StartTime <= ").AppendParam(query.endTime))
                 .Then(!string.IsNullOrEmpty(query.key), sq => sq.Append(" and f.FlowName like ").AppendParam("%" + query.key.SqlLikeFilter() + "%"))
        .GeneratePageObjectAsync(query);
        }


        public virtual async Task<Out_FlowStatistics> GetMyStatistics(IUserInfo user)
        {
            return (await new SqlBuilder(help).Append("SELECT  SUM(CASE WHEN Status = 1 THEN 1 ELSE 0 END) AS MyWaitCount,SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS MyDoingCount,SUM(CASE WHEN Status = 2 THEN 1 ELSE 0 END) AS MyFinishCount,SUM(CASE WHEN Status=3 THEN 1 ELSE 0 END) AS MyCancelCount FROM mz_flow where del_flag='0' and OrgId=").AppendParam(user.OrgId).Append(" and createId=").AppendParam(user.UserId).DoAsync<DoQuerySql<Out_FlowStatistics>>()).ToFirst();
        }
        public virtual async Task<int> GetPendingCount(IUserInfo user)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_flow_extension_attr a left join mz_flow_node n on a.ExecutionNodeId=n.Id left join mz_flow f on n.FlowId=f.Id where a.AttributeKey='User/" + user.UserId + "/" + user.OrgId + "' and n.Status=5 and f.Status=0").DoAsync<DoQueryScalar>()).GetValueInt();
        }
        public virtual async Task<int> GetCopyCount(IUserInfo user)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_flow_extension_attr where AttributeKey='CC/" + user.UserId + "/" + user.OrgId + "'").DoAsync<DoQueryScalar>()).GetValueInt();
        }
        public virtual async Task<int> GetFinishCount(IUserInfo user)
        {
            return (await new SqlBuilder(help).Append("select count(1) from mz_flow_extension_attr a left join mz_flow_node n on a.ExecutionNodeId=n.Id left join mz_flow f on n.FlowId=f.Id where a.AttributeKey='User/" + user.UserId + "/" + user.OrgId + "' and n.Status=3 and f.Status=2").DoAsync<DoQueryScalar>()).GetValueInt();
        }
    }
}
