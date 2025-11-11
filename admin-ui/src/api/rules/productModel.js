import request from '@/utils/request'

// 获取产品分类树
export function classTree() {
    return request({
        url: '/IoTService/IotClass/ListTree',
        method: 'get',
    })
}
// 获取在指定分类信息
export function classInfo(query) {
    return request({
        url: '/IoTService/IotClass/Info',
        method: 'get',
        params: query
    })
}
// 删除分类
export function removeClass(query) {
    return request({
        url: '/IoTService/IotClass/Remove',
        method: 'get',
        params: query
    })
}

//添加产品分类
export function addClass(data) {
    return request({
        url: '/IoTService/IotClass/Add',
        method: 'post',
        data: data
    })
}
//产品分类排序
export function classSort(data) {
    return request({
        url: '/IoTService/IotClass/Sort',
        method: 'post',
        data: data
    })
}
//编辑产品分类
export function editClass(data) {
    return request({
        url: '/IoTService/IotClass/Edit',
        method: 'post',
        data: data
    })
}
//拷贝产品
export function copyProduct(id){
    return request({
        url: '/IoTService/IotProduct/Copy',
        method: 'get',
        params: {id}
    })
}
//添加产品
export function addProduct(data) {
    return request({
        url: '/IoTService/IotProduct/Add',
        method: 'post',
        data: data
    })
}
//编辑产品
export function editProduct(data) {
    return request({
        url: '/IoTService/IotProduct/Edit',
        method: 'post',
        data: data
    })
}
// 获取接入通道配置列表
export function channelList() {
    return request({
        url: '/IoTService/IotProduct/ChannelList',
        method: 'get',
    })
}
// 通过接入方式获取通道配置信息
export function channelInfo(query) {
    return request({
        url: '/IoTService/IotProduct/Channel',
        method: 'get',
        params: query
    })
}
//获取产品列表信息
export function productList(query) {
    return request({
        url: '/IoTService/IotProduct/ListPage',
        method: 'get',
        params: query
    })
}
//批量获取物模型
export function tslList(ids) {
    return request({
        url: '/IoTService/IotProduct/TSLList',
        method: 'get',
        params: {ids}
    })
}
//获取单个产品信息
export function productInfo(query) {
    return request({
        url: '/IoTService/IotProduct/Info',
        method: 'get',
        params: query
    })
}
//删除产品
export function removeProduct(query) {
    return request({
        url: '/IoTService/IotProduct/Remove',
        method: 'get',
        params: query
    })
}
//执行方法调试
export function implementFunc(data) {
    return request({
        url: '/IoTService/IotProduct/DebugFunc',
        method: 'post',
        data: data
    })
}
//执行事件调试
export function implementEvent(data) {
    return request({
        url: '/IoTService/IotProduct/DebugEvent',
        method: 'post',
        data: data
    })
}
//执行属性调试
export function implementProperty(data) {
    return request({
        url: '/IoTService/IotProduct/DebugProperty',
        method: 'post',
        data: data
    })
}
//执行Modbus调试
export function implementModbus(data) {
    return request({
        url: '/IoTService/IotProduct/DebugModbus',
        method: 'post',
        data: data
    })
}
//执行下发文本调试
export function implementText(data) {
    return request({
        url: '/IoTService/IotProduct/DebugText',
        method: 'post',
        data: data
    })
}

//执行下发特殊消息
export function implementMsg(data) {
    return request({
        url: '/IoTService/IotProduct/DebugMsg',
        method: 'post',
        data: data
    })
}

//册除指定设备的历史信息
export function implementDelHis(data){
    return request({
        url: '/IoTService/IotProduct/DebugDelHistory',
        method: 'post',
        data: data
    })
}

//标识符模板树
export function getCodeListTree(t) {
    return request({
        url: '/IoTService/IotProduct/CodeListTree',
        method: 'get',
        params: {t}
    })
}

//统计规则列表
export function getPropRuleList(id) {
    return request({
        url: '/IoTService/IotProduct/PropRuleList',
        method: 'get',
        params: {id}
    })
}

//获取统计规则信息
export function getPropRuleInfo(id) {
    return request({
        url: '/IoTService/IotProduct/PropRuleInfo',
        method: 'get',
        params: {id}
    })
}

//添加统计规则
export function addPropRule(data){
    return request({
        url: '/IoTService/IotProduct/AddPropRule',
        method: 'post',
        data: data
    })
}

//修改统计规则
export function editPropRule(data){
    return request({
        url: '/IoTService/IotProduct/EditPropRule',
        method: 'post',
        data: data
    })
}

//删除统计规则
export function removePropRule(delid){
    return request({
        url: '/IoTService/IotProduct/RemovePropRule',
        method: 'get',
        params: { ids: delid instanceof Array ? delid.join(',') : delid }
    })
}

//获取报警列表
export function getWarnList(query) {
    return request({
        url: '/IoTService/IotWarning/ListPage',
        method: 'get',
        params: query
    })
}
//获取报警信息
export function getWarnInfo(warnid) {
    return request({
        url: '/IoTService/IotWarning/Info',
        method: 'get',
        params: {id:warnid}
    })
}
//通过工单编号获取报警信息
export function getWarnInfoByNumber(number){
    return request({
        url: '/IoTService/IotWarning/InfoByNumber',
        method: 'get',
        params: {number}
    })
}

//批量清除报警列表
export function clearAllWarning(data) {
    return request({
        url: '/IoTService/IotWarning/Clear',
        method: 'post',
        data: data
    })
}

//清除单条报警列表
export function clearEachWarning(data) {
    return request({
        url: '/IoTService/IotWarning/ClearOne',
        method: 'post',
        data: data
    })
}

//保存报警配置信息
export function saveWarnConfig(data){
    return request({
        url: '/IoTService/IotWarning/SaveConfig',
        method: 'post',
        data: data
    })
}

//获取报警配置信息
export function getWarnConfig(pid){
    return request({
        url: '/IoTService/IotWarning/ConfigInfo',
        method: 'get',
        params: {pid}
    })
}