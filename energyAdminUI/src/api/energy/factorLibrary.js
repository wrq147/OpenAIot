import request from '@/utils/request'
// 碳因子类型列表
export function selectFactorTypeTree(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectFactorTypeTree',
        method: 'post',
        data: data
    })
}
// 碳因子类型新增
export function addFactorType(data) {
    return request({
        url: '/EfficiencyService/Factor/AddFactorType',
        method: 'post',
        data: data
    })
}
// 碳因子类型编辑
export function editFactorType(data) {
    return request({
        url: '/EfficiencyService/Factor/EditFactorType',
        method: 'post',
        data: data
    })
}

// 碳因子类型删除
export function removeFactorType(data) {
    return request({
        url: '/EfficiencyService/Factor/RemoveFactorType',
        method: 'post',
        data: data
    })
}

// 碳因子年限版本
export function selectFactorYear(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectFactorYear',
        method: 'post',
        data: data
    })
}

// 碳因子列表
export function selectFactorList(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectFactorList',
        method: 'post',
        data: data
    })
}

// 碳因子新增
export function addFactor(data) {
    return request({
        url: '/EfficiencyService/Factor/AddFactor',
        method: 'post',
        data: data
    })
}

// 碳因子编辑
export function editFactor(data) {
    return request({
        url: '/EfficiencyService/Factor/EditFactor',
        method: 'post',
        data: data
    })
}

// 碳因子列表
export function removeFactor(data) {
    return request({
        url: '/EfficiencyService/Factor/RemoveFactor',
        method: 'post',
        data: data
    })
}
// 添加企业排放因子
export function saveFactorOrg(data) {
    return request({
        url: '/EfficiencyService/Factor/SaveFactorOrg',
        method: 'post',
        data: data
    })
}
//企业已选排放因子
export function selectFactorOrg(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectFactorOrg',
        method: 'post',
        data: data
    })
}
//获取企业所有的
export function SelectOrgAllFactorList(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectOrgFactorList',
        method: 'post',
        data: data
    })
}
//获取企业排放因子根据能源类型
export function selectFactorOrgByTypeId(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectFactorOrgByTypeId',
        method: 'post',
        data: data
    })
}
//获取企业能源类型
export function selectFactorTypeOrg(data) {
    return request({
        url: '/EfficiencyService/Factor/SelectFactorTypeOrg',
        method: 'post',
        data: data
    })
}