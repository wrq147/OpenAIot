import request from '@/utils/request'
// 生成单位编码
export function factoryUnitNumber() {
    return request({
        url: '/ProducerService/Unit/GenerateNumber',
        method: 'get',
    })
}
// 单位列表
export function factoryUnitListGet(params) {
    return request({
        url: '/ProducerService/Unit/List',
        method: 'get',
        params: params
    })
}
// 单位详情
export function factoryUnitInfo(params) {
    return request({
        url: '/ProducerService/Unit/Info',
        method: 'get',
        params: params
    })
}
// 删除单位
export function factoryUnitRemove(params) {
    return request({
        url: '/ProducerService/Unit/Remove',
        method: 'get',
        params: params
    })
}
// 单位添加
export function addUnitSave(data) {
    return request({
        url: '/ProducerService/Unit/Add',
        method: 'post',
        data: data
    })
}
// 单位编辑
export function editUnitSave(data) {
    return request({
        url: '/ProducerService/Unit/Edit',
        method: 'post',
        data: data
    })
}