using AuthService;
using AuthService.Controller;
using AuthService.Fields;
using ChannelUtility;
using Common;
using Common.Share;
using DeveloperService.Controller;
using IoTService.Business;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Label;
using TemplateAction.NetCore;
using TemplateAction.Route;

namespace IoTService.Controller
{
    public class IotDevice : AbstractLoginedController
    {
        private IotDeviceBLL _deviceBLL;
        private IotInfluxBLL _iotInflux;
        public IotDevice(IotDeviceBLL deviceBLL, IotInfluxBLL iotInflux)
        {
            _deviceBLL = deviceBLL;
            _iotInflux = iotInflux;
        }
        /// <summary>
        /// 获取设备离在线状态
        /// </summary>
        /// <param name="id">通讯Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<bool>> IsOnline(string id)
        {
            return this.Success(await _deviceBLL.IsOnline(id));
        }

        /// <summary>
        /// 设备列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<DefaultAjaxResult<PageObject<MZ_IotDevice>>> ListPage(In_DeviceListPage query)
        {
            return this.Success(await _deviceBLL.ListPage(query, GetUser()));
        }
        /// <summary>
        /// 获取指定设备信息
        /// </summary>
        /// <param name="id">设备Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotDevice>> Info(string id)
        {
            return this.Success(await _deviceBLL.Info(id, true));
        }
        /// <summary>
        /// 使用通讯Id获取设备信息
        /// </summary>
        /// <param name="dtuId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<MZ_IotDevice>> InfoOfDtuId(string dtuId)
        {
            return this.Success(await _deviceBLL.InfoByDtuId(dtuId));
        }

