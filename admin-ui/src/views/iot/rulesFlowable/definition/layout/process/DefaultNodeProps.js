//根节点默认属性
export const ROOT_PROPS = {
  assignedUser: [],
  formPerms: [],
};

//条件节点默认属性
export const CONDITION_PROPS = {
  groupsType: "OR", //条件组逻辑关系 OR、AND
  groups: [
    {
      groupType: "AND", //条件组内条件关系 OR、AND
      cids: [], //条件ID集合
      conditions: [], //组内子条件
    },
  ],
  expression: "", //自定义表达式，灵活构建逻辑关系
};
//数据转换属性
export const CONVERSION_PROPS = {
  func: `
    /**
     * 输入转换
     * @param {JsContext} context
     * @return Object
     */
    function handler(context){
        context.Next(context.Input());
    }`, //自定义脚本
};

//服务端修改设备属性
export const DATAWRITE_PROPS = {
  Writes: [], //写入数据
  Counts: [], //数据统计
};

//聚合数据属性
export const METRONOME_PROPS = {
  Way: 0, // 计数方式：0为次数、1为秒计数
  FieldName: "", //聚合的字段
  Count: 15, // 计数值
  CountLen: 0, //事件触发后保留的数据量，默认为0
};

//触发事件属性
export const MESSAGE_PROPS = {
  TargetType: null, // 目标类型：0为当前设备、1为选择设备
  TargetId: "", // 目标设备Id
  EventId: "", //执行的报警事件,取事件定义的code
};

//功能节点属性
export const FUNC_PROPS = {
  TargetType: null, // 目标类型：0为当前设备、1为选择设备
  TargetId: "", // 目标设备Id
  FunctionId: "", //执行的功能事件,取功能定义的code
  EventInput: false, //是否输入数据
  InputData: {}, //EventInput为false时，执行功能传的值
  ReturnOutput: false, //是否输出数据
};

//触发器节点默认属性
export const TRIGGER_PROPS = {
  method: "GET", //请求方法 支持GET/POST
  url: "", //URL地址，可以直接带参数
  headers: [],
  contentType: "FORM", //请求参数类型
  xparams: [],
  rawString: "",
  okTxt: "",
};

//延时节点默认属性
export const DELAY_PROPS = {
  type: "FIXED", //延时类型 FIXED:到达当前节点后延时固定时长 、AUTO:延时到 dateTime设置的时间
  time: 0, //延时时间
  unit: "M", //时间单位 D天 H小时 M分钟
  dateTime: "", //如果当天没有超过设置的此时间点，就延时到这个指定的时间，到了就直接跳过不延时
};

//异常检测节点默认属性
export const EXCEPT_PROPS = {
  CountId: "", //聚合数据Id
  ExceptType: "spike", //异常检测类型：峰值spike、更改change
  Confidence: 90, //[0， 100] 范围内的峰值检测置信度
  Min: 0,
  Max: 100,
};

//转发节点默认属性
export const REDIRECT_PROPS = {
  ProductId: "", //协议id
  TargetDtuIdsList: [], //转发后的设备通信Id
  Maping: "", //格式是:{"原标识符":"目标标识符","原标识符":"目标标识符"}，只有属性消息和事件消息有
};

//数据清除节点默认属性
export const CLEARDELTA_PROPS = {
  ClearType: null,
  Codes: [],
};

//设备调度器节点默认属性
export const SCHEDULER_PROPS = {
  DeviceList: [], //设备列表
  ScheAmount: 1, //调度数量
  DeviceSets: [], //设备列表配置
};
//设备调度器节点默认属性
export const PID_PROPS = {
  TargetType: null, //参考设备类型，0为当前设备，1表示选择设备
  refdevid: "", // 参考的设备id或选择当前设备
  refprop: "", // 参考的设备属性（浮点）
  targetval: "", // 目标值参数（浮点）
  pname: "", // 比例参数（浮点，一般在 10～50 之间）
  iname: "", // 积分时间（浮点，通常在 30～100 秒之间）
  dname: "", // 微分时间（浮点，一般在 5～20 秒之间）
  DevControllerItem: [], // 控制的设备列表{id:设备Id或选择当前设备 maxval:最大控制量参数 minval:最小控制量参数 code:执行的功能 codeval:功能的控制量参数 }
};
//触发通知节点默认属性
export const NOTICE_PROPS = {
  NoticeWay: 'APP', //通知方式:APP站内通知,EMAIL邮件通知,SMS短信通知//APP、EMAIL、SMS
  Title:'', //推送的内容（长度不超过50字符）
  TargetValue:"",//邮件通知时传固定邮箱，短信通知时传固定手机号，APP站内通知时传用户Id
  userName:"",//APP站内通知时用户名称保存一下
  userAvatar:"",//APP站内通知时用户头像保存一下
};

//标签赋值节点默认属性
export const TAG_PROPS = {
  TargetType: null, //目标类型：0为当前设备、1为选择设备
  TargetId: "", // 目标设备Id
  TagId: "", //标签标识
  Express: "", //赋值表达式
};

export const SETPROP_PROPS = {
  PropCode: "", //属性标识
  Express: "", //赋值表达式
};
export default {
  DELAY_PROPS,
  CONDITION_PROPS,
  ROOT_PROPS,
  TRIGGER_PROPS,
  CONVERSION_PROPS,
  DATAWRITE_PROPS,
  METRONOME_PROPS,
  MESSAGE_PROPS,
  FUNC_PROPS,
  EXCEPT_PROPS,
  REDIRECT_PROPS,
  CLEARDELTA_PROPS,
  TAG_PROPS,
  SCHEDULER_PROPS,
  PID_PROPS,
  SETPROP_PROPS,
  NOTICE_PROPS,
};
