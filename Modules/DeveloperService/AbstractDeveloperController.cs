using Common;
using Common.Share;
using DeveloperService.Model;
using TemplateAction.NetCore;

namespace DeveloperService
{
    public abstract class AbstractDeveloperController : TANetController, IDeveloperController
    {
        /// <summary>
        /// 获取当前开发者
        /// </summary>
        /// <returns></returns>
        protected MZ_Developer GetDeveloper()
        {
            return Context.Items["ApiDeveloper"] as MZ_Developer;
        }
    }
}