        /// <summary>
        /// 获取指定设备的功能列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_DeviceFunc>>> FuncList(string id)
        {
            return this.Success(await _deviceBLL.FuncList(id));
        }
        /// <summary>
        /// 执行设备的功能
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<IDictionary<string, object>>> ExeFunc(In_ExeFunc data)
        {
            return (await _deviceBLL.ExeFunc(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 将指定用户设置为使用者
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t">Id类型：0为Id,1为通讯Id,2为唯一编码</param>
        /// <param name="uid">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UseDeviceByUser(string id, int t, long uid)
        {
            var user = this.ServiceProvider.GetService<UserBLL>().GetUserInfoById(uid);
            ArtificialUser tmpuser = new ArtificialUser(uid, 0);
            return (await _deviceBLL.UseDevice(id, t, tmpuser)).ToAjaxResult();
        }
        /// <summary>
        /// 将当前用户的组织或个人设置为使用者
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t">Id类型：0为Id,1为通讯Id,2为唯一编码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UseDevice(string id, int t)
        {
            return (await _deviceBLL.UseDevice(id, t, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 将解除当前设备使用者的绑定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<AjaxResult> UnUseDevice(string id)
        {
            return (await _deviceBLL.UnUseDevice(id)).ToAjaxResult();
        }
        /// <summary>
        /// 获取指定设备的实时属性数据（离线返回null）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="needTag">是否显示标签绑定的属性</param>
        /// <param name="needSend">是否同时发送读取属性消息</param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<DeviceProperty>>))]
        public async Task<AjaxResult> Live(string id, bool needTag = false, bool needSend = false)
        {
            return (await _deviceBLL.Live(GetUser(), id, needTag, needSend ? 1 : 0)).ToAjaxResult();
        }
        /// <summary>
        /// 批量获取设备的实时数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_BatchLive>>> BatchLive(In_DevBatchList data)
        {
            var tmpuser = GetUser();
            List<Out_BatchLive> glives = new List<Out_BatchLive>();
            var deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            foreach (var dtu in data.DtuIds)
            {
                Out_BatchLive tlive = new Out_BatchLive();
                tlive.DeviceId = dtu;
                int sendWay = 0;
                if (data.needSend == true)
                {
                    sendWay = 1;
                }
                var tproplist = await deviceBLL.Live(tmpuser, dtu, data.needTag, sendWay);
                if (!tproplist.IsSuccess())
                {
                    continue;
                }
                if (tproplist.Data != null)
                {
                    if (data.Codes == null || data.Codes.Length == 0)
                    {
                        tlive.PropertyList = tproplist.Data;
                    }
                    else
                    {
                        tlive.PropertyList = tproplist.Data.Where(x => data.Codes.Contains(x.Code)).ToList();
                    }
                    glives.Add(tlive);
                }
            }
            return this.Success(glives);
        }
        /// <summary>
        /// 查询设备的异常数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<ExceptProperty>>> SelectExcepts(In_JVList query)
        {
            try
            {
                return this.Success(await this.ServiceProvider.GetService<IotExceptBLL>().SelectExceptList(query));
            }
            catch (Exception ex)
            {
                return this.Error<PageObject<ExceptProperty>>(15, ex.Message);
            }
        }
        /// <summary>
        /// 查询设备的历史数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<DeviceProperty>>> SelectHistory(In_HistoryList query)
        {
            try
            {
                return this.Success(await _iotInflux.SelectHistory(query));
            }
            catch (Exception ex)
            {
                return this.Error<PageObject<DeviceProperty>>(15, ex.Message);
            }
        }
        /// <summary>
        /// 统计设备数据，并返回
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<DefaultAjaxResult<List<Out_MergeItem>>> SelectMergeList(In_HistoryMergeList query)
        {
            return (await _iotInflux.SelectMergeList(query)).ToAjaxResult();
        }
        /// <summary>
        /// 查询设备的离在线历史数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<PageObject<Out_OnOffline>>> SelectOnlines(In_OnOfflineList query)
        {
            try
            {
                return this.Success(await _iotInflux.SelectOnlines(query));
            }
            catch (Exception ex)
            {
                return this.Error<PageObject<Out_OnOffline>>(15, ex.Message);
            }
        }
        /// <summary>
        /// 查询设备的标签值列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [TANetWebApiResponse(typeof(DefaultAjaxResult<List<Out_DeviceTagItem>>))]
        public async Task<AjaxResult> TagList(string id)
        {
            var device = await _deviceBLL.Info(id);
            if (device == null)
            {
                return this.Success(new List<Out_DeviceTagItem>());
            }
            return this.Success(await _deviceBLL.SelectTagsByDevice(device));
        }
        /// <summary>
        /// 查询协议的标签默认值列表
        /// </summary>
        /// <param name="id">协议id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<DefaultAjaxResult<List<Out_DeviceTagItem>>> TagListByProduct(string id)
        {
            return this.Success(await _deviceBLL.SelectTagsByProduct(id));
        }
        /// <summary>
        /// 保存设备标签值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> SaveTags(In_SaveTags data)
        {
            var device = await _deviceBLL.Info(data.id);
            if (device == null)
            {
                return this.Error<string>(14, "设备不存在");
            }
            return (await _deviceBLL.SaveTags(device, data.list, data.indate)).ToAjaxResult();
        }
        /// <summary>
        /// 保存设备属性值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> SaveProps(In_SaveProps data)
        {
            return (await _deviceBLL.SaveProps(data.Id, data.NewVals, data.Indate)).ToAjaxResult();
        }
        /// <summary>
        /// 删除历史数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> DelHistory(In_HistoryDelete data)
        {
            return (await this.ServiceProvider.GetService<IotInfluxBLL>().DeleteHistory(data)).ToAjaxResult();
        }
        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        [Des("新增了一台设备")]
        public async Task<AjaxResult> Add(MZ_IotDevice data)
        {
            return (await _deviceBLL.Insert(data, GetUser())).ToAjaxResult();
        }
        /// <summary>
        /// 编辑设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<AjaxResult> Edit(MZ_IotDevice data)
        {
            return (await _deviceBLL.Update(data, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 绑定物联设备
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AjaxResult> BindDevice(MZ_IotDevice data)
        {
            if (string.IsNullOrEmpty(data.DeviceNumber))
            {
                return this.Error<string>(12, "请传入批次编号");
            }
            IotDeviceBLL deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            var old = await deviceBLL.InfoByNumber(data.DeviceNumber);
            if (old == null)
            {
                return (await deviceBLL.Insert(data, GetUser())).ToAjaxResult();
            }
            else
            {
                return (await _deviceBLL.Update(data, GetUser())).ToAjaxResult();
            }
        }
        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        [Des("删除了一台设备")]
        public async Task<AjaxResult> Remove(string id)
        {
            return (await _deviceBLL.Remove(id, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 生成设备编号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [About("/IoTService/IotDevice/ListPage")]
        public async Task<DefaultAjaxResult<string>> GenerateNumber()
        {
            return this.Success(await _deviceBLL.GenerateNumber());
        }

        [About("/IoTService/IotDevice/ListPage")]
        [HttpPost]
        public async Task<AjaxResult> Import(bool updateSupport)
        {
            var form = await this.Context.Request.ReadFormAsync();
            if (form.Files.Length == 0)
            {
                return this.Error<string>(12, "请选择文件");
            }
            Stream st = form.Files[0].OpenReadStream();
            Dictionary<string, ParamImportToList> FiedNames = new Dictionary<string, ParamImportToList>();
            FiedNames.Add("第三方唯一编号", new ParamImportToList("DeviceNumber"));
            FiedNames.Add("图片地址", new ParamImportToList("PhotoUrl"));
            FiedNames.Add("设备名称", new ParamImportToList("Name"));
            FiedNames.Add("设备的DtuId", new ParamImportToList("DeviceId"));
            FiedNames.Add("协议Id", new ParamImportToList("ProductId"));
            FiedNames.Add("原价", new ParamImportToList("Price", val => Convert.ToDecimal(val)));
            FiedNames.Add("备注", new ParamImportToList("Remark"));
            List<MZ_IotDevice> list = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExcelToList<MZ_IotDevice>(st, FiedNames);
            return (await _deviceBLL.Import(list, updateSupport, GetUser())).ToAjaxResult();
        }

        /// <summary>
        /// 导出模板
        /// </summary>
        /// <returns></returns>
        [About("/IoTService/IotDevice/ListPage")]
        [HttpGet]
        public IResult ExportTemplate()
        {
            try
            {
                Dictionary<string, ParamRenderToExcel<MZ_IotDevice>> FiedNames = new Dictionary<string, ParamRenderToExcel<MZ_IotDevice>>();
                FiedNames.Add("DeviceNumber", new ParamRenderToExcel<MZ_IotDevice>("第三方唯一编号"));
                FiedNames.Add("PhotoUrl", new ParamRenderToExcel<MZ_IotDevice>("图片地址"));
                FiedNames.Add("Name", new ParamRenderToExcel<MZ_IotDevice>("设备名称"));
                FiedNames.Add("DeviceId", new ParamRenderToExcel<MZ_IotDevice>("设备的DtuId"));
                FiedNames.Add("ProductId", new ParamRenderToExcel<MZ_IotDevice>("协议Id"));
                FiedNames.Add("Price", new ParamRenderToExcel<MZ_IotDevice>("原价"));
                FiedNames.Add("Remark", new ParamRenderToExcel<MZ_IotDevice>("备注"));
                byte[] data = Context.Application.ServiceProvider.GetService<ExcelHelper>().ExportToBuffer<MZ_IotDevice>("设备表", new List<MZ_IotDevice>(), FiedNames);
                return Stream(Guid.NewGuid().ToString("N") + ".xls", data);
            }
            catch (Exception ex)
            {
                return this.Error<string>(12, ex.Message);
            }
        }
    }
}
