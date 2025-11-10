import request from '@/utils/request'
// 获取出库申请单列表
export function ApplyList(params) {
    return request({
        url: '/StorageService/Apply/List',
        method: 'get',
        params: params
    })
}
// 获取出库申请单详情
export function ApplyInfo(id) {
    return request({
        url: '/StorageService/Apply/Info',
        method: 'get',
        params: {id}
    })
}

// 获取出库申请单详情(通过编码)
export function ApplyInfoByNumber(number) {
    return request({
        url: '/StorageService/Apply/InfoByNumber',
        method: 'get',
        params: {number}
    })
}

// 生成出库申请单号
export function generateCKSQNumber() {
    return request({
        url: '/StorageService/Apply/GenerateNumber',
        method: 'get'
    })
}
// 申请待出库数量
export function applyWaitCount(params) {
    return request({
        url: '/StorageService/Apply/WaitCount',
        method: 'get',
        params:params
    })
}

// 增加出库申请
export function applyAdd(data) {
    return request({
        url: '/StorageService/Apply/Add',
        method: 'post',
        data: data
    })
}
// 编辑出库申请
export function applyEdit(data) {
    return request({
        url: '/StorageService/Apply/Edit',
        method: 'post',
        data: data
    })
}
// 出库申请提交（同时提交审核表单）
export function applySubmitModel(data) {
    return request({
        url: '/StorageService/Apply/SubmitModel',
        method: 'post',
        data: data
    })
}
// 出库申请流程初始化
export function applyFormData(data) {
    return request({
        url: '/StorageService/Apply/FormData',
        method: 'post',
        data: data
    })
}
// 取消出库申请单详情
export function cancelApply(params) {
    return request({
        url: '/StorageService/Apply/Cancel',
        method: 'get',
        params: params
    })
}
// 删除出库申请单详情
export function deleteApply(params) {
    return request({
        url: '/StorageService/Apply/Remove',
        method: 'get',
        params: params
    })
}