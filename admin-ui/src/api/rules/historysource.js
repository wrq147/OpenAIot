import request from '@/utils/request'

//获取历史存储列表信息
export function historySourceList(query) {
    return request({
        url: '/IoTService/IotSource/ListPage',
        method: 'get',
        params: query
    })
}


export function delHistorySource(delid) {
    return request({
        url: '/IoTService/IotSource/Remove',
        method: 'get',
        params: { ids: delid instanceof Array ? delid.join(',') : delid }
    })
}


export function historySourceInfo(id) {
    return request({
        url: '/IoTService/IotSource/Info',
        method: 'get',
        params: {id}
    })
}

export function addHistorySource(data) {
    return request({
        url: '/IoTService/IotSource/Add',
        method: 'post',
        data: data
    })
}
// 供应商编辑
export function editHistorySource(data) {
    return request({
        url: '/IoTService/IotSource/Edit',
        method: 'post',
        data: data
    })
}