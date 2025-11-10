using AuthService;
using Common.IdGenerator;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotGroupBLL
    {
        private IotGroupDAL _groupDAL;
        private IotDeviceDAL _deviceDAL;
        private SnowflakeHelper _snowflake;
        private ITAServiceProvider _provider;
        public IotGroupBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, IotGroupDAL groupDAL, IotDeviceDAL deviceDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _groupDAL = groupDAL;
            _deviceDAL = deviceDAL;
        }
        public virtual async Task<List<MZ_IotGroup>> SelectAllOfOrg(IUserInfo user)
        {
            return await _groupDAL.SelectList((x) => x.OrgId == user.OrgId, "Sort asc");
        }

        public virtual async Task<MZ_IotGroup> Info(string id)
        {
            return await _groupDAL.Select(id);
        }
        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _groupDAL.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }
        private async Task _UpdateChildren(string newPath, string oldPath)
        {
            List<MZ_IotGroup> children = await _groupDAL.SelectList(x => x.Path.StartsWith(oldPath));
            foreach (MZ_IotGroup child in children)
            {
                child.Path = newPath + child.Path.Substring(oldPath.Length);
            }
            if (children.Count > 0)
            {
                await _groupDAL.UpdateChildrenPath(children);
            }
        }
        public virtual async Task<BusResponse<int>> Update(MZ_IotGroup data)
        {
            data.OrgId = null;
            if (data.ParentId != null)
            {
                if (data.ParentId == data.Id)
                {
                    return BusResponse<int>.Error(111, "父级分组错误");
                }
                var old = await _groupDAL.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<int>.Error(112, "分组不存在");
                }
                if (data.ParentId == "")
                {
                    data.Path = data.Id + ",";
                }
                else
                {
                    var parent = await _groupDAL.Select(data.ParentId);
                    if (parent == null)
                    {
                        return BusResponse<int>.Error(113, "父分组不存在");
                    }
                    data.Path = parent.Path + data.Id + ",";
                }

                await _UpdateChildren(data.Path, old.Path);
            }
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            data.SetUpdateBy(user);
            return BusResponse<int>.Success(await _groupDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_IotGroup data)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加设备分组");
            }
            data.Id = _snowflake.NextId().ToString();
            data.ParentId ??= string.Empty;
            data.Remark ??= string.Empty;
            data.OrgId = user.OrgId;
            if (!string.IsNullOrEmpty(data.ParentId))
            {
                var parent = await _groupDAL.Select(data.ParentId);
                if (parent == null)
                {
                    return BusResponse<int>.Error(113, "父分组不存在");
                }
                data.Path = parent.Path + data.Id + ",";
            }
            else
            {
                data.Path = data.Id + ",";
            }
            data.SetCreateBy(user);
            return BusResponse<int>.Success(await _groupDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var old = await _groupDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "分组不存在");
            }
            //判断分组下面是否有设备
            if (await _deviceDAL.ExistDevice(old.Path))
            {
                return BusResponse<int>.Error(115, "分组存在设备，无法删除");
            }
            return BusResponse<int>.Success(await _groupDAL.Delete(x => x.Path.StartsWith(old.Path)));
        }

    }
}
