import request from '@/common/request.js'

// 库存列表
export function stockList(query) {
    return request.get({
        url: '/StorageService/Stock/List',
        query: query
    })
}

// 获取库存的记录
export function recordList(query) {
    return request.get({
        url: '/StorageService/Stock/RecordList',
        query: query
    })
}



// 手动添加
export function manualPile(data) {
    return request.post({
        url: '/StorageService/Stock/ManualPile',
        data: data
    })
}
export function submitManualPile(data) {
    return request.post({
        url: '/StorageService/Stock/SubmitManualPile',
        data: data
    })
}
//提交入库单（同时提交审核表单）
export function submitManualPileModel(data) {
    return request({
        url: '/StorageService/Stock/SubmitManualPileModel',
        method: 'post',
        data: data
    })
}
// 出库提交（同时提交审核表单）
export function leaveSubmitModel(data) {
    return request({
        url: '/StorageService/Stock/SubmitModel',
        method: 'post',
        data: data
    })
}
// 撤销入库单（同时提交审核表单）
export function cancelEnterModel(data) {
    return request.post({
        url: '/StorageService/Stock/CancelEnterModel',
        data: data
    })
}

// 撤销入库单
export function cancelEnter(data) {
    return request.post({
        url: '/StorageService/Stock/CancelEnter',
        data: data
    })
}

// 撤销出库单
export function cancelLeave(data) {
    return request.post({
        url: '/StorageService/Stock/CancelLeave',
        data: data
    })
}


// 入库列表
export function enterList(query) {
    return request.get({
        url: '/StorageService/Stock/EnterList',
        query: query
    })
}

// 入库设备查询列表
export function enterDevList(query) {
    return request.get({
        url: '/StorageService/Stock/EnterDevList',
        query: query
    })
}

// 出库列表
export function leaveList(query) {
    return request.get({
        url: '/StorageService/Stock/LeaveList',
        query: query
    })
}
// 增加出库
export function leaveAdd(data) {
    return request.post({
        url: '/StorageService/Stock/AddLeave',
        data: data
    })
}
// 编辑出库
export function leaveEdit(data) {
    return request.post({
        url: '/StorageService/Stock/EditLeave',
        data: data
    })
}
// 出库提交
export function leaveSubmit(data) {
    return request.post({
        url: '/StorageService/Stock/Submit',
        data: data
    })
}
// 生成入库单号
export function generateRKNumber() {
    return request({
        url: '/StorageService/Stock/GenerateRKNumber',
        method: 'get'
    })
}

// 生成出库单号
export function generateCKNumber() {
    return request({
        url: '/StorageService/Stock/GenerateCKNumber',
        method: 'get'
    })
}


// 获取入库单详情
export function getEnterInfo(id) {
    return request.get({
        url: '/StorageService/Stock/EnterInfo',
        query: { id }
    })
}
// 入库审核流程初始化
export function enterFormData(data) {
    return request.post({
        url: '/StorageService/Stock/EnterFormData',
        data: data
    })
}
// 出库审核流程初始化
export function leaveFormData(data) {
    return request({
        url: '/StorageService/Stock/LeaveFormData',
        method: 'post',
        data: data
    })
}
// 获取出库单详情
export function getLeaveInfo(id) {
    return request.get({
        url: '/StorageService/Stock/LeaveInfo',
        query: { id }
    })
}

// 查询入库单的关联的出库单
export function getLeaveBySource(id) {
    return request.get({
        url: '/StorageService/Stock/LeaveBySource',
        query: { id }
    })
}

// 手动入库查询扫码信息
export function inStockByKey(id) {
    return request.get({
        url: '/StorageService/Stock/InStockByKey',
        query: { key:id }
    })
}
// 手动出库查询扫码信息
export function OutStockByKey(id) {
    return request.get({
        url: '/StorageService/Stock/OutStockByKey',
        query: { key:id }
    })
}