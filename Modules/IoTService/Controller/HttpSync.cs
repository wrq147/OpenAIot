using AuthService;
using ChannelUtility.Config;
using Common;
using Common.Share;
using DeveloperService;
using DeveloperService.Model;
using IoTService.Business;
using IoTService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.Route;

namespace IoTService.Controller
{
    public class HttpSync : AbstractDeveloperController
    {
        private MZ_Developer _develper;
        /// <summary>
        /// 校验开发者权限
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task<IResult> CallAction(TAAction ac, object[] parameters)
        {
            _develper = GetDeveloper();
            if (_develper.UserType != 1)
            {
                return this.Error<string>(11, "必需为企业开发者");
            }
            return await base.CallAction(ac, parameters);
        }


        /// <summary>
        /// 刷新所属所有协议的缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<string>> RefreshAllProductCache()
        {
            var user = _develper.ToUserInfo();
            IotProductBLL proBLL = this.ServiceProvider.GetService<IotProductBLL>();
            await proBLL.RefreshAllProductCache(user);
            return this.Success("刷新成功");
        }
        /// <summary>
        /// 保存设备标签
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<string>> SaveTags(In_SyncSaveTags data)
        {
            var user = _develper.ToUserInfo();
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            var device = await deviceBLL.InfoByNumber(data.number);
            if (device == null)
            {
                return this.Error<string>(14, "设备不存在");
            }
            return (await deviceBLL.SaveTags(device, data.list, null)).ToAjaxResult();
        }

        /// <summary>
        /// 获取当前告警数量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<int>> WarnCount()
        {
            var user = _develper.ToUserInfo();
            IotWarningBLL warnBLL = this.ServiceProvider.GetService<IotWarningBLL>();
            return (await warnBLL.QueryCount(user)).ToAjaxResult();
        }
        /// <summary>
        /// 分页获取告警列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotWarning>>> WarnListPage(In_WarningListPage data)
        {
            var user = _develper.ToUserInfo();
            IotWarningBLL warnBLL = this.ServiceProvider.GetService<IotWarningBLL>();
            return this.Success(await warnBLL.ListPage(data, user));
        }
        /// <summary>
        /// 清理指定告警
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> ClearWarn(long id, string remark = "")
        {
            var user = _develper.ToUserInfo();
            IotWarningBLL warnBLL = this.ServiceProvider.GetService<IotWarningBLL>();
            return (await warnBLL.ClearWarning(id, remark, user)).ToAjaxResult();
        }
        /// <summary>
        /// 清理所有告警
        /// </summary>
        /// <param name="remark"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<int>> ClearAllWarn(string remark = "")
        {
            var user = _develper.ToUserInfo();
            IotWarningBLL warnBLL = this.ServiceProvider.GetService<IotWarningBLL>();
            return (await warnBLL.ClearAllWarning(remark, user)).ToAjaxResult();
        }
        /// <summary>
        /// 执行设备的功能
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<IDictionary<string, object>>> ExeFunc(In_SyncExeFunc data)
        {
            var user = _develper.ToUserInfo();
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            In_ExeFunc exeFunc = new In_ExeFunc();
            var olddev = await deviceBLL.InfoByNumber(data.Number);
            if (olddev == null)
            {
                return this.Error<IDictionary<string, object>>(32, "设备不存在");
            }
            exeFunc.Id = olddev.Id;
            exeFunc.FunctionId = data.FunctionId;
            if (data.Inputs == null)
            {
                exeFunc.Inputs = new Dictionary<string, object>();
            }
            else
            {
                exeFunc.Inputs = data.Inputs;
            }
            return (await deviceBLL.ExeFunc(exeFunc, user, olddev)).ToAjaxResult();
        }

