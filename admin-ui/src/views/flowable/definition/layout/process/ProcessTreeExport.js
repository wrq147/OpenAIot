
let Approval = () => import('../../../common/process/nodes/ApprovalNode.vue')
let Cc = () => import('../../../common/process/nodes/CcNode.vue')
let Concurrent = () => import('../../../common/process/nodes/ConcurrentNode.vue')
let Condition = () => import('../../../common/process/nodes/ConditionNode.vue')
let Trigger = () => import('../../../common/process/nodes/TriggerNode.vue')
let Delay = () => import('../../../common/process/nodes/DelayNode.vue')
let Empty = () => import('../../../common/process/nodes/EmptyNode.vue')
let Root = () => import('../../../common/process/nodes/RootNode.vue')
let Node = () => import('../../../common/process/nodes/Node.vue')
let Dataxe = () => import('../../../common/process/nodes/DataxeNode.vue')
let User = () => import('../../../common/process/nodes/UserNode.vue')
let Func = () => import('../../../common/process/nodes/FuncNode.vue')
export default {
    Approval,
    Cc,
    Concurrent,
    Condition,
    Trigger,
    Delay,
    Empty,
    Root,
    Node,
    Dataxe,
    User,
    Func
}