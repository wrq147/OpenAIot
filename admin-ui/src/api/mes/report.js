import request from '@/utils/request'

// 获取生产批次列表
export function WorkBatchList(params) {
    return request({
        url: '/MESService/Batch/List',
        method: 'get',
        params: params
    })
}

// 获取生产工单列表
export function mesOrderList(params) {
    return request({
        url: '/MESService/Order/List',
        method: 'get',
        params: params
    })
}

//获取生产工单信息
export function mesOrderInfo(params) {
    return request({
        url: '/MESService/Order/Info',
        method: 'get',
        params: params
    })
}

// 获取报工列表
export function ReportList(params) {
    return request({
        url: '/MESService/Report/List',
        method: 'get',
        params: params
    })
}
// 获取报工编号
export function GeneratePlaneNumber() {
    return request({
        url: '/MESService/Report/GeneratePlaneNumber',
        method: 'get',
    })
}
// 增加报工单
export function reportAdd(data) {
    return request({
        url: '/MESService/Report/Add',
        method: 'post',
        data: data
    })
}
// 编辑报工单
export function reportEdit(data) {
    return request({
        url: '/MESService/Report/Edit',
        method: 'post',
        data: data
    })
}
// 报工单提交（同时提交审核表单）
export function reportSubmitModel(data) {
    return request({
        url: '/MESService/Report/SubmitModel',
        method: 'post',
        data: data
    })
}
// 删除报工单（待提交和已取消状态使用）
export function reportRemove(id) {
    return request({
        url: '/MESService/Report/Remove?id=' + id,
        method: 'get',
    })
}
// 删除报工单（待提交和已取消状态使用）取消生产报工
export function reportCancel(id) {
    return request({
        url: '/MESService/Report/Cancel?id=' + id,
        method: 'get',
    })
}
// 生成报工的审核表单初始化
export function reportFormData(data) {
    return request({
        url: '/MESService/Report/FormData',
        method: 'post',
        data: data
    })
}

export function reportInfo(params) {
    return request({
        url: '/MESService/Report/Info',
        method: 'get',
        params: params
    })
}