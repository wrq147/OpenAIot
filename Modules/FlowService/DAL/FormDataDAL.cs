using Common;
using FlowService.FlowNode.FormFields;
using FlowService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlowService.DAL
{
    /// <summary>
    /// 表单实例操作
    /// </summary>
    public class FormDataDAL : BaseDbSupport
    {
        public virtual async Task AddFormData(List<MZ_FormDataItem> tlist)
        {
            await new SqlBuilder(help).Insert(tlist).DoAsync();
        }
        public virtual async Task UpdateFormData(List<MZ_FormDataItem> formItems)
        {
            foreach (MZ_FormDataItem item in formItems)
            {
                await new SqlBuilder(help).CreateOrUpdate(item).DoAsync<DoExecSql>();
            }
        }

        public virtual async Task<List<MZ_FormDataItem>> SelecFormData(long flowId)
        {
            return await new SqlBuilder(help).Query<MZ_FormDataItem>().Where(x => x.FlowId == flowId).ToListAsync();
        }
        public virtual async Task<List<MZ_FormDataItem>> SelecFormDataByIds(List<long> flowIds)
        {
            return await new SqlBuilder(help).Query<MZ_FormDataItem>().Where(x => flowIds.Contains(x.FlowId)).ToListAsync();
        }
    }
}
