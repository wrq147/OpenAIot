export const RuleLive = { //获取物联实时数据
    interfaceName: 'RuleLive',
    form: {
        id: null,
        needTag: false,
        needSend: false
    },
    formKey: ['id', 'needTag', 'needSend'], //接口查询的所有参数
    id: { //参数设置分别进行设置
        type: 'remoteSelect', //远程搜索下拉框
        key: 'Id',
        label: 'Name',
        value: 'DeviceId',
        name: 'deviceOptions', //下拉列表数组名称
        text: '目标设备',
        placeholder: '请输入设备的搜索关键词',
        changeFunc: null
    },
    needTag: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '显示' }, { label: false, text: '不显示' }],
        text: '标签属性',
    },
    needSend: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '同步' }, { label: false, text: '不同步' }],
        text: '同步显示',
    },
    isshowfilterParams: true,
    filterType: 'deviceList',
    filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Key: ""
    },
    rules: {
        id: [
            { required: true, message: '请选择目标设备', trigger: 'change' },
        ],
    }
}
export const CommonPag = { //获取产品名称列表
    interfaceName: 'CommonPag',
    form: {
        pageNum: 1,
        pageSize: 30,
        showTotal: false
    },
    formKey: ['pageNum', 'pageSize', 'showTotal'], //接口查询的所有参数
    isshowfilterParams: false,
    pageNum: {
        type: 'number', //数字类型
        min: 0,
        step: 1,
        text: '当前页号',
    },
    pageSize: {
        type: 'number',
        min: 0,
        step: 1,
        text: '分页大小',
    },
    showTotal: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '显示' }, { label: false, text: '不显示' }],
        text: '显示总数',
    },
}

export const GetDeviceList = { //查询设备列表
    interfaceName: 'GetDeviceList',
    form: { //接口查询的所有参数
        GroupId: null,
        pageNum: 1,
        pageSize: 30,
        Online: '',
        showTotal: false
    },
    formKey: ['GroupId', 'pageNum', 'pageSize', 'Online', 'showTotal'], //接口查询的所有参数
    GroupId: {
        type: 'cascader',
        name: 'groupOptions',
        value: 'Id',
        label: 'GroupName',
        children: 'Children',
        placeholder: '请输入分组的搜索关键词',
        text: '设备分组',
    },
    Online: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: "", text: '全部' }, { label: "0", text: '离线' }, { label: "1", text: '在线', hasmargin: true }, { label: "2", text: '未知', hasmargin: true }],
        text: '联网状态',
    },
    pageNum: {
        type: 'number', //数字类型
        min: 0,
        step: 1,
        text: '当前页号',
    },
    pageSize: {
        type: 'number',
        min: 0,
        step: 1,
        text: '分页大小',
    },
    showTotal: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '显示' }, { label: false, text: '不显示' }],
        text: '显示总数',
    },
    isshowfilterParams: true,
    filterType: 'deviceGroup',
    filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
    },
}
export const GetDeviceTimeData = { //获取指定设备的实时属性数据
    interfaceName: 'GetDeviceTimeData',
    form: {
        id: null, //设备通讯id
        needTag: false,
        needSend: false,
        needWait: false
    },
    formKey: ['id', 'needTag', 'needSend', 'needWait'], //接口查询的所有参数
    id: { //参数设置分别进行设置
        type: 'remoteSelect', //远程搜索下拉框
        key: 'Id',
        label: 'Name',
        value: 'DeviceId',
        name: 'deviceOptions', //下拉列表数组名称
        text: '目标设备',
        placeholder: '请输入设备的搜索关键词',
        changeFunc: null
    },
    needTag: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '显示' }, { label: false, text: '不显示' }],
        text: '标签属性',
    },
    needSend: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '同步' }, { label: false, text: '不同步' }],
        text: '同步显示',
    },
    needWait: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '是' }, { label: false, text: '否' }],
        text: '同时等待数据',
    },
    isshowfilterParams: true,
    filterType: 'deviceList',
    filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Key: ""
    },
    rules: {
        id: [
            { required: true, message: '请选择目标设备', trigger: 'blur' },
        ],
    }
}

