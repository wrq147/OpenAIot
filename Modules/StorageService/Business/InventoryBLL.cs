using AuthService;
using Common.IdGenerator;
using Common.Share;
using StorageService.DAL;
using StorageService.Model;
using Microsoft.Extensions.Options;
using MyAccess.Aop;
using TemplateAction.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Common.EventBus;
using ProducerService.Model;
using ProducerService.DAL;

namespace StorageService.Business
{
    public class InventoryBLL
    {
        private IOptions<GeneralOption> _conf;
        private ITAServiceProvider _provider;
        private OperatorHelper _operator;
        private SnowflakeHelper _snowflake;
        private InventoryDAL _inventoryDAL;
        private InventoryItemDAL _inventoryItemDAL;
        private InventoryUserDAL _inventoryUserDAL;
        private StoreHouseDAL _houseDAL;
        private StockPileDAL _pileDAL;
        private StockRecordDAL _recordDAL;
        public InventoryBLL(ITAServiceProvider provider, IOptions<GeneralOption> conf, OperatorHelper operatorHelper, SnowflakeHelper snowflake,
            InventoryDAL inventoryDAL, InventoryItemDAL inventoryItemDAL, InventoryUserDAL inventoryUserDAL, StoreHouseDAL houseDAL, StockPileDAL pileDAL, StockRecordDAL recordDAL)
        {
            _provider = provider;
            _conf = conf;
            _operator = operatorHelper;
            _snowflake = snowflake;
            _inventoryDAL = inventoryDAL;
            _inventoryItemDAL = inventoryItemDAL;
            _inventoryUserDAL = inventoryUserDAL;
            _houseDAL = houseDAL;
            _pileDAL = pileDAL;
            _recordDAL = recordDAL;
        }

