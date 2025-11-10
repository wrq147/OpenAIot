using System;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace TemplateAction.Label
{
    /// <summary>
    /// 输出接口
    /// </summary>
    public interface IResult
    {
        Task Output(TAAction ac);
    }
}
