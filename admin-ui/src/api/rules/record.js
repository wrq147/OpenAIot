import request from '@/utils/request'

export function recordList(query) {
    return request({
        url: '/IoTVideoService/Record/ListPage',
        method: 'get',
        params: query
    })
}

export function recordLogList(query){
    return request({
        url: '/IoTVideoService/Record/LogListPage',
        method: 'get',
        params: query
    })
}

export function recordInfo(id) {
    return request({
        url: '/IoTVideoService/Record/Info',
        method: 'get',
        params: {id}
    })
}

export function addRecord(data) {
    return request({
        url: '/IoTVideoService/Record/Add',
        method: 'post',
        data: data
    })
}

export function editRecord(data) {
    return request({
        url: '/IoTVideoService/Record/Edit',
        method: 'post',
        data: data
    })
}

export function removeRecord(id) {
    return request({
        url: '/IoTVideoService/Record/Remove',
        method: 'get',
        params: {id}
    })
}