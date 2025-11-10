using AuthService.Controller;
using Common.Share;
using Common;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using MESService.Model;
using MESService.Business;

namespace MESService.Controller
{
    /// <summary>
    /// 不良品项API
    /// </summary>
    public class Defect : AbstractLoginedController
    {
        private DefectBLL _defectBLL;
        public Defect(DefectBLL defectBLL)
        {
            _defectBLL = defectBLL;
        }
        /// <summary>
        /// 不良品项列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_DefectType>>> List(In_DefectList query)
        {
            return this.Success(await _defectBLL.SelectList(query, GetUser()));
        }
        /// <summary>
        /// 添加不良品项
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Defect/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_DefectType data)
        {
            return (await _defectBLL.Add(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 修改不良品项
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/MESService/Defect/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_DefectType data)
        {
            return (await _defectBLL.Edit(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 删除不良品项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/MESService/Defect/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _defectBLL.Delete(id, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 获取不良品项信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_DefectType>> Info(string id)
        {
            return (await _defectBLL.Info(id)).ToAjaxResult();
        }
    }
}
