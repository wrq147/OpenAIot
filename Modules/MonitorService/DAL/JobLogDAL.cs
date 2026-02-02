using Common;
using Common.Share;
using MonitorService.Model;
using MyAccess.DB;
using NPOI.SS.Formula.Functions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MonitorService.DAL
{
    public class JobLogDAL : BaseDbSupport
    {
        /// <summary>
        /// 获取quartz调度器日志的计划任务
        /// </summary>
        /// <param name="jobLog"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_JobLog>> SelectJobLogList(In_JobLogList query)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_JobLog>().Append("select * from mz_job_log where 1=1")
                    .Then(!string.IsNullOrEmpty(query.jobName), sql =>
                    {
                        sql.Append(" AND job_name like concat('%', ").AppendParam(query.jobName).Append(", '%')");
                    })
                    .Then(!string.IsNullOrEmpty(query.invoke_target), sql =>
                    {
                        sql.Append(" AND invoke_target like concat('%', ").AppendParam(query.invoke_target).Append(", '%')");
                    })
                    .Then(!string.IsNullOrEmpty(query.status), sql =>
                    {
                        sql.Append(" AND status =").AppendParam(query.status);
                    })
                    .Then(!string.IsNullOrEmpty(query.jobGroup), sql =>
                    {
                        sql.Append(" AND job_group =").AppendParam(query.jobGroup);
                    })
                    .Then(query.beginTime != null, sql =>
                    {
                        sql.Append(" and create_time >= ").AppendParam(query.beginTime);
                    })
                    .Then(query.endTime != null, sql =>
                    {
                        sql.Append(" and create_time <= ").AppendParam(query.endTime);
                    })
                    .GeneratePageObjectAsync(query, "create_time desc");
            }
        }


        /// <summary>
        /// 通过调度任务日志ID查询调度信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MZ_JobLog> SelectJobLogById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_job_log where job_log_id=").AppendParam(id).DoAsync<DoQuerySql<MZ_JobLog>>();
                return docmd.ToFirst();
            }
        }


        /// <summary>
        /// 新增任务日志
        /// </summary>
        /// <param name="jobLog"></param>
        /// <returns></returns>
        public async Task<long> InsertJobLog(MZ_JobLog jobLog)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(jobLog).DoReturnIdentityAsync();
                return docmd.LastInsertedId;
            }
        }



        /// <summary>
        /// 批量删除调度日志信息
        /// </summary>
        /// <param name="logIds"></param>
        /// <returns></returns>
        public async Task<int> DeleteJobLogByIds(long[] logIds)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Delete<MZ_JobLog>("job_log_id in (").AppendParam(logIds).Append(")").DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }


        /// <summary>
        /// 删除指定任务组和名称的日志
        /// </summary>
        /// <param name="name"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<int> DeleteJobLogByName(string name, string group)
        {
            using (DbHelp db = CreateDB())
            {
                if (string.IsNullOrEmpty(name))
                {
                    var docmd = await new SqlBuilder(db).Delete<MZ_JobLog>("job_group=").AppendParam(group).DoAsync<DoExecSql>();
                    return docmd.RowCount;
                }
                else if (string.IsNullOrEmpty(group))
                {
                    var docmd = await new SqlBuilder(db).Delete<MZ_JobLog>("job_name=").AppendParam(name).DoAsync<DoExecSql>();
                    return docmd.RowCount;
                }
                else
                {
                    var docmd = await new SqlBuilder(db).Delete<MZ_JobLog>("job_name=").AppendParam(name).Append(" and job_group=").AppendParam(group).DoAsync<DoExecSql>();
                    return docmd.RowCount;
                }
         
            }
        }
        public async Task<int> ClearFinsihJobLog(string jobName)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Delete<MZ_JobLog>("job_name=").AppendParam(jobName).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 清空任务日志
        /// </summary>
        public async Task<int> CleanJobLog()
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("truncate table mz_job_log").DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> CleanOver(DateTime overTime)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Delete<MZ_JobLog>("create_time<").AppendParam(overTime).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        public async Task<int> UpdateLog(MZ_JobLog entity)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Update(entity).DoAsync();
            }
        }
    }
}
