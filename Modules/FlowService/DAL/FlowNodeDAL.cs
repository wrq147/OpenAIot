using FlowService.Model;
using MyAccess.DB;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;

namespace FlowService.DAL
{
    public class FlowNodeDAL : BaseDbSupport
    {
        public virtual async Task<MZ_Flow_Node> SelecRootNode(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_flow_node where FlowId=").AppendParam(id).Append(" and StepId='root'").DoAsync<DoQuerySql<MZ_Flow_Node>>()).ToFirst();
        }
        public virtual async Task<List<MZ_Flow_Node>> SelectByFlowId(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_flow_node where FlowId=").AppendParam(id).DoAsync<DoQuerySql<MZ_Flow_Node>>()).ToList();
        }
        public virtual async Task<List<MZ_Flow_Node>> SelectByFlowId(List<long> ids)
        {
            return (await new SqlBuilder(help).Append("select * from mz_flow_node where FlowId in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<MZ_Flow_Node>>()).ToList();
        }
        public virtual async Task<List<MZ_Flow_ExtensionAttribute>> SelectAttributes(long nodeId)
        {
            return await new SqlBuilder(help).Query<MZ_Flow_ExtensionAttribute>().Where(x => x.ExecutionNodeId == nodeId).ToListAsync();
        }
        public virtual async Task InitNodeExtension(List<MZ_Flow_Node> list)
        {
            List<long> ids = list.Select(x => x.Id.Value).ToList();
            if (ids.Count == 0)
            {
                return;
            }
            var extions = (await new SqlBuilder(help).Append("select * from mz_flow_extension_attr where ExecutionNodeId in (").AppendParam(ids).Append(")").DoAsync<DoQuerySql<MZ_Flow_ExtensionAttribute>>()).ToList();
            Dictionary<long, List<MZ_Flow_ExtensionAttribute>> extdic = new Dictionary<long, List<MZ_Flow_ExtensionAttribute>>();
            foreach (var ext in extions)
            {
                List<MZ_Flow_ExtensionAttribute> tmplist;
                if (!extdic.TryGetValue(ext.ExecutionNodeId.Value, out tmplist))
                {
                    tmplist = new List<MZ_Flow_ExtensionAttribute>();
                }
                tmplist.Add(ext);
                extdic[ext.ExecutionNodeId.Value] = tmplist;
            }
            foreach (MZ_Flow_Node fn in list)
            {
                fn.ExtensionAttributes = new List<MZ_Flow_ExtensionAttribute>();
                List<MZ_Flow_ExtensionAttribute> tmplist;
                if (!extdic.TryGetValue(fn.Id.Value, out tmplist))
                {
                    tmplist = new List<MZ_Flow_ExtensionAttribute>();
                }
                fn.ExtensionAttributes = tmplist;
            }
        }

        public virtual async Task<MZ_Flow_Node> SelecFlowNodeById(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_flow_node where Id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Flow_Node>>()).ToFirst();
        }

    }
}
