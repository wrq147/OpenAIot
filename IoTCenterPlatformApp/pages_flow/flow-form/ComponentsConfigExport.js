export const ValueType = {
    string: 'String',
    object: 'Object',
    array: 'Array',
    number: 'Number',
    date: 'Date',
    user: 'User',
    device: 'Device',
    dept: 'Dept',
    dateRange: 'DateRange',
    boolean:'Boolean'
}

export const baseComponents = [{
    name: '布局',
    components: [{
        title: '分栏布局',
        name: 'SpanLayout',
        icon: 'el-icon-c-scale-to-original',
        value: [],
        valueType: ValueType.array,
        props: {
            items: []
        }
    }]
}, {
    name: '基础组件',
    components: [{
            title: '单行文本输入',
            name: 'TextInput',
            icon: 'el-icon-edit',
            value: '',
            valueType: ValueType.string,
            props: {
                required: false,
                enablePrint: true,
                defaultValue:''
            }
        },
        {
            title: '多行文本输入',
            name: 'TextareaInput',
            icon: 'el-icon-more-outline',
            value: '',
            valueType: ValueType.string,
            props: {
                required: false,
                enablePrint: true
            }
        },
        {
            title: '数字输入框',
            name: 'NumberInput',
            icon: 'el-icon-edit-outline',
            value: 0,
            valueType: ValueType.number,
            props: {
                required: false,
                enablePrint: true,
                min: 0,
                max: 100,
                precision: 0
            }
        },
        {
            title: '金额输入框',
            name: 'AmountInput',
            icon: 'iconfont icon-zhufangbutiezhanghu',
            value: 0,
            valueType: ValueType.number,
            props: {
                required: false,
                enablePrint: true,
                showChinese: true
            }
        },
        {
            title: '单选框',
            name: 'SelectInput',
            icon: 'el-icon-circle-check',
            value: '',
            valueType: ValueType.string,
            props: {
                required: false,
                enablePrint: true,
                expanding: false,
                options: ['选项1', '选项2']
            }
        },
        {
            title: '多选框',
            name: 'MultipleSelect',
            icon: 'iconfont icon-duoxuankuang',
            value: [],
            valueType: ValueType.array,
            props: {
                required: false,
                enablePrint: true,
                expanding: false,
                options: ['选项1', '选项2']
            }
        },
        {
            title: '日期时间点',
            name: 'DateTime',
            icon: 'el-icon-date',
            value: '',
            valueType: ValueType.date,
            props: {
                required: false,
                enablePrint: true,
                format: 'yyyy-MM-dd HH:mm',
            }
        },
        {
            title: '日期时间区间',
            name: 'DateTimeRange',
            icon: 'iconfont icon-kaoqin',
            valueType: ValueType.dateRange,
            props: {
                required: false,
                enablePrint: true,
                placeholder: ['开始时间', '结束时间'],
                format: 'yyyy-MM-dd HH:mm',
                showLength: false
            }
        },
        {
            title: '上传图片',
            name: 'ImageUpload',
            icon: 'el-icon-picture-outline',
            value: [],
            valueType: ValueType.array,
            props: {
                required: false,
                enablePrint: true,
                maxSize: 10, //图片最大大小MB
                maxNumber: 5, //最大上传数量
                enableZip: true //图片压缩后再上传
            }
        },
        {
            title: '上传附件',
            name: 'FileUpload',
            icon: 'el-icon-folder-opened',
            value: [],
            valueType: ValueType.array,
            props: {
                required: false,
                enablePrint: true,
                onlyRead: false, //是否只读，false只能在线预览，true可以下载
                maxSize: 10, //文件最大大小MB
                maxNumber: 5, //最大上传数量
                fileTypes: ["doc", "xls", "ppt", "txt", "pdf"] //限制文件上传类型
            }
        },
        {
            title: '人员选择',
            name: 'UserPicker',
            icon: 'el-icon-user',
            value: [],
            valueType: ValueType.user,
            props: {
                required: false,
                enablePrint: true,
                multiple: false
            }
        },
        {
            title: '设备选择',
            name: 'DevicPicker',
            icon: 'el-icon-setting',
            value: [],
            valueType: ValueType.device,
            props: {
                required: false, //是否必填
                enablePrint: true, //是否可打印
                limit: 1, //限制个数
                limit_product: [], //字符串数组  限制的产品
                //同步控制列表
                synclist: [{
                    code: "", //产品物模型的属性标识或标签标识
                    field_id: "" //表单字段ID
                }]
            }
        },
        {
            title: '部门选择',
            name: 'DeptPicker',
            icon: 'iconfont icon-map-site',
            value: [],
            valueType: ValueType.dept,
            props: {
                required: false,
                enablePrint: true,
                multiple: false
            }
        },
        {
            title: '说明文字',
            name: 'Description',
            icon: 'el-icon-warning-outline',
            value: '',
            valueType: ValueType.string,
            props: {
                required: false,
                enablePrint: true
            }
        },
    ]
}, {
    name: '扩展组件',
    components: [{
        title: '明细表',
        name: 'TableList',
        icon: 'el-icon-tickets',
        value: [],
        valueType: ValueType.array,
        props: {
            required: false,
            enablePrint: true,
            showBorder: true,
            rowLayout: true,
            IdxColName:[],
            showSummary: false,
            deductid: "",
            summaryColumns: [],
            summaryUnit: "元",
            maxSize: 0, //最大条数，为0则不限制
            columns: [] //列设置
        }
    },{
        title: '关联表单',
        name: 'ParamInput',
        icon: 'el-icon-link',
        value: '',
        valueType: ValueType.string,
        props: {
            required: false,
            enablePrint: false,
            formType:'',
            formId: '',
        }
    }]
}]



export default {
    baseComponents
}