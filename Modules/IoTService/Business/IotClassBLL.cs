using AuthService;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using IoTService.DAL;
using IoTService.Models;
using MyAccess.DB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotClassBLL
    {
        private ITAServiceProvider _provider;
        private IotClassDAL _classDAL;
        private SnowflakeHelper _snowflake;
        public IotClassBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, IotClassDAL classDAL)
        {
            _snowflake = snowflake;
            _provider = provider;
            _classDAL = classDAL;
        }

        public virtual async Task<List<MZ_IotClass>> SelectAllClass()
        {
            var user = _provider.GetUser();
            return await _classDAL.SelectList((x) => x.OrgId == user.OrgId, "Sort asc");
        }

        public virtual async Task<MZ_IotClass> Info(string id)
        {
            return await _classDAL.Select(id);
        }
        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _classDAL.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }
        private async Task _UpdateChildren(string newPath, string oldPath)
        {
            List<MZ_IotClass> children = await _classDAL.SelectList(x => x.Path.StartsWith(oldPath));
            foreach (MZ_IotClass child in children)
            {
                child.Path = newPath + child.Path.Substring(oldPath.Length);
            }
            if (children.Count > 0)
            {
                await _classDAL.UpdateChildrenPath(children);
            }
        }
        public virtual async Task<BusResponse<int>> Update(MZ_IotClass data)
        {
            var user = _provider.GetUser();
            var old = await _classDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(113, "分类不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "所属组织错误");
            }
            data.OrgId = null;
            if (data.ParentId != null)
            {
                if (data.ParentId == "")
                {
                    data.Path = data.Id + ",";
                }
                else
                {
                    var parent = await _classDAL.Select(data.ParentId);
                    if (parent == null)
                    {
                        return BusResponse<int>.Error(113, "父分类不存在");
                    }
                    data.Path = parent.Path + data.Id + ",";
                }

                await _UpdateChildren(data.Path, old.Path);
            }
            if (data.Sort != null)
            {
                await _classDAL.SortIncrease(old.OrgId.Value, data.Sort.Value);
            }
            if (data.Name != null && data.Name != old.Name)
            {
                await _provider.GetService<IotDeviceDAL>().SetNeedUpdateKeywords(data.Path);
                await BusUtility.Dispatch("UpdateDeviceKeywords", new
                {
                    OrgId = user.OrgId
                });
            }
            return BusResponse<int>.Success(await _classDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_IotClass data)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加设备分类");
            }
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.ParentId ??= string.Empty;
            data.Remark ??= string.Empty;
            data.PhotoUrl ??= string.Empty;
            if (data.Sort != null)
            {
                data.Sort = 0;
            }
            await _classDAL.SortIncrease(data.OrgId.Value, data.Sort.Value);
            if (!string.IsNullOrEmpty(data.ParentId))
            {
                var parent = await _classDAL.Select(data.ParentId);
                if (parent == null)
                {
                    return BusResponse<int>.Error(113, "父分类不存在");
                }
                data.Path = parent.Path + data.Id + ",";
            }
            else
            {
                data.Path = data.Id + ",";
            }
            return BusResponse<int>.Success(await _classDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var user = _provider.GetUser();
            var old = await _classDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "分类不存在");
            }
            if (old.OrgId != user.OrgId)
            {
                return BusResponse<int>.Error(114, "所属组织错误");
            }
            return BusResponse<int>.Success(await _classDAL.Delete(x => x.Path.StartsWith(old.Path)));
        }


    }
}
