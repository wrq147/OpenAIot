import request from '@/utils/request'
// 设备列表
export function equipmentPageList(data) {
    return request({
        url: '/EfficiencyService/Common/EquipmentPageList',
        method: 'post',
        data: data
    })
}
//添加设备
export function addEquipment(data) {
    return request({
        url: '/EfficiencyService/Common/AddEquipment',
        method: 'post',
        data: data
    })
}
//修改设备
export function updateEquipment(data) {
    return request({
        url: '/EfficiencyService/Common/UpdateEquipment',
        method: 'post',
        data: data
    })
}
//删除设备
export function removeEquipment(data) {
    return request({
        url: '/EfficiencyService/Common/RemoveEquipment',
        method: 'post',
        data: data
    })
}
//设备详情
export function equipmentInfo(data) {
    return request({
        url: '/EfficiencyService/Common/EquipmentInfo',
        method: 'post',
        data: data
    })
}
// 设备编码生成
export function equipmentCode(data) {
    return request({
        url: '/EfficiencyService/Common/EquipmentCode',
        method: 'post',
        data: data
    })
}