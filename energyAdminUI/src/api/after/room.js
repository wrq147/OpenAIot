import request from '@/utils/request'

//移除车间设备
export function removeRoomDevice(data) {
    return request({
        url: '/AfterService/RoomDevice/Remove',
        method: 'post',
        data: data
    })
}
//添加车间设备
export function addRoomDevice(data) {
    return request({
        url: 'AfterService/RoomDevice/Add',
        method: 'post',
        data: data
    })
}
// 获取车间分类树
export function roomCatetoryTree(query) {
    return request({
        url: '/AfterService/RoomCategory/ListTree',
        method: 'get',
        params: query
    })
}
// 获取在指定车间分类信息
export function roomCatetoryInfo(query) {
    return request({
        url: '/AfterService/RoomCategory/Info',
        method: 'get',
        params: query
    })
}
// 删除车间分类
export function removeRoomCatetory(query) {
    return request({
        url: '/AfterService/RoomCategory/Remove',
        method: 'get',
        params: query
    })
}

//添加车间分类
export function addRoomCatetory(data) {
    return request({
        url: '/AfterService/RoomCategory/Add',
        method: 'post',
        data: data
    })
}

//编辑车间分类
export function editRoomCatetory(data) {
    return request({
        url: '/AfterService/RoomCategory/Edit',
        method: 'post',
        data: data
    })
}
// 获取车间
export function deviceRoomList(query) {
    return request({
        url: '/AfterService/Room/List',
        method: 'get',
        params: query
    })
}
// 获取在指定车间
export function deviceRoomInfo(query) {
    return request({
        url: '/AfterService/Room/Info',
        method: 'get',
        params: query
    })
}
// 删除车间
export function removeDeviceRoom(query) {
    return request({
        url: '/AfterService/Room/Remove',
        method: 'get',
        params: query
    })
}

//添加车间
export function addDeviceRoom(data) {
    return request({
        url: '/AfterService/Room/Add',
        method: 'post',
        data: data
    })
}

//编辑车间
export function editDeviceRoom(data) {
    return request({
        url: '/AfterService/Room/Edit',
        method: 'post',
        data: data
    })
}