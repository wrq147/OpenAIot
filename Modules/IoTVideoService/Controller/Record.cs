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

        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotRecord>>> ListPage(In_RecordPage query)
        {
            return this.Success(await _recordBLL.SelectPage(query, GetUser()));
        }
    }
}
