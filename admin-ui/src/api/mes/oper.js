import request from '@/utils/request'

// 生产工序列表
export function operList(data) {
    return request({
        url: '/MESService/Oper/List',
        method: 'post',
        data
    })
}

// 生产工序详情
export function operInfo(params) {
    return request({
        url: '/MESService/Oper/Info',
        method: 'get',
        params
    })
}
export function routeOperInfo(params) {
    return request({
        url: '/MESService/Oper/RouteInfo',
        method: 'get',
        params
    })
}

// 添加生产工序
export function operAdd(data) {
    return request({
        url: '/MESService/Oper/Add',
        method: 'post',
        data: data
    })
}

// 修改生产工序
export function operEdit(data) {
    return request({
        url: '/MESService/Oper/Edit',
        method: 'post',
        data: data
    })
}

// 删除生产工序
export function operRemove(params) {
    return request({
        url: '/MESService/Oper/Remove',
        method: 'get',
        params: params
    })
}