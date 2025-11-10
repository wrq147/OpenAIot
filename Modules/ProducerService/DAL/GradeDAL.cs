using ProducerService.Model;
using Common;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerService.DAL
{
    public class GradeDAL : BaseRepository<MZ_Grade>
    {
        public virtual async Task<int> UpdateSort(List<string> idList)
        {
            string casestr = " case";
            for (int i = 0; i < idList.Count; i++)
            {
                casestr += " when Id =" + help.AddParam(idList[i]) + " then " + (i+1);
            }
            casestr += " end";
            return (await new SqlBuilder(help).Append("UPDATE mz_grade SET Sort = " + casestr + " where Id in (").AppendParam(idList).Append(")").DoAsync<DoExecSql>()).RowCount;
        }

    }
}
