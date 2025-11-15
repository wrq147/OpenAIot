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
    /// <summary>
    /// 
    /// </summary>
    public class Factor: AbstractLoginedController
    {
        private FactorBLL _factorBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factorBLL"></param>
        public Factor(FactorBLL factorBLL)
        {
            _factorBLL = factorBLL;
        }

        #region 排放因子及类别
        /// <summary>
        /// 获取排放因子类别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_FactorType>>> SelectFactorTypeTree()
        {
            return this.Success(_factorBLL.SelectFactorTypeTree().Result);
        }

        /// <summary>
        /// 获取排放因子类别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<Out_FactorType>> SelectFactorTypeInfo(string Id)
        {
            return this.Success(_factorBLL.SelectFactorType(Id).Result);
        }

        /// <summary>
        /// 新增排放因子类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddFactorType(In_FactorType in_FactorType)
        {
            return (await _factorBLL.AddFactorType(in_FactorType)).ToAjaxResult();
        }

        /// <summary>
        /// 修改排放因子类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> EditFactorType(In_FactorType in_FactorType)
        {
            return (await _factorBLL.UpdateFactorType(in_FactorType)).ToAjaxResult();
        }

        /// <summary>
        /// 删除排放因子类别
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveFactorType(string Id)
        {
            return (await _factorBLL.DeleteFactorTypeById(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 获取排放因子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_FactorTypeInfo>>> SelectFactorList(In_Factor query)
        {
            return this.Success(_factorBLL.SelectFactorList(query).Result);
        }

        /// <summary>
        /// 获取全部最新排放因子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_FactorTypeNew>>> SelectOrgFactorList()
        {
            return this.Success(_factorBLL.SelectOrgFactorList().Result);
        }

        /// <summary>
        /// 新增排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddFactor(In_Factor2 in_Factor)
        {
            return (await _factorBLL.AddFactor(in_Factor)).ToAjaxResult();
        }

        /// <summary>
        /// 修改排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> EditFactor(In_Factor2 in_Factor)
        {
            return (await _factorBLL.UpdateFactor(in_Factor)).ToAjaxResult();
        }

        /// <summary>
        /// 删除排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveFactor(string Id)
        {
            return (await _factorBLL.DeleteFactorById(Id)).ToAjaxResult();
        }

        #endregion

        #region 排放年份版本

        /// <summary>
        /// 新增排放年份
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddFactorYear(In_FactorYear in_Factor)
        {
            return (await _factorBLL.AddFactorYear(in_Factor)).ToAjaxResult();
        }

        /// <summary>
        /// 修改排放年份
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> EditFactorYear(In_FactorYear in_Factor)
        {
            return (await _factorBLL.UpdateFactorYear(in_Factor)).ToAjaxResult();
        }

        /// <summary>
        /// 删除排放年份
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveFactorYear(string Id)
        {
            return (await _factorBLL.DeleteFactorYearById(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 新增排放年份版本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> AddFactorYearVersion(In_FactorYearVersion in_Factor)
        {
            return (await _factorBLL.AddFactorYearVersion(in_Factor)).ToAjaxResult();
        }

        /// <summary>
        /// 修改排放年份版本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> EditFactorYearVersion(In_FactorYearVersion in_Factor)
        {
            return (await _factorBLL.UpdateFactorYearVersion(in_Factor)).ToAjaxResult();
        }

        /// <summary>
        /// 删除排放年份版本
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> RemoveFactorYearVersion(string Id)
        {
            return (await _factorBLL.DeleteFactorYearVersionById(Id)).ToAjaxResult();
        }

        /// <summary>
        /// 获取排放年份
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_FactorYear>>> SelectFactorYear()
        {
            return this.Success(_factorBLL.SelectFactorYear().Result);
        }

        #endregion

        #region 企业能源类型

        /// <summary>
        /// 新增企业排放因子
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> SaveFactorOrg(In_FactorOrg in_FactorOrg)
        {
            return (await _factorBLL.AddFactorOrg(in_FactorOrg)).ToAjaxResult();
        }

        /// <summary>
        /// 获取企业排放因子
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_Factor>>> SelectFactorOrg(string OrgId)
        {
            return this.Success(_factorBLL.SelectFactorOrg(OrgId).Result);
        }

        /// <summary>
        /// 获取企业排放因子根据能源类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_Factor>>> SelectFactorOrgByTypeId(string OrgId, string TypeId)
        {
            return this.Success(_factorBLL.SelectFactorOrgByTypeId(OrgId, TypeId).Result);
        }

        /// <summary>
        /// 获取企业能源类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<T_ENG_FactorType>>> SelectFactorTypeOrg(string OrgId)
        {
            return this.Success(_factorBLL.SelectFactorTypeOrg(OrgId).Result);
        }
        #endregion
    }
}
