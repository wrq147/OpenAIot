using AuthService.Controller;
using Common;
using Common.Share;
using EfficiencyService.Business;
using EfficiencyService.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace EfficiencyService.Controller
{

    public class Policy : AbstractLoginedController
    {
        private PolicyBLL _policyBLL;

        public Policy(PolicyBLL policyBLL)
        {
            _policyBLL = policyBLL;
        }

        /// <summary>
        /// 电价政策信息
        /// </summary>
        /// <param name="Id">电价政策编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<In_Policy>> Info(string Id)
        {
            In_Policy data = await _policyBLL.SelectPolicy(Id);
            if (data != null)
            {
                return this.Success(data);
            }
            else
            {
                return this.Error(888, "没有找到", data);
            }
        }

        /// <summary>
        /// 电价政策列表
        /// </summary>
        /// <param TimeName="query">请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<PageObject<Out_Policy>>> List(In_PolicyList query)
        {
            return this.Success(await _policyBLL.SelectPolicyList(query));
        }

        /// <summary>
        /// 新增电价政策
        /// </summary>
        /// <param name="in_Policy">电价政策信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddPolicy(In_Policy in_Policy)
        {
            return (await _policyBLL.AddPolicy(in_Policy)).ToAjaxResult();
        }

        /// <summary>
        /// 修改电价政策
        /// </summary>
        /// <param name="in_Policy">电价政策信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> EditPolicy(In_Policy in_Policy)
        {
            return (await _policyBLL.UpdatePolicy(in_Policy)).ToAjaxResult();
        }

        /// <summary>
        /// 删除电价政策
        /// </summary>
        /// <param name="Id">电价政策编码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemovePolicy(string Id)
        {
            return (await _policyBLL.DeletePolicy(Id)).ToAjaxResult();
        }

    }
}
