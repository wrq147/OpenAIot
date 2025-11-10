using System;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Label;

namespace TemplateAction.Core
{
    public class TAActionBuilder
    {

        private bool _async;
        public bool Async
        {
            get { return _async; }
        }
        private TAAction _request;
        public TAActionBuilder(ITAContext context, ControllerNode controller, ActionNode action, TAObjectCollection exparams)
        {
            _async = false;
            _request = new TAAction(context, controller, action, exparams);

            if (_request.ActionNode == null) return;
            _async = _request.ActionNode.Async;
        }



        /// <summary>
        /// 异步执行
        /// </summary>
        /// <returns></returns>
        public async Task<IResult> ExcuteAsync()
        {
            return await _request.Excute();
        }
        /// <summary>
        /// 同步执行
        /// </summary>
        /// <returns></returns>
        public IResult Excute()
        {
            return TAAsyncHelper.RunSync<IResult>(() => _request.Excute());
        }
        /// <summary>
        /// 异步输出
        /// </summary>
        /// <returns></returns>
        public async Task OutputAsync()
        {
            IResult rt = await _request.Excute();
            if (rt == null)
            {
                await new V404Result().Output(_request);
            }
            else
            {
                await rt.Output(_request);
            }
        }

        public void Output()
        {

            IResult rt = TAAsyncHelper.RunSync<IResult>(() => _request.Excute());
            if (rt == null)
            {
                TAAsyncHelper.RunSync(() => new V404Result().Output(_request));
            }
            else
            {
                TAAsyncHelper.RunSync(() => rt.Output(_request));
            }
        }
        /// <summary>
        /// 回调用异步输出
        /// </summary>
        /// <param name="res"></param>
        public void OutputAsync(ITAAsyncResult res)
        {
            _request.Excute().ContinueWith((t) =>
            {
                if (t.Exception != null)
                {
                    Exception ex = t.Exception;
                    if (t.Exception.InnerExceptions.Count > 0)
                    {
                        ex = t.Exception.InnerExceptions[0];
                    }
                    if (_request.ExceptionFun != null)
                    {
                        _request.ExceptionFun.Invoke(TAUtility.EXCEPTION_CODE, ex).Output(_request);
                    }
                    else
                    {
                        new TextResult(ex.Message).Output(_request).Wait();
                    }
                    res.Completed();
                    return;
                }

                if (t.Result == null)
                {
                    TAAsyncHelper.RunSync(() => new V404Result().Output(_request));
                }
                else
                {
                    TAAsyncHelper.RunSync(() => t.Result.Output(_request));
                }
                res.Completed();
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
