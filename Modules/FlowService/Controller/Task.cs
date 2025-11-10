using AuthService.Controller;
using FlowService.Business;
using FlowService.Model;
using System;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using System.Collections.Generic;
using Common.Share;
using MonitorService;

namespace FlowService.Controller
{
    /// <summary>
    /// 流程实例接口
    /// </summary>
    [About("/FlowService/Task")]
    public class Task : AbstractLoginedController
    {
        private TaskBLL _taskBLL;
        public Task(TaskBLL taskBLL)
        {
            _taskBLL = taskBLL;
        }
        /// <summary>
        /// 生成流程单号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("Add")]
        public async Task<DefaultAjaxResult<string>> GenerateNumber()
        {
            return this.Success(await _taskBLL.GenerateFFNumber());
        }
        /// <summary>
        /// 查询已办任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("List")]
        [HttpGet]
        public async Task<AjaxResult> FinishedList(In_FinishedList query)
        {
            return this.Success(await _taskBLL.GetFinishedList(query));
        }
        /// <summary>
        /// 查询抄送我的列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("CCList")]
        [HttpGet]
        public async Task<AjaxResult> CSList(In_CSList query)
        {
            return this.Success(await _taskBLL.GetCSList(query));
        }
        /// <summary>
        /// 获取流程定义列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> DefinitionList(In_DefinitionList query = null)
        {
            if (query == null)
            {
                query = new In_DefinitionList();
            }
            return this.Success(await _taskBLL.Select(query));
        }
        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("Add")]
        [HttpPost]
        public async Task<AjaxResult> Add(In_TaskAdd data)
        {
            if (data.assign == null)
            {
                data.assign = new Dictionary<string, List<Out_UserItem>>();
            }
            var user = GetUser();
            try
            {
                var res = (await _taskBLL.CreateFlow(data.templateId, data.model, data.assign, data.state, data.isEmbed ?? false, user.UserId, data.flowId)).ToAjaxResult();
                if (res.code == 0)
                {
                    if (data.state != 0)
                    {
                        var fdd = res.data as MZ_FormData;
                        if (data.state == 2)
                        {
                            this.ServiceProvider.GetService<OperLogThread>().PushLog($"发起流程【{fdd.Template.Name}】", fdd.flowId.ToString());
                        }
                        return this.Success(fdd.flowId);
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                return this.Error<string>(21, ex.Message);
            }

        }

        /// <summary>
        /// 我的发起的流程
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("FlowList")]
        [HttpGet]
        public async Task<AjaxResult> MyProcess(In_MyProcess query)
        {
            return this.Success(await _taskBLL.GetMyProcessList(query));
        }

        /// <summary>
        /// 取消流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("FlowList")]
        [HttpPost]
        public async Task<AjaxResult> Cancel(long id)
        {
            return (await _taskBLL.CancelFlow(id)).ToAjaxResult();
        }
        /// <summary>
        /// 删除我的流程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("FlowList")]
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _taskBLL.DeleteFlow(id)).ToAjaxResult();
        }



        /// <summary>
        /// 查询待办任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("WaitList")]
        [HttpGet]
        public async Task<AjaxResult> TodoList(In_TodoList query)
        {
            return this.Success(await _taskBLL.GetTodoTaskList(query));
        }

        /// <summary>
        /// 执行流程
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("WaitList")]
        [HttpPost]
        public async Task<AjaxResult> Excute(In_TaskExcute data)
        {
            return (await _taskBLL.ExcuteFlow(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 获取指定节点的操作表单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/FlowService/Task/WaitList,/FlowService/Task/List,/FlowService/Task/CCList")]
        [HttpGet]
        public async Task<AjaxResult> GetUserActionForm(long id)
        {
            return (await _taskBLL.GetUserActionForm(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取根节点的操作表单信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("Add")]
        [HttpGet]
        public async Task<AjaxResult> GetUserRootForm(long id)
        {
            return (await _taskBLL.GetUserRootForm(id)).ToAjaxResult();
        }

        /// <summary>
        /// 获取流程的输入参数
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_FlowQuery>>> GetQuery(In_FlowQuery query)
        {
            return (await _taskBLL.GetQueryList(query)).ToAjaxResult();
        }
    }
}
