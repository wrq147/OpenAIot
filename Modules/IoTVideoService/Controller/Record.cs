using AuthService.Controller;
using IoTVideoService.Business;
using IoTVideoService.Models;
using Common;
using Common.Share;
using TemplateAction.Route;

namespace IoTVideoService.Controller
{
    public class Record : AbstractLoginedController
    {
        private RecordBLL _recordBLL;
        public Record(RecordBLL recordBLL)
        {
            _recordBLL = recordBLL;
        }

        /// <summary>
        /// 查询录像计划列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotRecord>>> ListPage(In_RecordPage query)
        {
            return this.Success(await _recordBLL.SelectPage(query, GetUser()));
        }

        /// <summary>
        /// 查询录像日志列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotRecordLog>>> LogListPage(In_RecordLogPage query)
        {
            return this.Success(await _recordBLL.SelectLogPage(query));
        }

        /// <summary>
        /// 查询录像文件列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotRecordFile>>> FileListPage(In_RecordFilePage query)
        {

        }

        /// <summary>
        /// 添加录像计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Add(MZ_IotRecord data)
        {
            return (await _recordBLL.Insert(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 编辑录像计划
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> Edit(MZ_IotRecord data)
        {
            return (await _recordBLL.Update(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 录像计划详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotRecord>> Info(string id)
        {
            return (await _recordBLL.Info(id)).ToAjaxResult();
        }

        /// <summary>
        /// 删除录像计划
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<int>> Remove(string id)
        {
            return (await _recordBLL.Delete(id, GetUser())).ToAjaxResult();
        }
    }
}
