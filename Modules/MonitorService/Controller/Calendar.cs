using AuthService.Controller;
using Common.Share;
using Common;
using MonitorService.Business;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using MonitorService.Model;

namespace MonitorService.Controller
{
    public class Calendar : AbstractLoginedController
    {
        private CalendarBLL _calendarBLL;
        public Calendar(CalendarBLL calendarBLL)
        {
            _calendarBLL = calendarBLL;
        }


        /// <summary>
        /// 获取企业假期列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_HolidayOrg>>> List()
        {
            return (await _calendarBLL.GetHolidayOrgList(GetUser().OrgId)).ToAjaxResult();
        }


        /// <summary>
        /// 新增假期
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(DateTime data)
        {
            return (await _calendarBLL.AddHoliday(data, GetUser())).ToAjaxResult();
        }




        /// <summary>
        /// 删除假期
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(DateTime data)
        {
            await _calendarBLL.DelHoliday(data, GetUser());
            return this.Success<string>();
        }

        /// <summary>
        /// 获取节日类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_HolidayType>>> HolidayTypeList()
        {
            return (await _calendarBLL.HolidayTypeList(GetUser())).ToAjaxResult();
        }


        /// <summary>
        /// 新增节日类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> AddType(MZ_HolidayType data)
        {
            return (await _calendarBLL.AddHolidayType(data, GetUser())).ToAjaxResult();
        }


        /// <summary>
        /// 编辑节日类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> EditType(MZ_HolidayType data)
        {
            return (await _calendarBLL.EditHolidayType(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除节日类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> DelType(string id)
        {
            return (await _calendarBLL.DelHolidayType(id, GetUser())).ToAjaxResult();
        }
    }
}
