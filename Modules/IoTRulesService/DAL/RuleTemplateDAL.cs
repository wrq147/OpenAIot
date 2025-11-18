using Common;
using Common.Share;
using IoTRulesService.Model;
using IoTService.Models;
using MyAccess.Core;
using MyAccess.DB;
using MyAccess.DB.Builder.WhereToSql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IoTRulesService.DAL
{
    public class RuleTemplateDAL : BaseRepository<MZ_RuleTemplate>
    {
        /// <summary>
        /// 指定分组路径下面是否有设备
        /// </summary>
        /// <param name="groupPath"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistRule(string groupPath)
        {
            var rs = await new SqlBuilder(help).Query<MZ_RuleTemplate>().Append("select d.Id from mz_rule_template d left join mz_rule_group g on d.GroupId = g.Id where g.Path like ")
       .AppendParam(groupPath + '%').Append(" limit 1").ToFirstAsync();
            return rs != null ? true : false;
        }
        public virtual async Task<PageObject<MZ_RuleTemplate>> SelectByPage(In_ListPage query, string groupPath, long orgId)
        {
            Expression<Func<MZ_RuleTemplate, bool>> expression = x => x.OrgId == orgId;
            if (query.way != null)
            {
                expression = expression.And(x => x.TriggerWay == query.way);
            }
            if (!string.IsNullOrEmpty(groupPath))
            {
                string isexistGroup = "EXISTS(select Id from mz_rule_group where mz_rule_template.GroupId=Id and Path like '" + groupPath + "%')";
                expression = expression.And(x => SonSqlFun.SqlCondition(isexistGroup));
            }
            if (!string.IsNullOrEmpty(query.key))
            {
                expression=expression.And(x=> x.Name.Contains(query.key));
            }
            if (query.status != null)
            {
                expression = expression.And(x => x.Status == query.status);
            }
            if (!string.IsNullOrEmpty(query.from))
            {
                expression = expression.And(x => x.CreatedFrom == query.from);
            }
            return await SelectPage(expression, query, "update_time desc");
        }
        public virtual async Task<List<MZ_RuleTemplate>> SelectProPathByTriggers(string productPath, string msgType)
        {
            return await new SqlBuilder(help).Query<MZ_RuleTemplate>().Append("select r.* from mz_rule_trigger t inner join mz_rule_template r on t.RuleId=r.Id where r.Status='0' and t.TopicDevice=").AppendParam(productPath)
                .Append(" and t.TopicMsg=").AppendParam(msgType).ToListAsync();
        }
        public virtual async Task<List<MZ_RuleTemplate>> SelectDevPathByTriggers(string devicePath, string msgType)
        {
            return await new SqlBuilder(help).Query<MZ_RuleTemplate>().Append("select r.* from mz_rule_trigger t inner join mz_rule_template r on t.RuleId=r.Id where r.Status='0' and t.TopicDevice=").AppendParam(devicePath)
                .Append(" and t.TopicMsg=").AppendParam(msgType).ToListAsync();
        }
        public virtual async Task<List<MZ_RuleTemplate>> SelectByTriggers(string productPath, string devicePath, string msgType)
        {
            return await new SqlBuilder(help).Query<MZ_RuleTemplate>().Append("select r.* from mz_rule_trigger t inner join mz_rule_template r on t.RuleId=r.Id where r.Status='0' and (t.TopicDevice=").AppendParam(productPath)
                .Append(" or t.TopicDevice=").AppendParam(devicePath).Append(") and t.TopicMsg=").AppendParam(msgType).ToListAsync();
        }
    }
}
