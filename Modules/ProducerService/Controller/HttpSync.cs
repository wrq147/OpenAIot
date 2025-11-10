using ProducerService.Business;
using ProducerService.Model;
using DeveloperService;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using TemplateAction.Label;
using DeveloperService.Model;


namespace ProducerService.Controller
{
    /// <summary>
    /// 开发者用接口
    /// </summary>
    public class HttpSync : AbstractDeveloperController
    {
        private MZ_Developer _develper;
        /// <summary>
        /// 校验开发者权限
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            _develper = GetDeveloper();
            if (_develper.UserType != 1)
            {
                return this.Error<string>(11, "必需为企业开发者");
            }
            return await base.CallAction(ac, parameters);
        }


    }
}
