export const API_PARAMS = {
  // 获取物联实时数据
  Api01: {
    name: '获取物联实时数据',
    url: '/IoTRulesService/HttpRule/Live',
    method: 'GET',
    params: {
      id: { 
        label: '目标设备', 
        type: 'device-select', 
        required: true 
      },
      needTag: { 
        label: '标签属性', 
        type: 'radio', 
        options: [
          { label: true, text: '显示' }, 
          { label: false, text: '不显示' }
        ] 
      },
      needSend: { 
        label: '同步显示', 
        type: 'radio', 
        options: [
          { label: true, text: '同步' }, 
          { label: false, text: '不同步' }
        ] 
      }
    }
  },
  
  // 查询所有设备分组
  Api02: {
    name: '查询所有设备分组',
    url: '/IoTRulesService/HttpRule/SelectGroups',
    method: 'GET',
    params: {}
  },
  
  // 获取产品名称列表
  Api04: {
    name: '获取产品名称列表',
    url: '/IoTService/HttpSync/ProductNames',
    method: 'GET',
    params: {
      pageNum: { 
        label: '当前页号', 
        type: 'number', 
        defaultValue: 1 
      },
      pageSize: { 
        label: '分页大小', 
        type: 'number', 
        defaultValue: 30 
      },
      showTotal: { 
        label: '显示总数', 
        type: 'radio', 
        options: [
          { label: true, text: '显示' }, 
          { label: false, text: '不显示' }
        ] 
      }
    }
  },
  
  // 查询设备列表
  Api05: {
    name: '查询设备列表',
    url: '/IoTService/HttpSync/ListPage',
    method: 'GET',
    params: {
      Online: { 
        label: '联网状态', 
        type: 'radio', 
        options: [
          { label: "", text: '全部' }, 
          { label: "0", text: '离线' }, 
          { label: "1", text: '在线' }, 
          { label: "2", text: '未知' }
        ] 
      },
      pageNum: { 
        label: '当前页号', 
        type: 'number', 
        defaultValue: 1 
      },
      pageSize: { 
        label: '分页大小', 
        type: 'number', 
        defaultValue: 30 
      },
      showTotal: { 
        label: '显示总数', 
        type: 'radio', 
        options: [
          { label: true, text: '显示' }, 
          { label: false, text: '不显示' }
        ] 
      }
    }
  },
  
  // 获取指定设备的实时属性数据
  Api06: {
    name: '获取指定设备的实时属性数据',
    url: '/IoTRulesService/HttpRule/Live',
    method: 'GET',
    params: {
      id: { 
        label: '目标设备', 
        type: 'device-select', 
        required: true 
      },
      needTag: { 
        label: '标签属性', 
        type: 'radio', 
        options: [
          { label: true, text: '显示' }, 
          { label: false, text: '不显示' }
        ] 
      },
      needSend: { 
        label: '同步显示', 
        type: 'radio', 
        options: [
          { label: true, text: '同步' }, 
          { label: false, text: '不同步' }
        ] 
      },
      needWait: { 
        label: '等待数据', 
        type: 'radio', 
        options: [
          { label: true, text: '是' }, 
          { label: false, text: '否' }
        ] 
      }
    }
  },
  
  // 查询设备历史数据
  Api07: {
    name: '查询设备历史数据',
    url: '/IoTRulesService/HttpRule/SelectHistory',
    method: 'GET',
    params: {
      Number: { 
        label: '目标设备', 
        type: 'device-select', 
        required: true 
      },
      Code: { 
        label: '属性标识', 
        type: 'prop-select', 
        required: true 
      },
      dateRange: { 
        label: '时间范围', 
        type: 'date-range' 
      },
      pageNum: { 
        label: '当前页号', 
        type: 'number', 
        defaultValue: 0 
      },
      pageSize: { 
        label: '分页大小', 
        type: 'number', 
        defaultValue: 0 
      },
      showTotal: { 
        label: '显示总数', 
        type: 'radio', 
        options: [
          { label: true, text: '显示' }, 
          { label: false, text: '不显示' }
        ] 
      }
    }
  },
  
  // 查询规则列表
  Api08: {
    name: '查询规则列表',
    url: '/IoTRulesService/HttpRule/RuleListPage',
    method: 'GET',
    params: {
      key: { 
        label: '搜索关键词', 
        type: 'input' 
      },
      way: { 
        label: '触发方式', 
        type: 'radio', 
        options: [
          { label: null, text: '全部' }, 
          { label: "0", text: '订阅' }, 
          { label: "1", text: 'Http' }, 
          { label: "2", text: '定时' }
        ] 
      },
      status: { 
        label: '状态', 
        type: 'radio', 
        options: [
          { label: null, text: '全部' }, 
          { label: "0", text: '正常' }, 
          { label: "1", text: '暂停' }
        ] 
      },
      from: { 
        label: '创建源', 
        type: 'radio', 
        options: [
          { label: null, text: '全部' }, 
          { label: "pc", text: 'pc' }, 
          { label: "mobile", text: 'mobile' }
        ] 
      },
      dateRange: { 
        label: '时间范围', 
        type: 'date-range' 
      },
      pageNum: { 
        label: '当前页号', 
        type: 'number', 
        defaultValue: 1 
      },
      pageSize: { 
        label: '分页大小', 
        type: 'number', 
        defaultValue: 30 
      },
      showTotal: { 
        label: '显示总数', 
        type: 'radio', 
        options: [
          { label: true, text: '显示' }, 
          { label: false, text: '不显示' }
        ] 
      }
    }
  },
  
  // 查询规则模板详情
  Api09: {
    name: '查询规则模板详情',
    url: '/IoTRulesService/HttpRule/RuleInfo',
    method: 'GET',
    params: {
      id: { 
        label: '目标规则', 
        type: 'rules-select', 
        required: true 
      }
    }
  },
  
  // 按房间查询实时数据
  Api10: {
    name: '按房间查询实时数据',
    url: '/AfterService/HttpSync/RoomLive',
    method: 'GET',
    params: {
      roomId: { 
        label: '目标车间', 
        type: 'room-select', 
        required: true 
      },
      needTag: { 
        label: '标签属性', 
        type: 'radio', 
        options: [
          { label: true, text: '显示' }, 
          { label: false, text: '不显示' }
        ] 
      },
      needSend: { 
        label: '同步显示', 
        type: 'radio', 
        options: [
          { label: true, text: '同步' }, 
          { label: false, text: '不同步' }
        ] 
      },
      needWait: { 
        label: '等待数据', 
        type: 'radio', 
        options: [
          { label: true, text: '是' }, 
          { label: false, text: '否' }
        ] 
      }
    }
  },
  
  // 获取设备的标签列表
  Api11: {
    name: '获取设备的标签列表',
    url: '/IoTService/HttpSync/TagList',
    method: 'GET',
    params: {
      number: { 
        label: '目标设备', 
        type: 'device-select', 
        required: true 
      }
    }
  },
  
  // 通过第三方编号查询设备
  Api12: {
    name: '通过第三方编号查询设备',
    url: '/IoTService/HttpSync/DeviceByNumber',
    method: 'GET',
    params: {
      number: { 
        label: '目标设备', 
        type: 'device-select', 
        required: true 
      }
    }
  },
  
  // 计划任务汇总
  Api13: {
    name: '计划任务汇总',
    url: '/AfterService/HttpSync/PlaneTaskStatis',
    method: 'GET',
    params: {
      typeid: { 
        label: '计划类型', 
        type: 'plane-select', 
        required: true 
      }
    }
  },
  
  // 计划类型列表
  Api14: {
    name: '计划类型列表',
    url: '/AfterService/HttpSync/PlaneTaskTypeList',
    method: 'GET',
    params: {}
  },
  
  // 计划任务列表
  Api15: {
    name: '计划任务列表',
    url: '/AfterService/HttpSync/PlaneTaskStatisList',
    method: 'GET',
    params: {
      PlaneTypeId: { 
        label: '计划类型', 
        type: 'plane-select' 
      },
      dateRange: { 
        label: '时间范围', 
        type: 'date-range' 
      }
    }
  },
  
  // 查询设备历史统计数据
  Api16: {
    name: '查询设备历史统计数据',
    url: '/IoTRulesService/HttpRule/SelectMergeList',
    method: 'POST',
    params: {
      mergeWay: { 
        label: '统计方式', 
        type: 'select', 
        options: [
          { label: '最大值', value: 'max' },
          { label: '最小值', value: 'min' },
          { label: '平均值', value: 'mean' },
          { label: '合计', value: 'sum' },
          { label: '期初值', value: 'first' },
          { label: '期末值', value: 'last' }
        ],
        multiple: true,
        required: true 
      },
      windowWay: { 
        label: '展示方式', 
        type: 'select', 
        options: [
          { label: '按日', value: '0' },
          { label: '按月', value: '1' },
          { label: '按时', value: '2' },
          { label: '按分', value: '3' },
          { label: '按15分', value: '4' }
        ],
        required: true 
      },
      Numbers: { 
        label: '目标设备', 
        type: 'device-select', 
        multiple: true,
        required: true 
      },
      Code: { 
        label: '属性标识', 
        type: 'input', 
        required: true 
      },
      dateRange: { 
        label: '时间范围', 
        type: 'date-range', 
        required: true 
      }
    }
  }
};

// 接口元数据列表（用于表格展示）
export const API_LIST = [
  { label: "获取物联实时数据", value: "Api01", typename: '系统' },
  { label: "查询所有设备分组", value: "Api02", typename: '系统' },
  { label: "获取产品名称列表", value: "Api04", typename: '系统' },
  { label: "查询设备列表", value: "Api05", typename: '系统' },
  { label: "获取指定设备的实时属性数据", value: "Api06", typename: '系统' },
  { label: "查询设备历史数据", value: "Api07", typename: '系统' },
  { label: "查询规则列表", value: "Api08", typename: '系统' },
  { label: "查询规则模板详情", value: "Api09", typename: '系统' },
  { label: "按房间查询实时数据", value: "Api10", typename: '系统' },
  { label: "获取设备的标签列表", value: "Api11", typename: '系统' },
  { label: "通过第三方编号查询设备", value: "Api12", typename: '系统' },
  { label: "计划任务汇总", value: "Api13", typename: '系统' },
  { label: "计划类型列表", value: "Api14", typename: '系统' },
  { label: "计划任务列表", value: "Api15", typename: '系统' },
  { label: "查询设备历史统计数据", value: "Api16", typename: '系统' }
];