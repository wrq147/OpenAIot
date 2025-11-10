import request from '@/common/request.js'
// 获取设备列表
export function crmDeviceList(query) {
    return request.get({
        url: '/CRMService/Dev/List',
        data: query
    })
}
//设备基本信息
export function crmDeviceInfo(query) {
    return request.get({
        url: '/IoTService/IotDevice/Info',
        query: query
    })
}
//设备标签信息
export function crmDeviceTagInfo(query) {
    return request.get({
        url: '/IoTService/IotDevice/TagList',
        query: query
    })
}
//设备产品信息
export function crmDeviceProductInfo(query) {
    return request.get({
        url: '/IoTService/IotProduct/Info',
        query: query
    })
}
//获取设备功能列表
export function deviceFuncList(query) {
    return request.get({
        url: '/IoTService/IotDevice/FuncList',
        query: query
    })
}
 //执行设备功能
 export function deviceExeFunc(data) {
     return request.post({
         url: '/IoTService/IotDevice/ExeFunc',
         data: data
     })
 }
 //获取设备实时数据
 export function DeviceLiveInfo(query) {
     return request.get({
         url: '/IoTService/IotDevice/Live',
		 query:query
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
 //代理商扫码添加设备
 export function addUseDevice(id) {
     return request.get({
         url: '/IoTService/IotDevice/UseDevice?t=1&id='+id,
     })
 }
 
// 查询首页设备信息
export function deviceStatistics(query) {
    return request.get({
        url: '/IoTService/IotReport/StatisticsInfo',
        query: query
    })
}
//获取报警列表
export function warningList(query) {
    return request.get({
        url: '/IoTService/IotWarning/ListPage',
        query: query
    })
}

//批量清除报警列表
export function clearAllWarning(data) {
    return request.post({
        url: '/IoTService/IotWarning/Clear',
        data: data
    })
}
//修改设备信息
export function editDevice(data) {
    return request.post({
        url: '/CRMService/Dev/UpdateDevice',
        data: data
    })
}
//清除单条报警列表
export function clearEachWarning(data) {
    return request.post({
        url: '/IoTService/IotWarning/ClearOne',
        data: data
    })
}
//获取库存统计信息
export function StockStatisticsInfo(query) {
    return request.get({
        url: '/CRMService/CrmReport/StockInfo',
        query: query
    })
}
//获取库存出入口记录
export function StockDetailRecords(query) {
    return request.get({
        url: '/CRMService/CrmReport/DetailRecords',
        query: query
    })
}
//获取本人操作日志
export function personOperLogList(query) {
    return request.get({
        url: 'MonitorService/OperLog/PersonList',
        query: query
    })
}
//解绑设备
export function UnUseDevice(query) {
    return request.get({
        url: '/IoTService/IotDevice/UnUseDevice',
        query: query
    })
}
//使用通讯id获取设备信息
export function InfoOfDtuId(query) {
    return request.get({
        url: '/IoTService/IotDevice/InfoOfDtuId',
        query: query
    })
}
//获取已经升级了的设备
export function getUpdate(query) {
    return request.get({
        url: '/AuthService/Code/GetByKey?id=OldDtuUp',
        query: query
    })
}
