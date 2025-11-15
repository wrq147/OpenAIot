import request from '@/utils/request'
// 类型列表
export function classPageList(data) {
    return request({
        url: '/EfficiencyService/Common/ClassPageList',
        method: 'post',
        data: data
    })
}

// 类型列表详情
export function classInfo(data) {
    return request({
        url: '/EfficiencyService/Common/ClassInfo',
        method: 'post',
        data: data
    })
}

// 类型新增
export function classAdd(data) {
    return request({
        url: '/EfficiencyService/Common/AddClass',
        method: 'post',
        data: data
    })
}

// 类型编辑
export function classEdit(data) {
    return request({
        url: '/EfficiencyService/Common/UpdateClass',
        method: 'post',
        data: data
    })
}

// 类型删除
export function classRemove(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveClass',
        method: 'post',
        data: data
    })
}

// 获取企业碳核算信息
export function orgClassInfo(data) {
    return request({
        url: '/EfficiencyService/Common/OrgClassInfo',
        method: 'post',
        data: data
    })
}

// 删除企业碳核算信息
export function removeOrgClass(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveOrgClass',
        method: 'post',
        data: data
    })
}

// 新增企业碳核算信息
export function addOrgClass(data) {
    return request({
        url: '/EfficiencyService/Common/AddOrgClass',
        method: 'post',
        data: data
    })
}

// 编辑企业碳核算信息
export function updateOrgClass(data) {
    return request({
        url: '/EfficiencyService/Common/UpdateOrgClass',
        method: 'post',
        data: data
    })
}

// 按排放类型能碳分析报表
export function carbonReportByType(data) {
    return request({
        url: '/EfficiencyService/Production/CarbonReportByType',
        method: 'post',
        data: data
    })
}

// 按排放范围能碳分析报表
export function carbonReportByRange(data) {
    return request({
        url: '/EfficiencyService/Production/CarbonReportByRange',
        method: 'post',
        data: data
    })
}

// 查询碳排计划
export function selectCarbonPlan(data) {
    return request({
        url: '/EfficiencyService/Production/SelectCarbonPlan',
        method: 'post',
        data: data
    })
}

// 更新碳排计划
export function updateCarbonPlan(data) {
    return request({
        url: '/EfficiencyService/Production/UpdateCarbonPlan',
        method: 'post',
        data: data
    })
}

// 查询碳资产
export function selectCarbonAsset(data) {
    return request({
        url: '/EfficiencyService/Production/SelectCarbonAsset',
        method: 'post',
        data: data
    })
}


