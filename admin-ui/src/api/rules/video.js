import request from '@/utils/request'

export function videoSourceList(query) {
    return request({
        url: '/IoTVideoService/Source/ListPage',
        method: 'get',
        params: query
    })
}



export function addVideoSource(data) {
    return request({
        url: '/IoTVideoService/Source/Add',
        method: 'post',
        data: data
    })
}

export function editVideoSource(data) {
    return request({
        url: '/IoTVideoService/Source/Edit',
        method: 'post',
        data: data
    })
}

export function getVideoDetail(query) {
    return request({
        url: '/IoTVideoService/Source/Info',
        method: 'get',
        params: query
    })
}

export function removeVideoSource(query) {
    return request({
        url: '/IoTVideoService/Source/Remove',
        method: 'get',
        params: query
    })
}