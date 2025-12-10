using AuthService;
using Common;
using Common.EventBus;
using Common.IdGenerator;
using Common.Share;
using StorageService.DAL;
using StorageService.Model;
using FlowService.DAL;
using FlowService.FlowNode.Builder;
using FlowService.Model;
using IoTService.Models;
using MyAccess.Aop;
using ProducerService.DAL;
using TemplateAction.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using StorageService.Controller;
using ProducerService.Model;
using AuthService.Fields;

namespace StorageService.Business
{
    public class StockBLL
    {
        private ITAServiceProvider _provider;
        private SnowflakeHelper _snowflake;
        private StockPileDAL _pileDAL;
        private LeaveStockDAL _leaveStockDAL;
        private LeaveDetailDAL _leaveDetailDAL;
        private StockRecordDAL _recordDAL;
        private EnterStockDAL _enterDAL;
        private EnterDetailDAL _enterDetailDAL;
        private StoreHouseDAL _houseDAL;
        public StockBLL(ITAServiceProvider provider, SnowflakeHelper snowflake, StockPileDAL pileDAL,
            LeaveStockDAL leaveStockDAL, LeaveDetailDAL leaveDetailDAL,
            StockRecordDAL recordDAL, EnterStockDAL enterDAL, EnterDetailDAL enterDetailDAL, StoreHouseDAL houseDAL)
        {
            _provider = provider;
            _snowflake = snowflake;
            _pileDAL = pileDAL;
            _leaveStockDAL = leaveStockDAL;
            _leaveDetailDAL = leaveDetailDAL;
            _recordDAL = recordDAL;
            _enterDAL = enterDAL;
            _enterDetailDAL = enterDetailDAL;
            _houseDAL = houseDAL;
        }
        public async Task WarnExecute()
        {
            //发送打醒
            var tlist = await _pileDAL.SelectList(x => x.IsTrigger == false && ((x.MaxNum != -1 && x.MaxNum < x.Quantity) || (x.MinNum != -1 && x.MinNum > x.Quantity)));
            var stopcardGroups = tlist.GroupBy(x => new { x.HouseId });
            foreach (var group in stopcardGroups)
            {
                try
                {
                    List<string> targetIds = new List<string>();
                    MZ_StockPile lastpile = null;
                    foreach (var item in group)
                    {
                        targetIds.Add(item.TargetId);
                        lastpile = item;
                    }
                    if (targetIds.Count == 0)
                    {
                        continue;
                    }
                    MZ_StockPile pile = new MZ_StockPile();
                    pile.IsTrigger = true;
                    await _pileDAL.Update(pile, x => x.HouseId == group.Key.HouseId && targetIds.Contains(x.TargetId));


                    var houseItem = await _houseDAL.Select(group.Key.HouseId);
                    if (houseItem == null)
                    {
                        continue;
                    }
                    if (houseItem.LeaderId == null || houseItem.LeaderId <= 0)
                    {
                        continue;
                    }
                    var tmpadmin = await _provider.GetService<UserDAL>().GetAdminById(houseItem.LeaderId.Value);
                    if (tmpadmin == null)
                    {
                        continue;
                    }
                    List<TargetUser> targets = new List<TargetUser>();
                    targets.Add(new TargetUser()
                    {
                        uid = tmpadmin.Id.Value,
                        email = tmpadmin.Email,
                        phone = tmpadmin.Mobile
                    });

                    var nt = new NoticeEvent(2, targets.ToArray(), new string[] { "APP" });
                    nt.OrgId = houseItem.OrgId.Value;
                    nt.TargetType = "StockExcept";
                    nt.TargetUrl = "/crm/house/warn";
                    nt.Content = $"仓库 {houseItem.StoreName} 存在{targetIds.Count}条库存预警，请尽快处理。";
                    nt.Label = "库存预警";
                    await TAEventDispatcher.Instance.Dispatch(NoticeEvent.EventKey, nt);
                }
                catch { }
            }
            //所有库存安全的已提醒库存设置为未提醒
            MZ_StockPile newpile = new MZ_StockPile();
            newpile.IsTrigger = false;
            await _pileDAL.Update(newpile, x => x.IsTrigger == true && ((x.MaxNum != -1 && x.MaxNum >= x.Quantity) || (x.MinNum != -1 && x.MinNum <= x.Quantity)));
        }
        public async Task<string> GenerateCKNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("CK");
        }
        public async Task<string> GenerateRKNumber()
        {
            GeneralRedisHelper tmpredis = _provider.GetService<GeneralRedisHelper>();
            return await tmpredis.GenerateNumber("RK");
        }
        public virtual async Task<Out_Item> SelectItemByKey(string key, IUserInfo user)
        {
            return await _pileDAL.SelectItemByKey(key, user.OrgId);
        }
        public virtual async Task<BusResponse<MZ_LeaveStock>> SelectStockBySource(string sourceEnterId)
        {
            var leaveList = await _leaveStockDAL.SelectList(x => x.SourceEnterId == sourceEnterId);
            if (leaveList.Count > 0)
            {
                return BusResponse<MZ_LeaveStock>.Success(leaveList[0]);
            }
            return BusResponse<MZ_LeaveStock>.Error(3, "出库单不存在");
        }
        public virtual async Task<PageObject<Out_StockPie>> SelectList(In_PileList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider, "/StorageService/House/List");
            return await _pileDAL.SelectByPage(query, user, scope);
        }
        public virtual async Task<BusResponse<string>> SetPileWarn(List<In_UpdatePileItem> data)
        {
            if (data == null || data.Count == 0)
            {
                return BusResponse<string>.Error(111, "请传入参数");
            }
            try
            {
                foreach (var item in data)
                {
                    MZ_StockPile pile = new MZ_StockPile();
                    pile.HouseId = item.HouseId;
                    pile.TargetType = item.TargetType;
                    pile.TargetId = item.TargetId;
                    pile.MinNum = item.MinNum;
                    pile.MaxNum = item.MaxNum;
                    await _pileDAL.Update(pile);
                }
                return BusResponse<string>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<string>.Error(112, ex.Message);
            }

        }
        public async Task<PageObject<MZ_StockRecord>> SelectStockRecordByPage(In_StockRecordPage query)
        {
            return await _recordDAL.SelectByPage(query);
        }

        public virtual async Task<PageObject<MZ_LeaveStock>> SelectLeaveList(In_LeaveList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider, "/StorageService/House/List");
            var allpage = await _leaveStockDAL.SelectLeaveByPage(query, user, scope);
            //初始化仓库名称
            if (allpage.List.Count > 0)
            {
                var tmpfromids = allpage.List.Select(x => x.FromHouseId).ToList();
                var tmptoids = allpage.List.Select(x => x.ToHouseId).ToList();
                tmpfromids.AddRange(tmptoids);

                var tmpids = tmpfromids.Distinct().ToList();
                var alldict = await _houseDAL.SelectDict(tmpids);
                foreach (var sellitem in allpage.List)
                {
                    MZ_StoreHouse tmpstr;
                    if (alldict.TryGetValue(sellitem.FromHouseId, out tmpstr))
                    {
                        sellitem.FromHouseName = tmpstr.StoreName;
                    }
                    if (alldict.TryGetValue(sellitem.ToHouseId, out tmpstr))
                    {
                        sellitem.ToHouseName = tmpstr.StoreName;
                    }
                }

            }

            if (query.ShowItems == true)
            {
                #region 出库详情初始化
                if (allpage.List.Count > 0)
                {
                    List<string> stockIds = new List<string>();
                    foreach (var stock in allpage.List)
                    {
                        stockIds.Add(stock.Id);
                    }
                    var tdetailList = await _leaveDetailDAL.SelectLeaveList(stockIds);
                    foreach (var item in tdetailList)
                    {
                        var stockitem = allpage.List.Where(x => x.Id == item.StockId).FirstOrDefault();
                        if (stockitem != null)
                        {
                            if (stockitem.List == null)
                            {
                                stockitem.List = new List<MZ_LeaveDetail>();
                            }
                            stockitem.List.Add(item);
                        }

                    }
                }
                #endregion
            }
            return allpage;
        }
        public virtual async Task<PageObject<MZ_EnterStock>> SelectEnterList(In_EnterList query)
        {
            var user = _provider.GetUser();
            var scope = await user.GetScope(_provider, "/StorageService/House/List");
            var allpage = await _enterDAL.SelectEnterByPage(query, user, scope);
            //初始化仓库名称
            if (allpage.List.Count > 0)
            {
                var tmpfromids = allpage.List.Select(x => x.FromHouseId).ToList();
                var tmptoids = allpage.List.Select(x => x.ToHouseId).ToList();
                tmpfromids.AddRange(tmptoids);
                var tmpids = tmpfromids.Distinct().ToList();
                if (tmpids.Count > 0)
                {
                    var alldict = await _houseDAL.SelectDict(tmpids);
                    foreach (var sellitem in allpage.List)
                    {
                        MZ_StoreHouse tmpstr;
                        if (alldict.TryGetValue(sellitem.FromHouseId, out tmpstr))
                        {
                            sellitem.FromHouseName = tmpstr.StoreName;
                        }
                        if (alldict.TryGetValue(sellitem.ToHouseId, out tmpstr))
                        {
                            sellitem.ToHouseName = tmpstr.StoreName;
                        }
                    }
                }


            }

            if (query.ShowItems == true)
            {
                #region 入库详情初始化
                if (allpage.List.Count > 0)
                {
                    List<string> stockIds = new List<string>();
                    foreach (var stock in allpage.List)
                    {
                        stockIds.Add(stock.Id);
                    }
                    var tdetailList = await _enterDetailDAL.SelectEnterList(stockIds);
                    foreach (var item in tdetailList)
                    {
                        var stockitem = allpage.List.Where(x => x.Id == item.StockId).FirstOrDefault();
                        if (stockitem != null)
                        {
                            if (stockitem.List == null)
                            {
                                stockitem.List = new List<MZ_EnterDetail>();
                            }
                            stockitem.List.Add(item);
                        }

                    }
                }
                #endregion
            }
            return allpage;
        }
        public virtual async Task<PageObject<V_ProductBatch>> SelectDevList(In_EnterDevList query)
        {
            var context = _provider.GetService<ITAContext>();
            var user = Data_ServerTokenInfo.From(context);
            var listpage = await _pileDAL.SelectNoPile(query, user);
            var supplierIds = listpage.List.Select(x => x.Supplier).ToList();
            if (supplierIds.Count > 0)
            {
                var supplierDAL = _provider.GetService<SupplierDAL>();
                var suplierlist = await supplierDAL.SelectList(x => supplierIds.Contains(x.Id));
                foreach (var item in listpage.List)
                {
                    var supitem = suplierlist.Where(x => x.Id == item.Supplier).FirstOrDefault();
                    if (supitem != null)
                    {
                        item.SupplierName = supitem.SupplierName;
                    }
                }
            }
            return listpage;
        }
        public virtual async Task<BusResponse<MZ_LeaveStock>> LeaveInfoByNumber(string number)
        {
            var stocklist = await _leaveStockDAL.SelectList(x => x.StockNumber == number);
            if (stocklist.Count == 0)
            {
                return BusResponse<MZ_LeaveStock>.Error(4, "出库单不存在");
            }
            return await QueryLeaveInfo(stocklist[0]);
        }
        public virtual async Task<BusResponse<MZ_LeaveStock>> LeaveInfo(string id)
        {
            MZ_LeaveStock stock = await _leaveStockDAL.Select(id);
            if (stock == null)
            {
                return BusResponse<MZ_LeaveStock>.Error(121, "出库单不存在");
            }

            return await QueryLeaveInfo(stock);
        }
        private async Task<BusResponse<MZ_LeaveStock>> QueryLeaveInfo(MZ_LeaveStock stock)
        {
            //初始化出库详情列表
            stock.List = await _leaveDetailDAL.SelectList(x => x.StockId == stock.Id);

            #region 初始化仓库名称
            var tmpids = new List<string>();
            tmpids.Add(stock.FromHouseId);
            tmpids.Add(stock.ToHouseId);
            var alldict = await _houseDAL.SelectDict(tmpids);
            MZ_StoreHouse tmpstr;
            if (alldict.TryGetValue(stock.FromHouseId, out tmpstr))
            {
                stock.FromHouseName = tmpstr.StoreName;
                stock.FromHouseLeaveTemplateId = tmpstr.LeaveTemplateId;
                stock.ToHouseEnterTemplateId = tmpstr.EnterTemplateId;
            }
            if (alldict.TryGetValue(stock.ToHouseId, out tmpstr))
            {
                stock.ToHouseName = tmpstr.StoreName;
            }
            if (stock.ToOrgId > 0)
            {
                MZ_Org toOrg = await _provider.GetService<AuthService.OrgDAL>().SelectById(stock.ToOrgId.Value);
                if (toOrg != null)
                {
                    stock.ToName = toOrg?.OrgName;
                }
            }
            if (!string.IsNullOrEmpty(stock.CustomerId))
            {
                var customer = await _provider.GetService<StorageActionDAL>().SelectCustomerById(stock.CustomerId);
                if (customer != null)
                {
                    stock.CustomerName = customer.CustomerName;
                }
            }
            #endregion

            #region 产品批次初始化
            var prolist = await _provider.GetService<ProductBatchDAL>().SelectListByIds(stock.List.Select(x => x.TargetId).ToList());
            foreach (var item in stock.List)
            {
                var tmppro = prolist.Where(x => x.Id == item.TargetId).FirstOrDefault();
                if (tmppro != null)
                {
                    item.TargetNumber = tmppro.Number;
                    item.TargetName = tmppro.BatchName;
                    item.PhotoUrl = tmppro.PhotoUrl;
                }
            }
            #endregion

            return BusResponse<MZ_LeaveStock>.Success(stock);
        }

        public virtual async Task<BusResponse<Dictionary<string, object>>> CreateEnterTaskForm(MZ_EnterStock stock)
        {
            if (string.IsNullOrEmpty(stock.ToHouseId))
            {
                return BusResponse<Dictionary<string, object>>.Error(109, "请输入入库仓库");
            }
            var houseInfo = await _provider.GetService<StoreHouseDAL>().Select(stock.ToHouseId);
            if (houseInfo == null)
            {
                return BusResponse<Dictionary<string, object>>.Error(110, "仓库不存在");
            }
            if (string.IsNullOrEmpty(houseInfo.EnterFlowInitJson))
            {
                return BusResponse<Dictionary<string, object>>.Success(new Dictionary<string, object>());
            }
            var flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EnterFlowItem>>(houseInfo.EnterFlowInitJson);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var fitem in flowitems)
            {
                dict.Add(fitem.id, fitem.GetRealValue(stock));
            }
            return BusResponse<Dictionary<string, object>>.Success(dict);
        }

        public virtual async Task<BusResponse<Dictionary<string, object>>> CreateLeaveTaskForm(MZ_LeaveStock stock)
        {
            if (string.IsNullOrEmpty(stock.FromHouseId))
            {
                return BusResponse<Dictionary<string, object>>.Error(109, "请输入出库仓库");
            }
            var houseInfo = await _provider.GetService<StoreHouseDAL>().Select(stock.FromHouseId);
            if (houseInfo == null)
            {
                return BusResponse<Dictionary<string, object>>.Error(110, "仓库不存在");
            }
            if (string.IsNullOrEmpty(houseInfo.LeaveFlowInitJson))
            {
                return BusResponse<Dictionary<string, object>>.Success(new Dictionary<string, object>());
            }
            MZ_AdminInfo useAdmin = null;
            if (stock.LeaveMethod == 3)
            {
                var leaveApply = await _provider.GetService<LeaveApplyDAL>().Select(stock.SourceEnterId);
                if (leaveApply != null)
                {
                    useAdmin = await _provider.GetService<UserDAL>().GetAdminById(leaveApply.ApplyUserId.Value);
                }
            }
            var flowitems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LeaveFlowItem>>(houseInfo.LeaveFlowInitJson);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var fitem in flowitems)
            {
                dict.Add(fitem.id, fitem.GetRealValue(stock, useAdmin));
            }
            return BusResponse<Dictionary<string, object>>.Success(dict);
        }

        public virtual async Task<BusResponse<MZ_EnterStock>> EnterInfoByNumber(string number)
        {
            var stocklist = await _enterDAL.SelectList(x => x.StockNumber == number);
            if (stocklist.Count == 0)
            {
                return BusResponse<MZ_EnterStock>.Error(4, "入库单不存在");
            }
            return await QueryEnterInfo(stocklist[0]);
        }
        public virtual async Task<BusResponse<MZ_EnterStock>> EnterInfo(string id)
        {
            MZ_EnterStock stock = await _enterDAL.Select(id);
            if (stock == null)
            {
                return BusResponse<MZ_EnterStock>.Error(121, "入库单不存在");
            }
            return await QueryEnterInfo(stock);
        }
        private async Task<BusResponse<MZ_EnterStock>> QueryEnterInfo(MZ_EnterStock stock)
        {
            if (stock.FromOrgId > 0)
            {
                var fromOrg = await _provider.GetService<AuthService.OrgDAL>().SelectById(stock.FromOrgId.Value);
                if (fromOrg != null)
                {
                    stock.FromName = fromOrg.OrgName;
                }
            }

            //初始化入库详情列表
            stock.List = await _enterDetailDAL.SelectList(x => x.StockId == stock.Id);

            #region 初始化仓库名称
            var tmpids = new List<string>();
            tmpids.Add(stock.FromHouseId);
            tmpids.Add(stock.ToHouseId);
            var alldict = await _houseDAL.SelectDict(tmpids);
            MZ_StoreHouse tmpstr;
            if (alldict.TryGetValue(stock.FromHouseId, out tmpstr))
            {
                stock.FromHouseName = tmpstr.StoreName;
            }
            if (alldict.TryGetValue(stock.ToHouseId, out tmpstr))
            {
                stock.ToHouseName = tmpstr.StoreName;
                stock.ToHouseEnterTemplateId = tmpstr.EnterTemplateId;
                stock.FromHouseLeaveTemplateId = tmpstr.LeaveTemplateId;
            }
            #endregion

            #region 产品批次初始化
            var prolist = await _provider.GetService<ProductBatchDAL>().SelectListByIds(stock.List.Select(x => x.TargetId).ToList());
            foreach (var item in stock.List)
            {
                var tmppro = prolist.Where(x => x.Id == item.TargetId).FirstOrDefault();
                if (tmppro != null)
                {
                    item.TargetNumber = tmppro.Number;
                    item.TargetName = tmppro.BatchName;
                    item.PhotoUrl = tmppro.PhotoUrl;
                }
            }
            #endregion

            return BusResponse<MZ_EnterStock>.Success(stock);
        }
        public virtual async Task<BusResponse<string>> AddLeave(MZ_LeaveStock data, IUserInfo user)
        {
            if (data.List == null || data.List.Count == 0)
            {
                return BusResponse<string>.Error(121, "出库单详情不能为空");
            }

            if (string.IsNullOrEmpty(data.StockNumber))
            {
                data.StockNumber = await GenerateCKNumber();
            }
            else
            {
                if (await _leaveStockDAL.Some(x => x.StockNumber == data.StockNumber))
                {
                    return BusResponse<string>.Error(117, "出库单编码已被使用");
                }
            }

            if (data.LeaveMethod == 0)
            {
                if (string.IsNullOrEmpty(data.CustomerId))
                {
                    if (data.ToOrgId <= 0)
                    {
                        return BusResponse<string>.Error(117, "目标客户未邀请，无法出库");
                    }

                    Tmp_CustomerInfo customer = null;
                    var storageOption = _provider.GetService<IOptions<StorageOption>>().Value;
                    if (storageOption.is_leave_customer)
                    {
                        var rsp = await BusUtility.Call("GetCustomerByOrg", new
                        {
                            from = data.OrgId.Value,
                            to = data.ToOrgId.Value
                        });
                        if (rsp.IsSuccess())
                        {
                            customer = rsp.GetResult<Tmp_CustomerInfo>();
                        }
                    }

                    if (customer != null)
                    {
                        //出库给客户
                        data.CustomerId = customer.Id;
                        data.CustomerType = customer.CustomerType;
                    }
                    else
                    {
                        data.CustomerId = string.Empty;
                        data.CustomerType = 0;
                    }
                }
                else
                {
                    var customer = await _provider.GetService<StorageActionDAL>().SelectCustomerById(data.CustomerId);
                    if (customer == null)
                    {
                        return BusResponse<string>.Error(131, "客户不存在");
                    }
                    //出库给客户
                    data.CustomerId = customer.Id;
                    data.CustomerType = customer.CustomerType;
                    data.ToOrgId = customer.BindOrgId;
                    if (data.ToOrgId <= 0)
                    {
                        return BusResponse<string>.Error(118, "目标客户未邀请，无法出库");
                    }
                }

                var targetOrg = await _provider.GetService<AuthService.OrgDAL>().SelectById(data.ToOrgId.Value);
                if (targetOrg == null || targetOrg.del_flag != "0")
                {
                    return BusResponse<string>.Error(128, "目标企业已被删除，无法出库");
                }

                //设置目标仓库
                var tohouseList = await _houseDAL.SelectList(x => x.OrgId == data.ToOrgId && x.IsSystem == 1);
                if (tohouseList.Count <= 0)
                {
                    return BusResponse<string>.Error(126, "对方无系统仓库，无法入库");
                }
                data.ToHouseId = tohouseList[0].Id;
                data.SourceEnterId = string.Empty;
            }
            else if (data.LeaveMethod == 1)
            {
                if (string.IsNullOrEmpty(data.SourceEnterId))
                {
                    return BusResponse<string>.Error(140, "关联入库单不能为空");
                }
                var enterStock = await _enterDAL.Select(data.SourceEnterId);
                if (enterStock == null)
                {
                    return BusResponse<string>.Error(141, "入库单不存在");
                }
                if (enterStock.Status != 2 && enterStock.Status != 3 && enterStock.Status != 4)
                {
                    return BusResponse<string>.Error(142, "当前状态无法退货");
                }
                if (user.OrgId != enterStock.OrgId)
                {
                    return BusResponse<string>.Error(143, "无撤销此单据的权限");
                }
                data.CustomerId = string.Empty;
                data.CustomerType = 0;
                if (string.IsNullOrEmpty(enterStock.FromHouseId))
                {
                    data.ToOrgId = 0;
                    data.ToHouseId = string.Empty;
                }
                else
                {
                    data.ToOrgId = enterStock.FromOrgId;
                    data.ToHouseId = enterStock.FromHouseId;
                }
                data.FromHouseId = enterStock.ToHouseId;

            }
            else if (data.LeaveMethod == 2)
            {
                if (string.IsNullOrEmpty(data.ToHouseId))
                {
                    return BusResponse<string>.Error(133, "请传入调拔的目标仓库");
                }
                data.CustomerId = string.Empty;
                data.CustomerType = 0;

                var tohouse = await _houseDAL.Select(data.ToHouseId);
                if (tohouse == null)
                {
                    return BusResponse<string>.Error(134, "目标仓库不存在");
                }
                data.ToOrgId = tohouse.OrgId;
                data.SourceEnterId = string.Empty;
            }
            else if (data.LeaveMethod == 3)
            {
                if (string.IsNullOrEmpty(data.SourceEnterId))
                {
                    return BusResponse<string>.Error(136, "请传入领用申请单的Id");
                }
                var oldApply = (await _provider.GetService<LeaveApplyDAL>().SelectList(x => x.Id == data.SourceEnterId)).FirstOrDefault();
                if (oldApply == null)
                {
                    return BusResponse<string>.Error(141, "申请单不存在");
                }
                if (oldApply.Status != 2 || oldApply.OutStatus != 0)
                {
                    return BusResponse<string>.Error(142, "申请单状态错误");
                }

                data.ToOrgId = 0;
                data.ToHouseId = string.Empty;
                data.CustomerId = string.Empty;
                data.CustomerType = 0;

            }
            else
            {
                return BusResponse<string>.Error(128, "出库方式错误");
            }

            data.Id = _snowflake.NextId().ToString();
            data.OrgId = user.OrgId;
            data.SetCreateBy(user);
            data.Status = 0;
            data.FlowId = 0;
            foreach (var item in data.List)
            {
                item.StockId = data.Id;
                if (item.TargetType == 1)
                {
                    item.Quantity = 1;
                }
            }
            using (BLLTranScope scope = new BLLTranScope())
            {
                await _leaveStockDAL.Insert(data);
                await _leaveDetailDAL.Insert(data.List);
                if (data.LeaveMethod == 3)
                {
                    MZ_LeaveApply apply = new MZ_LeaveApply();
                    apply.OutStatus = 1;
                    apply.Id = data.SourceEnterId;
                    await _provider.GetService<LeaveApplyDAL>().Update(apply);
                }

                // 完成
                await scope.CompleteAsync();
            }

            return BusResponse<string>.Success(data.Id);
        }

        public virtual async Task<BusResponse<string>> EditLeave(MZ_LeaveStock data)
        {
            MZ_LeaveStock old = await _leaveStockDAL.Select(data.Id);
            if (old == null)
            {
                return BusResponse<string>.Error(123, "出库单不存在");
            }
            var user = _provider.GetUser();
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<string>.Error(124, "当前用户无权限");
            }

            if (!string.IsNullOrEmpty(data.CustomerId))
            {
                var customer = await _provider.GetService<StorageActionDAL>().SelectCustomerById(data.CustomerId);
                if (customer == null)
                {
                    return BusResponse<string>.Error(122, "客户不存在");
                }
                if (customer.BindOrgId == 0)
                {
                    return BusResponse<string>.Error(123, "未邀请的客户");
                }
                data.ToOrgId = customer.BindOrgId;
                data.CustomerId = data.CustomerId;
                data.CustomerType = customer.CustomerType;
            }
            data.StockNumber = null;
            data.LeaveMethod = null;
            data.SetUpdateBy(user);
            data.OrgId = null;
            data.Status = null;
            foreach (var item in data.List)
            {
                item.StockId = data.Id;
                if (item.TargetType == 1)
                {
                    item.Quantity = 1;
                }
            }
            using (BLLTranScope scope = new BLLTranScope())
            {
                await _leaveStockDAL.Update(data);

                await _leaveDetailDAL.Delete(x => x.StockId == data.Id);
                await _leaveDetailDAL.Insert(data.List);
                // 完成
                await scope.CompleteAsync();
            }

            return BusResponse<string>.Success(data.Id);
        }


        private async Task<BusResponse<int>> _AllStockOut(MZ_LeaveStock leave, IUserInfo user, bool inLock = false, bool newLeave = false, OutExtInfo info = null, MZ_StoreHouse fromHouse = null, MZ_LeaveApply leaveApply = null)
        {
            if (fromHouse == null)
            {
                fromHouse = await _houseDAL.Select(leave.FromHouseId);
            }
            int rstatus = (fromHouse != null && fromHouse.LeaveTemplateId > 0 && leave.FlowId > 0) ? 1 : 2;
            if (newLeave)
            {
                leave.SetCreateBy(user);
                leave.Status = rstatus;
                //生成出库单
                await _leaveStockDAL.Insert(leave);
                if (leave.List.Count > 0)
                {
                    //生成出库单详情
                    await _leaveDetailDAL.Insert(leave.List);
                }

            }
            else
            {
                MZ_LeaveStock data = new MZ_LeaveStock();
                data.Id = leave.Id;
                data.FlowId = leave.FlowId;
                data.SetUpdateBy(user);
                data.Status = rstatus;
                await _leaveStockDAL.Update(data);
            }

            MZ_EnterStock enterStock = null;
            List<string> deviceIds = new List<string>();
            if (leave.ToOrgId > 0)
            {
                enterStock = new MZ_EnterStock();
                enterStock.Id = _snowflake.NextId().ToString();
                enterStock.OrgId = leave.ToOrgId;
                enterStock.FromOrgId = leave.OrgId;
                enterStock.StockNumber = await GenerateRKNumber();
                enterStock.FromHouseId = leave.FromHouseId;
                enterStock.ToHouseId = leave.ToHouseId;
                enterStock.EnterMethod = leave.LeaveMethod;
                enterStock.ExpressNumber = leave.ExpressNumber;
                enterStock.ExpressCompany = leave.ExpressCompany;
                enterStock.ExpressPhone = leave.ExpressPhone;
                enterStock.InDate = leave.OutDate;
                enterStock.Remark = leave.Remark;
                enterStock.FlowId = 0;
                enterStock.SetCreateBy(user);

                if (info != null)
                {
                    info.enterId = enterStock.Id;
                }
                MZ_StoreHouse toHouse = null;
                if (!string.IsNullOrEmpty(leave.ToHouseId))
                {
                    toHouse = await _houseDAL.Select(leave.ToHouseId);
                }
                if (toHouse != null && toHouse.EnterTemplateId > 0)
                {
                    enterStock.Status = 0;
                }
                else
                {
                    enterStock.Status = 2;
                }

                enterStock.List = new List<MZ_EnterDetail>();
                foreach (var outItem in leave.List)
                {
                    MZ_EnterDetail detail = new MZ_EnterDetail();
                    detail.StockId = enterStock.Id;
                    detail.TargetType = outItem.TargetType;
                    detail.TargetId = outItem.TargetId;
                    detail.Quantity = outItem.Quantity;
                    detail.Price = outItem.Price;
                    enterStock.List.Add(detail);
                    if (outItem.TargetType == 1)
                    {
                        deviceIds.Add(outItem.TargetId);
                    }
                }
            }
            else
            {
                foreach (var outItem in leave.List)
                {
                    if (outItem.TargetType == 1)
                    {
                        deviceIds.Add(outItem.TargetId);
                    }
                }
            }


            //入库锁定中时，直接出库
            if (rstatus == 1)
            {
                //锁库存
                foreach (var t in leave.List)
                {
                    if (await _pileDAL.LockPile(leave.FromHouseId, t.TargetType.Value, t.TargetId, t.Quantity.Value) == 0)
                    {
                        throw new Exception("库存不足");
                    }
                }
            }
            else
            {
                foreach (var t in leave.List)
                {

                    //添加出库库存记录
                    await _recordDAL.InsertRecord(leave.OrgId.Value, leave.FromHouseId, t.TargetType.Value, t.TargetId, 0, leave.Id, t.Quantity.Value, t.Price.Value);
                    if (inLock)
                    {
                        //减锁库存
                        if (await _pileDAL.ReduceLock(leave.FromHouseId, t.TargetType.Value, t.TargetId, t.Quantity.Value, t.Price) == 0)
                        {
                            throw new Exception("锁库存不足");
                        }
                    }
                    else
                    {
                        //减库存
                        if (await _pileDAL.ReducePile(leave.FromHouseId, t.TargetType.Value, t.TargetId, t.Quantity.Value, t.Price) == 0)
                        {
                            throw new Exception("库存不足");
                        }
                    }

                }
                if (leave.ToOrgId > 0)
                {
                    //生成入库单
                    await _enterDAL.Insert(enterStock);
                    //生成入库单详情
                    await _enterDetailDAL.Insert(enterStock.List);

                    foreach (var item in enterStock.List)
                    {
                        //生成入库记录
                        await _recordDAL.InsertRecord(enterStock.OrgId.Value, enterStock.ToHouseId, item.TargetType.Value, item.TargetId, 1, enterStock.Id, item.Quantity.Value, item.Price.Value);
                        //增库存
                        MZ_StockPile pile = new MZ_StockPile();
                        pile.OrgId = enterStock.OrgId.Value;
                        pile.HouseId = enterStock.ToHouseId;
                        pile.TargetType = item.TargetType.Value;
                        pile.TargetId = item.TargetId;
                        pile.Price = item.Price.Value;
                        if (enterStock.Status == 2)
                        {
                            pile.LockQuantity = 0;
                            pile.Quantity = item.Quantity.Value;
                            await _pileDAL.IncreasePile(pile);
                        }
                        else
                        {
                            pile.LockQuantity = item.Quantity.Value;
                            pile.Quantity = 0;
                            await _pileDAL.IncreaseLock(pile);
                        }


                    }

                }


                if (leave.LeaveMethod == 3)
                {
                    if (deviceIds.Count > 0)
                    {
                        //更改设备使用者
                        await BusUtility.Dispatch("BatchIotOrg", new
                        {
                            Ids = deviceIds,
                            UseOrgId = leave.OrgId.Value,
                        });
                    }


                    if (leaveApply == null)
                    {
                        leaveApply = await _provider.GetService<LeaveApplyDAL>().Select(leave.SourceEnterId);
                    }
                    if (leaveApply != null)
                    {
                        if (deviceIds.Count > 0)
                        {
                            try
                            {
                                await BusUtility.Dispatch("LeaveApply", new
                                {
                                    OrgId = leaveApply.OrgId,
                                    LeaderId = leaveApply.ApplyUserId,
                                    DevIds = string.Join(',', deviceIds)
                                });
                            }
                            catch { }
                        }



                        MZ_LeaveApply apply = new MZ_LeaveApply();
                        apply.OutStatus = 2;
                        apply.Id = leave.SourceEnterId;
                        await _provider.GetService<LeaveApplyDAL>().Update(apply);
                    }

                }

                if (deviceIds.Count > 0 && leave.ToOrgId != leave.OrgId)
                {
                    if (leave.CustomerType == 0)
                    {
                        //更改设备拥有者
                        await BusUtility.Dispatch("BatchIotOrg", new
                        {
                            Ids = deviceIds,
                            OwnerOrgId = leave.ToOrgId.Value,
                            ClearOwnerOrgId = leave.LeaveMethod == 1 ? leave.OrgId.Value : 0
                        });
                    }
                    else if (leave.CustomerType == 1)
                    {
                        //更改设备使用者
                        await BusUtility.Dispatch("BatchIotOrg", new
                        {
                            Ids = deviceIds,
                            UseOrgId = leave.ToOrgId.Value,
                        });

                    }


                    try
                    {
                        await BusUtility.Dispatch("StockLeave", new
                        {
                            OrgId = leave.OrgId,
                            TargetOrgId = leave.ToOrgId,
                            DevIds = string.Join(',', deviceIds)
                        });
                    }
                    catch { }

                }
            }


            return BusResponse<int>.Success(rstatus);
        }
        public virtual async Task<BusResponse<int>> SubmitModel(IUserInfo user, In_SubmitStock data)
        {
            MZ_LeaveStock old = await _leaveStockDAL.Select(data.id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "出库单不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }
            if (old.Status == 2)
            {
                return BusResponse<int>.Error(125, "单据已出库");
            }
            if (old.Status != 0)
            {
                return BusResponse<int>.Error(127, "状态错误");
            }
            MZ_LeaveApply leaveApply = null;
            MZ_AdminInfo useAdmin = null;
            MZ_EnterStock enterStock = null;
            if (old.LeaveMethod == 0)
            {
                var targetOrg = await _provider.GetService<AuthService.OrgDAL>().SelectById(old.ToOrgId.Value);
                if (targetOrg == null || targetOrg.del_flag != "0")
                {
                    return BusResponse<int>.Error(128, "目标企业已被删除，无法出库");
                }
            }
            else if (old.LeaveMethod == 1)
            {
                enterStock = await _enterDAL.Select(old.SourceEnterId);
                if (enterStock == null)
                {
                    return BusResponse<int>.Error(141, "入库单不存在");
                }
            }
            else if (old.LeaveMethod == 3)
            {
                old.ToOrgId = 0;
                leaveApply = await _provider.GetService<LeaveApplyDAL>().Select(old.SourceEnterId);
                if (leaveApply != null)
                {
                    useAdmin = await _provider.GetService<UserDAL>().GetAdminById(leaveApply.ApplyUserId.Value);
                }
            }


            old.List = await _leaveDetailDAL.SelectList(x => x.StockId == data.id);
            var fromHouse = await _provider.GetService<StoreHouseDAL>().Select(old.FromHouseId);
            if (fromHouse.LeaveTemplateId != null && fromHouse.LeaveTemplateId > 0)
            {
                List<LeaveFlowItem> flowItems;
                if (string.IsNullOrEmpty(fromHouse.LeaveFlowInitJson))
                {
                    flowItems = new List<LeaveFlowItem>();
                }
                else
                {
                    flowItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LeaveFlowItem>>(fromHouse.LeaveFlowInitJson);
                }
                LeaveFlowCreate flowcreate = new LeaveFlowCreate();
                flowcreate.templateId = fromHouse.LeaveTemplateId.Value;
                flowcreate.model = data.model;
                flowcreate.assign = data.assign;
                if (!data.model.ContainsKey("@from"))
                {
                    flowcreate.model.Add("@from", old.StockNumber);
                }
                if (!data.model.ContainsKey("@fromtype"))
                {
                    flowcreate.model.Add("@fromtype", "出库单");
                }
                if (!data.model.ContainsKey("@FlowNumber"))
                {
                    flowcreate.model.Add("@FlowNumber", old.StockNumber);
                }

                flowcreate.UserId = user.UserId;
                flowcreate.flowId = data.flowId;
                foreach (var fitem in flowItems)
                {
                    if (!flowcreate.model.ContainsKey(fitem.id))
                    {
                        flowcreate.model.Add(fitem.id, fitem.GetRealValue(old, useAdmin));
                    }
                }
                var fcrsp = await BusUtility.Call("NewFlowTask", flowcreate);
                if (!fcrsp.IsSuccess())
                {
                    return BusResponse<int>.Error(144, fcrsp.Message);
                }
                old.FlowId = fcrsp.GetResult<long>();
            }
            using (BLLTranScope scope = new BLLTranScope())
            {
                try
                {
                    if (old.LeaveMethod == 1)
                    {
                        MZ_EnterStock newEnter = new MZ_EnterStock();
                        newEnter.Id = old.SourceEnterId;
                        newEnter.Status = 4;
                        newEnter.SetUpdateBy(user);
                        var rs = await _enterDAL.Update(newEnter, x => x.Id == enterStock.Id && x.Status == enterStock.Status);
                        if (rs <= 0)
                        {
                            return BusResponse<int>.Error(142, "入库单状态已变更，请重新发起退货");
                        }
                    }

                    var rsp = await _AllStockOut(old, user, false, false, null, fromHouse, leaveApply);
                    if (!rsp.IsSuccess())
                    {
                        await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
                        return BusResponse<int>.Error(143, rsp.Message);
                    }

                    await scope.CompleteAsync();
                    return rsp;
                }
                catch (Exception ex)
                {
                    if (old.FlowId > 0)
                    {
                        await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(old.FlowId.Value);
                    }
                    return BusResponse<int>.Error(322, ex.Message);
                }

            }
        }
        public virtual async Task<BusResponse<int>> Submit(IUserInfo user, string id, long flowId)
        {
            MZ_LeaveStock old = await _leaveStockDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(123, "出库单不存在");
            }
            if (user.OrgId != old.OrgId)
            {
                return BusResponse<int>.Error(124, "当前用户无权限");
            }
            if (old.Status == 2)
            {
                if (flowId > 0)
                {
                    MZ_LeaveStock newUpdateStock = new MZ_LeaveStock();
                    newUpdateStock.Id = id;
                    newUpdateStock.FlowId = flowId;
                    newUpdateStock.SetUpdateBy(user);
                    await _leaveStockDAL.Update(newUpdateStock);
                }
                return BusResponse<int>.Success(2);
            }
            if (old.Status != 0)
            {
                return BusResponse<int>.Error(127, "状态错误");
            }
            if (old.LeaveMethod == 0)
            {
                var targetOrg = await _provider.GetService<AuthService.OrgDAL>().SelectById(old.ToOrgId.Value);
                if (targetOrg == null || targetOrg.del_flag != "0")
                {
                    return BusResponse<int>.Error(128, "目标企业已被删除，无法出库");
                }
            }


            old.FlowId = flowId;
            old.List = await _leaveDetailDAL.SelectList(x => x.StockId == id);


            using (BLLTranScope scope = new BLLTranScope())
            {
                try
                {
                    if (old.LeaveMethod == 1)
                    {
                        var enterStock = await _enterDAL.Select(old.SourceEnterId);
                        if (enterStock == null)
                        {
                            return BusResponse<int>.Error(141, "入库单不存在");
                        }
                        MZ_EnterStock newEnter = new MZ_EnterStock();
                        newEnter.Id = old.SourceEnterId;
                        newEnter.Status = 4;
                        newEnter.SetUpdateBy(user);
                        var rs = await _enterDAL.Update(newEnter, x => x.Id == enterStock.Id && x.Status == enterStock.Status);
                        if (rs <= 0)
                        {
                            return BusResponse<int>.Error(142, "入库单状态已变更，请重新发起退货");
                        }
                    }
                    else if (old.LeaveMethod == 3)
                    {
                        old.ToOrgId = 0;
                    }
                    var rsp = await _AllStockOut(old, user);
                    if (rsp.IsSuccess())
                    {
                        await scope.CompleteAsync();
                    }
                    return rsp;
                }
                catch (Exception ex)
                {
                    return BusResponse<int>.Error(322, ex.Message);
                }

            }
        }
        public virtual async Task<BusResponse<int>> CancelLeave(string id)
        {
            var user = _provider.GetUser();

            var leaveStock = await _leaveStockDAL.Select(id);
            if (leaveStock == null)
            {
                return BusResponse<int>.Error(131, "出库单不存在");
            }
            if (user.OrgId != leaveStock.OrgId)
            {
                return BusResponse<int>.Error(132, "无撤销此单据的权限");
            }

            if (leaveStock.Status == 0)
            {
                MZ_LeaveStock newLeave = new MZ_LeaveStock();
                newLeave.Id = leaveStock.Id;
                newLeave.Status = 4;
                newLeave.SetUpdateBy(user);
                int rs = await _leaveStockDAL.Update(newLeave, x => x.Id == leaveStock.Id && x.Status == leaveStock.Status);
                if (rs > 0)
                {
                    //撤销流程
                    In_FlowQuery query = new In_FlowQuery();
                    query.Name = "@from";
                    query.Value = leaveStock.StockNumber;
                    var querylist = await _provider.GetService<FlowQueryDAL>().SelectQueryList(query);
                    foreach (var quitem in querylist)
                    {
                        try
                        {
                            await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(quitem.FlowId.Value);
                        }
                        catch { }
                    }
                    return BusResponse<int>.Success();
                }
                if (leaveStock.LeaveMethod == 3)
                {
                    MZ_LeaveApply apply = new MZ_LeaveApply();
                    apply.OutStatus = 0;
                    apply.Id = leaveStock.SourceEnterId;
                    await _provider.GetService<LeaveApplyDAL>().Update(apply);
                }
            }
            else if (leaveStock.Status == 1)
            {
                using (BLLTranScope scope = new BLLTranScope())
                {
                    MZ_LeaveStock newLeave = new MZ_LeaveStock();
                    newLeave.Id = leaveStock.Id;
                    newLeave.Status = 4;
                    newLeave.SetUpdateBy(user);
                    int rs = await _leaveStockDAL.Update(newLeave, x => x.Id == leaveStock.Id && x.Status == leaveStock.Status);
                    var stockList = await _leaveDetailDAL.SelectList(x => x.StockId == leaveStock.Id);
                    leaveStock.List = new List<MZ_LeaveDetail>();
                    foreach (var item in stockList)
                    {
                        await _pileDAL.LockBack(leaveStock.FromHouseId, item.TargetType.Value, item.TargetId, item.Quantity.Value);
                    }

                    if (leaveStock.LeaveMethod == 3)
                    {
                        MZ_LeaveApply apply = new MZ_LeaveApply();
                        apply.OutStatus = 0;
                        apply.Id = leaveStock.SourceEnterId;
                        await _provider.GetService<LeaveApplyDAL>().Update(apply);
                    }

                    // 完成
                    await scope.CompleteAsync();
                }

                //撤销流程
                if (leaveStock.FlowId.Value > 0)
                {
                    try
                    {
                        await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(leaveStock.FlowId.Value);
                    }
                    catch { }
                }
                return BusResponse<int>.Success();
            }
            else
            {
                return BusResponse<int>.Error(133, "当前状态无法撤销");
            }

            return BusResponse<int>.Error(141, "撤销失败,请重试");
        }

        public virtual async Task<BusResponse<int>> CancelEnterModel(In_StockCancelModel data)
        {
            var user = _provider.GetUser();

            var enterStock = await _enterDAL.Select(data.Id);
            if (enterStock == null)
            {
                return BusResponse<int>.Error(131, "入库单不存在");
            }
            if (user.OrgId != enterStock.OrgId)
            {
                return BusResponse<int>.Error(132, "无撤销此单据的权限");
            }
            if (enterStock.EnterMethod == 1)
            {
                return BusResponse<int>.Error(133, "无法撤销退货单");
            }
            if (enterStock.Status == 0 || enterStock.Status == 1)
            {
                //撤销待审核状态的入库单
                MZ_EnterStock newEnter = new MZ_EnterStock();
                newEnter.Id = data.Id;
                if (enterStock.Status == 0 && enterStock.EnterMethod == 3)
                {
                    newEnter.Status = 4;
                }
                else
                {
                    newEnter.Status = 3;
                }
                newEnter.SetUpdateBy(user);
                int rs = await _enterDAL.Update(newEnter, x => x.Id == data.Id && x.Status == enterStock.Status);
                if (rs > 0)
                {
                    In_FlowQuery query = new In_FlowQuery();
                    query.Name = "@from";
                    query.Value = enterStock.StockNumber;
                    var querylist = await _provider.GetService<FlowQueryDAL>().SelectQueryList(query);
                    foreach (var quitem in querylist)
                    {
                        try
                        {
                            await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(quitem.FlowId.Value);
                        }
                        catch { }
                    }
                    return BusResponse<int>.Success();
                }
            }
            else if (enterStock.Status == 2 || enterStock.Status == 3)
            {
                MZ_LeaveStock leaveStock = new MZ_LeaveStock();
                leaveStock.Id = _snowflake.NextId().ToString();
                leaveStock.SourceEnterId = enterStock.Id;
                leaveStock.OrgId = enterStock.OrgId;
                if (enterStock.EnterMethod == 3)
                {
                    leaveStock.ToOrgId = 0;
                    leaveStock.ToHouseId = string.Empty;
                }
                else
                {
                    leaveStock.ToOrgId = enterStock.FromOrgId;
                    leaveStock.ToHouseId = enterStock.FromHouseId;
                }

                leaveStock.CustomerId = string.Empty;
                leaveStock.CustomerType = 0;
                leaveStock.StockNumber = data.StockNumber;
                leaveStock.FromHouseId = enterStock.ToHouseId;
                leaveStock.LeaveMethod = 1;
                leaveStock.ExpressNumber = data.ExpressNumber;
                leaveStock.ExpressCompany = data.ExpressCompany;
                leaveStock.ExpressPhone = data.ExpressPhone;
                leaveStock.OutDate = data.OutDate;
                leaveStock.Remark = data.Remark;

                var stockList = await _enterDetailDAL.SelectList(x => x.StockId == data.Id);
                leaveStock.List = new List<MZ_LeaveDetail>();
                foreach (var item in stockList)
                {
                    MZ_LeaveDetail detailItem = new MZ_LeaveDetail();
                    detailItem.StockId = leaveStock.Id;
                    detailItem.Quantity = item.Quantity;
                    detailItem.TargetId = item.TargetId;
                    detailItem.TargetType = item.TargetType;
                    detailItem.Price = item.Price;
                    leaveStock.List.Add(detailItem);
                }

                var fromHouse = await _provider.GetService<StoreHouseDAL>().Select(leaveStock.FromHouseId);
                if (fromHouse.LeaveTemplateId != null && fromHouse.LeaveTemplateId > 0)
                {
                    List<LeaveFlowItem> flowItems;
                    if (string.IsNullOrEmpty(fromHouse.LeaveFlowInitJson))
                    {
                        flowItems = new List<LeaveFlowItem>();
                    }
                    else
                    {
                        flowItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<LeaveFlowItem>>(fromHouse.LeaveFlowInitJson);
                    }
                    LeaveFlowCreate flowcreate = new LeaveFlowCreate();
                    flowcreate.templateId = fromHouse.LeaveTemplateId.Value;
                    flowcreate.model = data.model;
                    flowcreate.assign = data.assign;
                    if (!data.model.ContainsKey("@from"))
                    {
                        flowcreate.model.Add("@from", leaveStock.StockNumber);
                    }
                    if (!data.model.ContainsKey("@fromtype"))
                    {
                        flowcreate.model.Add("@fromtype", "出库单");
                    }
                    if (!data.model.ContainsKey("@FlowNumber"))
                    {
                        flowcreate.model.Add("@FlowNumber", leaveStock.StockNumber);
                    }

                    flowcreate.UserId = user.UserId;
                    flowcreate.flowId = data.flowId;
                    foreach (var fitem in flowItems)
                    {
                        if (!flowcreate.model.ContainsKey(fitem.id))
                        {
                            flowcreate.model.Add(fitem.id, fitem.GetRealValue(leaveStock, null));
                        }
                    }
                    var fcrsp = await BusUtility.Call("NewFlowTask", flowcreate);
                    if (!fcrsp.IsSuccess())
                    {
                        return BusResponse<int>.Error(144, fcrsp.Message);
                    }
                    leaveStock.FlowId = fcrsp.GetResult<long>();
                }
                else
                {
                    leaveStock.FlowId = 0;
                }

                using (BLLTranScope scope = new BLLTranScope())
                {

                    MZ_EnterStock newEnter = new MZ_EnterStock();
                    newEnter.Id = data.Id;
                    newEnter.Status = 4;
                    newEnter.SetUpdateBy(user);
                    await _enterDAL.Update(newEnter, x => x.Id == data.Id && x.Status == enterStock.Status);

                    BusResponse<int> rsp = null;
                    try
                    {
                        rsp = await _AllStockOut(leaveStock, user, enterStock.Status == 3, true, null, fromHouse);
                        if (!rsp.IsSuccess())
                        {
                            await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(leaveStock.FlowId.Value);
                            return BusResponse<int>.Error(145, rsp.Message);
                        }
                        await scope.CompleteAsync();
                        return rsp;
                    }
                    catch (Exception ex)
                    {
                        if (leaveStock.FlowId > 0)
                        {
                            await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(leaveStock.FlowId.Value);
                        }
                        return BusResponse<int>.Error(322, ex.Message);
                    }
                }
            }
            return BusResponse<int>.Error(141, "撤销失败,请重试");
        }
        public virtual async Task<BusResponse<int>> CancelEnter(In_StockCancel data)
        {
            var user = _provider.GetUser();

            var enterStock = await _enterDAL.Select(data.Id);
            if (enterStock == null)
            {
                return BusResponse<int>.Error(131, "入库单不存在");
            }
            if (user.OrgId != enterStock.OrgId)
            {
                return BusResponse<int>.Error(132, "无撤销此单据的权限");
            }
            if (enterStock.EnterMethod == 1)
            {
                return BusResponse<int>.Error(133, "无法撤销退货单");
            }
            if (enterStock.Status == 0 || enterStock.Status == 1)
            {
                //撤销待审核状态的入库单
                MZ_EnterStock newEnter = new MZ_EnterStock();
                newEnter.Id = data.Id;
                if (enterStock.Status == 0 && enterStock.EnterMethod == 3)
                {
                    newEnter.Status = 4;
                }
                else
                {
                    newEnter.Status = 3;
                }
                newEnter.SetUpdateBy(user);
                int rs = await _enterDAL.Update(newEnter, x => x.Id == data.Id && x.Status == enterStock.Status);
                if (rs > 0)
                {
                    In_FlowQuery query = new In_FlowQuery();
                    query.Name = "@from";
                    query.Value = enterStock.StockNumber;
                    var querylist = await _provider.GetService<FlowQueryDAL>().SelectQueryList(query);
                    foreach (var quitem in querylist)
                    {
                        try
                        {
                            await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(quitem.FlowId.Value);
                        }
                        catch { }
                    }
                    return BusResponse<int>.Success();
                }
            }
            else if (enterStock.Status == 2 || enterStock.Status == 3)
            {
                using (BLLTranScope scope = new BLLTranScope())
                {

                    MZ_EnterStock newEnter = new MZ_EnterStock();
                    newEnter.Id = data.Id;
                    newEnter.Status = 4;
                    newEnter.SetUpdateBy(user);
                    int rs = await _enterDAL.Update(newEnter, x => x.Id == data.Id && x.Status == enterStock.Status);
                    if (rs > 0)
                    {
                        MZ_LeaveStock leaveStock = new MZ_LeaveStock();
                        leaveStock.Id = _snowflake.NextId().ToString();
                        leaveStock.SourceEnterId = enterStock.Id;
                        leaveStock.OrgId = enterStock.OrgId;
                        if (enterStock.EnterMethod == 3)
                        {
                            leaveStock.ToOrgId = 0;
                            leaveStock.ToHouseId = string.Empty;
                        }
                        else
                        {
                            leaveStock.ToOrgId = enterStock.FromOrgId;
                            leaveStock.ToHouseId = enterStock.FromHouseId;
                        }

                        leaveStock.CustomerId = string.Empty;
                        leaveStock.CustomerType = 0;
                        leaveStock.StockNumber = data.StockNumber;
                        leaveStock.FromHouseId = enterStock.ToHouseId;
                        leaveStock.LeaveMethod = 1;
                        leaveStock.ExpressNumber = data.ExpressNumber;
                        leaveStock.ExpressCompany = data.ExpressCompany;
                        leaveStock.ExpressPhone = data.ExpressPhone;
                        leaveStock.OutDate = data.OutDate;
                        leaveStock.Remark = data.Remark;
                        leaveStock.FlowId = data.FlowId == null ? 0 : data.FlowId;

                        var stockList = await _enterDetailDAL.SelectList(x => x.StockId == data.Id);
                        leaveStock.List = new List<MZ_LeaveDetail>();
                        foreach (var item in stockList)
                        {
                            MZ_LeaveDetail detailItem = new MZ_LeaveDetail();
                            detailItem.StockId = leaveStock.Id;
                            detailItem.Quantity = item.Quantity;
                            detailItem.TargetId = item.TargetId;
                            detailItem.TargetType = item.TargetType;
                            detailItem.Price = item.Price;
                            leaveStock.List.Add(detailItem);
                        }

                        BusResponse<int> rsp = null;
                        try
                        {
                            rsp = await _AllStockOut(leaveStock, user, enterStock.Status == 3, true);
                            if (rsp.IsSuccess())
                            {
                                await scope.CompleteAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            rsp = BusResponse<int>.Error(322, ex.Message);
                        }
                        if (rsp.IsSuccess() == false && data.FlowId != null && data.FlowId > 0)
                        {
                            try
                            {
                                await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(data.FlowId.Value);
                            }
                            catch { }
                        }

                        return rsp;

                    }
                }
            }
            return BusResponse<int>.Error(141, "撤销失败,请重试");
        }
        public virtual async Task<BusResponse<string>> ManualPile(In_ManualStock ipt, IUserInfo user)
        {
            if (ipt.List == null || ipt.List.Count == 0)
            {
                return BusResponse<string>.Error(141, "请选择要入库的设备或耗材");
            }
            if (string.IsNullOrEmpty(ipt.StockNumber))
            {
                return BusResponse<string>.Error(147, "入库单号不能为空");
            }
            MZ_EnterStock enterStock;
            if (ipt.Id == null)
            {
                enterStock = new MZ_EnterStock();
                enterStock.EnterMethod = 3;
                enterStock.Id = _snowflake.NextId().ToString();
                enterStock.OrgId = user.OrgId;
                enterStock.FromOrgId = user.OrgId;
                enterStock.StockNumber = ipt.StockNumber;
                enterStock.SetCreateBy(user);
                enterStock.Status = 0;
                enterStock.FlowId = 0;
                if (string.IsNullOrEmpty(enterStock.StockNumber))
                {
                    enterStock.StockNumber = await GenerateRKNumber();
                }
                else
                {
                    if (await _enterDAL.Some(x => x.StockNumber == ipt.StockNumber))
                    {
                        return BusResponse<string>.Error(117, "入库单编码已被使用");
                    }
                }
            }
            else
            {
                enterStock = await _enterDAL.Select(ipt.Id);
                if (enterStock == null)
                {
                    return BusResponse<string>.Error(143, "入库单不存在");
                }
                if (enterStock.OrgId != user.OrgId)
                {
                    return BusResponse<string>.Error(144, "无权操作这条入库单");
                }
                if (enterStock.Status > 0)
                {
                    return BusResponse<string>.Error(145, "入库单已提交，无法修改");
                }
                enterStock.SetUpdateBy(user);
            }

            enterStock.FromHouseId = string.Empty;
            enterStock.ToHouseId = ipt.HouseId;
            enterStock.InDate = ipt.InDate;
            enterStock.Remark = ipt.Remark;
            enterStock.ExpressNumber = ipt.ExpressNumber ?? string.Empty;
            enterStock.ExpressCompany = ipt.ExpressCompany ?? string.Empty;
            enterStock.ExpressPhone = ipt.ExpressPhone ?? string.Empty;

            var batchDAL = _provider.GetService<ProductBatchDAL>();
            foreach (var detail in ipt.List)
            {
                detail.StockId = enterStock.Id;
                var batchInfo = await batchDAL.SelectProductVById(detail.TargetId);
                if (batchInfo == null)
                {
                    return BusResponse<string>.Error(142, string.Format("目标批次不存在【{0}】", detail.TargetId));
                }

                detail.TargetType = batchInfo.ProductLabel == "F" ? 1 : 0;
                if (batchInfo.ProductLabel == "F")
                {
                    detail.Quantity = 1;
                    if (await _pileDAL.Some(x => x.TargetId == detail.TargetId && x.Quantity > 0))
                    {
                        return BusResponse<string>.Error(142, string.Format("批次【{0}】无法多次入库", batchInfo.Number));
                    }
                }

            }


            using (BLLTranScope scope = new BLLTranScope())
            {
                if (ipt.Id == null)
                {
                    //生成入库单
                    await _enterDAL.Insert(enterStock);
                    //生成入库单详情
                    await _enterDetailDAL.Insert(ipt.List);

                }
                else
                {
                    await _enterDAL.Update(enterStock);
                    await _enterDetailDAL.Delete(x => x.StockId == ipt.Id);
                    await _enterDetailDAL.Insert(ipt.List);
                }

                // 完成
                await scope.CompleteAsync();
            }
            return BusResponse<string>.Success(enterStock.Id);
        }

        public virtual async Task<BusResponse<int>> SubmitManualPileModel(IUserInfo user, In_SubmitManualPile data)
        {
            var old = await _enterDAL.Select(data.id);
            if (old == null)
            {
                return BusResponse<int>.Error(143, "入库单不存在");
            }
            if (old.Status == 2)
            {
                return BusResponse<int>.Error(144, "单据已入库");
            }
            if (old.Status != 0)
            {
                return BusResponse<int>.Error(127, "状态错误");
            }

            old.List = await _enterDetailDAL.SelectList(x => x.StockId == data.id);

            MZ_EnterStock enterStock = new MZ_EnterStock();
            enterStock.Id = data.id;
            var toHouse = await _houseDAL.Select(old.ToHouseId);
            if (toHouse != null && toHouse.EnterTemplateId > 0)
            {
                enterStock.Status = 1;
            }
            else
            {
                enterStock.Status = 2;
            }


            enterStock.SetUpdateBy(user);


            if (toHouse.EnterTemplateId != null && toHouse.EnterTemplateId > 0)
            {
                List<EnterFlowItem> flowItems;
                if (string.IsNullOrEmpty(toHouse.EnterFlowInitJson))
                {
                    flowItems = new List<EnterFlowItem>();
                }
                else
                {
                    flowItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EnterFlowItem>>(toHouse.EnterFlowInitJson);
                }

                EnterFlowCreate flowcreate = new EnterFlowCreate();
                flowcreate.templateId = toHouse.EnterTemplateId.Value;
                flowcreate.model = data.model;
                flowcreate.assign = data.assign;
                if (!data.model.ContainsKey("@from"))
                {
                    flowcreate.model.Add("@from", old.StockNumber);
                }
                if (!data.model.ContainsKey("@fromtype"))
                {
                    flowcreate.model.Add("@fromtype", "入库单");
                }
                if (!data.model.ContainsKey("@FlowNumber"))
                {
                    flowcreate.model.Add("@FlowNumber", old.StockNumber);
                }

                flowcreate.UserId = user.UserId;
                flowcreate.flowId = data.flowId;
                foreach (var fitem in flowItems)
                {
                    if (!flowcreate.model.ContainsKey(fitem.id))
                    {
                        flowcreate.model.Add(fitem.id, fitem.GetRealValue(old));
                    }
                }

                var fcrsp = await BusUtility.Call("NewFlowTask", flowcreate);
                if (!fcrsp.IsSuccess())
                {
                    return BusResponse<int>.Error(145, fcrsp.Message);
                }
                enterStock.FlowId = fcrsp.GetResult<long>();
            }
            else
            {
                enterStock.FlowId = 0;
            }

            using (BLLTranScope scope = new BLLTranScope())
            {
                try
                {
                    await _enterDAL.Update(enterStock);
                    if (old.EnterMethod == 3)
                    {
                        List<string> ids = new List<string>();
                        foreach (var item in old.List)
                        {
                            //生成入库记录
                            if (await _recordDAL.Some(x => x.HouseId == old.ToHouseId && x.TargetType == item.TargetType && x.TargetId == item.TargetId))
                            {
                                await _recordDAL.InsertRecord(old.OrgId.Value, old.ToHouseId, item.TargetType.Value, item.TargetId, 1, old.Id, item.Quantity.Value, item.Price.Value);
                            }
                            else
                            {
                                MZ_StockRecord recc = new MZ_StockRecord();
                                recc.OrgId = old.OrgId.Value;
                                recc.HouseId = old.ToHouseId;
                                recc.TargetType = item.TargetType;
                                recc.TargetId = item.TargetId;
                                recc.Remnant = 0;
                                recc.LockRemnant = 0;
                                recc.FormType = 1;
                                recc.FormId = old.Id;
                                recc.Quantity = item.Quantity.Value;
                                recc.StockPrice = 0;
                                recc.Price = item.Price.Value;
                                recc.CreatedOn = DateTime.Now;
                                await _recordDAL.Insert(recc);
                            }


                            //增库存
                            MZ_StockPile pile = new MZ_StockPile();
                            pile.OrgId = old.OrgId.Value;
                            pile.HouseId = old.ToHouseId;
                            pile.TargetType = item.TargetType.Value;
                            pile.TargetId = item.TargetId;
                            pile.Price = item.Price.Value;
                            if (enterStock.Status == 2)
                            {
                                pile.Quantity = item.Quantity.Value;
                                pile.LockQuantity = 0;
                                await _pileDAL.IncreasePile(pile);
                                //添加入库设备Id
                                if (item.TargetType == 1)
                                {
                                    ids.Add(item.TargetId);
                                }

                            }
                            else
                            {
                                pile.Quantity = 0;
                                pile.LockQuantity = item.Quantity.Value;
                                await _pileDAL.IncreaseLock(pile);
                            }

                        }

                        if (ids.Count > 0)
                        {
                            //修改设备拥有者
                            await BusUtility.Dispatch("BatchIotOrg", new
                            {
                                Ids = ids,
                                OwnerOrgId = user.OrgId
                            });

                        }
                    }


                    // 完成
                    await scope.CompleteAsync();
                }
                catch (Exception ex)
                {
                    if (enterStock.FlowId > 0)
                    {
                        await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(enterStock.FlowId.Value);
                    }
                    return BusResponse<int>.Error(128, ex.Message);
                }
            }

            return BusResponse<int>.Success(enterStock.Status.Value);
        }
        public virtual async Task<BusResponse<int>> SubmitManualPile(IUserInfo user, string id, long flowId)
        {
            var old = await _enterDAL.Select(id);
            if (old == null)
            {
                return BusResponse<int>.Error(143, "入库单不存在");
            }
            if (old.Status == 2)
            {
                if (flowId > 0)
                {
                    MZ_EnterStock newUpdateStock = new MZ_EnterStock();
                    newUpdateStock.Id = id;
                    newUpdateStock.FlowId = flowId;
                    newUpdateStock.SetUpdateBy(user);
                    await _enterDAL.Update(newUpdateStock);
                }
                return BusResponse<int>.Success(2);
            }
            if (old.Status != 0)
            {
                return BusResponse<int>.Error(127, "状态错误");
            }

            old.List = await _enterDetailDAL.SelectList(x => x.StockId == id);

            MZ_EnterStock enterStock = new MZ_EnterStock();
            enterStock.Id = id;
            var toHouse = await _houseDAL.Select(old.ToHouseId);
            if (toHouse != null && toHouse.EnterTemplateId > 0 && flowId > 0)
            {
                enterStock.Status = 1;
            }
            else
            {
                enterStock.Status = 2;
            }
            enterStock.FlowId = flowId;
            enterStock.SetUpdateBy(user);


            using (BLLTranScope scope = new BLLTranScope())
            {
                await _enterDAL.Update(enterStock);
                if (old.EnterMethod == 3)
                {
                    List<string> ids = new List<string>();
                    foreach (var item in old.List)
                    {
                        //生成入库记录
                        if (await _recordDAL.Some(x => x.HouseId == old.ToHouseId && x.TargetType == item.TargetType && x.TargetId == item.TargetId))
                        {
                            await _recordDAL.InsertRecord(old.OrgId.Value, old.ToHouseId, item.TargetType.Value, item.TargetId, 1, old.Id, item.Quantity.Value, item.Price.Value);
                        }
                        else
                        {
                            MZ_StockRecord recc = new MZ_StockRecord();
                            recc.OrgId = old.OrgId.Value;
                            recc.HouseId = old.ToHouseId;
                            recc.TargetType = item.TargetType;
                            recc.TargetId = item.TargetId;
                            recc.Remnant = 0;
                            recc.LockRemnant = 0;
                            recc.FormType = 1;
                            recc.FormId = old.Id;
                            recc.Quantity = item.Quantity.Value;
                            recc.StockPrice = 0;
                            recc.Price = item.Price.Value;
                            recc.CreatedOn = DateTime.Now;
                            await _recordDAL.Insert(recc);
                        }


                        //增库存
                        MZ_StockPile pile = new MZ_StockPile();
                        pile.OrgId = old.OrgId.Value;
                        pile.HouseId = old.ToHouseId;
                        pile.TargetType = item.TargetType.Value;
                        pile.TargetId = item.TargetId;
                        pile.Price = item.Price.Value;
                        if (enterStock.Status == 2)
                        {
                            pile.Quantity = item.Quantity.Value;
                            pile.LockQuantity = 0;
                            await _pileDAL.IncreasePile(pile);

                            //添加入库设备Id
                            if (item.TargetType == 1)
                            {
                                ids.Add(item.TargetId);
                            }
                        }
                        else
                        {
                            pile.Quantity = 0;
                            pile.LockQuantity = item.Quantity.Value;
                            await _pileDAL.IncreaseLock(pile);
                        }
                    }

                    if (ids.Count > 0)
                    {
                        //修改设备拥有者
                        await BusUtility.Dispatch("BatchIotOrg", new
                        {
                            Ids = ids,
                            OwnerOrgId = user.OrgId
                        });
                    }
                }

                // 完成
                await scope.CompleteAsync();
            }

            return BusResponse<int>.Success(enterStock.Status.Value);
        }


        public virtual async Task<BusResponse<string>> ClearPile(string number, IUserInfo user)
        {
            var tmplist = await _enterDAL.SelectList(x => x.StockNumber == number);
            if (tmplist.Count == 0)
            {
                return BusResponse<string>.Error(111, "入库单号不存在");
            }
            var enterStock = tmplist.FirstOrDefault();
            if (enterStock.OrgId != user.OrgId)
            {
                return BusResponse<string>.Error(112, "无权限清除当前入库单");
            }
            var enterDetailList = await _enterDetailDAL.SelectList(x => x.StockId == enterStock.Id);

            using (BLLTranScope scope = new BLLTranScope())
            {
                try
                {
                    foreach (var item in enterDetailList)
                    {
                        //清除物品的库存
                        await _pileDAL.Delete(x => x.TargetType == item.TargetType && x.TargetId == item.TargetId);
                        //清除物品的库存记录
                        await _recordDAL.Delete(x => x.TargetType == item.TargetType && x.TargetId == item.TargetId);
                        //清除物品的关联入库单详情
                        await _enterDetailDAL.Delete(x => x.TargetType == item.TargetType && x.TargetId == item.TargetId);
                        //清除物品的关联出库单详情
                        await _leaveDetailDAL.Delete(x => x.TargetType == item.TargetType && x.TargetId == item.TargetId);
                        //清除使用记录
                        if (item.TargetType == 1)
                        {
                            await BusUtility.Dispatch("UpdateIotOrg", new
                            {
                                Id = item.TargetId,
                                OwnerOrgId = 0,
                                UseOrgId = 0,
                                UseUserId = 0
                            });
                        }

                    }
                    //清除入库单详情为空的入库单
                    await _enterDAL.DeleteNotDetail();
                    //清除出库单详情为空的出库单
                    await _leaveStockDAL.DeleteNotDetail();
                    // 完成
                    await scope.CompleteAsync();
                    return BusResponse<string>.Success();
                }
                catch (Exception ex)
                {
                    return BusResponse<string>.Error(113, ex.Message);
                }
            }
        }


        public virtual async Task<BusResponse<string>> ScanEnterPile(In_ManualStock ipt, IUserInfo user)
        {
            var allidlist = ipt.List.Select(x => x.TargetId).ToList();
            var pilelist = await _pileDAL.SelectPilesFromParentOrg(allidlist, user.OrgId);
            if (pilelist.Count == 0)
            {
                return BusResponse<string>.Error(110, "系统无法找到所操作物品");
            }
            var hasInList = pilelist.Where(x => x.OrgId == user.OrgId).ToList();
            if (hasInList.Count > 0)
            {
                return BusResponse<string>.Error(121, "物品：" + string.Join(',', hasInList.Select(x => x.Name)) + "已入库存");
            }

            var sourceOrgList = pilelist.Select(x => x.HouseId).Distinct().ToList();
            if (sourceOrgList.Count > 1)
            {
                return BusResponse<string>.Error(111, "物品需要来自相同的仓库");
            }

            long leaveOrgId = pilelist[0].OrgId.Value;

            MZ_StoreHouse tmphouse = await _houseDAL.Select(pilelist[0].HouseId);
            if (tmphouse == null)
            {
                return BusResponse<string>.Error(113, "物品所属企业的仓库不存在");
            }
            if (tmphouse.LeaveTemplateId > 0)
            {
                return BusResponse<string>.Error(114, "物品所属企业设置了出库审核");
            }


            List<MZ_LeaveDetail> needLeaveDetails = new List<MZ_LeaveDetail>();
            string leavestockId = _snowflake.NextId().ToString();
            foreach (var detail in ipt.List)
            {
                var tmppile = pilelist.Where(x => x.TargetId == detail.TargetId).First();
                if (tmppile == null)
                {
                    continue;
                }
                if (detail.Quantity > tmppile.Quantity)
                {
                    return BusResponse<string>.Error(122, "物品：" + tmppile.Name + "的来源库存不足");
                }
                MZ_LeaveDetail leavedetail = new MZ_LeaveDetail();
                leavedetail.StockId = leavestockId;
                if (detail.TargetType == 1)
                {
                    leavedetail.Quantity = 1;
                }
                else
                {
                    leavedetail.Quantity = detail.Quantity;
                }
                leavedetail.Price = detail.Price;
                leavedetail.TargetType = detail.TargetType;
                leavedetail.TargetId = detail.TargetId;
                needLeaveDetails.Add(leavedetail);
            }
            ArtificialUser artificialUser = new ArtificialUser(2, leaveOrgId);
            MZ_LeaveStock leaveStock = new MZ_LeaveStock();
            leaveStock.ExpressCompany = string.Empty;
            leaveStock.ExpressNumber = string.Empty;
            leaveStock.ExpressPhone = string.Empty;
            leaveStock.ToOrgId = user.OrgId;
            leaveStock.LeaveMethod = 0;
            leaveStock.OutDate = DateTime.Now;

            Tmp_CustomerInfo customer = null;
            var storageOption = _provider.GetService<IOptions<StorageOption>>().Value;
            if (storageOption.is_leave_customer)
            {
                var tmprr = await BusUtility.Call("GetCustomerByOrg", new
                {
                    from = leaveOrgId,
                    to = user.OrgId
                });
                if (tmprr.IsSuccess())
                {
                    customer = tmprr.GetResult<Tmp_CustomerInfo>();
                }
            }

            if (customer != null)
            {
                leaveStock.CustomerId = customer.Id;
                leaveStock.CustomerType = customer.CustomerType;
            }
            else
            {
                leaveStock.CustomerId = string.Empty;
                leaveStock.CustomerType = 0;
            }
            leaveStock.StockNumber = await GenerateCKNumber();
            leaveStock.FromHouseId = tmphouse.Id;
            leaveStock.ToHouseId = ipt.HouseId;
            leaveStock.SourceEnterId = string.Empty;
            leaveStock.Id = leavestockId;
            leaveStock.OrgId = artificialUser.OrgId;
            leaveStock.SetCreateBy(artificialUser);
            leaveStock.Status = 0;
            leaveStock.FlowId = 0;
            leaveStock.Remark = ipt.Remark;
            leaveStock.List = needLeaveDetails;


            using (BLLTranScope scope = new BLLTranScope())
            {
                try
                {
                    OutExtInfo info = new OutExtInfo();
                    var rsp = await _AllStockOut(leaveStock, artificialUser, false, true, info);
                    if (rsp.IsSuccess())
                    {
                        await scope.CompleteAsync();
                    }
                    return BusResponse<string>.Success(info.enterId);
                }
                catch (Exception ex)
                {
                    return BusResponse<string>.Error(322, ex.Message);
                }

            }

        }

    }
    public class OutExtInfo
    {
        public string enterId { get; set; }
    }
}