        public virtual async Task<PageObject<MZ_Inventory>> SelectList(In_InventoryList query, bool isTask)
        {
            var user = _provider.GetUser();
            var pageList = await _inventoryDAL.SelectByPage(query, user, isTask);
            if (pageList.List.Count > 0)
            {
                var inventoryIds = pageList.List.Select(x => x.Id).ToList();
                Dictionary<string, MZ_Inventory> invDict = new Dictionary<string, MZ_Inventory>();
                foreach (var invitem in pageList.List)
                {
                    invDict.Add(invitem.Id, invitem);
                }
                var inventoryUsers = await _inventoryUserDAL.SelectListWithUser(x => inventoryIds.Contains(x.InventoryId));
                foreach (var item in inventoryUsers)
                {
                    MZ_Inventory invout;
                    if (invDict.TryGetValue(item.InventoryId, out invout))
                    {
                        if (invout.UserList == null)
                        {
                            invout.UserList = new List<MZ_InventoryUser>();
                        }
                        invout.UserList.Add(item);
                    }
                }
            }

            return pageList;
        }
        public virtual async Task<PageObject<Out_InventoryItem>> SelectItemList(In_InventoryItem data)
        {
            return await _inventoryItemDAL.SelectItemList(data);
        }
        public virtual async Task<BusResponse<Out_InventoryItem>> SelectItemInfo(string id, string number)
        {
            var itemInfo = await _inventoryItemDAL.SelectItemByNumber(id, number);
            return BusResponse<Out_InventoryItem>.Success(itemInfo);
        }
        public virtual async Task<BusResponse<int>> ConfirmItem(string id, string number, int count)
        {
            var user = _provider.GetUser();
            var info = await _inventoryDAL.Select(id);
            if (info == null)
            {
                return BusResponse<int>.Error(111, "盘点不存在");
            }

            if (info.Status == 2)
            {
                if (await _inventoryUserDAL.Count(x => x.InventoryId == id && x.TimeIn == 0 && x.UserId == user.UserId) == 0)
                {
                    return BusResponse<int>.Error(112, "您无权无权进行初盘");
                }
                var infoItem = await _inventoryItemDAL.SelectItemByNumber(id, number);
                if (infoItem == null)
                {
                    return BusResponse<int>.Error(113, "该物品不在盘点任务里");
                }
                MZ_InventoryItem item = new MZ_InventoryItem();
                item.Id = infoItem.Id;
                item.FirstCount = count;
                item.FirstUserId = user.UserId;
                item.FirstAtTime = DateTime.Now;
                await _inventoryItemDAL.Update(item);
                return BusResponse<int>.Success();
            }
            else if (info.Status == 3)
            {
                if (await _inventoryUserDAL.Count(x => x.InventoryId == id && x.TimeIn == 1 && x.UserId == user.UserId) == 0)
                {
                    return BusResponse<int>.Error(112, "您无权无权进行复盘");
                }
                var infoItem = await _inventoryItemDAL.SelectItemByNumber(id, number);
                if (infoItem == null)
                {
                    return BusResponse<int>.Error(113, "该物品不在盘点任务里");
                }
                MZ_InventoryItem item = new MZ_InventoryItem();
                item.Id = infoItem.Id;
                item.CheckCount = count;
                item.CheckUserId = user.UserId;
                item.CheckAtTime = DateTime.Now;
                await _inventoryItemDAL.Update(item);
                return BusResponse<int>.Success();
            }
            else
            {
                return BusResponse<int>.Error(121, "盘点状态错误");
            }
        }
        public virtual async Task<BusResponse<MZ_Inventory>> Info(string id)
        {
            var info = await _inventoryDAL.Select(id);
            if (info == null)
            {
                return BusResponse<MZ_Inventory>.Error(111, "盘点不存在");
            }

            var inventoryUsers = await _inventoryUserDAL.SelectListWithUser(x => x.InventoryId == info.Id);
            info.UserList = new List<MZ_InventoryUser>();
            foreach (var item in inventoryUsers)
            {
                info.UserList.Add(item);
            }
            if (!string.IsNullOrEmpty(info.HouseId))
            {
                info.House = await _houseDAL.Select(info.HouseId);
            }

            return BusResponse<MZ_Inventory>.Success(info);
        }
        public virtual async Task<BusResponse<int>> DelItems(string id, List<string> ids)
        {
            var info = await _inventoryDAL.Select(id);
            if (info == null)
            {
                return BusResponse<int>.Error(111, "盘点不存在");
            }
            if (info.Status != 0)
            {
                return BusResponse<int>.Error(112, "盘点状态错误");
            }
            if (ids == null || ids.Count == 0)
            {
                return BusResponse<int>.Error(111, "请传入要删除的盘点项");
            }
            return BusResponse<int>.Success(await _inventoryItemDAL.Delete(x => x.InventoryId == id && ids.Contains(x.TargetId)));
        }
        public virtual async Task<BusResponse<int>> AddItems(string id, List<MZ_InventoryItem> tlist)
        {
            var info = await _inventoryDAL.Select(id);
            if (info == null)
            {
                return BusResponse<int>.Error(111, "盘点不存在");
            }
            if (info.Status != 0)
            {
                return BusResponse<int>.Error(112, "盘点状态错误");
            }
            foreach (var item in tlist)
            {
                item.Id = MyAccess.Core.StringTool.GetGUID();
                item.InventoryId = id;
                item.OrgId = info.OrgId;
                item.HouseId = info.HouseId;
                item.FirstUserId = 0;
                item.CheckUserId = 0;
            }

            return BusResponse<int>.Success(await _inventoryItemDAL.Insert(tlist));
        }
        public virtual async Task<BusResponse<int>> AddItemsAll(In_AllItem data)
        {
            return BusResponse<int>.Success(await _inventoryItemDAL.InsertByHouseId(data));
        }
        public virtual async Task<BusResponse<int>> DelItemsAll(string id)
        {
            return BusResponse<int>.Success(await _inventoryItemDAL.Delete(x => x.InventoryId == id));
        }
        public virtual async Task<BusResponse<int>> Remove(string[] ids)
        {
            var user = _provider.GetUser();
            if (user.OrgId <= 0)
            {
                return BusResponse<int>.Error(124, "请切换使用企业用户");
            }
            return BusResponse<int>.Success(await _inventoryDAL.Delete(x => x.Status == 0 && x.OrgId == user.OrgId && ids.Contains(x.Id)));
        }
        [Trans]
        public virtual async Task<BusResponse<string>> Add(MZ_Inventory data)
        {
            var user = _provider.GetUser();
            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.Status = 0;
            data.SetCreateBy(user);
            if (data.UserList == null || data.UserList.Count() <= 0)
            {
                return BusResponse<string>.Error(123, "盘点人员不能为空");
            }
            foreach (var u in data.UserList)
            {
                u.InventoryId = data.Id;
            }
            await _inventoryUserDAL.Insert(data.UserList);
            await _inventoryDAL.Insert(data);
            return BusResponse<string>.Success(data.Id);
        }
        [Trans]
        public virtual async Task<BusResponse<int>> Edit(MZ_Inventory data)
        {
            MZ_Inventory old = await _inventoryDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "盘点不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }
            data.Status = null;
            data.OrgId = null;
            data.SetUpdateBy(user);

            if (data.UserList != null)
            {
                if (data.UserList.Count() <= 0)
                {
                    return BusResponse<int>.Error(123, "盘点人员不能为空");
                }
                await _inventoryUserDAL.Delete(x => x.InventoryId == data.Id);
                foreach (var u in data.UserList)
                {
                    u.InventoryId = data.Id;
                }
                await _inventoryUserDAL.Insert(data.UserList);
            }
            return BusResponse<int>.Success(await _inventoryDAL.Update(data));
        }
        public virtual async Task<BusResponse<int>> Submit(string id)
        {
            MZ_Inventory old = await _inventoryDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "盘点不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }
            if (old.Status != 0)
            {
                return BusResponse<int>.Error(125, "当前状态错误");
            }
            MZ_Inventory newInvt = new MZ_Inventory();
            newInvt.Id = id;
            newInvt.Status = 1;
            return BusResponse<int>.Success(await _inventoryDAL.Update(newInvt));
        }
        public virtual async Task<BusResponse<int>> Start(string[] ids)
        {
            foreach (string id in ids)
            {
                MZ_Inventory old = await _inventoryDAL.Select(id);
                if (old == null)
                {
                    return BusResponse<int>.Error(123, "盘点不存在");
                }
                var user = _provider.GetUser();
                if (user.OrgId != old.OrgId)
                {
                    return BusResponse<int>.Error(124, "当前用户无权限");
                }
                if (old.Status != 1)
                {
                    return BusResponse<int>.Error(125, "当前状态错误");
                }
                MZ_Inventory newInvt = new MZ_Inventory();
                newInvt.Id = id;
                newInvt.Status = 2;
                newInvt.StartOn = DateTime.Now;
                await _inventoryItemDAL.InitSnapQuantity(id);
                await _inventoryDAL.Update(newInvt);
            }

            return BusResponse<int>.Success();
        }
        public virtual async Task<BusResponse<int>> Check(string[] ids)
        {
            foreach (string id in ids)
            {
                MZ_Inventory old = await _inventoryDAL.Select(id);
                if (old == null)
                {
                    return BusResponse<int>.Error(123, "盘点不存在");
                }
                var user = _provider.GetUser();
                if (user.OrgId != old.OrgId)
                {
                    return BusResponse<int>.Error(124, "当前用户无权限");
                }
                if (old.Status != 2)
                {
                    return BusResponse<int>.Error(125, "当前状态错误");
                }
                var inventoryUsers = await _inventoryUserDAL.SelectListWithUser(x => x.InventoryId == old.Id);
                if (inventoryUsers == null || inventoryUsers.Where(x => x.TimeIn == 1).Count() == 0)
                {
                    return BusResponse<int>.Error(126, "未选择复盘人员,无法复盘");
                }


                MZ_Inventory newInvt = new MZ_Inventory();
                newInvt.Id = id;
                newInvt.Status = 3;
                newInvt.CheckOn = DateTime.Now;
                await _inventoryDAL.Update(newInvt);
            }

            return BusResponse<int>.Success();
        }
        public virtual async Task<BusResponse<int>> Cancel(string[] ids)
        {
            foreach (string id in ids)
            {
                MZ_Inventory old = await _inventoryDAL.Select(id);
                if (old == null)
                {
                    return BusResponse<int>.Error(123, "盘点不存在");
                }

                var user = _provider.GetUser();
                if (user.OrgId != old.OrgId)
                {
                    return BusResponse<int>.Error(124, "当前用户无权限");
                }
                if (old.Status != 1 && old.Status != 2 && old.Status != 3)
                {
                    return BusResponse<int>.Error(125, "盘点状态错误");
                }
                MZ_Inventory newInvt = new MZ_Inventory();
                newInvt.Id = id;
                newInvt.Status = 6;
                newInvt.StartOn = DateTime.Now;
                await _inventoryDAL.Update(newInvt);
            }

            return BusResponse<int>.Success();
        }
        public virtual async Task<BusResponse<int>> Finish(string[] ids)
        {
            foreach (string id in ids)
            {
                MZ_Inventory old = await _inventoryDAL.Select(id);
                if (old == null)
                {
                    return BusResponse<int>.Error(123, "盘点不存在");
                }

                var user = _provider.GetUser();
                if (user.OrgId != old.OrgId)
                {
                    return BusResponse<int>.Error(124, "当前用户无权限");
                }
                if (old.Status != 2 && old.Status != 3)
                {
                    return BusResponse<int>.Error(125, "盘点状态错误");
                }

                MZ_Inventory newInvt = new MZ_Inventory();
                newInvt.Id = id;
                newInvt.Status = 4;
                newInvt.EndOn = DateTime.Now;
                await _inventoryDAL.Update(newInvt);
            }

            return BusResponse<int>.Success();
        }

