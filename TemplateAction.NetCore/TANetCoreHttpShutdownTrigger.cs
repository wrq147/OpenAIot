using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpShutdownTrigger : IDisposable
    {
        private readonly CancellationTokenSource _cts;
        private readonly ManualResetEventSlim _resetEvent;
        private bool _disposed = false;
        private bool _exitedGracefully = false;

        public TANetCoreHttpShutdownTrigger(CancellationTokenSource cts, ManualResetEventSlim resetEvent)
        {
            _cts = cts;
            _resetEvent = resetEvent;
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            Console.CancelKeyPress += CancelKeyPress;
        }

        internal void SetExitedGracefully()
        {
            _exitedGracefully = true;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
            Console.CancelKeyPress -= CancelKeyPress;
        }

        private void CancelKeyPress(object sender, ConsoleCancelEventArgs eventArgs)
        {
            Shutdown();
            // 不要立即终止进程，等待主线程正常退出
            eventArgs.Cancel = true;
        }

        private void ProcessExit(object sender, EventArgs eventArgs)
        {
            Shutdown();
            if (_exitedGracefully)
            {
                //在Linux上，如果关闭是由SIGTERM触发的，则会用143退出代码发出信号。这里需将退出代码设置为0 https://github.com/dotnet/aspnetcore/issues/6526
                Environment.ExitCode = 0;
            }
        }

        private void Shutdown()
        {
            try
            {
                if (!_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
            }
            // When hosting with IIS in-process, we detach the Console handle on main thread exit.
            // Console.WriteLine may throw here as we are logging to console on ProcessExit.
            // We catch and ignore all exceptions here. Do not log to Console in thie exception handler.
            catch (Exception) { }
            // 等待指定的重置事件，保证关闭处理程序先执行
            _resetEvent.Wait();
        }
    }
}
