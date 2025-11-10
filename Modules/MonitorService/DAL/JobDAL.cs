using MyAccess.DB;
using System.Collections.Generic;
using Common.Share;
using MonitorService.Model;
using System.Threading.Tasks;
using Common;

namespace MonitorService.DAL
{
    public class JobDAL : BaseDbSupport
    {
        public async Task<List<string>> SelectErrorJobs()
        {
            using (DbHelp db = CreateDB())
            {
                var tsql = await new SqlBuilder(db).Append("select JOB_NAME from QRTZ_TRIGGERS where TRIGGER_STATE='ERROR'").DoAsync<DoQuerySql<string>>();
                return tsql.ToList();
            }
        }
        public async Task<PageObject<MZ_Job>> SelectJobList(In_JobList query)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_Job>().Append("select * from mz_job where 1=1")
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

        public async Task<MZ_Job> SelectJobById(long id)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_job where job_id=").AppendParam(id).DoAsync<DoQuerySql<MZ_Job>>();
                return docmd.ToFirst();
            }
        }
        public async Task<bool> ExistJob(string name, string group)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select count(1) from mz_job where job_name=").AppendParam(name).Append(" and job_group=").AppendParam(group).Append(" limit 1")
                    .DoAsync<DoQueryScalar>();
                return docmd.GetValueInt(0) > 0;
            }
        }
        public async Task<MZ_Job> SelectJobByName(string name, string group)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_job where job_name=").AppendParam(name).Append(" and job_group=").AppendParam(group).Append(" limit 1")
                    .DoAsync<DoQuerySql<MZ_Job>>();
                return docmd.ToFirst();
            }
        }

        /// <summary>
        /// 修改调度任务信息
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<int> UpdateJob(MZ_Job job)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Update(job).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
        /// <summary>
        /// 新增调度任务信息
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public async Task<long> InsertJob(MZ_Job job)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Insert(job).DoReturnIdentityAsync();
                return docmd.LastInsertedId;
            }
        }

        /// <summary>
        /// 通过调度ID删除调度任务信息
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<int> DeleteJobById(long jobId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Delete<MZ_Job>("job_id=").AppendParam(jobId).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

    }
}
