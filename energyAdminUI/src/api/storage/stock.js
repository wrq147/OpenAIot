import request from '@/utils/request'

import download from '@/plugins/download'
// 库存列表
export function stockList(query) {
    return request({
        url: '/StorageService/Stock/List',
        method: 'get',
        params: query
    })
}

// 获取库存的记录
export function recordList(query) {
    return request({
        url: '/StorageService/Stock/RecordList',
        method: 'get',
        params: query
    })
}



// 手动添加
export function manualPile(data) {
    return request({
        url: '/StorageService/Stock/ManualPile',
        method: 'post',
        data: data
    })
}
//提交入库单
export function submitManualPile(data) {
    return request({
        url: '/StorageService/Stock/SubmitManualPile',
        method: 'post',
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
// 撤销入库单
export function cancelEnter(data) {
    return request({
        url: '/StorageService/Stock/CancelEnter',
        method: 'post',
        data: data
    })
}

// 撤销入库单（同时提交审核表单）
export function cancelEnterModel(data) {
    return request({
        url: '/StorageService/Stock/CancelEnterModel',
        method: 'post',
        data: data
    })
}

// 撤销出库单
export function cancelLeave(data) {
    return request({
        url: '/StorageService/Stock/CancelLeave',
        method: 'post',
        data: data
    })
}


// 导出库存
export function exportExcel(query) {
    return download.resource('/StorageService/Stock/Export', query);
}


// 入库列表
export function enterList(query) {
    return request({
        url: '/StorageService/Stock/EnterList',
        method: 'get',
        params: query
    })
}
// 导出入库单
export function exportEnterExcel(query) {
    return download.resource('/StorageService/Stock/ExportEnter', query);
}

// 入库设备查询列表
export function enterDevList(query) {
    return request({
        url: '/StorageService/Stock/EnterDevList',
        method: 'get',
        params: query
    })
}

// 出库列表
export function leaveList(query) {
    return request({
        url: '/StorageService/Stock/LeaveList',
        method: 'get',
        params: query
    })
}
// 增加出库
export function leaveAdd(data) {
    return request({
        url: '/StorageService/Stock/AddLeave',
        method: 'post',
        data: data
    })
}
// 编辑出库
export function leaveEdit(data) {
    return request({
        url: '/StorageService/Stock/EditLeave',
        method: 'post',
        data: data
    })
}
// 出库提交
export function leaveSubmit(data) {
    return request({
        url: '/StorageService/Stock/Submit',
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
    return request({
        url: '/StorageService/Stock/EnterInfo',
        method: 'get',
        params: { id }
    })
}

// 通过工单编号获取入库单详情
export function getEnterInfoByNumber(number) {
    return request({
        url: '/StorageService/Stock/EnterInfoByNumber',
        method: 'get',
        params: { number }
    })
}
// 通过工单编号获取出库单详情
export function getLeaveInfoByNumber(number) {
    return request({
        url: '/StorageService/Stock/LeaveInfoByNumber',
        method: 'get',
        params: { number }
    })
}
// 获取出库单详情
export function getLeaveInfo(id) {
    return request({
        url: '/StorageService/Stock/LeaveInfo',
        method: 'get',
        params: { id }
    })
}
// 入库审核流程初始化
export function enterFormData(data) {
    return request({
        url: '/StorageService/Stock/EnterFormData',
        method: 'post',
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
// 查询入库单的关联的出库单
export function getLeaveBySource(id) {
    return request({
        url: '/StorageService/Stock/LeaveBySource',
        method: 'get',
        params: { id }
    })
}

// 扫码获取产品信息
export function getInStockByKey(params) {
    return request({
        url: '/StorageService/Stock/InStockByKey',
        method: 'get',
        params
    })
}

//扫码入库
export function scanEnterPile(data) {
    return request({
        url: '/StorageService/Stock/ScanEnterPile',
        method: 'post',
        data: data
    })
}
//设置库存预警
export function setPileWarn(data) {
    return request({
        url: '/StorageService/Stock/SetPileWarn',
        method: 'post',
        data: data
    })
}
