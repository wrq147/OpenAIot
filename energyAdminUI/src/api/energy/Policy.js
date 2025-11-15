import request from '@/utils/request'
// 计费标准列表
export function policyPageList(data) {
    return request({
        url: '/EfficiencyService/Policy/List',
        method: 'post',
        data: data
    })
}
// 添加计费标准
export function addPolicy(data) {
    return request({
        url: '/EfficiencyService/Policy/AddPolicy',
        method: 'post',
        data: data
    })
}
// 修改计费标准
export function editPolicy(data) {
    return request({
        url: '/EfficiencyService/Policy/EditPolicy',
        method: 'post',
        data: data
    })
}
// 删除计费标准
export function removePolicy(data) {
    return request({
        url: '/EfficiencyService/Policy/RemovePolicy',
        method: 'post',
        data: data
    })
}
// 计费详情
export function policyInfo(id) {
    return request({
        url: '/EfficiencyService/Policy/Info?id=' + id,
        method: 'post',
        // data: data
    })
}