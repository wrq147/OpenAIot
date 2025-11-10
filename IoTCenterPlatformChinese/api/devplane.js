import request from '@/common/request.js'
// 设备计划列表
export function devPlaneList(query) {
    return request.get({
        url: '/AfterService/DevPlane/List',
        query: query
    })
}
//设备流程列表
export function devFlowList(query) {
    return request.get({
        url: '/AfterService/DevPlane/DeviceFlowList',
        query: query
    })
}

// 指定设备计划详情
export function devPlaneInfo(query) {
    return request.get({
        url: '/AfterService/DevPlane/Info',
        query: query
    })
}
// 设备计划任务列表
export function devPlaneTaskList(query) {
    return request.get({
        url: '/AfterService/DevPlaneTask/List',
        query: query
    })
}
// 设备计划任务编号生成
export function devPlaneTaskNumber() {
    return request.get({
        url: '/AfterService/DevPlaneTask/GeneratePlaneNumber',
    })
}
//添加设备计划任务
export function devPlaneTaskAdd(data) {
    return request.post({
        url: '/AfterService/DevPlaneTask/Add',
        data: data
    })
}
// 生成流程初始化表单
export function taskFormData(query) {
    return request.get({
        url: '/AfterService/DevPlaneTask/FormData',
        query: query
    })
}
//添加设备计划任务
export function dayTaskList(query) {
    return request.get({
        url: '/AfterService/DevPlaneTask/DayList',
        query: query
    })
}
//废弃设备计划任务
export function cancelTask(query) {
    return request.get({
        url: '/AfterService/DevPlaneTask/Cancel',
        query: query
    })
}
//设备计划任务详情
export function devPlaneTaskInfo(query) {
    return request.get({
        url: '/AfterService/DevPlaneTask/Info',
        query: query
    })
}
