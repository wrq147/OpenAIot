using Common.DataAc;
using Common.Json;
using Common.Share;
using MyAccess.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Core;

namespace AuthService.Business
{
    public class DataChangeBLL
    {
        private ITAServiceProvider _provider;
        private UserDAL _userDAL;
        public DataChangeBLL(ITAServiceProvider provider, UserDAL userDAL)
        {
            _provider = provider;
            _userDAL = userDAL;
        }
        [Trans]
        public virtual async Task<BusResponse<int>> DoActionEvent(ActionChangeData evt)
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
                        if (con.TargetField == "Signature")
                        {
                            string val = System.Text.Json.JsonSerializer.Deserialize<string>(con.Value, MyDefaultTextJsonConfig.DefaultOptions);
                            if (val == null)
                            {
                                throw new Exception("签名值不能为null");
                            }
                            if (val == "$waitsign")
                            {
                                con.FieldValue = "WaitSignature";
                            }
                            else
                            {
                                con.FinalValue = val;
                            }
                            con.Compare = "=";
                            conds.Add(con);
                        }
                        else if (con.TargetField == "Id")
                        {
                            var tmpval = System.Text.Json.JsonSerializer.Deserialize<object>(con.Value, MyDefaultTextJsonConfig.DefaultOptions);
                            if (tmpval is List<object> selectedlist)
                            {
                                var tmpdd = selectedlist.First() as IDictionary<string, object>;
                                con.FinalValue = Convert.ToInt64(tmpdd["id"]);
                                con.Compare = "=";
                                conds.Add(con);
                            }
                            else
                            {
                                con.FinalValue = Convert.ToInt64(tmpval);
                                con.Compare = "=";
                                conds.Add(con);
                            }
                        }
                    }

                    //过滤值
                    foreach (var acc in evt.actions)
                    {
                        switch (acc.TargetField)
                        {
                            case "Signature":
                                {
                                    string val = System.Text.Json.JsonSerializer.Deserialize<string>(acc.Value, MyDefaultTextJsonConfig.DefaultOptions);
                                    if (val == null)
                                    {
                                        throw new Exception("签名值不能为null");
                                    }
                                    if (val == "$waitsign")
                                    {
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "Signature",
                                            FieldValue = "WaitSignature"
                                        });
                                    }
                                    else
                                    {
                                        actions.Add(new ActionInfo()
                                        {
                                            TargetField = "Signature",
                                            FinalValue = val
                                        });
                                    }


                                }
                                break;
                        }
                    }

                };

                filter.Invoke(newCondition, newAction);

                await _userDAL.DoAsync(evt, newCondition, newAction);
                return BusResponse<int>.Success();
            }
            catch (Exception ex)
            {
                return BusResponse<int>.Error(133, ex.Message);
            }


        }
    }
}
