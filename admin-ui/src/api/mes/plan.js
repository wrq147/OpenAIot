import request from '@/utils/request'

// 生产计划列表
export function planList(params) {
    return request({
        url: '/MESService/Plan/List',
        method: 'get',
        params: params
    })
}

// 添加生产计划
export function operAdd(data) {
    return request({
        url: '/MESService/Plan/Add',
        method: 'post',
        data: data
    })
}

// 修改生产计划
export function operEdit(data) {
    return request({
        url: '/MESService/Plan/Edit',
        method: 'post',
        data: data
    })
}

// 删除生产计划
export function operRemove(params) {
    return request({
        url: '/MESService/Plan/Remove',
        method: 'get',
        params: params
    })
}

// 生产计划详情
export function operInfo(params) {
    return request({
        url: '/MESService/Plan/Info',
        method: 'get',
        params: params
    })
}

// 生成生产计划编号
export function planeNumber(params) {
    return request({
        url: '/MESService/Plan/GeneratePlaneNumber',
        method: 'get',
        params: params
    })
}

// 生产计划审核表单初始化
export function planeFormData(data) {
    return request({
        url: '/MESService/Plan/FormData',
        method: 'post',
        data: data
    })
}

// 提交生产计划
export function planeSubmitModel(data) {
    return request({
        url: '/MESService/Plan/SubmitModel',
        method: 'post',
        data: data
    })
}

// 取消生产计划
export function planeCancel(params) {
    return request({
        url: '/MESService/Plan/Cancel',
        method: 'get',
        params: params
    })
}