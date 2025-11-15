import request from '@/utils/request'
// 获取跟进记录
export function followList(params) {
    return request({
        url: '/CRMService/Follow/List',
        method: 'get',
        params: params
    })
}
// 获取记录信息
export function FollowInfo(params) {
    return request({
        url: '/CRMService/Follow/Info',
        method: 'get',
        params: params
    })
}

// 添加跟进记录
export function addFollow(data) {
    return request({
        url: '/CRMService/Follow/Add',
        method: 'post',
        data: data
    })
}
// 修改跟进记录
export function editFollow(data) {
    return request({
        url: '/CRMService/Follow/Edit',
        method: 'post',
        data: data
    })
}
// 删除跟进记录
export function delFollow(params) {
    return request({
        url: '/CRMService/Follow/Remove',
        method: 'get',
        params: params
    })
}