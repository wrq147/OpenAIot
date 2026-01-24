using AuthService;
using AuthService.Model;
using ChannelUtility.Config;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using MyAccess.Aop;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotUpdateBLL
    {
        private ITAServiceProvider _provider;
        private IotUpdateDAL _updateDAL;
        public IotUpdateBLL(ITAServiceProvider provider, IotUpdateDAL updateDAL)
        {
            _provider = provider;
            _updateDAL = updateDAL;
        }
        public virtual async Task<PageObject<Out_UpdateItem>> ListPage(In_UpdatePage query)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            query.OrgId = user.OrgId;
            return await _updateDAL.SelectUpdateListPage(query);
        }
        public virtual async Task<BusResponse<string>> ManualUp(string id)
        {
            var deviceDAL = _provider.GetService<IotDeviceDAL>();
            var updateItem = await _updateDAL.Select(id);
            if (updateItem == null)
            {
                return BusResponse<string>.Error(110, "升级任务已不存在");
            }
            var device = await deviceDAL.Select(updateItem.Id);
            if (device == null)
            {
                await _updateDAL.Delete(x => x.Id == updateItem.Id);
                return BusResponse<string>.Error(111, "设备不存在");
            }
            var rs = await _provider.GetService<ServerBusProxy>().DownSyncDevice(device, updateItem.Version.Value);
            if (rs.IsSuccess())
            {
                await _updateDAL.Delete(x => x.Id == updateItem.Id);
                return BusResponse<string>.Success();
            }
            return BusResponse<string>.Error(112, $"升级失败，原因：{rs.Message}");
        }

        public virtual async Task<BusResponse<string>> ClearErr()
        {
            var user = _provider.GetUser();
            await _updateDAL.UpdateToNewest(user.OrgId);
            await _updateDAL.Delete(x => x.OrgId == user.OrgId && x.Status == 2);
            return BusResponse<string>.Success();
        }
    }
}
