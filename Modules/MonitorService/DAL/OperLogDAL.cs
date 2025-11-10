using MyAccess.DB;
using System;
using Common.Share;
using MonitorService.Model;
using System.Threading.Tasks;
using Common;


namespace MonitorService.DAL
{
    public class OperLogDAL : BaseDbSupport
    {
        /// <summary>
        /// 新增操作日志
        /// </summary>
        /// <param name="operLog"></param>
        public long InsertOperlog(MZ_OperLog operLog)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = new SqlBuilder(db).Insert(operLog).DoReturnIdentity();
                return docmd.LastInsertedId;
            }
        }


        /// <summary>
        /// 查询系统操作日志集合
        /// </summary>
        /// <param name="query"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_OperLog>> SelectOperLogList(In_OperLogList query, IUserInfo user)
        {
            using (DbHelp db = CreateDB())
            {
                return await new SqlBuilder(db).Query<MZ_OperLog>().Append("select * from mz_oper_log where 1=1")
                    .Then(!string.IsNullOrEmpty(query.title), sql =>
                    {
                        sql.Append(" AND title like concat('%', ").AppendParam(query.title).Append(", '%')");
                    })
                    .Then(query.status != null, sql =>
                    {
                        sql.Append(" AND status =").AppendParam(query.status);
                    })
                    .Then(!string.IsNullOrEmpty(query.operName), sql =>
                    {
                        sql.Append(" AND oper_name like concat('%', ").AppendParam(query.operName).Append(", '%')");
                    })
                    .Then(user != null, sql => sql.Append(" and oper_uid=").AppendParam(user.UserId).Append(" and oper_org=").AppendParam(user.OrgId))
                    .Then(query.beginTime != null, sql =>
                    {
                        sql.Append(" and oper_time >= ").AppendParam(query.beginTime);
                    })
                    .Then(query.endTime != null, sql =>
                    {
                        sql.Append(" and oper_time <= ").AppendParam(query.endTime);
                    })
                    .GeneratePageObjectAsync(query, "oper_time desc");
            }
        }


        /// <summary>
        /// 批量删除系统操作日志
        /// </summary>
        /// <param name="operIds"></param>
        /// <returns></returns>
        public async Task<int> DeleteOperLogByIds(long[] operIds)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Delete<MZ_OperLog>("oper_id in (").AppendParam(operIds).Append(")").DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }


        /// <summary>
        /// 查询操作日志详细
        /// </summary>
        /// <param name="operId"></param>
        /// <returns></returns>
        public async Task<MZ_OperLog> SelectOperLogById(long operId)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("select * from mz_oper_log where oper_id=").AppendParam(operId).DoAsync<DoQuerySql<MZ_OperLog>>();
                return docmd.ToFirst();
            }
        }


        /// <summary>
        /// 清空操作日志
        /// </summary>
        public async Task<int> CleanOperLog()
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Append("truncate table mz_oper_log").DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }

        public async Task<int> CleanOver(DateTime overTime)
        {
            using (DbHelp db = CreateDB())
            {
                var docmd = await new SqlBuilder(db).Delete<MZ_OperLog>("oper_time<").AppendParam(overTime).DoAsync<DoExecSql>();
                return docmd.RowCount;
            }
        }
    }
}