export const GetRuleList = { //查询规则列表
    interfaceName: 'GetRuleList',
    form: { //接口查询的所有参数
        key: null,
        way: null,
        status: null,
        from: null,
        pageNum: 1,
        pageSize: 30,
        showTotal: false
    },
    formKey: ['key', 'way', 'status', 'from', 'dateRange', 'pageNum', 'pageSize', 'showTotal'], //接口查询的所有参数
    key: {
        type: 'inputText',
        text: '搜索关键词',
        placeholder: '请输入搜索关键词',
    },
    way: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: null, text: '全部' }, { label: "0", text: '订阅' }, { label: "1", text: 'Http', hasmargin: true }, { label: "2", text: '定时', hasmargin: true }],
        text: '触发方式',
    },
    status: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: null, text: '全部' }, { label: "0", text: '正常' }, { label: "1", text: '暂停', hasmargin: true }],
        text: '状态',
    },
    from: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: null, text: '全部' }, { label: "pc", text: 'pc' }, { label: "mobile", text: 'mobile', hasmargin: true }],
        text: '创建源',
    },
    pageNum: {
        type: 'number', //数字类型
        min: 0,
        step: 1,
        text: '当前页号',
    },
    pageSize: {
        type: 'number',
        min: 0,
        step: 1,
        text: '分页大小',
    },
    showTotal: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '显示' }, { label: false, text: '不显示' }],
        text: '显示总数',
    },
    dateParams: {
        isShow: true, //是否显示时间范围选择
        type: 'datetimerange',
        timeParams: ['BeginTime', 'EndTime'],
        timeText: ['开始日期', '结束日期'],
        text: '时间范围',
    },
    dateRange: {
        text: '时间范围'
    },
}

export const GetRuleListDetail = { //查询规则模板详情
    interfaceName: 'GetRuleListDetail',
    form: { //接口查询的所有参数
        id: null,
    },
    formKey: ['id'], //接口查询的所有参数
    id: {
        type: 'remoteSelect', //远程搜索下拉框
        key: 'Id',
        label: 'Name',
        value: 'Id',
        name: 'rulesOptions', //下拉列表数组名称
        text: '目标规则',
        placeholder: '请输入规则的搜索关键词',
        changeFunc: null //选择完设备后是否需要执行切换设备的方法
    },
    rules: {
        id: [
            { required: true, message: '请选择规则', trigger: 'change' },
        ]
    },
    isshowfilterParams: true,
    filterType: 'rulesList',
    filterParams: {
        pageNum: 1,
        pageSize: 30,
        key: '',
        way: null,
        status: null,
        GroupId: null,
    },
}
export const PlaneTaskStatis = { //计划任务汇总
    interfaceName: 'PlaneTaskStatis',
    form: { //接口查询的所有参数
        typeid: null,
    },
    formKey: ['typeid'], //接口查询的所有参数
    typeid: { //参数设置分别进行设置
        type: 'remoteSelect', //远程搜索下拉框
        key: 'Id',
        label: 'Name',
        value: 'Id',
        name: 'planeOptions', //下拉列表数组名称
        text: '计划类型',
        placeholder: '请输入计划类型的搜索关键词',
        changeFunc: null
    },
    isshowfilterParams: true,
    filterType: 'planeList',
    filterParams: {
        // pageNum: 1,
        pageSize: 0,
        // Key: ""
    },
    rules: {
        typeid: [
            { required: true, message: '请选择计划类型', trigger: 'change' },
        ]
    },
}
export const PlaneTaskStatisList = { //计划任务汇总
    interfaceName: 'PlaneTaskStatisList',
    form: { //接口查询的所有参数
        PlaneTypeId: null,
    },
    formKey: ['PlaneTypeId', 'dateRange'], //接口查询的所有参数
    PlaneTypeId: { //参数设置分别进行设置
        type: 'remoteSelect', //远程搜索下拉框
        key: 'Id',
        label: 'Name',
        value: 'Id',
        name: 'planeOptions', //下拉列表数组名称
        text: '计划类型',
        placeholder: '请输入计划类型的搜索关键词',
        changeFunc: null
    },
    dateParams: {
        isShow: true, //是否显示时间范围选择
        type: 'datetimerange',
        timeParams: ['CreaetStart', 'CreaetEnd'],
        timeText: ['开始日期', '结束日期'],
        text: '时间范围',
    },
    isshowfilterParams: true,
    filterType: 'planeList',
    filterParams: {
        // pageNum: 1,
        pageSize: 0,
        // Key: ""
    },
    // rules: {
    //     PlaneTypeId: [
    //         { required: true, message: '请选择计划类型', trigger: 'change' },
    //     ]
    // },
    dateRange: {
        text: '时间范围'
    },
}
export const GetDeviceTagList = { //获取设备的标签列表//通过第三方编号查询设备
    interfaceName: 'GetDeviceTagList',
    form: { //接口查询的所有参数
        number: null,
    },
    formKey: ['number'], //接口查询的所有参数
    number: { //参数设置分别进行设置
        type: 'remoteSelect', //远程搜索下拉框
        key: 'Id',
        label: 'Name',
        value: 'DeviceNumber',
        name: 'deviceOptions', //下拉列表数组名称
        text: '目标设备',
        placeholder: '请输入设备的搜索关键词',
        changeFunc: null
    },
    isshowfilterParams: true,
    filterType: 'deviceList',
    filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Key: ""
    },
    rules: {
        number: [
            { required: true, message: '请选择目标设备', trigger: 'change' },
        ],
    }
}

