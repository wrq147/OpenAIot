import request from '@/common/request.js'
// 获取出库申请单列表
export function ApplyList(query) {
    return request.get({
        url: '/StorageService/Apply/List',
        query: query
    })
}
// 获取出库申请单详情
export function ApplyInfo(id) {
    return request.get({
        url: '/StorageService/Apply/Info',
        query: {id}
    })
}

// 获取出库申请单详情(通过编码)
export function ApplyInfoByNumber(number) {
    return request.get({
        url: '/StorageService/Apply/InfoByNumber',
        query: {number}
    })
}

// 生成出库申请单号
export function generateCKSQNumber() {
    return request.get({
        url: '/StorageService/Apply/GenerateNumber',
    })
}
// 申请待出库数量
export function applyWaitCount(query) {
    return request.get({
        url: '/StorageService/Apply/WaitCount',
        query:query
    })
}

// 增加出库申请
export function applyAdd(data) {
    return request.post({
        url: '/StorageService/Apply/Add',
        data: data
    })
}
// 编辑出库申请
export function applyEdit(data) {
    return request.post({
        url: '/StorageService/Apply/Edit',
        data: data
    })
}
// 出库申请提交（同时提交审核表单）
export function applySubmitModel(data) {
    return request.post({
        url: '/StorageService/Apply/SubmitModel',
        data: data
    })
}
// 出库申请流程初始化
export function applyFormData(data) {
    return request.post({
        url: '/StorageService/Apply/FormData',
        data: data
    })
}
// 取消出库申请单详情
export function cancelApply(query) {
    return request.get({
        url: '/StorageService/Apply/Cancel',
        query: query
    })
}
// 删除出库申请单详情
export function deleteApply(query) {
    return request.get({
        url: '/StorageService/Apply/Remove',
        query: query
    })
}