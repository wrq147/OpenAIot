using Common;
using FlowService.FlowNode;
using FlowService.Model;
using MyAccess.Aop;
using MyAccess.DB;
using System;
using System.Threading.Tasks;

namespace FlowService.DAL
{
    public class FlowTemplateLinkDAL : BaseDbSupport
    {
        /// <summary>
        /// 更新流程模板的提交
        /// </summary>
        /// <param name="id"></param>
        /// <param name="depts"></param>
        [Trans]
        public virtual async Task SetFlowTemplateLinks(long id, ObjData[] targets)
        {
            var sql = new SqlBuilder(help).Delete<MZ_FlowTemplateLink>("TemplateId = ").AppendParam(id);
            await sql.DoAsync<DoExecSql>();
            if (targets.Length > 0)
            {
                MZ_FlowTemplateLink[] flowlinks = new MZ_FlowTemplateLink[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    flowlinks[i] = new MZ_FlowTemplateLink();
                    flowlinks[i].LinkId = targets[i].id;
                    flowlinks[i].LinkType = targets[i].type == "user" ? "U" : "D";
                    flowlinks[i].TemplateId = id;
                }
                await new SqlBuilder(help).Insert(flowlinks).DoAsync();

            }
        }

    }
}
