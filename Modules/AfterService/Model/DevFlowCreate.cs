using AuthService;
using AfterService.DAL;
using FlowService.FlowNode;
using FlowService.Model;
using IoTService.Models;
using TemplateAction.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AfterService.Model
{
    public class DevFlowCreate
    {
        /// <summary>
        /// 流程模板ID
        /// </summary>
        public long templateId { get; set; }
        /// <summary>
        /// 提交的表单数据
        /// </summary>
        public Dictionary<string, object> model { get; set; }
        /// <summary>
        /// 自选人
        /// </summary>
        public Dictionary<string, List<Out_UserItem>> assign { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 要编辑的工作流
        /// </summary>
        public long flowId { get; set; }
    }

    public class DevFlowItem
    {
        public string id { get; set; }
        public string title { get; set; }
        /// <summary>
        /// 表单项类型
        /// </summary>
        public string eltype { get; set; }
        /// <summary>
        /// 0为自定义、1为系统值
        /// </summary>
        public int way { get; set; }
        public object val { get; set; }

        public async Task<object> GetRealValue(ITAServiceProvider provider, MZ_IotDevice device, MZ_PlaneType plane, DeviceEventData data = null, MZ_AdminInfo submitUser = null, List<Out_DeviceWithRoome> roomList = null, List<MZ_RoomCategory> categoryList = null, List<MZ_AdminInfo> leaders = null)
        {
            if (way == 1)
            {

                string sysval = (string)val;
                switch (sysval)
                {
                    case "计划类型":
                        return plane.Name;
                    case "设备名称":
                        return device.Name;
                    case "设备编号":
                        return device.DeviceNumber;
                    case "通讯编号":
                        return device.DeviceId;
                    case "同名参数":
                        {
                            if (data == null || data.EventOutput == null)
                            {
                                return string.Empty;
                            }
                            if (data.EventOutput.TryGetValue(this.title, out object newval))
                            {
                                return newval;
                            }
                            return string.Empty;
                        }
                    case "目标设备":
                        {
                            List<DeviceData> tlist = new List<DeviceData>();
                            tlist.Add(new DeviceData()
                            {
                                id = device.Id,
                                type = "device",
                                name = device.Name,
                                photoUrl = device.PhotoUrl,
                                deviceNumber = device.DeviceNumber
                            });
                            return tlist;
                        }
                    case "提交人":
                        {
                            if (submitUser == null)
                            {
                                return string.Empty;
                            }
                            List<ObjData> tlist = new List<ObjData>();
                            tlist.Add(new ObjData()
                            {
                                id = submitUser.Id.Value,
                                type = "user",
                                name = submitUser.RealName,
                                avatar = submitUser.Avatar
                            });
                            return tlist;
                        }
                    case "提交人部门":
                        {
                            if (submitUser == null)
                            {
                                return string.Empty;
                            }
                            return submitUser.dept_name;
                        }
                    case "提交人姓名":
                        {
                            if (submitUser == null)
                            {
                                return string.Empty;
                            }
                            return submitUser.RealName;
                        }
                    case "房间名称":
                        {
                            if (roomList == null)
                            {
                                roomList = await provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(device.Id);
                            }

                            if (roomList.Count > 0)
                            {
                                return string.Join(',', roomList.Select(x => x.RoomName).ToList());
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                    case "房间分类名":
                        {
                            if (categoryList == null)
                            {
                                if (roomList == null)
                                {
                                    roomList = await provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(device.Id);
                                }
                                var cateIds = roomList.Select(x => x.CategoryId).ToList();
                                categoryList = await provider.GetService<RoomCategoryDAL>().SelectList(x => cateIds.Contains(x.Id));
                            }
                            if (categoryList.Count > 0)
                            {
                                return string.Join(',', categoryList.Select(x => x.Name).ToList());
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                    case "设备责任人":
                        {
                            if (leaders == null)
                            {
                                if (roomList == null)
                                {
                                    roomList = await provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(device.Id);
                                }

                                if (roomList.Count > 0)
                                {
                                    var tmpids = roomList.Select(x => x.LeaderId).ToList();
                                    leaders = await provider.GetService<UserDAL>().GetUserListByIds(tmpids);
                                }
                            }
                            List<ObjData> tlist = new List<ObjData>();
                            foreach (var ld in leaders)
                            {
                                tlist.Add(new ObjData()
                                {
                                    id = ld.Id.Value,
                                    type = "user",
                                    name = ld.RealName,
                                    avatar = ld.Avatar
                                });
                            }
                            return tlist;
                        }
                    case "设备责任与协作人":
                        {
                            if (leaders == null)
                            {
                                if (roomList == null)
                                {
                                    roomList = await provider.GetService<RoomDeviceDAL>().QueryDeivceWithRoom(device.Id);
                                }

                                if (roomList.Count > 0)
                                {
                                    var tmpids = new List<long>();
                                    foreach (var roomitem in roomList)
                                    {
                                        tmpids.Add(roomitem.LeaderId);
                                        if (!string.IsNullOrEmpty(roomitem.Helper))
                                        {
                                            long[] longArray = Array.ConvertAll(roomitem.Helper.Split(',', StringSplitOptions.RemoveEmptyEntries), s => long.Parse(s));
                                            tmpids.AddRange(longArray);
                                        }
                                    }
                                    leaders = await provider.GetService<UserDAL>().GetUserListByIds(tmpids);
                                }
                            }
                            List<ObjData> tlist = new List<ObjData>();
                            foreach (var ld in leaders)
                            {
                                tlist.Add(new ObjData()
                                {
                                    id = ld.Id.Value,
                                    type = "user",
                                    name = ld.RealName,
                                    avatar = ld.Avatar
                                });
                            }
                            return tlist;
                        }
                }
                return string.Empty;
            }
            else
            {
                return val;
            }
        }
    }
}
