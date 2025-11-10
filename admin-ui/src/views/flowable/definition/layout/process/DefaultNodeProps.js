//审批节点默认属性
export const APPROVAL_PROPS = {
    assignedType: "ASSIGN_USER",
    mode: "AND",
    sign: false,
    nobody: {
        handler: "TO_PASS",
        assignedUser: []
    },
    timeLimit: {
        timeout: {
            unit: "H",
            value: 0
        },
        handler: {
            type: "REFUSE",
            notify: {
                once: true,
                hour: 1
            }
        }
    },
    assignedUser: [],
    formPerms: [],
    selfSelect: {
        multiple: false
    },
    leaderTop: {
        endCondition: "TOP",
        endLevel: 1,
    },
    leader: {
        level: 1
    },
    role: [],
    refuse: {
        type: 'TO_END', //驳回规则 TO_END  TO_NODE  TO_BEFORE
        target: '' //驳回到指定ID的节点
    },
    formUser: '',
    OptionInitItem: [] //节点操作默认赋值初始化
}

//根节点默认属性
export const ROOT_PROPS = {
    assignedUser: [],
    formPerms: []
}

//条件节点默认属性
export const CONDITION_PROPS = {
    groupsType: "OR", //条件组逻辑关系 OR、AND
    groups: [{
        groupType: "AND", //条件组内条件关系 OR、AND
        cids: [], //条件ID集合
        conditions: [] //组内子条件
    }],
    expression: "" //自定义表达式，灵活构建逻辑关系
}

//抄送节点默认属性
export const CC_PROPS = {
    shouldAdd: false,
    assignedUser: [],
    formPerms: []
}

//触发器节点默认属性
export const TRIGGER_PROPS = {
    type: 'WEBHOOK',
    http: {
        method: 'GET', //请求方法 支持GET/POST
        url: '', //URL地址，可以直接带参数
        headers: [ //http header
            {
                name: '',
                isField: true,
                value: '' //支持表达式 ${xxx} xxx为表单字段名称
            }
        ],
        contentType: 'FORM', //请求参数类型
        xparams: [ //请求参数
            {
                name: '',
                isField: true, //是表单字段还是自定义
                value: '' //支持表达式 ${xxx} xxx为表单字段名称
            }
        ],
        retry: 1,
        handlerByScript: false,
        script: 'function handlerOk(res) {\n  return true;\n}\n\nfunction handlerFail(res) {\n  return true;\n}'
    },
    email: {
        subject: '',
        to: [],
        content: ''
    },
    flow: {
        templateId: '', // 流程模板Id
        creator: '', //目标流程发起人：为空则为当前流程的发起人，传入人员表单id
        assign: [{
            NodeId: '',
            NodeName: '',
            fieldid: '',
        }], //自选人，为null则使用原流程的自选人
        items: [{ //目标流程表单初始化
            id: '', // 目标表单id
            title: '', // 目标表单名
            eltype: '', // 目标表单类型
            fieldid: '', // 值映射的表单id
        }],
        flowas: '' //目标流程的流程编码赋值给当前表单
    }
}

//延时节点默认属性
export const DELAY_PROPS = {
    type: "FIXED", //延时类型 FIXED:到达当前节点后延时固定时长 、AUTO:延时到 dateTime设置的时间
    time: 0, //延时时间
    unit: "M", //时间单位 D天 H小时 M分钟
    dateTime: "" //如果当天没有超过设置的此时间点，就延时到这个指定的时间，到了就直接跳过不延时
}


//数据执行节点默认属性
export const DATA_AC_PROPS = {
    targetform: "", //目标表单
    action: "Update", //执行的动作Update、Delete
    conditions: [], //过滤条件
    fields: [] //执行插入或更新的字段
}

export const FUNC_PROPS = {
    FieldId: "", //设备表单项
    Items: [] //执行功能项
}


export const USER_PROPS = {
    formPerms: [],
    assignedType: "ASSIGN_USER",
    assignedUser: [],
    role: [],
    formUser: '',
    formDevice: '',
    enableChange: false,
    exeTxt: "执行",
    refuseTxt: "驳回",
    timeLimit: {
        timeout: {
            unit: "H",
            value: 0
        },
        handler: {
            type: "REFUSE",
            notify: {
                once: true,
                hour: 1
            }
        }
    },
    refuse: {
        type: 'TO_END', //驳回规则 TO_END  TO_NODE  TO_BEFORE
        target: '' //驳回到指定ID的节点
    },
    OptionInitItem: [] //节点操作默认赋值初始化
}

export default {
    APPROVAL_PROPS,
    CC_PROPS,
    DELAY_PROPS,
    CONDITION_PROPS,
    ROOT_PROPS,
    TRIGGER_PROPS,
    DATA_AC_PROPS,
    USER_PROPS,
    FUNC_PROPS
}