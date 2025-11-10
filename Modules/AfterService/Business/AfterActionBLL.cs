using AfterService.DAL;
using Common.DataAc;
using Common.IdGenerator;
using Common.Share;
using FlowService.FlowNode.Builder;
using IoTService.DAL;
using Microsoft.Extensions.Logging;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AfterService.Business
{
    public class AfterActionBLL
    {
        private ITAServiceProvider _provider;
        private AfterActionDAL _afterActionDAL;
        private ILogger<AfterActionBLL> _log;
        public AfterActionBLL(ITAServiceProvider provider, AfterActionDAL afterActionDAL, ILoggerFactory factory)
        {
            _provider = provider;
            _afterActionDAL = afterActionDAL;
            _log = factory.CreateLogger<AfterActionBLL>();
        }
        [Trans]
        public virtual async Task<BusResponse<int>> DoPlaneActionEvent(ActionChangeData evt)
        {
            try
            {
                bool endFlow = false;
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
                            con.TargetField = "PlaneNumber";
                            con.FinalValue = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(con.Value);
                            con.Compare = "=";
                            conds.Add(con);
                        }
                    }
                    //过滤值
                    foreach (var acc in evt.actions)
                    {
                        switch (acc.TargetField)
                        {
                            case "TaskStatus":
                                {
                                    var tmpint = Newtonsoft.Json.JsonConvert.DeserializeObject<int?>(acc.Value);
                                    if (tmpint == null)
                                    {
                                        continue;
                                    }
                                    conds.Add(new ActionCondition()
                                    {
                                        TargetField = "TaskStatus",
                                        Compare = "<>",
                                        FinalValue = tmpint
                                    });
                                    acc.FinalValue = tmpint;
                                    actions.Add(acc);
                                    if (tmpint == 1)
                                    {
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "DispatchOn",
                                            FinalValue = DateTime.Now
                                        });
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "DispatchUserId",
                                            FinalValue = evt.UpdateId
                                        });
                                    }
                                    else if (tmpint == 2)
                                    {
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "ExecutedOn",
                                            FinalValue = DateTime.Now
                                        });
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "ExeUserId",
                                            FinalValue = evt.UpdateId
                                        });
                                    }
                                    else if (tmpint == 3)
                                    {
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "FinishedOn",
                                            FinalValue = DateTime.Now
                                        });
                                    }
                                    else if (tmpint == 5)
                                    {
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "CheckOn",
                                            FinalValue = DateTime.Now
                                        });
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "CheckUserId",
                                            FinalValue = evt.UpdateId
                                        });
                                    }
                                    else if (tmpint == 7)
                                    {
                                        endFlow = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                break;

                            case "@Reject":
                                {
                                    actions.Add(new ActionInfo()
                                    {
                                        FinalValue = 7,
                                        TargetField = "TaskStatus"
                                    });
                                    actions.Add(new ActionInfo()
                                    {
                                        TargetField = "EndOn",
                                        FinalValue = DateTime.Now
                                    });
                                }
                                break;
                        }
                    }

                };

                filter.Invoke(newCondition, newAction);
                int rs = await _afterActionDAL.DoAsync(evt, newCondition, newAction);
                if (endFlow == true)
                {
                    await _provider.GetService<WorkflowExecutor>().TerminateWorkflow(evt.FlowId);
                }
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
