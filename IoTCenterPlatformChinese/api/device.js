import request from '@/common/request.js'
// 获取设备列表
export function crmDeviceList(query) {
    return request.get({
        url: '/AfterService/Dev/List',
        query: query
    })
}
//设备计划设备列表
export function planeDevList(query){
    return request.get({
        url: '/AfterService/DevPlane/DevList',
        query: query
    })
}
//获取iot设备列表信息
export function iotDeviceList(query) {
    return request.get({
        url: '/IoTService/IotDevice/ListPage',
        query: query
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
    return request.get({
        url: '/IoTService/IotDevice/SelectHistory',
        query:query
    })
}
//获取指定设备离在线的历史数据
export function DeviceOnLineOldInfo(query) {
    return request.get({
        url: '/IoTService/IotDevice/SelectOnlines',
        query:query
    })
}
//获取指定设备的异常数据
export function abnormalDataInfo(query) {
    return request.get({
        url: '/IoTService/IotDevice/SelectExcepts',
        query:query
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
        url: '/AfterService/Dev/UpdateDevice',
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
        url: '/StorageService/StReport/StockInfo',
        query: query
    })
}
//获取库存出入口记录
export function StockDetailRecords(query) {
    return request.get({
        url: '/StorageService/StReport/DetailRecords',
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

//获取车间分类树
export function typeListTree(query) {
    return request.get({
        url: '/AfterService/RoomCategory/ListTree',
        query: query
    })
}
//新增车间分类
export function typeListAdd(data) {
    return request.post({
        url: '/AfterService/RoomCategory/Add',
        data: data
    })
}
//编辑车间分类
export function typeListEdit(data) {
    return request.post({
        url: '/AfterService/RoomCategory/Edit',
        data: data
    })
}
//删除车间分类
export function typeListDelete(query) {
    return request.get({
        url: '/AfterService/RoomCategory/Remove',
        query: query
    })
}
//获取车间列表
export function roomList(query) {
    return request.get({
        url: '/AfterService/Room/List',
        query: query
    })
}
//添加车间信息
export function roomListAdd(data) {
    return request.post({
        url: '/AfterService/Room/Add',
        data: data
    })
}
//编辑车间信息
export function roomListEdit(data) {
    return request.post({
        url: '/AfterService/Room/Edit',
        data: data
    })
}
//删除车间信息
export function roomListDelete(query) {
    return request.get({
        url: '/AfterService/Room/Remove',
        query: query
    })
}
//获取车间信息
export function roomListInfo(query) {
    return request.get({
        url: '/AfterService/Room/Info',
        query: query
    })
}
//删除设备与车间的关联
export function roomListRemove(data) {
    return request.post({
        url: '/AfterService/RoomDevice/Remove',
        data: data
    })
}
//关联设备与车间
export function roomListDevAdd(data) {
    return request.post({
        url: '/AfterService/RoomDevice/Add',
        data: data
    })
}