using AuthService.Business;
using AuthService.Model;
using Common;
using Common.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace AuthService.Controller
{
    /// <summary>
    /// App升级管理
    /// </summary>
    public class UpgradeMan : AbstractLoginedController
    {
        private UpgradeBLL _upgradeBLL;
        public UpgradeMan(UpgradeBLL upgradeBLL)
        {
            _upgradeBLL = upgradeBLL;
        }


        /// <summary>
        /// 获取App升级列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/AuthService/UpgradeMan/")]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Upgrade>>> List(In_UpgradeList query)
        {
            return this.Success(await _upgradeBLL.SelectList(query));
        }



        /// <summary>
        /// 获取指定升级详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/AuthService/UpgradeMan/")]
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Upgrade>> Info(int id)
        {
            return this.Success(await _upgradeBLL.SelectInfo(id));
        }



        /// <summary>
        /// 新增App升级
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/AuthService/UpgradeMan/")]
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Upgrade data)
        {
            return (await _upgradeBLL.InsertUpgrade(data)).ToAjaxResult();
        }


        /// <summary>
        /// 删除App升级
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [About("/AuthService/UpgradeMan/")]
        [HttpGet]
        public async Task<AjaxResult> Remove(int[] ids)
        {
            return (await _upgradeBLL.DeleteByIds(ids)).ToAjaxResult();
        }
    }
}
