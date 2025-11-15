import request from '@/utils/request'
// 生成供应商编码
export function factorySupplierNumber() {
    return request({
        url: '/ProducerService/Supplier/GenerateNumber',
        method: 'get',
    })
}
// 供应商列表
export function factorySupplierListGet(params) {
    return request({
        url: '/ProducerService/Supplier/List',
        method: 'get',
        params: params
    })
}
// 供应商列表post
export function factorySupplierListPost(data) {
    return request({
        url: '/ProducerService/Supplier/List',
        method: 'post',
        data: data
    })
}
// 删除供应商
export function factorySupplierRemove(params) {
    return request({
        url: '/ProducerService/Supplier/Remove',
        method: 'get',
        params: params
    })
}
// 供应商详情
export function factorySupplierInfo(params) {
    return request({
        url: '/ProducerService/Supplier/Info',
        method: 'get',
        params: params
    })
}
// 供应商添加
export function addSupplierSave(data) {
    return request({
        url: '/ProducerService/Supplier/Add',
        method: 'post',
        data: data
    })
}
// 供应商编辑
export function editSupplierSave(data) {
    return request({
        url: '/ProducerService/Supplier/Edit',
        method: 'post',
        data: data
    })
}