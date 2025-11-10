using Common;
using MyAccess.DB;
using NPOI.HSSF.Record;
using ReportService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ReportService.DAL
{
    public class ReportGroupDAL : BaseRepository<MZ_ReportGroup>
    {
        public virtual async Task<int> UpdateChildrenPath(List<MZ_ReportGroup> list)
        {
            SqlBuilder sql = new SqlBuilder(help);
            string whenlist = string.Empty;
            string whenin = string.Empty;
            foreach (MZ_ReportGroup cls in list)
            {
                string tmpid = help.AddParam(cls.Id);
                whenlist = whenlist + " when " + tmpid + " then '" + cls.Path + "'";
                whenin += "," + tmpid;
            }
            whenin = whenin.Substring(1);
            sql.Append("update mz_report_group set Path=case Id ").Append(whenlist).Append(" end").Append(" where Id in (" + whenin + ")");
            return (await sql.DoAsync<DoExecSql>()).RowCount;
        }
    }
}
