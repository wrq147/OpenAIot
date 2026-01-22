using Common.Share;
using System;
using TemplateAction.Core;

namespace Common
{
    public static class TABaseControllerExtension
    {

        /// <summary>
        /// 错误时调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="code">错误代码</param>
        /// <param name="mess"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DefaultAjaxResult<T> Error<T>(this TABaseController controller, int code, string mess, T data = default)
        {
            return new DefaultAjaxResult<T>(code, mess, data);
        }

        /// <summary>
        /// 成功时调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DefaultAjaxResult<T> Success<T>(this TABaseController controller, T data = default)
        {
            return new DefaultAjaxResult<T>(Constants.SUCCESS_CODE, string.Empty, data);
        }
    }
}
