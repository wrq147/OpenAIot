using AuthService;
using Common.IdGenerator;
using Common.Share;
using StorageService.DAL;
using StorageService.Model;
using MyAccess.Aop;
using TemplateAction.Core;
using Common.DataAc;
using Microsoft.Extensions.Logging;
using Common.EventBus;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using ProducerService.DAL;
using Common.Json;

namespace StorageService.Business
{
    public class StorageActionBLL
    {
        private ITAServiceProvider _provider;
        private StorageActionDAL _crmActionDAL;
        private StockRecordDAL _recordDAL;
        private EnterStockDAL _enterDAL;
        private EnterDetailDAL _enterDetailDAL;
        private StockPileDAL _pileDAL;
        private LeaveStockDAL _leaveStockDAL;
        private LeaveDetailDAL _leaveDetailDAL;
        private SnowflakeHelper _snowflake;
        private ILogger<StorageActionBLL> _log;
        public StorageActionBLL(ITAServiceProvider provider, StorageActionDAL crmActionDAL, SnowflakeHelper snowflake, LeaveStockDAL leaveStockDAL, LeaveDetailDAL leaveDetailDAL,
            StockRecordDAL recordDAL, EnterStockDAL enterDAL, EnterDetailDAL enterDetailDAL, StockPileDAL pileDAL, ILoggerFactory factory)
        {
            _provider = provider;
            _crmActionDAL = crmActionDAL;
            _snowflake = snowflake;
            _leaveStockDAL = leaveStockDAL;
            _leaveDetailDAL = leaveDetailDAL;
            _recordDAL = recordDAL;
            _enterDAL = enterDAL;
            _enterDetailDAL = enterDetailDAL;
            _pileDAL = pileDAL;
            _log = factory.CreateLogger<StorageActionBLL>();
        }

