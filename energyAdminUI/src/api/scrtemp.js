import request from '@/utils/request'
//获取模板列表
export function iotScriptList(query) {
    return request({
        url: '/IoTService/IotScript/ListPage',
        method: 'get',
        params: query
    })
}

//删除脚本
export function removeIotScriptt(query) {
    return request({
        url: '/IoTService/IotScript/Remove',
        method: 'get',
        params: query
    })
}
//获取模板信息
export function iotScriptInfo(query) {
    return request({
        url: '/IoTService/IotScript/Info',
        method: 'get',
        params: query
    })
}
// 添加脚本模板
export function addIotScript(data) {
    return request({
        url: '/IoTService/IotScript/Add',
        method: 'post',
        data: data
    })
}
// 编辑脚本模板
export function editIotScript(data) {
    return request({
        url: '/IoTService/IotScript/Edit',
        method: 'post',
        data: data
    })
}