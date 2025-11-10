using Common.Share;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService
{
    public class LoginLogBLL
    {
        private LoginLogDAL _loginLog;
        private ITAServiceProvider _provider;
        public LoginLogBLL(ITAServiceProvider provider, LoginLogDAL loginLog)
        {
            _provider = provider;
            _loginLog = loginLog;
        }


        /// <summary>
        /// 新增系统登录日志
        /// </summary>
        /// <param name="logininfor"></param>
        public virtual async Task<int> InsertLoginLog(MZ_LoginLog logininfor)
        {
            return await _loginLog.AddLoginLog(logininfor);
        }


        /// <summary>
        /// 查询系统登录日志集合
        /// </summary>
        /// <param name="logininfor"></param>
        /// <returns></returns>
        public virtual async Task<PageObject<MZ_LoginLog>> SelectLogininforList(In_LoginLogList query)
        {
            return await _loginLog.SelectLoginLogList(query);
        }


        /// <summary>
        /// 批量删除系统登录日志
        /// </summary>
        /// <param name="infoIds"></param>
        /// <returns></returns>
        public virtual async Task<int> DeleteLoginLogByIds(long[] ids)
        {
            return await _loginLog.DeleteLoginLogByIds(ids);
        }


        /// <summary>
        /// 清空系统登录日志
        /// </summary>
        public virtual async Task<int> CleanLoginLog()
        {
            return await _loginLog.CleanLoginLog();
        }
    }
}
