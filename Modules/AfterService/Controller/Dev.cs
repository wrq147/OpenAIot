using AuthService.Controller;
using Common.Share;
using AfterService.Business;
using IoTService.Models;
using Common;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.NetCore;
using TemplateAction.Route;
using AfterService.Model;

namespace AfterService.Controller
{
    /// <summary>
    /// 客户设备列表
    /// </summary>
    public class Dev : AbstractLoginedController
    {
        private KFDeviceBLL _deviceBLL;
        public Dev(KFDeviceBLL deviceBLL)
        {
            _deviceBLL = deviceBLL;
        }

        /// <summary>
        /// 客户设备列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotDevice>>> List(In_KFDevListPage query)
        {
            var user = GetUser();
            var scope = await user.GetScope(this.ServiceProvider, "/AfterService/Room/List");
            return this.Success(await _deviceBLL.ListPage(query, user, scope));
        }
        /// <summary>
        /// 客户产品列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_KFProductName>>> ProductList(OutKFProductPage query)
        {
            return this.Success(await _deviceBLL.SelectKFProductList(query));
        }

        /// <summary>
        /// 客户修改设备名称
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> UpdateDevice(In_UpdateDevice data)
        {
            return (await _deviceBLL.UpdateDevice(data)).ToAjaxResult();
        }
        /// <summary>
        /// 将设备移动到指定房间
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> TransferTo(In_TransferTo data)
        {
            return (await _deviceBLL.TransferTo(data)).ToAjaxResult();
        }
        /// <summary>
        /// 获取设备的打印数据源信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<Out_DeviceOfRoom>> DevInfo(string id = "")
        {
            return (await _deviceBLL.DevInfo(id)).ToAjaxResult();
        }
    }
}
