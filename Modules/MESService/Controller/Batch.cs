using AuthService.Controller;
using Common;
using Common.Share;
using MESService.Business;
using MESService.Model;
using ProducerService.Business;
using ProducerService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Route;

namespace MESService.Controller
{
    /// <summary>
    /// 生产批次接口
    /// </summary>
    public class Batch : AbstractLoginedController
    {
        private WorkBatchBLL _batchBLL;
        public Batch(WorkBatchBLL batchBLL)
        {
            _batchBLL = batchBLL;
        }
        /// <summary>
        /// 获取生产批次列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_WorkBatch>>> List(In_WorkBatchList query)
        {
            return this.Success(await _batchBLL.SelectByPage(query, GetUser()));
        }
    }
}