export const GetTimeData = { //按分组查询实时数据
    interfaceName: 'GetTimeData',
    form: { //接口查询的所有参数
        GroupId: null,
        needTag: false,
        needSend: false,
        needWait: false
    },
    formKey: ['GroupId', 'needTag', 'needSend', 'needWait'], //接口查询的所有参数
    GroupId: {
        type: 'cascader',
        name: 'groupOptions',
        value: 'Id',
        label: 'GroupName',
        children: 'Children',
        placeholder: '请输入分组的搜索关键词',
        text: '设备分组',
    },
    needTag: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '显示' }, { label: false, text: '不显示' }],
        text: '标签属性',
    },
    needSend: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '同步' }, { label: false, text: '不同步' }],
        text: '同步显示',
    },
    needWait: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '是' }, { label: false, text: '否' }],
        text: '同时等待数据',
    },
    isshowfilterParams: true,
    filterType: 'deviceGroup',
    filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
    },
    rules: {
        GroupId: [
            { required: true, message: '请选择分组', trigger: 'change' },
        ],
    }
}
export const DeviceHistory = { //查询设备历史数据
    interfaceName: 'DeviceHistory',
    form: { //接口查询的所有参数
        Number: undefined,
        Code: undefined,
        pageNum: 0,
        pageSize: 0,
        showTotal: false
    },
    formKey: ['Number', 'Code', 'dateRange', 'pageNum', 'pageSize', 'showTotal'], //接口查询的所有参数
    Number: { //参数设置分别进行设置
        type: 'remoteSelect', //远程搜索下拉框
        key: 'Id',
        label: 'Name',
        value: 'DeviceNumber',
        name: 'deviceOptions', //下拉列表数组名称
        text: '目标设备',
        placeholder: '请输入设备的搜索关键词',
        changeFunc: 'properties' //选择完设备后是否需要执行切换设备的方法
    },
    Code: {
        type: 'select', //正常下拉框
        key: 'code',
        label: 'name',
        value: 'code',
        name: 'propOptions', //下拉列表数组名称
        text: '属性标识',
        placeholder: '请选择'
    },
    pageNum: {
        type: 'number', //数字类型
        min: 0,
        step: 1,
        text: '当前页号',
    },
    pageSize: {
        type: 'number',
        min: 0,
        step: 1,
        text: '分页大小',
    },
    showTotal: {
        type: 'radio', //排列单选按钮类型
        list: [{ label: true, text: '显示' }, { label: false, text: '不显示' }],
        text: '显示总数',
    },
    dateParams: {
        isShow: true, //是否显示时间范围选择
        type: 'datetimerange',
        timeParams: ['BeginTime', 'EndTime'],
        timeText: ['开始日期', '结束日期'],
        text: '时间范围',
    },
    isshowfilterParams: true,
    filterType: 'deviceList',
    filterParams: {
        HasDeviceId: true,
        pageNum: 1,
        pageSize: 30,
        Key: ""
    },
    dateRange: {
        text: '时间范围'
    },
    rules: {
        Number: [
            { required: true, message: '请选择设备', trigger: 'change' },
        ],
        Code: [
            { required: true, message: '请选择属性标识', trigger: 'change' },
        ],
    }
}