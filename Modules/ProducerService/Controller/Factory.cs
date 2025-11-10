using ProducerService.Business;
using ProducerService.Model;
using AuthService.Controller;
using TemplateAction.Core;
using TemplateAction.Route;
using Common.Share;
using Common;
using System.Threading.Tasks;

namespace ProducerService.Controller
{
    /// <summary>
    /// 工厂API
    /// </summary>
    public class Factory : AbstractLoginedController
    {
        private FactoryBLL _factoryBLL;
        public Factory(FactoryBLL factory)
        {
            _factoryBLL = factory;
        }
        /// <summary>
        /// 工厂列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_FactoryItem>>> List(In_FactoryList query)
        {
            return this.Success(await _factoryBLL.SelectList(query));
        }


        /// <summary>
        /// 添加新的工厂
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        [About]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Add(MZ_Factory factory)
        {
            return (await _factoryBLL.Add(factory)).ToAjaxResult();
        }
        /// <summary>
        /// 生产商配置专用
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        [About("/AgentMan/")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> Set(MZ_Factory factory)
        {
            var user = GetUser();
            factory.Id = user.OrgId;
            return (await _factoryBLL.Edit(factory)).ToAjaxResult();
        }
        /// <summary>
        /// 获取工厂信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_Factory>> Info(long id)
        {
            if (id <= 0)
            {
                var user = GetUser();
                id = user.OrgId;
            }

            return (await _factoryBLL.Info(id)).ToAjaxResult();
        }


        /// <summary>
        /// 删除工厂
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About]
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> Remove(long id)
        {
            return (await _factoryBLL.Delete(id)).ToAjaxResult();
        }
    }
}
