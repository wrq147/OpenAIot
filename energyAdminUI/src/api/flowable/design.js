import request from '@/utils/request'
import download from '@/plugins/download'

//生成唯一Id
export function generateId() {
    return request({
        url: '/FlowService/Flow/GenerateId',
        method: 'get'
    })
}

// 查询流程分组
export function getFormGroups(param) {
    return request({
        url: '/FlowService/Group/List',
        method: 'get',
        params: param
    })
}

// 流程分组排序
export function groupItemsSort(param) {
    return request({
        url: '/FlowService/Group/Sort',
        method: 'post',
        data: param
    })
}
export function delGroup(param) {
    return request({
        url: '/FlowService/Group/Remove',
        method: 'get',
        params: param
    })
}
// 更新流程分组
export function updateGroup(data) {
    return request({
        url: '/FlowService/Group/Edit',
        method: "post",
        data: data
    })
}
//添加流程分组
export function addGroup(data) {
    return request({
        url: '/FlowService/Group/Add',
        method: "post",
        data: data
    });
}

// 查询表单详情
export function getFormDetail(id) {
    return request({
        url: '/FlowService/Flow/Info/' + id,
        method: 'get'
    })
}

// 获取可变动数据列表
export function getDataList() {
    return request({
        url: '/FlowService/Flow/DataList',
        method: 'get'
    })
}

// 新增或修改流程信息
export function updateForm(param) {
    return request({
        url: '/FlowService/Flow/SaveFlowDetail',
        method: 'post',
        data: param
    })
}

export function delForm(id) {
    return request({
        url: '/FlowService/Flow/Remove/' + id,
        method: 'get'
    })
}
export function getRecord(param) { //获取流程记录
    return request({
        url: '/FlowService/Flow/GetFlowRecord',
        method: 'post',
        data: param
    })
}

export function exportRecord(query) { //获取流程记录
    // 导出用户
    return download.resource('/FlowService/Flow/ExportRecord', query);
}
export default {
    getFormGroups,
    groupItemsSort,
    getFormDetail,
    updateGroup,
    delGroup,
    addGroup,
    updateForm,
    delForm,
    getRecord,
    exportRecord
}