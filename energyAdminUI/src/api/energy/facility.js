import request from '@/utils/request'
// 添加设施或设施分组
export function addFacility(data) {
    return request({
        url: '/EfficiencyService/Common/AddFacility',
        method: 'post',
        data: data
    })
}
// 修改设施或设施分组
export function updateFacility(data) {
    return request({
        url: '/EfficiencyService/Common/UpdateFacility',
        method: 'post',
        data: data
    })
}
// 删除设施或设施分组
export function removeFacility(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveFacility',
        method: 'post',
        data: data
    })
}
// 设施或设施分组树形列表
export function facilityTree(data) {
    return request({
        url: '/EfficiencyService/Common/FacilityTree',
        method: 'post',
        data: data
    })
}
// 设施或设施分组详情
export function facilityInfo(data) {
    return request({
        url: '/EfficiencyService/Common/FacilityInfo',
        method: 'post',
        data: data
    })
}
// 设施绑定设备
export function facilityBindEquipment(data) {
    return request({
        url: '/EfficiencyService/Common/FacilityBindEquipment',
        method: 'post',
        data: data
    })
}
// 设施解绑设备
export function removeFacilityBindEquipment(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveFacilityBindEquipment',
        method: 'post',
        data: data
    })
}
// 设施编码生成
export function facilityCode(data) {
    return request({
        url: '/EfficiencyService/Common/FacilityCode',
        method: 'post',
        data: data
    })
}