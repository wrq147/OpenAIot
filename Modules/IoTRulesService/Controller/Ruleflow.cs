using AuthService.Controller;
using Common;
using Common.Share;
using IoTRulesService.Business;
using IoTRulesService.Model;
using IoTService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace IoTRulesService.Controller
{
    public class RuleFlow : AbstractLoginedController
    {
        private RuleBLL _ruleBLL;
        public RuleFlow(RuleBLL ruleBLL)
        {
            _ruleBLL = ruleBLL;
        }
        /// <summary>
        /// 分页查询规则模板
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ListPage(In_ListPage query)
        {
            return this.Success(await _ruleBLL.ListPage(query, GetUser()));
        }

        /// <summary>
        /// 获取规则模板
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return (await _ruleBLL.GetRule(id)).ToAjaxResult();
        }
        /// <summary>
        /// 添加规则
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [TANetValid]
        public async Task<AjaxResult> Add(MZ_RuleTemplate data)
        {
            return (await _ruleBLL.AddRule(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑规则
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [TANetValid]
        public async Task<AjaxResult> Edit(MZ_RuleTemplate data)
        {
            return (await _ruleBLL.EditRule(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 重置规则，参数会重置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Reset(long id)
        {
            return (await _ruleBLL.Reset(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _ruleBLL.Remove(id)).ToAjaxResult();
        }
        /// <summary>
        /// 启用规则调试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> DebugOn(long id)
        {
            await TAEventDispatcher.Instance.Dispatch(RuleChangeEvent.EventKey, new RuleChangeEvent(true, id));
            return this.Success(0);
        }
        /// <summary>
        /// 关闭规则调试
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> DebugOff(long id)
        {
            await TAEventDispatcher.Instance.Dispatch(RuleChangeEvent.EventKey, new RuleChangeEvent(false, id));
            return this.Success(0);
        }
        /// <summary>
        /// 获取规则节点列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About()]
        public async Task<DefaultAjaxResult<List<Out_RuleNodeItem>>> NodeList()
        {
            var dict = await this.ServiceProvider.GetService<IotRedisHelper>().HashGetAllAsync<string>("RuleExeNodes");
            List<Out_RuleNodeItem> items = new List<Out_RuleNodeItem>();
            foreach (var kvp in dict)
            {
                Out_RuleNodeItem tmp = new Out_RuleNodeItem();
                tmp.name = kvp.Key;
                if (DateTime.TryParse(kvp.Value, out DateTime dt))
                {
                    tmp.time = dt;
                }
                items.Add(tmp);
            }
            return this.Success(items);
        }
        /// <summary>
        /// 强制下线规则节点
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTRulesService/RuleFlow/NodeList")]
        public async Task<AjaxResult> ForcedDown(string name)
        {
            await this.ServiceProvider.GetService<ServerBusProxy>().ForcedDownNode(name);
            return this.Success<int>();
        }
        /// <summary>
        /// 上线指定名称的规则节点
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTRulesService/RuleFlow/NodeList")]
        public async Task<AjaxResult> RegNode(string name)
        {
            await this.ServiceProvider.GetService<ServerBusProxy>().RegName(name);
            return this.Success<int>();
        }
    }
}
