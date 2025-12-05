using AuthService;
using Common;
using MonitorService.Business;
using MonitorService.Model;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;

namespace MonitorService
{
    public class OperLogThread
    {
        private ConcurrentBag<MZ_OperLog> _logs;
        private ITAServiceProvider _provider;
        private Thread _thread;
        private ManualResetEvent _mre = new ManualResetEvent(false);
        private bool _isAborted;
        public OperLogThread(ITAServiceProvider provider)
        {
            _isAborted = false;
            _logs = new ConcurrentBag<MZ_OperLog>();
            _provider = provider;
            _thread = new Thread(new ThreadStart(Excute));
            _thread.Start();
        }
        public void PushLog(string title, string result)
        {
            this.PushLog(TAAction.Current, title, result);
        }
        public void PushLog(TAAction ac, string title, string result)
        {
            if (ac == null)
            {
                return;
            }

            //存储操作日志
            MZ_OperLog operLog = new MZ_OperLog();
            operLog.title = title;
            operLog.status = 0;
            // 请求的地址
            operLog.oper_ip = IpHelper.GetIpAddr(ac.Context.Request);
            operLog.oper_location = string.Empty;
            operLog.oper_url = ac.Context.Request.Url.ToString();
            operLog.operator_type = ac.Context.GetTerminal();

            var loginUser = Data_ServerTokenInfo.From(ac.Context);
            if (loginUser != null)
            {
                operLog.oper_name = loginUser.UserName;
                operLog.oper_uid = loginUser.UserId;
                operLog.oper_org = loginUser.OrgId;
            }
            else
            {
                operLog.oper_name = string.Empty;
                operLog.oper_uid = 0;
                operLog.oper_org = 0;
            }

            Exception e = ac.Context.Items["OperException"] as Exception;
            if (e != null)
            {
                operLog.status = 1;
                operLog.error_msg = (e.Message + ";" + e.StackTrace).Limit(2000);
            }
            else
            {
                operLog.error_msg = string.Empty;
            }

            // 设置方法名称
            string className = ac.Controller;
            string methodName = ac.Action;
            operLog.method = ac.NameSpace + "." + className + "." + methodName;
            // 设置请求方式
            operLog.request_method = ac.Context.Request.HttpMethod;
            operLog.oper_time = DateTime.Now;

            //获取请求参数
            operLog.TmpParamObject = ac.Context.Items["OperParam"] as object[];
            operLog.oper_param = string.Empty;
            operLog.json_result = result.Limit(2000);
            _logs.Add(operLog);
            _mre.Set();
        }
        public void PushLog(MZ_OperLog log)
        {
            _logs.Add(log);
            _mre.Set();
        }

        private void Excute()
        {
            while (!_isAborted)
            {
                try
                {
                    MZ_OperLog log;
                    while (_logs.TryTake(out log))
                    {
                        if (log.TmpParamObject != null)
                        {
                            log.oper_param = Newtonsoft.Json.JsonConvert.SerializeObject(log.TmpParamObject).Limit(2000);
                        }
                        _provider.GetService<OperLogBLL>().InsertOperlog(log);
                    }
                    _mre.WaitOne();
                    _mre.Reset();
                }
                catch (ThreadAbortException)
                {
                    _isAborted = true;
                }
                catch (ThreadInterruptedException)
                {
                    _isAborted = true;
                }
            }

        }
    }
}
