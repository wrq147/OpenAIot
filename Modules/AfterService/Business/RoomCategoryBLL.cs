using Common.IdGenerator;
using AfterService.DAL;
using AfterService.Model;
using Common.Share;
using TemplateAction.Core;
using AuthService;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;


namespace AfterService.Business
{
    public class RoomCategoryBLL
    {
        private ITAServiceProvider _provider;
        private RoomCategoryDAL _roomCategoryDAL;
        private SnowflakeHelper _snowflake;
        public RoomCategoryBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, RoomCategoryDAL roomCategoryDAL)
        {
            _snowflake = snowflake;
            _provider = provider;
            _roomCategoryDAL = roomCategoryDAL;
        }
        public virtual async Task<List<MZ_RoomCategory>> SelectAll(long orgid)
        {
            var user = _provider.GetUser();
            return await _roomCategoryDAL.SelectList((x) => x.OrgId == user.OrgId && x.TargetOrgId == orgid, "Sort asc");
        }
        public virtual async Task<MZ_RoomCategory> Info(string id)
        {
            var roomCategory = await _roomCategoryDAL.Select(id);
            if (!string.IsNullOrEmpty(roomCategory.CustomerId))
            {
                var tmpname = await _provider.GetService<RoomDAL>().GetRoomCustomerName(roomCategory.CustomerId);
                if (!string.IsNullOrEmpty(tmpname))
                {
                    roomCategory.TargetName = tmpname;
                }
            }
            else if (roomCategory.TargetOrgId > 0)
            {
                MZ_Org org = await _provider.GetService<OrgDAL>().SelectById(roomCategory.TargetOrgId.Value);
                if (org != null)
                {
                    roomCategory.TargetName = org.OrgName;
                }
            }
            return roomCategory;
        }
        public virtual async Task<BusResponse<int>> UpdateSort(List<string> idList)
        {
            try
            {
                return BusResponse<int>.Success(await _roomCategoryDAL.UpdateSort(idList));
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(112, ex.Message);
            }
        }

        private async Task _UpdateChildren(string newPath, string oldPath)
        {
            List<MZ_RoomCategory> children = await _roomCategoryDAL.SelectList(x => x.Path.StartsWith(oldPath));
            foreach (MZ_RoomCategory child in children)
            {
                child.Path = newPath + child.Path.Substring(oldPath.Length);
            }
            if (children.Count > 0)
            {
                await _roomCategoryDAL.UpdateChildrenPath(children);
            }
        }
        public virtual async Task<BusResponse<int>> Update(MZ_RoomCategory data)
        {
            data.OrgId = null;
            if (data.ParentId != null)
            {
                if (data.ParentId == data.Id)
                {
                    return BusResponse<int>.Error(111, "父级分组错误");
                }
                var old = await _roomCategoryDAL.Select(data.Id);
                if (old == null)
                {
                    return BusResponse<int>.Error(112, "分类不存在");
                }
                if (data.ParentId == "")
                {
                    data.Path = data.Id + ",";
                }
                else
                {
                    var parent = await _roomCategoryDAL.Select(data.ParentId);
                    if (parent == null)
                    {
                        return BusResponse<int>.Error(113, "父分类不存在");
                    }
                    data.Path = parent.Path + data.Id + ",";
                }

                await _UpdateChildren(data.Path, old.Path);
            }

            return BusResponse<int>.Success(await _roomCategoryDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Insert(MZ_RoomCategory data)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(112, "非企业用户无法添加房间分类");
            }
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.ParentId ??= string.Empty;
            data.TargetOrgId ??= 0;

            if (!string.IsNullOrEmpty(data.ParentId))
            {
                var parent = await _roomCategoryDAL.Select(data.ParentId);
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
            return BusResponse<int>.Success(await _roomCategoryDAL.Insert(data));
        }

        public virtual async Task<BusResponse<int>> Remove(string id)
        {
            var old = await _roomCategoryDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(114, "分类不存在");
            }

            return BusResponse<int>.Success(await _roomCategoryDAL.Delete(x => x.Path.StartsWith(old.Path)));
        }

    }
}
