import request from '@/common/request.js'

// 获取工厂列表
export function factoryList(data) {
    return request.get({
        url: '/CRMService/Factory/List',
        query:data
    })
}

// 添加新工厂
export function addFactory(data) {
    return request.post({
        url: '/CRMService/Factory/Add',
        data: data
    })
}

// 获取工厂信息
export function factoryInfo(id) {
    return request.get({
        url: '/CRMService/Factory/Info',
        query:{id}
    })
}

// 删除工厂
export function delFactory(id) {
    return request.get({
        url: '/CRMService/Factory/Remove',
        query:{id}
    });
}

// 厂家配置专用
export function setFactoryConfig(data) {
    return request.post({
        url: '/CRMService/Factory/Set',
        data: data
    })
}

// 生成厂家邀请码
export function factoryInvite(data) {
    return request.post({
        url: '/CRMService/Agent/AddAllInvite',
        data: data
    })
}
// 厂家获取代理商列表
export function factorygetAgent(data) {
    return request.get({
        url: '/CRMService/Agent/AllList',
        query: data
    })
}

// 生成厂家邀请码
export function joinInvite(data) {
    return request.post({
        url: '/CRMService/Agent/JoinInvite',
        data: data
    })
}

// 取消代理
export function CancelProxy(query) {
    return request.get({
        url: '/CRMService/Agent/CancelProxy',
        query: query
    })
}
// 获取代理信息
export function agentInfo(query) {
    return request.get({
        url: '/CRMService/Agent/Info',
        query: query
    })
}
// 获取代理信息
export function orgInfo(query) {
    return request.get({
        url: '/AuthService/Org/Info',
        query: query
    })
}

// 获取私海客户
export function privateCustomer(params) {
    return request({
        url: 'CRMService/Customer/List',
        method: 'get',
        params: params
    })
}