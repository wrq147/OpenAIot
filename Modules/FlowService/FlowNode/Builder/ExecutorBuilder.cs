using AuthService;
using FlowService.FlowNode.Builder.Step;
using FlowService.Model;
using log4net.Core;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json.Linq;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TemplateAction.Common;
using TemplateAction.Core;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace FlowService.FlowNode.Builder
{
    /// <summary>
    /// 流程定义构建工厂
    /// </summary>
    public class ExecutorBuilder
    {
        private ITAServiceProvider _provider;
        public ITAServiceProvider Provider { get { return _provider; } }
        private List<WorkflowStep> _steps = new List<WorkflowStep>();
        private BuilderContext _context;
        private Dictionary<string, List<Out_UserItem>> _assign;
        private MZ_FlowTemplate _template;
        private List<DeletedSteps> _deletedSteps = new List<DeletedSteps>();
        private HashSet<string> _undeletedSteps = new HashSet<string>();
        public ExecutorBuilder(ITAServiceProvider provider, MZ_FormData formData)
        {
            _assign = formData.Assign;
            _context = new BuilderContext();
            _context.FormItems = new Dictionary<string, object>();
            foreach (var fi in formData.Model)
            {
                _context.FormItems.Add(fi.Key, fi.Value);
            }
            _context.InputParams = formData.inputParams;
            _context.Fields = formData.Fields;
            _context.UserDeptList = formData.UserDeptList;
            _context.Creator = formData.CreateUserId;
            _context.Executor = formData.CreateUserId;
            _template = formData.Template;
            _provider = provider;
        }
        public void AddStep(WorkflowStep step)
        {
            if (string.IsNullOrEmpty(step.Id))
            {
                step.Id = "_i" + _steps.Count;
            }
            step.Index = _steps.Count;
            _steps.Add(step);
        }

        public async Task<WorkflowExecutor> Build(FlowBaseNode root, int state)
        {
            WorkflowExecutor exe = _provider.GetService<WorkflowExecutor>();
            BuildWorkflowNode<StartStep>(new LinkedList<NodeLinkItem>(), root, null, state);
            this.End();
            var unlist = this._steps.Where(x => _undeletedSteps.Contains(x.Id)).Select(x => x.Index);

            //清除跳转节点
            foreach (var unitem in unlist)
            {
                for (int i = _deletedSteps.Count - 1; i >= 0; i--)
                {
                    var deleteditem = _deletedSteps[i];
                    if (deleteditem.Start <= unitem && unitem <= deleteditem.End)
                    {
                        _deletedSteps.RemoveAt(i);
                    }
                }
            }

            //无用节点合并
            if (_deletedSteps.Count > 1)
            {
                List<DeletedSteps> newDeleted = new List<DeletedSteps>();
                _deletedSteps.Sort((a, b) => a.Start.CompareTo(b.Start));
                for (int i = _deletedSteps.Count - 1; i >= 0; i--)
                {
                    int preIdx = i - 1;
                    if (preIdx >= 0)
                    {
                        var preStep = _deletedSteps[preIdx];
                        if (preStep.End < _deletedSteps[i].End)
                        {
                            newDeleted.Add(_deletedSteps[i]);
                        }
                    }
                    else
                    {
                        newDeleted.Add(_deletedSteps[i]);
                    }
                }
                _deletedSteps = newDeleted;
            }

            //替换掉无用节点
            foreach (var delitem in _deletedSteps)
            {
                for (int i = delitem.Start; i <= delitem.End; i++)
                {
                    var tmpNode = _steps[i];
                    var firstNode = _steps[delitem.Start];
                    _steps[i] = new NextStep()
                    {
                        Id = tmpNode.Id,
                        Name = tmpNode.Name,
                        ParentIndex = firstNode.ParentIndex,
                        Level = firstNode.Level,
                        Index = tmpNode.Index
                    };
                }
            }

            exe.Init(_steps);
            return await Task.FromResult(exe);
        }

        private void BuildWorkflowNode<T>(LinkedList<NodeLinkItem> levelnodes, FlowBaseNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            if (node == null) return;
            levelnodes.AddLast(new NodeLinkItem(node.type, node.id));
            switch (node.type)
            {
                case "ROOT":
                    BuildWorkflowNode(levelnodes, node.children, this.Start((RootNode)node), state);
                    break;
                case "EMPTY":
                    BuildWorkflowNode(levelnodes, node.children, stepBuilder, state);
                    break;
                case "APPROVAL":
                    BuildApproval(levelnodes, (SPNode)node, stepBuilder, state);
                    break;
                case "USER":
                    BuildOperator(levelnodes, (OperatorNode)node, stepBuilder, state);
                    break;
                case "CONDITIONS":
                    var nodes = (ConditionGroupNode)node;
                    BuildCondition(levelnodes, nodes.branchs, 0, stepBuilder, state);
                    BuildWorkflowNode(levelnodes, node.children, stepBuilder, state);
                    break;
                case "CC":
                    BuildCS(levelnodes, (CSNode)node, stepBuilder, state);
                    break;
                case "TRIGGER":
                    BuildTrigger(levelnodes, (TriggerNode)node, stepBuilder, state);
                    break;
                case "DELAY":
                    BuildDelay(levelnodes, (DelayNode)node, stepBuilder, state);
                    break;
                case "CONCURRENTS":
                    BuildParallel(levelnodes, (ConcurrentGroupNode)node, stepBuilder, state);
                    BuildWorkflowNode(levelnodes, node.children, stepBuilder, state);
                    break;
                case "DATAXE":
                    BuildData(levelnodes, (DataNode)node, stepBuilder, state);
                    break;
                case "FUNC":
                    BuildFunc(levelnodes, (FuncNode)node, stepBuilder, state);
                    break;
            }
            levelnodes.RemoveLast();
        }

        private void BuildOperator<T>(LinkedList<NodeLinkItem> levelnodes, OperatorNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            foreach (var fp in node.props.formPerms)
            {
                if (fp.perm == "E")
                    _context.FormItems.Remove(fp.id);
            }

            List<string> assigners = new List<string>();
            switch (node.props.assignedType)
            {
                case "ASSIGN_USER":
                    //指定办理人
                    assigners = node.props.assignedUser.Select(x => x.id.ToString()).ToList();
                    break;
                case "SELF":
                    //发起人自己
                    assigners.Add(_context.Creator.ToString());
                    break;
                case "FORM_USER":
                    //表单内联系人
                    break;
                case "ROLE":
                    //指定角色
                    var roles = node.props.role.Select(x => x.id).ToList();
                    TAAsyncHelper.RunSync(async () =>
                    {
                        var userIds = await _provider.GetService<DAL.OrgDAL>().SelectUserByRoles(roles).ConfigureAwait(false);
                        assigners = userIds.ConvertAll(x => x.ToString());
                    });
                    break;
                case "EQUIP_USER":
                    break;
            }

            var tbuilder = stepBuilder.Then<OperatorTask>(x =>
            {
                x.Step.timeLimit = node.props.timeLimit;
                x.Step.refuseTxt = node.props.refuseTxt;
                x.Step.Name = node.name;
                x.Step.FormPerms = node.props.formPerms;
                x.Step.optionInit = node.props.optionInit;
                x.Step.Id = node.id;
                if (node.props.assignedType == "FORM_USER")
                {
                    x.Step.formUser = node.props.formUser;
                    x.Step.formDevice = string.Empty;
                    x.Step.AssignedPrincipal = string.Empty;
                }
                else if (node.props.assignedType == "EQUIP_USER")
                {
                    x.Step.formUser = string.Empty;
                    x.Step.formDevice = node.props.formDevice;
                    x.Step.AssignedPrincipal = string.Empty;
                }
                else
                {
                    x.Step.formUser = string.Empty;
                    x.Step.formDevice = string.Empty;
                    x.Step.AssignedPrincipal = string.Join(',', assigners);
                }

            }).WithOption(node.props.exeTxt, "primary").Do(then =>
            {
                then.Start<OperatorStep>(x =>
                {
                    x.Step.Name = node.props.exeTxt;
                });
            });

            if (node.props.enableChange)
            {
                tbuilder.WithOption("变更办理人", "primary").Do(then =>
                 {
                     then.Start<OperatorTask>(x =>
                     {
                         x.Step.timeLimit = node.props.timeLimit;
                         x.Step.refuseTxt = node.props.refuseTxt;
                         x.Step.Name = "变更办理人";
                         x.Step.FormPerms = node.props.formPerms;
                         x.Step.formUser = node.props.formUser;
                         x.Step.AssignedPrincipal = string.Empty;
                     }).WithOption(node.props.exeTxt, "primary").Do(then =>
                     {
                         then.Start<OperatorStep>(x =>
                         {
                             x.Step.Name = node.props.exeTxt;
                         });
                     }).WithOption(node.props.refuseTxt, "danger").Do(then =>
                     {
                         var reject = then.Start<RejectedStep>(x =>
                         {
                             x.Step.Name = node.props.refuseTxt;
                         });
                         if (node.props.refuse.type == "TO_END")
                         {
                             //直接结束流程
                             reject.Step.ToEnd = true;
                             reject.Then<EndStep>();
                         }
                         else if (node.props.refuse.type == "TO_BEFORE")
                         {
                             //驳回到上级办理节点
                             var tmpnode = levelnodes.Last.Previous;
                             while (tmpnode != null)
                             {
                                 if (tmpnode.Value.type == "USER")
                                 {
                                     break;
                                 }
                                 tmpnode = tmpnode.Previous;
                             }
                             if (tmpnode == null)
                             {
                                 reject.Then<EndStep>();
                             }
                             else
                             {
                                 reject.Then<GotoStep>(x =>
                                 {
                                     x.Step.TargetId = tmpnode.Value.id;
                                 });
                                 _undeletedSteps.Add(tmpnode.Value.id);
                             }
                         }
                         else if (node.props.refuse.type == "TO_NODE")
                         {
                             //驳回到指定节点
                             reject.Then<GotoStep>(x =>
                             {
                                 x.Step.TargetId = node.props.refuse.target;
                             });
                             _undeletedSteps.Add(node.props.refuse.target);
                         }

                     });
                 });
            }

            tbuilder.WithOption(node.props.refuseTxt, "danger").Do(then =>
            {
                var reject = then.Start<RejectedStep>(x =>
                {
                    x.Step.Name = node.props.refuseTxt;
                });
                if (node.props.refuse.type == "TO_END")
                {
                    //直接结束流程
                    reject.Then<EndStep>();
                }
                else if (node.props.refuse.type == "TO_BEFORE")
                {
                    //驳回到上级办理节点
                    var tmpnode = levelnodes.Last.Previous;
                    while (tmpnode != null)
                    {
                        if (tmpnode.Value.type == "USER")
                        {
                            break;
                        }
                        tmpnode = tmpnode.Previous;
                    }
                    if (tmpnode == null)
                    {
                        reject.Then<EndStep>();
                    }
                    else
                    {
                        reject.Then<GotoStep>(x =>
                        {
                            x.Step.TargetId = tmpnode.Value.id;
                        });
                        _undeletedSteps.Add(tmpnode.Value.id);
                    }
                }
                else if (node.props.refuse.type == "TO_NODE")
                {
                    //驳回到指定节点
                    reject.Then<GotoStep>(x =>
                    {
                        x.Step.TargetId = node.props.refuse.target;
                    });
                    _undeletedSteps.Add(node.props.refuse.target);
                }

            });

            BuildWorkflowNode(levelnodes, node.children, tbuilder, state);
        }


        /// <summary>
        /// 创建审批节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="levelnodes"></param>
        /// <param name="node"></param>
        /// <param name="stepBuilder"></param>
        /// <param name="state"></param>
        /// <exception cref="Exception"></exception>
        private async void BuildApproval<T>(LinkedList<NodeLinkItem> levelnodes, SPNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            foreach (var fp in node.props.formPerms)
            {
                if (fp.perm == "E")
                    _context.FormItems.Remove(fp.id);
            }


            List<string> assigners = new List<string>();
            bool canadd = false;
            switch (node.props.assignedType)
            {
                case "ASSIGN_USER":
                    //指定审批人
                    assigners = node.props.assignedUser.Select(x => x.id.ToString()).ToList();
                    break;
                case "SELF":
                    //发起人自己
                    assigners.Add(_context.Creator.ToString());
                    break;
                case "SELF_SELECT":
                    //发起人自选
                    {
                        canadd = true;
                        List<Out_UserItem> usrlist;
                        if (_assign.TryGetValue(node.id, out usrlist))
                        {
                            foreach (var oui in usrlist)
                            {
                                assigners.Add(oui.Id);
                            }
                        }
                    }
                    break;
                case "LEADER_TOP":
                    //多级主管依次审批
                    {
                        var tmporgDAL = _provider.GetService<DAL.OrgDAL>();
                        var firstDept = await tmporgDAL.SelectDeptByUid(_context.Creator, _template.OrgId.Value);
                        if (firstDept == null)
                        {
                            throw new Exception("用户不存在主管");
                        }
                        if (node.props.leaderTop.endCondition == "TOP")
                        {
                            string[] deptPath = firstDept.ancestors.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            long[] lids = Array.ConvertAll(deptPath, x => long.Parse(x));
                            var deptlist = await tmporgDAL.SelectByList(lids);
                            for (int i = lids.Length - 1; i >= 0; i--)
                            {
                                var deptpath = lids[i];
                                var tmpdept = deptlist.Where(x => x.dept_id == deptpath).First();
                                if (tmpdept != null)
                                {
                                    var tmpLeaders = await tmporgDAL.SelectDeptLeaders(tmpdept.dept_id.Value);
                                    foreach (var tmpuid in tmpLeaders)
                                    {
                                        assigners.Add(tmpuid.ToString());
                                    }
                                }
                            }
                        }
                        else if (node.props.leaderTop.endCondition == "LEAVE")
                        {
                            string[] deptPath = firstDept.ancestors.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            long[] lids = Array.ConvertAll(deptPath, x => long.Parse(x));
                            var deptlist = await tmporgDAL.SelectByList(lids);
                            int edidx = lids.Length - node.props.leaderTop.endLevel;
                            for (int i = lids.Length - 1; i >= 0; i--)
                            {
                                if (i < edidx)
                                {
                                    break;
                                }

                                var deptpath = lids[i];
                                var tmpdept = deptlist.Where(x => x.dept_id == deptpath).First();
                                if (tmpdept != null)
                                {
                                    var tmpLeaders = await tmporgDAL.SelectDeptLeaders(tmpdept.dept_id.Value);
                                    foreach (var tmpuid in tmpLeaders)
                                    {
                                        assigners.Add(tmpuid.ToString());
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "LEADER":
                    //发起人的指定级主管审批
                    {
                        var tmporgDAL = _provider.GetService<DAL.OrgDAL>();
                        var firstDept = await tmporgDAL.SelectDeptByUid(_context.Creator, _template.OrgId.Value);
                        if (firstDept == null)
                        {
                            throw new Exception("用户不存在主管");
                        }
                        if (node.props.leader.level == 1)
                        {
                            var tmpLeaders = await tmporgDAL.SelectDeptLeaders(firstDept.dept_id.Value);
                            foreach (var tmpuid in tmpLeaders)
                            {
                                assigners.Add(tmpuid.ToString());
                            }
                        }
                        else
                        {
                            string[] deptPath = firstDept.ancestors.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            int curlevel = deptPath.Length - node.props.leader.level;
                            if (curlevel < 0)
                            {
                                throw new Exception($"01用户的{node.props.leader.level}级主管不存在");
                            }
                            var nndept = await tmporgDAL.SelectById(Convert.ToInt64(deptPath[curlevel]));
                            if (nndept == null)
                            {
                                throw new Exception($"02用户的{node.props.leader.level}级主管不存在");
                            }
                            var tmpLeaders = await tmporgDAL.SelectDeptLeaders(nndept.dept_id.Value);
                            foreach (var tmpuid in tmpLeaders)
                            {
                                assigners.Add(tmpuid.ToString());
                            }
                        }
                    }
                    break;
                case "FORM_USER":
                    //表单内联系人
                    break;
                case "ROLE":
                    //指定角色
                    var roles = node.props.role.Select(x => x.id).ToList();
                    TAAsyncHelper.RunSync(async () =>
                    {
                        var userIds = await _provider.GetService<DAL.OrgDAL>().SelectUserByRoles(roles).ConfigureAwait(false);
                        assigners = userIds.ConvertAll(x => x.ToString());
                    });
                    break;
            }

            if ((node.props.assignedType != "SELF_SELECT" || state == 2) && node.props.assignedType != "FORM_USER")
            {
                if (assigners.Count == 0)
                {
                    if (node.props.nobody.handler == "TO_PASS")
                    {
                        //自动通过
                        BuildWorkflowNode(levelnodes, node.children, stepBuilder, state);
                        return;
                    }
                    else if (node.props.nobody.handler == "TO_REFUSE")
                    {
                        //自动驳回
                        stepBuilder.Then<EndStep>();
                        return;

                    }
                    else if (node.props.nobody.handler == "TO_USER")
                    {
                        //转交到指定人员
                        assigners = node.props.nobody.assignedUser.Select(x => x.id.ToString()).ToList();
                    }
                    else
                    {
                        throw new Exception("流程模板格式错误");
                    }
                }
            }


            var tbuilder = stepBuilder.Then<ApprovalTask>(x =>
            {
                if (_template.sign == true)
                {
                    x.Step.Sign = true;
                }
                else
                {
                    x.Step.Sign = node.props.sign;
                }

                x.Step.mode = node.props.mode;
                x.Step.timeLimit = node.props.timeLimit;
                x.Step.Name = node.name;
                x.Step.FormPerms = node.props.formPerms;
                x.Step.optionInit = node.props.optionInit;
                x.Step.Id = node.id;
                if (node.props.assignedType == "FORM_USER")
                {
                    x.Step.formUser = node.props.formUser;
                }
                else
                {
                    x.Step.formUser = string.Empty;
                }
                x.Step.AssignedPrincipal = string.Join(',', assigners);
                x.Step.CanAdd = canadd;
            }).WithOption("同意", "primary").Do(then =>
            {
                then.Start<ApprovedStep>(x =>
                {
                    x.Step.Name = "同意";
                });
            }).WithOption("驳回", "danger").Do(then =>
            {
                var reject = then.Start<RejectedStep>(x =>
                {
                    x.Step.Name = "驳回";
                });
                if (node.props.refuse.type == "TO_END")
                {
                    //直接结束流程
                    reject.Then<EndStep>();
                }
                else if (node.props.refuse.type == "TO_BEFORE")
                {
                    //驳回到上级审批节点
                    var tmpnode = levelnodes.Last.Previous;
                    while (tmpnode != null)
                    {
                        if (tmpnode.Value.type == "APPROVAL")
                        {
                            break;
                        }
                        tmpnode = tmpnode.Previous;
                    }
                    if (tmpnode == null)
                    {
                        reject.Then<EndStep>();
                    }
                    else
                    {
                        reject.Then<GotoStep>(x =>
                        {
                            x.Step.TargetId = tmpnode.Value.id;
                        });
                        _undeletedSteps.Add(tmpnode.Value.id);
                    }
                }
                else if (node.props.refuse.type == "TO_NODE")
                {
                    //驳回到指定节点
                    reject.Then<GotoStep>(x =>
                    {
                        x.Step.TargetId = node.props.refuse.target;
                    });
                    _undeletedSteps.Add(node.props.refuse.target);
                }

            });

            BuildWorkflowNode(levelnodes, node.children, tbuilder, state);

        }



        /// <summary>
        /// 创建条件节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="levelnodes"></param>
        /// <param name="nodes"></param>
        /// <param name="i"></param>
        /// <param name="stepBuilder"></param>
        /// <param name="state"></param>
        private void BuildCondition<T>(LinkedList<NodeLinkItem> levelnodes, ConditionNode[] nodes, int i, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            if (i >= nodes.Length)
            {
                return;
            }
            ConditionNode node = nodes[i];
            var res = node.props.ToExpressionString(_context);
            var nextBuilder = stepBuilder.If(null, x =>
            {
                x.Step.ConditionName = "data";
                x.Step.ConditionBody = res;
                if (i == nodes.Length - 1)
                {
                    x.Step.IsLastCondition = true;
                }
            }).Do(then =>
            {
                BuildWorkflowNode(levelnodes, node.children, then.Start<NextStep>(), state);
            });
            if (res == "false")
            {
                _deletedSteps.Add(new DeletedSteps()
                {
                    Start = nextBuilder.Step.Index,
                    End = _steps.Count - 1
                });
            }
            BuildCondition(levelnodes, nodes, i + 1, nextBuilder, state);
        }
        private void BuildCS<T>(LinkedList<NodeLinkItem> levelnodes, CSNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            List<string> assigners = new List<string>();
            if (node.props.assignedUser.Length > 0)
            {
                assigners = node.props.assignedUser.Select(x => x.id.ToString()).ToList();
            }
            if (node.props.shouldAdd)
            {
                List<Out_UserItem> usrlist;
                if (_assign.TryGetValue(node.id, out usrlist))
                {
                    foreach (var s in usrlist)
                    {
                        if (!assigners.Contains(s.Id))
                        {
                            assigners.Add(s.Id);
                        }
                    }
                }
            }
            var builder = stepBuilder.Then<CSStep>(x =>
            {
                x.Step.Name = node.name;
                x.Step.Id = node.id;
                x.Step.AssignedPrincipal = string.Join(',', assigners);
                x.Step.FormPerms = node.props.formPerms;
                x.Step.ShouldAdd = node.props.shouldAdd;
            });
            BuildWorkflowNode(levelnodes, node.children, builder, state);
        }

        private void BuildTrigger<T>(LinkedList<NodeLinkItem> levelnodes, TriggerNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            var builder = stepBuilder.Then<TriggerStep>(x =>
            {
                x.Step.Name = node.name;
                x.Step.Id = node.id;
                x.Step.props = node.props;
            });
            BuildWorkflowNode(levelnodes, node.children, builder, state);
        }
        private void BuildDelay<T>(LinkedList<NodeLinkItem> levelnodes, DelayNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            var builder = stepBuilder.Then<DelayStep>(x =>
            {
                x.Step.Name = node.name;
                x.Step.Id = node.id;
                x.Step.props = node.props;
            });
            BuildWorkflowNode(levelnodes, node.children, builder, state);
        }
        private void BuildParallel<T>(LinkedList<NodeLinkItem> levelnodes, ConcurrentGroupNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            var builder = stepBuilder.Parallel(x =>
            {
                x.Step.Name = node.name;
                x.Step.Id = node.id;
            });
            foreach (var branchNode in node.branchs)
            {
                builder.Do(then =>
                {
                    BuildWorkflowNode(levelnodes, branchNode.children, then.Start<NextStep>(), state);
                });
            }
        }

        private void BuildData<T>(LinkedList<NodeLinkItem> levelnodes, DataNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            var builder = stepBuilder.Then<DataStep>(x =>
            {
                x.Step.Name = node.name;
                x.Step.Id = node.id;
                x.Step.props = node.props;
            });
            BuildWorkflowNode(levelnodes, node.children, builder, state);
        }

        private void BuildFunc<T>(LinkedList<NodeLinkItem> levelnodes, FuncNode node, StepBuilder<T> stepBuilder, int state) where T : WorkflowStep
        {
            var builder = stepBuilder.Then<FuncStep>(x =>
            {
                x.Step.Name = node.name;
                x.Step.Id = node.id;
                x.Step.props = node.props;
            });
            BuildWorkflowNode(levelnodes, node.children, builder, state);
        }
    }

    public class DeletedSteps
    {
        public int Start { get; set; }
        public int End { get; set; }
    }
}
