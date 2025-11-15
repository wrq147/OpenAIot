import request from '@/utils/request'

// 设备计划列表
export function devPlaneList(params) {
    return request({
        url: '/AfterService/DevPlane/List',
        method: 'get',
        params: params
    })
}

//添加设备计划
export function devPlaneAdd(data) {
    return request({
        url: '/AfterService/DevPlane/Add',
        method: 'post',
        data: data
    })
}

//修改设备计划
export function devPlaneEdit(data) {
    return request({
        url: '/AfterService/DevPlane/Edit',
        method: 'post',
        data: data
    })
}
// 删除指定设备计划
export function devPlaneRemove(params) {
    return request({
        url: '/AfterService/DevPlane/Remove',
        method: 'get',
        params: params
    })
}
// 指定设备计划详情
export function devPlaneInfo(params) {
    return request({
        url: '/AfterService/DevPlane/Info',
        method: 'get',
        params: params
    })
}
// 设备计划任务列表
export function devPlaneTaskList(params) {
    return request({
        url: '/AfterService/DevPlaneTask/List',
        method: 'get',
        params: params
    })
}
// 设备计划任务编号生成
export function devPlaneTaskNumber() {
    return request({
        url: '/AfterService/DevPlaneTask/GeneratePlaneNumber',
        method: 'get'
    })
}
//添加设备计划任务
export function devPlaneTaskAdd(data) {
    return request({
        url: '/AfterService/DevPlaneTask/Add',
        method: 'post',
        data: data
    })
}
// 生成流程初始化表单
export function taskFormData(params) {
    return request({
        url: '/AfterService/DevPlaneTask/FormData',
        method: 'get',
        params: params
    })
}
//添加设备计划任务
export function dayTaskList(params) {
    return request({
        url: '/AfterService/DevPlaneTask/DayList',
        method: 'get',
        params: params
    })
}
//废弃设备计划任务
export function cancelTask(params) {
    return request({
        url: '/AfterService/DevPlaneTask/Cancel',
        method: 'get',
        params: params
    })
}

//设备计划任务详情
export function devPlaneTaskInfo(params) {
    return request({
        url: '/AfterService/DevPlaneTask/Info',
        method: 'get',
        params: params
    })
}