using AuthService.Controller;
using CardService.Business;
using CardService.Model;
using Common;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;

namespace CardService.Controller
{
    /// <summary>
    /// 名片API
    /// </summary>
    public class Card : AbstractLoginedController
    {
        private CardBLL _cardBLL;
        public Card(CardBLL cardBLL)
        {
            _cardBLL = cardBLL;
        }
        /// <summary>
        /// 获取我的名片列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> List(In_Card_List query)
        {
            return this.Success(await _cardBLL.SelectList(query));
        }
        /// <summary>
        /// 获取指定名片的信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Info(long id)
        {
            return this.Success(await _cardBLL.SelectById(id));
        }
        /// <summary>
        /// 添加名片
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Add(MZ_Card data)
        {
            
            return (await _cardBLL.Add(data)).ToAjaxResult();
        }
        /// <summary>
        /// 编辑名片
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Edit(MZ_Card data)
        {

            return (await _cardBLL.Update(data)).ToAjaxResult();
        }
        /// <summary>
        /// 删除名片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Remove(long id)
        {
            return (await _cardBLL.Delete(id)).ToAjaxResult();
        }
        /// <summary>
        /// 切换名片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> Switch(long id)
        {
            return (await _cardBLL.Switch(id)).ToAjaxResult();
        }


    }
}
