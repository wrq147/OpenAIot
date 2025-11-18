using AuthService;
using ChannelUtility;
using Common;
using Common.Share;
using DeveloperService;
using DeveloperService.Model;
using IoTRulesService.Business;
using IoTRulesService.Model;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;

namespace IoTRulesService.Controller
{
    public class HttpRule : AbstractDeveloperController
    {
        private MZ_Developer _develper;
        public HttpRule() { }
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
        /// <summary>
        /// 查询规则列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RuleListPage(In_ListPage query)
        {
            var ruleBLL = this.ServiceProvider.GetService<RuleBLL>();
            return this.Success(await ruleBLL.ListPage(query, _develper.ToUserInfo()));
        }
        /// <summary>
        /// 查询规则模板详情
        /// </summary>
        /// <param name="id">规则Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> RuleInfo(long id)
        {
            var ruleBLL = this.ServiceProvider.GetService<RuleBLL>();
            return (await ruleBLL.GetRule(id)).ToAjaxResult();
        }

        /// <summary>
        /// 保存规则参数
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> SaveRuleParams(In_RuleParams data)
        {
            var ruleBLL = this.ServiceProvider.GetService<RuleBLL>();
            MZ_RuleTemplate rule = new MZ_RuleTemplate();
            rule.Id = data.Id;
            rule.HttpParams = data.HttpParams;
            return (await ruleBLL.EditRule(rule, _develper.ToUserInfo())).ToAjaxResult();
        }
        /// <summary>
        /// 重置规则，参数会重置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> ResetRule(long id)
        {
            var ruleBLL = this.ServiceProvider.GetService<RuleBLL>();
            return (await ruleBLL.Reset(id, _develper.ToUserInfo())).ToAjaxResult();
        }
        /// <summary>
        /// 启用或暂停规则
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> EnableRule(In_RuleEnable data)
        {
            var ruleBLL = this.ServiceProvider.GetService<RuleBLL>();
            MZ_RuleTemplate rule = new MZ_RuleTemplate();
            rule.Id = data.Id;
            rule.Status = data.IsEnable ? "0" : "1";
            return (await ruleBLL.EditRule(rule, _develper.ToUserInfo())).ToAjaxResult();
        }
        /// <summary>
        /// 触发规则引擎的指定Http规则
        /// </summary>
        /// <param name="data">参数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Execute(In_Execute data)
        {
            var ruleBLL = this.ServiceProvider.GetService<RuleBLL>();
            return (await ruleBLL.Execute(data.Id, 1, null, 0, data.Inputs)).ToAjaxResult();
        }

        /// <summary>
        /// 获取设备离在线状态
        /// </summary>
        /// <param name="id">通讯Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<bool>> IsOnline(string id)
        {
            var deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            return this.Success(await deviceBLL.IsOnline(id));
        }
        /// <summary>
        /// 获取指定设备的实时属性数据（离线返回null）
        /// </summary>
        /// <param name="id">通讯Id</param>
        /// <param name="needTag">是否需要显示同步到标签的属性</param>
        /// <param name="needSend">是否同时发送读取属性消息</param>
        /// <param name="needWait">是否同时等待数据</param>
        /// <param name="waitProps">需要等待的属性（不传则等待全部，多个用逗号隔开）</param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<DeviceProperty>>> Live(string id, bool needTag = false, bool needSend = false, bool needWait = true, string waitProps = "")
        {
            var deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            int sendWay = 0;
            if (needSend)
            {
                if (needWait)
                {
                    sendWay = 2;
                }
                else
                {
                    sendWay = 1;
                }
            }
            return (await deviceBLL.Live(_develper.ToUserInfo(), id, needTag, sendWay, waitProps)).ToAjaxResult();
        }

    
        /// <summary>
        /// 批量获取设备的实时数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<Out_BatchLive>>> BatchLive(In_DevBatchList data)
        {
            var deveuser = _develper.ToUserInfo();
            List<Out_BatchLive> glives = new List<Out_BatchLive>();
            var deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            foreach (var dtu in data.DtuIds)
            {
                Out_BatchLive tlive = new Out_BatchLive();
                tlive.DeviceId = dtu;
                int sendWay = 0;
                if (data.needSend == true)
                {
                    sendWay = 1;
                }
                var tproplist = await deviceBLL.Live(deveuser, dtu, data.needTag, sendWay);
                if (!tproplist.IsSuccess())
                {
                    continue;
                }
                if (tproplist.Data != null)
                {
                    if (data.Codes == null || data.Codes.Length == 0)
                    {
                        tlive.PropertyList = tproplist.Data;
                    }
                    else
                    {
                        tlive.PropertyList = tproplist.Data.Where(x => data.Codes.Contains(x.Code)).ToList();
                    }
                    glives.Add(tlive);
                }
            }
            return this.Success(glives);
        }
        /// <summary>
        /// 统计设备数据，并返回
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<Out_MergeItem>>> SelectMergeList(In_HistoryMergeListSync query)
        {
            var influxBLL = this.ServiceProvider.GetService<IotInfluxBLL>();
            return (await influxBLL.SelectMergeList(query)).ToAjaxResult();
        }
        /// <summary>
        /// 查询设备的历史数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<PageObject<DeviceProperty>>> SelectHistory(In_HistoryListSync query)
        {
            try
            {
                var influxBLL = this.ServiceProvider.GetService<IotInfluxBLL>();
                return this.Success(await influxBLL.SelectHistory(query));
            }
            catch (Exception ex)
            {
                return this.Error<PageObject<DeviceProperty>>(15, ex.Message);
            }

        }
        /// <summary>
        /// 查询设备的离在线历史数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<PageObject<Out_OnOffline>>> SelectOnlines(In_OnOfflineListSync query)
        {
            try
            {
                var influxBLL = this.ServiceProvider.GetService<IotInfluxBLL>();
                return this.Success(await influxBLL.SelectOnlines(query));
            }
            catch (Exception ex)
            {
                return this.Error<PageObject<Out_OnOffline>>(15, ex.Message);
            }

        }

    }
}
