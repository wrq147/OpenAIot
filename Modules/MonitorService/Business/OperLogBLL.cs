using AuthService;
using Common.Share;
using MonitorService.Controller;
using MonitorService.DAL;
using MonitorService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MonitorService.Business
{
    public class OperLogBLL
    {
        private OperLogDAL _operLog;
        private ITAServiceProvider _provider;
        public OperLogBLL(OperLogDAL operlog,ITAServiceProvider provider)
        {
            _operLog = operlog;
            _provider = provider;
        }



        /// <summary>
        /// 新增操作日志
        /// </summary>
        /// <param name="operLog"></param>
        /// <returns></returns>
        public long InsertOperlog(MZ_OperLog operLog)
        {
            return _operLog.InsertOperlog(operLog);
        }


        /// <summary>
        /// 查询系统操作日志集合
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_OperLog>> SelectOperLogList(In_OperLogList query)
        {
            return await _operLog.SelectOperLogList(query, null);
        }
        /// <summary>
        /// 查询本人的操作日志
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<PageObject<MZ_OperLog>> SelectOperLogListByPerson(In_OperLogList query)
        {
            return await _operLog.SelectOperLogList(query, _provider.GetUser());
        }
        /// <summary>
        /// 批量删除系统操作日志
        /// </summary>
        /// <param name="operIds"></param>
        /// <returns></returns>
        public async Task<int> DeleteOperLogByIds(long[] operIds)
        {
            return await _operLog.DeleteOperLogByIds(operIds);
        }


        /// <summary>
        /// 查询操作日志详细
        /// </summary>
        /// <param name="operId"></param>
        /// <returns></returns>
        public async Task<MZ_OperLog> SelectOperLogById(long operId)
        {
            return await _operLog.SelectOperLogById(operId);
        }


        /// <summary>
        /// 清空操作日志
        /// </summary>
        public async Task<int> CleanOperLog()
        {
            return await _operLog.CleanOperLog();
        }
        /// <summary>
        /// 清空超一个月的日志
        /// </summary>
        /// <returns></returns>
        public async Task<int> CleanOver()
        {
            await _provider.GetService<JobLogDAL>().CleanOver(DateTime.Now.AddDays(-30));
            return await _operLog.CleanOver(DateTime.Now.AddDays(-30));
        }
    }
}
