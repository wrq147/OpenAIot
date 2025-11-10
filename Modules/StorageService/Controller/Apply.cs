using AuthService.Controller;
using Common.Share;
using StorageService.Business;
using StorageService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using Common;
namespace StorageService.Controller
{
    public class Apply : AbstractLoginedController
    {
        private ApplyBLL _applyBLL;
        public Apply(ApplyBLL applyBLL)
        {
            _applyBLL = applyBLL;
        }
        /// <summary>
        /// 获取出库申请单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_LeaveApply>>> List(In_LeaveApplyList query)
        {
            return this.Success((await _applyBLL.SelectByPage(query, GetUser())));
        }

        /// <summary>
        /// 待出库申请单数量
        /// </summary>
        /// <param name="houseId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> WaitCount(string houseId = "")
        {
            return this.Success(await _applyBLL.WaitCount(GetUser(), houseId));
        }
        /// <summary>
        /// 获取出库申请单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_LeaveApply>> Info(string id)
        {
            return (await _applyBLL.Info(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取出库申请单详情
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_LeaveApply>> InfoByNumber(string number)
        {
            return (await _applyBLL.InfoByNumber(number)).ToAjaxResult();
        }

        /// <summary>
        /// 新增出库申请单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Apply/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_LeaveApply data)
        {
            return (await _applyBLL.Add(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改出库申请单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Apply/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_LeaveApply data)
        {
            return (await _applyBLL.Edit(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 提交出库申请单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/StorageService/Apply/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> SubmitModel(In_SubmitStock data)
        {
            return (await _applyBLL.SubmitModel(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 取消出库申请单（待审核状态使用）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Apply/List")]
        public async Task<DefaultAjaxResult<string>> Cancel(string id)
        {
            return (await _applyBLL.Cancel(id, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 删除出库申请单（待提交和已取消状态使用）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Apply/List")]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _applyBLL.Remove(id, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 生成出库申请单编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/StorageService/Apply/List")]
        public async Task<DefaultAjaxResult<string>> GenerateNumber()
        {
            return this.Success(await _applyBLL.GenerateApplyNumber());
        }


        /// <summary>
        /// 生成申请单的初始化表单数据
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/StorageService/Apply/List")]
        public async Task<DefaultAjaxResult<Dictionary<string, object>>> FormData(MZ_LeaveApply apply)
        {
            return (await _applyBLL.CreateTaskForm(apply)).ToAjaxResult();
        }

    }
}
