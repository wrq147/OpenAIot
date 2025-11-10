import request from '@/common/request.js'
// 生成单位编码
export function factoryUnitNumber() {
    return request.get({
        url: '/ProducerService/Unit/GenerateNumber',
    })
}
// 单位列表
export function factoryUnitListGet(query) {
    return request.get({
        url: '/ProducerService/Unit/List',
        query: query
    })
}
// 单位详情
export function factoryUnitInfo(query) {
    return request.get({
        url: '/ProducerService/Unit/Info',
        query: query
    })
}
// 删除单位
export function factoryUnitRemove(query) {
    return request.get({
        url: '/ProducerService/Unit/Remove',
        query: query
    })
}
// 单位添加
export function addUnitSave(data) {
    return request.post({
        url: '/ProducerService/Unit/Add',
        data: data
    })
}
// 单位编辑
export function editUnitSave(data) {
    return request.post({
        url: '/ProducerService/Unit/Edit',
        data: data
    })
}