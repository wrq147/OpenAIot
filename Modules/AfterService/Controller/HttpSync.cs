using AfterService.Business;
using AfterService.Model;
using DeveloperService;
using System.Threading.Tasks;
using TemplateAction.Core;
using TemplateAction.Route;
using Common;
using TemplateAction.Label;
using DeveloperService.Model;
using AuthService;
using Common.Share;
using System.Collections.Generic;
using IoTService.Business;
using IoTService.Models;


namespace AfterService.Controller
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
        /// 获取计划任务的统计信息
        /// </summary>
        /// <param name="typeid">计划类型编码</param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<Out_PlaneStatis>> PlaneTaskStatis(string typeid)
        {
            var user = _develper.ToUserInfo();
            return this.Success(await this.ServiceProvider.GetService<DevPlaneTaskBLL>().GetPlaneStatistics(user, typeid));
        }
        /// <summary>
        /// 获取计划任务统计信息列表（按类型分组统计）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<Out_PlaneStatisItem>>> PlaneTaskStatisList(In_PlaneTaskStatisList data)
        {
            var user = _develper.ToUserInfo();
            return this.Success(await this.ServiceProvider.GetService<DevPlaneTaskBLL>().GetPlaneTaskStatisList(user, data));
        }
        /// <summary>
        /// 获取计划类型列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]

        public async Task<DefaultAjaxResult<List<ObjectItem>>> PlaneTaskTypeList()
        {
            var user = _develper.ToUserInfo();
            return this.Success(await this.ServiceProvider.GetService<DevPlaneBLL>().SelectNameList(user));
        }
        /// <summary>
        /// 查询房间列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<MZ_Room>>> RoomList()
        {
            var user = _develper.ToUserInfo();
            var tlist = await this.ServiceProvider.GetService<RoomBLL>().QueryList(new In_RoomList(), user);
            return this.Success(tlist);
        }
        /// <summary>
        /// 按房间查询实时数据（离线返回null）
        /// </summary>
        /// <param name="roomId">房间Id</param>
        /// <param name="needTag">是否需要显示同步到标签的属性</param>
        /// <param name="needSend">是否同时发送读取属性消息</param>
        /// <param name="needWait">是否同时等待数据</param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<Out_RoomLive>>> RoomLive(string roomId, bool needTag = false, bool needSend = false, bool needWait = true)
        {
            var user = _develper.ToUserInfo();
            List<Out_RoomLive> glives = new List<Out_RoomLive>();
            var kfDeviceBLL = this.ServiceProvider.GetService<KFDeviceBLL>();
            var deviceBLL = this.ServiceProvider.GetService<IotDeviceBLL>();
            In_KFDevListPage query = new In_KFDevListPage();
            query.showAll = true;
            query.RoomId = roomId;
            var tlist = await kfDeviceBLL.ListPage(query, user, null);
            foreach (var dev in tlist.List)
            {
                Out_RoomLive tlive = new Out_RoomLive();
                tlive.Id = dev.Id;
                tlive.DeviceName = dev.Name;
                tlive.DeviceNumber = dev.DeviceNumber;
                tlive.DeviceId = dev.DeviceId;
                int sendWay = 0;
                if (needSend)
                {
                    if (needWait)
                    {
                        sendWay = 2;
                    }
                    else
                    {
                        sendWay = 1;
                    }
                }
                var tproplist = await deviceBLL.Live(_develper.ToUserInfo(), dev.DeviceId, needTag, sendWay);
                if (!tproplist.IsSuccess())
                {
                    continue;
                }
                tlive.PropertyList = tproplist.Data;
                glives.Add(tlive);
            }
            return this.Success(glives);
        }
        /// <summary>
        /// 获取设备分布的统计信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="dstates">设备运行状态统计：维修、保养等（多个逗号分隔）</param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<Dictionary<string, int>>> Info(string code, string dstates = "")
        {
            var deviceBLL = this.ServiceProvider.GetService<KFDeviceBLL>();
            var user = _develper.ToUserInfo();
            return this.Success(await deviceBLL.SelectKFDevAreaInfo(code, dstates, user, null));
        }
        /// <summary>
        /// 获取设备分布数量列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<Out_DevAreaData>>> List(string code = "100000")
        {
            var deviceBLL = this.ServiceProvider.GetService<KFDeviceBLL>();
            var user = _develper.ToUserInfo();
            return this.Success(await deviceBLL.SelectAreaDataList(code, user, null));
        }
        /// <summary>
        /// 通过经纬度获取指定范围内的区域设备数量
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<Out_DevAreaData>>> RangeAreaList(In_DevRangeAreaList query)
        {
            var deviceBLL = this.ServiceProvider.GetService<KFDeviceBLL>();
            var user = _develper.ToUserInfo();
            return this.Success(await deviceBLL.SelectKFDevAreaDataListByRange(query, user, null));
        }
        /// <summary>
        /// 通过经纬度获取指定范围内的设备
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ShareCheck]
        public async Task<DefaultAjaxResult<List<MZ_IotDevice>>> RangeList(In_DevRangeList query)
        {
            var deviceBLL = this.ServiceProvider.GetService<KFDeviceBLL>();
            var user = _develper.ToUserInfo();
            return this.Success(await deviceBLL.SelectKFDeviceListByRange(query, user, null));
        }
    }
}