        private async Task FlowLeaveErr(string id)
        {
            StockBLL stockBLL = _provider.GetService<StockBLL>();
            List<MZ_LeaveStock> oldlist = await _leaveStockDAL.SelectList(x => x.StockNumber == id);
            if (oldlist.Count <= 0)
            {
                return;
            }
            MZ_LeaveStock old = oldlist[0];
            if (old.LeaveMethod == 3)
            {
                MZ_LeaveApply apply = new MZ_LeaveApply();
                apply.OutStatus = 3;
                apply.Id = old.SourceEnterId;
                await _provider.GetService<LeaveApplyDAL>().Update(apply);
            }
        }
        private async Task FlowLeave(string id)
        {
            StockBLL stockBLL = _provider.GetService<StockBLL>();
            List<MZ_LeaveStock> oldlist = await _leaveStockDAL.SelectList(x => x.StockNumber == id);
            if (oldlist.Count <= 0)
            {
                return;
            }
            MZ_LeaveStock old = oldlist[0];

            var tlist = await _leaveDetailDAL.SelectList(x => x.StockId == old.Id);

            var toHouse = await _provider.GetService<StoreHouseDAL>().Select(old.ToHouseId);
            MZ_EnterStock enterStock = new MZ_EnterStock();
            enterStock.Id = _snowflake.NextId().ToString();
            enterStock.OrgId = old.ToOrgId;
            enterStock.FromOrgId = old.OrgId;
            enterStock.StockNumber = await stockBLL.GenerateRKNumber();
            enterStock.FromHouseId = old.FromHouseId;
            enterStock.ToHouseId = old.ToHouseId;
            enterStock.EnterMethod = old.LeaveMethod;
            enterStock.ExpressNumber = old.ExpressNumber;
            enterStock.ExpressCompany = old.ExpressCompany;
            enterStock.ExpressPhone = old.ExpressPhone;
            enterStock.createId = 0;
            enterStock.create_time = DateTime.Now;
            enterStock.updateId = 0;
            enterStock.update_time = enterStock.create_time;
            enterStock.InDate = old.OutDate;
            enterStock.Remark = string.Empty;
            enterStock.FlowId = 0;
            if (toHouse != null && toHouse.EnterTemplateId > 0)
            {
                enterStock.Status = 0;
            }
            else
            {
                enterStock.Status = 2;
            }
            List<MZ_EnterDetail> enterlist = new List<MZ_EnterDetail>();
            foreach (var outItem in tlist)
            {
                MZ_EnterDetail detail = new MZ_EnterDetail();
                detail.StockId = enterStock.Id;
                detail.TargetType = outItem.TargetType;
                detail.TargetId = outItem.TargetId;
                detail.Quantity = outItem.Quantity;
                detail.Price = outItem.Price;
                enterlist.Add(detail);
            }
            List<string> deviceIds = new List<string>();
            if (old.Status == 1)
            {
                foreach (var t in tlist)
                {
                    //添加出库库存记录
                    await _recordDAL.InsertRecord(old.OrgId.Value, old.FromHouseId, t.TargetType.Value, t.TargetId, 0, old.Id, t.Quantity.Value, t.Price.Value);
                    //减锁库存
                    if (await _pileDAL.ReduceLock(old.FromHouseId, t.TargetType.Value, t.TargetId, t.Quantity.Value, t.Price) == 0)
                    {
                        throw new Exception("锁库存不足");
                    }

                    if (t.TargetType == 1)
                    {
                        deviceIds.Add(t.TargetId);
                    }
                }
            }
            else if (old.Status == 0)
            {
                foreach (var t in tlist)
                {

                    //添加出库库存记录
                    await _recordDAL.InsertRecord(old.OrgId.Value, old.FromHouseId, t.TargetType.Value, t.TargetId, 0, old.Id, t.Quantity.Value, t.Price.Value);
                    //减库存
                    if (await _pileDAL.ReducePile(old.FromHouseId, t.TargetType.Value, t.TargetId, t.Quantity.Value, t.Price) == 0)
                    {
                        throw new Exception("库存不足");
                    }

                    if (t.TargetType == 1)
                    {
                        deviceIds.Add(t.TargetId);
                    }
                }
            }
            else
            {
                return;
            }

            if (old.FromHouseId != old.ToHouseId)
            {
                //生成入库单
                await _enterDAL.Insert(enterStock);
                //生成入库单详情
                await _enterDetailDAL.Insert(enterlist);

                foreach (var item in enterlist)
                {
                    //生成入库记录
                    await _recordDAL.InsertRecord(enterStock.OrgId.Value, enterStock.ToHouseId, item.TargetType.Value, item.TargetId, 1, enterStock.Id, item.Quantity.Value, item.Price.Value);
                    //增库存
                    MZ_StockPile pile = new MZ_StockPile();
                    pile.OrgId = enterStock.OrgId.Value;
                    pile.HouseId = enterStock.ToHouseId;
                    pile.TargetType = item.TargetType.Value;
                    pile.TargetId = item.TargetId;
                    pile.Price = item.Price;
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




            if (old.LeaveMethod == 3)
            {
                if (deviceIds.Count > 0)
                {
                    //更改设备使用者
                    await BusUtility.Dispatch("BatchIotOrg", new
                    {
                        Ids = deviceIds,
                        UseOrgId = old.OrgId.Value,
                    });

                }


                var leaveApply = await _provider.GetService<LeaveApplyDAL>().Select(old.SourceEnterId);
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
                    apply.Id = old.SourceEnterId;
                    await _provider.GetService<LeaveApplyDAL>().Update(apply);
                }
            }

            if (deviceIds.Count > 0 && old.ToOrgId != old.OrgId)
            {
                if (old.CustomerType == 0)
                {
                    //更改设备拥有者
                    await BusUtility.Dispatch("BatchIotOrg", new
                    {
                        Ids = deviceIds,
                        OwnerOrgId = old.ToOrgId.Value,
                        ClearOwnerOrgId = old.LeaveMethod == 1 ? old.OrgId.Value : 0
                    });

                }
                else if (old.CustomerType == 1)
                {
                    //更改设备使用者
                    await BusUtility.Dispatch("BatchIotOrg", new
                    {
                        Ids = deviceIds,
                        UseOrgId = old.ToOrgId.Value,
                    });
                }

                try
                {
                    await BusUtility.Dispatch("StockLeave", new
                    {
                        OrgId = old.OrgId,
                        TargetOrgId = old.ToOrgId,
                        DevIds = string.Join(',', deviceIds)
                    });
                }
                catch { }
            }
        }

        private async Task FlowEnter(string id)
        {
            List<MZ_EnterStock> oldlist = await _enterDAL.SelectList(x => x.StockNumber == id);
            if (oldlist.Count <= 0)
            {
                return;
            }
            MZ_EnterStock old = oldlist[0];
            if (old.Status == 1)
            {
                var enterlist = await _enterDetailDAL.SelectList(x => x.StockId == old.Id);
                foreach (var item in enterlist)
                {
                    //库存解锁
                    await _pileDAL.LockBack(old.ToHouseId, item.TargetType.Value, item.TargetId, item.Quantity.Value);
                }
            }
            else if (old.Status == 0)
            {
                old.List = await _enterDetailDAL.SelectList(x => x.StockId == old.Id);
                List<string> deviceIds = new List<string>();
                foreach (var item in old.List)
                {
                    //添加入库批次Id
                    if (item.TargetType == 1)
                    {
                        deviceIds.Add(item.TargetId);
                    }

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
                    pile.Quantity = item.Quantity.Value;
                    pile.LockQuantity = 0;
                    await _pileDAL.IncreasePile(pile);
                }
                if (deviceIds.Count > 0)
                {
                    //修改设备拥有者
                    await BusUtility.Dispatch("BatchIotOrg", new
                    {
                        Ids = deviceIds,
                        OwnerOrgId = old.OrgId.Value
                    });
                }

            }

        }
        [Trans]
        public virtual async Task<BusResponse<int>> DoStockActionEvent(ActionChangeData evt)
        {
            try
            {
                int isStatusChange = 0;
                string targetId = string.Empty;

                List<ActionCondition> newCondition = new List<ActionCondition>();
                List<ActionInfo> newAction = new List<ActionInfo>();
                Action<List<ActionCondition>, List<ActionInfo>> filter = (conds, actions) =>
                {
                    //过滤条件
                    conds.Add(new ActionCondition()
                    {
                        TargetField = "OrgId",
                        Compare = "=",
                        FinalValue = evt.OrgId
                    });
                    foreach (var con in evt.conditions)
                    {
                        if (con.TargetField == "@Number")
                        {
                            con.TargetField = "StockNumber";
                            targetId = System.Text.Json.JsonSerializer.Deserialize<string>(con.Value, MyDefaultTextJsonConfig.DefaultOptions);
                            con.FinalValue = targetId;
                            con.Compare = "=";
                            conds.Add(con);
                        }
                    }
                    //过滤值
                    foreach (var acc in evt.actions)
                    {
                        switch (acc.TargetField)
                        {
                            case "Status":
                                {
                                    var tmpint = System.Text.Json.JsonSerializer.Deserialize<int?>(acc.Value, MyDefaultTextJsonConfig.DefaultOptions);
                                    if (tmpint == null)
                                    {
                                        continue;
                                    }
                                    if (tmpint == 2)
                                    {
                                        conds.Add(new ActionCondition()
                                        {
                                            TargetField = "Status",
                                            Compare = "<>",
                                            FinalValue = 2
                                        });

                                        isStatusChange = 2;
                                        acc.FinalValue = 2;
                                    }
                                    else if (tmpint == 3)
                                    {
                                        conds.Add(new ActionCondition()
                                        {
                                            TargetField = "Status",
                                            Compare = "<>",
                                            FinalValue = 3
                                        });
                                        isStatusChange = 3;
                                        acc.FinalValue = 3;
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    actions.Add(acc);
                                    actions.Add(new ActionInfo()
                                    {
                                        TargetField = "update_time",
                                        FinalValue = DateTime.Now
                                    });
                                    actions.Add(new ActionInfo()
                                    {
                                        TargetField = "updateId",
                                        FinalValue = evt.UpdateId
                                    });
                                }
                                break;
                            case "@Reject":
                                actions.Add(new ActionInfo()
                                {
                                    FinalValue = 3,
                                    TargetField = "Status"
                                });
                                actions.Add(new ActionInfo()
                                {
                                    TargetField = "update_time",
                                    FinalValue = DateTime.Now
                                });
                                actions.Add(new ActionInfo()
                                {
                                    TargetField = "updateId",
                                    FinalValue = evt.UpdateId
                                });
                                break;
                        }
                    }

                };

                filter.Invoke(newCondition, newAction);

                if (!string.IsNullOrEmpty(targetId) && isStatusChange > 0)
                {
                    //执行状态变更
                    if (isStatusChange == 2)
                    {
                        if (evt.TargetName == "出库单")
                        {
                            await FlowLeave(targetId);
                        }
                        else
                        {
                            await FlowEnter(targetId);
                        }
                    }
                    else if (isStatusChange == 3)
                    {
                        if (evt.TargetName == "出库单")
                        {
                            await FlowLeaveErr(targetId);
                        }
                    }
                }

                int rs = await _crmActionDAL.DoAsync(evt, newCondition, newAction);
                return BusResponse<int>.Success(rs);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.StackTrace);
                return BusResponse<int>.Error(133, ex.Message);
            }


        }

        [Trans]
        public virtual async Task<BusResponse<int>> DoApplyActionEvent(ActionChangeData evt)
        {
            try
            {
                List<ActionCondition> newCondition = new List<ActionCondition>();
                List<ActionInfo> newAction = new List<ActionInfo>();
                Action<List<ActionCondition>, List<ActionInfo>> filter = (conds, actions) =>
                {
                    //过滤条件
                    conds.Add(new ActionCondition()
                    {
                        TargetField = "OrgId",
                        Compare = "=",
                        FinalValue = evt.OrgId
                    });
                    foreach (var con in evt.conditions)
                    {
                        if (con.TargetField == "@Number")
                        {
                            con.TargetField = "ApplyNumber";
                            con.FinalValue = System.Text.Json.JsonSerializer.Deserialize<string>(con.Value, MyDefaultTextJsonConfig.DefaultOptions);
                            con.Compare = "=";
                            conds.Add(con);
                        }
                    }
                    //过滤值
                    foreach (var acc in evt.actions)
                    {
                        switch (acc.TargetField)
                        {
                            case "Status":
                                {
                                    var tmpint = System.Text.Json.JsonSerializer.Deserialize<int?>(acc.Value, MyDefaultTextJsonConfig.DefaultOptions);
                                    if (tmpint == null)
                                    {
                                        continue;
                                    }
                                    conds.Add(new ActionCondition()
                                    {
                                        TargetField = "Status",
                                        Compare = "=",
                                        FinalValue = 1
                                    });
                                    acc.FinalValue = tmpint;
                                    actions.Add(acc);
                                }
                                break;
                            case "@Reject":
                                actions.Add(new ActionInfo()
                                {
                                    FinalValue = 3,
                                    TargetField = "Status"
                                });
                                break;
                        }
                    }
                };

                filter.Invoke(newCondition, newAction);
                int rs = await _crmActionDAL.DoAsync(evt, newCondition, newAction);
                return BusResponse<int>.Success(rs);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.StackTrace);
                return BusResponse<int>.Error(133, ex.Message);
            }
        }

    }
}