        [Trans]
        public virtual async Task<BusResponse<int>> Repair(string id, List<MZ_InventoryItem> items)
        {
            MZ_Inventory old = await _inventoryDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "盘点不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }
            if (old.Status != 4)
            {
                return BusResponse<int>.Error(125, "盘点状态错误");
            }
            var ids = items.Select(x => x.Id).ToList();
            var oldList = await _inventoryItemDAL.SelectList(x => ids.Contains(x.Id));
            Dictionary<string, MZ_InventoryItem> oldDict = new Dictionary<string, MZ_InventoryItem>();
            foreach (MZ_InventoryItem oldItem in oldList)
            {
                oldDict.Add(oldItem.Id, oldItem);
            }

            List<string> deviceIds = new List<string>();
            foreach (MZ_InventoryItem item in items)
            {
                if (oldDict.TryGetValue(item.Id, out MZ_InventoryItem oldii))
                {
                    MZ_InventoryItem invItem = new MZ_InventoryItem();
                    invItem.Id = item.Id;
                    invItem.Count = item.Count;
                    invItem.DiffCount = item.Count - oldii.SnapQuantity;
                    await _inventoryItemDAL.Update(invItem);


                    //修改库存
                    List<MZ_StockPile> oldPileList = (await _pileDAL.SelectList(x => x.HouseId == old.HouseId && x.TargetType == item.TargetType && x.TargetId == item.TargetId));
                    if (oldPileList.Count == 0)
                    {
                        return BusResponse<int>.Error(126, "盘点的物品不存在");
                    }
                    decimal absCount = Math.Abs(invItem.DiffCount.Value);
                    if (invItem.DiffCount > 0)
                    {
                        //生成盘盈修正记录
                        await _recordDAL.InsertRecord(old.OrgId.Value, old.HouseId, item.TargetType.Value, item.TargetId, 3, id, absCount, oldPileList[0].Price.Value);

                        MZ_StockPile pile = new MZ_StockPile();
                        pile.HouseId = old.HouseId;
                        pile.TargetType = item.TargetType;
                        pile.TargetId = item.TargetId;
                        pile.Quantity = absCount;
                        pile.Price = oldPileList[0].Price;
                        await _pileDAL.IncreasePile(pile);
                    }
                    else if (invItem.DiffCount < 0)
                    {
                        //生成盘亏修正记录
                        await _recordDAL.InsertRecord(old.OrgId.Value, old.HouseId, item.TargetType.Value, item.TargetId, 2, id, absCount, oldPileList[0].Price.Value);
                        if (await _pileDAL.ReducePile(old.HouseId, item.TargetType.Value, item.TargetId, absCount, oldPileList[0].Price) == 0)
                        {
                            return BusResponse<int>.Error(131, "库存不足");
                        }
                        if (item.TargetType == 1)
                        {
                            deviceIds.Add(item.TargetId);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }

            if (deviceIds.Count > 0)
            {
                //修改设备拥有者
                await BusUtility.Dispatch("BatchIotOrg", new
                {
                    Ids = deviceIds,
                    OwnerOrgId = 0
                });
            }

            MZ_Inventory newInven = new MZ_Inventory();
            newInven.Id = id;
            newInven.Status = 5;
            await _inventoryDAL.Update(newInven);

            return BusResponse<int>.Success();
        }
    }
}
