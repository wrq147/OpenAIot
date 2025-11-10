using AuthService.Controller;
using Common;
using Common.Share;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using WeiXinService.Business;
using WeiXinService.Model;

namespace WeiXinService.Controller
{
    public class Corp : AbstractLoginedController
    {
        private CorpSyncBLL _corpSyncBLL;
        public Corp(CorpSyncBLL corpSyncBLL)
        {
            _corpSyncBLL = corpSyncBLL;
        }
        /// <summary>
        /// 启动定时同步企业微信
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [About("/WeiXinService/Account/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> StartSync(In_CorpSync data)
        {
            return (await _corpSyncBLL.StartSync(data)).ToAjaxResult();
        }
        /// <summary>
        /// 关闭定时同步企业微信
        /// </summary>
        /// <param name="appid"></param>
        /// <returns></returns>
        [About("/WeiXinService/Account/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> StopSync(string appid)
        {
            return (await _corpSyncBLL.StopSync(appid)).ToAjaxResult();
        }
        /// <summary>
        /// 手动同步企业微信
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="username"></param>
        /// <returns>创建的任务Id</returns>
        [About("/WeiXinService/Account/List")]
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> ManualSync(string appid, string username)
        {
            var syncdata = await _corpSyncBLL.Info(appid, username);
            return this.Success(await _corpSyncBLL.CreateTask(syncdata));
        }
        /// <summary>
        /// 获取企业微信同步信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/WeiXinService/Account/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_CorpSync>> Info(string id)
        {
            return this.Success(await _corpSyncBLL.Info(id));
        }
        /// <summary>
        /// 获取企业微信同步任务详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [About("/WeiXinService/Account/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_CorpTask>> TaskInfo(string id)
        {
            return this.Success(await _corpSyncBLL.TaskInfo(id));
        }
        /// <summary>
        /// 获取企业微信同步任务列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [About("/WeiXinService/Account/List")]
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_CorpTask>>> TaskList(In_TaskListPage query)
        {
            return this.Success(await _corpSyncBLL.TaskListPage(query));
        }

    }
}
