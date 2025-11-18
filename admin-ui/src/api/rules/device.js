import request from '@/utils/request'
import download from '@/plugins/download'

//添加设备
export function addDevice(data) {
    return request({
        url: '/IoTService/IotDevice/Add',
        method: 'post',
        data: data
    })
}
//编辑设备
export function editDevice(data) {
    return request({
        url: '/IoTService/IotDevice/Edit',
        method: 'post',
        data: data
    })
}
//手动升级设备
export function manualUpdate(data) {
    return request({
        url: '/IoTService/IotUpdate/DeviceUp',
        method: 'post',
        data: data
    })
}
//清除升级失败的
export function clearError() {
    return request({
        url: '/IoTService/IotUpdate/ClearError',
        method: 'post'
    })
}

//获取设备列表信息
export function DeviceList(query) {
    return request({
        url: '/IoTService/IotDevice/ListPage',
        method: 'get',
        params: query
    })
}

//获取升级列表信息
export function updateList(query) {
    return request({
        url: '/IoTService/IotUpdate/ListPage',
        method: 'get',
        params: query
    })
}
//获取指定设备的信息
export function DeviceInfo(query) {
    return request({
        url: '/IoTService/IotDevice/Info',
        method: 'get',
        params: query
    })
}
//获取指定设备的实时数据
export function DeviceLiveInfo(query) {
    return request({
        url: '/IoTService/IotDevice/Live',
        method: 'get',
        params: query
    })
}
//获取指定设备的历史数据
export function DeviceOldInfo(query) {
    return request({
        url: '/IoTService/IotDevice/SelectHistory',
        method: 'get',
        params: query
    })
}
//删除历史数据
export function iotDeviceDelHistory(data) {
    return request({
        url: '/IoTService/IotDevice/DelHistory',
        method: 'post',
        data: data
    })
}

//获取指定设备离在线的历史数据
export function DeviceOnLineOldInfo(query) {
    return request({
        url: '/IoTService/IotDevice/SelectOnlines',
        method: 'get',
        params: query
    })
}
//获取指定设备的异常数据
export function abnormalDataInfo(query) {
    return request({
        url: '/IoTService/IotDevice/SelectExcepts',
        method: 'get',
        params: query
    })
}
//删除设备
export function removeDevice(query) {
    return request({
        url: '/IoTService/IotDevice/Remove',
        method: 'get',
        params: query
    })
}

//编辑设备标签
export function saveDeviceTag(data) {
    return request({
        url: '/IoTService/IotDevice/SaveTags',
        method: 'post',
        data: data
    })
}

//编辑设备属性
export function saveDeviceProp(data) {
    return request({
        url: '/IoTService/IotDevice/SaveProps',
        method: 'post',
        data: data
    })
}
//获取设备标签信息
export function DeviceTagList(query) {
    return request({
        url: '/IoTService/IotDevice/TagList',
        method: 'get',
        params: query
    })
}
//添加设备时获取设备对应协议标签信息
export function productTagList(query) {
    return request({
        url: '/IoTService/IotDevice/TagListByProduct',
        method: 'get',
        params: query
    })
}

//生成设备的设备编号
export function GenerateDeviceNumber() {
    return request({
        url: '/IoTService/IotDevice/GenerateNumber',
        method: 'get'
    })
}

//执行设备功能
export function deviceExeFunc(data) {
    return request({
        url: '/IoTService/IotDevice/ExeFunc',
        method: 'post',
        data: data
    })
}
//执行设备功能列表
export function deviceFuncList(query) {
    return request({
        url: '/IoTService/IotDevice/FuncList',
        method: 'get',
        params: query
    })
}

// 下载设备导入模板
export function exportemplate() {
    return download.resource('/IoTService/IotDevice/ExportTemplate', {});
}


//保存物联网的配置信息
export function saveConfig(data) {
    return request({
        url: '/IoTService/IotConfig/Update',
        method: 'post',
        data: data
    })
}

//获取物联网的配置信息
export function getConfig() {
    return request({
        url: '/IoTService/IotConfig/Info',
        method: 'get'
    })
}
//获取物联网卡充值列表
export function iotCardList(query) {
    return request({
        url: '/IoTService/IotCard/ListPage',
        method: 'get',
        params: query
    })
}

//物联网卡解绑
export function iotCardUnbind(data) {
    return request({
        url: '/IoTService/IotCard/UnUsing',
        method: 'post',
        data: data
    })
}
//物联网卡绑定设备
export function iotCardUsingDevice(data) {
    return request({
        url: '/IoTService/IotCard/UsingDevice',
        method: 'post',
        data: data
    })
}
//物联网卡删除
export function iotCardRemove(query) {
    return request({
        url: '/IoTService/IotCard/Remove',
        method: 'get',
        params: query
    })
}
//物联网卡续费
export function iotCardRecharge(query) {
    return request({
        url: '/IoTService/IotCard/Recharge',
        method: 'get',
        params: query
    })
}
//指定物联网卡信息
export function iotCardInfo(query) {
    return request({
        url: '/IoTService/IotCard/Info',
        method: 'get',
        params: query
    })
}
//手动停卡
export function iotStopCard(query) {
    return request({
        url: '/IoTService/IotCard/StopCard',
        method: 'get',
        params: query
    })
}

//物联网卡立即同步
export function iotCardSyncCard(query) {
    return request({
        url: '/IoTService/IotCard/SyncCard',
        method: 'get',
        params: query
    })
}
// 下载物联网卡充值导入模板
export function exportemplateCard() {
    return download.resource('/IoTService/IotCard/ExportTemplate', {});
}


//设备运行状态
export function devRunStatisticsInfo(query) {
    return request({
        url: '/IoTService/IotReport/RunStatisticsInfo',
        method: 'get',
        params: query
    });
}