        /// <summary>
        /// 设备列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotDevice>>> ListPage(In_DeviceListPage query)
        {
            var user = _develper.ToUserInfo();
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            return this.Success(await deviceBLL.ListPage(query, user));
        }
        /// <summary>
        /// 通过设备唯一编号获取设备信息
        /// </summary>
        /// <param name="number">设备唯一编号</param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<MZ_IotDevice>> DeviceByNumber(string number)
        {
            var user = _develper.ToUserInfo();
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            var old = await deviceBLL.InfoByNumber(number, true);
            if (old == null)
            {
                return this.Error<MZ_IotDevice>(32, "设备不存在");
            }
            if (old.OrgId != _develper.OrgId)
            {
                return this.Error<MZ_IotDevice>(33, "设备不在您的企业下");
            }
            return this.Success(old);
        }

        /// 获取产品名称列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<PageObject<Out_ProductName>>> ProductNames(In_ProductNamePage query)
        {
            var user = _develper.ToUserInfo();
            IotProductBLL productBLL = this.ServiceProvider.GetService<IotProductBLL>();
            return this.Success(await productBLL.ProductNamePage(query, user));
        }

        /// <summary>
        /// 通过批次编号获取设备的标签列表
        /// </summary>
        /// <param name="number">批次编号</param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<AjaxResult> TagList(string number)
        {
            var user = _develper.ToUserInfo();
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            var device = await deviceBLL.InfoByNumber(number);
            if (device == null)
            {
                return this.Success(new List<Out_DeviceTagItem>());
            }
            return this.Success(await deviceBLL.SelectTagsByDevice(device));
        }
        /// <summary>
        /// 获取协议名称列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<PageObject<Out_ProductName>>> ProtocalNames(In_ProductNamePage query)
        {
            var user = _develper.ToUserInfo();
            IotProductBLL productBLL = this.ServiceProvider.GetService<IotProductBLL>();
            return this.Success(await productBLL.ProductNamePage(query, user));
        }
        /// <summary>
        /// 获取协议信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="notsl"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<MZ_IotProduct>> ProtocalInfo(string id)
        {
            var user = _develper.ToUserInfo();
            IotProductBLL productBLL = this.ServiceProvider.GetService<IotProductBLL>();
            var rs = await productBLL.Info(id, false);
            if (rs.OrgId != user.OrgId)
            {
                return this.Error<MZ_IotProduct>(33, "协议不在您的企业下");
            }
            return this.Success(rs);
        }
        /// <summary>
        /// 添加开发者的物联设备信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> AddDevice(In_SyncDevice data)
        {
            if (string.IsNullOrEmpty(data.DeviceNumber))
            {
                return this.Error<string>(12, "请传入批次编号");
            }
            if (data.DeviceNumber.StartsWith("SB"))
            {
                return this.Error<string>(13, "批次编号不可使用SB开头");
            }
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            MZ_IotDevice dev = new MZ_IotDevice();
            dev.PhotoUrl = data.PhotoUrl;
            dev.ProductId = data.ProtocalId;
            dev.MesProductId = data.ProductId;
            dev.Name = data.Name;
            dev.DeviceId = data.DeviceId;
            dev.Tags = data.Tags;
            var user = _develper.ToUserInfo();
            dev.DeviceNumber = data.DeviceNumber;
            return (await deviceBLL.Insert(dev, user)).ToAjaxResult();
        }

