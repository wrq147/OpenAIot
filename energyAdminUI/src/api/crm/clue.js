import request from '@/utils/request'

// 获取私海线索
export function privateClue(params) {
    return request({
        url: 'CRMService/Clue/PriList',
        method: 'get',
        params: params
    })
}
// 获取公海线索
export function publicClue(params) {
    return request({
        url: 'CRMService/Clue/PubList',
        method: 'get',
        params: params
    })
}
// 获取线索详情
export function clueInfo(params) {
    return request({
        url: '/CRMService/Clue/Info',
        method: 'get',
        params: params
    })
}
// 领取线索
export function clueDraw(params) {
    return request({
        url: '/CRMService/Clue/Draw',
        method: 'get',
        params: params
    })
}
// 退回线索
export function clueReturn(params) {
    return request({
        url: '/CRMService/Clue/Return',
        method: 'get',
        params: params
    })
}
// 删除私海线索
export function privateClueDel(params) {
    return request({
        url: '/CRMService/Clue/Remove',
        method: 'get',
        params: params
    })
}
// 删除公海线索
export function publicClueDel(params) {
    return request({
        url: '/CRMService/Clue/PubRemove',
        method: 'get',
        params: params
    })
}
// 添加公海线索
export function pubAddClue(data) {
    return request({
        url: '/CRMService/Clue/PubAdd',
        method: 'post',
        data: data
    })
}
// 添加私海线索
export function priAddClue(data) {
    return request({
        url: '/CRMService/Clue/Add',
        method: 'post',
        data: data
    })
}

//修改公海线索
export function pubEditClue(data) {
    return request({
        url: '/CRMService/Clue/PubEdit',
        method: 'post',
        data: data
    })
}
// 修改私海线索
export function priEditClue(data) {
    return request({
        url: '/CRMService/Clue/Edit',
        method: 'post',
        data: data
    })
}
// 转换线索为客户
export function priTransformClue(data) {
    return request({
        url: '/CRMService/Clue/Transform',
        method: 'post',
        data: data
    })
}