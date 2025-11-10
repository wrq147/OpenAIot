using Common;
using FlowService.Model;
using MyAccess.Aop;
using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowService.DAL
{
    public class FormDAL : BaseDbSupport
    {
        /// <summary>
        /// 获取表单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<MZ_Form> SelecFormById(long id)
        {
            return (await new SqlBuilder(help).Append("select * from mz_form where Id = ").AppendParam(id).DoAsync<DoQuerySql<MZ_Form>>()).ToFirst();
        }
        public virtual async Task<MZ_Form> SelectFormByTemplateId(long id)
        {
            return (await new SqlBuilder(help).Append("select f.* from mz_flow_template t left join mz_form f on t.FormId=f.Id where t.Id = ").AppendParam(id).DoAsync<DoQuerySql<MZ_Form>>()).ToFirst();
        }

        public virtual async Task<List<Out_FlowForm>> SelectFormByFlowId(List<long> ids)
        {
            return (await new SqlBuilder(help).Append("select f.*,l.Id as FlowId from mz_flow  l left join mz_flow_template t on l.TemplateId=t.Id left join mz_form f on t.FormId=f.Id  where l.Id in (").AppendParam(ids).Append(")")
                .DoAsync<DoQuerySql<Out_FlowForm>>()).ToList();
        }
        /// <summary>
        /// 添加表单
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<int> InsertFormAsync(MZ_Form form)
        {
            return (await new SqlBuilder(help).Insert(form)
       .DoReturnIdentityAsync()).RowCount;
        }
        /// <summary>
        /// 更新表单
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        [Trans]
        public virtual async Task<int> UpdateFormAsync(MZ_Form form)
        {
            return (await new SqlBuilder(help).Update(form)
                .DoAsync<DoExecSql>()).RowCount;
        }
        [Trans]
        public virtual async Task<int> DeleteFormAsync(long id)
        {
            try
            {
                return (await new SqlBuilder(help).Delete<MZ_Form>("Id = ").AppendParam(id).DoAsync<DoExecSql>()).RowCount;
            }
            catch
            {
                return -1;
            }
        }
    }
}
