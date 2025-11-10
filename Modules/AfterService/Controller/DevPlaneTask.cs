using AuthService.Controller;
using Common.Share;
using AfterService.Business;
using AfterService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;
using Common;
using TemplateAction.Core;

namespace AfterService.Controller
{
    public class DevPlaneTask : AbstractLoginedController
    {
        private DevPlaneTaskBLL _devPlaneTaskBLL;
        public DevPlaneTask(DevPlaneTaskBLL devPlaneTaskBLL)
        {
            _devPlaneTaskBLL = devPlaneTaskBLL;
        }
        /// <summary>
        /// 计划任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_PlaneTask>>> List(In_DevPlaneTask query)
        {
            return this.Success(await _devPlaneTaskBLL.SelectList(query));
        }
        /// <summary>
        /// 计划任务日视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_PlaneDay>>> DayList(In_DevPlaneTask query)
        {
            return this.Success(await _devPlaneTaskBLL.SelectDayPage(query));
        }
        /// <summary>
        /// 获取计划任务详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_PlaneTask>> Info(string id)
        {
            return (await _devPlaneTaskBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 生成计划任务单编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/AfterService/DevPlaneTask/Add")]
        public async Task<DefaultAjaxResult<string>> GeneratePlaneNumber()
        {
            return this.Success(await _devPlaneTaskBLL.GeneratePlaneNumber());
        }
        /// <summary>
        /// 生成初始化表单数据
        /// </summary>
        /// <param name="devId"></param>
        /// <param name="planeId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Dictionary<string, object>>> FormData(string devId, string planeId)
        {
            return (await _devPlaneTaskBLL.CreateTaskForm(devId, planeId)).ToAjaxResult();
        }
        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AfterService/DevPlaneTask/Add")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(In_AddPlaneTask data)
        {
            return (await _devPlaneTaskBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/AfterService/DevPlaneTask/Cancel")]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Cancel(string id)
        {
            return (await _devPlaneTaskBLL.Cancel(id)).ToAjaxResult();
        }
    }
}
