import request from '@/utils/request'

// 获取私海客户
export function privateCustomer(params) {
    return request({
        url: 'CRMService/Customer/List',
        method: 'get',
        params: params
    })
}
// 获取公海客户
export function publicCustomer(params) {
    return request({
        url: 'CRMService/Customer/PubList',
        method: 'get',
        params: params
    })
}
// 获取客户详情
export function customerInfo(params) {
    return request({
        url: '/CRMService/Customer/Info',
        method: 'get',
        params: params
    })
}
// 生成客户编号
export function GenerateNumber() {
    return request({
        url: '/CRMService/Customer/GenerateNumber',
        method: 'get'
    })
}

// 领取客户
export function customerDraw(params) {
    return request({
        url: '/CRMService/Customer/Draw',
        method: 'get',
        params: params
    })
}
// 退回客户
export function customerReturn(params) {
    return request({
        url: '/CRMService/Customer/Return',
        method: 'get',
        params: params
    })
}
// 删除私海客户
export function privateCustomerDel(params) {
    return request({
        url: '/CRMService/Customer/Remove',
        method: 'get',
        params: params
    })
}
// 删除公海客户
export function publicCustomerDel(params) {
    return request({
        url: '/CRMService/Customer/PubRemove',
        method: 'get',
        params: params
    })
}
// 添加公海客户
export function pubAddCustomer(data) {
    return request({
        url: '/CRMService/Customer/PubAdd',
        method: 'post',
        data: data
    })
}
// 添加私海客户
export function priAddCustomer(data) {
    return request({
        url: '/CRMService/Customer/Add',
        method: 'post',
        data: data
    })
}

//修改公海客户
export function pubEditCustomer(data) {
    return request({
        url: '/CRMService/Customer/PubEdit',
        method: 'post',
        data: data
    })
}
// 修改私海客户
export function priEditCustomer(data) {
    return request({
        url: '/CRMService/Customer/Edit',
        method: 'post',
        data: data
    })
}

// 代理商生成客户或代理商邀请码
export function customerInvite(data) {
    return request({
        url: '/CRMService/Agent/AddInvite',
        method: 'post',
        data: data
    })
}
// 代理商取消客户或代理商邀请
export function customerUnInvite(params) {
    return request({
        url: '/CRMService/Customer/UnBind',
        method: 'get',
        params: params
    })
}
