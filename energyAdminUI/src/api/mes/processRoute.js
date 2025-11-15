import request from '@/utils/request'

// 工艺路线列表
export function routeList(params) {
    return request({
        url: '/MESService/Route/List',
        method: 'get',
        params: params
    })
}

// 添加工艺路线
export function routeAdd(data) {
    return request({
        url: '/MESService/Route/Add',
        method: 'post',
        data: data
    })
}

// 修改工艺路线
export function routeEdit(data) {
    return request({
        url: '/MESService/Route/Edit',
        method: 'post',
        data: data
    })
}

// 删除工艺路线
export function routeRemove(params) {
    return request({
        url: '/MESService/Route/Remove',
        method: 'get',
        params: params
    })
}