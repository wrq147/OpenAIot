using MyAccess.DB;
using System;
using System.Collections.Generic;
using Common.Share;
using System.Threading.Tasks;
using Common;

namespace AuthService
{
    public class LoginLogDAL : BaseDbSupport
    {
        public virtual async Task<int> AddLoginLog(MZ_LoginLog log)
        {
            return (await new SqlBuilder(help).Insert(log).DoAsync<DoExecSql>()).RowCount;
        }
        public virtual string LimitLoginTime(long uid)
        {
            DateTime limitTime = DateTime.Now;
            help.AddParam("@UserId", uid);
            help.AddParam("@ErrLogType", 1);
            help.AddParam("@SuccessLogType", 0);
            help.AddParam("@CreateDate", DateTime.Now.AddHours(-24));

            var sql = new SqlBuilder(help).Query<MZ_LoginLog>().Append("select * from mz_login_log where UserId=@UserId and (Status=@ErrLogType or Status=@SuccessLogType) and CreateDate>@CreateDate order by CreateDate desc").Take(5);

            DoQuerySql<MZ_LoginLog> execsql = sql.Do<DoQuerySql<MZ_LoginLog>>();
            List<MZ_LoginLog> sysloglist = execsql.ToList();
            int errcount = 0;
            foreach (MZ_LoginLog lg in sysloglist)
            {
                if (lg.Status == 0)
                {
                    break;
                }
                errcount++;
            }

            if (errcount == 3)
            {
                limitTime = sysloglist[0].CreateDate.Value.AddMinutes(5);
            }
            else if (errcount == 4)
            {
                limitTime = sysloglist[0].CreateDate.Value.AddHours(1);
            }
            else if (errcount >= 5)
            {
                limitTime = sysloglist[0].CreateDate.Value.AddHours(24);
            }
            else
            {
                limitTime = DateTime.Now.AddMonths(-1);
            }

            TimeSpan ts = limitTime - DateTime.Now;
            if (ts.TotalSeconds > 0)
            {
                if (ts.TotalHours >= 1)
                {
                    return "帐户已经冻结。请" + (int)ts.TotalHours + "小时后再尝试登录";
                }
                else if (ts.TotalMinutes >= 1 && ts.TotalMinutes < 60)
                {
                    return "帐户已经冻结。请" + (int)ts.TotalMinutes + "分钟后再尝试登录";
                }
                else
                {
                    return "帐户已经冻结。请" + (int)ts.TotalSeconds + "秒后再尝试登录";
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// 查询系统登录日志集合
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_LoginLog>> SelectLoginLogList(In_LoginLogList query)
        {
            return await new SqlBuilder(help).Query<MZ_LoginLog>().Append("select l.*,u.UserName from mz_login_log l left join mz_admin u on l.UserId=u.Id where 1=1")
                  .Then(!string.IsNullOrEmpty(query.ipaddr), sql =>
                  {
                      sql.Append(" AND l.IPAddress like concat('%', ").AppendParam(query.ipaddr).Append(", '%')");
                  })
                  .Then(query.status != null, sql =>
                  {
                      sql.Append(" AND l.status =").AppendParam(query.status);
                  })
                  .Then(!string.IsNullOrEmpty(query.userName), sql =>
                  {
                      sql.Append(" AND u.UserName like concat('%', ").AppendParam(query.userName).Append(", '%')");
                  })
                  .Then(query.beginTime != null, sql =>
                  {
                      sql.Append(" and l.CreateDate >= ").AppendParam(query.beginTime);
                  })
                  .Then(query.endTime != null, sql =>
                  {
                      sql.Append(" and l.CreateDate <= ").AppendParam(query.endTime);
                  })
                  .GeneratePageObjectAsync(query, "l.CreateDate desc");
        }

        /// <summary>
        /// 批量删除系统登录日志
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteLoginLogByIds(long[] ids)
        {
            return (await new SqlBuilder(help).Delete<MZ_LoginLog>("SysLogID in (").AppendParam(ids).Append(")").DoAsync<DoExecSql>()).RowCount;
        }

        /// <summary>
        /// 清空系统登录日志
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> CleanLoginLog()
        {
            return (await new SqlBuilder(help).Append("truncate table mz_login_log").DoAsync<DoExecSql>()).RowCount;
        }
        public virtual async Task<MZ_LoginLog> SelectNearest(long uid, string terminal)
        {
            var sql = new SqlBuilder(help).Query<MZ_LoginLog>().Append("select * from mz_login_log where UserId=").AppendParam(uid).Append(" and Terminal=").AppendParam(terminal).Append(" order by CreateDate desc");
            return await sql.ToFirstAsync();
        }
    }
}