        /// <summary>
        /// 修改开发者的物联设备信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> EditDevice(In_SyncDevice data)
        {
            if (string.IsNullOrEmpty(data.DeviceNumber))
            {
                return this.Error<string>(12, "请传入批次编号");
            }
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            MZ_IotDevice dev = new MZ_IotDevice();
            if (!string.IsNullOrEmpty(data.PhotoUrl))
            {
                dev.PhotoUrl = data.PhotoUrl;
            }
            if (!string.IsNullOrEmpty(data.ProtocalId))
            {
                dev.ProductId = data.ProtocalId;
            }
            if (!string.IsNullOrEmpty(data.ProductId))
            {
                dev.MesProductId = data.ProductId;
            }
            if (!string.IsNullOrEmpty(data.Name))
            {
                dev.Name = data.Name;
            }
            if (!string.IsNullOrEmpty(data.DeviceId))
            {
                dev.DeviceId = data.DeviceId;
            }
            if (data.Tags != null)
            {
                dev.Tags = data.Tags;
            }

            var user = _develper.ToUserInfo();
            dev.DeviceNumber = data.DeviceNumber;
            var old = await deviceBLL.InfoByNumber(data.DeviceNumber);
            if (old == null)
            {
                return this.Error<MZ_IotDevice>(32, "设备不存在");
            }
            dev.Id = old.Id;
            return (await deviceBLL.Update(dev, user, old)).ToAjaxResult();
        }
        /// <summary>
        /// 绑定开发者的物联设备信息(在不知道是添加还是更新的情况下使用)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> BindDevice(In_SyncDevice data)
        {
            if (string.IsNullOrEmpty(data.DeviceNumber))
            {
                return this.Error<string>(12, "请传入批次编号");
            }
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            var old = await deviceBLL.InfoByNumber(data.DeviceNumber);
            if (old == null)
            {
                MZ_IotDevice dev = new MZ_IotDevice();
                dev.PhotoUrl = data.PhotoUrl;
                dev.ProductId = data.ProtocalId;
                dev.MesProductId = data.ProductId;
                dev.Name = data.Name;
                dev.DeviceId = data.DeviceId;
                dev.Tags = data.Tags;
                var user = _develper.ToUserInfo();
                dev.DeviceNumber = data.DeviceNumber;
                return (await deviceBLL.Insert(dev, user)).ToAjaxResult();
            }
            else
            {
                MZ_IotDevice dev = new MZ_IotDevice();
                if (!string.IsNullOrEmpty(data.PhotoUrl))
                {
                    dev.PhotoUrl = data.PhotoUrl;
                }
                if (!string.IsNullOrEmpty(data.ProtocalId))
                {
                    dev.ProductId = data.ProtocalId;
                }
                if (!string.IsNullOrEmpty(data.ProductId))
                {
                    dev.MesProductId = data.ProductId;
                }
                if (!string.IsNullOrEmpty(data.Name))
                {
                    dev.Name = data.Name;
                }
                if (!string.IsNullOrEmpty(data.DeviceId))
                {
                    dev.DeviceId = data.DeviceId;
                }
                if (data.Tags != null)
                {
                    dev.Tags = data.Tags;
                }
                if (!string.IsNullOrEmpty(data.DeviceNumber))
                {
                    dev.DeviceNumber = data.DeviceNumber;
                }
                dev.Id = old.Id;
                var user = _develper.ToUserInfo();
                return (await deviceBLL.Update(dev, user, old)).ToAjaxResult();
            }
        }
        /// <summary>
        /// 删除开发者的设备
        /// </summary>
        /// <param name="number">设备唯一编号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> DelDevice(string number)
        {
            var user = _develper.ToUserInfo();
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            var old = await deviceBLL.InfoByNumber(number);
            if (old == null)
            {
                return this.Error<int>(32, "设备不存在");
            }
            return (await deviceBLL.Remove(old.Id, user)).ToAjaxResult();
        }

        /// <summary>
        /// 设备的物联卡续费
        /// </summary>
        /// <param name="number"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> Recharge(string number, int month)
        {
            var user = _develper.ToUserInfo();
            IotCardBLL cardBLL = this.ServiceProvider.GetService<IotCardBLL>();
            var cardInfo = await cardBLL.GetCard(number);
            if (cardInfo == null)
            {
                return this.Error<MZ_IotDevice>(33, "物联卡不存在");
            }
            string[] ids = new string[] { cardInfo.Id };
            return (await cardBLL.Recharge(ids, month)).ToAjaxResult();
        }
        /// <summary>
        /// 获取设备的物联卡信息
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotCard>> GetCard(string number)
        {
            var user = _develper.ToUserInfo();
            IotCardBLL cardBLL = this.ServiceProvider.GetService<IotCardBLL>();
            return this.Success(await cardBLL.GetCard(number));
        }
        /// <summary>
        /// 批量获取设备的物联卡信息
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<MZ_IotCard>>> GetCardList(string[] numbers)
        {
            var user = _develper.ToUserInfo();
            IotCardBLL cardBLL = this.ServiceProvider.GetService<IotCardBLL>();
            return this.Success(await cardBLL.GetCardList(numbers));
        }
    }
}
