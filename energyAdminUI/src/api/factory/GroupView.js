import request from '@/utils/request'
// 公共分组列表
export function groupViewListGet(params) {
    return request({
        url: '/AuthService/GroupView/List',
        method: 'get',
        params: params
    })
}
// 公共分组详情
export function groupViewInfo(params) {
    return request({
        url: '/AuthService/GroupView/Info',
        method: 'get',
        params: params
    })
}
// 删除公共分组
export function groupViewRemove(params) {
    return request({
        url: '/AuthService/GroupView/Remove',
        method: 'get',
        params: params
    })
}

// 公共分组排序
export function setGroupViewSort(data) {
    return request({
        url: '/AuthService/GroupView/Sort',
        method: 'post',
        data: data
    })
}
// 公共分组添加
export function addGroupViewSave(data) {
    return request({
        url: '/AuthService/GroupView/Add',
        method: 'post',
        data: data
    })
}
// 公共分组编辑
export function editGroupViewSave(data) {
    return request({
        url: '/AuthService/GroupView/Edit',
        method: 'post',
        data: data
    })
}