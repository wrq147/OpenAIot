using Common.DataAc;
using Common.Share;
using IoTService.DAL;
using MyAccess.Aop;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace IoTService.Business
{
    public class IotActionBLL
    {
        private ITAServiceProvider _provider;
        public IotActionBLL(ITAServiceProvider provider)
        {
            _provider = provider;
        }

        [Trans]
        public virtual async Task<BusResponse<int>> DoActionEvent(ActionChangeData evt)
        {
            try
            {
                bool isWarnDealed = false;
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
                            con.TargetField = "WarnNumber";
                            targetId = JsonConvert.DeserializeObject<string>(con.Value);
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
                                    var tmpint = JsonConvert.DeserializeObject<int>(acc.Value);
                                    if (tmpint == 0 || tmpint == 2)
                                    {
                                        conds.Add(new ActionCondition()
                                        {
                                            TargetField = "Status",
                                            Compare = "<>",
                                            FinalValue = tmpint
                                        });
                                        acc.FinalValue = tmpint;
                                        actions.Add(acc);
                                    }
                                    else if (tmpint == 1)
                                    {
                                        conds.Add(new ActionCondition()
                                        {
                                            TargetField = "Status",
                                            Compare = "<>",
                                            FinalValue = 1
                                        });
                                        actions.Add(acc);
                                        acc.FinalValue = 1;

                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "ClearOn",
                                            FinalValue = DateTime.Now
                                        });

                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "ClearId",
                                            FinalValue = evt.UpdateId
                                        });
                                        isWarnDealed = true;
                                    }
                                    else
                                    {
                                        break;
                                    }


                                }
                                break;
                            case "ClearRemark":
                                {
                                    actions.Add(new ActionInfo()
                                    {
                                        TargetField = "ClearRemark",
                                        FinalValue = JsonConvert.DeserializeObject<string>(acc.Value)
                                    });
                                }
                                break;

                        }
                    }
                };

                filter.Invoke(newCondition, newAction);
                await _provider.GetService<IotActionDAL>().DoAsync(evt, newCondition, newAction);

                //清除沉默周期
                if (isWarnDealed && !string.IsNullOrEmpty(targetId))
                {
                    var warnInfo = await _provider.GetService<IotWarningDAL>().SelectWarnDev(targetId);
                    if (warnInfo != null)
                    {
                        var redis = _provider.GetService<IotRedisHelper>();
                        await redis.KeyDeleteAsync("IotQuick::" + warnInfo.DeviceId + "::" + warnInfo.Code);
                    }
                }
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(133, ex.Message);
            }


        }
    }
}
