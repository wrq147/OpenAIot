using Common.DataAc;
using Common.EventBus;
using Common.Share;
using MESService.DAL;
using MESService.Model;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace MESService.Business
{
    public class MesActionBLL
    {
        private ITAServiceProvider _provider;
        public MesActionBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }
        public virtual async Task<BusResponse<int>> DoReportActionEvent(ActionChangeData evt)
        {
            try
            {
                var mesActionDAL = _provider.GetService<MesActionDAL>();
                int isStatusChange = 0;
                List<ActionCondition> newCondition = new List<ActionCondition>();
                List<ActionInfo> newAction = new List<ActionInfo>();
                string filterNumber = null;
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
                            con.TargetField = "Number";
                            con.FinalValue = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(con.Value);
                            con.Compare = "=";
                            filterNumber = (string)con.FinalValue;
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
                                    var tmpint = Newtonsoft.Json.JsonConvert.DeserializeObject<int?>(acc.Value);
                                    if (tmpint == null)
                                    {
                                        continue;
                                    }
                                    isStatusChange = tmpint.Value;
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
                                    FinalValue = 4,
                                    TargetField = "Status"
                                });
                                break;
                        }
                    }
                };

                filter.Invoke(newCondition, newAction);
                int rs = await mesActionDAL.DoAsync(evt, newCondition, newAction);
                if (isStatusChange == 2)
                {
                    //报工成功，计算任务进度
                    if (!string.IsNullOrEmpty(filterNumber))
                    {
                        await BusUtility.Dispatch("ResetNumberTaskInfo", new
                        {
                            OrgId = evt.OrgId,
                            Number = filterNumber
                        });
                    }
                }
                return BusResponse<int>.Success(rs);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(133, ex.Message);
            }
        }

        public virtual async Task<BusResponse<int>> DoPlanActionEvent(ActionChangeData evt)
        {
            try
            {
                var mesActionDAL = _provider.GetService<MesActionDAL>();
                int isStatusChange = 0;
                List<ActionCondition> newCondition = new List<ActionCondition>();
                List<ActionInfo> newAction = new List<ActionInfo>();
                string filterNumber = null;
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
                            con.TargetField = "Number";
                            con.FinalValue = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(con.Value);
                            con.Compare = "=";
                            filterNumber = (string)con.FinalValue;
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
                                    var tmpint = Newtonsoft.Json.JsonConvert.DeserializeObject<int?>(acc.Value);
                                    if (tmpint == null)
                                    {
                                        continue;
                                    }
                                    isStatusChange = tmpint.Value;
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
                                    FinalValue = 6,
                                    TargetField = "Status"
                                });
                                break;
                        }
                    }
                };

                filter.Invoke(newCondition, newAction);
                int rs = await mesActionDAL.DoAsync(evt, newCondition, newAction);
                if (isStatusChange == 2)
                {
                    //生成生产工单
                    if (!string.IsNullOrEmpty(filterNumber))
                    {
                        await BusUtility.Dispatch("GenerateNumberWorkOrder", new
                        {
                            OrgId = evt.OrgId,
                            Number = filterNumber
                        });
                    }
                }
                return BusResponse<int>.Success(rs);
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(133, ex.Message);
            }
        }

    }
}
