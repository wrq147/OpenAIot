import request from '@/utils/request'

// 获取商机列表
export function opportList(params) {
    return request({
        url: '/CRMService/Opportunity/List',
        method: 'get',
        params: params
    })
}
// 推进商机
export function opportForward(params) {
    return request({
        url: '/CRMService/Opportunity/forward',
        method: 'get',
        params: params
    })
}
// // 删除商机
export function opportDel(params) {
    return request({
        url: '/CRMService/Opportunity/Remove',
        method: 'get',
        params: params
    })
}
// 添加商机
export function opportAdd(data) {
    return request({
        url: '/CRMService/Opportunity/Add',
        method: 'post',
        data: data
    })
}
// 修改商机
export function opportEdit(data) {
    return request({
        url: '/CRMService/Opportunity/Edit',
        method: 'post',
        data: data
    })
}
// 生成商机编号
export function GenerateNumber() {
    return request({
        url: '/CRMService/Opportunity/GenerateNumber',
        method: 'get'
    })
}
// 销售阶段
export function periodList() {
    return request({
        url: '/CRMService/Period/List',
        method: 'get'
    })
}
// 获取商机信息
export function opportInfo(params) {
    return request({
        url: '/CRMService/Opportunity/Info',
        method: 'get',
        params: params
    })
}
//跟进记录列表
export function recordList(params) {
    return request({
        url: '/CRMService/Follow/List',
        method: 'get',
        params: params
    })
}
//添加评论
export function AddComment(data) {
    return request({
        url: '/DiscussService/Comment/Add',
        method: 'post',
        data: data
    })
}