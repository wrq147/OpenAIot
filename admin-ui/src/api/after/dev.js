import request from '@/utils/request'

// 设备列表
export function myDeviceList(params) {
    return request({
        url: '/AfterService/Dev/List',
        method: 'get',
        params: params
    })
}
//产品列表
export function myProductList(params) {
    return request({
        url: '/AfterService/Dev/ProductList',
        method: 'get',
        params: params
    })
}
//设备列表
export function planeDevList(params) {
    return request({
        url: '/AfterService/DevPlane/DevList',
        method: 'get',
        params: params
    })
}
//设备统计分析数据
export function devSelectMergeList(query) {
    return request({
        url: '/IoTService/IotDevice/SelectMergeList',
        method: 'post',
        data: query
    });
}
//地图设备分布列表
export function devMapRangeList(params) {
    return request({
        url: '/AfterService/DevMap/RangeList',
        method: 'get',
        params: params
    })
}
//地图区域设备统计
export function devRangeAreaList(params) {
    return request({
        url: '/AfterService/DevMap/RangeAreaList',
        method: 'get',
        params: params
    })
}
//地图设备统计
export function devMapList(params) {
    return request({
        url: '/AfterService/DevMap/List',
        method: 'get',
        params: params
    })
}
//设备离在线信息
export function devMapInfo(params) {
    return request({
        url: '/AfterService/DevMap/Info',
        method: 'get',
        params: params
    })
}