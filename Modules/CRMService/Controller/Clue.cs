using CRMService.Business;
using CRMService.Model;
using AuthService.Controller;
using Common.Share;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using TemplateAction.NetCore;

namespace CRMService.Controller
{
    public class Clue : AbstractLoginedController
    {
        private ClueBLL _clueBLL;
        public Clue(ClueBLL clueBLL)
        {
            _clueBLL = clueBLL;
        }
        /// <summary>
        /// 公海线索
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Clue>>> PubList(In_ClueList query)
        {
            query.Belong = 1;
            return this.Success(await _clueBLL.SelectList(query));
        }
        /// <summary>
        /// 私海线索
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_Clue>>> PriList(In_ClueList query)
        {
            query.Belong = 2;
            return this.Success(await _clueBLL.SelectList(query));
        }

        /// <summary>
        /// 添加线索（私海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        [Des("添加私海线索")]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Clue data)
        {
            return (await _clueBLL.Add(data, false)).ToAjaxResult();
        }
        /// <summary>
        /// 添加线索（公海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        [Des("添加公海线索")]
        public async Task<DefaultAjaxResult<string>> PubAdd(MZ_Clue data)
        {
            return (await _clueBLL.Add(data, true)).ToAjaxResult();
        }
        /// <summary>
        /// 获取线索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Clue>> Info(long id)
        {
            return (await _clueBLL.Info(id)).ToAjaxResult();
        }


        /// <summary>
        /// 修改线索（私海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        [Des("修改一条私海线索")]
        public async Task<DefaultAjaxResult<string>> Edit(MZ_Clue data)
        {
            return (await _clueBLL.Edit(data, false)).ToAjaxResult();
        }
        /// <summary>
        /// 修改线索（公海）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        [TANetValid]
        [Des("修改一条公海线索")]
        public async Task<DefaultAjaxResult<string>> PubEdit(MZ_Clue data)
        {
            return (await _clueBLL.Edit(data, true)).ToAjaxResult();
        }

        /// <summary>
        /// 转换为客户
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/CRMService/Clue/Edit")]
        [HttpPost]
        [Des("转换一条线索")]
        public async Task<DefaultAjaxResult<string>> Transform(In_ClueTrans data)
        {
            return (await _clueBLL.Transform(data)).ToAjaxResult();
        }
        /// <summary>
        /// 领取线索
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/CRMService/Clue/Edit")]
        [HttpGet]
        [Des("领取一条线索")]
        public async Task<DefaultAjaxResult<string>> Draw(string id)
        {
            return (await _clueBLL.Draw(id)).ToAjaxResult();
        }
        /// <summary>
        /// 退回线索
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [About("/CRMService/Clue/Edit")]
        [HttpGet]
        [Des("退回一条线索")]
        public async Task<DefaultAjaxResult<string>> Return(string id, string reason)
        {
            return (await _clueBLL.Return(id, reason)).ToAjaxResult();
        }
        /// <summary>
        /// 删除线索（私海）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        [Des("删除一条私海线索")]
        public async Task<DefaultAjaxResult<string>> Remove(string id)
        {
            return (await _clueBLL.Delete(id, false)).ToAjaxResult();
        }
        /// <summary>
        /// 删除线索（公海）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        [Des("删除一条公海线索")]
        public async Task<DefaultAjaxResult<string>> PubRemove(string id)
        {
            return (await _clueBLL.Delete(id, true)).ToAjaxResult();
        }
    }
}
