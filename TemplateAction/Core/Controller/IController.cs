using System;
using System.Reflection;
using System.Threading.Tasks;
using TemplateAction.Label;
namespace TemplateAction.Core
{
    public interface IController
    {
        TAAction IntentAction { get; }
        /// <summary>
        /// 绑定当前action
        /// </summary>
        /// <param name="controller"></param>
        void Bind(TAAction handle);
        /// <summary>
        /// 执行动作
        /// </summary>
        Task<IResult> CallAction(TAAction ac, object[] parameters);


        /// <summary>
        /// 异常时执行
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        IResult Exception(int code, Exception ex);

    }
}
