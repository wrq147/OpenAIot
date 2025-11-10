using Common;
using Common.Share;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace FlowService.DAL
{
    public class FlowTemplateDAL : BaseDbSupport
    {
        public virtual async Task<List<MZ_FlowTemplate>> SelecFlowTemplateByIds(List<long> ids)
        {
            return (await new SqlBuilder(help).Append("select * from mz_flow_template where Id in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<MZ_FlowTemplate>>()).ToList();
        }
        public virtual async Task<MZ_FlowTemplate> SelecFlowTemplateById(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_flow_template where Id = ").AppendParam(id).DoAsync<DoQuerySql<MZ_FlowTemplate>>()).ToFirst();
        }
        public async Task<List<MZ_FlowTemplate>> Select(In_DefinitionList query)
        {
            using (DbHelp db = CreateDB())
            {
                var qcmd = await new SqlBuilder(db).Append("select distinct t.*,g.Name as GroupName from mz_flow_template_link lk left join mz_flow_template t on lk.TemplateId=t.Id left join mz_flow_group g on t.GroupId=g.Id where Status='0' and del_flag='0'")
                .Then(!string.IsNullOrEmpty(query.name), sq => sq.Append(" AND t.Name like concat('%',").AppendParam(query.name).Append(", '%')"))
                .Then(query.orgId != null, sq => sq.Append(" AND t.OrgId = ").AppendParam(query.orgId))
                .Then(query.IsEmbed != null, sq => sq.Append(" AND t.startlimit=").AppendParam(query.IsEmbed))
                .Then(query.groupId != null, sq => sq.Append(" AND t.GroupId=").AppendParam(query.groupId))
                    .Append(" order by Sort asc").DoAsync<DoQuerySql<MZ_FlowTemplate>>();
                return qcmd.ToList();
            }
        }
        public async Task<List<MZ_FlowTemplate>> SelectAll(long orgId)
        {
            using (DbHelp db = CreateDB())
            {
                var qcmd = await new SqlBuilder(db).Append("select * from mz_flow_template where OrgId=").AppendParam(orgId)
                    .Append(" order by Sort asc").DoAsync<DoQuerySql<MZ_FlowTemplate>>();
                return qcmd.ToList();
            }
        }

        /// <summary>
        /// 更新工作流模板
        /// </summary>
        /// <param name="template"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<int> UpdateFlowTemplateAsync(MZ_FlowTemplate template, IUserInfo user)
        {
            template.SetUpdateBy(user);
            return (await new SqlBuilder(help).Update(template)
                .DoAsync<DoExecSql>()).RowCount;
        }


        /// <summary>
        /// 新增工作流模板
        /// </summary>
        /// <param name="template"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<int> InsertFlowTemplateAsync(MZ_FlowTemplate template, IUserInfo user)
        {
            template.SetCreateBy(user);
            return (await new SqlBuilder(help).Insert(template)
                .DoReturnIdentityAsync()).RowCount;
        }

        /// <summary>
        /// 删除工作流模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<int> DeleteFlowTemplateAsync(long id)
        {
            try
            {
                return (await new SqlBuilder(help).Delete<MZ_FlowTemplate>("Id = ").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 获取流程记录
        /// </summary>
        /// <param name="query"></param>
        /// <param name="dict"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<Out_FlowRecord>> GetFlowRecord(In_FlowRecord query, Dictionary<string, FormField> dict)
        {

            return await new SqlBuilder(help).Query<Out_FlowRecord>().Append("select f.Id,f.FinishTime,f.Status,f.create_time,f.update_time,u.RealName,u.dept_id,d.dept_name from mz_flow f left join mz_admin_ov u on f.createId=u.Id and f.OrgId=u.OrgId left join mz_dept d on u.dept_id=d.dept_id where u.IsPrimary=1 and f.TemplateId=").AppendParam(query.TemplateId)
                 .Then(query.beginTime != null, sq => sq.Append(" and f.create_time >= ").AppendParam(query.beginTime))
                 .Then(query.endTime != null, sq => sq.Append(" and f.create_time <= ").AppendParam(query.endTime))
                 .Then(query.StartFinish != null, sq => sq.Append(" and f.FinishTime>=").AppendParam(query.StartFinish))
                 .Then(query.EndFinish != null, sq => sq.Append(" and f.FinishTime<=").AppendParam(query.EndFinish))
                 .Then(query.Status != null, sq => sq.Append(" and f.Status=").AppendParam(query.Status))
                 .Then(query.Depts != null && query.Depts.Length > 0, sq => sq.Append(" and u.dept_id in (").AppendParam(query.Depts).Append(")"))
                 .Then(query.Users != null && query.Users.Length > 0, sq => sq.Append(" and f.createId in (").AppendParam(query.Users).Append(")"))
                 .Then(query.Id != null, sq => sq.Append(" and f.Id=").AppendParam(query.Id))
                 .Then(query.Items != null, tsql =>
                 {
                     tsql.Append(" and EXISTS(select 1 from mz_flow_value v where f.Id=v.FlowId ");
                     foreach (var item in query.Items)
                     {
                         if (item.Value != null)
                         {
                             if (dict.TryGetValue(item.FieldId, out var ff))
                             {
                                 switch (ff.name)
                                 {
                                     case "NumberInput":
                                         tsql = tsql.Append(" and v.FieldId=").AppendParam(item.FieldId).Append(" and v.NumberValue=").AppendParam(long.Parse(item.Value));
                                         break;
                                     default:
                                         tsql = tsql.Append(" and v.FieldId=").AppendParam(item.FieldId).Append(" and v.Value=").AppendParam(Newtonsoft.Json.JsonConvert.SerializeObject(item.Value));
                                         break;
                                 }
                             }
                         }
                         else if (item.ValueArray != null && item.ValueArray.Length > 0)
                         {
                             if (dict.TryGetValue(item.FieldId, out var ff))
                             {
                                 switch (ff.name)
                                 {
                                     case "DevicPicker":
                                         tsql = tsql.Append(" and v.FieldId like ").AppendParam(item.FieldId + "@%").Append(" and v.Value in (").AppendParam(item.ValueArray).Append(")");
                                         break;
                                     default:
                                         var tmpll = Array.ConvertAll<string, long>(item.ValueArray, long.Parse);
                                         tsql = tsql.Append(" and v.FieldId like ").AppendParam(item.FieldId + "@%").Append(" and v.NumberValue in (").AppendParam(tmpll).Append(")");
                                         break;
                                 }
                             }
                         }
                         else if (item.Min != null && item.Max != null)
                         {
                             tsql = tsql.Append(" and v.FieldId=").AppendParam(item.FieldId).Append(" and v.NumberValue>=").AppendParam(item.Min).Append(" and v.NumberValue<=").AppendParam(item.Max);
                         }
                         else if (item.Min != null)
                         {
                             tsql = tsql.Append(" and v.FieldId=").AppendParam(item.FieldId).Append(" and v.NumberValue>=").AppendParam(item.Min);
                         }
                         else if (item.Max != null)
                         {
                             tsql = tsql.Append(" and v.FieldId=").AppendParam(item.FieldId).Append(" and v.NumberValue<=").AppendParam(item.Max);
                         }
                     }
                     tsql.Append(")");
                 })
                .GeneratePageObjectAsync(query, "f.create_time desc");
        }

    }
}
