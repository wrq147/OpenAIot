using Common;
using FlowService.Model;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MyAccess.DB.Builder.WhereToSql;

namespace FlowService.DAL
{
    public class FlowQueryDAL : BaseRepository<MZ_FlowQuery>
    {
        public virtual async Task<Dictionary<string, string>> SelectFlowQuerys(long id)
        {
            var rt = await SelectList(x => x.FlowId == id);
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in rt)
            {
                dict.Add(item.Name, item.Value);
            }
            return dict;
        }
        public virtual async Task<List<MZ_FlowQuery>> SelectQueryList(In_FlowQuery query)
        {
            Expression<Func<MZ_FlowQuery, bool>> expression = x => true;
            if (query.FlowId != null)
            {
                expression = expression.And(x => x.FlowId == query.FlowId);
            }
            else
            {
                expression = expression.And(x => x.Name == query.Name);
                expression = expression.And(x => x.Value == query.Value);
            }

            if (query.TemplateId != null)
            {
                expression = expression.And(x => x.TemplateId == query.TemplateId);
            }

            return await new SqlBuilder(help).Query<MZ_FlowQuery>().Where(expression).ToListAsync();
        }
    }
}
