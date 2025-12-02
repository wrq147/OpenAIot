import request from '@/utils/request'

//移除房间设备
export function removeRoomDevice(data) {
    return request({
        url: '/AfterService/RoomDevice/Remove',
        method: 'post',
        data: data
    })
}
//添加房间设备
export function addRoomDevice(data) {
    return request({
        url: 'AfterService/RoomDevice/Add',
        method: 'post',
        data: data
    })
}
// 获取房间分类树
export function roomCatetoryTree(query) {
    return request({
        url: '/AfterService/RoomCategory/ListTree',
        method: 'get',
        params: query
    })
}
// 获取在指定房间分类信息
export function roomCatetoryInfo(query) {
    return request({
        url: '/AfterService/RoomCategory/Info',
        method: 'get',
        params: query
    })
}
// 删除房间分类
export function removeRoomCatetory(query) {
    return request({
        url: '/AfterService/RoomCategory/Remove',
        method: 'get',
        params: query
    })
}

//添加房间分类
export function addRoomCatetory(data) {
    return request({
        url: '/AfterService/RoomCategory/Add',
        method: 'post',
        data: data
    })
}

//编辑房间分类
export function editRoomCatetory(data) {
    return request({
        url: '/AfterService/RoomCategory/Edit',
        method: 'post',
        data: data
    })
}
// 获取房间
export function deviceRoomList(query) {
    return request({
        url: '/AfterService/Room/List',
        method: 'get',
        params: query
    })
}
// 获取在指定房间
export function deviceRoomInfo(query) {
    return request({
        url: '/AfterService/Room/Info',
        method: 'get',
        params: query
    })
}
// 删除房间
export function removeDeviceRoom(query) {
    return request({
        url: '/AfterService/Room/Remove',
        method: 'get',
        params: query
    })
}

//添加房间
export function addDeviceRoom(data) {
    return request({
        url: '/AfterService/Room/Add',
        method: 'post',
        data: data
    })
}

//编辑房间
export function editDeviceRoom(data) {
    return request({
        url: '/AfterService/Room/Edit',
        method: 'post',
        data: data
    })
}